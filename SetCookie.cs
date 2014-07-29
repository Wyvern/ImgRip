namespace ImgRip
{
    using System;
    using System.Windows.Forms;

    partial class SetCookie : Form
    {
        public SetCookie()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetCookie_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27) Close();
        }
    }
}
