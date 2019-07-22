using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace M101
{
    public class Step3
    {
        private const string _ACTIV_NAME = "Step3";
        private const string _EXE_NAME = "mProjectPP";
        private static string _EXE_PATH = Path.Combine(Montage.BIN_PATH, _EXE_NAME);

        public JObject Main(JObject args)
        {
            Action<string> log = (string msg) => Montage.Log(args, $"{_ACTIV_NAME} {msg}");

            log("Start");

            Montage.ServerURL = args.GetValue("serverURL").ToString();

            var task = args.GetValue("task") as JObject;
            var src = task.GetValue("src").ToString();
            var dst = task.GetValue("dst").ToString();

            log("Download");
            {
                Montage.Download(src, Montage.TEMPLATE
                ).Wait();
            }

            log("Run " + _EXE_NAME);

            using (var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _EXE_PATH,
                    RedirectStandardOutput = true
                }
            })
            {
                proc.StartInfo.Arguments = $"{src} {dst} {Montage.TEMPLATE}";
                proc.Start();
                proc.WaitForExit();
                log(proc.StandardOutput.ReadToEnd());
            }

            log("Upload");
            {
                Montage.Upload(dst, Montage.ToArea(dst)).Wait();
            }

            log("Finish");
            return args;
        }
    }
}
