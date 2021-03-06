<<<<<<< HEAD
﻿namespace ImgRip
{
    using System;
    using System.Windows.Forms;
    using Google.GData.Client;
    /// <summary>
    /// UI Callback Helper
    /// </summary>
    static class UICallBack
    {
        public static void cbEnable(this Control ctl, bool enable)
        {
            if (ctl.IsHandleCreated)
                ctl.Invoke(new Action(() => ctl.Enabled = enable));
        }

        public static void EnableControls(bool enable, params Control[] ctls)
        {
            foreach (var ctl in ctls)
            {
                if (ctl.IsHandleCreated)
                    ctl.Invoke(new Action(() => ctl.Enabled = enable));
            }
        }

        public static void cbAdd(this ListView lv, object data, int imgindex)
        {
            if (lv.IsHandleCreated)
                switch (WebCloud.Service)
                {
                    case WebCloud.CloudType.Flickr:
                        break;
                    case WebCloud.CloudType.Facebook:
                        break;
                    case WebCloud.CloudType.GDrive:
                    case WebCloud.CloudType.Picasa:
                        var ae = data as AtomEntry;
                        lv.Invoke(new Action(() => lv.Items.Add(new ListViewItem(ae.Title.Text, imgindex) { Tag = ae, ToolTipText = ae.AlternateUri.Content })));
                        break;
                }
        }
    }
}
=======
﻿namespace ImgRip
{
    using System;
    using System.Windows.Forms;
    using Google.GData.Client;
    /// <summary>
    /// UI Callback Helper
    /// </summary>
    static class UICallBack
    {
        public static void cbEnable(this Control ctl, bool enable)
        {
            if (ctl.IsHandleCreated)
                ctl.Invoke(new Action(() => ctl.Enabled = enable));
        }

        public static void EnableControls(bool enable, params Control[] ctls)
        {
            foreach (var ctl in ctls)
            {
                if (!ctl.IsHandleCreated) break;
                ctl.Invoke(new Action(() => ctl.Enabled = enable));
            }
        }

        public static void cbAdd(this ListView lv, object data, int imgindex)
        {
            if (lv.IsHandleCreated)
                switch (WebCloud.Service)
                {
                    case WebCloud.CloudType.Flickr:
                        break;
                    case WebCloud.CloudType.Facebook:
                        break;
                    case WebCloud.CloudType.GDrive:
                    case WebCloud.CloudType.Picasa:
                        var ae = data as AtomEntry;
                        lv.Invoke(new Action(() => lv.Items.Add(new ListViewItem(ae.Title.Text, imgindex) { Tag = ae, ToolTipText = ae.AlternateUri.Content })));
                        break;
                }
        }
    }
}
>>>>>>> parent of fb9b497... Sync from codeplex
