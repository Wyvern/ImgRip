namespace ImgRipper
{
    using System.Collections.Specialized;
    using System.Drawing;
    using System.IO;
    using System.Net;
    
    class Fetcher
    {
        public Fetcher()
        {
            Imgs = new NameValueCollection();
        }
        #region Properties definitions
        public NameValueCollection Imgs { get; set; }
        public bool Canceled { get; set; }
        public RipperAction PushState { get; set; }
        public ParseStyle Style { get; set; }
        public FileInfo Current { get; set; }
        public string ImageLocation { get; set; }
        public bool SkipPage { get; set; }
        public string Title { get; set; }
        public bool TooSmall { get; set; }
        public string Address { get; set; }
        public string NextPage { get; set; }
        #endregion
        
        public void GetFile(string url, string cookie, out Bitmap bmp)
        {
            using (var wc = new WebClient())
            {
                wc.Headers["Cookie"] = "JSESSIONID=" + cookie;
                using (Stream s = wc.OpenRead(url))
                {
                    bmp = new Bitmap(s);
                    s.Close();
                }
            }
        }
        
        public void GetFile(string url, string file)
        {
            using (var wc = new WebClient())
            {
                wc.Headers["Referer"] = url;
                wc.DownloadFile(url, file);
            }
        }

        public void Reset()
        {
            PushState = RipperAction.Download;
            Imgs.Clear();
            Canceled = false;
            Title = default(string);
            Address = default(string);
            Title = default(string);
        }
    }
    /// <summary>
    /// Supported parseable web site Enumeration.
    /// </summary>
    enum ParseStyle {NotSupport, Heels, Duide, KeAiBbs, Tu11, MeiTuiJi, PalAthCx};
    /// <summary>
    /// Represent the download button action type.
    /// </summary>
    enum RipperAction { Download, Cancel };
}
