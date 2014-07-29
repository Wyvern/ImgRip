<<<<<<< HEAD
﻿namespace ImgRip
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using Google.Documents;
    using Google.GData.Client;
    using Google.GData.Photos;
    using Google.Picasa;

    partial class WebCloud : Form
    {
        public WebCloud()
        {
            InitializeComponent();
        }

        bool working = false;
        string LoginName
        {
            get
            {
                var name = tbName.Text.Trim();
                if (name.LastIndexOf('@') < 0) name += "@gmail.com";
                return name;
            }
        }
        bool Aborted = false;
        string Prompt { set { if (!IsHandleCreated) return;  Invoke(new Action(() => CloudStatus.Text = value)); } }
        string Title { set { if (!IsHandleCreated)return; Invoke(new Action(() => this.Text = value)); } }
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
            CloudStatus.Text = "Waiting..."; lvCloud.Items.Clear();
            tbName.ReadOnly = tbPass.ReadOnly = true; btnCreate.Enabled = false;
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    DR = new DocumentsRequest(new RequestSettings("Ripper", LoginName, tbPass.Text) { AutoPaging = true, UseSSL = true });
                    new Thread(new ThreadStart(() =>
                    {
                        try
                        {
                            DR.Service.QueryClientLoginToken();
                            var items = DR.GetFolders().Entries;
                            Invoke(new Action(() => lvCloud.BeginUpdate()));
                            foreach (var item in items)
                                lvCloud.cbAdd(item.AtomEntry, 0);
                            Invoke(new Action(() => lvCloud.EndUpdate()));
                        }
                        catch (Exception li)
                        {
                            if (this.IsHandleCreated)
                            {
                                Prompt = li.Message;
                                Invoke(new Action(() => tbName.ReadOnly = false));
                                Invoke(new Action(() => tbPass.ReadOnly = false));
                                btnSign.cbEnable(true); return;
                            }
                        }
                        if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                        Prompt = lvCloud.Items.Count + " item(s)";
                        UICallBack.EnableControls(true, btnSign, txtFolderName, btnCreate);
                        UICallBack.EnableControls(false, btnDelete, btnUp, btnAdd);
                        if (this.IsHandleCreated)
                        {
                            Invoke(new Action(() => tbName.ReadOnly = false));
                            Invoke(new Action(() => tbPass.ReadOnly = false));
                        }
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
                    PR = new PicasaRequest(new RequestSettings("Ripper", LoginName, tbPass.Text) { AutoPaging = true, UseSSL = true });
                    new Thread(new ThreadStart(() =>
                    {
                        try
                        {
                            PR.Service.QueryClientLoginToken();
                            var items = PR.GetAlbums().Entries;
                            Invoke(new Action(() => lvCloud.BeginUpdate()));
                            foreach (var item in items) lvCloud.cbAdd(item.AtomEntry, 0);
                            Invoke(new Action(() => lvCloud.EndUpdate()));
                        }
                        catch (Exception li)
                        {
                            if (this.IsHandleCreated)
                            {
                                Prompt = li.Message;
                                Invoke(new Action(() => tbName.ReadOnly = false));
                                Invoke(new Action(() => tbPass.ReadOnly = false));
                                btnSign.cbEnable(true);
                                return;
                            }
                        }
                        if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                        Prompt = lvCloud.Items.Count + " Album(s)";
                        UICallBack.EnableControls(true, btnSign, txtFolderName, btnCreate);
                        UICallBack.EnableControls(false, cbPublic, btnDelete, btnUp, btnAdd);
                        if (this.IsHandleCreated)
                        {
                            Invoke(new Action(() => tbName.ReadOnly = false));
                            Invoke(new Action(() => tbPass.ReadOnly = false));
                        }
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
                        if (doc.Type == Document.DocumentType.Folder)
                        {
                            Folder = Folder ?? new Stack<Document>();
                            CloudStatus.Text = "Listing \"" + doc.Title + "\"";
                            Folder.Push(doc);
                            btnDelete.Enabled = btnAdd.Enabled = btnCreate.Enabled = false;
                            lvCloud.Items.Clear();
                            new Thread(new ThreadStart(() =>
                            {
                                var items = DR.GetFolderContent(doc).Entries;
                                Invoke(new Action(() => lvCloud.BeginUpdate()));
                                foreach (var item in items)
                                    lvCloud.cbAdd(item.AtomEntry, item.Type == Document.DocumentType.Folder ? 0 : 2);
                                Invoke(new Action(() => lvCloud.EndUpdate()));
                                if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                                UICallBack.EnableControls(true, btnUp, btnCreate, btnAdd, txtFolderName);
                                Title = doc.Title;
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
                        if (AlbumID != null) return;
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
                                Invoke(new Action(() => lvCloud.BeginUpdate()));
                                foreach (var item in items)
                                    lvCloud.cbAdd(item.AtomEntry, 1);
                                Invoke(new Action(() => lvCloud.EndUpdate()));
                                if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                                UICallBack.EnableControls(true, btnUp, btnAdd, txtFolderName);
                                btnCreate.cbEnable(false);
                                Title = a.Title;
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
            if (working) return;
            working = true;
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    if (Folder == null || Folder.Count == 0) { working = false; return; }
                    Document doc = Folder.Pop();
                    lvCloud.Items.Clear();
                    btnUp.Enabled = btnDelete.Enabled = btnCreate.Enabled = txtFolderName.Enabled = btnAdd.Enabled = false;
                    if (Folder.Count == 0)
                    {
                        CloudStatus.Text = "Waiting...";
                        new Thread(new ThreadStart(() =>
                        {
                            var items = DR.GetFolders().Entries;
                            Invoke(new Action(() => lvCloud.BeginUpdate()));
                            foreach (var item in items)
                                if (item.ParentFolders.Count == 0)
                                    lvCloud.cbAdd(item.AtomEntry, 0);
                            Invoke(new Action(() => lvCloud.EndUpdate()));
                            if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                            UICallBack.EnableControls(true, btnCreate, txtFolderName);
                            btnUp.cbEnable(false);
                            Prompt = items.Count() + " item(s)";
                            Title = "GDrive";
                            working = false;
                        })).Start();
                    }
                    else
                    {
                        doc = Folder.Peek();
                        CloudStatus.Text = "Listing \"" + doc.Title + "\"";
                        new Thread(new ThreadStart(() =>
                        {
                            var items = DR.GetFolderContent(doc).Entries;
                            Invoke(new Action(() => lvCloud.BeginUpdate()));
                            foreach (var item in items)
                                lvCloud.cbAdd(item.AtomEntry, item.Type == Document.DocumentType.Folder ? 0 : 2);
                            Invoke(new Action(() => lvCloud.EndUpdate()));
                            if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                            UICallBack.EnableControls(true, btnAdd, btnUp, btnCreate, txtFolderName);
                            Title = doc.Title;
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
                    if (AlbumID == null) { working = false; return; }
                    lvCloud.Items.Clear();
                    CloudStatus.Text = "Waiting...";
                    btnUp.Enabled = btnDelete.Enabled = btnCreate.Enabled = txtFolderName.Enabled = btnAdd.Enabled = false;
                    new Thread(new ThreadStart(() =>
                    {
                        var items = PR.GetAlbums().Entries;
                        Invoke(new Action(() => lvCloud.BeginUpdate()));
                        foreach (var item in items)
                            lvCloud.cbAdd(item.AtomEntry, 0);
                        Invoke(new Action(() => lvCloud.EndUpdate()));
                        if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                        AlbumID = null;
                        UICallBack.EnableControls(true, txtFolderName, btnCreate);
                        Prompt = items.Count() + " Albums(s)";
                        Title = "Picasa";
                        working = false;
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
            { CloudStatus.Text = "Invalid album name!"; return; }
            if (lvCloud.FindItemWithText(foldername) != null) { CloudStatus.Text = "\"" + foldername + "\" already existed."; return; }
            CloudStatus.Text = "Creating \"" + foldername + "\"";
            txtFolderName.Enabled = btnCreate.Enabled = btnUp.Enabled = false;
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
                        UICallBack.EnableControls(true, txtFolderName, btnCreate);
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
                        UICallBack.EnableControls(true, txtFolderName, btnCreate);
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
                    var items = lvCloud.SelectedItems.OfType<ListViewItem>().ToArray();
                    if (MessageBox.Show("Are you sure want to delete items below:\n" + string.Join(", ", items.Select(i => "\"" + i.Text + "\"").ToArray()), "Delete Cloud Storage Items", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
            var items = arg as ListViewItem[];
            Action<ListViewItem> RemoveItem = lvi => lvCloud.Items.Remove(lvi);
            switch (Service)
            {
                #region GDrive & Picasa
                case CloudType.GDrive:
                case CloudType.Picasa:
                    foreach (var lvi in items)
                    {
                        if (Aborted)
                        {
                            Prompt = "Operation Cancelled!";
                            btnUp.cbEnable(Service == CloudType.GDrive ? (Folder != null && Folder.Count > 0) : AlbumID != null);
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
                        Invoke(RemoveItem, lvi);
                    }
                    btnUp.cbEnable(Service == CloudType.GDrive ? (Folder != null && Folder.Count > 0) : AlbumID != null);
                    Prompt = lvCloud.Items.Count + (Service == CloudType.GDrive ? " items" : (AlbumID == null ? " Albums(s)" : " Photos(s)"));
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

        void AddCloudFile(object arg)
        {
            var files = arg as string[];
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    Document @base = null;
                    if (Folder != null && Folder.Count > 0)
                        @base = Folder.Peek();
                    var DocType = new Dictionary<string, string>
                    { 
                    { ".jpg", "image/jpeg" }, { ".png", "image/png" }, { ".gif", "image/gif" }, { ".tiff", "image/tiff" },{ ".bmp", "image/bmp" },
                    { ".mov", "video/quicktime" },  { ".psd", "application/photoshop" },{ ".avi", "video/x-msvideo"}, { ".mpg", "video/mpeg"},
                    { ".wmv", "video/x-ms-wmv" }, {".asf","video/x-ms-asf"},{".tif","video/x-ms-asf"},
                    {"Default",".txt;.rtf;.ppt;.pptx;.pps;.htm;.html;.xls;.xlsx;.ods;.csv;.tsb;.doc;.docx;.pages;.ai;.dxf;.eps;.ps;.xps;.ttf"}
                    };
                    files = files.Where(f => (File.GetAttributes(f) & FileAttributes.Directory) == 0).ToArray();
                    foreach (var file in files)
                    {
                        if (Aborted) { Prompt = "Operation Cancelled!"; Aborted = false; break; }
                        if (!File.Exists(file)) continue;
                        string filename = Path.GetFileName(file), ext = Path.GetExtension(file).ToLower();
                        if (DocType.ContainsKey(ext) || DocType["Default"].Split(';').Contains(ext))
                        {
                            Prompt = "Adding \"" + filename + "\"";
                            try
                            {
                                var de = DocType.ContainsKey(ext) ? DR.Service.UploadFile(file, filename, DocType[ext], true) : DR.Service.UploadDocument(file, filename);
                                var @new = new Document { AtomEntry = de };
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
                    }
                    Prompt = lvCloud.Items.Count + " items";
                    btnUp.cbEnable(@base != null);
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
                        var dirs = files.Where(f => (File.GetAttributes(f) & FileAttributes.Directory) != 0).ToArray();
                        foreach (var dir in dirs)
                        {
                            var a = new Album();
                            a.Title = Path.GetFileName(dir);
                            Prompt = "Creating Album: \"" + a.Title + "\"";
                            var @new = PR.Insert<Album>(new Uri(PicasaQuery.CreatePicasaUri(LoginName)), a);
                            lvCloud.cbAdd(@new.AtomEntry, 0);
                            if (cldCache != null) cldCache.Add(new ListViewItem(@new.Title, 0) { Tag = @new.AtomEntry, ToolTipText = @new.AtomEntry.AlternateUri.Content });
                            var aid = @new.Id;
                            var photos = Directory.GetFiles(dir);
                            AddtoAlbum(photos, aid);
                        }
                        Prompt = lvCloud.Items.Count + " Album(s)";
                    }
                    else
                    {
                        AddtoAlbum(files, AlbumID);
                        Prompt = lvCloud.Items.Count + " Photos(s)";
                    }
                    btnUp.cbEnable(AlbumID != null);
                    break;
                #endregion
            }
            System.Media.SystemSounds.Exclamation.Play();
            btnSign.cbEnable(true);
        }

        void AddtoAlbum(string[] files, string aId)
        {
            //raw formats (.cr2, .nef, .orf, etc.) - "image/x-image-raw"
            var PicasaType = new Dictionary<string, string>
                    { 
                    { ".jpg", "image/jpeg"}, { ".gif", "image/gif" }, { ".bmp", "image/bmp" }, { ".mov", "video/quicktime" }, { ".psd", "application/photoshop" },
                     { ".avi", "video/x-msvideo"}, { ".mpg", "video/mpeg"}, { ".wmv", "video/x-ms-wmv" },{".asf","video/x-ms-asf"},
                     {".tif","video/x-ms-asf"},{".png","image/png"},{".cr2","image/x-image-raw"},{".nef","image/x-image-raw"},{".orf","image/x-image-raw"}
                    };
            files = files.Where(f => (File.GetAttributes(f) & FileAttributes.Directory) == 0).ToArray();
            foreach (var file in files)
            {
                if (Aborted) { Prompt = "Operation Cancelled!"; Aborted = false; break; }
                if (!File.Exists(file)) continue;
                string filename = Path.GetFileName(file), ext = Path.GetExtension(file).ToLower();
                if (!PicasaType.ContainsKey(ext) || string.IsNullOrEmpty(ext)) continue;
                Prompt = "Adding \"" + filename + "\"";
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        var ae = PR.Service.Insert(new Uri(PicasaQuery.CreatePicasaUri(LoginName, aId)), fs, PicasaType[ext], filename);
                        if (AlbumID != null)
                        {
                            lvCloud.cbAdd(ae, 1);
                            if (cldCache != null) cldCache.Add(new ListViewItem(filename, 1) { Tag = ae, ToolTipText = ae.AlternateUri.Content });
                        }
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
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == ofd.ShowDialog(this))
            {
                Aborted = btnUp.Enabled = btnSign.Enabled = false;
                new Thread(AddCloudFile).Start(ofd.FileNames);
            }
        }

        private void lvCloud_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ListViewItem lvi = lvCloud.Items[e.Item];
            if (string.IsNullOrEmpty(e.Label) || e.Label == lvi.Text) { e.CancelEdit = true;return; }
            CloudStatus.Text = "Renaming...";
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
                        Invoke(new Action(() => lvi.ToolTipText = @new.AlternateUri.Content));
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
                if (e.KeyCode == Keys.Back)
                    btnUp_Click(sender, e);
                if (e.KeyCode == Keys.Escape)
                {
                    Aborted = true;
                    CloudStatus.Text = "Pending Cancelled...";
                }
                if (e.Control && e.KeyCode == Keys.V)
                {
                    var files = Clipboard.GetFileDropList();
                    if (files.Count > 0)
                    {
                        Aborted = btnUp.Enabled = btnSign.Enabled = false;
                        var sa = new string[files.Count];
                        files.CopyTo(sa, 0);
                        new Thread(AddCloudFile).Start(sa);
                    }
                }
                ListViewItem lvi = lvCloud.FocusedItem;
                if (lvi != null)
                    if (e.KeyCode == Keys.F2)
                        lvi.BeginEdit();
                    else if (e.KeyCode == Keys.C && e.Control)
                        Clipboard.SetText(lvi.ToolTipText);
            }
        }

        private void lvCloud_DragDrop(object sender, DragEventArgs e)
        {
            var lv = sender as ListView;
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null || files.Length == 0) return;
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
                    break;
                #endregion
            }
            Aborted = btnUp.Enabled = btnSign.Enabled = false;
            new Thread(AddCloudFile).Start(files);
        }

        private void lvCloud_DragEnter(object sender, DragEventArgs e)
        {
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
                    txtFolderName.Location = new System.Drawing.Point(CmdPanel.Location.X + btnUp.Location.X, txtFolderName.Location.Y);
                    txtFolderName.Width = CmdPanel.Width - 5;
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
                switch (Service)
                {
                    #region GDrive
                    case CloudType.GDrive:
                        var lvi = lvCloud.SelectedItems[0];
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
                            cbPublic.Enabled = cbPublic.ThreeState = cbPublic.AutoCheck = true;
                            uint total = 0, check = 0;
                            foreach (ListViewItem item in lvCloud.SelectedItems)
                            {
                                var a = new Album() { AtomEntry = item.Tag as AtomEntry };
                                total += a.NumPhotos;
                                if (a.Access == "public") check++;
                            }
                            cbPublic.CheckState = check == 0 ? CheckState.Unchecked : check == lvCloud.SelectedItems.Count ? CheckState.Checked : CheckState.Indeterminate;
                            cbPublic.ThreeState = cbPublic.CheckState == CheckState.Indeterminate;
                            cbPublic.AutoCheck = !cbPublic.ThreeState;
                            Prompt = string.Format("Total: {0} Photos", total);
                        }
                        else
                        {
                            long total = 0;
                            foreach (ListViewItem item in lvCloud.SelectedItems)
                                total += new Photo() { AtomEntry = item.Tag as AtomEntry }.Size;
                            Prompt = string.Format("Total: {0} KB", total / 1024);
                        }
                        break;
                    #endregion
                }
            }
            else
            {
                btnDelete.Enabled = cbPublic.Enabled = false;
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
            string text = txtFolderName.Text.Trim();
            lvCloud.SelectedItems.Clear();
            if (string.IsNullOrEmpty(text))
                if (txtFolderName.Text.Length > 0) return;
                else
                {
                    if (lvCloud.Items.Count != cldCache.Count)
                    {
                        lvCloud.SuspendLayout();
                        lvCloud.Items.Clear();
                        lvCloud.Items.AddRange(cldCache.ToArray());
                    }
                }
            else
            {
                lvCloud.SuspendLayout();
                lvCloud.Items.Clear();
                lvCloud.Items.AddRange(cldCache.Where(_ => _.Text.ContainsEx(text)).ToArray());
            }
            lvCloud.ResumeLayout();
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

        private void txtFolderName_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFolderName.Tag as string)) { txtFolderName.SelectAll(); txtFolderName.Tag = txtFolderName.SelectedText; }

        }

        private void txtFolderName_Leave(object sender, EventArgs e)
        {
            txtFolderName.Tag = null;
        }

        private void lvCloud_Leave(object sender, EventArgs e)
        {
            cbPublic.Enabled = cbPublic.Focused;
        }

        private void cbPublic_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPublic.Focused && cbPublic.ThreeState == false)
            {
                if (working) return;
                var lvis = lvCloud.SelectedItems; if (lvis.Count == 0) return;
                var cb = sender as CheckBox; var state = cb.Checked;
                var text = new string[lvis.Count];
                var albums = new Album[lvis.Count];
                for (int i = 0; i < text.Length; i++)
                {
                    text[i] = lvis[i].Text;
                    albums[i] = new Album() { AtomEntry = lvis[i].Tag as AtomEntry };
                }
                var prompt = string.Format("Are you sure to set \"{0}\" {1}?", string.Join(", ", text), state ? "public" : "private");
                working = true;
                switch (state)
                {
                    case true:
                        if (DialogResult.Yes == MessageBox.Show(prompt, "Share Album(s) in Picasa", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        {
                            foreach (var item in albums)
                                item.Access = "public";
                            CloudStatus.Text = "Setting Album(s) public";
                        }
                        else { cbPublic.Checked = false; working = false; return; }
                        break;
                    case false:
                        if (DialogResult.Yes == MessageBox.Show(prompt, "Protect Album(s) in Picasa", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        {
                            foreach (var item in albums)
                                item.Access = "private";
                            CloudStatus.Text = "Setting Album(s) private";
                        }
                        else { cbPublic.Checked = true; working = false; return; }
                        break;
                }
                cbPublic.Enabled = false;
                new Thread(new ThreadStart(() =>
                {
                    for (var i = 0; i < albums.Length; i++)
                    {
                        var @new = albums[i].PicasaEntry.Update();
                        Invoke(new Action(() => { lvis[i].ToolTipText = @new.AlternateUri.Content; lvis[i].Tag = @new; }));

                        if (cldCache != null)
                        {
                            var cache = cldCache.Single(_ => _.ToolTipText == albums[i].AtomEntry.AlternateUri.Content);
                            cache.Tag = @new; cache.ToolTipText = @new.AlternateUri.Content;
                        }
                    }
                    Prompt = "Done"; cbPublic.cbEnable(true); working = false;
                })).Start();
            }
        }

        private void cbPublic_Leave(object sender, EventArgs e)
        {
            cbPublic.Enabled = false;
        }
    }
}
=======
﻿namespace ImgRip
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using Google.Documents;
    using Google.GData.Client;
    using Google.GData.Photos;
    using Google.Picasa;

    partial class WebCloud : Form
    {
        public WebCloud()
        {
            InitializeComponent();
        }

        bool working = false;
        string LoginName { get { return tbName.Text.Trim(); } }
        bool Aborted = false;
        string Prompt { set { if (!IsHandleCreated)return; Invoke(new Action(() => CloudStatus.Text = value)); } }

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
            CloudStatus.Text = "Waiting..."; lvCloud.Items.Clear();
            tbName.ReadOnly = tbPass.ReadOnly = true; btnCreate.Enabled = false;
            const string LoginError="Log in Failed!";
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    DR = new DocumentsRequest(new RequestSettings("Ripper", LoginName, tbPass.Text) { AutoPaging = true});
                    new Thread(new ThreadStart(() =>
                    {
                        try
                        {
                            DR.Service.QueryClientLoginToken();
                            var items = DR.GetFolders().Entries;
                            foreach (var item in items)
                                lvCloud.cbAdd(item.AtomEntry, 0);
                        }
                        catch (Exception)
                        {
                            if (this.IsHandleCreated)
                            {
                                Prompt = LoginError;
                                Invoke(new Action(() => tbName.ReadOnly = false));
                                Invoke(new Action(() => tbPass.ReadOnly = false));
                                btnSign.cbEnable(true); return;
                            }
                        }
                        if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                        Prompt = lvCloud.Items.Count + " item(s)";
                        UICallBack.EnableControls(true, btnSign, txtFolderName, btnCreate);
                        UICallBack.EnableControls(false, btnDelete, btnUp, btnAdd);
                        if (this.IsHandleCreated)
                        {
                            Invoke(new Action(() => tbName.ReadOnly = false));
                            Invoke(new Action(() => tbPass.ReadOnly = false));
                        }
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
                    PR = new PicasaRequest(new RequestSettings("Ripper", LoginName, tbPass.Text) { AutoPaging = true });
                    new Thread(new ThreadStart(() =>
                    {
                        try
                        {
                            PR.Service.QueryClientLoginToken();
                            var items = PR.GetAlbums().Entries;
                            foreach (var item in items) lvCloud.cbAdd(item.AtomEntry, 0);
                        }
                        catch (Exception)
                        {
                            if (this.IsHandleCreated)
                            {
                                Prompt = LoginError;
                                Invoke(new Action(() => tbName.ReadOnly = false));
                                Invoke(new Action(() => tbPass.ReadOnly = false));
                                btnSign.cbEnable(true); return;
                            }
                        }
                        if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                        Prompt = lvCloud.Items.Count + " Album(s)";
                        UICallBack.EnableControls(true, btnSign, txtFolderName, btnCreate);
                        UICallBack.EnableControls(false, cbPublic, btnDelete, btnUp, btnAdd);
                        if (this.IsHandleCreated)
                        {
                            Invoke(new Action(() => tbName.ReadOnly = false));
                            Invoke(new Action(() => tbPass.ReadOnly = false));
                        }
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
                        if (doc.Type == Document.DocumentType.Folder)
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
                                UICallBack.EnableControls(true, btnUp, btnCreate, btnAdd, txtFolderName);
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
                                UICallBack.EnableControls(true, btnUp, btnAdd, txtFolderName);
                                btnCreate.cbEnable(false);
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
            if (working) return;
            working = true;
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    if (Folder == null || Folder.Count == 0) { working = false; return; }
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
                            UICallBack.EnableControls(true, btnCreate, txtFolderName);
                            btnUp.cbEnable(false);
                            Prompt = items.Count() + " item(s)";
                            working = false;
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
                            UICallBack.EnableControls(true, btnAdd, btnUp, btnCreate, txtFolderName);
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
                    if (AlbumID == null) { working = false; return; }
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
                        UICallBack.EnableControls(true, txtFolderName, btnCreate);
                        Prompt = items.Count() + " Albums(s)";
                        working = false;
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
            txtFolderName.Enabled = btnCreate.Enabled = btnUp.Enabled = false;
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
                        UICallBack.EnableControls(true, txtFolderName, btnCreate);
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
                        UICallBack.EnableControls(true, txtFolderName, btnCreate);
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
                    var items = lvCloud.SelectedItems.OfType<ListViewItem>().ToArray();
                    if (MessageBox.Show("Are you sure want to delete items below:\n" + string.Join(", ", items.Select(i => "\""+i.Text+"\"").ToArray()), "Delete Cloud Storage Items", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
            var items = arg as ListViewItem[];
            Action<ListViewItem> RemoveItem = lvi => lvCloud.Items.Remove(lvi);
            switch (Service)
            {
                #region GDrive & Picasa
                case CloudType.GDrive:
                case CloudType.Picasa:
                    foreach (var lvi in items)
                    {
                        if (Aborted)
                        {
                            Prompt = "Operation Cancelled!";
                            btnUp.cbEnable(Service == CloudType.GDrive ? (Folder != null && Folder.Count > 0) : AlbumID != null);
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
                        Invoke(RemoveItem, lvi);
                    }
                    btnUp.cbEnable(Service == CloudType.GDrive ? (Folder != null && Folder.Count > 0) : AlbumID != null);
                    Prompt = lvCloud.Items.Count + (Service == CloudType.GDrive ? " items" : (AlbumID == null ? " Albums(s)" : " Photos(s)"));
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

        void AddCloudFile(object arg)
        {
            var files = arg as string[];
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    Document @base = null;
                    if (Folder != null && Folder.Count > 0)
                        @base = Folder.Peek();
                    var DocType = new Dictionary<string, string>
                    { 
                    { ".jpg", "image/jpeg" }, { ".png", "image/png" }, { ".gif", "image/gif" }, { ".tiff", "image/tiff" },{ ".bmp", "image/bmp" },
                    { ".mov", "video/quicktime" },  { ".psd", "application/photoshop" },{ ".avi", "video/x-msvideo"}, { ".mpg", "video/mpeg"},
                    { ".wmv", "video/x-ms-wmv" }, {".asf","video/x-ms-asf"},{".tif","video/x-ms-asf"},
                    {"Default",".txt;.rtf;.ppt;.pptx;.pps;.htm;.html;.xls;.xlsx;.ods;.csv;.tsb;.doc;.docx;.pages;.ai;.dxf;.eps;.ps;.xps;.ttf"}
                    };
                    files = files.Where(f => (File.GetAttributes(f) & FileAttributes.Directory) == 0).ToArray();
                    foreach (var file in files)
                    {
                        if (Aborted) { Prompt = "Operation Cancelled!"; Aborted = false; break; }
                        if (!File.Exists(file)) continue;
                        string filename = Path.GetFileName(file), ext = Path.GetExtension(file).ToLower();
                        if (!DocType.ContainsKey(ext) && !DocType["Default"].Split(';').Contains(ext) || string.IsNullOrEmpty(ext)) continue;
                        Prompt = "Adding \"" + filename + "\"";
                        try
                        {
                            var de = DocType.ContainsKey(ext) ? DR.Service.UploadFile(file, filename, DocType[ext], true) : DR.Service.UploadDocument(file, filename);
                            var @new = new Document { AtomEntry = de };
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
                    btnUp.cbEnable(@base != null);
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
                        var dirs = files.Where(f => (File.GetAttributes(f) & FileAttributes.Directory) != 0).ToArray();
                        foreach (var dir in dirs)
                        {
                            var a = new Album();
                            a.Title = Path.GetFileName(dir);
                            Prompt = "Creating Album: \"" + a.Title + "\"";
                            var @new = PR.Insert<Album>(new Uri(PicasaQuery.CreatePicasaUri(LoginName)), a);
                            lvCloud.cbAdd(@new.AtomEntry, 0);
                            if (cldCache != null) cldCache.Add(new ListViewItem(@new.Title, 0) { Tag = @new.AtomEntry, ToolTipText = @new.AtomEntry.AlternateUri.Content });
                            var aid = @new.Id;
                            var photos = Directory.GetFiles(dir);
                            AddtoAlbum(photos, aid);
                        }
                        Prompt = lvCloud.Items.Count + " Album(s)";
                    }
                    else
                    {
                        AddtoAlbum(files, AlbumID);  
                        Prompt = lvCloud.Items.Count + " Photos(s)";
                    }
                    btnUp.cbEnable(AlbumID != null);
                    break;
                #endregion
            }
            System.Media.SystemSounds.Exclamation.Play();
            btnSign.cbEnable(true);
        }

        void AddtoAlbum(string[] files, string aId)
        {
            //raw formats (.cr2, .nef, .orf, etc.) - "image/x-image-raw"
            var PicasaType = new Dictionary<string, string>
                    { 
                    { ".jpg", "image/jpeg"}, { ".gif", "image/gif" }, { ".bmp", "image/bmp" }, { ".mov", "video/quicktime" }, { ".psd", "application/photoshop" },
                     { ".avi", "video/x-msvideo"}, { ".mpg", "video/mpeg"}, { ".wmv", "video/x-ms-wmv" },{".asf","video/x-ms-asf"},
                     {".tif","video/x-ms-asf"},{".png","image/png"},{".cr2","image/x-image-raw"},{".nef","image/x-image-raw"},{".orf","image/x-image-raw"}
                    };
            files = files.Where(f => (File.GetAttributes(f) & FileAttributes.Directory) == 0).ToArray();
            foreach (var file in files)
            {
                if (Aborted) { Prompt = "Operation Cancelled!"; Aborted = false; break; }
                if (!File.Exists(file)) continue;
                string filename = Path.GetFileName(file), ext = Path.GetExtension(file).ToLower();
                if (!PicasaType.ContainsKey(ext) || string.IsNullOrEmpty(ext)) continue;
                Prompt = "Adding \"" + filename + "\"";
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        var ae = PR.Service.Insert(new Uri(PicasaQuery.CreatePicasaUri(LoginName, aId)), fs, PicasaType[ext], filename);
                        if (AlbumID != null)
                        {
                            lvCloud.cbAdd(ae, 1);
                            if (cldCache != null) cldCache.Add(new ListViewItem(filename, 1) { Tag = ae, ToolTipText = ae.AlternateUri.Content });
                        }
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
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == ofd.ShowDialog(this))
            {
                Aborted = btnUp.Enabled = btnSign.Enabled = false;
                new Thread(AddCloudFile).Start(ofd.FileNames);
            }
        }

        private void lvCloud_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (working) return;
            working = true;
            ListViewItem lvi = lvCloud.Items[e.Item];
            if (string.IsNullOrEmpty(e.Label) || e.Label == lvi.Text) { e.CancelEdit = true; working = false; return; }
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
                        Invoke(new Action(() => lvi.ToolTipText = @new.AlternateUri.Content));
                        lvi.Tag = @new;
                        Prompt = "Done";
                        working = false;
                    })).Start();
                    break;
                #endregion
            }
        }

        private void lvCloud_KeyDown(object sender, KeyEventArgs e)
        {
            if (lvCloud.Focused)
            {
                if (e.KeyCode == Keys.Back)
                    btnUp_Click(sender, e);
                if (e.KeyCode == Keys.Escape)
                {
                    Aborted = true;
                    CloudStatus.Text = "Pending Cancelled...";
                }
                ListViewItem lvi = lvCloud.FocusedItem;
                if (lvi != null)
                    if (e.KeyCode == Keys.F2)
                        lvi.BeginEdit();
                    else if (e.KeyCode == Keys.C && e.Control)
                        Clipboard.SetText(lvi.ToolTipText);
                    //Press (Shift+)F4 to set Album public or private.
                    else if (Service == CloudType.Picasa && AlbumID == null && e.KeyCode == Keys.F4)
                        ShareAlbum(lvi, e.Shift);
            }
        }

        void ShareAlbum(ListViewItem lvi, bool shift)
        {
            if (working) return;
            working = true;
            var a = new Album() { AtomEntry = lvi.Tag as AtomEntry };
            if (shift)
            {
                if (a.Access == "private") { working = false; return; }
                if (DialogResult.Yes == MessageBox.Show(string.Format("Are you sure to set \"{0}\" Album private?", lvi.Text), "Protect Album in Picasa", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    a.Access = "private"; CloudStatus.Text = "Setting \"" + lvi.Text + "\" Album private";
                }
                else { working = false; return; }
            }
            else
            {
                if (a.Access == "public") { working = false; return; }
                if (DialogResult.Yes == MessageBox.Show(string.Format("Are you sure to set \"{0}\" Album public?", lvi.Text), "Share Album in Picasa", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    a.Access = "public"; CloudStatus.Text = "Setting \"" + lvi.Text + "\" Album public";
                }
                else { working = false; return; }
            }
            new Thread(new ThreadStart(() =>
            {
                var @new = a.PicasaEntry.Update(); lvi.Tag = @new; Invoke(new Action(() => lvi.ToolTipText = @new.AlternateUri.Content));
                if (cldCache != null)
                {
                    var cache = cldCache.Single(_ => _.ToolTipText == a.AtomEntry.AlternateUri.Content);
                    cache.Tag = @new; cache.ToolTipText = @new.AlternateUri.Content;
                }
                Prompt = "Done"; working = false;
            })).Start();
        }

        private void lvCloud_DragDrop(object sender, DragEventArgs e)
        {
            var lv = sender as ListView;
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files.Length == 0) return;
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
                    break;
                #endregion
            }
            Aborted = btnUp.Enabled = btnSign.Enabled = false;
            new Thread(AddCloudFile).Start(files);
        }

        private void lvCloud_DragEnter(object sender, DragEventArgs e)
        {
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
                    txtFolderName.Location = new System.Drawing.Point(CmdPanel.Location.X + btnUp.Location.X, txtFolderName.Location.Y);
                    txtFolderName.Width = CmdPanel.Width - 5;
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
                switch (Service)
                {
                    #region GDrive
                    case CloudType.GDrive:
                        var lvi = lvCloud.SelectedItems[0];
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
                            Prompt = string.Format("Total: {0} KB", total / 1024);
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
            string text = txtFolderName.Text.Trim();
            if (string.IsNullOrEmpty(text))
                if (txtFolderName.Text.Length > 0) return;
                else
                {
                    if (lvCloud.Items.Count != cldCache.Count)
                    {
                        lvCloud.SuspendLayout();
                        lvCloud.Items.Clear();
                        lvCloud.Items.AddRange(cldCache.ToArray());
                    }
                }
            else
            {
                lvCloud.SuspendLayout();
                lvCloud.Items.Clear();
                lvCloud.Items.AddRange(cldCache.Where(_ => _.Text.ContainsEx(text)).ToArray());
            }
            lvCloud.ResumeLayout();
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

        private void txtFolderName_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFolderName.Tag as string)) { txtFolderName.SelectAll(); txtFolderName.Tag = txtFolderName.SelectedText; }

        }

        private void txtFolderName_Leave(object sender, EventArgs e)
        {
            txtFolderName.Tag = null;
        }
    }
}
>>>>>>> parent of fb9b497... Sync from codeplex
