using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace M101
{
    public class Step7
    {
        private const string _ACTIV_NAME = "Step7";
        private const string _EXE_NAME = "mDiff";
        private static string _EXE_PATH = Path.Combine(Montage.BIN_PATH, _EXE_NAME);

        public JObject Main(JObject args)
        {
            Action<string> log = (string msg) => Montage.Log(args, $"{_ACTIV_NAME} {msg}");
            try
            {

                log("Start");

                Montage.ServerURL = args.GetValue("serverURL").ToString();

                var task = args.GetValue("task") as JObject;
                var src1 = task.GetValue("src1").ToString();
                var src2 = task.GetValue("src2").ToString();
                var dst = task.GetValue("dst").ToString();

                log("Download");
                {
                    Montage.Download(src1, src2, Montage.ToArea(src1), Montage.ToArea(src2), Montage.TEMPLATE).Wait();
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
                    proc.StartInfo.Arguments = $"{src1} {src2} {dst} {Montage.TEMPLATE}";
                    proc.Start();
                    proc.WaitForExit();
                    log(proc.StandardOutput.ReadToEnd());
                }

                log("Upload");
                {
                    Montage.Upload(dst, Montage.ToArea(dst)).Wait();
                }

                log("Finish");
            }
            catch (Exception e)
            {
                log(e.ToString());
            }
            return args;
        }
    }
}
