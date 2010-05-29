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
            this.flSites = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flSites
            // 
            this.flSites.AutoScroll = true;
            this.flSites.BackColor = System.Drawing.Color.Gainsboro;
            this.flSites.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flSites.Location = new System.Drawing.Point(9, 9);
            this.flSites.Name = "flSites";
            this.flSites.Size = new System.Drawing.Size(376, 154);
            this.flSites.TabIndex = 10;
            // 
            // Sites
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 172);
            this.Controls.Add(this.flSites);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Sites";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Support Sites";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flSites;

    }
}
