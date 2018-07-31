using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zm3u8Downloader
{
    public partial class PartUrl : Form
    {
        m3u8File m3u8;
        public PartUrl()
        {
            InitializeComponent();
        }
        public PartUrl(m3u8File m3u8)
        {
            InitializeComponent();
            this.m3u8 = m3u8;
            this.Text = m3u8.name;
            init();
        }
         void init()
        {



                for (int i = 0; i < m3u8.DownloadUrl.Count; i++)
            {
                int index = dataGridView1.Rows.Add();
                Regex reg = new Regex(@".*/(.*\.ts).*");
                Match m = reg.Match(m3u8.DownloadUrl[i]);
                if (m.Success)
                {
                    dataGridView1.Rows[index].Cells[0].Value = m.Result("$1");
                }
                dataGridView1.Rows[index].Cells[1].Value = m3u8.DownloadUrl[i];
            }
        }
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
    }
}
