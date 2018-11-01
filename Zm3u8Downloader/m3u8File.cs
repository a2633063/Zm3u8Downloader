using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Zm3u8Downloader.Aria2Download;

namespace Zm3u8Downloader
{
    public class m3u8File
    {
        public FileInfo file;
        string _name;
        string _path;
        long _totalTime = -1;
        public int stateId = (int)Status.NoStart;
        public string keyPath = null;
        public List<String> DownloadUrl = new List<string>();
        public List<int> DownloadState = new List<int>();



        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string path
        {
            get { return _path; }
            set { _path = value; }
        }

        public string totalTime
        {
            get { return string.Format("{0:D2}:{1:D2}:{2:D2}", _totalTime / 3600, _totalTime / 60 % 60, _totalTime % 60); }
        }

        public int partCount
        {
            get { return DownloadUrl.Count; }

        }
        public String state
        {
            get
            {

                switch ((Status)stateId)
                {
                    case Status.NoStart: return "排队";
                    case Status.Paused: return "暂停";
                    
                    case Status.Failed: return "失败";
                    case Status.Exit: case Status.Finished: return "下载完成";
                    case Status.Running:
                        //return "正在下载";
                    default:
                    
                        return stateId + "/" + DownloadUrl.Count;
                }
            }

        }
        public m3u8File(String str)
        {
            file = new FileInfo(str);
            name = file.Name.Replace(file.Extension, "");
            path = file.DirectoryName;
            getFileInfo();
        }

        void getFileInfo()
        {
            StreamReader sr = file.OpenText();
            string nextLine;
            Regex regPart = new Regex(@"^#EXTINF:(\d*),");
            bool urlFlag = false;
            while ((nextLine = sr.ReadLine()) != null)
            {
                #region 获取KEY地址/链接
                if (keyPath == null)
                {
                    Regex reg = new Regex(@"^#EXT-X-KEY:.*,URI=""(.*?)"".*");
                    Match m = reg.Match(nextLine);
                    if (m.Success)
                    {
                        String keyString = m.Result("$1");

                        if (keyString.StartsWith("http"))
                        {
                            keyPath = keyString;
                        }
                        else
                        {
                            String str = Path.Combine(path, keyString);
                            FileInfo f = new FileInfo(str);
                            keyPath = f.FullName;
                        }

                    }
                }
                #endregion

                Match m1 = regPart.Match(nextLine);
                if (m1.Success)
                {

                    if (_totalTime < 0) _totalTime = 0;
                    _totalTime += Convert.ToInt32(m1.Result("$1"));
                    urlFlag = true;
                }
                else if (urlFlag)
                {
                    urlFlag = false;
                    DownloadUrl.Add(nextLine);

                    //Console.WriteLine(DownloadUrl.Count+","+nextLine);
                }
                DownloadStateClear();
            }
            sr.Close();
        }

        public void DownloadStateClear()
        {
            DownloadState.Clear();
            for (int i = 0; i < DownloadUrl.Count; i++)
            {
                DownloadState.Add((int)Status.NoStart);
            }
        }

    }
}
