namespace ImgRipper
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Forms;

    partial class Sites : Form
    {
        public Sites()
        {
            InitializeComponent();
        }

        void item_MouseHover(object sender, System.EventArgs e)
        {
            var ll = sender as LinkLabel;
            ttSite.Show(ll.Tag as string, ll);
        }

        void item_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(((LinkLabel)sender).Tag as string);
        }

        private void Sites_Load(object sender, System.EventArgs e)
        {
            var Sites = new Dictionary<string, string>() {
            { "Pics100", "http://pics100.net" }, { "Duide", "http://duide.com" }, { "Tu11", "http://tu11.cc" }, {"Tu25","http://www.tu25.com"},
            { "MeiTuiJi", "http://meituiji.com" }, { "PalAthCx", "http://pal.ath.cx" }, { "DeskCityCom", "http://deskcity.com" }, 
            { "Heels", "http://www.heels.cn/web/index" }, { "WallCoo", "http://www.wallcoo.net" },  { "MeiTu", "http://www.meitushow.com" },
            { "Mtswa", "http://www.mtswa.com" },  { "Ttmnw", "http://www.ttmnw.com" }, {"6S8","http://www.6s8.net/meinv"},
            { "Xyuba", "http://www.xyuba.com" }, { "Yzmnw", "http://www.yzmnw.com" }, { "GirlCity", "http://www.girlcity.cn" }, 
            { "DeskCityCn", "http://www.deskcity.cn" }, { "China016", "http://www.china016.com" }, {"6188","http://www.6188.net"},
            { "169PP", "http://www.169pp.com" },{"1000RT","http://www.1000rt.com"},{"77MeiTu","http://www.77meitu.com"}
            ,{"158KK","http://www.158kk.com"},{"920MM","http://www.920mm.com"},{"Voc","http://mm.voc.com.cn"}};
            foreach (var site in Sites)
            {
                var item = new LinkLabel() { Text = site.Key, Tag = site.Value, Margin = new Padding(5) };
                item.LinkClicked += item_LinkClicked;
                item.MouseHover += item_MouseHover;
                flSites.Controls.Add(item);
            }
        }
    }
}
