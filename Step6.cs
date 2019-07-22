using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace M101
{
    public class Step6
    {
        static private string _ACTIV_NAME = "Step6";

        public JObject Main(JObject args)
        {
            Action<string> log = (string msg) => Montage.Log(args, $"{_ACTIV_NAME} {msg}");
            log("Start");

            Montage.ServerURL = args.GetValue("serverURL").ToString();

            log("Downlaod");
            {
                Montage.Download(Montage.DIFF_TBL).Wait();
            }

            log("Process " + Montage.DIFF_TBL);
            using (var sr = new StreamReader(Montage.DIFF_TBL))
            {
                var diffTasks = new JArray();
                var fields = sr.ReadLine();

                // data type
                sr.ReadLine();

                string f1, f2, rst;
                int f1Start = -1, f2Start = -1, rstStart = -1;
                int count = 0;

                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null) break;

                    (f1, f1Start) = Montage.ReadFromTbl(fields, line, "plus", f1Start);
                    (f2, f2Start) = Montage.ReadFromTbl(fields, line, "minus", f2Start);
                    (rst, rstStart) = Montage.ReadFromTbl(fields, line, "diff", rstStart);

                    var dst = rst;

                    var diffTask = new JObject();
                    diffTask.Add("src1", f1);
                    diffTask.Add("src2", f2);
                    diffTask.Add("dst", dst);
                    diffTasks.Add(diffTask);

                    count++;
                }

                args.Add("diffTasks", diffTasks);
            }

            log("Finish");
            return args;
        }
    }
}
