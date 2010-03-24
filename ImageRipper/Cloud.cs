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
        string Prompt { set { statusStrip1.Invoke(new Action(() => CloudStatus.Text = value)); } }
        
        //Local ListViewItems memory cache
        Collection<ListViewItem> cldCache;
        
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
                        MessageBox.Show("Please check your Google account", "Invalid login"); return;
                    }
                    
                    DR = new DocumentsRequest(new RequestSettings("Ripper", LoginName, tbPass.Text) {AutoPaging=true});
                    CloudStatus.Text = "Waiting..."; lvCloud.Items.Clear();
                    Func<Feed<Document>> GE = () => DR.GetFolders();
                    try
                    {
                        GE.BeginInvoke(ar =>
                                                  {
                                                      Docs = GE.EndInvoke(ar).Entries.ToList();
                                                      foreach (var item in Docs)
                                                          if (item.ParentFolders.Count == 0)
                                                              lvCloud.cbAdd(item.Id, item.Title, 0, item.AtomEntry.AlternateUri.Content);
                                                      if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                                                      btnSign.cbEnable(true);
                                                      txtFolderName.cbEnable(true);
                                                      btnCreate.cbEnable(true);
                                                      btnDelete.cbEnable(false);
                                                      btnUp.cbEnable(false);
                                                      btnAdd.cbEnable(false);
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
                        MessageBox.Show("Please check your Google account", "Invalid login"); return;
                    }

                    CloudStatus.Text = "Waiting..."; lvCloud.Items.Clear();
                    PR = new PicasaRequest(new RequestSettings("Ripper", LoginName, tbPass.Text) { AutoPaging=true});
                    Func<Feed<Album>> GA = () => PR.GetAlbums();
                    GA.BeginInvoke(_ =>
                    {
                        Albums = GA.EndInvoke(_).Entries.ToList();
                        foreach (var item in Albums)
                            lvCloud.cbAdd(item.Id, item.Title, 0, item.AtomEntry.AlternateUri.Content);
                        if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                        txtFolderName.cbEnable(true);
                        btnCreate.cbEnable(true);
                        cbPublic.cbEnable(false);
                        btnUp.cbEnable(false);
                        btnAdd.cbEnable(false);
                        btnDelete.cbEnable(false);
                        btnSign.cbEnable(true);
                        Prompt = "Picasa: " + Albums.Count + " Album(s)";
                    }, null);
                    break;
                #endregion
            }
            btnSign.Enabled = false;
            Properties.Settings.Default.CloudID = LoginName;
            Properties.Settings.Default.Password = tbPass.Text;
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
                        Document doc = Docs.Where(_ => _.Type == Document.DocumentType.Folder).Single(_ => _.Id == lvi.Name);
                        if (doc != null)
                        {
                            Folder = Folder ?? new Stack<Document>();
                            CloudStatus.Text = "Listing \"" + doc.Title + "\"";
                            Folder.Push(doc);
                            btnDelete.Enabled = btnAdd.Enabled = btnCreate.Enabled = false;
                            lvCloud.Items.Clear();
                            Func<Feed<Document>> DFC = () => DR.GetFolderContent(doc);
                            DFC.BeginInvoke(ar =>
                                {
                                    Docs = DFC.EndInvoke(ar).Entries.ToList();
                                    foreach (var item in Docs)
                                        lvCloud.cbAdd(item.Id, item.Title, item.Type == Document.DocumentType.Folder ? 0 : 2, item.AtomEntry.AlternateUri.Content);
                                    if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                                    btnUp.cbEnable(true);
                                    btnCreate.cbEnable(true);
                                    btnAdd.cbEnable(true);
                                    txtFolderName.cbEnable(true);
                                    Prompt = string.Join(" > ", Folder.Select(_ => _.Title).Reverse().ToArray()) + ": " + Docs.Count + " item(s)";
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
                        Album a = Albums.Single(_ => _.Id == lvi.Name);
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
                                        lvCloud.cbAdd(item.Id, item.Title, 1, item.AtomEntry.AlternateUri.Content);
                                    if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                                    btnUp.cbEnable(true);
                                    btnCreate.cbEnable(false);
                                    txtFolderName.cbEnable(true);
                                    btnAdd.cbEnable(true);
                                    Prompt = a.Title + ": " + Photos.Count + " Photo(s)";
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
                    btnUp.Enabled = btnDelete.Enabled = btnCreate.Enabled = txtFolderName.Enabled = btnAdd.Enabled = false;
                    if (Folder.Count == 0)
                    {
                        CloudStatus.Text = "Waiting...";
                        Func<Feed<Document>> GE = () => DR.GetFolders();
                        GE.BeginInvoke(ar =>
                        {
                            Docs = GE.EndInvoke(ar).Entries.ToList();
                            foreach (var item in Docs)
                                if (item.ParentFolders.Count == 0)
                                    lvCloud.cbAdd(item.Id, item.Title, 0, item.AtomEntry.AlternateUri.Content);
                            if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                            txtFolderName.cbEnable(true);
                            //btnAddFiles.cbEnable(true);
                            btnCreate.cbEnable(true);
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
                                    lvCloud.cbAdd(item.Id, item.Title, item.Type == Document.DocumentType.Folder ? 0 : 2, item.AtomEntry.AlternateUri.Content);
                                if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                                btnUp.cbEnable(true);
                                btnAdd.cbEnable(true);
                                btnCreate.cbEnable(true);
                                txtFolderName.cbEnable(true);
                                Prompt = string.Join(" > ", Folder.Select(_ => _.Title).Reverse().ToArray()) + ": " + Docs.Count + " item(s)";
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
                    btnUp.Enabled = btnDelete.Enabled = btnCreate.Enabled = txtFolderName.Enabled = btnAdd.Enabled = false;
                    Func<Feed<Album>> GA = () => PR.GetAlbums();
                    GA.BeginInvoke(_ =>
                    {
                        Albums = GA.EndInvoke(_).Entries.ToList();
                        foreach (var item in Albums)
                            lvCloud.cbAdd(item.Id, item.Title, 0, item.AtomEntry.AlternateUri.Content);
                        if (cldCache != null) { cldCache.Clear(); cldCache = null; }
                        Photos.Clear(); AlbumID = null;
                        txtFolderName.cbEnable(true);
                        btnCreate.cbEnable(true);
                        Prompt = "Picasa: " + Albums.Count + " Albums(s)";
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
            btnCreate.Enabled = false;
            btnUp.Enabled = false;
            switch (Service)
            {
                #region GDrive
                case CloudType.GDrive:
                    Func<Document> CD = () => DR.CreateDocument(new Document() { Title = foldername, Type = Document.DocumentType.Folder });
                    Document @base = null;
                    string title, tip;
                    if (Folder != null && Folder.Count > 0) @base = Folder.Peek();
                    CD.BeginInvoke(c =>
                        {
                            var @new = CD.EndInvoke(c);
                            if (@base != null)
                            {
                                Func<Document> MD = () => DR.MoveDocumentTo(@base, @new);
                                MD.BeginInvoke(m =>
                                    {
                                        @new = MD.EndInvoke(m);
                                    }, null);
                            }
                            title = @new.Title; tip = @new.AtomEntry.AlternateUri.Content;
                            lvCloud.cbAdd(@new.Id, title, 0, tip);
                            if (cldCache != null) cldCache.Add(new ListViewItem(title, 0) { Name = @new.Id, ToolTipText = tip });
                            Docs.Add(@new);
                            txtFolderName.cbEnable(true);
                            btnCreate.cbEnable(true);
                            btnUp.cbEnable(@base != null);
                            Prompt = (@base == null ? "GDrive" : string.Join(" > ", Folder.Select(f =>f.Title).Reverse().ToArray())) + ": " + Docs.Count + " item(s)";
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
                    cbPublic.ThreeState = false;
                    a.Access = cbPublic.Checked ? "public" : "private";
                    Func<Album> CA = () => PR.Insert<Album>(new Uri(PicasaQuery.CreatePicasaUri(LoginName)), a);
                    CA.BeginInvoke(_ =>
                        {
                            var @new = CA.EndInvoke(_);
                            lvCloud.cbAdd(@new.Id, foldername, 0, @new.AtomEntry.AlternateUri.Content);
                            if (cldCache != null) cldCache.Add(new ListViewItem(foldername, 0) {Name=@new.Id, ToolTipText = @new.AtomEntry.AlternateUri.Content });
                            Albums.Add(@new);
                            btnCreate.cbEnable(true);
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
                    List<ListViewItem> items = new List<ListViewItem>();
                    foreach (ListViewItem item in lvCloud.SelectedItems)
                        items.Add(item);
                    if (MessageBox.Show("Are you sure to delete items below:\n" + string.Join(", ", items.Select(i => i.Text).ToArray()), "Delete Cloud Items", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        btnDelete.Enabled = btnUp.Enabled = false;
                        ThreadPool.QueueUserWorkItem(DeleteCloudItem, items);
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
                            var doc = Docs.Single(_ => _.Id == lvi.Name);
                            doc.DocumentEntry.Delete();
                            Docs.Remove(doc);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message, "Delete " + lvi.Text + " ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                        lvCloud.Invoke(RemoveItem, lvi);
                        if (cldCache != null) cldCache.Remove(lvi);
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
                                var p = Photos.Single(_ => _.Id == lvi.Name);
                                PR.Delete(p); Photos.Remove(p);
                            }
                            else
                            {
                                var a = Albums.Single(_ => _.Id == lvi.Name);
                                PR.Delete(a); ; Albums.Remove(a);
                            }
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.InnerException.Message, "Delete " + lvi.Text + " ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                        lvCloud.Invoke(RemoveItem, lvi);
                        if (cldCache != null) cldCache.Remove(lvi);
                    }
                    btnUp.cbEnable(AlbumID != null);
                    Prompt = "Done";
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
                            lvCloud.cbAdd(@new.Id, de.Title.Text, 2, ttt);
                            if (cldCache != null) cldCache.Add(new ListViewItem(de.Title.Text, 2) {Name=@new.Id, ToolTipText = ttt });
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
                                Photo p=new Photo() { AtomEntry = ae };
                                Photos.Add(p);
                                fs.Close();
                                lvCloud.cbAdd(p.Id, filename, 1, ae.AlternateUri.Content);
                                if (cldCache != null) cldCache.Add(new ListViewItem(filename, 1) { Name = p.Id, ToolTipText = ae.AlternateUri.Content });
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
                    Document doc = Docs.Single(_ => _.Id == lvi.Name);
                    doc.Title = e.Label;
                    Func<Document> DU = () => DR.Update<Document>(doc);
                    DU.BeginInvoke(_ =>
                    {
                        Document @new = DU.EndInvoke(_); Docs.Remove(doc); Docs.Add(@new); lvi.ToolTipText = @new.AtomEntry.AlternateUri.Content; 
                        if (cldCache != null)
                        {
                            ListViewItem item = cldCache.Single(i => i.Name == lvi.Name);
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
                        Album a = Albums.Single(_ => _.Id == lvi.Name);
                        a.Title = e.Label;
                        Func<Album> PU = () => PR.Update<Album>(a);
                        PU.BeginInvoke(_ =>
                        {
                            Album @new = PU.EndInvoke(_); Albums.Remove(a); Albums.Add(@new); lvi.ToolTipText = @new.AtomEntry.AlternateUri.Content;
                            if (cldCache != null)
                            {
                                ListViewItem item = cldCache.Single(i => i.Name == lvi.Name);
                                item.Text = e.Label; item.ToolTipText = lvi.ToolTipText;
                            }
                            Prompt = "Done";
                        }, null);
                    }
                    else
                    {
                        Photo p = Photos.Single(_ => _.Id == lvi.Name);
                        p.Title = e.Label;
                        Func<Photo> PU = () => PR.Update<Photo>(p);
                        PU.BeginInvoke(_ =>
                        {
                            Photo @new = PU.EndInvoke(_); Photos.Remove(p); Photos.Add(@new); lvi.ToolTipText = @new.AtomEntry.AlternateUri.Content;
                            if (cldCache != null)
                            {
                                ListViewItem item = cldCache.Single(i => i.Name == lvi.Name);
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
                    if (DR == null||!btnAdd.Enabled)
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
                        Document doc= Docs.Single(_ => _.Id == lvi.Name);
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
                                total += Albums.Single(_ => _.Id == item.Name).NumPhotos;
                                if (Albums.Single(_ => _.Id == item.Name).Access == "public") check++;
                            }
                            cbPublic.CheckState = check == 0 ? CheckState.Unchecked : check == lvCloud.SelectedItems.Count ? CheckState.Checked : CheckState.Indeterminate;
                            Prompt = string.Format("Total: {0} Photos", total);
                        }
                        else
                            Prompt = "Updated: " + Photos.Single(_ => _.Id == lvi.Name).Updated.ToShortDateString();
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
                        cbPublic.CheckState = CheckState.Unchecked;
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
                lvCloud.Items.AddRange(cldCache.ToArray());
                return;
            }
            lvCloud.Items.AddRange(cldCache.Where(_ => _.Text.ToLower().Contains(text)).ToArray());
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
