namespace ImgRipper
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Windows.Forms;

    using ImgRipper.Properties;
    using HAP = HtmlAgilityPack;
    partial class Ripper : Form
    {
        public bool Batch { get; set; }
        Fetcher rip = new Fetcher();
        bool FullScreen { get; set; }
        public int Range { get; set; }

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
                            if (lvRip.Groups[rip.Title] == null) lvRip.Groups.Add(rip.Title, rip.Title + string.Format(" [{0}P]", rip.Imgs.Count));
                            lvi.Group = lvRip.Groups[rip.Title];
                            lvRip.Items.Add(lvi).EnsureVisible();
                            lvi.ForeColor = lvi.Index % 2 == 0 ? Color.DarkGreen : Color.DarkBlue;
                        }
                        #endregion
                    }));
            }
        }

        public int From { get; set; }
        public int To { get; set; }

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
                    //Begin download action
                    bwDownload.RunWorkerAsync();
                    ((Button) sender).Image = Resources.Cancel;
                    rip.PushState = RipperAction.Cancel;
                    break;
                case RipperAction.Cancel:
                    Batch = false;
                    rip.Canceled = true;
                    rip.NextPage = null;
                    tbParse.ReadOnly = false;
                    tbDir.ReadOnly = false;
                    if (bwDownload.IsBusy)
                        bwDownload.CancelAsync();
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

        private void DownloadFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            if ((e.Result = Parse(Address)) != null) return;
            if (rip.Canceled) { e.Result = "User Cancelled!"; rip.NextPage = null; return; }
            Fetch(e);
        }

        private void Fetch(DoWorkEventArgs e)
        {
            ResetStatus();
            for (int idx = 0; idx < rip.Imgs.Count; idx++)
            {
                if (rip.Canceled) { e.Cancel = true; return; }
                rip.Address = rip.Imgs[idx];
                FileInfo fi = new FileInfo(Path.Combine(Dir, rip.Imgs.Keys[idx]));
                rip.Current = fi;
                //number 1, name 0, size 2, state 3
                string No = (idx + 1).ToString();
                if (fi.Exists)
                {
                    SetListViewItem = new string[] { fi.Name, No, fi.Length / 1024 + " KB", "Existed" };
                    continue;
                }
                if (mainSplit.Panel2Collapsed) mainSplit.Invoke(new Action(() => mainSplit.Panel2Collapsed = false));
                try
                {
                    if (rip.Style == ParseStyle.Heels)
                    #region For Heels.cn
                    {
                        Bitmap bmp = null;
                        string filesize;
                        bool succeed = false;
                        if (string.IsNullOrEmpty(Settings.Default.Cookie))
                        {
                            this.Invoke(new Action(() =>
                            {
                                new SetCookie().ShowDialog(this);
                            }));
                        }
                        if (string.IsNullOrEmpty(Settings.Default.Cookie))
                        {
                            e.Result = "NULL Cookie!";
                            SetListViewItem = new string[] { fi.Name,No,  null, "Cancelled" };
                            return;
                        }
                        while (!succeed)
                        {
                            if (rip.SkipPage)
                            {
                                rip.SkipPage = false;
                                SetListViewItem = new string[] {  fi.Name, No,null, "Skipped" };
                                return;
                            }
                            try
                            {
                                bmp = rip.GetBitmap(rip.Address, Settings.Default.Cookie);
                                succeed = true;
                            }
                            catch (Exception)
                            {
                                if (idx == 0)
                                {
                                    if (Batch)
                                        rip.SkipPage = true;
                                    SetListViewItem = new string[] {  fi.Name,No, null, "Not enough points!" };
                                    return;
                                }
                                SetListViewItem = new string[] { fi.Name,No,  null, "Retry after 5 secs..." };
                                Thread.Sleep(5000);
                                if (rip.Canceled)
                                {
                                    e.Cancel = true;
                                    rip.NextPage = null;
                                    SetListViewItem = new string[] { fi.Name, No, null, "Cancelled" };
                                    return;
                                }
                                else
                                {
                                    if (Batch) RipStatus.Invoke(new Action(() => lbBatch.Text = " #" + (Range - (To - From)) + "/" + Range + " Pages"));
                                    SetListViewItem = new string[] { fi.Name, No, null, "Downloading..." };
                                }
                            }
                        }
                        #region Check whether the file is too small, dimension less than 768x768 pixels
                        rip.Tiny = false;
                        if (bmp.Width >= 768 && bmp.Height >= 768 || cmmiSaveAll.Checked)
                        {
                            bmp.Save(rip.ImageLocation = fi.ToString());
                            fi.Refresh();
                        }
                        else
                        {
                            rip.Tiny = true;
                            rip.ImageLocation = null;
                        }
                        #endregion
                        pbPreview.Image = bmp.Clone() as Image;
                        filesize = fi.Exists ? fi.Length / 1024 + " KB" : "";
                        SetListViewItem = new string[] { fi.Name, No, filesize, rip.Tiny ? "Dropped" : "Finished" };
                        bmp.Dispose();
                    }
                    #endregion
                    else
                    #region For Others
                    {
                        if (rip.SkipPage)
                        {
                            rip.SkipPage = false;
                            SetListViewItem = new string[] { fi.Name, No, null, "Skipped" };
                            return;
                        }
                        if (Batch) RipStatus.Invoke(new Action(()=>lbBatch.Text=" #" + (Range - (To - From)) + "/" + Range+" Pages"));
                        SetListViewItem = new string[] { fi.Name,No,  null, "Downloading" };
                        rip.GetFile(rip.Address, fi.ToString());
                        fi.Refresh();
                        SetListViewItem = new string[] { fi.Name,No,  fi.Length / 1024 + " KB", "Finished" };
                        pbPreview.ImageLocation = rip.ImageLocation = fi.ToString();
                    }
                    #endregion
                    bwDownload.ReportProgress((idx + 1) * 100 / rip.Imgs.Count);
                }
                catch (WebException exp)
                {
                    SetListViewItem = new string[] {  fi.Name,No, null, exp.Message};
                }
            }
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
            else if (host.Contains("keaibbs"))
                return ParseStyle.KeAiBbs;
            else if (host.Contains("tu11.cc"))
                return ParseStyle.Tu11;
            else if (host.Contains("meituiji"))
                return ParseStyle.MeiTuiJi;
            else if (host.Contains("pal.ath.cx"))
                return ParseStyle.PalAthCx;
            else if (host.Contains("deskcity"))
                return ParseStyle.DeskCity;
            if (host.Contains("pics100"))
                return ParseStyle.Pics100;
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
                var doc = new HAP.HtmlWeb() { AutoDetectEncoding=true}.Load(url);
                rip.Title = doc.DocumentNode.SelectSingleNode("//title").InnerText;
                switch (rip.Style)
                {
                    #region Parse Heels.cn site

                    case ParseStyle.Heels:
                        {
                            rip.Title = rip.Title.Substring(rip.Title.LastIndexOf('-') + 1);
                            HAP.HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//div/a/img[@src]");
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
                            HAP.HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//a/img[@border=1]");
                            if (links == null || links.Count == 0) return "No picture found in this page";
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string href = lnk.Attributes["src"].Value.Replace("thumbnails", "images");
                                string[] name = href.Split('/','_');
                                string key = name[1].ToUpper() + '-' + name[3] + ".jpg";
                                rip.Imgs[key] = url.Replace(url.Substring(url.LastIndexOf('/')+1), href);
                            }
                        }
                        break;
                    #endregion

                    #region Parse tuku.keaibbs.com site

                    case ParseStyle.KeAiBbs:
                        {
                            int countofpage = doc.DocumentNode.SelectNodes("//option").Count;
                            HAP.HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//td/a/img[@src]");
                            if (links == null || links.Count == 0) return "No picture found in this page";
                            string path = "http://tuku.keaibbs.com" + url.Replace("/index.html", "");
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string[] tokens = lnk.Attributes["src"].Value.Split('/');
                                string key = tokens[tokens.Length - 1].Substring(2);
                                rip.Imgs[key.Substring(0, key.LastIndexOf('_')) + ".jpg"] = path + "/originalimages/" + key;
                            }
                            for (int i = 2; i <= countofpage; i++)
                            {
                                if (rip.Canceled) return "User Cancelled";
                                Prompt = "Parsing... Page " + i + "/" + countofpage;
                                doc = new HAP.HtmlWeb().Load(path + "/index" + i + ".html");
                                links = doc.DocumentNode.SelectNodes("//td/a/img[@src]");
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    string[] tokens = lnk.Attributes["src"].Value.Split('/');
                                    string key = tokens[tokens.Length - 1].Substring(2);
                                    rip.Imgs[key.Substring(0, key.LastIndexOf('_')) + ".jpg"] = path + "/originalimages/" + key;
                                }
                            }
                        }
                        break;

                    #endregion

                    #region Parse Tu11.cc site
                    case ParseStyle.Tu11:
                        {
                            HAP.HtmlNode nextpageNode = doc.DocumentNode.SelectSingleNode("//p[@align='center']/a[last()]");
                            rip.NextPage = nextpageNode.Attributes["href"].Value;
                            rip.NextPage = rip.NextPage.StartsWith("/new/") ? "http://www.tu11.cc" + rip.NextPage : null;
                            HAP.HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//div[@id='content']/img[@src]");
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
                            HAP.HtmlNode nextpageNode = doc.DocumentNode.SelectSingleNode("//ul[@class='pagelist']/li[last()]/a");
                            rip.NextPage = nextpageNode.Attributes["href"].Value;
                            rip.NextPage = rip.NextPage != "#" ? url.Replace(pageName, rip.NextPage) : null;
                            HAP.HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//div[@id='newsContent']/a[@href]");
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
                            HAP.HtmlNode nextpageNode = doc.DocumentNode.SelectSingleNode("//div/a[@class='next']");
                            rip.NextPage = nextpageNode != null ? nextpageNode.Attributes["href"].Value : null;
                            rip.NextPage = rip.NextPage != null ? "http://pal.ath.cx" + rip.NextPage : null;
                            HAP.HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src]");
                            if (links == null || links.Count == 0) return "No picture found in this page";
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string imgsrc = lnk.Attributes["src"].Value;
                                string[] segs = imgsrc.Split("/-".ToCharArray());
                                string id = segs[1], file = segs[segs.Length-1];
                                string alt = lnk.Attributes["alt"].Value;
                                if (file != alt) continue;
                                imgsrc = imgsrc.Replace(id, (uint.Parse(id) - 1).ToString());
                                string address = "http://pal.ath.cx" + imgsrc;
                                int mark = rip.Title.IndexOf('[');
                                if (mark > 0) rip.Title = rip.Title.Substring(0, mark);
                                string name = rip.Title + string.Format("{0:000}", rip.Imgs.Count) + ".jpg";
                                rip.Imgs[name] = address;
                            }
                            while (rip.NextPage != null)
                            {
                                doc = new HAP.HtmlWeb().Load(rip.NextPage);
                                nextpageNode = doc.DocumentNode.SelectSingleNode("//div/a[@class='next']");
                                rip.NextPage = nextpageNode != null ? nextpageNode.Attributes["href"].Value : null;
                                rip.NextPage = rip.NextPage != null ? "http://pal.ath.cx" + rip.NextPage : null;
                                links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src]");
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    string imgsrc = lnk.Attributes["src"].Value;
                                    string[] segs = imgsrc.Split("/-".ToCharArray());
                                    string id = segs[1], file = segs[segs.Length - 1];
                                    string alt = lnk.Attributes["alt"].Value;
                                    if (file != alt) continue;
                                    imgsrc = imgsrc.Replace(id, (uint.Parse(id) - 1).ToString());
                                    string address = "http://pal.ath.cx" + imgsrc;
                                    int mark = rip.Title.IndexOf('[');
                                    if (mark > 0) rip.Title = rip.Title.Substring(0, mark);
                                    string name = rip.Title + string.Format("{0:000}", rip.Imgs.Count) + ".jpg";
                                    rip.Imgs[name] = address;
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Parse DeskCity.com site

                    case ParseStyle.DeskCity:
                        {
                            rip.Title = rip.Title.Split('|')[0];
                            HAP.HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src]");
                            if (links == null || links.Count == 0) return "No picture found in this page";
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string img = lnk.Attributes["src"].Value;
                                string key = img.Split("/-".ToCharArray())[4];
                                rip.Imgs[key + ".jpg"] = "http://www.deskcity.com" + img.Replace(img.Substring(img.LastIndexOf(key)), key + ".jpg");
                            }
                            string nextpage = null;
                            HAP.HtmlNode next = doc.DocumentNode.SelectSingleNode("//div[@class='pagination']");
                            if (next != null && next.HasChildNodes) next = next.LastChild; else return null;
                            if (next.Attributes["href"] != null)
                                nextpage = next.Attributes["href"].Value;
                            while (nextpage != null)
                            {
                                doc = new HAP.HtmlWeb().Load("http://www.deskcity.com" + nextpage);
                                links = doc.DocumentNode.SelectNodes("//a[@href]/img[@src]");
                                if (links == null || links.Count == 0) return "No picture found in this page";
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    string img = lnk.Attributes["src"].Value;
                                    string key = img.Split("/-".ToCharArray())[4];
                                    rip.Imgs[key + ".jpg"] = "http://www.deskcity.com"  + img.Replace(img.Substring(img.LastIndexOf(key)), key + ".jpg");
                                }
                                next = doc.DocumentNode.SelectSingleNode("//div[@class='pagination']").LastChild;
                                if (next.Attributes["href"] != null)
                                    nextpage = next.Attributes["href"].Value;
                                else
                                    nextpage = null;
                            }
                        }
                        break;

                    #endregion

                    #region Parse Pics100.net site
                    
                    case ParseStyle.Pics100:
                        {
                            var part = rip.Title.Split("[]".ToCharArray());
                            rip.Title = part[2]; int countofpage = int.Parse(part[part.Length-2].TrimEnd("pP".ToCharArray()));
                            HAP.HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//p[@align]/img[@src]");
                            if (links == null || links.Count == 0) return "No picture found in this page";
                            foreach (HAP.HtmlNode lnk in links)
                            {
                                string img = lnk.Attributes["src"].Value;
                                string key = string.Format("{0} {1:000}.jpg", rip.Title, 0);
                                rip.Imgs[key] = "http://www.pics100.net" + img;
                            }
                            for (int i = 2; i <= countofpage; i++)
                            {
                                if (rip.Canceled) return "User Cancelled!";
                                Prompt = string.Format("Parsing page {0} of {1}", i, countofpage);
                                url = string.Format("{0}_{1}.html", url.LastIndexOf('_') > 0 ? url.Split('_')[0] : url.Replace(".html", ""), i);
                                doc = new HAP.HtmlWeb().Load(url);
                                links = doc.DocumentNode.SelectNodes("//p[@align]/img[@src]");
                                if (links == null || links.Count == 0) break;
                                foreach (HAP.HtmlNode lnk in links)
                                {
                                    string img = lnk.Attributes["src"].Value;
                                    string key = string.Format("{0} {1:000}.jpg", rip.Title, i-1);
                                    rip.Imgs[key] = "http://www.pics100.net" + img;
                                }
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
                            bwDownload.RunWorkerAsync();
                            break;
                        case RipperAction.Cancel:
                            rip.NextPage = null;
                            if (bwDownload.CancellationPending) return;
                            rip.Canceled = true;
                            if (bwDownload.IsBusy)
                                bwDownload.CancelAsync();
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
                            bwDownload.RunWorkerAsync();
                            btnDownloadCancel.Image = Resources.Cancel;
                            rip.PushState = RipperAction.Cancel;
                            Batch = From != To;
                            break;
                        case RipperAction.Cancel:
                            Batch = false;
                            if (bwDownload.CancellationPending) return;
                            rip.Canceled = true;
                            if (bwDownload.IsBusy)
                                bwDownload.CancelAsync();
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
                FileInfo fi = new FileInfo(rip.ImageLocation);
                rip.ImageLocation = null;
                var lvi = lvRip.FindItemWithText(fi.Name);
                if (lvi != null)
                {
                    lvi.SubItems[3].Text = "Deleted";
                    lvi.Font = new Font(lvi.Font, FontStyle.Strikeout);
                }
            }
        }

        private void cmmiDeleteAll_Click(object sender, EventArgs e)
        {
            if (rip.Imgs != null && rip.Imgs.Count > 0 && rip.Style == ParseStyle.Heels)
            {
                foreach (string name in rip.Imgs.AllKeys)
                {
                    string file = Path.Combine(Dir, name);
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                        ListViewItem lvi = lvRip.FindItemWithText(name);
                        if (lvi != null)
                        {
                            lvi.SubItems[3].Text = "Deleted";
                            lvi.Font = new Font(lvi.Font, FontStyle.Strikeout);
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
                        File.Delete(path);
                        lvi.SubItems[3].Text = "Deleted";
                        lvi.Font = new Font(lvi.Font, FontStyle.Strikeout);
                    }
                }
            }
        }

        private void cmmiDownloadFile(object sender, EventArgs e)
        {
            var args = new System.Collections.Generic.List<DownloadFileArgs>();
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
                    if (rip.Style == ParseStyle.Heels)
                    {
                        Bitmap bmp = rip.GetBitmap(item.Url, Settings.Default.Cookie);
                        bmp.Save(Path.Combine(Dir, item.Name));
                        bmp.Dispose();
                    }
                    else
                        rip.GetFile(item.Url, Path.Combine(Dir, item.Name));
                    SetListViewItem = new string[] { item.Name, null, null, "Finished" };
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
                        cmmiDeleteAll_Click(sender, e);
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
                        if ((fbDir.ShowDialog()) == DialogResult.OK)
                            Dir = fbDir.SelectedPath;
                        else return false;
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
            cmmiDropGroup.Visible = cmmiSaveAll.Visible = (rip != null && rip.Style == ParseStyle.Heels) ? true : false;
            cmmiSave.Visible = cmmiRemove.Visible = cmmiCopyName.Visible = lvRip.SelectedItems.Count > 0 ? true : false;
            cmmiClear.Enabled = lvRip.Items.Count == 0 ? false : true;
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
    }
}