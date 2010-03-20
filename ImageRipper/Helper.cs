namespace ImgRipper
{
    using System;
    using System.Windows.Forms;
    /// <summary>
    /// UI Callback Helper class for extension method
    /// </summary>
    static class UICallBack
    {
        public static void cbEnable(this Control ctl, bool enable)
        {
            ctl.Invoke(new Action(() => ctl.Enabled = enable));
        }
        public static void cbAdd(this ListView lv, string text, int imgindex, string tip)
        {
            lv.Invoke(new Action(()=>lv.Items.Add(new ListViewItem(text, imgindex) { ToolTipText = tip })));
        }
    }
}
