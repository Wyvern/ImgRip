namespace ImgRipper
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using ImgRipper.Properties;
    using HAP = HtmlAgilityPack;
    using System.Collections.Generic;

    partial class Ripper : Form
    {
        Fetcher rip = new Fetcher();
        bool FullScreen { get; set; }
        internal int Range { get; set; }
        internal bool Batch { get; set; }

        public Ripper()
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
        bool ToggleProgressBar { set { RipStatus.Invoke(new Action(() => tsPB.Visible = value)); } }
        string Prompt { set { RipStatus.Invoke(new Action(() => tsLabel.Text = value)); } }
        int SetProgressBar { set {  RipStatus.Invoke(new Action(() => tsPB.Value = value)); } }
        string[] SetListViewItem
        {
            set
            {
                lvRip.Invoke(new Action( () =>
                    {
                        //No. 1, name 0, size 2, state 3
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
                            lvi.ForeColor = lvi.Index % 2 == 0 ? Color.DarkGreen : Color.DarkBlue;
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
                    ((Button) sender).Image = Resources.Cancel;
                    rip.PushState = RipperAction.Cancel;
                    break;
                case RipperAction.Cancel:
                    Batch = false;
                    rip.Canceled = true;  rip.Cancel();  rip.NextPage = null;
                    tbParse.ReadOnly = false;
                    tbDir.ReadOnly = false;
                    if (bwFetch.IsBusy)
                        bwFetch.CancelAsync();
                    ((Button) sender).Enabled = false;
                    break;
            }
        }

        private void ResetStatus()
        {
            Prompt = "Downloading...";
            SetProgressBar = 0;
            ToggleProgressBar = true;
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
            for (int idx = 0; idx < rip.Imgs.Count; idx++)
            {
                rip.Address = rip.Imgs[idx];
                FileInfo fi = new FileInfo(Path.Combine(Dir, rip.Imgs.Keys[idx]));
                rip.Current = fi;
                //number 1, name 0, size 2, state 3
                string Order = (idx + 1).ToString();
                if (fi.Exists)
                {
                    SetListViewItem = new [] { fi.Name, Order, fi.Length / 1024 + " KB", "Existed" };
                    continue;
                }
               
                SetListViewItem = new [] { fi.Name, Order, null, "Downloading" };
                if (Batch) RipStatus.Invoke(new Action(() => lbBatch.Text = string.Format(" #{0}/{1} Pages", (Range - (To - From)), Range)));
                try
                {
                    if (rip.Style == ParseStyle.Heels)
                    #region For Heels.cn
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
                                SetListViewItem = new [] { fi.Name, Order, null, "Check Cookie / Wait 5 secs" };
                                Thread.Sleep(5000);
                            }
                        }
                    }
                    #endregion
                    else
                    #region For Others
                    {
                        rip.GetFile(rip.Address, fi.ToString());//Block here
                        if (RipCheck(Order, e)) return;
                    }
                    #endregion
                    if (mainSplit.Panel2Collapsed) mainSplit.Invoke(new Action(() => mainSplit.Panel2Collapsed = false));
                    fi.Refresh();
                    SetListViewItem = new [] { fi.Name, Order, fi.Length / 1024 + " KB", "Finished" };
                    pbPreview.ImageLocation = rip.ImageLocation = fi.ToString();
                    bwFetch.ReportProgress((idx + 1) * 100 / rip.Imgs.Count);
                }
                catch (Exception exp)
                {
                    SetListViewItem = new [] {  fi.Name,Order, null, exp.Message};
                }
            }
        }

        bool RipCheck(string order, DoWorkEventArgs e)
        {
            if (rip.Dropped) { rip.Dropped = false; SetListViewItem = new [] { rip.Current.Name, order, null, "Dropped" }; return true; }
            if (rip.Canceled) { e.Cancel = true; rip.NextPage = null; SetListViewItem = new [] { rip.Current.Name, order, null, "Cancelled" }; return true; }
            if (rip.SkipPage)
            {
                rip.SkipPage = false;
                SetListViewItem = new [] { rip.Current.Name, order, null, "Skipped" };
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
            if (host.Contains("heels"))
                return ParseStyle.Heels;
            else if (host.Contains("duide"))
                return ParseStyle.Duide;
            else if (host.Contains("tu11.cc"))
                return ParseStyle.Tu11;
            else if (host.Contains("meituiji"))
                return ParseStyle.MeiTuiJi;
            else if (host.Contains("pal.ath.cx"))
                return ParseStyle.PalAthCx;
            else if (host.Contains("deskcity"))
                return ParseStyle.DeskCity;
            else if (host.Contains("pics100"))
                return ParseStyle.Pics100;
            else if (host.Contains("wallcoo.net") || host.Contains("wallcoo.com"))
                return ParseStyle.WallCoo;
            else if (host.Contains("mtswa"))
                return ParseStyle.Mtswa;
            else return ParseStyle.NotSupport;
        }

        /// <summary>
        /// Parse URL address and generate dataset collection to store download information
        /// </summary>
        /// <param name="url">The address value from txtParse TextBox control</param>
        private string Parse(string url)
        {
            Prompt = "Parsing " + Enum.GetName(typeof(ParseStyle), rip.Style);
            try
            {
                var doc = new HAP.HtmlWeb() { AutoDetectEncoding = true }.Load(url);
                rip.Title = doc.DocumentNode.SelectSingleNode("//title").InnerText;
                switch (rip.Style)
                {
                    #region Parse Heels.cn site

                    case ParseStyle.Heels:
                        {
                            rip.Title = rip.Title.Substring(rip.Title.LastIndexOf('-') + 1);
                            var links = doc.DocumentNode.SelectNodes("//div/a/img[@src]");
                            if (links == null || links.Count == 0) return "No picture found in this page";
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
                            var links = doc.DocumentNode.SelectNodes("//a/img[@border=1]");
                            if (links == null || links.Count == 0) return "No picture found in this page";
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string href = lnk.Attributes["src"].Value.Replace("thumbnails", "images");
                                string[] name = href.Split('/','_');
                                string key = string.Format("{0}-{1}.jpg", name[1].ToUpper(), name[3]);
                                rip.Imgs[key] = url.Replace(url.Substring(url.LastIndexOf('/')+1), href);
                            }
                        }
                        break;
                    #endregion

                    #region Parse Tu11.cc site
                    case ParseStyle.Tu11:
                        {
                            var nextpageNode = doc.DocumentNode.SelectSingleNode("//p[@align='center']/a[last()]");
                            rip.NextPage = nextpageNode.Attributes["href"].Value;
                            rip.NextPage = rip.NextPage.StartsWith("/new/") ? "http://www.tu11.cc" + rip.NextPage : null;
                            var links = doc.DocumentNode.SelectNodes("//div[@id='content']/img[@src]");
                            if (links == null || links.Count == 0) return "No picture found in this page";
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
                            rip.Title = doc.DocumentNode.SelectSingleNode("//div[@id='newsName']").InnerText;
                            string pageName = url.Substring(url.LastIndexOf('/') + 1);
                            var nextpageNode = doc.DocumentNode.SelectSingleNode("//ul[@class='pagelist']/li[last()]/a");
                            rip.NextPage = nextpageNode.Attributes["href"].Value;
                            rip.NextPage = rip.NextPage != "#" ? url.Replace(pageName, rip.NextPage) : null;
                            var links = doc.DocumentNode.SelectNodes("//div[@id='newsContent']/a[@href]");
                            if (links == null || links.Count == 0) return "No picture found in this page";
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = "http://www.meituiji.com" + lnk.Attributes["href"].Value;
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
                                if (rip.Canceled) return "User Cancelled";
                                var links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src][@class]");
                                if (links == null || links.Count == 0) return "No picture found in this page";
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    string imgsrc = lnk.Attributes["src"].Value;
                                    string id = imgsrc.Split("/-".ToCharArray())[1], file = imgsrc.Substring(imgsrc.LastIndexOf('/') + 1);
                                    string alt = lnk.Attributes["alt"].Value;
                                    if (!file.Contains(alt)) continue;
                                    imgsrc = imgsrc.Replace(id, (uint.Parse(id) - 1).ToString());
                                    string address = "http://pal.ath.cx" + imgsrc;
                                    int mark = rip.Title.IndexOf('[');
                                    if (mark > 0) rip.Title = rip.Title.Substring(0, mark);
                                    string name = string.Format("{0} {1:000}.jpg", rip.Title, rip.Imgs.Count);
                                    rip.Imgs[name] = address;
                                }
                                HAP.HtmlNode nextpageNode = doc.DocumentNode.SelectSingleNode("//div/a[@class='next']");
                                rip.NextPage = nextpageNode != null ? nextpageNode.Attributes["href"].Value : null;
                                rip.NextPage = rip.NextPage != null ? "http://pal.ath.cx" + rip.NextPage : null;
                                if (rip.NextPage != null) doc = new HAP.HtmlWeb().Load(rip.NextPage);
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
                                if (rip.Canceled) return "User Cancelled";
                                var links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src]");
                                if (links == null || links.Count == 0) return "No picture found in this page";
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    string img = lnk.Attributes["src"].Value;
                                    string key = img.Split("/-".ToCharArray())[4];
                                    rip.Imgs[key+".jpg"] = "http://www.deskcity.com" + img.Replace(img.Substring(img.LastIndexOf(key)), key + ".jpg");
                                }
                                HAP.HtmlNode next = doc.DocumentNode.SelectSingleNode("//div[@class='pagination']");
                                if (next != null && next.HasChildNodes) next = next.LastChild; else return null;
                                rip.NextPage = next.Attributes["href"] != null ? next.Attributes["href"].Value : null;
                                if (rip.NextPage != null) doc = new HAP.HtmlWeb().Load("http://www.deskcity.com" + rip.NextPage);
                            } while (rip.NextPage != null);
                        }
                        break;

                    #endregion

                    #region Parse Pics100.net site
                    
                    case ParseStyle.Pics100:
                        {
                            var part = rip.Title.Split('[',']');
                            rip.Title = part[2]; int countofpage = int.Parse(part[part.Length-2].TrimEnd('p','P'));
                           while(rip.Imgs.Count!=countofpage)
                            {
                                if (rip.Canceled) return "User Cancelled!";
                                var links = doc.DocumentNode.SelectNodes("//p[@align]/img[@src]");
                                if (links == null || links.Count == 0) return "No picture found in this page";
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    string img = lnk.Attributes["src"].Value;
                                    string name = string.Format("{0} {1:000}.jpg", rip.Title, rip.Imgs.Count);
                                    rip.Imgs[name] = "http://www.pics100.net" + img;
                                }
                                if (rip.Imgs.Count == countofpage) break;
                                Prompt = string.Format("Parsing page {0} of {1}", rip.Imgs.Count+1, countofpage);
                               var mark=url.LastIndexOf('_');
                               var nextpage = string.Format("{0}_{1}.html", mark > 0 ? url.Substring(0, mark) : url.Replace(".html", ""), rip.Imgs.Count);
                                if (nextpage.EndsWith("_1.html")) nextpage = nextpage.Replace("_1.html", ".html");
                                if (nextpage.Equals(url, StringComparison.OrdinalIgnoreCase)) nextpage = string.Format("{0}_{1}.html", mark > 0 ? url.Substring(0, mark) : url.Replace(".html", ""), rip.Imgs.Count+1);
                                doc = new HAP.HtmlWeb().Load(nextpage);
                            }
                        }
                        break;

                    #endregion

                    #region Parse WallCoo.net site

                    case ParseStyle.WallCoo:
                        {
                            rip.Title = rip.Title.Substring(0, rip.Title.LastIndexOf('-'));
                            var folder = url.Substring(0, url.LastIndexOf('/'));
                            do
                            {
                                if (rip.Canceled) return "User Cancelled!";
                                var links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src][@alt][@title]");
                                if (links == null || links.Count == 0) return "No picture found in this page";
                                var wxh = doc.DocumentNode.SelectSingleNode("//h2/strong").InnerText.Split('|');
                                var wh = wxh[wxh.Length - 2].Replace('*', 'x').Trim();
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    var img = lnk.Attributes["src"].Value;
                                    if (rip.Title.IndexOf(':') > 0) rip.Title = rip.Title.Substring(rip.Title.IndexOf(':') + 1);
                                    var key = string.Format("{0}{1:00}.jpg", rip.Title, rip.Imgs.Count+1);
                                    var mock = folder + string.Format("/wallpapers/{0}/{1}", wh, img.Split('/')[1]);
                                    rip.Imgs[key] = mock.Substring(0, mock.LastIndexOf('s')) + ".jpg";
                                }
                                var nextpage = doc.DocumentNode.SelectNodes("//a[@href][@class='navigationtext']");
                                var next = nextpage[nextpage.Count - 1];
                                if (next != null) rip.NextPage = next.InnerText.Contains("&gt") ? folder + "/" + next.Attributes["href"].Value : null;
                                if (rip.NextPage != null) doc = new HAP.HtmlWeb().Load(rip.NextPage);
                            } while (rip.NextPage != null);
                        }
                        break;

                    #endregion

                    #region Parse Mtswa.com site
                    case ParseStyle.Mtswa:
                        {
                            rip.Title = rip.Title.Split('-')[0];
                            var links = doc.DocumentNode.SelectNodes("//img[@onload][@onclick]");
                            if (links == null || links.Count == 0) return "No picture found in this page";
                            var nextpageNode = doc.DocumentNode.SelectSingleNode("//div[@class='cPageBox']").LastChild.FirstChild;
                            var pageid = doc.DocumentNode.SelectSingleNode("//li[@class='thisclass']/a[@href='#']").InnerText;
                            var id = int.Parse(pageid) - 1;
                            rip.NextPage = nextpageNode.Attributes["href"].Value;
                            rip.NextPage = rip.NextPage == "#" ? null : url.Substring(0, url.LastIndexOf('/') + 1) + rip.NextPage;
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string address = lnk.Attributes["src"].Value; 
                                string name = string.Format("{0} {1:000}.jpg", rip.Title, 4*id+ rip.Imgs.Count);
                                rip.Imgs[name] = address;
                            }
                        }
                        break;
                    #endregion

                    case ParseStyle.NotSupport:
                        return "Invalid Site Url!";
                }
            }
            catch (Exception)
            {
                return "Parse ERROR!";
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
                            rip.Canceled = true;rip.Cancel();
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
                            rip.Canceled = true;rip.Cancel();
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

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
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
            string number; int value;
            switch (rip.Style)
            {
                case ParseStyle.Heels:
                    if (!Address.StartsWith("http://www.heels.cn/web/viewthread?thread=")) return;
                    number = Address.Split('=')[1];
                    if (int.TryParse(number, out value))
                    {
                        value += step;
                        Address = Address.Replace(number, value.ToString());
                    }
                    break;

                case ParseStyle.Duide:
                    if (!Address.StartsWith("http://www.duide.com/ggfdrdsuy")) return;
                    number = Address.Substring(Address.LastIndexOfAny("abc".ToCharArray())+1).Split('.')[0];
                    if (int.TryParse(number, out value))
                    {
                        value += step;
                        Address = Address.Replace(number, value.ToString());
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
            if ((rip.Style = CheckUrl(Address)) == ParseStyle.NotSupport) return;
            if (rip.Style == ParseStyle.Heels)
            {
                if (!Address.TrimStart().StartsWith("http://www.heels.cn/web/viewthread?thread=")) return;
                string text = Address.Split('=')[1];
                int pageid;
                if (int.TryParse(text, out pageid))
                    new Batch(pageid).ShowDialog(this);
            }
            else if (rip.Style == ParseStyle.Duide)
            {
                if (!Address.TrimStart().StartsWith("http://www.duide.com/ggfdrdsuy/")) return;
                string text = Address.Substring(Address.LastIndexOfAny("abc".ToCharArray()) + 1).Split('.')[0];
                int pageid;
                if (int.TryParse(text, out pageid))
                    new Batch(pageid).ShowDialog(this);
            }
            else
                MessageBox.Show("Please imput URL address which support batch operation.", "Can not take batch operation on this site!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    string path = Path.Combine(Dir, name);
                    if (rip.Current != null && rip.Current.Name == name) { rip.Dropped = true; rip.Cancel(); continue; }
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

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvRip.FocusedItem!=null)
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
                    string path=Path.Combine(Dir, item.Name);
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
                    SetListViewItem = new string[] { item.Name, null, new FileInfo(path).Length/1024+" KB", "Finished" };
                }
                catch (Exception exp)
                {
                    SetListViewItem = new string[] { item.Name, null, null, exp.Message };
                }
            }
        }

        class DownloadFileArgs
        {
            public string Url { get; set; }
            public string Name { get; set; }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
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
                if ((rip.Style = CheckUrl(Address)) == ParseStyle.NotSupport) return false;
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
            cmmiNextPage.Visible = Batch;
            cmmiSave.Visible = cmmiRemove.Visible = cmmiCopyName.Visible = lvRip.SelectedItems.Count > 0 ? true : false;
            cmmiClear.Enabled = cmmiDropGroup.Enabled = lvRip.Items.Count == 0 ? false : true;
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