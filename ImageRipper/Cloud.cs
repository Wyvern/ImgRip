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

        string LoginName { get { return loginName.Text.Trim(); } }
        bool Aborted = false;
        string Prompt { set { statusStrip1.Invoke(new Action(() => CloudStatus.Text = value)); } }
        
        //Local ListViewItems memory cache
        Collection<ListViewItem> cldItems;
        
        //Google Docs client objects
        DocumentsRequest DR;
        List<Document> Docs;
        Stack<Document> Folder;
        //Picasa Web Albums client objects
        PicasaRequest PR;
        List<Photo> Photos;
        List<Album> Albums;
        public string AlbumID { get; set; }

        internal CloudType Service { get; set; }

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
                    if (!Regex.IsMatch(LoginName, @"^[\w\.-]+@(google|gmail|googlemail).com$", RegexOptions.IgnoreCase))
                    {
                        MessageBox.Show("Please check your Google account", "Invalidate login");
                        return;
                    }
                    
                    DR = new DocumentsRequest(new RequestSettings("Ripper", LoginName, loginPass.Text));
                    CloudStatus.Text = "Waiting..."; lvCloud.Items.Clear();
                    Func<Feed<Document>> GE = () => DR.GetFolders();
                    try
                    {
                        GE.BeginInvoke(ar =>
                                                  {
                                                      Docs = GE.EndInvoke(ar).Entries.ToList();
                                                      foreach (var item in Docs)
                                                          lvCloud.cbAdd(item.Title, item.Type == Document.DocumentType.Folder ? 0 : 2, item.AtomEntry.AlternateUri.Content);
                                                      if (cldItems != null) { cldItems.Clear(); cldItems = null; }
                                                      btnSign.cbEnable(true);
                                                      txtFolderName.cbEnable(true);
                                                      btnCreateFolder.cbEnable(true);
                                                      btnDelete.cbEnable(false);
                                                      btnUp.cbEnable(false);
                                                      btnAddFiles.cbEnable(false);
                                                      Prompt = "GDrive: " + Docs.Count + " item(s)";
                                                  }, null);
                    }
                    catch (Exception exp)
                    {
                        Prompt = exp.InnerException.Message;
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
                    if (!Regex.IsMatch(LoginName, @"^[\w\.-]+@(google|gmail|googlemail).com$", RegexOptions.IgnoreCase))
                    {
                        MessageBox.Show("Please check your Google account", "Invalidate login");
                        return;
                    }

                    CloudStatus.Text = "Waiting..."; lvCloud.Items.Clear();
                    PR = new PicasaRequest(new RequestSettings("Ripper", LoginName, loginPass.Text));
                    Func<Feed<Album>> GA = () => PR.GetAlbums();
                    GA.BeginInvoke(_ =>
                    {
                        Albums = GA.EndInvoke(_).Entries.ToList();
                        foreach (var item in Albums)
                            lvCloud.cbAdd(item.Title, 0, item.AtomEntry.AlternateUri.Content);
                        if (cldItems != null) { cldItems.Clear(); cldItems = null; }
                        txtFolderName.cbEnable(true);
                        btnCreateFolder.cbEnable(true);
                        cbPublic.cbEnable(true);
                        btnUp.cbEnable(false);
                        btnAddFiles.cbEnable(false);
                        btnDelete.cbEnable(false);
                        btnSign.cbEnable(true);
                        Prompt = "Picasa: " + Albums.Count + " item(s)";
                    }, null);
                    break;
                #endregion
            }
            btnSign.Enabled = false;
            Properties.Settings.Default.CloudID = LoginName;
            Properties.Settings.Default.Password = loginPass.Text;
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
                        Document doc = Docs.Where(_ => _.Type == Document.DocumentType.Folder).FirstOrDefault(_ => _.Title == lvi.Text);
                        if (doc != null)
                        {
                            Folder = Folder ?? new Stack<Document>();
                            CloudStatus.Text = "Listing \"" + doc.Title + "\"";
                            Folder.Push(doc);
                            btnDelete.Enabled = btnAddFiles.Enabled = btnCreateFolder.Enabled = false;
                            lvCloud.Items.Clear();
                            Func<Feed<Document>> DFC = () => DR.GetFolderContent(doc);
                            DFC.BeginInvoke(ar =>
                                {
                                    Docs = DFC.EndInvoke(ar).Entries.ToList();
                                    foreach (var item in Docs)
                                        lvCloud.cbAdd(item.Title, item.Type == Document.DocumentType.Folder ? 0 : 2, item.AtomEntry.AlternateUri.Content);
                                    if (cldItems != null) { cldItems.Clear(); cldItems = null; }
                                    btnUp.cbEnable(true);
                                    btnCreateFolder.cbEnable(true);
                                    btnAddFiles.cbEnable(true);
                                    txtFolderName.cbEnable(true);
                                    Prompt = doc.Title + ": " + Docs.Count + " item(s)";
                                }, null);
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
                        Album a = Albums.FirstOrDefault(_ => _.Title == lvi.Text);
                        if (a != null)
                        {
                            CloudStatus.Text = "Listing \"" + a.Title + "\"";
                            btnDelete.Enabled = cbPublic.Enabled = txtFolderName.Enabled = false;
                            lvCloud.Items.Clear();
                            AlbumID = a.Id;
                            Func<Feed<Photo>> GPA = () => PR.GetPhotosInAlbum(AlbumID);
                            GPA.BeginInvoke(_ =>
                                {
                                    Photos = GPA.EndInvoke(_).Entries.ToList();
                                    foreach (var item in Photos)
                                        lvCloud.cbAdd(item.Title, 1, item.AtomEntry.AlternateUri.Content);
                                    if (cldItems != null) { cldItems.Clear(); cldItems = null; }
                                    btnUp.cbEnable(true);
                                    btnCreateFolder.cbEnable(false);
                                    txtFolderName.cbEnable(true);
                                    btnAddFiles.cbEnable(true);
                                    Prompt = a.Title + " " + Photos.Count + " item(s)";
                                }, null);
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
                    btnUp.Enabled = btnDelete.Enabled = btnCreateFolder.Enabled = txtFolderName.Enabled = btnAddFiles.Enabled = false;
                    if (Folder.Count == 0)
                    {
                        CloudStatus.Text = "Waiting...";
                        Func<Feed<Document>> GE = () => DR.GetFolders();
                        GE.BeginInvoke(ar =>
                        {
                            Docs = GE.EndInvoke(ar).Entries.ToList();
                            foreach (var item in Docs)
                            {
                                lvCloud.cbAdd(item.Title, item.Type == Document.DocumentType.Folder ? 0 : 2, item.AtomEntry.AlternateUri.Content);
                            }
                            if (cldItems != null) { cldItems.Clear(); cldItems = null; }
                            txtFolderName.cbEnable(true);
                            //btnAddFiles.cbEnable(true);
                            btnCreateFolder.cbEnable(true);
                            btnUp.cbEnable(false);
                            Prompt = "GDrive: " + Docs.Count + " item(s)";
                        }, null);
                    }
                    else
                    {
                        doc = Folder.Peek();
                        CloudStatus.Text = "Listing \"" + doc.Title + "\"";
                        Func<Feed<Document>> DFC = () => DR.GetFolderContent(doc);
                        DFC.BeginInvoke(ar =>
                            {
                                Docs = DFC.EndInvoke(ar).Entries.ToList();
                                foreach (var item in Docs)
                                    lvCloud.cbAdd(item.Title, item.Type == Document.DocumentType.Folder ? 0 : 2, item.AtomEntry.AlternateUri.Content);
                                if (cldItems != null) { cldItems.Clear(); cldItems = null; }
                                btnUp.cbEnable(true);
                                btnAddFiles.cbEnable(true);
                                btnCreateFolder.cbEnable(true);
                                txtFolderName.cbEnable(true);
                                Prompt = doc.Title + ": " + Docs.Count + " item(s)";
                            }, null);
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
                    btnUp.Enabled = btnDelete.Enabled = btnCreateFolder.Enabled = txtFolderName.Enabled = btnAddFiles.Enabled = false;
                    Func<Feed<Album>> GA = () => PR.GetAlbums();
                    GA.BeginInvoke(_ =>
                    {
                        Albums = GA.EndInvoke(_).Entries.ToList();
                        foreach (var item in Albums)
                            lvCloud.cbAdd(item.Title, 0, item.AtomEntry.AlternateUri.Content);
                        if (cldItems != null) { cldItems.Clear(); cldItems = null; }
                        Photos.Clear(); AlbumID = null;
                        cbPublic.cbEnable(true);
                        txtFolderName.cbEnable(true);
                        btnCreateFolder.cbEnable(true);
                        Prompt = "Picasa: " + Albums.Count + " item(s)";
                    }, null);
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
            btnCreateFolder.Enabled = false;
            btnUp.Enabled = false;
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    Func<Document> CD = () => DR.CreateDocument(new Document() { Title = foldername, Type = Document.DocumentType.Folder });
                    Document @base = null;
                    string title, ttt;
                    if (Folder != null && Folder.Count > 0) @base = Folder.Peek();
                    CD.BeginInvoke(_ =>
                        {
                            var @old = CD.EndInvoke(_);
                            title = @old.Title; ttt = @old.AtomEntry.AlternateUri.Content;
                            var @new = @old;
                            if (@base != null)
                            {
                                Func<Document> MD = () => DR.MoveDocumentTo(@base, @old);
                                MD.BeginInvoke(__ =>
                                    {
                                        @new = MD.EndInvoke(__);
                                        title = @new.Title; ttt = @new.AtomEntry.AlternateUri.Content;
                                    }, null);
                            }
                            lvCloud.cbAdd(title, 0, ttt);
                            if (cldItems != null) cldItems.Add(new ListViewItem(title, 0) { ToolTipText = ttt });
                            Docs.Add(@new);
                            txtFolderName.cbEnable(true);
                            btnCreateFolder.cbEnable(true);
                            btnUp.cbEnable(@base != null);
                            Prompt = (@base == null ? "GDrive" : @base.Title) + ": " + Docs.Count + " item(s)";
                        }, null);
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
                    a.Access = cbPublic.Checked ? "public" : "private";
                    btnCreateFolder.Enabled = false;
                    Func<Album> CA = () => PR.Insert<Album>(new Uri(PicasaQuery.CreatePicasaUri(LoginName)), a);
                    CA.BeginInvoke(_ =>
                        {
                            var @new = CA.EndInvoke(_);
                            lvCloud.cbAdd(foldername, 0, @new.AtomEntry.AlternateUri.Content);
                            if (cldItems != null) cldItems.Add(new ListViewItem(foldername, 0) { ToolTipText = @new.AtomEntry.AlternateUri.Content });
                            Albums.Add(@new);
                            btnCreateFolder.cbEnable(true);
                            txtFolderName.cbEnable(true);
                            Prompt = "Picasa " + Albums.Count + " item(s)";
                        }, null);
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
                    btnDelete.Enabled =btnUp.Enabled = false;
                    List<ListViewItem> items = new List<ListViewItem>();
                    foreach (ListViewItem item in lvCloud.SelectedItems)
                        items.Add(item);
                    ThreadPool.QueueUserWorkItem(DeleteCloudItem, items);
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
            IEnumerable<ListViewItem> items = arg as IEnumerable<ListViewItem>;
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
                            var doc = Docs.First(_ => _.Title == lvi.Text);
                            DR.Service.Delete(doc.AtomEntry);
                            Docs.Remove(doc);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message, "Delete " + lvi.Text + " ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                        lvCloud.Invoke(RemoveItem, lvi);
                        if (cldItems != null) cldItems.Remove(lvi);
                    }
                    btnUp.cbEnable((Folder != null && Folder.Count > 0));
                    Prompt = "Done";
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
                            if (AlbumID != null)
                            {
                                var p = Photos.First(_ => _.Title == lvi.Text);
                                PR.Delete<Photo>(p); Photos.Remove(p);
                            }
                            else
                            {
                                var a = Albums.First(_ => _.Title == lvi.Text);
                                PR.Delete<Album>(a); ; Albums.Remove(a);
                            }
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.InnerException.Message, "Delete " + lvi.Text + " ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                        lvCloud.Invoke(RemoveItem, lvi);
                        if (cldItems != null) cldItems.Remove(lvi);
                    }
                    btnUp.cbEnable(AlbumID != null);
                    Prompt = "Done";
                    break;
                #endregion
            }
            System.Media.SystemSounds.Exclamation.Play();
        }

        void AddCloudFile(object arg)
        {
            string[] files = arg as string[];
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    Document @base = null, @new;
                    if (Folder != null && Folder.Count > 0)
                        @base = Folder.Peek();
                    Dictionary<string, string> MIME = new Dictionary<string, string>
                    { 
                    { ".txt", "text/plain" }, { ".htm", "text/html" }, { ".html", "text/html" }, 
                    { ".jpg", "image/jpeg" }, { ".jpeg", "image/jpeg" }, { ".png", "image/png" }, 
                    { ".gif", "image/gif" }, { ".tif", "image/tiff" },{"","text/plain" }
                    };
                    foreach (var file in files)
                    {
                        string filename = Path.GetFileName(file), ext = Path.GetExtension(file);
                        if (Aborted) { Prompt = "Operation Cancelled!"; Aborted = false; return; }
                        if (!MIME.ContainsKey(ext)) continue;
                        Prompt = "Adding \"" + filename + "\"";
                        try
                        {
                            var de = DR.Service.UploadFile(file, filename, MIME[ext], true);
                            @new = new Document() { AtomEntry = de };
                            if (@base != null)
                            {
                                Prompt = "Moving \"" + @new.Title + "\" to " + @base.Title;
                                @new = DR.MoveDocumentTo(@base, @new);
                            }
                            string ttt = @new.AtomEntry.AlternateUri.Content;
                            lvCloud.cbAdd(de.Title.Text, 2, ttt);
                            if (cldItems != null) cldItems.Add(new ListViewItem(de.Title.Text, 2) { ToolTipText = ttt });
                            Docs.Add(@new);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message, "Upload " + filename + " ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }
                    Prompt = "Done";
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
                    foreach (var file in files)
                    {  
                        string filename = Path.GetFileName(file);
                        if (Aborted) { Prompt = "Operation Cancelled!"; Aborted = false; return; }
                        Prompt = "Adding \"" + filename + "\"";
                        try
                        {
                            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                            {
                                var ae = PR.Service.Insert(new Uri(PicasaQuery.CreatePicasaUri(LoginName, AlbumID)), fs, "image/jpeg", filename);
                                Photos.Add(new Photo() { AtomEntry = ae });
                                fs.Close();
                                lvCloud.cbAdd(filename, 1, ae.AlternateUri.Content);
                                if (cldItems != null) cldItems.Add(new ListViewItem(filename, 1) { ToolTipText=ae.AlternateUri.Content});
                            }
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message, "Upload " + filename + " ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }
                    Prompt = "Done";
                    break;
                #endregion
            }
            System.Media.SystemSounds.Exclamation.Play();
            btnUp.cbEnable(true);
            btnSign.cbEnable(true);
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            if (Docs != null ||AlbumID!=null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                ofd.Filter = "JPG files (*.jpg)|*.jpg|PNG files (*.png)|*.png|All files (*.*)|*.*";
                ofd.InitialDirectory = ((Ripper)this.Owner).Dir ?? System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                if (DialogResult.OK == ofd.ShowDialog(this))
                {
                    foreach (string file in ofd.SafeFileNames)
                    {
                        if (lvCloud.FindItemWithText(file) != null)
                        {
                            CloudStatus.Text = "\"" + file + "\" already existed in this folder.";
                            return;
                        }
                    }
                    Aborted = false;
                    btnUp.Enabled = false;
                    btnSign.Enabled = false;
                    ThreadPool.QueueUserWorkItem(AddCloudFile, ofd.FileNames);
                }
            }
        }

        private void lvCloud_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ListViewItem lvi = lvCloud.FocusedItem;
            if (string.IsNullOrEmpty(e.Label) || e.Label == lvi.Text) { e.CancelEdit = true; return; }
            CloudStatus.Text = "Syncing...";
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    Document doc = Docs.First(_ => _.Title == lvi.Text);
                    doc.Title = e.Label;
                    Func<Document> DU = () => DR.Update<Document>(doc);
                    DU.BeginInvoke(_ =>
                    {
                        Document @new = DU.EndInvoke(_); Docs.Remove(doc); Docs.Add(@new); lvi.ToolTipText = @new.AtomEntry.AlternateUri.Content; 
                        if (cldItems != null)
                        {
                            ListViewItem item = cldItems.First(__=> __.Text == lvi.Text);
                            item.Text = e.Label; item.ToolTipText = lvi.ToolTipText;
                        }
                        Prompt = "Done";
                    }, null);
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
                        Album a = Albums.First(_ => _.Title == lvi.Text);
                        a.Title = e.Label;
                        Func<Album> PU = () => PR.Update<Album>(a);
                        PU.BeginInvoke(_ =>
                        {
                            Album @new = PU.EndInvoke(_); Albums.Remove(a); Albums.Add(@new); lvi.ToolTipText = @new.AtomEntry.AlternateUri.Content;
                            if (cldItems != null)
                            {
                                ListViewItem item = cldItems.First(__ => __.Text == lvi.Text);
                                item.Text = e.Label; item.ToolTipText = lvi.ToolTipText;
                            }
                            Prompt = "Done";
                        }, null);
                    }
                    else
                    {
                        Photo p = Photos.First(_ => _.Title == lvi.Text);
                        p.Title = e.Label;
                        Func<Photo> PU = () => PR.Update<Photo>(p);
                        PU.BeginInvoke(_ =>
                        {
                            Photo @new = PU.EndInvoke(_); Photos.Remove(p); Photos.Add(@new); lvi.ToolTipText = @new.AtomEntry.AlternateUri.Content;
                            if (cldItems != null)
                            {
                                ListViewItem item = cldItems.First(__ => __.Text == lvi.Text);
                                item.Text = e.Label; item.ToolTipText = lvi.ToolTipText;
                            }
                            Prompt = "Done";
                        }, null);
                    }
                    break;
                #endregion
            }
            
        }

        private void lvCloud_KeyDown(object sender, KeyEventArgs e)
        {
            if (lvCloud.Focused)
            {
            ListViewItem lvi = lvCloud.FocusedItem;
            if (lvi != null && e.KeyCode == Keys.F2)
                lvi.BeginEdit();
            if (lvi != null && e.KeyCode == Keys.C && e.Control)
                Clipboard.SetText(lvi.ToolTipText);
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
            foreach (string file in files)
            {
                string filename = Path.GetFileName(file);
                if (lv.FindItemWithText(filename) != null)
                {
                    CloudStatus.Text = "\"" + filename + "\" already existed in this folder.";
                    return;
                }
            }
            Aborted = false;
            btnUp.Enabled = false;
            btnSign.Enabled = false;
            ThreadPool.QueueUserWorkItem(AddCloudFile, files);
        }

        private void lvCloud_DragEnter(object sender, DragEventArgs e)
        {
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    if (DR == null||!btnAddFiles.Enabled)
                    { e.Effect = DragDropEffects.None; return; }
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
                    { e.Effect = DragDropEffects.None; return; }
                    break;
                #endregion
            }

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
                    lblCloudID.Text = Text = "GDrive";
                    break;
                case CloudType.Flickr:
                    lblCloudID.Text = Text = "Flickr";
                    break;
                case CloudType.Facebook:
                    lblCloudID.Text = Text = "Facebook";
                    break;
                case CloudType.Picasa:
                    lblCloudID.Text = Text = "Picasa";
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
                        Document doc= Docs.FirstOrDefault(_ => _.Title == lvi.Text);
                        Prompt = "Type: " + Enum.GetName(typeof(Document.DocumentType), doc.Type);
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
                            uint total = 0;
                            foreach (ListViewItem item in lvCloud.SelectedItems)
                            {
                                total+=Albums.FirstOrDefault(_ => _.Title == item.Text).NumPhotos;
                            }
                            Prompt = "Total: " + total + " Photos";
                        }
                        else
                            Prompt = lvi.Text;
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
                        Prompt = Docs.Count + " item(s)";
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
                        Prompt = (AlbumID == null ? Albums.Count : Photos.Count) + " item(s)";
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
            string text = txtFolderName.Text.Trim().ToLower();
            lvCloud.Items.Clear();
            if (string.IsNullOrEmpty(text))
            {
                lvCloud.Items.AddRange(cldItems.ToArray());
                return;
            }
            lvCloud.Items.AddRange(cldItems.Where(_ => _.Text.ToLower().Contains(text)).ToArray());
        }

        private void txtFolderName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cldItems == null)
            {
                cldItems = new Collection<ListViewItem>();
                foreach (ListViewItem item in lvCloud.Items)
                    cldItems.Add(item);
            }
        }

        private void WebCloud_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (cldItems != null) { cldItems.Clear(); cldItems = null; }
        }
    }
}
