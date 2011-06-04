namespace ImgRip
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.llSites = new System.Windows.Forms.LinkLabel();
            this.btnGo = new ImgRip.SplitButton();
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
            this.cmmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.cmmiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.cmmiClear = new System.Windows.Forms.ToolStripMenuItem();
            this.cmmiBatch = new System.Windows.Forms.ToolStripMenuItem();
            this.cmmiPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.RipStatus = new System.Windows.Forms.StatusStrip();
            this.tsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbBatch = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsPB = new System.Windows.Forms.ToolStripProgressBar();
            this.tsCloud = new System.Windows.Forms.ToolStripSplitButton();
            this.tsiGData = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiPicasa = new System.Windows.Forms.ToolStripMenuItem();
            this.tsHome = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUrl = new System.Windows.Forms.Label();
            this.tbDir = new System.Windows.Forms.TextBox();
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.bwFetch = new System.ComponentModel.BackgroundWorker();
            this.ttRipper = new System.Windows.Forms.ToolTip(this.components);
            this.tmMinus = new System.Windows.Forms.Timer(this.components);
            this.tmPlus = new System.Windows.Forms.Timer(this.components);
            this.fbDir = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            this.cmsBatch.SuspendLayout();
            this.cmsLV.SuspendLayout();
            this.RipStatus.SuspendLayout();
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
            this.mainSplit.Panel1.Controls.Add(this.llSites);
            this.mainSplit.Panel1.Controls.Add(this.btnGo);
            this.mainSplit.Panel1.Controls.Add(this.tbParse);
            this.mainSplit.Panel1.Controls.Add(this.llCookie);
            this.mainSplit.Panel1.Controls.Add(this.llFolder);
            this.mainSplit.Panel1.Controls.Add(this.lvRip);
            this.mainSplit.Panel1.Controls.Add(this.RipStatus);
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
            // llSites
            // 
            this.llSites.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llSites.AutoSize = true;
            this.llSites.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.llSites.LinkColor = System.Drawing.SystemColors.Highlight;
            this.llSites.Location = new System.Drawing.Point(646, 14);
            this.llSites.Name = "llSites";
            this.llSites.Size = new System.Drawing.Size(42, 19);
            this.llSites.TabIndex = 33;
            this.llSites.TabStop = true;
            this.llSites.Text = "Sites";
            this.ttRipper.SetToolTip(this.llSites, "Support sites list");
            this.llSites.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llSites_LinkClicked);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnGo.ContextMenuStrip = this.cmsBatch;
            this.btnGo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnGo.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
            this.btnGo.Image = global::ImgRip.Properties.Resources.Download;
            this.btnGo.Location = new System.Drawing.Point(708, 5);
            this.btnGo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(71, 73);
            this.btnGo.TabIndex = 3;
            this.btnGo.TabStop = false;
            this.btnGo.Tag = "";
            this.ttRipper.SetToolTip(this.btnGo, "Start/Stop, Page Up/Down");
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.Go_Click);
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
            this.btnBatch.Image = global::ImgRip.Properties.Resources.Batch;
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
            this.tbParse.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbParse.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.tbParse.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ImgRip.Properties.Settings.Default, "Url", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbParse.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.tbParse.Location = new System.Drawing.Point(43, 13);
            this.tbParse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbParse.Name = "tbParse";
            this.tbParse.Size = new System.Drawing.Size(595, 23);
            this.tbParse.TabIndex = 20;
            this.tbParse.TabStop = false;
            this.tbParse.Text = global::ImgRip.Properties.Settings.Default.Url;
            this.tbParse.WordWrap = false;
            this.tbParse.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tb_MouseClick);
            this.tbParse.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbParse_DragDrop);
            this.tbParse.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbParse_DragEnter);
            this.tbParse.Leave += new System.EventHandler(this.tb_Leave);
            // 
            // llCookie
            // 
            this.llCookie.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llCookie.AutoSize = true;
            this.llCookie.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.llCookie.LinkColor = System.Drawing.SystemColors.Highlight;
            this.llCookie.Location = new System.Drawing.Point(646, 56);
            this.llCookie.Name = "llCookie";
            this.llCookie.Size = new System.Drawing.Size(56, 19);
            this.llCookie.TabIndex = 32;
            this.llCookie.TabStop = true;
            this.llCookie.Text = "Cookie";
            this.ttRipper.SetToolTip(this.llCookie, "Set Cookie value");
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
            this.lvRip.SelectedIndexChanged += new System.EventHandler(this.lvSelectedIndexChanged);
            this.lvRip.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvKeyDown);
            // 
            // chName
            // 
            this.chName.DisplayIndex = 1;
            this.chName.Text = "Name";
            this.chName.Width = 150;
            // 
            // chNumber
            // 
            this.chNumber.DisplayIndex = 0;
            this.chNumber.Text = "No.";
            this.chNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chNumber.Width = 40;
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
            this.chState.Width = 147;
            // 
            // cmsLV
            // 
            this.cmsLV.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmmiNextPage,
            this.cmmiDropGroup,
            this.cmmiCopyName,
            this.cmmiSave,
            this.cmmiRemove,
            this.cmmiClear,
            this.cmmiBatch,
            this.cmmiPreview});
            this.cmsLV.Name = "contextMenuStrip1";
            this.cmsLV.ShowCheckMargin = true;
            this.cmsLV.ShowImageMargin = false;
            this.cmsLV.ShowItemToolTips = false;
            this.cmsLV.Size = new System.Drawing.Size(153, 202);
            this.cmsLV.Opening += new System.ComponentModel.CancelEventHandler(this.cmsLV_Opening);
            // 
            // cmmiNextPage
            // 
            this.cmmiNextPage.Name = "cmmiNextPage";
            this.cmmiNextPage.Size = new System.Drawing.Size(152, 22);
            this.cmmiNextPage.Text = "Next";
            this.cmmiNextPage.Visible = false;
            this.cmmiNextPage.Click += new System.EventHandler(this.cmmiNextPage_Click);
            // 
            // cmmiDropGroup
            // 
            this.cmmiDropGroup.Name = "cmmiDropGroup";
            this.cmmiDropGroup.Size = new System.Drawing.Size(152, 22);
            this.cmmiDropGroup.Text = "Drop";
            this.cmmiDropGroup.Visible = false;
            this.cmmiDropGroup.Click += new System.EventHandler(this.cmmiDrop_Click);
            // 
            // cmmiCopyName
            // 
            this.cmmiCopyName.Name = "cmmiCopyName";
            this.cmmiCopyName.Size = new System.Drawing.Size(152, 22);
            this.cmmiCopyName.Text = "Copy";
            this.cmmiCopyName.Visible = false;
            this.cmmiCopyName.Click += new System.EventHandler(this.cmmiCopyName_Click);
            // 
            // cmmiSave
            // 
            this.cmmiSave.Name = "cmmiSave";
            this.cmmiSave.Size = new System.Drawing.Size(152, 22);
            this.cmmiSave.Text = "Save";
            this.cmmiSave.Visible = false;
            this.cmmiSave.Click += new System.EventHandler(this.cmmiDownloadFile);
            // 
            // cmmiRemove
            // 
            this.cmmiRemove.Name = "cmmiRemove";
            this.cmmiRemove.Size = new System.Drawing.Size(152, 22);
            this.cmmiRemove.Text = "Remove";
            this.cmmiRemove.Visible = false;
            this.cmmiRemove.Click += new System.EventHandler(this.cmmiDeleteFile);
            // 
            // cmmiClear
            // 
            this.cmmiClear.Name = "cmmiClear";
            this.cmmiClear.Size = new System.Drawing.Size(152, 22);
            this.cmmiClear.Text = "Clear";
            this.cmmiClear.Visible = false;
            this.cmmiClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // cmmiBatch
            // 
            this.cmmiBatch.CheckOnClick = true;
            this.cmmiBatch.Name = "cmmiBatch";
            this.cmmiBatch.Size = new System.Drawing.Size(152, 22);
            this.cmmiBatch.Text = "Batch";
            this.cmmiBatch.Visible = false;
            this.cmmiBatch.CheckedChanged += new System.EventHandler(this.cmmiBatch_CheckedChanged);
            // 
            // cmmiPreview
            // 
            this.cmmiPreview.Checked = true;
            this.cmmiPreview.CheckOnClick = true;
            this.cmmiPreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cmmiPreview.Name = "cmmiPreview";
            this.cmmiPreview.Size = new System.Drawing.Size(152, 22);
            this.cmmiPreview.Text = "Preview";
            // 
            // RipStatus
            // 
            this.RipStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RipStatus.AutoSize = false;
            this.RipStatus.Dock = System.Windows.Forms.DockStyle.None;
            this.RipStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.RipStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLabel,
            this.lbBatch,
            this.tsPB,
            this.tsCloud,
            this.tsHome});
            this.RipStatus.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.RipStatus.Location = new System.Drawing.Point(1, 465);
            this.RipStatus.Name = "RipStatus";
            this.RipStatus.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.RipStatus.ShowItemToolTips = true;
            this.RipStatus.Size = new System.Drawing.Size(785, 22);
            this.RipStatus.TabIndex = 22;
            // 
            // tsLabel
            // 
            this.tsLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsLabel.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.tsLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.tsLabel.Name = "tsLabel";
            this.tsLabel.Size = new System.Drawing.Size(41, 17);
            this.tsLabel.Text = "Done";
            // 
            // lbBatch
            // 
            this.lbBatch.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.lbBatch.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lbBatch.Name = "lbBatch";
            this.lbBatch.Size = new System.Drawing.Size(0, 17);
            // 
            // tsPB
            // 
            this.tsPB.Name = "tsPB";
            this.tsPB.Size = new System.Drawing.Size(100, 16);
            this.tsPB.Visible = false;
            // 
            // tsCloud
            // 
            this.tsCloud.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsCloud.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsCloud.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiGData,
            this.tsiPicasa});
            this.tsCloud.Image = ((System.Drawing.Image)(resources.GetObject("tsCloud.Image")));
            this.tsCloud.Name = "tsCloud";
            this.tsCloud.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tsCloud.Size = new System.Drawing.Size(32, 20);
            this.tsCloud.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsCloud.ToolTipText = "Cloud web storage service.";
            this.tsCloud.Click += new System.EventHandler(this.CloudToolStrip_Click);
            // 
            // tsiGData
            // 
            this.tsiGData.Image = ((System.Drawing.Image)(resources.GetObject("tsiGData.Image")));
            this.tsiGData.Name = "tsiGData";
            this.tsiGData.Size = new System.Drawing.Size(114, 22);
            this.tsiGData.Text = "GDrive";
            this.tsiGData.Click += new System.EventHandler(this.CloudItem_Click);
            // 
            // tsiPicasa
            // 
            this.tsiPicasa.Image = ((System.Drawing.Image)(resources.GetObject("tsiPicasa.Image")));
            this.tsiPicasa.Name = "tsiPicasa";
            this.tsiPicasa.Size = new System.Drawing.Size(114, 22);
            this.tsiPicasa.Text = "Picasa";
            this.tsiPicasa.Click += new System.EventHandler(this.CloudItem_Click);
            // 
            // tsHome
            // 
            this.tsHome.AutoToolTip = true;
            this.tsHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsHome.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tsHome.Image = ((System.Drawing.Image)(resources.GetObject("tsHome.Image")));
            this.tsHome.IsLink = true;
            this.tsHome.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.tsHome.Margin = new System.Windows.Forms.Padding(0, 3, 5, 2);
            this.tsHome.Name = "tsHome";
            this.tsHome.Size = new System.Drawing.Size(16, 17);
            this.tsHome.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.tsHome.ToolTipText = "Home at CodePlex";
            this.tsHome.Click += new System.EventHandler(this.tsHome_Click);
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
            this.tbDir.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ImgRip.Properties.Settings.Default, "Dir", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbDir.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.tbDir.Location = new System.Drawing.Point(43, 55);
            this.tbDir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbDir.Name = "tbDir";
            this.tbDir.Size = new System.Drawing.Size(595, 23);
            this.tbDir.TabIndex = 4;
            this.tbDir.TabStop = false;
            this.tbDir.Text = global::ImgRip.Properties.Settings.Default.Dir;
            this.tbDir.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tb_MouseClick);
            this.tbDir.Leave += new System.EventHandler(this.tb_Leave);
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
            this.pbPreview.DoubleClick += new System.EventHandler(this.pbDoubleClick);
            // 
            // bwFetch
            // 
            this.bwFetch.WorkerReportsProgress = true;
            this.bwFetch.WorkerSupportsCancellation = true;
            this.bwFetch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Fetch_DoWork);
            this.bwFetch.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.DownloadFiles_ProgressChanged);
            this.bwFetch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DownloadFiles_RunWorkerCompleted);
            // 
            // ttRipper
            // 
            this.ttRipper.AutomaticDelay = 300;
            // 
            // tmMinus
            // 
            this.tmMinus.Tick += new System.EventHandler(this.btnMinus_Click);
            // 
            // tmPlus
            // 
            this.tmPlus.Tick += new System.EventHandler(this.btnPlus_Click);
            // 
            // fbDir
            // 
            this.fbDir.Description = "Select folder to store files";
            this.fbDir.SelectedPath = global::ImgRip.Properties.Settings.Default.Dir;
            // 
            // Main
            // 
            this.AcceptButton = this.btnGo;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnGo;
            this.ClientSize = new System.Drawing.Size(787, 488);
            this.Controls.Add(this.mainSplit);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(450, 300);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "F11: FullScreen,  [Shift+] Space: Preview ON|OFF";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Ripper_KeyDown);
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel1.PerformLayout();
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            this.cmsBatch.ResumeLayout(false);
            this.cmsLV.ResumeLayout(false);
            this.RipStatus.ResumeLayout(false);
            this.RipStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SplitButton btnGo;
        private System.Windows.Forms.TextBox tbDir;
        private System.Windows.Forms.FolderBrowserDialog fbDir;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.TextBox tbParse;
        private System.Windows.Forms.PictureBox pbPreview;
        private System.Windows.Forms.SplitContainer mainSplit;
        private System.Windows.Forms.StatusStrip RipStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsLabel;
        private System.Windows.Forms.ToolTip ttRipper;
        private System.Windows.Forms.Timer tmMinus;
        private System.Windows.Forms.Timer tmPlus;
        private System.Windows.Forms.ContextMenuStrip cmsLV;
        private System.Windows.Forms.ToolStripMenuItem cmmiNextPage;
        private System.Windows.Forms.ToolStripMenuItem cmmiDropGroup;
        private System.Windows.Forms.ToolStripStatusLabel lbBatch;
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
        private System.Windows.Forms.ToolStripSplitButton tsCloud;
        private System.Windows.Forms.ToolStripProgressBar tsPB;
        private System.Windows.Forms.ToolStripMenuItem tsiGData;
        private System.Windows.Forms.ToolStripMenuItem tsiPicasa;
        private System.Windows.Forms.ColumnHeader chNumber;
        private System.Windows.Forms.ToolStripStatusLabel tsHome;
        private System.ComponentModel.BackgroundWorker bwFetch;
        private System.Windows.Forms.LinkLabel llSites;
        private System.Windows.Forms.ToolStripMenuItem cmmiBatch;
        private System.Windows.Forms.ToolStripMenuItem cmmiPreview;
    }
}

