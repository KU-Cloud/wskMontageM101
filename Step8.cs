using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace M101
{
    public class Step8
    {
        private const string _ACTIV_NAME = "Step8";
        private const string _EXE_NAME = "mFitExec";
        private static string _EXE_PATH = Path.Combine(Montage.BIN_PATH, _EXE_NAME);

        public JObject Main(JObject args)
        {
            Action<string> log = (string msg) => Montage.Log(args, $"{_ACTIV_NAME} {msg}");
            log("Start");

            Montage.ServerURL = args.GetValue("serverURL").ToString();

            Directory.CreateDirectory(Montage.DIFF_DIR);

            log("Download");
            {
                var diffs = args.GetValue("diffs") as JArray;
                var files = new List<KeyValuePair<string, string>>(diffs.Count);
                files.Add(KeyValuePair.Create(Montage.DIFF_TBL, Montage.DIFF_TBL));
                foreach (var diff in diffs)
                {
                    files.Add(KeyValuePair.Create(diff.ToString(), Path.Combine(Montage.DIFF_DIR, diff.ToString())));
                }

                Montage.Download(files.ToArray()).Wait();
            }


            log("Run " + _EXE_NAME);
            using (var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _EXE_PATH,
                    Arguments = $"{Montage.DIFF_TBL} {Montage.FITS_TBL} {Montage.DIFF_DIR}",
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
                Montage.Upload(Montage.FITS_TBL).Wait();
            }

            log("Finish");
            return args;
        }
    }
}
