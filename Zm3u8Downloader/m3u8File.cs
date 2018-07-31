using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zm3u8Downloader
{
    public class m3u8File
    {
        public FileInfo file;
        public string name;
        public string path;
        public int PartNo = -1;
        public long totalTime = -1;
        public string keyPath=null;
        public List<String> DownloadUrl = new List<string>();

        public m3u8File(String str)
        {
            file = new FileInfo(str);
            name = file.Name.Replace(file.Extension,"") ;
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
                    if (PartNo < 0) PartNo = 0;
                    PartNo++;
                    if (totalTime < 0) totalTime = 0;
                    totalTime+= Convert.ToInt32(m1.Result("$1"));
                    urlFlag = true;
                }else if (urlFlag)
                {
                    urlFlag = false;
                    DownloadUrl.Add(nextLine);
                    //Console.WriteLine(DownloadUrl.Count+","+nextLine);
                }
            }
            sr.Close();
        }
    }
}
