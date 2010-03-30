namespace ImgRipper
{
    using System;
    using System.Windows.Forms;

    using ImgRipper.Properties;

    partial class Batch : Form
    {
        int _seed;

        public Batch(int seed)
        {
            InitializeComponent();
            _seed = seed;
            udFrom.Value = udTo.Value = seed;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            var ripper = this.Owner as Ripper;
            if (udFrom.Value > 0 && udFrom.Value < udTo.Value)
            {
                ripper.From = decimal.ToInt32(udFrom.Value); ripper.To = decimal.ToInt32(udTo.Value);
                ripper.Address = new Uri(ripper.Address.AbsoluteUri.Replace(_seed.ToString(), ripper.From.ToString()));
                ripper.Batch = true;
                ripper.Range = ripper.To - ripper.From + 1;
                Close();
            }
            else
            {
                ToolTip tt = new ToolTip();
                tt.IsBalloon = true;
                tt.ToolTipIcon = ToolTipIcon.Warning;
                tt.ToolTipTitle = "Invalid Range";
                tt.Show("Out of range values", lblCaption, 2000);
            }
        }
    }
}
