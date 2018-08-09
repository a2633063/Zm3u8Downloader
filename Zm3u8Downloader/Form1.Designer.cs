namespace Zm3u8Downloader
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnSelectDownload = new System.Windows.Forms.Button();
            this.btnPartUrlCheck = new System.Windows.Forms.Button();
            this.btnDownloadAll = new System.Windows.Forms.Button();
            this.chkMerge = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowDrop = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 88);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(862, 319);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView1_RowPostPaint);
            this.dataGridView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragDrop);
            this.dataGridView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragEnter);
            // 
            // btnSelectDownload
            // 
            this.btnSelectDownload.Location = new System.Drawing.Point(12, 59);
            this.btnSelectDownload.Name = "btnSelectDownload";
            this.btnSelectDownload.Size = new System.Drawing.Size(87, 23);
            this.btnSelectDownload.TabIndex = 1;
            this.btnSelectDownload.Text = "下载选中项";
            this.btnSelectDownload.UseVisualStyleBackColor = true;
            this.btnSelectDownload.Click += new System.EventHandler(this.btnSelectDownload_Click);
            // 
            // btnPartUrlCheck
            // 
            this.btnPartUrlCheck.Location = new System.Drawing.Point(187, 59);
            this.btnPartUrlCheck.Name = "btnPartUrlCheck";
            this.btnPartUrlCheck.Size = new System.Drawing.Size(100, 23);
            this.btnPartUrlCheck.TabIndex = 3;
            this.btnPartUrlCheck.Text = "查看分片链接";
            this.btnPartUrlCheck.UseVisualStyleBackColor = true;
            this.btnPartUrlCheck.Click += new System.EventHandler(this.btnPartUrlCheck_Click);
            // 
            // btnDownloadAll
            // 
            this.btnDownloadAll.Location = new System.Drawing.Point(106, 58);
            this.btnDownloadAll.Name = "btnDownloadAll";
            this.btnDownloadAll.Size = new System.Drawing.Size(75, 23);
            this.btnDownloadAll.TabIndex = 4;
            this.btnDownloadAll.Text = "下载全部";
            this.btnDownloadAll.UseVisualStyleBackColor = true;
            this.btnDownloadAll.Click += new System.EventHandler(this.btnDownloadAll_Click);
            // 
            // chkMerge
            // 
            this.chkMerge.AutoSize = true;
            this.chkMerge.Location = new System.Drawing.Point(293, 63);
            this.chkMerge.Name = "chkMerge";
            this.chkMerge.Size = new System.Drawing.Size(96, 16);
            this.chkMerge.TabIndex = 5;
            this.chkMerge.Text = "自动合并文件";
            this.chkMerge.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 606);
            this.Controls.Add(this.chkMerge);
            this.Controls.Add(this.btnDownloadAll);
            this.Controls.Add(this.btnPartUrlCheck);
            this.Controls.Add(this.btnSelectDownload);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnSelectDownload;
        private System.Windows.Forms.Button btnPartUrlCheck;
        private System.Windows.Forms.Button btnDownloadAll;
        private System.Windows.Forms.CheckBox chkMerge;
    }
}

