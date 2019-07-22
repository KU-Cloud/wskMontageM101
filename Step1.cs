using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace M101
{

    public class Step1
    {
        private const string _ACTIV_NAME = "Step1";
        private const string _EXE_NAME = "mImgtbl";
        private static string _EXE_PATH = Path.Combine(Montage.BIN_PATH, _EXE_NAME);

        public JObject Main(JObject args)
        {
            Action<string> log = (string msg) => Montage.Log(args, $"{_ACTIV_NAME} {msg}");
            log("Start");

            Montage.ServerURL = args.GetValue("serverURL").ToString();

            Directory.CreateDirectory(Montage.RAW_DIR);

            log("Download");
            {
                var files = new List<KeyValuePair<string, string>>(Montage.RAWS.Length);
                foreach (var raw in Montage.RAWS)
                {
                    files.Add(KeyValuePair.Create(raw, Path.Combine(Montage.RAW_DIR, raw)));
                }

                Montage.Download(files.ToArray()).Wait();
            }


            log("Run " + _EXE_NAME);
            using (var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _EXE_PATH,
                    Arguments = $"{Montage.RAW_DIR} {Montage.RAW_TBL}",
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
                Montage.Upload(Montage.RAW_TBL).Wait();
            }

            log("Finish");
            return args;
        }
    }
}
