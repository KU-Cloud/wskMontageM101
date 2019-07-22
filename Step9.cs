using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace M101
{
    public class Step9
    {
        private const string _ACTIV_NAME = "Step9";
        private const string _EXE_NAME = "mBgModel";
        private static string _EXE_PATH = Path.Combine(Montage.BIN_PATH, _EXE_NAME);

        public JObject Main(JObject args)
        {
            Action<string> log = (string msg) => Montage.Log(args, $"{_ACTIV_NAME} {msg}");
            log("Start");

            Montage.ServerURL = args.GetValue("serverURL").ToString();

            log("Download");
            {
                Montage.Download(Montage.IMG_TBL, Montage.FITS_TBL);
            }


            log("Run " + _EXE_NAME);
            using (var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _EXE_PATH,
                    Arguments = $"{Montage.IMG_TBL} {Montage.FITS_TBL} {Montage.CORR_TBL}",
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
                Montage.Upload(Montage.CORR_TBL).Wait();
            }

            log("Finish");
            return args;
        }
    }
}
