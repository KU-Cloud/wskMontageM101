using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace M101
{
    public class Step10
    {
        private const string _ACTIV_NAME = "Step10";
        private const string _EXE_NAME = "mBgExec";
        private static string _EXE_PATH = Path.Combine(Montage.BIN_PATH, _EXE_NAME);

        public JObject Main(JObject args)
        {
            Action<string> log = (string msg) => Montage.Log(args, $"{_ACTIV_NAME} {msg}");
            log("Start");

            Montage.ServerURL = args.GetValue("serverURL").ToString();

            Directory.CreateDirectory(Montage.PROJ_DIR);
            Directory.CreateDirectory(Montage.CORR_DIR);

            log("Download");
            {
                var projected = args.GetValue("projected") as JArray;
                var files = new List<KeyValuePair<string, string>>(Montage.RAWS.Length + 2);
                files.Add(KeyValuePair.Create(Montage.IMG_TBL, Montage.IMG_TBL));
                files.Add(KeyValuePair.Create(Montage.CORR_TBL, Montage.CORR_TBL));
                foreach (var proj in projected)
                {
                    var trg = proj.ToString();
                    files.Add(KeyValuePair.Create(trg, Path.Combine(Montage.PROJ_DIR, trg)));
                    files.Add(KeyValuePair.Create(Montage.ToArea(trg), Path.Combine(Montage.PROJ_DIR, Montage.ToArea(trg))));
                }

                Montage.Download(files.ToArray()).Wait();
            }


            log("Run " + _EXE_NAME);
            using (var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _EXE_PATH,
                    Arguments = $"-p {Montage.PROJ_DIR} {Montage.IMG_TBL} {Montage.CORR_TBL} {Montage.CORR_DIR}",
                    RedirectStandardOutput = true
                }
            })
            {
                proc.Start();
                proc.WaitForExit();
                log(proc.StandardOutput.ReadToEnd());
            }

            log("Upload");
            {
                var files = Directory.GetFiles(Montage.CORR_DIR);
                Montage.Upload(files).Wait();
            }

            log("Finish");
            return args;
        }
    }
}
