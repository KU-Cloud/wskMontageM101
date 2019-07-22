using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace M101
{
    public class Step4
    {
        private const string _ACTIV_NAME = "Step4";
        private const string _EXE_NAME = "mImgtbl";
        private static string _EXE_PATH = Path.Combine(Montage.BIN_PATH, _EXE_NAME);

        public JObject Main(JObject args)
        {
            Action<string> log = (string msg) => Montage.Log(args, $"{_ACTIV_NAME} {msg}");
            log("Start");

            Montage.ServerURL = args.GetValue("serverURL").ToString();

            Directory.CreateDirectory(Montage.PROJ_DIR);

            log("Download");
            {
                var projected = args.GetValue("projected") as JArray;
                var files = new List<KeyValuePair<string, string>>(Montage.RAWS.Length);
                foreach (var fit in projected)
                {
                    files.Add(KeyValuePair.Create(fit.ToString(), Path.Combine(Montage.PROJ_DIR, fit.ToString())));
                }

                Montage.Download(files.ToArray()).Wait();
            }


            log("Run " + _EXE_NAME);
            using (var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _EXE_PATH,
                    Arguments = $"{Montage.PROJ_DIR} {Montage.IMG_TBL}",
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
                Montage.Upload(Montage.IMG_TBL).Wait();
            }

            log("Finish");
            return args;
        }
    }
}
