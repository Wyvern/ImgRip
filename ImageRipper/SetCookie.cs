namespace ImgRipper
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    partial class SetCookie : Form
    {
        public SetCookie(string cookie)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(cookie))
                tbCookie.Text = cookie;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Ripper rip = Owner as Ripper;
            rip.Cookie = tbCookie.Text.Trim();
            Close();
        }
    }
}
