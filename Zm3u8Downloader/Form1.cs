using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Zm3u8Downloader.Aria2Download;
using static Zm3u8Downloader.ffmpegMerge;

namespace Zm3u8Downloader
{
    public partial class Form1 : Form
    {
        const int MAX_DOWNLOAD_COUNT = 3;

        List<m3u8File> m3u8FileList = new List<m3u8File>();

        int failCount = 0;
        int Downloading = 0;

        class FileId
        {
            public int fileId = -1;
            public int partId = -1;

            public FileId(int a, int b)
            {
                fileId = a;
                partId = b;
            }
        };


        int fileId = 0;
        int partId = 0;

        void DownloadParamterInit()
        {

            foreach (m3u8File m in m3u8FileList)
            {
                m.DownloadStateClear();
            }
            failCount = 0;
            fileId = 0;
            partId = 0;
            log.Text = "";
        }



        static int mainThreadId;
        public Form1()
        {

            mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            mainThreadSynContext = SynchronizationContext.Current;
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;  //禁止编译器对跨线程访问做检查
            dataGridView1.Columns[0].DataPropertyName = "name";
            dataGridView1.Columns[1].DataPropertyName = "path";
            dataGridView1.Columns[2].DataPropertyName = "totalTime";
            dataGridView1.Columns[3].DataPropertyName = "partCount";
            dataGridView1.Columns[4].DataPropertyName = "state";
            // dataGridView1.DataSource = new BindingList<m3u8File>(m3u8FileList);
            //dataGridView1.DataSource = m3u8FileList;


            //dataGridView1.Columns[0].DataPropertyName = "name";
            //dataGridView1.DataSource = m3u8FileList;




        }
        #region dataGridView事件

        #region dataGridView增加序号
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //自动编号，与数据无关
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
               e.RowBounds.Location.Y,
               dataGridView1.RowHeadersWidth - 4,
               e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics,
                  (e.RowIndex + 1).ToString(),
                   dataGridView1.RowHeadersDefaultCellStyle.Font,
                   rectangle,
                   dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                   TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion

        #region dataGridView1文件拖入处理
        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            //string[] array = e.Data.GetData(DataFormats.FileDrop);
            foreach (String s in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                if (s.EndsWith(".m3u8"))
                {
                    addM3u8File(s);
                }
                else if (Directory.Exists(s))
                {//文件夹
                    DirectoryInfo dir = new DirectoryInfo(s);
                    FileInfo[] fil = dir.GetFiles("*.m3u8");
                    foreach (FileInfo f in fil)
                    {
                        addM3u8File(f.FullName);
                    }
                }
            }
            dataGridView1.DataSource = new List<m3u8File>();
            dataGridView1.DataSource = m3u8FileList;

        }


        #endregion

        #region dataGridView1 选中一行有效数据时显示当前下载分片
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)//小于等于0 为没有选中任何行
            {
                int row = dataGridView1.SelectedRows[0].Index;

                dataGridView2.Rows.Clear();
                for (int i = 0; i < m3u8FileList[row].DownloadUrl.Count; i++)
                {
                    int index = dataGridView2.Rows.Add();
                }
                List<String> DownloadTsFile = new List<string>();
                for (int i = 0; i < m3u8FileList[row].DownloadUrl.Count; i++)
                {
                    updateDownloadState(row, i);
                }

            }

        }
        #endregion
        #endregion

        #region 线程处理函数
        private delegate void SetTextCallback(object sender, string text);
        private delegate void AppendTextCallback(object sender, string text);
        //线程相关函数
        private void SetText(object sender, string text)
        {
            // InvokeRequired需要比较调用线程ID和创建线程ID
            // 如果它们不相同则返回true
            if (((TextBox)sender).InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { sender, text });
            }
            else
            {
                ((TextBox)sender).Text = text;
            }
        }
        private void AppendText(object sender, string text)
        {
            // InvokeRequired需要比较调用线程ID和创建线程ID
            // 如果它们不相同则返回true
            if (((TextBox)sender).InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { sender, text });
            }
            else
            {
                ((TextBox)sender).AppendText(text);
            }
        }

        #endregion


        #region 功能子函数

        #region 增加一个m3u8文件
        void addM3u8File(String filepath)
        {
            m3u8FileList.Add(new m3u8File(filepath));

        }
        #endregion

        #region 开始下载下一个ts文件
        private SynchronizationContext mainThreadSynContext;
        void DownloadNextFile()
        {
            mainThreadSynContext.Post(new SendOrPostCallback(DownloadNextFile1), null);
        }
        void DownloadNextFile1(object state)
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId)
            {
                Console.WriteLine("主线程" + partId);
            }
            else
            {
                Console.WriteLine("子线程" + partId);
            }
            if (fileId >= m3u8FileList.Count)
            {
                return;
            }


            if (partId == 0)
            {
                log.AppendText("开始下载文件" + fileId + ":");
                
            }
            //dataGridView1.Rows[fileId].Cells[4].Value = string.Format("{0:D3}/{1:D3}", partId, m3u8FileList[fileId].DownloadUrl.Count); 
            m3u8FileList[fileId].stateId = partId;
            dataGridView1.DataSource = new List<m3u8File>();
            dataGridView1.DataSource = m3u8FileList;
            log.AppendText("    分片" + partId + "\n");

            Console.WriteLine("开始下载文件" + m3u8FileList[fileId].name + "*************************************************");
            String url = m3u8FileList[fileId].DownloadUrl[partId];
            String partName = url.Substring(url.LastIndexOf('/') + 1);

            DownloadAllDataReceived = new Aria2DownloadCallBack(OnDownloadAllDataReceived);
            Aria2Download download = new Aria2Download(url, m3u8FileList[fileId].name, DownloadAllDataReceived, new FileId(fileId, partId));
            download.Start();
            m3u8FileList[fileId].DownloadState[partId] = 0;
            updateDownloadState(fileId, partId);
            Downloading++;
            partId++;
            if (partId >= m3u8FileList[fileId].DownloadUrl.Count)
            {
                partId = 0;
                Status res = Status.Finished;
                for (int i=0;i<m3u8FileList[fileId].DownloadState.Count;i++)
                {
                   if( m3u8FileList[fileId].DownloadState[i] == (int)Status.Failed)
                    {
                        res = Status.Failed;
                        break;
                    }
                }
                try
                {
                    m3u8FileList[fileId].stateId = (int)res;
                    m3u8FileList[fileId].stateId = 0;
                }
                catch (Exception)
                {

                    //throw;
                }
                fileId++;
                dataGridView1.DataSource = new List<m3u8File>();
                dataGridView1.DataSource = m3u8FileList;
            }
        }
        #endregion

        void updateDownloadState(int row, int i)
        {
            if (dataGridView1.CurrentRow==null || dataGridView1.SelectedRows[0].Index != row) return;


            String url = m3u8FileList[row].DownloadUrl[i];
            String stateStr = "";

            dataGridView2.Rows[i].Cells[0].Value = url.Substring(url.LastIndexOf('/') + 1);
            switch ((Status)m3u8FileList[row].DownloadState[i])
            {
                case Status.NoStart: stateStr = ""; break;
                case Status.Paused: stateStr = "暂停"; break;
                case Status.Running: stateStr = "正在下载"; break;
                case Status.Failed: stateStr = "失败"; break;
                case Status.Exit: case Status.Finished: stateStr = "下载完成"; break;

                default:
                    stateStr = m3u8FileList[row].DownloadState[i] + "%";
                    break;
            }
            dataGridView2.Rows[i].Cells[1].Value = stateStr;
        }
        #endregion




        Aria2DownloadCallBack DownloadAllDataReceived;
        private void btnDownloadAll_Click(object sender, EventArgs e)
        {
            DownloadParamterInit();
            if (dataGridView1.Rows.Count < 1) //小于等于0 为没有任何行
            {
                MessageBox.Show("内容为空!");
                return;
            }

            //从第0行开始下载

            for (int i = 0; i < MAX_DOWNLOAD_COUNT; i++)
            {
                DownloadNextFile();
            }


        }
        public void OnDownloadAllDataReceived(Status status, int progress, Object param)
        {
            int a = ((FileId)param).fileId;
            int b = ((FileId)param).partId;
            m3u8FileList[a].DownloadState[b] = (int)status;
            switch (status)
            {
                case Status.NoStart: break;
                case Status.Paused: break;
                case Status.Running: m3u8FileList[a].DownloadState[b] = progress; break;
                case Status.Failed: Downloading--; failCount++; break;
                case Status.Finished: Downloading--; break;
                case Status.Exit: Console.WriteLine("process_Exited."+a+","+b); DownloadNextFile();break;
            }

            try
            {
                int row = dataGridView1.SelectedRows[0].Index;
                if (a == row)
                {
                    updateDownloadState(row, b);
                }
            }
            catch (Exception)
            {

               // throw;
            }
        }

        private void btnListClear_Click(object sender, EventArgs e)
        {
            m3u8FileList.Clear();
            dataGridView1.DataSource = new List<m3u8File>();
            dataGridView1.DataSource = m3u8FileList;
            dataGridView2.Rows.Clear();
            //dataGridView1.Rows.Clear();
        }
    }
}
