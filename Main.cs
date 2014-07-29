//#define CheckSites
namespace ImgRip
{
    using ImgRip.Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml.Linq;

    partial class Main : Form
    {
        bool FullScreen { get; set; }
        internal Site[] sites;
        internal long Range { get; set; }
        internal bool Batch { get { return To - From > 0; } set { cmmiBatch.Checked = cmmiBatch.Enabled = cmmiNextPage.Enabled = value; } }

        public Main()
        {
            InitializeComponent();
            btnGo.Tag = RipperAction.Download;
            tsHome.Alignment = ToolStripItemAlignment.Right;
            btnGo.UpArrowMouseUp += (s, e) => tmPlus.Enabled = false;
            btnGo.DownArrowMouseUp += (s, e) => tmMinus.Enabled = false;
            btnGo.UpArrowMouseDown += (s, e) =>
            {
                if (tmMinus.Enabled) tmMinus.Enabled = false;
                if ((Fetch.Site = CheckUrl(Address)) == null || (RipperAction)btnGo.Tag == RipperAction.Cancel) return;
                tmPlus.Enabled = true;
            };
            btnGo.DownArrowMouseDown += (s, e) =>
            {
                if (tmPlus.Enabled) tmPlus.Enabled = false;
                if ((Fetch.Site = CheckUrl(Address)) == null || (RipperAction)btnGo.Tag == RipperAction.Cancel) return;
                tmMinus.Enabled = true;
            };
            sites = ReadXML();
            //CheckSites(); WriteXML();
        }

        Site[] ReadXML()
        {
            var xe = XElement.Parse(Resources.SiteList);
            var sites = from s in xe.Descendants("Site")
                        select new Site(s.Attribute("Name").Value, s.Attribute("Domain").Value, s.Attribute("Image").Value)
                        {
                            Type = s.Attribute("Type") == null ? "Default" : s.Attribute("Type").Value,
                            Next = s.Attribute("Next") == null ? "" : s.Attribute("Next").Value,
                            Screen = s.Attribute("Screen") == null ? "" : s.Attribute("Screen").Value
                        };
            return sites.OrderBy(_ => _.Name).ToArray();
        }

        Site[] CheckSites()
        {
            var website = sites.ToList();
            foreach (var site in sites)
            {
                HttpWebRequest request = WebRequest.Create(string.Format("http://{0}", site.Domain)) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = null;
                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                    response.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    website.Remove(site);
                }
            }
            return website.ToArray();
        }

        void WriteXML()
        {
            var doc = new XDocument();
            doc.Add(new XElement("ImageRipper", new XElement("List")));
            foreach (var item in sites)
            {
                var el = new XElement("Site",
                    new XAttribute("Name", item.Name),
                    new XAttribute("Domain", item.ToString()),
                    new XAttribute("Image", item.Image),
                    string.IsNullOrEmpty(item.Next) ? null : new XAttribute("Next", item.Next),
                    string.IsNullOrEmpty(item.Screen) ? null : new XAttribute("Screen", item.Screen));
                if (item.Type != "Default")
                    el.Add(new XAttribute("Type", item.Type));
                doc.Descendants("List").First().Add(el);
            }
            doc.Save(@".\..\..\Sites.xml");
        }

        /// <summary>
        /// UI Callback properties
        /// </summary>
        bool ShowProgressBar { set { Invoke(new Action(() => tsPB.Visible = value)); } }
        string Prompt { set { Invoke(new Action(() => tsLabel.Text = value)); } }
        int ProgressPercentage { set { Invoke(new Action(() => tsPB.Value = value)); } }
        string[] LviUpdate
        {
            set
            {
                Invoke(new Action(() =>
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
                            lvi.ToolTipText = Fetch.Address;
                            if (lvRip.Groups[Fetch.Title] == null) lvRip.Groups.Add(Fetch.Title, string.Format("{0} [{1}P]", Fetch.Title, Fetch.Images.Count));
                            lvi.Group = lvRip.Groups[Fetch.Title];
                            lvRip.Items.Add(lvi).EnsureVisible();
                            lvi.ForeColor = lvi.Index % 2 == 0 ? Color.ForestGreen : Color.RoyalBlue;
                        }
                        #endregion
                    }));
            }
        }

        internal long From { get; set; }
        internal long To { get; set; }

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

        private void Go_Click(object sender, EventArgs e)
        {
            switch (((RipperAction)btnGo.Tag))
            {
                case RipperAction.Download:
                    if (!CanDownload) return;
                    tbParse.ReadOnly = true;
                    tbDir.ReadOnly = true;
                    Settings.Default.Save();
                    Go();
                    break;
                case RipperAction.Cancel:
                    Batch = false;
                    Fetch.Canceled = true; Fetch.Cancel();
                    bwFetch.CancelAsync();
                    ((Button)sender).Enabled = false;
                    break;
            }
        }

        private void ResetStatus()
        {
            Prompt = string.Empty;
            ShowProgressBar = true;
            ProgressPercentage = 0;
        }

        private void Fetch_DoWork(object sender, DoWorkEventArgs e)
        {
            if ((e.Result = Parse(Address.Trim())) != null) { Fetch.Next = null; return; }
            if (Fetch.Canceled) { e.Cancel = true; Fetch.Next = null; return; }
#if ! TRACE
            FetchFile(e);
#endif
        }

        private void FetchFile(DoWorkEventArgs e)
        {
            ResetStatus();
            if (Fetch.Images.Count == 0) e.Result = "Not a photo gallery!";
            for (int idx = 0; idx < Fetch.Images.Count; idx++)
            {
                var kvp = Fetch.Images.ElementAt(idx);
                Fetch.Address = kvp.Value;
                var fi = new FileInfo(Path.Combine(Dir, kvp.Key));
                Fetch.Current = fi;
                //number #1, name #0, size #2, state #3
                var Order = (idx + 1).ToString();
                if (fi.Exists)
                {
                    LviUpdate = new[] { fi.Name, Order, fi.Length / 1024 + " KB", "Existed" };
                    continue;
                }
                LviUpdate = new[] { fi.Name, Order, null, "Downloading" };
                var percentage = idx * 100 / Fetch.Images.Count;
                bwFetch.ReportProgress(percentage);
                if (Batch) Invoke(new Action(() => lbBatch.Text = string.Format(" #{0}/{1} Pages", (Range - (To - From)), Range)));

                #region Download files
                switch (Fetch.Site.Type)
                {
                    default:
                        try
                        {
                            Fetch.GetFile(Fetch.Address, fi.ToString());
                        }
                        catch (Exception exp)
                        {
                            LviUpdate = new[] { fi.Name, Order, null, exp.Message };
                            if (fi.Exists) fi.Delete();
                        }
                        break;
                    case "Heels":
                        bool succeed = false;
                        if (string.IsNullOrEmpty(Settings.Default.Cookie))
                        {
                            Invoke(new Action(() =>
                            {
                                new SetCookie().ShowDialog(this);
                            }));
                        }
                        while (!succeed)
                        {
                            using (var s = Fetch.GetStream(Fetch.Address, Settings.Default.Cookie))
                            using (var bmp = Image.FromStream(s))
                            {
                                try
                                {
                                    bmp.Save(fi.ToString());
                                    s.Close();
                                }
                                catch (Exception)
                                {
                                    s.Close(); s.Dispose(); bmp.Dispose();
                                    if (Check(Order, e)) return;
                                    LviUpdate = new[] { fi.Name, Order, null, "Invalid Cookie / Wait 5 secs" };
                                    Thread.Sleep(5000);
                                }
                                succeed = true;
                            }
                        }
                        break;
                    case "Heel":
                        using (var s = Fetch.GetStream(Fetch.Address))
                        using (var bmp = Image.FromStream(s))
                        {
                            try
                            {
                                bmp.Save(fi.ToString());
                                s.Close();
                            }
                            catch (Exception exp)
                            {
                                s.Close(); s.Dispose(); bmp.Dispose();
                                if (Check(Order, e)) return;
                                LviUpdate = new[] { fi.Name, Order, null, exp.Message };
                            }
                        }
                        break;
                }
                #endregion

                if (Check(Order, e)) return;
                fi.Refresh();
                if (!fi.Exists) { LviUpdate = new[] { fi.Name, Order, null, "Download Failed!" }; continue; }
                if (mainSplit.Panel2Collapsed) Invoke(new Action(() => mainSplit.Panel2Collapsed = !cmmiPreview.Checked));
                LviUpdate = new[] { fi.Name, Order, fi.Length / 1024 + " KB", "Finished" };
                pbPreview.ImageLocation = Fetch.Location = fi.ToString();
                bwFetch.ReportProgress((idx + 1) * 100 / Fetch.Images.Count);
            }
        }

        bool Check(string order, DoWorkEventArgs e)
        {
            if (Fetch.Dropped) { Fetch.Dropped = false; LviUpdate = new[] { Fetch.Current.Name, order, null, "Dropped" }; return true; }
            if (Fetch.Canceled) { e.Cancel = true; Fetch.Next = null; LviUpdate = new[] { Fetch.Current.Name, order, null, "Cancelled" }; return true; }
            if (Fetch.Skip)
            {
                Fetch.Skip = false;
                LviUpdate = new[] { Fetch.Current.Name, order, null, "Skipped" };
                return true;
            }
            return false;
        }

        private Site CheckUrl(string address)
        {
            string host;
            try
            {
                var Url = new Uri(address);
                host = Url.Host;
            }
            catch (System.UriFormatException format)
            {
                tsLabel.Text = format.Message;
                return null;
            }
            try
            {
                return sites.SingleOrDefault(_ => host.Replace("www.", "").Equals(_.ToString(), StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception e)
            { MessageBox.Show(e.Message, "Duplicate site definition"); return null; }
        }

        /// <summary>
        /// Parse URL address and generate dataset collection to store download information
        /// </summary>
        /// <param name="url">The address value from txtParse TextBox control</param>
        private string Parse(string url)
        {
            Prompt = string.Format("Analyzing: {0} ", Fetch.Site.Name);
            var uri = new Uri(url);
            try
            {
                var folder = url.Substring(0, url.LastIndexOf('/') + 1);
                switch (Fetch.Site.Type)
                {
                    #region Parse Heels.cn site
                    case "Heels":
                        return Fetch.Parse(url, fnName: n => n.Substring(n.LastIndexOf('=') + 1) + ".jpg", fnAddress: a => "http://www.heels.cn/web/" + a.Replace("_small", ""));
                    #endregion

                    #region Parse Duide.com site
                    case "Duide":
                        return Fetch.Parse(url, fnName:
                            n => url.Substring(url.LastIndexOf('/') + 1).Split('.')[0].ToUpper() + "-" + n.Substring(n.LastIndexOf('/') + 1).Split('_')[0] + ".jpg",
                            fnAddress:
                            a =>
                            {
                                var href = a.Replace("thumbnails", "images");
                                return url.Replace(url.Substring(url.LastIndexOf('/') + 1), href);
                            });
                    #endregion

                    #region Parse Pal.Ath.Cx site
                    case "PalAthCx":
                        return Fetch.Parse(url, fnAddress:
                            a =>
                            {
                                var id = a.Split('/')[1];
                                id = (int.Parse(id.Substring(0, id.LastIndexOf('-'))) - 1).ToString() + "-2";
                                return string.Format("http://{0}:{1}/", uri.Host, uri.Port) + id + a.Substring(a.LastIndexOf('/'));
                            });
                    #endregion

                    #region Parse DeskCity.com site
                    case "DeskCity":
                        return Fetch.Parse(url, hnc =>
                        {
                            var page = hnc.SingleOrDefault();
                            page = page != null && page.HasChildNodes ? page.LastChild : null;
                            return page.Attributes["href"] != null ? page.Attributes["href"].Value : null;
                        },
                        fnAddress: a =>
                        {
                            var key = a.Split("/-".ToCharArray())[4];
                            return "http://www.deskcity.com" + a.Replace(a.Substring(a.LastIndexOf(key)), key + ".jpg");
                        });
                    #endregion

                    #region Parse Pics100.net site
                    case "Pics100":
                        return Fetch.Parse(url, hnc => { var page = hnc.SingleOrDefault(n => n.InnerText == "下一页"); return page != null ? "http://www.pics100.net" + page.Attributes["href"].Value : null; });
                    #endregion

                    #region Parse WallCoo.[Net|Com] site
                    case "WallCoo":
                        return Fetch.Parse(url,
                        fnAddress: a =>
                            {
                                var img = a.Split('/')[1];
                                var key = img.Substring(0, img.LastIndexOf('s')) + ".jpg";
                                return folder + string.Format("wallpapers/{0}/{1}", Fetch.Screen, key);
                            },
                            fnScreen: hn =>
                            {
                                var wxh = hn.InnerText.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                return wxh[wxh.Length - 2].Replace('*', 'x').Trim();
                            });
                    #endregion

                    #region Parse [Desk|Girl]City.cn sites
                    case "GirlCity":
                        return Fetch.Parse(url,
                            hnc =>
                            {
                                var index = hnc.IndexOf(hnc.Last(o => o.Attributes["selected"] != null));
                                return (index + 1 == hnc.Count) ? null : folder + hnc[index + 1].Attributes["value"].Value;
                            },
                            fnAddress: a =>
                            {
                                var key = a.Substring(a.LastIndexOf('/') + 1).Remove(0, 3);
                                return a.Substring(0, a.LastIndexOf('/') + 1) + key;
                            });
                    #endregion

                    #region Parse mm.voc.com.cn site
                    case "Voc":
                        return Fetch.Parse(url, fnName: n => n.Substring(n.LastIndexOf('/') + 1).Split('-')[0] + ".jpg");
                    #endregion

                    #region Parse MeiTuShow.com, 909MM.com site
                    case "MeiTuShow":
                    case "MM909":
                        return Fetch.Parse(url, hnc =>
                        {
                            var page = hnc.SingleOrDefault(n => n.NextSibling.Name == "br");
                            return page == null ? null : folder + page.Attributes["href"].Value;
                        });
                    #endregion

                    #region Parse China-Girl.info & Tuku.cn site
                    case "Tuku":
                    case "ChinaGirl":
                        return Fetch.Parse(url, fnAddress: a => { var name = a.Substring(a.LastIndexOf('/') + 2); return a.Substring(0, a.LastIndexOf('/') + 1) + name; });
                    #endregion

                    #region Parse ZhuoKu.com site
                    case "ZhuoKu":
                        return Fetch.Parse(url, hnc =>
                        {
                            var index = hnc.IndexOf(hnc.Last(o => o.Attributes["selected"] != null));
                            return (index + 1 == hnc.Count) ? null : folder + hnc[index + 1].Attributes["value"].Value;
                        }, fnAddress: a => a.Replace("//img", "//bizhi").Replace("/thumbs/tn_", "/"));
                    #endregion

                    #region Parse BeautyLeg.cc site
                    case "BeautyLeg":
                        return Fetch.Parse(url, fnAddress: a => "http://www.beautyleg.cc" + a.Replace("/thumbs/", "/albums/").Split('?')[0]);
                    #endregion

                    #region Parse 25MeiNv.com site
                    case "Meinv25":
                        return Fetch.Parse(url,
                            hnc =>
                            {
                                var page = hnc[hnc.Count - 2];
                                return page.Attributes["href"].Value.StartsWith("javascript:") ? null : "http://www.25meinv.com" + page.Attributes["href"].Value;
                            },
                        n =>
                        {
                            var mark = url.LastIndexOf('_');
                            var num = mark > 0 ? int.Parse(url.Substring(mark + 1)) : 0;
                            return string.Format("{0}{1:000}.jpg", Fetch.Title, num);
                        });
                    #endregion

                    #region Parse 52Desktop.cn site
                    case "Desktop":
                        return Fetch.Parse(url, hnc =>
                        {
                            var page = hnc.Last(); var href = page.Attributes["href"].Value.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                            var first = page.PreviousSibling.PreviousSibling.Attributes["href"].Value;
                            return !href.Equals(first, StringComparison.OrdinalIgnoreCase) ? folder + href : null;
                        });
                    #endregion

                    #region Parse Ivsky.com site
                    case "Ivsky":
                        return Fetch.Parse(url, fnAddress: a => a.Replace("/m/", "/img/"));
                    #endregion

                    #region Parse Winddesktop.com qfdesk.cn site
                    case "QFZM":
                        return Fetch.Parse(url, fnAddress: a => folder + a.Replace("/160/", "/original/").Remove(0, 1));
                    #endregion

                    #region Parse KKdesk.com site
                    case "KKDesk":
                        return Fetch.Parse(url, fnAddress: a => a.Split('=')[1]);
                    #endregion

                    #region Parse Ydesk.com site
                    case "YDesk":
                        return Fetch.Parse(url,
                        fnNextPage: hnc => { var page = hnc.SingleOrDefault(n => n.InnerText == "下一页"); return page != null ? page.Attributes["href"].Value : null; },
                        fnAddress: a => a.Replace("/thumb/", "/" + Fetch.Screen + "/"),
                        fnScreen: hn =>
                        {
                            var list = hn.InnerText.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            return list[list.Length - 1].Trim();
                        });
                    #endregion

                    #region Parse Leafweb.cn, Pic86.cn site
                    case "LeafWeb":
                        return Fetch.Parse(url, fnAddress: a => a.Replace("_show.jpg", ".jpg"));
                    #endregion

                    #region Parse Kpbz.net, KuDesk.com site
                    case "Kpbz":
                        return Fetch.Parse(url, fnAddress: a => "http://www.kpbz.net" + a.Replace("-lp.jpg", ".jpg"));
                    #endregion

                    #region Parse KuDesk.com site
                    case "KuDesk":
                        return Fetch.Parse(url, fnAddress: a => a.Replace("-lp.jpg", ".jpg"));
                    #endregion

                    #region Parse 51bizhi.com site
                    case "Bizhi":
                        return Fetch.Parse(url, fnAddress: a => "http://www.51bizhi.com" + a.Replace("/S/", "/B/"),
                        fnNextPage: hnc => { var page = hnc[hnc.Count - 2]; return page.InnerText == "��һҳ" ? "http://www.51bizhi.com" + page.Attributes["href"].Value : null; });
                    #endregion

                    #region Parse Seedesk.cn site
                    case "SeeDesk":
                        return Fetch.Parse(url, fnAddress: a => "http://www.seedesk.cn" + a.Replace("_s.jpg", ".jpg"));
                    #endregion

                    #region H2004.com site
                    case "H2004":
                        return Fetch.Parse(url,
                            hnc =>
                            {
                                var page = hnc[hnc.Count - 2];
                                return page.Name == "font" ? null : page.Attributes["tppabs"].Value;
                            }, fnAddress: _ => "http://www.h2004.com" + _.Remove(0, 5));
                    #endregion

                    #region Faloo.com site
                    case "Faloo":
                        return Fetch.Parse(url, fnAddress: _ => _.Replace("120x120", "0x0"));
                    #endregion

                    #region 6611.us site
                    case "6611":
                        return Fetch.Parse(url, fnAddress: _ => _.Replace("/thumb/", "/"));
                    #endregion

                    #region piclove.com site
                    case "PicLove":
                        return Fetch.Parse(url, fnAddress: a => "http://www.piclove.com" + a.Replace("-lp.jpg", ".jpg"));
                    #endregion

                    #region KissQi.com site
                    case "KissQi":
                        return Fetch.Parse(url, hnc =>
                        {
                            var page = hnc.Last(); var href = page.Attributes["href"].Value.Split("()".ToCharArray())[1];
                            var segs = href.Split(','); var next = segs[segs.Length - 1].Trim('\'').Replace("[page]", segs[segs.Length - 2]);
                            return next.CompareWithLength(url) ? next : null;
                        },
                            fnAddress: _ => "http://www.kissqi.com" + _.TrimStart('.'));
                    #endregion

                    #region 1meng.com site
                    case "Meng":
                        return Fetch.Parse(url, fnName: _ => _.Substring(_.LastIndexOf('=') + 1) + ".jpg");
                    #endregion

                    #region 24Meinv.com site
                    case "Meinv":
                        return Fetch.Parse(url, fnAddress: _ =>
                        {
                            var mark = _.LastIndexOf('/'); var path = _.Substring(0, mark + 1);
                            var name = _.Substring(mark + 1); name = name.Remove(0, 1); path = path.Replace("//pic.", "//img.");
                            return path + name;
                        });
                    #endregion

                    #region Leg99.cn site
                    case "Leg99":
                        return Fetch.Parse(url, fnNextPage: hnc =>
                        {
                            var cur = hnc.Last(_ => _.Attributes["selected"] != null);
                            var curidx = hnc.IndexOf(cur);
                            return curidx == hnc.Count - 1 ? null : folder + hnc[curidx + 1].Attributes["value"].Value;
                        });
                    #endregion

                    #region 8264.com site
                    case "8264":
                        return Fetch.Parse(url, fnAddress: _ => _.Replace(".thumb.jpg", ""));
                    #endregion

                    #region Luscious site
                    case "Luscious":
                        return Fetch.Parse(url, fnAddress: _ => _.Replace("thumb_100_", "").Replace("static3.", "static.").Replace("static2.", "static."));
                    #endregion

                    #region LegPic.net site
                    case "LegPic":
                        return Fetch.Parse(url, fnAddress: _ => "http://www.legpic.net/" + _.TrimStart("../".ToCharArray()).Replace("/thumbnail/TN-", "/"),
                        fnNextPage: _ =>
                        {
                            var next = _.LastOrDefault();
                            if (next == null) return null;
                            return "http://www.legpic.net/" + next.Attributes["href"].Value.TrimStart("../".ToCharArray());
                        });
                    #endregion

                    #region BeautyLeg.com site
                    case "BLeg":
                        return Fetch.Parse(url, fnAddress: _ => _.StartsWith("http://photo.beautyleg.com/album")?_ :null
                        );
                    #endregion

                    #region Parse other sites
                    default:
                        return Fetch.Parse(url);
                    #endregion
                }
            }
            catch (Exception)
            {
                return (Fetch.Title == null ? "Download" : "Analyze") + " Failed!";
            }
        }

        private void DownloadFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Fetch.Reset();
            btnGo.Tag = RipperAction.Download;
            tsPB.Visible = false;
            lbBatch.Text = null;
            System.Media.SystemSounds.Exclamation.Play();
            if (e.Cancelled || e.Error != null)
            {
                tsLabel.Text = e.Error != null ? e.Error.Message : "Task cancelled.";
                Stop();
            }
            else
            {
                tsLabel.Text = e.Result != null ? e.Result as string : "Task finished.";
                if (Fetch.Next != null)
                {
                    Address = Fetch.Next;
                    Go();
                }
                else if (Batch)
                {
                    From++;
                    AdjustURL(1);
                    Batch = From != To;
                    Go();
                }
                else
                {
                    Stop();
                }
            }
        }

        private void Stop()
        {
            btnGo.Image = Resources.Download;
            btnGo.Enabled = true;
            cmmiBatch.Enabled = false;
            tbParse.ReadOnly = false;
            tbDir.ReadOnly = false;
        }

        private void Go()
        {
            btnGo.Image = Resources.Cancel;
            btnGo.Tag = RipperAction.Cancel;
            bwFetch.RunWorkerAsync();
        }

        private void DownloadFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tsPB.Value = e.ProgressPercentage;
            tsLabel.Text = e.ProgressPercentage + "%";
        }

        private void pbDoubleClick(object sender, EventArgs e)
        {
            if (Fetch.Location != null && File.Exists(Fetch.Location))
                Process.Start(Fetch.Location);
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
            string number, url = Address;
            try
            {
                switch (Fetch.Site.Type)
                {
                    default:
                        var mark = url.LastIndexOf('=');
                        if (mark > 0)
                            number = url.Substring(mark + 1);
                        else
                        {
                            mark = url.LastIndexOf('.');
                            var start = url.LastIndexOf('/') + 1;
                            number = mark > 0 ? url.Substring(start, mark - start) : url.Substring(start);
                        }
                    ChangeUrl:
                        var value = int.Parse(number);
                        value += step; if (value < 0) return;
                        var m = url.LastIndexOf(number);
                        Address = url.Substring(0, m) + value.ToString(new string('0', number.Length)) + url.Substring(m + number.Length);
                        break;
                    case "Duide":
                        number = url.Substring(url.LastIndexOf('/') + 2).Replace(".htm", "");
                        goto ChangeUrl;
                    case "PalAthCx":
                        var seg = url.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        number = seg[seg.Length - 1];
                        goto ChangeUrl;
                }
            }
            catch (Exception)
            {
                return;
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
            Fetch.Site = CheckUrl(Address);
            try
            {
                switch (Fetch.Site.Type)
                {
                    default:
                        string text = Address.Split('=')[1];
                    OpenBatchDialog:
                        int pageid;
                        if (int.TryParse(text, out pageid))
                            new Batch(pageid).ShowDialog(this);
                        break;
                    case "Duide":
                        text = Address.Substring(Address.LastIndexOfAny("abc".ToCharArray()) + 1).Split('.')[0];
                        goto OpenBatchDialog;
                    case "PalAthCx":
                        var seg = Address.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        text = seg[seg.Length - 1];
                        goto OpenBatchDialog;
                }
            }
            catch (Exception)
            { return; }
        }

        private void cmmiNextPage_Click(object sender, EventArgs e)
        {
            if (Batch)
                Fetch.Skip = true;
        }

        private void cmmiDelete_Click(object sender, EventArgs e)
        {
            if (File.Exists(Fetch.Location))
            {
                File.Delete(Fetch.Location);
                var lvi = lvRip.FindItemWithText(Path.GetFileName(Fetch.Location));
                Fetch.Location = null;
                if (lvi != null)
                {
                    lvi.SubItems[3].Text = "Deleted";
                    lvi.Font = new Font(lvi.Font, FontStyle.Strikeout);
                }
            }
        }

        private void cmmiDrop_Click(object sender, EventArgs e)
        {
            if (Fetch.Images != null && Fetch.Images.Count > 0)
            {
                foreach (string name in Fetch.Images.Keys)
                {
                    if (Fetch.Current != null && Fetch.Current.Name == name) { Fetch.Dropped = true; Fetch.Cancel(); continue; }
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
                Fetch.Skip = true;
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
                    Fetch.Location = pbPreview.ImageLocation = file;
                }
                else
                    Fetch.Location = null;
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
            var args = new List<Tuple<string, string>>();
            foreach (ListViewItem lvi in lvRip.SelectedItems)
            {
                if (!File.Exists(Path.Combine(Dir, lvi.Text)))
                {
                    args.Add(Tuple.Create(lvi.ToolTipText/*Url*/, lvi.Text/*Name*/));
                    lvi.SubItems[3].Text = "Downloading";
                    lvi.Font = new Font(lvi.Font, FontStyle.Regular);
                }
            }
            if (args.Count > 0)
                new Thread(DownloadFile).Start(args);
        }

        void DownloadFile(object args)
        {
            var dfi = args as List<Tuple<string, string>>;
            foreach (var item in dfi)
            {
                try
                {
                    string path = Path.Combine(Dir, item.Item2);
                    if (Fetch.Site.Type == "Heels")
                    {
                        using (Stream s = Fetch.GetStream(item.Item1, Settings.Default.Cookie))
                        {
                            using (Image bmp = Image.FromStream(s))
                            {
                                s.Close(); bmp.Save(path);
                                bmp.Dispose();
                            }
                        }
                    }
                    else
                        Fetch.GetFile(item.Item1, path);
                    LviUpdate = new[] { item.Item2, null, new FileInfo(path).Length / 1024 + " KB", "Finished" };
                }
                catch (Exception)
                {
                    LviUpdate = new[] { item.Item2, null, null, "Failed" };
                }
            }
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
                    if (tbParse.Focused || tbDir.Focused) break;
                    if (e.Shift) { mainSplit.Panel2Collapsed = !mainSplit.Panel2Collapsed; cmmiPreview.Checked = !mainSplit.Panel2Collapsed; break; }
                    mainSplit.Panel1Collapsed = !mainSplit.Panel1Collapsed;
                    cmmiPreview.Checked = !mainSplit.Panel1Collapsed;
                    break;
                case Keys.Escape:
                    if (FormBorderStyle == FormBorderStyle.None)
                    {
                        FullScreen = false;
                        FormBorderStyle = FormBorderStyle.Sizable;
                        Bounds = (Rectangle)this.Tag;
                        mainSplit.Panel1Collapsed = false;
                        if ((Fetch.Location == null) && pbPreview.Image == null)
                            mainSplit.Panel2Collapsed = true;
                    }
                    break;
                case Keys.F11:
                    if (FullScreen)
                    {
                        FullScreen = false;
                        FormBorderStyle = FormBorderStyle.Sizable;
                        Bounds = (Rectangle)this.Tag;
                        if ((Fetch.Location == null) && pbPreview.Image == null)
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
                if ((Fetch.Site = CheckUrl(Address)) == null) { tsLabel.Text = "Not support!"; return false; }
#if  !TRACE
                if (!Directory.Exists(Dir))
                {
                    if (DialogResult.Yes == MessageBox.Show("Do you want to create new folder to store files?", "Directory doesn't exist!", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        try { Directory.CreateDirectory(Dir); return true; }
                        catch (Exception exp) { MessageBox.Show(exp.Message, "Create Directory failed!", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                    }
                    else
                        return false;
                }
#endif
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
            cmmiNextPage.Visible = cmmiBatch.Visible = Batch;
            cmmiSave.Visible = cmmiRemove.Visible = cmmiCopyName.Visible = lvRip.SelectedItems.Count > 0;
            cmmiDropGroup.Visible = cmmiClear.Visible = lvRip.Items.Count > 0;
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
            new WebCloud() { Text = tsmi.Text }.Show();
        }

        private void tsHome_Click(object sender, EventArgs e)
        {
            Process.Start("http://imgrip.codeplex.com");
        }

        private void llSites_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Sites().Show(this);
        }

        private void tb_MouseClick(object sender, MouseEventArgs e)
        {
            var ctl = sender as TextBox;
            if (string.IsNullOrEmpty(ctl.Tag as string)) { if (ctl.SelectionLength == 0) ctl.SelectAll(); ctl.Tag = ctl.SelectedText; }
        }

        private void tb_Leave(object sender, EventArgs e)
        {
            var ctl = sender as TextBox;
            ctl.Tag = null;
        }

        private void cmmiBatch_CheckedChanged(object sender, EventArgs e)
        {
            Batch = cmmiBatch.Checked;
        }
    }

    /// <summary>
    /// Represent the download button action type.
    /// </summary>
    enum RipperAction { Download, Cancel };
}