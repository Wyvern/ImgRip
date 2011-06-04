namespace ImgRip
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using Google.GData.Client;
    using HAP = HtmlAgilityPack;

    static class Helper
    {
        public static bool CompareWithLength(this string src, string s)
        {
            if (src.Length != s.Length)
            {
                return src.Length > s.Length;
            }
            else
                return string.Compare(src, s, true) > 0;
        }

        public static bool ContainsEx(this string src, string check, StringComparison sc=StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(check)) return false;
            return src.IndexOf(check, sc) >= 0;
        }
    }

    static class Fetch
    {
        static WebClient wc;

        #region Properties Definitions
        public static Dictionary<string, string> Images { get; private set; }
        public static bool Canceled { get; set; }
        public static bool Dropped { get; set; }
        public static Site Site { get; set; }
        public static FileInfo Current { get; set; }
        public static string Location { get; set; }
        public static bool Skip { get; set; }
        public static string Title { get; set; }
        public static string Address { get; set; }
        public static string Next { get; set; }
        public static string Screen { get; private set; }
        #endregion

        public static Stream GetStream(string url, string cookie = null)
        {
            Stream r = null;
            using (wc = new WebClient())
            {
                if (cookie != null)
                    wc.Headers[HttpRequestHeader.Cookie] = string.Format("JSESSIONID={0}", cookie);
                var are = new AutoResetEvent(false);
                wc.OpenReadCompleted += (s, e) => { if (!e.Cancelled) r = e.Result; are.Set(); };
                wc.OpenReadAsync(new Uri(url));
                are.WaitOne(); are.Close();
            }
            return r;
        }

        public static void GetFile(string url, string file)
        {
            using (wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.Referer] = url;
                var are = new AutoResetEvent(false);
                wc.DownloadFileCompleted += (s, e) =>
                { if ((e.Cancelled || e.Error != null) && File.Exists(file)) File.Delete(file); are.Set(); };
                wc.DownloadFileAsync(new Uri(url), file);
                are.WaitOne(); are.Close();
            }
        }
        public static void Cancel()
        {
            if (wc != null) wc.CancelAsync();
        }

        public static string Parse(string url, Func<HAP.HtmlNodeCollection, string> fnNextPage = null,
            Func<string, string> fnName = null, Func<string, string> fnAddress = null, Func<HAP.HtmlNode, string> fnScreen = null)
        {
            var hw = new HAP.HtmlWeb();
            var doc = hw.Load(url);
            if (Canceled) return "User Cancelled.";
            Title = doc.DocumentNode.SelectSingleNode("//title").InnerText;
            Title = Title.Trim().Split('-', '_', '(', '[', '|', '.', ':', '、')[0];
            var uri = new Uri(url); var host = uri.Host; var port = uri.Port;
            var folder = url.Substring(0, url.LastIndexOf('/') + 1);
            fnName = fnName ?? (n => n.Substring(n.LastIndexOf('/') + 1));
            fnAddress = fnAddress ?? (a => a.StartsWith("http://") ? a :
                (string.Format("http://{0}{1}{2}", host, port == 80 ? "" : ":" + port.ToString(), a.StartsWith("/") ? "" : "/") + a));
            #region Get screen resolution info
            if (!string.IsNullOrEmpty(Site.Screen))
            {
                var screenlist = doc.DocumentNode.SelectSingleNode(Site.Screen);
                Screen = fnScreen(screenlist);
            }
            #endregion
            var links = doc.DocumentNode.SelectNodes(Site.Image);
            if (links == null || links.Count == 0) return "No gallery found in this page.";
            Images = new Dictionary<string, string>();
            #region Get Photo collection
            foreach (var lnk in links)
            {
                var item = lnk;
                switch (Site.Type)
                {
                    case "PalAthCx":
                        var href = lnk.ParentNode.Attributes["href"].Value;
                        if (!href.EndsWith(".html", StringComparison.OrdinalIgnoreCase)) continue;
                        break;
                    case "Leg99":
                        item = lnk.ParentNode;
                        break;
                }

                var atag = item.Name.ToLower() == "a";
                var src = atag ? item.Attributes["href"].Value.Trim() : item.Attributes["file"] != null ? item.Attributes["file"].Value.Trim() : item.Attributes["src"].Value.Trim();
                var address = fnAddress(HttpUtility.HtmlDecode(src));
                var name = item.InnerText == string.Empty ? fnName(address) : item.InnerText;
                Images[name] = address;
                System.Diagnostics.Trace.WriteLine(string.Format("Name:\"{0}\"\n\tUrl:\t\"{1}\"", name, address));
            }
            System.Diagnostics.Trace.WriteLine(string.Format("Total <{0}> photos in page \"{1}\".", Images.Count, Title));
            #endregion
            if (string.IsNullOrEmpty(Site.Next)) return null;
            #region Get Next page url
            fnNextPage = fnNextPage ?? (hnc =>
            {
                if (hnc == null) return null;
                var page = hnc.LastOrDefault();
                if (page == null || page.Name != "a" || page.Attributes["href"] == null) return null;
                var href = page.Attributes["href"].Value; if (href == "#" || href.StartsWith("javascript")) return null;
                var next = href.StartsWith("http://") ?
                href : href.StartsWith("./") ?
                href.Replace("./", folder) : href.StartsWith("/") ?
                string.Format("http://{0}{1}", host, port == 80 ? "" : ":" + port.ToString()) + href : (href.StartsWith("?") ?
                url.Split('?')[0] + href : href.IndexOf('/') > 0 ? string.Format("http://{0}/", host) : folder) + href;
                if (Site.Type == "BitAuto") return next;
                var pq = new Uri(next).PathAndQuery.TrimEnd('/');
                var cur = uri.PathAndQuery.TrimEnd('/');
                return pq.CompareWithLength(cur) ? next : null;
            });
            #endregion
            var pages = doc.DocumentNode.SelectNodes(Site.Next);
            Next = pages == null ? null : fnNextPage(pages);
            System.Diagnostics.Trace.WriteLine(string.Format("Next page is: {0}", Next));
            return null;
        }

        public static void Reset()
        {
            if (Images != null) Images.Clear();
            Canceled = Dropped = Skip = false;
            Current = null;
            Title = Address = string.Empty;
        }
    }

    class Site
    {
        public Site(string name, string domain, string xpImage, string xpNext=null)
        {
            Name = name;
            Domain = domain;
            Image = xpImage;
            Next = xpNext;
        }

        string Domain { get; set; }

        public string Image { get; private set; }
        public string Next { get; set; }
        public string Name { get; private set; }
        public string Screen { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return Domain;
        }
    }
}
