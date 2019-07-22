using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace M101
{
    public static class Montage
    {
        private static int _ID = (new Random()).Next();
        private static HttpClient _HTTP_CLIENT = new HttpClient();
        private const string _STORAGE_NAME = "files";
        private static Uri _SERVER_URL = new Uri("http://163.152.111.104:5122/");
        public const string BIN_PATH = "/home/Montage/bin/";
        public const string TMP_PATH = "/home/";

        public static readonly string[] RAWS = {
            "2mass-atlas-990214n-j1100244.fits",
            "2mass-atlas-990214n-j1100256.fits",
            "2mass-atlas-990214n-j1110021.fits",
            "2mass-atlas-990214n-j1110032.fits",
            "2mass-atlas-990214n-j1180244.fits",
            "2mass-atlas-990214n-j1180256.fits",
            "2mass-atlas-990214n-j1190021.fits",
            "2mass-atlas-990214n-j1190032.fits",
            "2mass-atlas-990214n-j1200244.fits",
            "2mass-atlas-990214n-j1200256.fits",
        };

        public const string RAW_DIR = "rawdir";
        public const string PROJ_DIR = "projdir";
        public const string CORR_DIR = "corrdir";
        public const string DIFF_DIR = "diffdir";

        public const string TEMPLATE = "template.hdr";
        public const string RAW_TBL = "images-rawdir.tbl";
        public const string IMG_TBL = "images.tbl";
        public const string DIFF_TBL = "diffs.tbl";
        public const string FITS_TBL = "fits.tbl";
        public const string CORR_TBL = "corrections.tbl";
        public const string UNCORR_FITS = "uncorrected.fits";
        public const string UNCORR_AREA_FITS = "uncorrected_area.fits";
        public const string LAST_FITS = "m101_mosaic.fits";

        public static string ServerURL
        {
            get
            {
                return _SERVER_URL.ToString();
            }
            set
            {
                _SERVER_URL = new Uri(value);
            }
        }

        public static Uri StorageURL
        {
            get
            {
                return new Uri(_SERVER_URL, _STORAGE_NAME);
            }
        }

        public static async Task Upload(params string[] filenames)
        {
            var streams = new FileStream[filenames.Length];

            using (var content = new MultipartFormDataContent())
            {
                for (var i = 0; i < filenames.Length; i++)
                {
                    var filename = filenames[i];
                    var stream = streams[i] = new FileStream(filename, FileMode.Open);
                    content.Add(new StreamContent(stream), Path.GetFileName(filename), Path.GetFileName(filename));
                }

                var res = await _HTTP_CLIENT.PostAsync(StorageURL, content);
                res.EnsureSuccessStatusCode();
            }

            foreach (var stream in streams)
            {
                stream.Dispose();
            }
        }

        public static async Task Download(params KeyValuePair<string, string>[] filenames)
        {
            var clis = new WebClient[filenames.Length];
            var tasks = new Task[filenames.Length];


            for (var i = 0; i < filenames.Length; i++)
            {
                var filename = filenames[i].Key;
                var dest = filenames[i].Value;
                var cli = clis[i] = new WebClient();
                var tcs = new TaskCompletionSource<object>();

                tasks[i] = tcs.Task;

                cli.DownloadFileCompleted += (sender, args) => tcs.SetResult(null);
                cli.DownloadFileAsync(new Uri(_SERVER_URL, Path.Combine(_STORAGE_NAME, filename)), dest);
            }

            await Task.WhenAll(tasks);
        }
        public static Task Download(params string[] filenames)
        {
            var kvs = new KeyValuePair<string, string>[filenames.Length];
            for (var i = 0; i < filenames.Length; i++)
            {
                var filename = filenames[i];
                kvs[i] = KeyValuePair.Create(filename, filename);
            }
            return Download(kvs);
        }

        public static void Log(JObject args, string msg)
        {
            JArray logs;
            if (args.TryGetValue("log", out var tmp))
            {
                logs = tmp as JArray;
            }
            else
            {
                logs = new JArray();
                args.Add("log", logs);
            }

            var log = new JArray();
            log.Add(_ID);
            log.Add(DateTimeOffset.Now.ToUnixTimeMilliseconds());
            log.Add(msg);

            logs.Add(log);
        }

        public static (string, int) ReadFromTbl(string fields, string line, string what, int start = -1)
        {
            if (start < 0)
            {
                var pivot = fields.IndexOf(what);
                if (pivot < 0) return ("", -1);

                for (int i = pivot; i >= 0; i--)
                {
                    if (fields[i] != '|') continue;
                    start = i + 1;
                    break;
                }

                if (start < 0) return ("", -1);
            }

            int end = start;
            if (line[end] == ' ') for (; end < line.Count(); end++)
                {
                    if (line[end] == ' ') continue;
                    break;
                }
            for (; end < line.Count(); end++)
            {
                if (line[end] != ' ') continue;
                break;
            }

            return (line.Substring(start, end - start).Trim(), start);
        }

        public static string ToArea(string fits)
        {
            return Path.GetFileNameWithoutExtension(fits) + "_area.fits";
        }
    }
}
