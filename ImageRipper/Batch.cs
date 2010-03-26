namespace ImgRipper
{
    using System;
    using System.Windows.Forms;

    partial class BatchAction : Form
    {
        public BatchAction(int seed)
        {
            InitializeComponent();
            udFrom.Value = udTo.Value = seed;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            //Do batch download actions.
            Ripper ripper = this.Owner as Ripper;
            if (udFrom.Value > 0 && udFrom.Value < udTo.Value)
            {
                ripper.From = Convert.ToInt32(udFrom.Value); ripper.To = Convert.ToInt32(udTo.Value);
                ripper.Range = ripper.To - ripper.From + 1;
                ripper.Address = new Uri(ripper.Address.AbsoluteUri.Replace(ripper.Address.Query, "?thread=" + ripper.From));
                ripper.Batch = true;
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
