namespace ImgRip
{
    using System;
    using System.Windows.Forms;

    partial class Batch : Form
    {
        long _seed;

        public Batch(long seed)
        {
            InitializeComponent();
            _seed = seed;
            udFrom.Maximum = udTo.Maximum = long.MaxValue;
            udFrom.Value = udTo.Value = seed;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            var ripper = this.Owner as Main;
            if (udFrom.Value > 0 && udFrom.Value < udTo.Value)
            {
                ripper.From = decimal.ToInt32(udFrom.Value); ripper.To = decimal.ToInt32(udTo.Value);
                var n = _seed.ToString();
                var m = ripper.Address.LastIndexOf(n);
                ripper.Address = ripper.Address.Substring(0, m) + ripper.From.ToString() + ripper.Address.Substring(m + n.Length);
                ripper.Batch = true;
                ripper.Range = ripper.To - ripper.From + 1;
                Close();
            }
            else
            {
                var tt = new ToolTip();
                tt.IsBalloon = true;
                tt.ToolTipIcon = ToolTipIcon.Warning;
                tt.ToolTipTitle = "Invalid Range";
                tt.Show("Out of range values", lblCaption, 2000);
            }
        }
    }
}
