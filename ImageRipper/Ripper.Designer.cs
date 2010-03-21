namespace ImgRipper
{
    partial class Ripper
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ripper));
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.btnDownloadCancel = new Wyvern.SplitButton();
            this.cmsBatch = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnBatch = new System.Windows.Forms.ToolStripMenuItem();
            this.tbParse = new System.Windows.Forms.TextBox();
            this.llCookie = new System.Windows.Forms.LinkLabel();
            this.llFolder = new System.Windows.Forms.LinkLabel();
            this.lvRip = new System.Windows.Forms.ListView();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsLV = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmmiNextPage = new System.Windows.Forms.ToolStripMenuItem();
            this.cmmiDropGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.cmmiCopyName = new System.Windows.Forms.ToolStripMenuItem();
            this.cmmiSaveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cmmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.cmmiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.cmmiClear = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBatch = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.CloudToolStrip = new System.Windows.Forms.ToolStripSplitButton();
            this.SkyDriveItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FlickrItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FacebookItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PicasaItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblUrl = new System.Windows.Forms.Label();
            this.tbDir = new System.Windows.Forms.TextBox();
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.DownloadFiles = new System.ComponentModel.BackgroundWorker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tmMinus = new System.Windows.Forms.Timer(this.components);
            this.tmPlus = new System.Windows.Forms.Timer(this.components);
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            this.cmsBatch.SuspendLayout();
            this.cmsLV.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // mainSplit
            // 
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.Location = new System.Drawing.Point(0, 0);
            this.mainSplit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.mainSplit.Name = "mainSplit";
            // 
            // mainSplit.Panel1
            // 
            this.mainSplit.Panel1.AutoScroll = true;
            this.mainSplit.Panel1.Controls.Add(this.btnDownloadCancel);
            this.mainSplit.Panel1.Controls.Add(this.tbParse);
            this.mainSplit.Panel1.Controls.Add(this.llCookie);
            this.mainSplit.Panel1.Controls.Add(this.llFolder);
            this.mainSplit.Panel1.Controls.Add(this.lvRip);
            this.mainSplit.Panel1.Controls.Add(this.statusStrip1);
            this.mainSplit.Panel1.Controls.Add(this.lblUrl);
            this.mainSplit.Panel1.Controls.Add(this.tbDir);
            this.mainSplit.Panel1.Padding = new System.Windows.Forms.Padding(1);
            this.mainSplit.Panel1MinSize = 0;
            // 
            // mainSplit.Panel2
            // 
            this.mainSplit.Panel2.Controls.Add(this.pbPreview);
            this.mainSplit.Panel2Collapsed = true;
            this.mainSplit.Panel2MinSize = 0;
            this.mainSplit.Size = new System.Drawing.Size(787, 488);
            this.mainSplit.SplitterDistance = 407;
            this.mainSplit.SplitterWidth = 5;
            this.mainSplit.TabIndex = 23;
            // 
            // btnDownloadCancel
            // 
            this.btnDownloadCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownloadCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDownloadCancel.ContextMenuStrip = this.cmsBatch;
            this.btnDownloadCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDownloadCancel.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
            this.btnDownloadCancel.Image = global::ImgRipper.Properties.Resources.Download;
            this.btnDownloadCancel.Location = new System.Drawing.Point(715, 12);
            this.btnDownloadCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDownloadCancel.Name = "btnDownloadCancel";
            this.btnDownloadCancel.Size = new System.Drawing.Size(64, 66);
            this.btnDownloadCancel.TabIndex = 3;
            this.btnDownloadCancel.TabStop = false;
            this.btnDownloadCancel.UseVisualStyleBackColor = true;
            this.btnDownloadCancel.Click += new System.EventHandler(this.DownloadCancel_Click);
            // 
            // cmsBatch
            // 
            this.cmsBatch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBatch});
            this.cmsBatch.Name = "cmsButton";
            this.cmsBatch.ShowItemToolTips = false;
            this.cmsBatch.Size = new System.Drawing.Size(110, 26);
            // 
            // btnBatch
            // 
            this.btnBatch.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnBatch.Image = ((System.Drawing.Image)(resources.GetObject("btnBatch.Image")));
            this.btnBatch.Name = "btnBatch";
            this.btnBatch.Size = new System.Drawing.Size(109, 22);
            this.btnBatch.Text = "Batch";
            this.btnBatch.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.btnBatch.Click += new System.EventHandler(this.btnBatch_Click);
            // 
            // tbParse
            // 
            this.tbParse.AllowDrop = true;
            this.tbParse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbParse.AutoCompleteCustomSource.AddRange(new string[] {
            "http://www.heels.cn/web/viewthread?thread=",
            "http://www.duide.com/",
            "http://meituiji.com/",
            "http://tu11.cc/"});
            this.tbParse.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbParse.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbParse.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ImgRipper.Properties.Settings.Default, "txtParse", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbParse.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.tbParse.Location = new System.Drawing.Point(43, 13);
            this.tbParse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbParse.Name = "tbParse";
            this.tbParse.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.tbParse.Size = new System.Drawing.Size(666, 23);
            this.tbParse.TabIndex = 20;
            this.tbParse.TabStop = false;
            this.tbParse.Text = global::ImgRipper.Properties.Settings.Default.txtParse;
            this.tbParse.WordWrap = false;
            this.tbParse.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbParse_DragDrop);
            this.tbParse.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbParse_DragEnter);
            // 
            // llCookie
            // 
            this.llCookie.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llCookie.AutoSize = true;
            this.llCookie.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.llCookie.LinkColor = System.Drawing.SystemColors.Highlight;
            this.llCookie.Location = new System.Drawing.Point(653, 55);
            this.llCookie.Name = "llCookie";
            this.llCookie.Size = new System.Drawing.Size(56, 19);
            this.llCookie.TabIndex = 32;
            this.llCookie.TabStop = true;
            this.llCookie.Text = "Cookie";
            this.llCookie.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llCookie_LinkClicked);
            // 
            // llFolder
            // 
            this.llFolder.AutoSize = true;
            this.llFolder.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.llFolder.LinkColor = System.Drawing.SystemColors.Highlight;
            this.llFolder.Location = new System.Drawing.Point(4, 55);
            this.llFolder.Name = "llFolder";
            this.llFolder.Size = new System.Drawing.Size(30, 19);
            this.llFolder.TabIndex = 31;
            this.llFolder.TabStop = true;
            this.llFolder.Text = "Dir";
            this.llFolder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.llFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llFolder_LinkClicked);
            // 
            // lvRip
            // 
            this.lvRip.AllowColumnReorder = true;
            this.lvRip.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvRip.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chNumber,
            this.chSize,
            this.chState});
            this.lvRip.ContextMenuStrip = this.cmsLV;
            this.lvRip.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lvRip.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvRip.Location = new System.Drawing.Point(1, 85);
            this.lvRip.Name = "lvRip";
            this.lvRip.ShowItemToolTips = true;
            this.lvRip.Size = new System.Drawing.Size(785, 380);
            this.lvRip.TabIndex = 30;
            this.lvRip.UseCompatibleStateImageBehavior = false;
            this.lvRip.View = System.Windows.Forms.View.Details;
            this.lvRip.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.lvRip.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
            // 
            // chName
            // 
            this.chName.DisplayIndex = 1;
            this.chName.Text = "Name";
            this.chName.Width = 77;
            // 
            // chNumber
            // 
            this.chNumber.DisplayIndex = 0;
            this.chNumber.Text = "#";
            this.chNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chNumber.Width = 30;
            // 
            // chSize
            // 
            this.chSize.Text = "Size";
            this.chSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chState
            // 
            this.chState.Text = "State";
            this.chState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chState.Width = 200;
            // 
            // cmsLV
            // 
            this.cmsLV.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmmiNextPage,
            this.cmmiDropGroup,
            this.cmmiCopyName,
            this.cmmiSaveAll,
            this.cmmiSave,
            this.cmmiRemove,
            this.cmmiClear});
            this.cmsLV.Name = "contextMenuStrip1";
            this.cmsLV.ShowCheckMargin = true;
            this.cmsLV.ShowImageMargin = false;
            this.cmsLV.ShowItemToolTips = false;
            this.cmsLV.Size = new System.Drawing.Size(118, 158);
            this.cmsLV.Opening += new System.ComponentModel.CancelEventHandler(this.cmsLV_Opening);
            // 
            // cmmiNextPage
            // 
            this.cmmiNextPage.Name = "cmmiNextPage";
            this.cmmiNextPage.Size = new System.Drawing.Size(117, 22);
            this.cmmiNextPage.Text = "Next";
            this.cmmiNextPage.Visible = false;
            this.cmmiNextPage.Click += new System.EventHandler(this.cmmiNextPage_Click);
            // 
            // cmmiDropGroup
            // 
            this.cmmiDropGroup.Name = "cmmiDropGroup";
            this.cmmiDropGroup.Size = new System.Drawing.Size(117, 22);
            this.cmmiDropGroup.Text = "Drop";
            this.cmmiDropGroup.Visible = false;
            this.cmmiDropGroup.Click += new System.EventHandler(this.cmmiDeleteAll_Click);
            // 
            // cmmiCopyName
            // 
            this.cmmiCopyName.Name = "cmmiCopyName";
            this.cmmiCopyName.Size = new System.Drawing.Size(117, 22);
            this.cmmiCopyName.Text = "Copy";
            this.cmmiCopyName.Visible = false;
            this.cmmiCopyName.Click += new System.EventHandler(this.cmmiCopyName_Click);
            // 
            // cmmiSaveAll
            // 
            this.cmmiSaveAll.CheckOnClick = true;
            this.cmmiSaveAll.Name = "cmmiSaveAll";
            this.cmmiSaveAll.Size = new System.Drawing.Size(117, 22);
            this.cmmiSaveAll.Text = "Small";
            this.cmmiSaveAll.Visible = false;
            // 
            // cmmiSave
            // 
            this.cmmiSave.Name = "cmmiSave";
            this.cmmiSave.Size = new System.Drawing.Size(117, 22);
            this.cmmiSave.Text = "Save";
            this.cmmiSave.Visible = false;
            this.cmmiSave.Click += new System.EventHandler(this.cmmiDownloadFile);
            // 
            // cmmiRemove
            // 
            this.cmmiRemove.Name = "cmmiRemove";
            this.cmmiRemove.Size = new System.Drawing.Size(117, 22);
            this.cmmiRemove.Text = "Remove";
            this.cmmiRemove.Visible = false;
            this.cmmiRemove.Click += new System.EventHandler(this.cmmiDeleteFile);
            // 
            // cmmiClear
            // 
            this.cmmiClear.Name = "cmmiClear";
            this.cmmiClear.Size = new System.Drawing.Size(117, 22);
            this.cmmiClear.Text = "Clear";
            this.cmmiClear.Visible = false;
            this.cmmiClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lblBatch,
            this.toolStripProgressBar1,
            this.CloudToolStrip});
            this.statusStrip1.Location = new System.Drawing.Point(1, 465);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(785, 22);
            this.statusStrip1.TabIndex = 22;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(41, 17);
            this.toolStripStatusLabel1.Text = "Done";
            // 
            // lblBatch
            // 
            this.lblBatch.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblBatch.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblBatch.Name = "lblBatch";
            this.lblBatch.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // CloudToolStrip
            // 
            this.CloudToolStrip.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.CloudToolStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CloudToolStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SkyDriveItem,
            this.FlickrItem,
            this.FacebookItem,
            this.PicasaItem});
            this.CloudToolStrip.Image = ((System.Drawing.Image)(resources.GetObject("CloudToolStrip.Image")));
            this.CloudToolStrip.Name = "CloudToolStrip";
            this.CloudToolStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CloudToolStrip.Size = new System.Drawing.Size(32, 20);
            this.CloudToolStrip.Text = "SkyDrive";
            this.CloudToolStrip.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.CloudToolStrip.ToolTipText = "Cloud web storage service.";
            this.CloudToolStrip.Click += new System.EventHandler(this.CloudToolStrip_Click);
            // 
            // SkyDriveItem
            // 
            this.SkyDriveItem.Image = ((System.Drawing.Image)(resources.GetObject("SkyDriveItem.Image")));
            this.SkyDriveItem.Name = "SkyDriveItem";
            this.SkyDriveItem.Size = new System.Drawing.Size(127, 22);
            this.SkyDriveItem.Text = "GDrive";
            this.SkyDriveItem.Click += new System.EventHandler(this.CloudItem_Click);
            // 
            // FlickrItem
            // 
            this.FlickrItem.Image = ((System.Drawing.Image)(resources.GetObject("FlickrItem.Image")));
            this.FlickrItem.Name = "FlickrItem";
            this.FlickrItem.Size = new System.Drawing.Size(127, 22);
            this.FlickrItem.Text = "Flickr";
            this.FlickrItem.Visible = false;
            this.FlickrItem.Click += new System.EventHandler(this.CloudItem_Click);
            // 
            // FacebookItem
            // 
            this.FacebookItem.Image = ((System.Drawing.Image)(resources.GetObject("FacebookItem.Image")));
            this.FacebookItem.Name = "FacebookItem";
            this.FacebookItem.Size = new System.Drawing.Size(127, 22);
            this.FacebookItem.Text = "Facebook";
            this.FacebookItem.Visible = false;
            this.FacebookItem.Click += new System.EventHandler(this.CloudItem_Click);
            // 
            // PicasaItem
            // 
            this.PicasaItem.Image = ((System.Drawing.Image)(resources.GetObject("PicasaItem.Image")));
            this.PicasaItem.Name = "PicasaItem";
            this.PicasaItem.Size = new System.Drawing.Size(127, 22);
            this.PicasaItem.Text = "Picasa";
            this.PicasaItem.Click += new System.EventHandler(this.CloudItem_Click);
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.lblUrl.Location = new System.Drawing.Point(4, 14);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(34, 19);
            this.lblUrl.TabIndex = 19;
            this.lblUrl.Text = "Url:";
            // 
            // tbDir
            // 
            this.tbDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDir.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbDir.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.tbDir.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ImgRipper.Properties.Settings.Default, "txtDir", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbDir.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.tbDir.Location = new System.Drawing.Point(43, 55);
            this.tbDir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbDir.Name = "tbDir";
            this.tbDir.Size = new System.Drawing.Size(608, 23);
            this.tbDir.TabIndex = 4;
            this.tbDir.TabStop = false;
            this.tbDir.Text = global::ImgRipper.Properties.Settings.Default.txtDir;
            // 
            // pbPreview
            // 
            this.pbPreview.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pbPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbPreview.InitialImage = null;
            this.pbPreview.Location = new System.Drawing.Point(0, 0);
            this.pbPreview.Margin = new System.Windows.Forms.Padding(0);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(96, 100);
            this.pbPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPreview.TabIndex = 22;
            this.pbPreview.TabStop = false;
            this.pbPreview.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // DownloadFiles
            // 
            this.DownloadFiles.WorkerReportsProgress = true;
            this.DownloadFiles.WorkerSupportsCancellation = true;
            this.DownloadFiles.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DownloadFiles_DoWork);
            this.DownloadFiles.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.DownloadFiles_ProgressChanged);
            this.DownloadFiles.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DownloadFiles_RunWorkerCompleted);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 300;
            this.toolTip1.ReshowDelay = 100;
            // 
            // tmMinus
            // 
            this.tmMinus.Tick += new System.EventHandler(this.btnMinus_Click);
            // 
            // tmPlus
            // 
            this.tmPlus.Tick += new System.EventHandler(this.btnPlus_Click);
            // 
            // Ripper
            // 
            this.AcceptButton = this.btnDownloadCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 488);
            this.Controls.Add(this.mainSplit);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(450, 300);
            this.Name = "Ripper";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Ripper - Designed by Wyvern";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Ripper_KeyDown);
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel1.PerformLayout();
            this.mainSplit.Panel2.ResumeLayout(false);
            this.mainSplit.ResumeLayout(false);
            this.cmsBatch.ResumeLayout(false);
            this.cmsLV.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Wyvern.SplitButton btnDownloadCancel;
        private System.Windows.Forms.TextBox tbDir;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        internal System.ComponentModel.BackgroundWorker DownloadFiles;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.TextBox tbParse;
        private System.Windows.Forms.PictureBox pbPreview;
        private System.Windows.Forms.SplitContainer mainSplit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer tmMinus;
        private System.Windows.Forms.Timer tmPlus;
        private System.Windows.Forms.ContextMenuStrip cmsLV;
        private System.Windows.Forms.ToolStripMenuItem cmmiNextPage;
        private System.Windows.Forms.ToolStripMenuItem cmmiSaveAll;
        private System.Windows.Forms.ToolStripMenuItem cmmiDropGroup;
        private System.Windows.Forms.ToolStripStatusLabel lblBatch;
        private System.Windows.Forms.ContextMenuStrip cmsBatch;
        private System.Windows.Forms.ToolStripMenuItem btnBatch;
        private System.Windows.Forms.ToolStripMenuItem cmmiClear;
        private System.Windows.Forms.ListView lvRip;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chSize;
        private System.Windows.Forms.ColumnHeader chState;
        private System.Windows.Forms.ToolStripMenuItem cmmiCopyName;
        private System.Windows.Forms.LinkLabel llFolder;
        private System.Windows.Forms.LinkLabel llCookie;
        private System.Windows.Forms.ToolStripMenuItem cmmiSave;
        private System.Windows.Forms.ToolStripMenuItem cmmiRemove;
        private System.Windows.Forms.ToolStripSplitButton CloudToolStrip;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripMenuItem SkyDriveItem;
        private System.Windows.Forms.ToolStripMenuItem FlickrItem;
        private System.Windows.Forms.ToolStripMenuItem FacebookItem;
        private System.Windows.Forms.ToolStripMenuItem PicasaItem;
        private System.Windows.Forms.ColumnHeader chNumber;
    }
}

