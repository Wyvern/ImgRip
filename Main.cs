namespace ImgRipper
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using System.Web;

    using ImgRipper.Properties;
    using HAP = HtmlAgilityPack;

    partial class Main : Form
    {
        Fetcher rip = new Fetcher();
        bool FullScreen { get; set; }
        internal int Range { get; set; }
        internal bool Batch { get; set; }

        public Main()
        {
            InitializeComponent();
            tsHome.Alignment = ToolStripItemAlignment.Right;
            btnDownloadCancel.UpArrowMouseUp += (s, e) => tmPlus.Enabled = false;
            btnDownloadCancel.DownArrowMouseUp += (s, e) => tmMinus.Enabled = false;
            btnDownloadCancel.UpArrowMouseDown += (s, e) =>
            {
                if ((rip.Style = CheckUrl(Address)) == ParseStyle.NotSupport) return;
                if (tmMinus.Enabled) tmMinus.Enabled = false;
                tmPlus.Enabled = true;
            };
            btnDownloadCancel.DownArrowMouseDown += (s, e) =>
            {
                if ((rip.Style = CheckUrl(Address)) == ParseStyle.NotSupport) return;
                if (tmPlus.Enabled) tmPlus.Enabled = false;
                tmMinus.Enabled = true;
            };
        }

        /// <summary>
        /// UI Callback properties
        /// </summary>
        bool pbToggle { set { RipStatus.Invoke(new Action(() => tsPB.Visible = value)); } }
        string Prompt { set { RipStatus.Invoke(new Action(() => tsLabel.Text = value)); } }
        int pbSet { set { RipStatus.Invoke(new Action(() => tsPB.Value = value)); } }
        string[] LviUpdate
        {
            set
            {
                lvRip.Invoke(new Action(() =>
                    {
                        //No. 1, Name 0, Size 2, State 3
                        string name = value[0], number = value[1], size = value[2], state = value[3];
                        var lvi = lvRip.FindItemWithText(name);
                        #region Update existed item
                        if (lvi != null)
                        {
                            if (!string.IsNullOrEmpty(number)) lvi.SubItems[1].Text = number;
                            if (!string.IsNullOrEmpty(size)) lvi.SubItems[2].Text = size;
                            if (!string.IsNullOrEmpty(state)) lvi.SubItems[3].Text = state;
                            if (lvi.Font.Style != FontStyle.Regular)
                                lvi.Font = new Font(lvi.Font, FontStyle.Regular);
                        }
                        #endregion
                        #region Add new item to group
                        else
                        {
                            lvi = new ListViewItem(value);
                            lvi.ToolTipText = rip.Address;
                            if (lvRip.Groups[rip.Title] == null) lvRip.Groups.Add(rip.Title, string.Format("{0} of [{1}P]", rip.Title, rip.Imgs.Count));
                            lvi.Group = lvRip.Groups[rip.Title];
                            lvRip.Items.Add(lvi).EnsureVisible();
                            lvi.ForeColor = lvi.Index % 2 == 0 ?Color.ForestGreen:Color.RoyalBlue;
                        }
                        #endregion
                    }));
            }
        }

        internal int From { get; set; }
        internal int To { get; set; }

        internal string Dir
        {
            get { return tbDir.Text; }
            set { tbDir.Text = value; }
        }

        public string Address
        {
            get { return tbParse.Text; }
            set { tbParse.Text = value; }
        }

        private void DownloadCancel_Click(object sender, EventArgs e)
        {
            switch (rip.PushState)
            {
                case RipperAction.Download:
                    if (!CanDownload) return;
                    tbParse.ReadOnly = true;
                    tbDir.ReadOnly = true;
                    Settings.Default.Save();
                    bwFetch.RunWorkerAsync();
                    ((Button)sender).Image = Resources.Cancel;
                    rip.PushState = RipperAction.Cancel;
                    break;
                case RipperAction.Cancel:
                    Batch = false;
                    rip.Canceled = true; rip.Cancel(); rip.NextPage = null;
                    tbParse.ReadOnly = false;
                    tbDir.ReadOnly = false;
                    bwFetch.CancelAsync();
                    ((Button)sender).Enabled = false;
                    break;
            }
        }

        private void ResetStatus()
        {
            Prompt = "Downloading...";
            pbSet = 0;
            pbToggle = true;
        }

        private void Fetch_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "HAP"))
            {
                var asm = AppDomain.CurrentDomain.Load(Resources.HAP);
                AppDomain.CurrentDomain.AssemblyResolve += (o, a) => a.Name == asm.FullName ? asm : null;
            }
            if ((e.Result = Parse(Address)) != null) return;
            if (rip.Canceled) { e.Result = "User Cancelled!"; rip.NextPage = null; return; }
            FetchFile(e);
        }

        private void FetchFile(DoWorkEventArgs e)
        {
            ResetStatus();
            if (rip.Imgs.Count == 0) e.Result = "Not a photo gallery!";
            for (int idx = 0; idx < rip.Imgs.Count; idx++)
            {
                rip.Address = rip.Imgs[idx];
                FileInfo fi = new FileInfo(Path.Combine(Dir, rip.Imgs.Keys[idx]));
                rip.Current = fi;
                //number 1, name 0, size 2, state 3
                var Order = (idx + 1).ToString();
                if (fi.Exists)
                {
                    LviUpdate = new[] { fi.Name, Order, fi.Length / 1024 + " KB", "Existed" };
                    continue;
                }
                LviUpdate = new[] { fi.Name, Order, null, "Downloading" };
                if (Batch) RipStatus.Invoke(new Action(() => lbBatch.Text = string.Format(" #{0}/{1} Pages", (Range - (To - From)), Range)));

                #region For Heels.cn
                if (rip.Style == ParseStyle.Heels)
                {
                    bool succeed = false;
                    if (string.IsNullOrEmpty(Settings.Default.Cookie))
                    {
                        this.Invoke(new Action(() =>
                        {
                            new SetCookie().ShowDialog(this);
                        }));
                    }
                    while (!succeed)
                    {
                        try
                        {
                            using (var s = rip.GetStream(rip.Address, Settings.Default.Cookie))
                            using (var bmp = Image.FromStream(s))
                            {
                                s.Close(); bmp.Save(fi.ToString());
                                bmp.Dispose();
                            }
                            succeed = true;
                        }
                        catch (Exception)
                        {
                            if (RipCheck(Order, e)) return;
                            LviUpdate = new[] { fi.Name, Order, null, "Check Cookie / Wait 5 secs" };
                            Thread.Sleep(5000);
                        }
                    }
                }
                #endregion

                #region For Others
                else
                    try
                    {
                        rip.GetFile(rip.Address, fi.ToString());
                        if (RipCheck(Order, e)) return;
                    }
                    catch (Exception exp)
                    {
                        LviUpdate = new[] { fi.Name, Order, null, exp.Message };
                    }
                #endregion
                fi.Refresh();
                if (!fi.Exists) { e.Result = "Download failed!"; LviUpdate = new[] { fi.Name, Order, null, "Failed" }; return; }
                if (mainSplit.Panel2Collapsed) mainSplit.Invoke(new Action(() => mainSplit.Panel2Collapsed = false));
                LviUpdate = new[] { fi.Name, Order, fi.Length / 1024 + " KB", "Finished"};
                pbPreview.ImageLocation = rip.ImageLocation = fi.ToString();
                bwFetch.ReportProgress((idx + 1) * 100 / rip.Imgs.Count);
            }
        }

        bool RipCheck(string order, DoWorkEventArgs e)
        {
            if (rip.Dropped) { rip.Dropped = false; LviUpdate = new[] { rip.Current.Name, order, null, "Dropped" }; return true; }
            if (rip.Canceled) { e.Cancel = true; rip.NextPage = null; LviUpdate = new[] { rip.Current.Name, order, null, "Cancelled" }; return true; }
            if (rip.SkipPage)
            {
                rip.SkipPage = false;
                LviUpdate = new[] { rip.Current.Name, order, null, "Skipped" };
                return true;
            }
            return false;
        }

        private ParseStyle CheckUrl(string address)
        {
            Uri Url;
            try
            {
                Url = new Uri(address);
            }
            catch (System.UriFormatException format)
            {
                tsLabel.Text = format.Message;
                return ParseStyle.NotSupport;
            }
            string host = Url.Host.ToLower();
            if (host.Contains("heels.cn"))
                return ParseStyle.Heels;
            else if (host.Contains("duide.com"))
                return ParseStyle.Duide;
            else if (host.Contains("tu11.cc"))
                return ParseStyle.Tu11;
            else if (host.Contains("meituiji.com"))
                return ParseStyle.MeiTuiJi;
            else if (host.Contains("pal.ath.cx"))
                return ParseStyle.PalAthCx;
            else if (host.Contains("deskcity.com"))
                return ParseStyle.DeskCity;
            else if (host.Contains("pics100"))
                return ParseStyle.Pics100;
            else if (host.Contains("wallcoo.net") || host.Contains("wallcoo.com"))
                return ParseStyle.WallCoo;
            else if (host.Contains("mtswa"))
                return ParseStyle.Mtswa;
            else if (host.Contains("ttmnw"))
                return ParseStyle.Ttmnw;
            else if (host.Contains("xyuba"))
                return ParseStyle.Xyuba;
            else if (host.Contains("yzmnw"))
                return ParseStyle.Yzmnw;
            else if (host.Contains("girlcity.cn") || host.Contains("deskcity.cn"))
                return ParseStyle.GirlCity;
            else if (host.Contains("china016"))
                return ParseStyle.China016;
            else if (host.Contains("169pp.com"))
                return ParseStyle.PP169;
            else if (host.Contains("1000rt.com"))
                return ParseStyle.RT1000;
            else if (host.Contains("77meitu.com"))
                return ParseStyle.MeiTu77;
            else if (host.Contains("158kk.com"))
                return ParseStyle.KK158;
            else if (host.Contains("920mm.com"))
                return ParseStyle.MM920;
            else if (host.Contains("tu25.com"))
                return ParseStyle.Tu25;
            else if (host.Contains("mm.voc.com.cn"))
                return ParseStyle.Voc;
            else if (host.Contains("6188.net"))
                return ParseStyle.Net6188;
            else if (host.Contains("meitushow"))
                return ParseStyle.MeiTuShow;
            else if (host.Contains("6s8.net"))
                return ParseStyle.Net6s8;
            else return ParseStyle.NotSupport;
        }

        /// <summary>
        /// Parse URL address and generate dataset collection to store download information
        /// </summary>
        /// <param name="url">The address value from txtParse TextBox control</param>
        private string Parse(string url)
        {
            Prompt = "Parsing " + Enum.GetName(typeof(ParseStyle), rip.Style);
            const string NOTFOUND = "No album found in this page", CANCELLED = "User Cancelled";
            try
            {
                var hw = new HAP.HtmlWeb();
                var doc = hw.Load(url);
                rip.Title = doc.DocumentNode.SelectSingleNode("//title").InnerText;
                switch (rip.Style)
                {
                    #region Parse Heels.cn site

                    case ParseStyle.Heels:
                        {
                            if (rip.Canceled) return CANCELLED;
                            rip.Title = rip.Title.Substring(rip.Title.LastIndexOf('-') + 1);
                            var links = doc.DocumentNode.SelectNodes("//div/a/img[@src]");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string href = lnk.Attributes["src"].Value;
                                string key = href.Substring(href.LastIndexOf('=') + 1);
                                rip.Imgs[key + ".jpg"] = "http://www.heels.cn/web/getattachment?attach=" + key;
                            }
                        }
                        break;

                    #endregion

                    #region Parse Duide.com site

                    case ParseStyle.Duide:
                        {
                            if (rip.Canceled) return CANCELLED;
                            var links = doc.DocumentNode.SelectNodes("//a/img[@border=1]");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string href = lnk.Attributes["src"].Value.Replace("thumbnails", "images");
                                string[] name = href.Split('/', '_');
                                string key = string.Format("{0}-{1}.jpg", name[1].ToUpper(), name[3]);
                                rip.Imgs[key] = url.Replace(url.Substring(url.LastIndexOf('/') + 1), href);
                            }
                        }
                        break;
                    #endregion

                    #region Parse Tu11.cc site
                    case ParseStyle.Tu11:
                        {
                            if (rip.Canceled) return CANCELLED;
                            var nextpageNode = doc.DocumentNode.SelectNodes("//p[@align='center']//a").Last();
                            rip.NextPage = nextpageNode.Attributes["href"].Value;
                            rip.NextPage = rip.NextPage.StartsWith("/new/") ? "http://www.tu11.cc" + rip.NextPage : null;
                            var links = doc.DocumentNode.SelectNodes("//div[@id='content']/img[@src]");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = lnk.Attributes["src"].Value;
                                string name = address.Substring(address.LastIndexOf('/') + 1);
                                rip.Imgs[name] = address;
                            }
                        }
                        break;
                    #endregion

                    #region Parse MeiTuiJi.com site
                    case ParseStyle.MeiTuiJi:
                        {
                            if (rip.Canceled) return CANCELLED;
                            rip.Title = rip.Title.Split('-')[0];
                            string pageName = url.Substring(url.LastIndexOf('/') + 1);
                            var nextpageNode = doc.DocumentNode.SelectNodes("//ul[@class='pagelist']//a").Last();
                            rip.NextPage = nextpageNode.Attributes["href"].Value;
                            rip.NextPage = rip.NextPage != "#" ? url.Replace(pageName, rip.NextPage) : null;
                            var links = doc.DocumentNode.SelectNodes("//img[@src][@onload]");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = "http://www.meituiji.com" + lnk.Attributes["src"].Value;
                                string name = address.Substring(address.LastIndexOf('/') + 1);
                                rip.Imgs[name] = address;
                            }
                        }
                        break;
                    #endregion

                    #region Parse Pal.Ath.Cx site
                    case ParseStyle.PalAthCx:
                        {
                            do
                            {
                                if (rip.Canceled) return CANCELLED;
                                var links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src][@class]");
                                if (links == null || links.Count == 0) return NOTFOUND;
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    string imgsrc = lnk.Attributes["src"].Value;
                                    string href = lnk.ParentNode.Attributes["href"].Value;
                                    if (!href.EndsWith(".html", StringComparison.OrdinalIgnoreCase)) continue;
                                    string address = "http://pal.ath.cx" + imgsrc;
                                    int mark = rip.Title.IndexOf('[');
                                    if (mark > 0) rip.Title = rip.Title.Substring(0, mark);
                                    string name = string.Format("{0}{1:000}.jpg", rip.Title, rip.Imgs.Count);
                                    rip.Imgs[name] = address;
                                }
                                HAP.HtmlNode nextpageNode = doc.DocumentNode.SelectSingleNode("//div/a[@class='next']");
                                rip.NextPage = nextpageNode != null ? "http://pal.ath.cx" + nextpageNode.Attributes["href"].Value : null;
                                if (rip.NextPage != null) doc = hw.Load(rip.NextPage);
                            } while (rip.NextPage != null);
                        }
                        break;
                    #endregion

                    #region Parse DeskCity.com site

                    case ParseStyle.DeskCity:
                        {
                            rip.Title = rip.Title.Split('|')[0];
                            do
                            {
                                if (rip.Canceled) return CANCELLED;
                                var links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src]");
                                if (links == null || links.Count == 0) return NOTFOUND;
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    string img = lnk.Attributes["src"].Value;
                                    string key = img.Split("/-".ToCharArray())[4];
                                    rip.Imgs[key + ".jpg"] = "http://www.deskcity.com" + img.Replace(img.Substring(img.LastIndexOf(key)), key + ".jpg");
                                }
                                HAP.HtmlNode next = doc.DocumentNode.SelectSingleNode("//div[@class='pagination']");
                                if (next != null && next.HasChildNodes) next = next.LastChild; else return null;
                                rip.NextPage = next.Attributes["href"] != null ? next.Attributes["href"].Value : null;
                                if (rip.NextPage != null) doc = hw.Load("http://www.deskcity.com" + rip.NextPage);
                            } while (rip.NextPage != null);
                        }
                        break;

                    #endregion

                    #region Parse Pics100.net site

                    case ParseStyle.Pics100:
                        {
                            var part = rip.Title.Split('[', ']');
                            rip.Title = part[2];
                            if (rip.Canceled) return "User Cancelled!";
                            var links = doc.DocumentNode.SelectNodes("//p[@align]/img[@src]");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            var mark = url.LastIndexOf('_');
                            var num = mark > 0 ? int.Parse(url.Substring(mark + 1, url.LastIndexOf('.') - mark - 1)) : 1;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string img = lnk.Attributes["src"].Value;
                                string name = string.Format("{0} {1:000}.jpg", rip.Title, num);
                                rip.Imgs[name] = "http://www.pics100.net" + img;
                            }
                            var nextpageNode = doc.DocumentNode.SelectNodes("//b[@id='miracleflower']/a");
                            var node = nextpageNode.SingleOrDefault(n => n.InnerText == "下一页");
                            rip.NextPage = node != null ? "http://www.pics100.net" + node.Attributes["href"].Value : null;
                        }
                        break;

                    #endregion

                    #region Parse WallCoo.[Net|Com] site

                    case ParseStyle.WallCoo:
                        {
                            rip.Title = rip.Title.Substring(0, rip.Title.LastIndexOf('-'));
                            var folder = url.Substring(0, url.LastIndexOf('/'));
                            do
                            {
                                if (rip.Canceled) return "User Cancelled!";
                                var links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src][@alt][@title]");
                                if (links == null || links.Count == 0) return NOTFOUND;
                                var wxh = doc.DocumentNode.SelectSingleNode("//h2/strong").InnerText.Split('|');
                                var wh = wxh[wxh.Length - 2].Replace('*', 'x').Trim();
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    var img = lnk.Attributes["src"].Value;
                                    img = img.Split('/')[1];
                                    if (rip.Title.IndexOf(':') > 0) rip.Title = rip.Title.Substring(rip.Title.IndexOf(':') + 1);
                                    var key = img.Substring(0, img.LastIndexOf('s')) + ".jpg";
                                    var imgurl = folder + string.Format("/wallpapers/{0}/{1}", wh, key);
                                    rip.Imgs[HttpUtility.UrlDecode(key)] = imgurl;
                                }
                                var nextpage = doc.DocumentNode.SelectNodes("//div[@id='navigation']//li").Last();
                                nextpage = nextpage.HasChildNodes ? nextpage.ChildNodes["a"] : null;
                                rip.NextPage = nextpage != null ? folder + "/" + nextpage.Attributes["href"].Value : null;
                                if (rip.NextPage != null) doc = hw.Load(rip.NextPage);
                            } while (rip.NextPage != null);
                        }
                        break;

                    #endregion

                    #region Parse [Mtswa|Ttmnw|Yzmnw|Xyuba].com sites
                    case ParseStyle.Mtswa:
                    case ParseStyle.Ttmnw:
                    case ParseStyle.Yzmnw:
                    case ParseStyle.Xyuba:
                        {
                            rip.Title = rip.Title.Split('-')[0];
                            var links = doc.DocumentNode.SelectNodes("//img[@src]" + (rip.Style != ParseStyle.Xyuba ? "[@onload]" : "[@id='bigimg']"));
                            if (links == null || links.Count == 0) return NOTFOUND;
                            var nextpageNode = (rip.Style == ParseStyle.Mtswa || rip.Style == ParseStyle.Ttmnw)
                                ? doc.DocumentNode.SelectNodes("//div[@class='cPageBox']//a")
                             : doc.DocumentNode.SelectNodes("//ul[@class='pagelist']//a");
                            var pageid = doc.DocumentNode.SelectSingleNode("//li[@class='thisclass']/a[@href='#']").InnerText;
                            var id = int.Parse(pageid) - 1;
                            var node = nextpageNode.Last().Attributes["href"].Value;
                            rip.NextPage = node == "#" ? null : url.Substring(0, url.LastIndexOf('/') + 1) + node;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = (rip.Style == ParseStyle.Xyuba ? "http://www.xyuba.com" : string.Empty) + lnk.Attributes["src"].Value;
                                string name = string.Format("{0} {1:000}.jpg", rip.Title, (rip.Style == ParseStyle.Xyuba ? 3 : 4) * id + rip.Imgs.Count);
                                rip.Imgs[name] = address;
                            }
                        }
                        break;
                    #endregion

                    #region Parse [Desk|Girl]City.cn sites

                    case ParseStyle.GirlCity:
                        {
                            {
                                var pagename = url.Substring(url.LastIndexOf('/') + 1);
                                if (rip.Canceled) return CANCELLED;
                                var links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src][@border='1']");
                                if (links == null || links.Count == 0) return NOTFOUND;
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    string img = lnk.Attributes["src"].Value;
                                    string key = img.Substring(img.LastIndexOf('/') + 1).Remove(0, 3);
                                    rip.Imgs[key] = img.Substring(0, img.LastIndexOf('/') + 1) + key;
                                }
                                var pages = doc.DocumentNode.SelectNodes("//select/option").Where(p => p.Attributes["value"].Value != pagename);
                                foreach (var page in pages)
                                {
                                    var nextpage = url.Substring(0, url.LastIndexOf('/') + 1) + page.Attributes["value"].Value;
                                    doc = hw.Load(nextpage); if (rip.Canceled) return CANCELLED;
                                    links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src][@border='1']");
                                    foreach (HAP.HtmlNode lnk in links)
                                    {
                                        string img = lnk.Attributes["src"].Value;
                                        string key = img.Substring(img.LastIndexOf('/') + 1).Remove(0, 3);
                                        rip.Imgs[key] = img.Substring(0, img.LastIndexOf('/') + 1) + img;
                                    }
                                }
                            }
                        }
                        break;

                    #endregion

                    #region Parse China016.com and 77MeiTu.com sites

                    case ParseStyle.China016:
                    case ParseStyle.MeiTu77:
                        {
                            var folder = url.Substring(0, url.LastIndexOf('/') + 1);
                            if (rip.Canceled) return CANCELLED;
                            var links = doc.DocumentNode.SelectNodes("//img[@src]");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string img = lnk.Attributes["src"].Value;
                                string key = img.Substring(img.LastIndexOf('/') + 1);
                                rip.Imgs[key] = (rip.Style == ParseStyle.China016 ? "http://www.china016.com" : "") + img;
                            }
                            var pages = doc.DocumentNode.SelectNodes("//a[@href][@title]");
                            if (pages != null && pages[pages.Count - 2].PreviousSibling != null)
                                rip.NextPage = folder + pages[pages.Count - 2].Attributes["href"].Value.Remove(0, 2);
                            else rip.NextPage = null;
                        }
                        break;

                    #endregion

                    #region Parse 169PP.com and 1000rt.com sites

                    case ParseStyle.PP169:
                    case ParseStyle.RT1000:
                        {
                            if (rip.Canceled) return CANCELLED;
                            var links = doc.DocumentNode.SelectNodes("//p[@align='center']/img[@src]");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string img = lnk.Attributes["src"].Value;
                                string key = img.Substring(img.LastIndexOf('/') + 1);
                                rip.Imgs[key] = img;
                            }
                        }
                        break;

                    #endregion

                    #region Parse 920MM.com site

                    case ParseStyle.MM920:
                        {
                            if (rip.Canceled) return CANCELLED;
                            var folder = url.Substring(0, url.LastIndexOf('/') + 1);
                            var pagename = url.Substring(url.LastIndexOf('/') + 1);
                            var links = doc.DocumentNode.SelectNodes("//ul[@class='file']/img");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = lnk.Attributes["src"].Value.TrimEnd('\r');
                                string name = address.Substring(address.LastIndexOf('/') + 1);
                                rip.Imgs[name] = address;
                            }
                            var page = doc.DocumentNode.SelectNodes("//ul[@class='image']/a[@href]").Last();
                            if (page != null)
                            {
                                var nextpage = page.Attributes["href"].Value;
                                if (!nextpage.Equals(pagename, StringComparison.OrdinalIgnoreCase))
                                    rip.NextPage = folder + nextpage;
                                else
                                    rip.NextPage = null;
                            }
                            else rip.NextPage = null;
                        }
                        break;
                    #endregion

                    #region Parse 158KK.com site

                    case ParseStyle.KK158:
                        {
                            if (rip.Canceled) return CANCELLED;
                            rip.Title = rip.Title.Split('_')[0];
                            var folder = url.Substring(0, url.LastIndexOf('/') + 1);
                            var links = doc.DocumentNode.SelectNodes("//div[@class='bimg']//img");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = lnk.Attributes["src"].Value;
                                string name = address.Substring(address.LastIndexOf('/') + 1);
                                rip.Imgs[name] = address;
                            }
                            var page = doc.DocumentNode.SelectNodes("//div[@id='simg']/a").Last();
                            rip.NextPage = page.NextSibling.Name != "span" ? folder + page.Attributes["href"].Value : null;
                        }
                        break;
                    #endregion

                    #region Parse Tu25.com site

                    case ParseStyle.Tu25:
                        {
                            if (rip.Canceled) return CANCELLED;
                            rip.Title = rip.Title.Split('-')[0];
                            var links = doc.DocumentNode.SelectNodes("//p/img[@src]");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = lnk.Attributes["src"].Value;
                                string name = address.Substring(address.LastIndexOf('/') + 1);
                                rip.Imgs[name] = address;
                            }
                            var page = doc.DocumentNode.SelectNodes("//p[@align='center']/a").Last();
                            var pageUrl = page.Attributes["href"].Value;
                            rip.NextPage = pageUrl.StartsWith("/") ? "http://www.tu25.com" + pageUrl: null;
                        }
                        break;
                    #endregion

                    #region Parse mm.voc.com.cn site

                    case ParseStyle.Voc:
                        {
                            if (rip.Canceled) return CANCELLED;
                            rip.Title = rip.Title.Split('-')[0];
                            var links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src][@onload]");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = lnk.Attributes["src"].Value;
                                string name = address.Substring(address.LastIndexOf('/') + 1).Split('-')[0] + ".jpg";
                                rip.Imgs[name] = address;
                            }
                        }
                        break;
                    #endregion

                    #region Parse 6188.net site

                    case ParseStyle.Net6188:
                        {
                            if (rip.Canceled) return CANCELLED;
                            var links = doc.DocumentNode.SelectNodes("//p[@id]/a[@href]/img[@src]");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = "http://www.6188.net" + lnk.Attributes["src"].Value;
                                string name = address.Substring(address.LastIndexOf('/') + 1);
                                rip.Imgs[name] = address;
                            }
                            var page = doc.DocumentNode.SelectNodes("//div[@class='page']/a").Last().PreviousSibling;
                            rip.NextPage = page.Name == "a" ? "http://www.6188.net" + page.Attributes["href"].Value : null;
                        }
                        break;
                    #endregion

                    #region Parse MeiTuShow.com site

                    case ParseStyle.MeiTuShow:
                        {
                            if (rip.Canceled) return CANCELLED;
                            rip.Title = rip.Title.Split('_')[0];
                            var folder = url.Substring(0, url.LastIndexOf('/') + 1);
                            var links = doc.DocumentNode.SelectNodes("//div[@class='pics']//img");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = "http://www.meitushow.com" + lnk.Attributes["src"].Value;
                                string name = address.Substring(address.LastIndexOf('/') + 1);
                                rip.Imgs[name] = address;
                            }
                            var page = doc.DocumentNode.SelectNodes("//div[@class='page']/a").FirstOrDefault(n=>n.NextSibling.Name=="br");
                            rip.NextPage = page != null ? folder + page.Attributes["href"].Value : null;
                        }
                        break;
                    #endregion

                    #region Parse 6s8.net site

                    case ParseStyle.Net6s8:
                        {
                            if (rip.Canceled) return CANCELLED;
                            rip.Title = rip.Title.Split('-')[0];
                            var folder = url.Substring(0, url.LastIndexOf('/') + 1);
                            var links = doc.DocumentNode.SelectNodes("//span[@class='STYLE1']/img");
                            if (links == null || links.Count == 0) return NOTFOUND;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = "http://www.6s8.net" + lnk.Attributes["src"].Value;
                                string name = address.Substring(address.LastIndexOf('/') + 1);
                                rip.Imgs[name] = address;
                            }
                            var pages = doc.DocumentNode.SelectNodes("//div[@class='turnPage']/a");
                            var page = pages.Last().FirstChild.Name=="font" ? pages[pages.Count-2] : null;
                            rip.NextPage = page != null ? folder + page.Attributes["href"].Value.TrimStart("./".ToCharArray()) : null;
                        }
                        break;
                    #endregion
                    case ParseStyle.NotSupport:
                        return "Invalid Site Url!";
                }
            }
            catch (Exception)
            {
                return (rip.Title == null ? "Download" : "Parse") + " Failed!";
            }
            return null;
        }

        private void DownloadFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            rip.Reset();
            tsPB.Visible = false;
            lbBatch.Text = null;
            System.Media.SystemSounds.Exclamation.Play();
            if (e.Cancelled)
            {
                tsLabel.Text = "Task cancelled.";
                btnDownloadCancel.Image = Resources.Download;
                btnDownloadCancel.Enabled = true;
                btnBatch.Enabled = true;
                tbParse.ReadOnly = false;
                tbDir.ReadOnly = false;
            }
            else
            {
                tsLabel.Text = e.Result != null ? e.Result as string : "Task finished.";
                if (rip.NextPage != null)
                {
                    switch (rip.PushState)
                    {
                        case RipperAction.Download:
                            btnDownloadCancel.Image = Resources.Cancel;
                            rip.PushState = RipperAction.Cancel;
                            Address = rip.NextPage;
                            bwFetch.RunWorkerAsync();
                            break;
                        case RipperAction.Cancel:
                            rip.NextPage = null;
                            if (bwFetch.CancellationPending) return;
                            rip.Canceled = true; rip.Cancel();
                            if (bwFetch.IsBusy)
                                bwFetch.CancelAsync();
                            btnDownloadCancel.Enabled = false;
                            break;
                    }
                }
                else if (Batch)
                {
                    switch (rip.PushState)
                    {
                        case RipperAction.Download:
                            From++;
                            AdjustURL(1);
                            bwFetch.RunWorkerAsync();
                            btnDownloadCancel.Image = Resources.Cancel;
                            rip.PushState = RipperAction.Cancel;
                            Batch = From != To;
                            break;
                        case RipperAction.Cancel:
                            Batch = false;
                            if (bwFetch.CancellationPending) return;
                            rip.Canceled = true; rip.Cancel();
                            if (bwFetch.IsBusy)
                                bwFetch.CancelAsync();
                            btnDownloadCancel.Enabled = false;
                            break;
                    }
                }
                else
                {
                    btnDownloadCancel.Image = Resources.Download;
                    btnDownloadCancel.Enabled = true;
                    btnBatch.Enabled = true;
                    tbParse.ReadOnly = false;
                    tbDir.ReadOnly = false;
                }
            }
        }

        private void DownloadFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tsPB.Value = e.ProgressPercentage;
        }

        private void pbDoubleClick(object sender, EventArgs e)
        {
            if (rip != null && rip.ImageLocation != null && File.Exists(rip.ImageLocation))
                Process.Start(rip.ImageLocation);
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            AdjustURL(1);
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            AdjustURL(-1);
        }

        /// <summary>
        /// Adjust URL querystring value by increment/decrement number variable
        /// </summary>
        /// <param name="pm">The PlusMinus enum value indicate the action type.</param>
        private void AdjustURL(int step)
        {
            if (tbParse.ReadOnly == true) return;
            string number; int value;
            switch (rip.Style)
            {
                case ParseStyle.Heels:
                    if (!Address.StartsWith("http://www.heels.cn/web/viewthread?thread=", StringComparison.OrdinalIgnoreCase)) return;
                    number = Address.Split('=')[1];
                    if (int.TryParse(number, out value))
                    {
                        value += step; if (value < 0) return;
                        Address = Address.Replace(number, value.ToString());
                    }
                    break;

                case ParseStyle.Duide:
                    if (!Address.StartsWith("http://www.duide.com/", StringComparison.OrdinalIgnoreCase)) return;
                    number = Address.Substring(Address.LastIndexOf('/') + 2).Replace(".htm", "");
                    if (int.TryParse(number, out value))
                    {
                        value += step; if (value < 0) return;
                        Address = Address.Replace(number, value.ToString());
                    }
                    break;

                case ParseStyle.PalAthCx:
                    if (!Address.StartsWith("http://pal.ath.cx/", StringComparison.OrdinalIgnoreCase)) return;
                    var seg = Address.Split('/');
                    number = seg[seg.Length - 2];
                    if (int.TryParse(number, out value))
                    {
                        value += step; if (value < 0) return;
                        Address = string.Format("{0}{1}/", Address.Substring(0, Address.LastIndexOf(number)), value.ToString(new string('0', number.Length)));
                    }
                    break;
            }
        }

        /// <summary>
        /// Clear the listbox items, and reset dataset to initial state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (lvRip.Items.Count > 0)
            {
                lvRip.Items.Clear();
                lvRip.Groups.Clear();
            }
        }

        private void btnBatch_Click(object sender, EventArgs e)
        {
            rip.Style = CheckUrl(Address);
            switch (rip.Style)
            {
                case ParseStyle.NotSupport: return;
                case ParseStyle.Heels:
                    {
                        if (!Address.TrimStart().StartsWith("http://www.heels.cn/web/viewthread?thread=")) return;
                        string text = Address.Split('=')[1];
                        int pageid;
                        if (int.TryParse(text, out pageid))
                            new Batch(pageid).ShowDialog(this);
                    }
                    break;
                case ParseStyle.Duide:
                    {
                        if (!Address.TrimStart().StartsWith("http://www.duide.com/ggfdrdsuy/")) return;
                        string text = Address.Substring(Address.LastIndexOfAny("abc".ToCharArray()) + 1).Split('.')[0];
                        int pageid;
                        if (int.TryParse(text, out pageid))
                            new Batch(pageid).ShowDialog(this);
                    }
                    break;
                default:
                    MessageBox.Show("URL address cann't take batch operation.", "No batch operation support on this site!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private void cmmiNextPage_Click(object sender, EventArgs e)
        {
            if (Batch)
                rip.SkipPage = true;
        }

        private void cmmiDelete_Click(object sender, EventArgs e)
        {
            if (File.Exists(rip.ImageLocation))
            {
                File.Delete(rip.ImageLocation);
                var lvi = lvRip.FindItemWithText(Path.GetFileName(rip.ImageLocation));
                rip.ImageLocation = null;
                if (lvi != null)
                {
                    lvi.SubItems[3].Text = "Deleted";
                    lvi.Font = new Font(lvi.Font, FontStyle.Strikeout);
                }
            }
        }

        private void cmmiDrop_Click(object sender, EventArgs e)
        {
            if (rip.Imgs != null && rip.Imgs.Count > 0)
            {
                foreach (string name in rip.Imgs.AllKeys)
                {
                    if (rip.Current != null && rip.Current.Name == name) { rip.Dropped = true; rip.Cancel(); continue; }
                    string path = Path.Combine(Dir, name);
                    if (File.Exists(path))
                    {
                        var lvi = lvRip.FindItemWithText(name);
                        if (lvi != null)
                        {
                            if (lvi.SubItems[3].Text == "Downloading") continue;
                            lvi.SubItems[3].Text = "Deleted";
                            lvi.Font = new Font(lvi.Font, FontStyle.Strikeout);
                            File.Delete(path);
                        }
                    }
                }
                rip.SkipPage = true;
            }
        }

        private void lvSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvRip.FocusedItem != null)
            {
                string file = Path.Combine(Dir, lvRip.FocusedItem.Text);
                if (File.Exists(file))
                {
                    if (mainSplit.Panel2Collapsed) mainSplit.Panel2Collapsed = false;
                    rip.ImageLocation = pbPreview.ImageLocation = file;
                }
                else
                    rip.ImageLocation = null;
            }
        }

        private void cmmiDeleteFile(object sender, EventArgs e)
        {
            if (lvRip.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in lvRip.SelectedItems)
                {
                    string path = Path.Combine(Dir, lvi.Text);
                    if (File.Exists(path))
                    {
                        if (lvi.SubItems[3].Text == "Downloading") continue;
                        File.Delete(path);
                        lvi.SubItems[3].Text = "Deleted";
                        lvi.Font = new Font(lvi.Font, FontStyle.Strikeout);
                    }
                }
            }
        }

        private void cmmiDownloadFile(object sender, EventArgs e)
        {
            var args = new List<DownloadFileArgs>();
            foreach (ListViewItem lvi in lvRip.SelectedItems)
            {
                if (!File.Exists(Path.Combine(Dir, lvi.Text)))
                {
                    args.Add(new DownloadFileArgs { Url = lvi.ToolTipText, Name = lvi.Text });
                    lvi.SubItems[3].Text = "Downloading";
                    lvi.Font = new Font(lvi.Font, FontStyle.Regular);
                }
            }
            if (args.Count > 0)
                new Thread(DownloadFile).Start(args);
        }

        void DownloadFile(object args)
        {
            var dfa = args as System.Collections.Generic.List<DownloadFileArgs>;
            foreach (var item in dfa)
            {
                try
                {
                    string path = Path.Combine(Dir, item.Name);
                    if (rip.Style == ParseStyle.Heels)
                    {
                        using (Stream s = rip.GetStream(item.Url, Settings.Default.Cookie))
                        {
                            using (Image bmp = Image.FromStream(s))
                            {
                                s.Close(); bmp.Save(path);
                                bmp.Dispose();
                            }
                        }
                    }
                    else
                        rip.GetFile(item.Url, path);
                    LviUpdate = new string[] { item.Name, null, new FileInfo(path).Length / 1024 + " KB", "Finished" };
                }
                catch (Exception exp)
                {
                    LviUpdate = new string[] { item.Name, null, null, exp.Message };
                }
            }
        }

        class DownloadFileArgs
        {
            public string Url { get; set; }
            public string Name { get; set; }
        }

        private void lvKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (e.Shift)
                        cmmiDrop_Click(sender, e);
                    else
                        cmmiDeleteFile(sender, e);
                    break;
                case Keys.C:
                    if (e.Control && lvRip.FocusedItem != null)
                        Clipboard.SetText(lvRip.FocusedItem.ToolTipText);
                    break;
            }
        }

        private void Ripper_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (e.Shift) { mainSplit.Panel2Collapsed = true; break; }
                    if (tbParse.Focused || tbDir.Focused) break;
                    mainSplit.Panel1Collapsed = !mainSplit.Panel1Collapsed;
                    break;
                case Keys.Escape:
                    if (FormBorderStyle == FormBorderStyle.None)
                    {
                        FullScreen = false;
                        FormBorderStyle = FormBorderStyle.Sizable;
                        Bounds = (Rectangle)this.Tag;
                        mainSplit.Panel1Collapsed = false;
                        if ((rip == null || rip.ImageLocation == null) && pbPreview.Image == null)
                            mainSplit.Panel2Collapsed = true;
                    }
                    break;
                case Keys.F11:
                    if (FullScreen)
                    {
                        FullScreen = false;
                        FormBorderStyle = FormBorderStyle.Sizable;
                        Bounds = (Rectangle)this.Tag;
                        if ((rip == null || rip.ImageLocation == null) && pbPreview.Image == null)
                            mainSplit.Panel2Collapsed = true;
                    }
                    else
                    {
                        this.Tag = Bounds;
                        mainSplit.Panel1Collapsed = true;
                        mainSplit.Panel2Collapsed = false;
                        FormBorderStyle = FormBorderStyle.None;
                        Bounds = Screen.PrimaryScreen.Bounds;
                        FullScreen = true;
                    }
                    break;
            }
        }

        bool CanDownload
        {
            get
            {
                if ((rip.Style = CheckUrl(Address)) == ParseStyle.NotSupport) { tsLabel.Text = "Not supported!"; return false; }
                if (!Directory.Exists(Dir))
                {
                    if (DialogResult.Yes == MessageBox.Show("Do you want to create new folder to store files?", "Invalid Directory!", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        try { Directory.CreateDirectory(Dir); return true; }
                        catch (Exception exp) { MessageBox.Show(exp.Message, "Create Directory failed!", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                    }
                    else
                        return false;
                }
                return true;
            }
        }

        private void cmmiCopyName_Click(object sender, EventArgs e)
        {
            if (lvRip.FocusedItem != null)
                Clipboard.SetText(lvRip.FocusedItem.Text);
        }

        private void llFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if ((fbDir.ShowDialog()) == DialogResult.OK)
                Dir = fbDir.SelectedPath;
        }

        private void llCookie_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new SetCookie().ShowDialog(this);
        }

        private void tbParse_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void tbParse_DragDrop(object sender, DragEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = (string)e.Data.GetData(DataFormats.Text);
        }

        private void cmsLV_Opening(object sender, CancelEventArgs e)
        {
            cmmiNextPage.Visible = Batch; cmmiClear.Enabled = lvRip.Items.Count > 0;
            cmmiSave.Visible = cmmiRemove.Visible = cmmiCopyName.Visible = lvRip.SelectedItems.Count > 0 ? true : false;
            cmmiDropGroup.Visible = rip.Imgs.Count > 0;
        }

        private void CloudToolStrip_Click(object sender, EventArgs e)
        {
            tsCloud.ShowDropDown();
        }

        private void CloudItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            switch (tsmi.Text)
            {
                case "GDrive": WebCloud.Service = WebCloud.CloudType.GDrive; break;
                case "Flickr": WebCloud.Service = WebCloud.CloudType.Flickr; break;
                case "Facebook": WebCloud.Service = WebCloud.CloudType.Facebook; break;
                case "Picasa": WebCloud.Service = WebCloud.CloudType.Picasa; break;
            }
            new WebCloud().Show(this);
        }

        private void tsHome_Click(object sender, EventArgs e)
        {
            Process.Start("http://imgrip.codeplex.com");
        }

        private void llSites_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Sites().ShowDialog(this);
        }
    }
}