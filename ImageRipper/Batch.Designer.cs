namespace ImgRipper
{
    partial class BatchAction
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
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblCaption = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.btnSet = new System.Windows.Forms.Button();
            this.udFrom = new System.Windows.Forms.NumericUpDown();
            this.udTo = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.udFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udTo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Font = new System.Drawing.Font("Microsoft YaHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFrom.ForeColor = System.Drawing.Color.Teal;
            this.lblFrom.Location = new System.Drawing.Point(5, 51);
            this.lblFrom.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(67, 26);
            this.lblFrom.TabIndex = 1;
            this.lblFrom.Text = "From:";
            // 
            // lblCaption
            // 
            this.lblCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCaption.AutoSize = true;
            this.lblCaption.Font = new System.Drawing.Font("Georgia", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCaption.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblCaption.Location = new System.Drawing.Point(6, 11);
            this.lblCaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(315, 23);
            this.lblCaption.TabIndex = 2;
            this.lblCaption.Text = "Please input the range of pages";
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Microsoft YaHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTo.ForeColor = System.Drawing.Color.Teal;
            this.lblTo.Location = new System.Drawing.Point(172, 51);
            this.lblTo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(41, 26);
            this.lblTo.TabIndex = 4;
            this.lblTo.Text = "To:";
            // 
            // btnSet
            // 
            this.btnSet.AutoSize = true;
            this.btnSet.Font = new System.Drawing.Font("Microsoft YaHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSet.ForeColor = System.Drawing.Color.Teal;
            this.btnSet.Location = new System.Drawing.Point(113, 94);
            this.btnSet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(100, 36);
            this.btnSet.TabIndex = 5;
            this.btnSet.Text = "Submit";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // udFrom
            // 
            this.udFrom.AutoSize = true;
            this.udFrom.Location = new System.Drawing.Point(72, 52);
            this.udFrom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.udFrom.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.udFrom.Name = "udFrom";
            this.udFrom.Size = new System.Drawing.Size(100, 29);
            this.udFrom.TabIndex = 7;
            this.udFrom.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // udTo
            // 
            this.udTo.AutoSize = true;
            this.udTo.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.udTo.Location = new System.Drawing.Point(213, 52);
            this.udTo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.udTo.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.udTo.Name = "udTo";
            this.udTo.Size = new System.Drawing.Size(100, 29);
            this.udTo.TabIndex = 8;
            this.udTo.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // BatchAction
            // 
            this.AcceptButton = this.btnSet;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(326, 142);
            this.Controls.Add(this.udTo);
            this.Controls.Add(this.udFrom);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.lblTo);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.lblFrom);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BatchAction";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batch Download";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.udFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udTo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.NumericUpDown udFrom;
        private System.Windows.Forms.NumericUpDown udTo;
    }
}