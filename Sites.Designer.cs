namespace ImgRipper
{
    partial class Sites
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.flSites = new System.Windows.Forms.FlowLayoutPanel();
            this.ttSite = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // flSites
            // 
            this.flSites.AutoScroll = true;
            this.flSites.AutoSize = true;
            this.flSites.BackColor = System.Drawing.Color.Lavender;
            this.flSites.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flSites.Font = new System.Drawing.Font("Georgia", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flSites.Location = new System.Drawing.Point(9, 9);
            this.flSites.Margin = new System.Windows.Forms.Padding(0);
            this.flSites.Name = "flSites";
            this.flSites.Size = new System.Drawing.Size(466, 247);
            this.flSites.TabIndex = 10;
            // 
            // ttSite
            // 
            this.ttSite.AutomaticDelay = 300;
            this.ttSite.AutoPopDelay = 5000;
            this.ttSite.InitialDelay = 300;
            this.ttSite.ReshowDelay = 0;
            // 
            // Sites
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.ClientSize = new System.Drawing.Size(484, 265);
            this.Controls.Add(this.flSites);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Sites";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Support Sites";
            this.Load += new System.EventHandler(this.Sites_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flSites;
        private System.Windows.Forms.ToolTip ttSite;

    }
}
