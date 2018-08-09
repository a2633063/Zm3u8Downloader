using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Zm3u8Downloader.Aria2Download;

namespace Zm3u8Downloader
{
    public class ffmpegMerge
    {
        public delegate void ffmpegMergeCallBack(Status status);

        public Status status;
        public ffmpegMergeCallBack callBack;
        public string uri;
        public string filename;

        public ffmpegMerge(string uri, string name, ffmpegMergeCallBack cb)
        {
            this.uri = uri.Replace("\\", "/");
            this.filename = name.Replace("\\", "/");
            this.callBack = cb;
            status = Status.Paused;
        }


        public void Start()
        {
            //新建线程
            Process process = new Process();
            process.StartInfo.FileName = @".\ffmpeg.exe";
            //            process.StartInfo.Arguments = " -o \"" + filename + "\" -c -x 2 " + uri;
            process.StartInfo.Arguments = @" -protocol_whitelist ""file,http,https,rtp,udp,tcp,crypto"" -i  """ + uri  + "\" -c copy -bsf:a aac_adtstoasc \"" + filename;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += process_OutputDataReceived;
            process.ErrorDataReceived += process_OutputDataReceived;
            process.Exited += process_Exited;
            status = Status.Running;
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

        }



        private delegate void UpdateStatusDelegate(string status);
        bool success = true;
        private void process_Exited(object sender, System.EventArgs e)
        {
            if (success)
            {
                status = Status.Finished;
            }else
            {
                status = Status.Failed;
            }
            callBack.Invoke(status);
            Console.WriteLine("process_Exited");
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            
            if (e.Data == null) return;
            //debug
            //Console.WriteLine(e.Data);
            //end debug

            if (
                e.Data.Contains("error")
                || e.Data.Contains("fail")
                )
            {
                status = Status.Failed;
                success = false;
                callBack.Invoke(status);
            }
            else
            {
                status = Status.Running;
                callBack.Invoke(status);
            }


        }
    }
}
