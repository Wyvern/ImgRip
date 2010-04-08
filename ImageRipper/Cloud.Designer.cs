namespace ImgRipper
{
    partial class WebCloud
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebCloud));
            this.lvCloud = new System.Windows.Forms.ListView();
            this.lvImageList = new System.Windows.Forms.ImageList(this.components);
            this.lblCloudID = new System.Windows.Forms.Label();
            this.lblPass = new System.Windows.Forms.Label();
            this.cldStatus = new System.Windows.Forms.StatusStrip();
            this.CloudStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtFolderName = new System.Windows.Forms.TextBox();
            this.gpLogin = new System.Windows.Forms.GroupBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btnSign = new System.Windows.Forms.Button();
            this.tbPass = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.CmdPanel = new System.Windows.Forms.Panel();
            this.cbPublic = new System.Windows.Forms.CheckBox();
            this.ttCloud = new System.Windows.Forms.ToolTip(this.components);
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.cldStatus.SuspendLayout();
            this.gpLogin.SuspendLayout();
            this.CmdPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvCloud
            // 
            this.lvCloud.AllowDrop = true;
            this.lvCloud.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvCloud.ForeColor = System.Drawing.Color.Blue;
            this.lvCloud.LabelEdit = true;
            this.lvCloud.Location = new System.Drawing.Point(0, 118);
            this.lvCloud.Name = "lvCloud";
            this.lvCloud.ShowItemToolTips = true;
            this.lvCloud.Size = new System.Drawing.Size(627, 320);
            this.lvCloud.SmallImageList = this.lvImageList;
            this.lvCloud.TabIndex = 1;
            this.lvCloud.UseCompatibleStateImageBehavior = false;
            this.lvCloud.View = System.Windows.Forms.View.List;
            this.lvCloud.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lvCloud_AfterLabelEdit);
            this.lvCloud.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvCloud_ItemSelectionChanged);
            this.lvCloud.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvCloud_DragDrop);
            this.lvCloud.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvCloud_DragEnter);
            this.lvCloud.Enter += new System.EventHandler(this.lvCloud_Enter);
            this.lvCloud.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvCloud_KeyDown);
            this.lvCloud.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvCloud_MouseClick);
            this.lvCloud.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvCloud_MouseDoubleClick);
            // 
            // lvImageList
            // 
            this.lvImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("lvImageList.ImageStream")));
            this.lvImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.lvImageList.Images.SetKeyName(0, "Folder");
            this.lvImageList.Images.SetKeyName(1, "Image");
            this.lvImageList.Images.SetKeyName(2, "Document");
            // 
            // lblCloudID
            // 
            this.lblCloudID.AutoSize = true;
            this.lblCloudID.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.lblCloudID.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblCloudID.Location = new System.Drawing.Point(4, 24);
            this.lblCloudID.Name = "lblCloudID";
            this.lblCloudID.Size = new System.Drawing.Size(51, 19);
            this.lblCloudID.TabIndex = 3;
            this.lblCloudID.Text = "Name";
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.lblPass.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblPass.Location = new System.Drawing.Point(4, 63);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(76, 19);
            this.lblPass.TabIndex = 4;
            this.lblPass.Text = "Password";
            // 
            // cldStatus
            // 
            this.cldStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cldStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CloudStatus});
            this.cldStatus.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.cldStatus.Location = new System.Drawing.Point(0, 438);
            this.cldStatus.Name = "cldStatus";
            this.cldStatus.Size = new System.Drawing.Size(627, 24);
            this.cldStatus.TabIndex = 8;
            this.cldStatus.Text = "statusStrip1";
            // 
            // CloudStatus
            // 
            this.CloudStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.CloudStatus.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Bold);
            this.CloudStatus.ForeColor = System.Drawing.SystemColors.Highlight;
            this.CloudStatus.Name = "CloudStatus";
            this.CloudStatus.Size = new System.Drawing.Size(44, 19);
            this.CloudStatus.Text = "Done";
            // 
            // txtFolderName
            // 
            this.txtFolderName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolderName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFolderName.Enabled = false;
            this.txtFolderName.Location = new System.Drawing.Point(427, 80);
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.Size = new System.Drawing.Size(178, 25);
            this.txtFolderName.TabIndex = 12;
            this.ttCloud.SetToolTip(this.txtFolderName, "Type to search or add new album");
            this.txtFolderName.TextChanged += new System.EventHandler(this.txtFolderName_TextChanged);
            this.txtFolderName.Enter += new System.EventHandler(this.txtFolderName_Enter);
            this.txtFolderName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFolderName_KeyPress);
            // 
            // gpLogin
            // 
            this.gpLogin.Controls.Add(this.tbName);
            this.gpLogin.Controls.Add(this.btnSign);
            this.gpLogin.Controls.Add(this.lblCloudID);
            this.gpLogin.Controls.Add(this.lblPass);
            this.gpLogin.Controls.Add(this.tbPass);
            this.gpLogin.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.gpLogin.ForeColor = System.Drawing.Color.BlueViolet;
            this.gpLogin.Location = new System.Drawing.Point(12, 5);
            this.gpLogin.Name = "gpLogin";
            this.gpLogin.Size = new System.Drawing.Size(347, 100);
            this.gpLogin.TabIndex = 13;
            this.gpLogin.TabStop = false;
            this.gpLogin.Text = "Login";
            // 
            // tbName
            // 
            this.tbName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ImgRipper.Properties.Settings.Default, "CloudID", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbName.Location = new System.Drawing.Point(78, 21);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(194, 25);
            this.tbName.TabIndex = 0;
            this.tbName.Text = global::ImgRipper.Properties.Settings.Default.CloudID;
            // 
            // btnSign
            // 
            this.btnSign.AutoSize = true;
            this.btnSign.Font = new System.Drawing.Font("Microsoft YaHei", 10F);
            this.btnSign.Image = global::ImgRipper.Properties.Resources.Login;
            this.btnSign.Location = new System.Drawing.Point(278, 28);
            this.btnSign.Name = "btnSign";
            this.btnSign.Size = new System.Drawing.Size(56, 51);
            this.btnSign.TabIndex = 2;
            this.btnSign.UseVisualStyleBackColor = true;
            this.btnSign.Click += new System.EventHandler(this.ConnectCloud_Click);
            // 
            // tbPass
            // 
            this.tbPass.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ImgRipper.Properties.Settings.Default, "Password", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbPass.Location = new System.Drawing.Point(78, 60);
            this.tbPass.Name = "tbPass";
            this.tbPass.Size = new System.Drawing.Size(194, 25);
            this.tbPass.TabIndex = 1;
            this.tbPass.Text = global::ImgRipper.Properties.Settings.Default.Password;
            this.tbPass.UseSystemPasswordChar = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Image = global::ImgRipper.Properties.Resources.Files;
            this.btnAdd.Location = new System.Drawing.Point(121, 9);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(50, 50);
            this.btnAdd.TabIndex = 11;
            this.ttCloud.SetToolTip(this.btnAdd, "Add Photos");
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAddFiles_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Enabled = false;
            this.btnCreate.Image = global::ImgRipper.Properties.Resources.New;
            this.btnCreate.Location = new System.Drawing.Point(180, 9);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(50, 50);
            this.btnCreate.TabIndex = 10;
            this.ttCloud.SetToolTip(this.btnCreate, "Create Album");
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreateFolder_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Image = global::ImgRipper.Properties.Resources.Delete;
            this.btnDelete.Location = new System.Drawing.Point(62, 9);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(50, 50);
            this.btnDelete.TabIndex = 9;
            this.ttCloud.SetToolTip(this.btnDelete, "Delete Items");
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnUp
            // 
            this.btnUp.Enabled = false;
            this.btnUp.Font = new System.Drawing.Font("Microsoft YaHei", 10F);
            this.btnUp.Image = global::ImgRipper.Properties.Resources.Up;
            this.btnUp.Location = new System.Drawing.Point(3, 9);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(50, 50);
            this.btnUp.TabIndex = 7;
            this.ttCloud.SetToolTip(this.btnUp, "Up to folder");
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // CmdPanel
            // 
            this.CmdPanel.AutoSize = true;
            this.CmdPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CmdPanel.Controls.Add(this.btnUp);
            this.CmdPanel.Controls.Add(this.btnDelete);
            this.CmdPanel.Controls.Add(this.btnCreate);
            this.CmdPanel.Controls.Add(this.btnAdd);
            this.CmdPanel.Location = new System.Drawing.Point(372, 5);
            this.CmdPanel.Name = "CmdPanel";
            this.CmdPanel.Size = new System.Drawing.Size(233, 62);
            this.CmdPanel.TabIndex = 14;
            // 
            // cbPublic
            // 
            this.cbPublic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbPublic.Enabled = false;
            this.cbPublic.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.cbPublic.Image = ((System.Drawing.Image)(resources.GetObject("cbPublic.Image")));
            this.cbPublic.Location = new System.Drawing.Point(372, 71);
            this.cbPublic.Name = "cbPublic";
            this.cbPublic.Size = new System.Drawing.Size(48, 42);
            this.cbPublic.TabIndex = 13;
            this.cbPublic.ThreeState = true;
            this.ttCloud.SetToolTip(this.cbPublic, "(Shift+) F4 to share ON/OFF");
            this.cbPublic.UseVisualStyleBackColor = true;
            // 
            // ttCloud
            // 
            this.ttCloud.AutomaticDelay = 300;
            // 
            // ofd
            // 
            this.ofd.DefaultExt = "jpg";
            this.ofd.Filter = "JPG files|*.jpg|PNG files|*.png|BMP files|*.bmp|All files|*.*";
            this.ofd.InitialDirectory = global::ImgRipper.Properties.Settings.Default.txtDir;
            this.ofd.Multiselect = true;
            // 
            // WebCloud
            // 
            this.AcceptButton = this.btnSign;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(627, 462);
            this.Controls.Add(this.cbPublic);
            this.Controls.Add(this.CmdPanel);
            this.Controls.Add(this.gpLogin);
            this.Controls.Add(this.txtFolderName);
            this.Controls.Add(this.cldStatus);
            this.Controls.Add(this.lvCloud);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 10F);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(593, 470);
            this.Name = "WebCloud";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cloud";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WebCloud_FormClosed);
            this.Load += new System.EventHandler(this.WebCloud_Load);
            this.cldStatus.ResumeLayout(false);
            this.cldStatus.PerformLayout();
            this.gpLogin.ResumeLayout(false);
            this.gpLogin.PerformLayout();
            this.CmdPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvCloud;
        private System.Windows.Forms.Button btnSign;
        private System.Windows.Forms.Label lblCloudID;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbPass;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.StatusStrip cldStatus;
        private System.Windows.Forms.ToolStripStatusLabel CloudStatus;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtFolderName;
        private System.Windows.Forms.GroupBox gpLogin;
        private System.Windows.Forms.ImageList lvImageList;
        private System.Windows.Forms.Panel CmdPanel;
        private System.Windows.Forms.CheckBox cbPublic;
        private System.Windows.Forms.ToolTip ttCloud;
        private System.Windows.Forms.OpenFileDialog ofd;
    }
}