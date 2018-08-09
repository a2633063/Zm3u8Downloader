using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Zm3u8Downloader.Aria2Download;
using static Zm3u8Downloader.ffmpegMerge;

namespace Zm3u8Downloader
{
    class m3u8Download
    {
        public delegate void m3u8DownloadCallBack(Aria2Download.Status status,int id, String str);

        public int id;
        public Aria2Download.Status status;
        public m3u8DownloadCallBack callBack;
        public m3u8File m3u8;
        int downloadNo = 0;
        bool automerge = false;

        public m3u8Download(m3u8File m,int id,bool automerge, m3u8DownloadCallBack cb)
        {
            m3u8 = m;
            callBack = cb;
            status = 0;
            this.id = id;
            this.automerge = automerge;
        }


        public void start()
        {
            String filename;
            if (downloadNo == 0)
            {//写入m3u8文件
                String fileContent = File.ReadAllText(m3u8.file.FullName);//读取所有内容
                Regex regFileContent = new Regex(@".*//(.*?.ts).*");
                string result = regFileContent.Replace(fileContent, "$1");
                regFileContent = new Regex(@"URI="".*""");
                 result = regFileContent.Replace(result, @"URI=""key.key""");

                if (!Directory.Exists(@"Downloads\\" + m3u8.name)) Directory.CreateDirectory(@"Downloads\\" + m3u8.name);
                File.WriteAllText(@".\Downloads\" + m3u8.name + "\\" + m3u8.name+".m3u8", result);
            }



            Regex reg = new Regex(@".*/(.*\.ts).*");
            Match m = reg.Match(m3u8.DownloadUrl[downloadNo]);
            if (m.Success)
            {
                filename = m.Result("$1");
            }
            else filename = m3u8.name+"_"+downloadNo + ".ts";

            Aria2DownloadCallBack cb = new Aria2DownloadCallBack(OnDataReceived);
            Aria2Download mydownload = new Aria2Download(m3u8.DownloadUrl[downloadNo], "Downloads\\"+m3u8.name+"\\"+filename, cb);
            mydownload.Start();
            Console.WriteLine("Download start");
        }

        void m3u8Merge()
        {
            if (!automerge) return;
            ffmpegMergeCallBack cb = new ffmpegMergeCallBack(ffmpegMergeCallBackReceived);
            ffmpegMerge ffmpegmerge = new ffmpegMerge(@".\Downloads\" + m3u8.name + "\\" + m3u8.name + ".m3u8", @".\Out\"+ m3u8.name+".mp4", cb);
            ffmpegmerge.Start();
        }
        public void OnDataReceived(Aria2Download.Status status, int progress)
        {

            //            dt.Rows[0][4] = "当前进度: " + progress + "%";
            
                this.status = status;
                switch (status)
                {
                case Aria2Download.Status.Finished:
                    //下载完成
                    downloadNo++;
                    if (downloadNo < m3u8.DownloadUrl.Count)
                    {//下载下一个
                        if (callBack != null) callBack(Aria2Download.Status.Running, id, "下载:" + (downloadNo - 1).ToString() + "/" + m3u8.DownloadUrl.Count + "  " + progress + "%");
                        start();
                    }else if(downloadNo == m3u8.DownloadUrl.Count)
                    {
                        if (m3u8.keyPath.StartsWith("http"))
                        {
                            Aria2DownloadCallBack cb = new Aria2DownloadCallBack(OnDataReceived);
                            Aria2Download mydownload = new Aria2Download(m3u8.keyPath,  "Downloads\\"+ m3u8.name + "\\key.key" , cb);
                            mydownload.Start();
                            Console.WriteLine(m3u8.name+":开始下载key");
                        }else
                        {
                            try
                            {
                                File.Copy(m3u8.keyPath, m3u8.name + "\\key.key", true);

                                if (automerge)
                                {
                                    this.status = Aria2Download.Status.Running;
                                    if (callBack != null) callBack(status, id, "下载完成  " + m3u8.DownloadUrl.Count + "/" + m3u8.DownloadUrl.Count);
                                    m3u8Merge();
                                }
                                else
                                {
                                    this.status = Aria2Download.Status.Finished;
                                    if (callBack != null) callBack(status, id, "下载完成  " + m3u8.DownloadUrl.Count + "/" + m3u8.DownloadUrl.Count);
                                }
                            }
                            catch
                            {
                                this.status = Aria2Download.Status.Failed;
                                if (callBack != null) callBack(status, id, "下载Key失败  " + m3u8.DownloadUrl.Count + "/" + m3u8.DownloadUrl.Count);
                            }
                        }
                    }
                    else if(downloadNo > m3u8.DownloadUrl.Count)
                    {
                        if (automerge)
                        {
                            this.status = Aria2Download.Status.Running;
                            if (callBack != null) callBack(status, id, "下载完成  " + m3u8.DownloadUrl.Count + "/" + m3u8.DownloadUrl.Count);
                            m3u8Merge();
                        }
                        else
                        {
                            this.status = Aria2Download.Status.Finished;
                            if (callBack != null) callBack(status, id, "下载完成  " + m3u8.DownloadUrl.Count + "/" + m3u8.DownloadUrl.Count);
                        }
                    }
                    break;
                    case Aria2Download.Status.Running:
                        if (callBack != null) callBack(status, id, "下载:" + downloadNo + "/" + m3u8.DownloadUrl.Count +"  "+ progress + "%");
                        break;
                    case Aria2Download.Status.Failed:
                        if (callBack != null) callBack(status, id, "失败!" + downloadNo + "/" + m3u8.DownloadUrl.Count);
                        break;
                    case Aria2Download.Status.Paused:
                        if (callBack != null) callBack(status, id, "下载暂停 " + downloadNo + "/" + m3u8.DownloadUrl.Count + "  " + progress + "%");
                        break;
                }
            
            Console.WriteLine("当前进度: " + progress + "%");
        }


        public void ffmpegMergeCallBackReceived(Status status)
        {
            if (status == Status.Failed)
            {
                if (callBack != null) callBack(status, id, "合并失败" + downloadNo + "/" + m3u8.DownloadUrl.Count);

                Console.WriteLine("ffmpegMergeStatus Failed");
            }
            else if (status == Status.Finished)
            {
                if (callBack != null) callBack(status, id, "合并完成" + downloadNo + "/" + m3u8.DownloadUrl.Count);

                Console.WriteLine("ffmpegMergeStatus Finished");
            }
            else if (status == Status.Running)
            {
                if (callBack != null) callBack(status, id, "正在合并" + downloadNo + "/" + m3u8.DownloadUrl.Count);

            }

        }
    }
}
