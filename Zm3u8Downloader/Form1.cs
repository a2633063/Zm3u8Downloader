using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Zm3u8Downloader.Aria2Download;
using static Zm3u8Downloader.ffmpegMerge;
using static Zm3u8Downloader.m3u8Download;

namespace Zm3u8Downloader
{
    public partial class Form1 : Form
    {
        const int MAX_DOWNLOAD_COUNT = 3;
        DataTable dt = new DataTable();
        List<Aria2Download.Status> finishList = new List<Aria2Download.Status>();
        List<int> falseList = new List<int>();
        List<int> successList = new List<int>();

        void DownloadParamterInit()
        {
            finishList.Clear();
            falseList.Clear();
            successList.Clear();

        }
        public Form1()
        {
            InitializeComponent();
            dt.Columns.Add("文件", typeof(String));
            dt.Columns.Add("本地路径", typeof(String));
            dt.Columns.Add("分片数量", typeof(String));
            dt.Columns.Add("时长", typeof(String));
            dt.Columns.Add("状态", typeof(String));
            dt.Columns.Add("m3u8File", typeof(m3u8File));
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[dataGridView1.Columns.Count - 1].Visible = false;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[2].Width = 80;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].Width = 80;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;



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

        void addM3u8File(String filepath)
        {
            m3u8File m3u8file = new m3u8File(filepath);
            //int index = dataGridView1.Rows.Add();
            //dataGridView1.Rows[index].Cells[0].Value = m3u8file.name;
            //dataGridView1.Rows[index].Cells[1].Value = m3u8file.path;
            //dataGridView1.Rows[index].Cells[2].Value = m3u8file.PartNo;
            //TimeSpan ts = new TimeSpan(0, 0, (int)m3u8file.totalTime);
            //dataGridView1.Rows[index].Cells[3].Value = string.Format("{0:d2}:{1:d2}:{2:d2}", ts.Hours, ts.Minutes, ts.Seconds);
            TimeSpan ts = new TimeSpan(0, 0, (int)m3u8file.totalTime);
            dt.Rows.Add(m3u8file.name, m3u8file.path, m3u8file.PartNo, string.Format("{0:d2}:{1:d2}:{2:d2}", ts.Hours, ts.Minutes, ts.Seconds), "", m3u8file);

        }

        private void btnSelectDownload_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)//小于等于0 为没有选中任何行
            {
                m3u8DownloadCallBack cb = new m3u8DownloadCallBack(OnSelectDownloadDataReceived);
                int t = dataGridView1.SelectedRows[0].Index;// 获取当前行的 行号               
                m3u8Download download = new m3u8Download((m3u8File)(dt.Rows[t][5]), t, chkMerge.Checked, cb);
                download.start();
            }
            else
            {
                MessageBox.Show("请选择一行！");
            }

        }
        public void OnSelectDownloadDataReceived(Aria2Download.Status status, int id, string str)
        {
            dt.Rows[id][4] = str;
        }


        private void btnPartUrlCheck_Click(object sender, EventArgs e)
        {


            if (dataGridView1.SelectedRows.Count > 0)//小于等于0 为没有选中任何行
            {
                string a = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();// 获取当前行 某个单元格数据

                string b = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();// 获取当前行 某个单元格数据

                int t = dataGridView1.SelectedRows[0].Index;// 获取当前行的 行号
                object v1 = dt.Rows[t][5];
                PartUrl f = new PartUrl((m3u8File)(dt.Rows[t][5]));
                f.ShowDialog();
            }
            else
            {
                MessageBox.Show("请选择一行！");
            }
        }

        m3u8DownloadCallBack DownloadAllDataReceived;
        private void btnDownloadAll_Click(object sender, EventArgs e)
        {
            DownloadParamterInit();
            DownloadAllDataReceived = new m3u8DownloadCallBack(OnDownloadAllDataReceived); ;
            if (dataGridView1.Rows.Count > 0)//小于等于0 为没有任何行
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    finishList.Add(Aria2Download.Status.NoStart);
                int t = 0;
                for (int i = 0; i < MAX_DOWNLOAD_COUNT && i < dataGridView1.Rows.Count; i++)
                {
                    m3u8Download download = new m3u8Download((m3u8File)(dt.Rows[t][5]), t, chkMerge.Checked, DownloadAllDataReceived);
                    finishList[t] = Aria2Download.Status.Running;
                    download.start();
                    t++;
                }

            }
            else
            {
                MessageBox.Show("内容为空!");
            }
        }
        public void OnDownloadAllDataReceived(Aria2Download.Status status, int id, string str)
        {
            dt.Rows[id][4] = str;
            if (status == Aria2Download.Status.Failed || status == Aria2Download.Status.Finished)
            {//处理完成一个(包括下载成功或失败),处理下一个
                finishList[id] = status;

                for (id = id + 1; id < dt.Rows.Count; id++)
                {
                    if (finishList[id] == Aria2Download.Status.NoStart)
                    {//未下载
                        m3u8Download download = new m3u8Download((m3u8File)(dt.Rows[id][5]), id, chkMerge.Checked, DownloadAllDataReceived);
                        download.start();
                        finishList[id] = Aria2Download.Status.Running;
                        break;
                    }

                }

                if (id >= dt.Rows.Count)
                {
                    for (int i = 0; i < finishList.Count; i++)
                    {
                        if (finishList[i] == Aria2Download.Status.Running)
                        {
                            falseList.Clear();
                            successList.Clear();
                            return;//等待其他下载完毕
                        }
                        else if (finishList[i] == Aria2Download.Status.Failed) falseList.Add(i);
                        else if (finishList[i] == Aria2Download.Status.Finished) successList.Add(i);
                    }
                    MessageBox.Show("成功:" + successList.Count + ",失败" + falseList.Count + "个", "下载完成");
                }

            }
        }

    }
}
