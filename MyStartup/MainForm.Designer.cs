namespace MyStartup
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cbStartWithSystem = new System.Windows.Forms.CheckBox();
            this.cbBlockScreenOff = new System.Windows.Forms.CheckBox();
            this.cbBlockSystemSleep = new System.Windows.Forms.CheckBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.ColumnIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnURL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDomain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnInterval = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLast = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnNeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBoxInterval = new System.Windows.Forms.TextBox();
            this.textBoxURL = new System.Windows.Forms.TextBox();
            this.groupBoxAdd = new System.Windows.Forms.GroupBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripForDataGridView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemVisit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemModifyInterval = new System.Windows.Forms.ToolStripMenuItem();
            this.cbNotAutoVisit = new System.Windows.Forms.CheckBox();
            this.textBoxDelayHour = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelInfo = new System.Windows.Forms.Label();
            this.buttonUpdateLastVisitByChromeHistory = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.groupBoxAdd.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.contextMenuStripForDataGridView.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbStartWithSystem
            // 
            this.cbStartWithSystem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbStartWithSystem.AutoSize = true;
            this.cbStartWithSystem.Checked = true;
            this.cbStartWithSystem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbStartWithSystem.Location = new System.Drawing.Point(924, 12);
            this.cbStartWithSystem.Name = "cbStartWithSystem";
            this.cbStartWithSystem.Size = new System.Drawing.Size(72, 16);
            this.cbStartWithSystem.TabIndex = 4;
            this.cbStartWithSystem.Text = "开机启动";
            this.cbStartWithSystem.UseVisualStyleBackColor = true;
            // 
            // cbBlockScreenOff
            // 
            this.cbBlockScreenOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBlockScreenOff.AutoSize = true;
            this.cbBlockScreenOff.Checked = true;
            this.cbBlockScreenOff.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBlockScreenOff.Location = new System.Drawing.Point(900, 34);
            this.cbBlockScreenOff.Name = "cbBlockScreenOff";
            this.cbBlockScreenOff.Size = new System.Drawing.Size(96, 16);
            this.cbBlockScreenOff.TabIndex = 3;
            this.cbBlockScreenOff.Text = "阻止屏幕关闭";
            this.cbBlockScreenOff.UseVisualStyleBackColor = true;
            // 
            // cbBlockSystemSleep
            // 
            this.cbBlockSystemSleep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBlockSystemSleep.AutoSize = true;
            this.cbBlockSystemSleep.Checked = true;
            this.cbBlockSystemSleep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBlockSystemSleep.Location = new System.Drawing.Point(900, 56);
            this.cbBlockSystemSleep.Name = "cbBlockSystemSleep";
            this.cbBlockSystemSleep.Size = new System.Drawing.Size(96, 16);
            this.cbBlockSystemSleep.TabIndex = 5;
            this.cbBlockSystemSleep.Text = "阻止系统休眠";
            this.cbBlockSystemSleep.UseVisualStyleBackColor = true;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnIndex,
            this.ColumnURL,
            this.ColumnDomain,
            this.ColumnInterval,
            this.ColumnLast,
            this.ColumnNeed});
            this.dataGridView.Location = new System.Drawing.Point(12, 12);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(773, 513);
            this.dataGridView.TabIndex = 6;
            // 
            // ColumnIndex
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnIndex.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnIndex.FillWeight = 32F;
            this.ColumnIndex.HeaderText = "序号";
            this.ColumnIndex.Name = "ColumnIndex";
            this.ColumnIndex.ReadOnly = true;
            // 
            // ColumnURL
            // 
            this.ColumnURL.FillWeight = 200F;
            this.ColumnURL.HeaderText = "网址";
            this.ColumnURL.Name = "ColumnURL";
            this.ColumnURL.ReadOnly = true;
            // 
            // ColumnDomain
            // 
            this.ColumnDomain.FillWeight = 120F;
            this.ColumnDomain.HeaderText = "域名";
            this.ColumnDomain.Name = "ColumnDomain";
            this.ColumnDomain.ReadOnly = true;
            // 
            // ColumnInterval
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnInterval.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnInterval.FillWeight = 68F;
            this.ColumnInterval.HeaderText = "间隔(天)";
            this.ColumnInterval.Name = "ColumnInterval";
            this.ColumnInterval.ReadOnly = true;
            // 
            // ColumnLast
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnLast.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnLast.HeaderText = "上次";
            this.ColumnLast.Name = "ColumnLast";
            this.ColumnLast.ReadOnly = true;
            // 
            // ColumnNeed
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnNeed.DefaultCellStyle = dataGridViewCellStyle5;
            this.ColumnNeed.FillWeight = 80F;
            this.ColumnNeed.HeaderText = "剩余(小时)";
            this.ColumnNeed.Name = "ColumnNeed";
            this.ColumnNeed.ReadOnly = true;
            // 
            // textBoxInterval
            // 
            this.textBoxInterval.Location = new System.Drawing.Point(6, 47);
            this.textBoxInterval.Name = "textBoxInterval";
            this.textBoxInterval.Size = new System.Drawing.Size(193, 21);
            this.textBoxInterval.TabIndex = 7;
            // 
            // textBoxURL
            // 
            this.textBoxURL.Location = new System.Drawing.Point(6, 20);
            this.textBoxURL.Name = "textBoxURL";
            this.textBoxURL.Size = new System.Drawing.Size(193, 21);
            this.textBoxURL.TabIndex = 9;
            // 
            // groupBoxAdd
            // 
            this.groupBoxAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAdd.Controls.Add(this.buttonAdd);
            this.groupBoxAdd.Controls.Add(this.textBoxURL);
            this.groupBoxAdd.Controls.Add(this.textBoxInterval);
            this.groupBoxAdd.Location = new System.Drawing.Point(791, 414);
            this.groupBoxAdd.Name = "groupBoxAdd";
            this.groupBoxAdd.Size = new System.Drawing.Size(205, 111);
            this.groupBoxAdd.TabIndex = 10;
            this.groupBoxAdd.TabStop = false;
            this.groupBoxAdd.Text = "添加";
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(6, 74);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(193, 23);
            this.buttonAdd.TabIndex = 10;
            this.buttonAdd.Text = "添加";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "notifyIcon";
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(101, 48);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.showToolStripMenuItem.Text = "显示";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.ShowToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.exitToolStripMenuItem.Text = "退出";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // contextMenuStripForDataGridView
            // 
            this.contextMenuStripForDataGridView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemVisit,
            this.toolStripMenuItemDelete,
            this.toolStripMenuItemModifyInterval});
            this.contextMenuStripForDataGridView.Name = "contextMenuStripForDataGridView";
            this.contextMenuStripForDataGridView.Size = new System.Drawing.Size(125, 70);
            this.contextMenuStripForDataGridView.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripForDataGridView_Opening);
            // 
            // toolStripMenuItemVisit
            // 
            this.toolStripMenuItemVisit.Name = "toolStripMenuItemVisit";
            this.toolStripMenuItemVisit.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItemVisit.Text = "访问网址";
            this.toolStripMenuItemVisit.Click += new System.EventHandler(this.ToolStripMenuItemVisit_Click);
            // 
            // toolStripMenuItemDelete
            // 
            this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
            this.toolStripMenuItemDelete.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItemDelete.Text = "删除项目";
            this.toolStripMenuItemDelete.Click += new System.EventHandler(this.ToolStripMenuItemDelete_Click);
            // 
            // toolStripMenuItemModifyInterval
            // 
            this.toolStripMenuItemModifyInterval.Name = "toolStripMenuItemModifyInterval";
            this.toolStripMenuItemModifyInterval.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItemModifyInterval.Text = "修改间隔";
            this.toolStripMenuItemModifyInterval.Click += new System.EventHandler(this.ToolStripMenuItemModifyInterval_Click);
            // 
            // cbNotAutoVisit
            // 
            this.cbNotAutoVisit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbNotAutoVisit.AutoSize = true;
            this.cbNotAutoVisit.Location = new System.Drawing.Point(829, 85);
            this.cbNotAutoVisit.Name = "cbNotAutoVisit";
            this.cbNotAutoVisit.Size = new System.Drawing.Size(15, 14);
            this.cbNotAutoVisit.TabIndex = 11;
            this.cbNotAutoVisit.UseVisualStyleBackColor = true;
            // 
            // textBoxDelayHour
            // 
            this.textBoxDelayHour.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDelayHour.Location = new System.Drawing.Point(850, 81);
            this.textBoxDelayHour.Name = "textBoxDelayHour";
            this.textBoxDelayHour.Size = new System.Drawing.Size(39, 21);
            this.textBoxDelayHour.TabIndex = 12;
            this.textBoxDelayHour.Text = "12";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(892, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "小时内不自动访问";
            // 
            // labelInfo
            // 
            this.labelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(826, 116);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(167, 12);
            this.labelInfo.TabIndex = 14;
            this.labelInfo.Text = "到 0000-00-00 00:00:00 结束";
            // 
            // buttonUpdateLastVisitByChromeHistory
            // 
            this.buttonUpdateLastVisitByChromeHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpdateLastVisitByChromeHistory.Location = new System.Drawing.Point(791, 154);
            this.buttonUpdateLastVisitByChromeHistory.Name = "buttonUpdateLastVisitByChromeHistory";
            this.buttonUpdateLastVisitByChromeHistory.Size = new System.Drawing.Size(205, 47);
            this.buttonUpdateLastVisitByChromeHistory.TabIndex = 15;
            this.buttonUpdateLastVisitByChromeHistory.Text = "更新上次访问时间（通过Chrome历史记录，Chrome需关闭）";
            this.buttonUpdateLastVisitByChromeHistory.UseVisualStyleBackColor = true;
            this.buttonUpdateLastVisitByChromeHistory.Click += new System.EventHandler(this.ButtonUpdateLastVisitByChromeHistory_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 537);
            this.Controls.Add(this.buttonUpdateLastVisitByChromeHistory);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxDelayHour);
            this.Controls.Add(this.cbNotAutoVisit);
            this.Controls.Add(this.groupBoxAdd);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.cbBlockSystemSleep);
            this.Controls.Add(this.cbStartWithSystem);
            this.Controls.Add(this.cbBlockScreenOff);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MyStartup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.groupBoxAdd.ResumeLayout(false);
            this.groupBoxAdd.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStripForDataGridView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbStartWithSystem;
        private System.Windows.Forms.CheckBox cbBlockScreenOff;
        private System.Windows.Forms.CheckBox cbBlockSystemSleep;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TextBox textBoxInterval;
        private System.Windows.Forms.TextBox textBoxURL;
        private System.Windows.Forms.GroupBox groupBoxAdd;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnURL;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDomain;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnInterval;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLast;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNeed;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripForDataGridView;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemVisit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelete;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemModifyInterval;
        private System.Windows.Forms.CheckBox cbNotAutoVisit;
        private System.Windows.Forms.TextBox textBoxDelayHour;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button buttonUpdateLastVisitByChromeHistory;
    }
}

