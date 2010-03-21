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
            this.lblLivePass = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.CloudStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtFolderName = new System.Windows.Forms.TextBox();
            this.gpLogin = new System.Windows.Forms.GroupBox();
            this.loginName = new System.Windows.Forms.TextBox();
            this.btnSign = new System.Windows.Forms.Button();
            this.loginPass = new System.Windows.Forms.TextBox();
            this.btnAddFiles = new System.Windows.Forms.Button();
            this.btnCreateFolder = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.CommandPanel = new System.Windows.Forms.Panel();
            this.cbPublic = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.gpLogin.SuspendLayout();
            this.CommandPanel.SuspendLayout();
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
            this.lvCloud.Location = new System.Drawing.Point(12, 118);
            this.lvCloud.Name = "lvCloud";
            this.lvCloud.ShowItemToolTips = true;
            this.lvCloud.Size = new System.Drawing.Size(553, 290);
            this.lvCloud.SmallImageList = this.lvImageList;
            this.lvCloud.TabIndex = 1;
            this.lvCloud.UseCompatibleStateImageBehavior = false;
            this.lvCloud.View = System.Windows.Forms.View.List;
            this.lvCloud.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lvCloud_AfterLabelEdit);
            this.lvCloud.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvCloud_ItemSelectionChanged);
            this.lvCloud.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvCloud_DragDrop);
            this.lvCloud.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvCloud_DragEnter);
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
            // lblLivePass
            // 
            this.lblLivePass.AutoSize = true;
            this.lblLivePass.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.lblLivePass.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblLivePass.Location = new System.Drawing.Point(4, 63);
            this.lblLivePass.Name = "lblLivePass";
            this.lblLivePass.Size = new System.Drawing.Size(76, 19);
            this.lblLivePass.TabIndex = 4;
            this.lblLivePass.Text = "Password";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CloudStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 408);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(577, 24);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
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
            this.txtFolderName.Location = new System.Drawing.Point(399, 76);
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.Size = new System.Drawing.Size(166, 25);
            this.txtFolderName.TabIndex = 12;
            this.txtFolderName.TextChanged += new System.EventHandler(this.txtFolderName_TextChanged);
            this.txtFolderName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFolderName_KeyPress);
            // 
            // gpLogin
            // 
            this.gpLogin.Controls.Add(this.loginName);
            this.gpLogin.Controls.Add(this.btnSign);
            this.gpLogin.Controls.Add(this.lblCloudID);
            this.gpLogin.Controls.Add(this.lblLivePass);
            this.gpLogin.Controls.Add(this.loginPass);
            this.gpLogin.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.gpLogin.ForeColor = System.Drawing.Color.BlueViolet;
            this.gpLogin.Location = new System.Drawing.Point(12, 5);
            this.gpLogin.Name = "gpLogin";
            this.gpLogin.Size = new System.Drawing.Size(319, 107);
            this.gpLogin.TabIndex = 13;
            this.gpLogin.TabStop = false;
            this.gpLogin.Text = "Login";
            // 
            // loginName
            // 
            this.loginName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ImgRipper.Properties.Settings.Default, "CloudID", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.loginName.Location = new System.Drawing.Point(78, 21);
            this.loginName.Name = "loginName";
            this.loginName.Size = new System.Drawing.Size(174, 25);
            this.loginName.TabIndex = 0;
            this.loginName.Text = global::ImgRipper.Properties.Settings.Default.CloudID;
            // 
            // btnSign
            // 
            this.btnSign.AutoSize = true;
            this.btnSign.Font = new System.Drawing.Font("Microsoft YaHei", 10F);
            this.btnSign.Image = global::ImgRipper.Properties.Resources.Login;
            this.btnSign.Location = new System.Drawing.Point(258, 28);
            this.btnSign.Name = "btnSign";
            this.btnSign.Size = new System.Drawing.Size(56, 51);
            this.btnSign.TabIndex = 2;
            this.btnSign.UseVisualStyleBackColor = true;
            this.btnSign.Click += new System.EventHandler(this.ConnectCloud_Click);
            // 
            // loginPass
            // 
            this.loginPass.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ImgRipper.Properties.Settings.Default, "Password", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.loginPass.Location = new System.Drawing.Point(78, 60);
            this.loginPass.Name = "loginPass";
            this.loginPass.Size = new System.Drawing.Size(174, 25);
            this.loginPass.TabIndex = 1;
            this.loginPass.Text = global::ImgRipper.Properties.Settings.Default.Password;
            this.loginPass.UseSystemPasswordChar = true;
            // 
            // btnAddFiles
            // 
            this.btnAddFiles.Enabled = false;
            this.btnAddFiles.Image = global::ImgRipper.Properties.Resources.Files;
            this.btnAddFiles.Location = new System.Drawing.Point(121, 11);
            this.btnAddFiles.Name = "btnAddFiles";
            this.btnAddFiles.Size = new System.Drawing.Size(50, 50);
            this.btnAddFiles.TabIndex = 11;
            this.btnAddFiles.UseVisualStyleBackColor = true;
            this.btnAddFiles.Click += new System.EventHandler(this.btnAddFiles_Click);
            // 
            // btnCreateFolder
            // 
            this.btnCreateFolder.Enabled = false;
            this.btnCreateFolder.Image = global::ImgRipper.Properties.Resources.New;
            this.btnCreateFolder.Location = new System.Drawing.Point(180, 9);
            this.btnCreateFolder.Name = "btnCreateFolder";
            this.btnCreateFolder.Size = new System.Drawing.Size(50, 50);
            this.btnCreateFolder.TabIndex = 10;
            this.btnCreateFolder.UseVisualStyleBackColor = true;
            this.btnCreateFolder.Click += new System.EventHandler(this.btnCreateFolder_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Image = global::ImgRipper.Properties.Resources.Delete;
            this.btnDelete.Location = new System.Drawing.Point(62, 9);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(50, 50);
            this.btnDelete.TabIndex = 9;
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
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // CommandPanel
            // 
            this.CommandPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CommandPanel.AutoSize = true;
            this.CommandPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CommandPanel.Controls.Add(this.btnUp);
            this.CommandPanel.Controls.Add(this.btnDelete);
            this.CommandPanel.Controls.Add(this.btnCreateFolder);
            this.CommandPanel.Controls.Add(this.btnAddFiles);
            this.CommandPanel.Location = new System.Drawing.Point(332, 4);
            this.CommandPanel.Name = "CommandPanel";
            this.CommandPanel.Size = new System.Drawing.Size(233, 64);
            this.CommandPanel.TabIndex = 14;
            // 
            // cbPublic
            // 
            this.cbPublic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbPublic.Enabled = false;
            this.cbPublic.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this.cbPublic.Image = ((System.Drawing.Image)(resources.GetObject("cbPublic.Image")));
            this.cbPublic.Location = new System.Drawing.Point(332, 62);
            this.cbPublic.Name = "cbPublic";
            this.cbPublic.Size = new System.Drawing.Size(58, 55);
            this.cbPublic.TabIndex = 13;
            this.cbPublic.UseVisualStyleBackColor = true;
            // 
            // WebCloud
            // 
            this.AcceptButton = this.btnSign;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(577, 432);
            this.Controls.Add(this.cbPublic);
            this.Controls.Add(this.CommandPanel);
            this.Controls.Add(this.gpLogin);
            this.Controls.Add(this.txtFolderName);
            this.Controls.Add(this.statusStrip1);
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
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gpLogin.ResumeLayout(false);
            this.gpLogin.PerformLayout();
            this.CommandPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvCloud;
        private System.Windows.Forms.Button btnSign;
        private System.Windows.Forms.Label lblCloudID;
        private System.Windows.Forms.Label lblLivePass;
        private System.Windows.Forms.TextBox loginName;
        private System.Windows.Forms.TextBox loginPass;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel CloudStatus;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCreateFolder;
        private System.Windows.Forms.Button btnAddFiles;
        private System.Windows.Forms.TextBox txtFolderName;
        private System.Windows.Forms.GroupBox gpLogin;
        private System.Windows.Forms.ImageList lvImageList;
        private System.Windows.Forms.Panel CommandPanel;
        private System.Windows.Forms.CheckBox cbPublic;
    }
}