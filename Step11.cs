using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace M101
{
    public class Step11
    {
        private const string _ACTIV_NAME = "Step11";
        private const string _EXE_NAME = "mAdd";
        private static string _EXE_PATH = Path.Combine(Montage.BIN_PATH, _EXE_NAME);

        public JObject Main(JObject args)
        {
            Action<string> log = (string msg) => Montage.Log(args, $"{_ACTIV_NAME} {msg}");
            log("Start");

            Montage.ServerURL = args.GetValue("serverURL").ToString();

            Directory.CreateDirectory(Montage.CORR_DIR);

            log("Download");
            {
                var projected = args.GetValue("projected") as JArray;
                var files = new List<KeyValuePair<string, string>>(Montage.RAWS.Length + 2);
                files.Add(KeyValuePair.Create(Montage.IMG_TBL, Montage.IMG_TBL));
                files.Add(KeyValuePair.Create(Montage.TEMPLATE, Montage.TEMPLATE));
                foreach (var proj in projected)
                {
                    var trg = proj.ToString();
                    files.Add(KeyValuePair.Create(trg, Path.Combine(Montage.CORR_DIR, trg)));
                    files.Add(KeyValuePair.Create(Montage.ToArea(trg), Path.Combine(Montage.CORR_DIR, Montage.ToArea(trg))));
                }

                Montage.Download(files.ToArray()).Wait();
            }


            log("Run " + _EXE_NAME);
            using (var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _EXE_PATH,
                    Arguments = $"-p {Montage.CORR_DIR} {Montage.IMG_TBL} {Montage.TEMPLATE} {Montage.LAST_FITS}",
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
                Montage.Upload(Montage.LAST_FITS).Wait();
            }

            log("Finish");
            return args;
        }
    }
}
