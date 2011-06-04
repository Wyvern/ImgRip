namespace ImgRip
{
    partial class SetCookie
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
            this.lblCookie = new System.Windows.Forms.Label();
            this.tbCookie = new System.Windows.Forms.TextBox();
            this.btnSummit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCookie
            // 
            this.lblCookie.AutoSize = true;
            this.lblCookie.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblCookie.Location = new System.Drawing.Point(41, 21);
            this.lblCookie.Name = "lblCookie";
            this.lblCookie.Size = new System.Drawing.Size(302, 26);
            this.lblCookie.TabIndex = 0;
            this.lblCookie.Text = "Get Cookie from your browser";
            // 
            // tbCookie
            // 
            this.tbCookie.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ImgRip.Properties.Settings.Default, "Cookie", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbCookie.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbCookie.Location = new System.Drawing.Point(12, 65);
            this.tbCookie.Name = "tbCookie";
            this.tbCookie.Size = new System.Drawing.Size(360, 29);
            this.tbCookie.TabIndex = 1;
            this.tbCookie.Text = global::ImgRip.Properties.Settings.Default.Cookie;
            // 
            // btnSummit
            // 
            this.btnSummit.ForeColor = System.Drawing.SystemColors.Highlight;
            this.btnSummit.Location = new System.Drawing.Point(141, 107);
            this.btnSummit.Name = "btnSummit";
            this.btnSummit.Size = new System.Drawing.Size(103, 40);
            this.btnSummit.TabIndex = 2;
            this.btnSummit.Text = "Summit";
            this.btnSummit.UseVisualStyleBackColor = true;
            this.btnSummit.Click += new System.EventHandler(this.button1_Click);
            // 
            // SetCookie
            // 
            this.AcceptButton = this.btnSummit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(384, 162);
            this.Controls.Add(this.btnSummit);
            this.Controls.Add(this.tbCookie);
            this.Controls.Add(this.lblCookie);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.Color.Green;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetCookie";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Cookie";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SetCookie_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCookie;
        private System.Windows.Forms.TextBox tbCookie;
        private System.Windows.Forms.Button btnSummit;
    }
}