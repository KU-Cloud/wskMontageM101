using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace M101
{
    public class Step2
    {
        static private string _ACTIV_NAME = "Step2";

        public JObject Main(JObject args)
        {
            Action<string> log = (string msg) => Montage.Log(args, $"{_ACTIV_NAME} {msg}");
            log("Start");

            Montage.ServerURL = args.GetValue("serverURL").ToString();

            log("Downlaod");
            {
                Montage.Download(Montage.RAW_TBL).Wait();
            }

            log("Process " + Montage.RAW_TBL);
            using (var sr = new StreamReader(Montage.RAW_TBL))
            {
                var projTasks = new JArray();

                // header
                sr.ReadLine();

                var fields = sr.ReadLine();

                // data type
                sr.ReadLine();

                string hdu, fname;
                int hduStart = -1, fnameStart = -1;
                int count = 0;

                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null) break;

                    (hdu, hduStart) = Montage.ReadFromTbl(fields, line, "hdu", hduStart);
                    (fname, fnameStart) = Montage.ReadFromTbl(fields, line, "fname", fnameStart);

                    var dst = $"hdu{hdu}_{Path.GetFileName(fname)}";

                    var projTask = new JObject();
                    projTask.Add("src", fname);
                    projTask.Add("dst", dst);
                    projTasks.Add(projTask);

                    count++;
                }

                args.Add("projTasks", projTasks);
            }

            log("Finish");
            return args;
        }
    }
}
