namespace ImgRipper
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;

    using Google.GData.Client;
    using Google.GData.Documents;
    using Google.Documents;
    using Google.GData.Photos;
    using Google.Picasa;

    partial class WebCloud : Form
    {
        public WebCloud()
        {
            InitializeComponent();
        }

        string LoginName { get { return tbName.Text.Trim(); } }
        bool Aborted = false;
        string Prompt { set { cldStatus.Invoke(new Action(() => CloudStatus.Text = value)); } }
        
        //Local ListViewItems memory cache
        Collection<ListViewItem> cldCache;
        Stack<Document> Folder;
        DocumentsRequest DR;
        PicasaRequest PR;
        public string AlbumID { get; set; }

        internal static CloudType Service { get; set; }

        // Supported Cloud storage service
        internal enum CloudType
        {
            GDrive, Flickr, Facebook, Picasa
        }

        private void ConnectCloud_Click(object sender, EventArgs e)
        {
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    CloudStatus.Text = "Waiting..."; lvCloud.Items.Clear();
                    DR = new DocumentsRequest(new RequestSettings("Ripper", LoginName, tbPass.Text) {AutoPaging=true});
                    new Thread(new ThreadStart(() =>
                    {
                        try { DR.Service.QueryClientLoginToken(); }
                        catch (Exception exp) { Prompt = exp.Message; btnSign.cbEnable(true); return; }
                        var items = DR.GetFolders().Entries;
                        foreach (var item in items)
                            if (item.ParentFolders.Count == 0)
                                lvCloud.cbAdd(item.AtomEntry, 0);
                        if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                        btnSign.cbEnable(true);
                        txtFolderName.cbEnable(true);
                        btnCreate.cbEnable(true);
                        btnDelete.cbEnable(false);
                        btnUp.cbEnable(false);
                        btnAdd.cbEnable(false);
                        Prompt = items.Count() + " item(s)";
                    })).Start();
                    break;
                #endregion

                #region Flickr
                case CloudType.Flickr:
                    break;
                #endregion

                #region Facebook
                case CloudType.Facebook:
                    break;
                #endregion

                #region Picasa
                case CloudType.Picasa:
                    CloudStatus.Text = "Waiting..."; lvCloud.Items.Clear();
                    PR = new PicasaRequest(new RequestSettings("Ripper", LoginName, tbPass.Text) { AutoPaging=true});
                    new Thread(new ThreadStart(() =>
                    {
                        try { PR.Service.QueryClientLoginToken(); }
                        catch (Exception exp) { Prompt = exp.Message; btnSign.cbEnable(true); return; }
                        var items = PR.GetAlbums().Entries;
                        foreach (var item in items)
                            lvCloud.cbAdd(item.AtomEntry, 0);
                        if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                        txtFolderName.cbEnable(true);
                        btnCreate.cbEnable(true);
                        cbPublic.cbEnable(false);
                        btnUp.cbEnable(false);
                        btnAdd.cbEnable(false);
                        btnDelete.cbEnable(false);
                        btnSign.cbEnable(true);
                        Prompt = items.Count() + " Album(s)";
                    })).Start();
                    break;
                #endregion
            }
            btnSign.Enabled = false;
            Properties.Settings.Default.Save();
        }

        private void lvCloud_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lvCloud.FocusedItem != null)
            {
                ListViewItem lvi = lvCloud.FocusedItem;
                switch (Service)
                {
                    #region GDrive
                    case CloudType.GDrive:
                        Document doc = new Document() { AtomEntry = lvi.Tag as AtomEntry };
                        if (doc.Type==Document.DocumentType.Folder)
                        {
                            Folder = Folder ?? new Stack<Document>();
                            CloudStatus.Text = "Listing \"" + doc.Title + "\"";
                            Folder.Push(doc);
                            btnDelete.Enabled = btnAdd.Enabled = btnCreate.Enabled = false;
                            lvCloud.Items.Clear();
                            new Thread(new ThreadStart(() =>
                            {
                                var items = DR.GetFolderContent(doc).Entries;
                                foreach (var item in items)
                                    lvCloud.cbAdd(item.AtomEntry, item.Type == Document.DocumentType.Folder ? 0 : 2);
                                if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                                btnUp.cbEnable(true);
                                btnCreate.cbEnable(true);
                                btnAdd.cbEnable(true);
                                txtFolderName.cbEnable(true);
                                Prompt = string.Join(" > ", Folder.Select(_ => _.Title).Reverse().ToArray()) + ": " + items.Count() + " item(s)";
                            })).Start();
                        }
                        break;
                    #endregion

                    #region Flickr
                    case CloudType.Flickr:
                        break;
                    #endregion

                    #region Facebook
                    case CloudType.Facebook:
                        break;
                    #endregion

                    #region Picasa
                    case CloudType.Picasa:
                        Album a = new Album() { AtomEntry = lvi.Tag as AtomEntry };
                        if (a != null)
                        {
                            CloudStatus.Text = "Listing \"" + a.Title + "\"";
                            btnDelete.Enabled = cbPublic.Enabled = txtFolderName.Enabled = false;
                            lvCloud.Items.Clear();
                            AlbumID = a.Id;
                            new Thread(new ThreadStart(() =>
                            {
                                var items = PR.GetPhotosInAlbum(AlbumID).Entries;
                                foreach (var item in items)
                                    lvCloud.cbAdd(item.AtomEntry, 1);
                                if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                                btnUp.cbEnable(true);
                                btnCreate.cbEnable(false);
                                txtFolderName.cbEnable(true);
                                btnAdd.cbEnable(true);
                                Prompt = a.Title + ": " + items.Count() + " Photo(s)";
                            })).Start();
                        }
                        break;
                    #endregion
                }
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    if (Folder == null || Folder.Count == 0) return;
                    Document doc = Folder.Pop();
                    lvCloud.Items.Clear();
                    btnUp.Enabled = btnDelete.Enabled = btnCreate.Enabled = txtFolderName.Enabled = btnAdd.Enabled = false;
                    if (Folder.Count == 0)
                    {
                        CloudStatus.Text = "Waiting...";
                        new Thread(new ThreadStart(() =>
                        {
                            var items = DR.GetFolders().Entries;
                            foreach (var item in items)
                                if (item.ParentFolders.Count == 0)
                                    lvCloud.cbAdd(item.AtomEntry, 0);
                            if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                            txtFolderName.cbEnable(true);
                            //btnAddFiles.cbEnable(true);
                            btnCreate.cbEnable(true);
                            btnUp.cbEnable(false);
                            Prompt = items.Count() + " item(s)";
                        })).Start();
                    }
                    else
                    {
                        doc = Folder.Peek();
                        CloudStatus.Text = "Listing \"" + doc.Title + "\"";
                        new Thread(new ThreadStart(() =>
                        {
                            var items = DR.GetFolderContent(doc).Entries;
                            foreach (var item in items)
                                lvCloud.cbAdd(item.AtomEntry, item.Type == Document.DocumentType.Folder ? 0 : 2);
                            if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                            btnUp.cbEnable(true);
                            btnAdd.cbEnable(true);
                            btnCreate.cbEnable(true);
                            txtFolderName.cbEnable(true);
                            Prompt = string.Join(" > ", Folder.Select(_ => _.Title).Reverse().ToArray()) + ": " + items.Count() + " item(s)";
                        })).Start();
                    }
                    break;
                #endregion

                #region Flickr
                case CloudType.Flickr:
                    break;
                #endregion

                #region Facebook
                case CloudType.Facebook:
                    break;
                #endregion

                #region Picasa
                case CloudType.Picasa:
                    lvCloud.Items.Clear();
                    CloudStatus.Text = "Waiting...";
                    btnUp.Enabled = btnDelete.Enabled = btnCreate.Enabled = txtFolderName.Enabled = btnAdd.Enabled = false;
                    new Thread(new ThreadStart(() =>
                    {
                        var items = PR.GetAlbums().Entries;
                        foreach (var item in items)
                            lvCloud.cbAdd(item.AtomEntry, 0);
                        if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                        AlbumID = null;
                        txtFolderName.cbEnable(true);
                        btnCreate.cbEnable(true);
                        Prompt = items.Count() + " Albums(s)";
                    })).Start();
                    break;
                #endregion
            }
        }

        private void btnCreateFolder_Click(object sender, EventArgs e)
        {
            string foldername = txtFolderName.Text.Trim();
            if (string.IsNullOrEmpty(foldername)
                || foldername.EndsWith(".")
                || foldername.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            { CloudStatus.Text = "Invalid folder name!"; return; }
            if (lvCloud.FindItemWithText(foldername) != null) { CloudStatus.Text = "\"" + foldername + "\" already existed."; return; }
            CloudStatus.Text = "Creating \"" + foldername + "\"";
            txtFolderName.Enabled = false;
            btnCreate.Enabled = false;
            btnUp.Enabled = false;
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    Document @base = null;
                    string title, tip;
                    if (Folder != null && Folder.Count > 0) @base = Folder.Peek();
                    new Thread(new ThreadStart(() =>
                    {
                        var @new = DR.CreateDocument(new Document() { Title = foldername, Type = Document.DocumentType.Folder });
                        if (@base != null)
                            @new = DR.MoveDocumentTo(@base, @new);
                        title = @new.Title; tip = @new.AtomEntry.AlternateUri.Content;
                        lvCloud.cbAdd(@new.AtomEntry, 0);
                        if (cldCache != null) cldCache.Add(new ListViewItem(title, 0) { Tag = @new.AtomEntry, ToolTipText = tip });
                        txtFolderName.cbEnable(true);
                        btnCreate.cbEnable(true);
                        btnUp.cbEnable(@base != null);
                        Prompt = (@base == null ? "GDrive" : string.Join(" > ", Folder.Select(f => f.Title).Reverse().ToArray())) + ": " + lvCloud.Items.Count + " item(s)";
                    })).Start();
                    break;
                #endregion

                #region Flickr
                case CloudType.Flickr:
                    break;
                #endregion

                #region Facebook
                case CloudType.Facebook:
                    break;
                #endregion

                #region Picasa
                case CloudType.Picasa:
                    var a = new Album();
                    a.Title = foldername;
                    cbPublic.ThreeState = false;
                    a.Access = cbPublic.Checked ? "public" : "private";
                    new Thread(new ThreadStart(() =>
                    {
                        var @new = PR.Insert<Album>(new Uri(PicasaQuery.CreatePicasaUri(LoginName)), a);
                        lvCloud.cbAdd(@new.AtomEntry, 0);
                        if (cldCache != null) cldCache.Add(new ListViewItem(foldername, 0) { Tag = @new.AtomEntry, ToolTipText = @new.AtomEntry.AlternateUri.Content });
                        btnCreate.cbEnable(true);
                        txtFolderName.cbEnable(true);
                        Prompt = lvCloud.Items.Count + " item(s)";
                    })).Start();
                    break;
                #endregion
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int total = lvCloud.SelectedItems.Count;
            if (total == 0) { CloudStatus.Text = "No item(s) selected."; return; }
            switch (Service)
            {
                #region GDrive & Picasa
                case CloudType.GDrive:
                case CloudType.Picasa:
                    ListViewItem[] items = new ListViewItem[total];
                    for (int i = 0; i < total; i++)
                    {
                        items[i] = lvCloud.SelectedItems[i];
                    }
                    if (MessageBox.Show("Are you sure to delete items below:\n" + string.Join(", ", items.Select(i => i.Text).ToArray()), "Delete Cloud Items", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        btnDelete.Enabled = btnUp.Enabled = false;
                        new Thread(DeleteCloudItem).Start(items);
                    }
                    break;
                #endregion

                #region Flickr
                case CloudType.Flickr:
                    break;
                #endregion

                #region Facebook
                case CloudType.Facebook:
                    break;
                #endregion
            }
        }

        void DeleteCloudItem(object arg)
        {
            ListViewItem[] items = arg as ListViewItem[];
            Action<ListViewItem> RemoveItem = lvi => lvCloud.Items.Remove(lvi);
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    foreach (var lvi in items)
                    {
                        if (Aborted)
                        {
                            Prompt = "Operation Cancelled!";
                            btnUp.cbEnable((Folder != null && Folder.Count > 0));
                            Aborted = false;
                            return;
                        }
                        Prompt = "Deleting \"" + lvi.Text + "\"";
                        try
                        {
                            (lvi.Tag as AtomEntry).Delete();
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message, "Delete \"" + lvi.Text + "\" ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                        if (cldCache != null) cldCache.Remove(cldCache.Single(_ => _.ToolTipText == lvi.ToolTipText));
                        lvCloud.Invoke(RemoveItem, lvi);
                    }
                    btnUp.cbEnable((Folder != null && Folder.Count > 0));
                    Prompt = lvCloud.Items.Count + " items";
                    break;
                #endregion

                #region Flickr
                case CloudType.Flickr:
                    break;
                #endregion

                #region Facebook
                case CloudType.Facebook:
                    break;
                #endregion

                #region Picasa
                case CloudType.Picasa:
                    foreach (var lvi in items)
                    {
                        if (Aborted)
                        {
                            Prompt = "Operation Cancelled!";
                            btnUp.cbEnable(AlbumID != null);
                            Aborted = false;
                            return;
                        }
                        Prompt = "Deleting \"" + lvi.Text + "\"";
                        try
                        {
                            (lvi.Tag as AtomEntry).Delete();
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.InnerException.Message, "Delete \"" + lvi.Text + "\" ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                        if (cldCache != null) cldCache.Remove(cldCache.Single(_ => _.ToolTipText == lvi.ToolTipText));
                        lvCloud.Invoke(RemoveItem, lvi);
                    }
                    btnUp.cbEnable(AlbumID != null);
                    Prompt = lvCloud.Items.Count + (AlbumID == null ? " Albums(s)" : " Photos(s)");
                    break;
                #endregion
            }
        }

        void AddCloudFile(object arg)
        {
            string[] files = arg as string[];
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    Document @base = null;
                    if (Folder != null && Folder.Count > 0)
                        @base = Folder.Peek();
                    Dictionary<string, string> GDrive = new Dictionary<string, string>
                    { 
                    { ".jpg", "image/jpeg" },{".rtf",""},{".ppt",""},{".pps",""},{ ".htm", "" }, { ".html", "" },{".xls",""},{".xlsx",""},{".ods",""},
                    { ".png", "image/png" }, { ".gif", "image/gif" }, { ".tiff", "image/tiff" },{ ".bmp", "image/bmp" }, { ".mov", "video/quicktime" },
                     { ".psd", "application/photoshop" },{ ".avi", "video/x-msvideo"}, { ".mpg", "video/mpeg"}, { ".wmv", "video/x-ms-wmv" },
                     {".asf","video/x-ms-asf"},{".tif","video/x-ms-asf"},{".csv",""},{".tsb",""},{".doc",""},{".docx",""},{".txt","text/plain"},
                    };
                    foreach (var file in files)
                    {
                        if (Aborted) { Prompt = "Operation Cancelled!"; Aborted = false; return; }
                        string filename = Path.GetFileName(file), ext = Path.GetExtension(file).ToLower();
                        if (!GDrive.ContainsKey(ext)) continue;
                        Prompt = "Adding \"" + filename + "\"";
                        try
                        {
                            var de = string.IsNullOrEmpty(GDrive[ext]) ? 
                                DR.Service.UploadDocument(file, filename) :
                                DR.Service.UploadFile(file, filename, GDrive[ext], true);
                            var @new = new Document() { AtomEntry = de };
                            //de.Delete();
                            if (@base != null)
                            {
                                Prompt = "Moving \"" + @new.Title + "\" to " + @base.Title;
                                @new = DR.MoveDocumentTo(@base, @new);
                            }
                            string ttt = @new.AtomEntry.AlternateUri.Content;
                            lvCloud.cbAdd(@new.AtomEntry, 2);
                            if (cldCache != null) cldCache.Add(new ListViewItem(@new.Title, 2) { Tag = @new.AtomEntry, ToolTipText = ttt });
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message, "Upload \"" + filename + "\" ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }
                    Prompt = lvCloud.Items.Count + " items";
                    break;
                #endregion

                #region Flickr
                case CloudType.Flickr:
                    break;
                #endregion

                #region Facebook
                case CloudType.Facebook:
                    break;
                #endregion

                #region Picasa
                case CloudType.Picasa:
                    //raw formats (.cr2, .nef, .orf, etc.) - "image/x-image-raw"
                    Dictionary<string, string> Picasa = new Dictionary<string, string>
                    { 
                    { ".jpg", "image/jpeg"}, { ".gif", "image/gif" }, { ".bmp", "image/bmp" }, { ".mov", "video/quicktime" }, { ".psd", "application/photoshop" },
                     { ".avi", "video/x-msvideo"}, { ".mpg", "video/mpeg"}, { ".wmv", "video/x-ms-wmv" },{".asf","video/x-ms-asf"},
                     {".tif","video/x-ms-asf"},{".png","image/png"},{".cr2","image/x-image-raw"},{".nef","image/x-image-raw"},{".orf","image/x-image-raw"}
                    };
                    foreach (var file in files)
                    {
                        if (Aborted) { Prompt = "Operation Cancelled!"; Aborted = false; return; }
                        string filename = Path.GetFileName(file), ext = Path.GetExtension(file).ToLower();
                        if (!Picasa.ContainsKey(ext)) continue;
                        Prompt = "Adding \"" + filename + "\"";
                        using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            try
                            {
                                var ae = PR.Service.Insert(new Uri(PicasaQuery.CreatePicasaUri(LoginName, AlbumID)), fs, Picasa[ext], filename);
                                lvCloud.cbAdd(ae, 1);
                                if (cldCache != null) cldCache.Add(new ListViewItem(filename, 1) { Tag = ae, ToolTipText = ae.AlternateUri.Content });
                                fs.Close();
                            }
                            catch (Exception exp)
                            {
                                fs.Close();
                                MessageBox.Show(exp.Message, "Upload \"" + filename + "\" ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                            finally
                            {
                                fs.Dispose();
                            }
                        }
                    }
                    Prompt = lvCloud.Items.Count + " Photos(s)";
                    break;
                #endregion
            }
            System.Media.SystemSounds.Exclamation.Play();
            btnUp.cbEnable(true);
            btnSign.cbEnable(true);
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == ofd.ShowDialog(this))
            {
                Aborted = false;
                btnUp.Enabled = false;
                btnSign.Enabled = false;
                new Thread(AddCloudFile).Start(ofd.FileNames);
            }
        }

        private void lvCloud_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ListViewItem lvi = lvCloud.Items[e.Item];
            if (string.IsNullOrEmpty(e.Label) || e.Label == lvi.Text) { e.CancelEdit = true; return; }
            CloudStatus.Text = "Syncing...";
            switch (Service)
            {
                #region Flickr
                case CloudType.Flickr:
                    break;
                #endregion

                #region Facebook
                case CloudType.Facebook:
                    break;
                #endregion

                #region Picasa & GDrive
                case CloudType.GDrive:
                case CloudType.Picasa:
                    var ae = (lvi.Tag as AtomEntry);
                    ae.Title.Text = e.Label;
                    new Thread(new ThreadStart(() =>
                    {
                        var @new = ae.Update();
                        if (cldCache != null)
                        {
                            ListViewItem item = cldCache.Single(i => i.ToolTipText == lvi.ToolTipText);
                            item.Text = e.Label; item.ToolTipText = @new.AlternateUri.Content; item.Tag = @new;
                        }
                        lvi.ToolTipText = @new.AlternateUri.Content;
                        lvi.Tag = @new;
                        Prompt = "Done";
                    })).Start();
                    break;
                #endregion
            }
        }

        private void lvCloud_KeyDown(object sender, KeyEventArgs e)
        {
            if (lvCloud.Focused)
            {
            ListViewItem lvi = lvCloud.FocusedItem;
            if (lvi != null)
                if (e.KeyCode == Keys.F2)
                    lvi.BeginEdit();
                else if (e.KeyCode == Keys.C && e.Control)
                    Clipboard.SetText(lvi.ToolTipText);
                    //Press (Shift+)F4 to set Album public or private.
                else if (Service == CloudType.Picasa && AlbumID == null && e.KeyCode == Keys.F4)
                {
                    var a = new Album() { AtomEntry = lvi.Tag as AtomEntry };
                    if (e.Shift)
                    {
                        if (a.Access == "private") return;
                        if (DialogResult.Yes == MessageBox.Show(string.Format("Are you sure to set \"{0}\" Album private?", lvi.Text), "Protect Album in Picasa", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        {
                            a.Access = "private"; CloudStatus.Text = "Setting \"" + lvi.Text + "\" Album private";
                        }
                        else return;
                    }
                    else
                    {
                        if (a.Access == "public") return;
                        if (DialogResult.Yes == MessageBox.Show(string.Format("Are you sure to set \"{0}\" Album public?", lvi.Text), "Share Album in Picasa", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        {
                            a.Access = "public"; CloudStatus.Text = "Setting \"" + lvi.Text + "\" Album public";
                        }
                        else return;
                    }
                    new Thread(new ThreadStart(() =>
                    {
                        var @new = a.PicasaEntry.Update(); lvi.Tag = @new; lvCloud.Invoke(new Action(() => lvi.ToolTipText = @new.AlternateUri.Content));
                        if (cldCache != null)
                        {
                            var cache = cldCache.Single(_ => _.ToolTipText == a.AtomEntry.AlternateUri.Content);
                            cache.Tag = @new; cache.ToolTipText = @new.AlternateUri.Content;
                        }
                        Prompt = "Done";
                    })).Start();
                }
            }
            if (e.KeyCode == Keys.Escape)
            {
                Aborted = true;
                CloudStatus.Text = "Pending Cancelled...";
            }
        }

        private void lvCloud_DragDrop(object sender, DragEventArgs e)
        {
            ListView lv = sender as ListView;
            string[] files;
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    if (DR == null) return;
                    break;
                #endregion

                #region Flickr
                case CloudType.Flickr:
                    break;
                #endregion

                #region Facebook
                case CloudType.Facebook:
                    break;
                #endregion

                #region Picasa
                case CloudType.Picasa:
                    if (AlbumID == null) return;
                    break;
                #endregion
            }
            files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files.Length == 0) return;
            Aborted = false;
            btnUp.Enabled = false;
            btnSign.Enabled = false;
            new Thread(AddCloudFile).Start(files);
        }

        private void lvCloud_DragEnter(object sender, DragEventArgs e)
        {
            if (!btnAdd.Enabled)
            { e.Effect = DragDropEffects.None; return; }
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void WebCloud_Load(object sender, EventArgs e)
        {
            switch (Service)
            {
                case CloudType.GDrive:
                    gpLogin.Text = "GDrive";
                    cbPublic.Visible = false;
                    txtFolderName.Location = new System.Drawing.Point(txtFolderName.Location.X - 60, txtFolderName.Location.Y);
                    txtFolderName.Width = 230;
                    break;
                case CloudType.Flickr:
                    gpLogin.Text = "Flickr";
                    break;
                case CloudType.Facebook:
                    gpLogin.Text = "Facebook";
                    break;
                case CloudType.Picasa:
                    gpLogin.Text = "Picasa";
                    break;
            }
        }

        private void lvCloud_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (lvCloud.SelectedItems.Count > 0)
            {
                btnDelete.Enabled = true;
                ListViewItem lvi = lvCloud.SelectedItems[0];
                switch (Service)
                {
                    #region GDrive
                    case CloudType.GDrive:
                        var doc = (lvi.Tag as AtomEntry);
                        Prompt = "Updated: " + doc.Updated.ToShortDateString();
                        break;
                    #endregion

                    #region Flickr
                    case CloudType.Flickr:
                        break;
                    #endregion

                    #region Facebook
                    case CloudType.Facebook:
                        break;
                    #endregion

                    #region Picasa
                    case CloudType.Picasa:
                        if (AlbumID == null)
                        {
                            cbPublic.Enabled = true;
                            cbPublic.AutoCheck = false;
                            uint total = 0, check = 0;
                            foreach (ListViewItem item in lvCloud.SelectedItems)
                            {
                                var a = new Album() { AtomEntry = item.Tag as AtomEntry };
                                total += a.NumPhotos;
                                if (a.Access == "public") check++;
                            }
                            cbPublic.CheckState = check == 0 ? CheckState.Unchecked : check == lvCloud.SelectedItems.Count ? CheckState.Checked : CheckState.Indeterminate;
                            Prompt = string.Format("Total: {0} Photos", total);
                        }
                        else
                        {
                            long total = 0;
                            foreach (ListViewItem item in lvCloud.SelectedItems)
                                total += new Photo() { AtomEntry = item.Tag as AtomEntry }.Size;
                            Prompt = string.Format("Total: {0} KB", total/1024);
                        }
                        break;
                    #endregion
                }
            }
            else
            {
                btnDelete.Enabled = false;
                switch (Service)
                {
                    #region GDrive
                    case CloudType.GDrive:
                        Prompt = lvCloud.Items.Count + " item(s)";
                        break;
                    #endregion

                    #region Flickr
                    case CloudType.Flickr:
                        break;
                    #endregion

                    #region Facebook
                    case CloudType.Facebook:
                        break;
                    #endregion

                    #region Picasa
                    case CloudType.Picasa:
                        cbPublic.CheckState = CheckState.Unchecked;
                        Prompt = lvCloud.Items.Count + (AlbumID == null ? " Albums(s)" : " Photos(s)");
                        break;
                    #endregion
                }
            }
        }

        private void lvCloud_MouseClick(object sender, MouseEventArgs e)
        {
            if (lvCloud.SelectedItems.Count > 0)
                btnDelete.Enabled = true;
            else
                btnDelete.Enabled = false;
        }

        private void txtFolderName_TextChanged(object sender, EventArgs e)
        {
            if (cldCache == null) return;
            string text = txtFolderName.Text.Trim().ToLower();
            lvCloud.Items.Clear();
            if (string.IsNullOrEmpty(text))
                lvCloud.Items.AddRange(cldCache.ToArray());
            else
                lvCloud.Items.AddRange(cldCache.Where(_ => _.Text.ToLower().Contains(text)).ToArray());
            CloudStatus.Text = lvCloud.Items.Count + " items";
        }

        private void txtFolderName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cldCache == null)
            {
                cldCache = new Collection<ListViewItem>();
                foreach (ListViewItem item in lvCloud.Items)
                    cldCache.Add(item);
            }
        }

        private void WebCloud_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (cldCache != null) { cldCache.Clear(); cldCache = null; }
        }

        private void txtFolderName_Enter(object sender, EventArgs e)
        {
            cbPublic.Enabled = AlbumID == null;
            cbPublic.ThreeState = false;
            cbPublic.AutoCheck = true;
            cbPublic.Checked = false;
        }

        private void lvCloud_Enter(object sender, EventArgs e)
        {
            if (lvCloud.Items.Count > 0 && AlbumID == null)
            {
                cbPublic.AutoCheck = false;
                cbPublic.ThreeState = true;
            }
        }
    }
}
