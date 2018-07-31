using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Zm3u8Downloader.Aria2Download;

namespace Zm3u8Downloader
{
    class m3u8Download
    {
        public delegate void m3u8DownloadCallBack(Status status,int id, String str);

        public int id;
        public Status status;
        public m3u8DownloadCallBack callBack;
        public m3u8File m3u8;
        int downloadNo = 0;

        public m3u8Download(m3u8File m,int id, m3u8DownloadCallBack cb)
        {
            m3u8 = m;
            callBack = cb;
            status = 0;
            this.id = id;
        }


        public void start()
        {
            String filename;
            Regex reg = new Regex(@".*/(.*\.ts).*");
            Match m = reg.Match(m3u8.DownloadUrl[downloadNo]);
            if (m.Success)
            {
                filename = m.Result("$1");
            }
            else filename = DateTime.Now.ToString("yyyyMMdd_HHmmss")+".ts";

            DownloadCallBack cb = new DownloadCallBack(OnDataReceived);
            Aria2Download mydownload = new Aria2Download(m3u8.DownloadUrl[downloadNo], filename, cb);
            mydownload.Start();
            Console.WriteLine("ok");
        }
        public void OnDataReceived(Status status, int progress)
        {

            //            dt.Rows[0][4] = "当前进度: " + progress + "%";
            if (status == Status.Finished)
            {//下载完成
                downloadNo++;
                if (downloadNo < m3u8.DownloadUrl.Count)
                {//下载下一个
                    start();
                }else if(callBack!=null)
                {
                    this.status = Status.Finished;
                    callBack(status, id, "下载完成  " + m3u8.DownloadUrl.Count+"/"+ m3u8.DownloadUrl.Count);
                }
            }else
            {
                this.status = status;
                switch (status)
                {
                    case Status.Running:
                        if (callBack != null) callBack(status, id, "下载:" + downloadNo + "/" + m3u8.DownloadUrl.Count +"  "+progress+"%");
                        break;
                    case Status.Failed:
                        if (callBack != null) callBack(status, id, "失败!" + downloadNo + "/" + m3u8.DownloadUrl.Count);
                        break;
                    case Status.Paused:
                        if (callBack != null) callBack(status, id, "下载暂停 " + downloadNo + "/" + m3u8.DownloadUrl.Count + "  " + progress + "%");
                        break;
                }
            }
            Console.WriteLine("当前进度: " + progress + "%");
        }
    }
}
