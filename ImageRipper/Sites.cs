namespace ImgRipper
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    partial class Sites : Form
    {
        public Sites()
        {
            InitializeComponent();
            var Sites = new Dictionary<string, string>() { { "Pics100", "http://pics100.net" }, { "Duide", "http://duide.com" }, { "Tu11", "http://tu11.cc" }, { "MeiTuiJi", "http://meituiji.com" }, { "PalAthCx", "http://pal.ath.cx" }, { "DeskCity", "http://deskcity.com" }, { "Heels", "http://www.heels.cn/web/index" }, { "WallCoo", "http://www.wallcoo.net" }};
            foreach (var site in Sites)
            {
                var item = new LinkLabel() { Text = site.Key, Font = new Font("Georgia", 15f), Tag = site.Value, Margin = new Padding(5) };
                item.LinkClicked += (s, e) => { var ll = s as LinkLabel; Process.Start(ll.Tag as string); };
                flSites.Controls.Add(item);
            }
        }
    }
}
