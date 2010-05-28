namespace ImgRipper
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Threading;
    
    class Fetcher
    {
        public Fetcher()
        {
            Imgs = new NameValueCollection();
        }
        WebClient wc;
        #region Properties Definitions
        public NameValueCollection Imgs { get; set; }
        public bool Canceled { get; set; }
        public bool Dropped { get; set; }
        public RipperAction PushState { get; set; }
        public ParseStyle Style { get; set; }
        public FileInfo Current { get; set; }
        public string ImageLocation { get; set; }
        public bool SkipPage { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string NextPage { get; set; }
        #endregion

        public Stream GetStream(string url, string cookie)
        {
            Stream r = null;
            using (wc = new WebClient())
            {
                wc.Headers["Cookie"] = string.Format("JSESSIONID={0}", cookie);
                var mre = new ManualResetEvent(false);
                wc.OpenReadCompleted += (s, e) => { if (!e.Cancelled) r = e.Result; mre.Set(); };
                wc.OpenReadAsync(new Uri(url));
                mre.WaitOne(); mre.Close();
            }
            return r;
        }

        public void GetFile(string url, string file)
        {
            using (wc = new WebClient())
            {
                wc.Headers["Referer"] = url;
                var mre = new ManualResetEvent(false);
                wc.DownloadFileCompleted += (s, e) => { if ((e.Cancelled || e.Error != null) && File.Exists(file)) File.Delete(file); mre.Set(); };
                wc.DownloadFileAsync(new Uri(url), file);
                mre.WaitOne(); mre.Close();
            }
        }
        public void Cancel()
        {
            if (wc != null) wc.CancelAsync();
        }

        public void Reset()
        {
            PushState = RipperAction.Download;
            Imgs.Clear();
            Canceled = Dropped = false;
            Current = null;
            Title = Address = string.Empty;
        }
    }
    /// <summary>
    /// Supported parseable web site Enumeration.
    /// </summary>
    enum ParseStyle {NotSupport, Heels, Duide, Tu11, MeiTuiJi, PalAthCx, DeskCity, Pics100, WallCoo,Mtswa};
    /// <summary>
    /// Represent the download button action type.
    /// </summary>
    enum RipperAction { Download, Cancel };
}
