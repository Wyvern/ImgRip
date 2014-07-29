<<<<<<< HEAD
﻿namespace ImgRip
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;

    partial class Sites : Form
    {
        public Sites()
        {
            InitializeComponent();
            flPanel.MouseWheel += flPanel_MouseWheel;
        }

        void flPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!flPanel.HasChildren) return;
            var hs = flPanel.HorizontalScroll;
            var width = flPanel.Controls[0].Width;
            int val = hs.Value;
            val += e.Delta < 0 ? width : -width;
            if (val < 0) { hs.Value = 0; flPanel.ScrollControlIntoView(flPanel.Controls[0]); }
            else if (val > hs.Maximum) hs.Value = hs.Maximum;
            else hs.Value = val;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            flPanel_MouseWheel(flPanel, e);
        }

        static readonly string labelPrompt = "http://{0}", sitePrompt = "Total {0} sites.";

        void item_MouseEnter(object sender, System.EventArgs e)
        {
            Prompt.Text = string.Format(labelPrompt, ((LinkLabel)sender).Tag as string);
        }

        void item_MouseLeave(object sender, System.EventArgs e)
        {
            Prompt.Text = string.Format(sitePrompt, flPanel.Controls.Count);
        }

        void item_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(string.Format(labelPrompt, ((LinkLabel)sender).Tag as string));
        }

        private void Sites_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27) this.Close();
        }

        static LinkLabel[] lla;

        private void Sites_Load(object sender, EventArgs e)
        {
            var owner = this.Owner as Main;
            lla = new LinkLabel[owner.sites.Length];
            int i = 0;
            foreach (var site in owner.sites)
            {
                var item = new LinkLabel { Text = site.Name, Tag = site.ToString()};
                item.LinkClicked += item_LinkClicked;
                item.MouseEnter += item_MouseEnter;
                item.MouseLeave += item_MouseLeave;
                lla[i++] = item;
            }
            flPanel.Controls.AddRange(lla);
            Prompt.Text = string.Format(sitePrompt, flPanel.Controls.Count);
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            var term = tbSearch.Text.Trim();
            if (string.IsNullOrEmpty(term))
                if (tbSearch.Text.Length > 0) return;
                else { if (lla.Length != flPanel.Controls.Count) { flPanel.SuspendLayout(); flPanel.Controls.Clear(); flPanel.Controls.AddRange(lla); } }
            else
            {
                var match = lla.Where(_ => ((string)_.Tag).ContainsEx(term)).ToArray();
                if (match.Length == lla.Length && lla.Length == flPanel.Controls.Count) return;
                flPanel.SuspendLayout(); flPanel.Controls.Clear(); flPanel.Controls.AddRange(match);
            }
            flPanel.ResumeLayout();
            Prompt.Text = string.Format(sitePrompt, flPanel.Controls.Count);
        }
    }
}
=======
﻿namespace ImgRip
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;

    partial class Sites : Form
    {
        public Sites()
        {
            InitializeComponent();
        }

        void item_MouseEnter(object sender, System.EventArgs e)
        {
            var ll = sender as LinkLabel;
            Prompt.Text = string.Format("{0}", ll.Tag as string);
        }

        void item_MouseLeave(object sender, System.EventArgs e)
        {
            Prompt.Text = string.Format("Total {0} sites.", flPanel.Controls.Count);
        }

        void item_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(((LinkLabel)sender).Tag as string);
        }

        private void Sites_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27) this.Close();
        }

        LinkLabel[] lla;

        private void Sites_Load(object sender, EventArgs e)
        {
            var owner = this.Owner as Main;
            flPanel.SuspendLayout();
            foreach (var site in owner.sites)
            {
                var item = new LinkLabel { Text = site.Name, Tag = "http://" + site.ToString(), AutoSize = true };
                item.LinkClicked += item_LinkClicked;
                item.MouseEnter += item_MouseEnter;
                item.MouseLeave += item_MouseLeave;
                flPanel.Controls.Add(item);
            }
            flPanel.ResumeLayout();
            lla = new LinkLabel[flPanel.Controls.Count];
            flPanel.Controls.CopyTo(lla, 0);
            Prompt.Text = string.Format("Total {0} sites.", flPanel.Controls.Count);
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            var term = tbSearch.Text.Trim();
            if (string.IsNullOrEmpty(term))
                if (tbSearch.Text.Length > 0) return;
                else { if (lla.Length != flPanel.Controls.Count) { flPanel.SuspendLayout(); flPanel.Controls.Clear(); flPanel.Controls.AddRange(lla); } }
            else
            {
                var match = lla.Where(_ => ((string)_.Tag).ContainsEx(term)).ToArray();
                if (match.Length == lla.Length && lla.Length == flPanel.Controls.Count) return;
                flPanel.SuspendLayout(); flPanel.Controls.Clear(); flPanel.Controls.AddRange(match);
            }
            flPanel.ResumeLayout();
            Prompt.Text = string.Format("Total {0} sites.", flPanel.Controls.Count);
        }
    }
}
>>>>>>> parent of fb9b497... Sync from codeplex
