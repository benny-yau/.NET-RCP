using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FlashDevelop;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace FileExplorer
{
    public class PluginUI : DockPanelControl
    {
        private System.Windows.Forms.ListView fileView;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ContextMenuStrip menu;
        private System.Windows.Forms.ToolStripSeparator separatorItem1;
        private System.Windows.Forms.ToolStripMenuItem runButton;
        private System.Windows.Forms.ToolStripMenuItem renameButton;
        private System.Windows.Forms.ToolStripMenuItem deleteButton;
        private System.Windows.Forms.ToolStripMenuItem shellButton;
        private System.Windows.Forms.ToolStripSpringComboBox selectedPath;
        private System.Windows.Forms.ToolStripButton browseButton;
        private System.Windows.Forms.ColumnHeader fileHeader;
        private System.Windows.Forms.ColumnHeader sizeHeader;
        private System.Windows.Forms.ColumnHeader typeHeader;
        private System.Windows.Forms.ColumnHeader modifiedHeader;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ImageList imageList;
        private System.Boolean updateInProgress;
        private System.Int64 lastUpdateTimeStamp;
        private System.Int32 prevColumnClick;
        private ListViewSorter listViewSorter;
        private FileSystemWatcher watcher;
        private PluginMain pluginMain;
        private String autoSelectItem;
        private int lastSelectedIndex = -1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private int topVisibleIndex = -1;

        public PluginUI(PluginMain pluginMain)
        {
            this.pluginMain = pluginMain;
            this.listViewSorter = new ListViewSorter();
            this.InitializeComponent();
            this.InitializeGraphics();
            this.InitializeContextMenu();
            this.InitializeLayout();
            this.InitializeTexts();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.watcher = new System.IO.FileSystemWatcher();
            this.modifiedHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.typeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fileView = new System.Windows.Forms.ListView();
            this.fileHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sizeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.selectedPath = new System.Windows.Forms.ToolStripSpringComboBox();
            this.browseButton = new System.Windows.Forms.ToolStripButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.watcher)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // watcher
            // 
            this.watcher.EnableRaisingEvents = true;
            this.watcher.NotifyFilter = ((System.IO.NotifyFilters)((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName)));
            this.watcher.SynchronizingObject = this;
            this.watcher.Changed += new System.IO.FileSystemEventHandler(this.WatcherChanged);
            this.watcher.Created += new System.IO.FileSystemEventHandler(this.WatcherChanged);
            this.watcher.Deleted += new System.IO.FileSystemEventHandler(this.WatcherChanged);
            this.watcher.Renamed += new System.IO.RenamedEventHandler(this.WatcherRenamed);
            // 
            // modifiedHeader
            // 
            this.modifiedHeader.Text = "Modified";
            this.modifiedHeader.Width = 120;
            // 
            // typeHeader
            // 
            this.typeHeader.Text = "Type";
            // 
            // fileView
            // 
            this.fileView.AllowDrop = true;
            this.fileView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fileView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileHeader,
            this.sizeHeader,
            this.typeHeader,
            this.modifiedHeader});
            this.fileView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileView.Location = new System.Drawing.Point(1, 26);
            this.fileView.Name = "fileView";
            this.fileView.Size = new System.Drawing.Size(278, 326);
            this.fileView.TabIndex = 5;
            this.fileView.UseCompatibleStateImageBehavior = false;
            this.fileView.View = System.Windows.Forms.View.Details;
            this.fileView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.fileView_AfterLabelEdit);
            this.fileView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.FileViewColumnClick);
            this.fileView.ItemActivate += new System.EventHandler(this.FileViewItemActivate);
            this.fileView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.fileView_ItemDrag);
            this.fileView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FileViewMouseUp);
            // 
            // fileHeader
            // 
            this.fileHeader.Text = "Files";
            this.fileHeader.Width = 190;
            // 
            // sizeHeader
            // 
            this.sizeHeader.Text = "Size";
            this.sizeHeader.Width = 55;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Open a folder to list the files in the folder";
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedPath,
            this.browseButton});
            this.toolStrip.Location = new System.Drawing.Point(1, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 1, 2, 2);
            this.toolStrip.Size = new System.Drawing.Size(278, 26);
            this.toolStrip.Stretch = true;
            this.toolStrip.TabIndex = 6;
            // 
            // selectedPath
            // 
            this.selectedPath.Name = "selectedPath";
            this.selectedPath.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.selectedPath.Size = new System.Drawing.Size(219, 23);
            this.selectedPath.SelectedIndexChanged += new System.EventHandler(this.SelectedPathSelectedIndexChanged);
            this.selectedPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SelectedPathKeyDown);
            // 
            // browseButton
            // 
            this.browseButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.browseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.browseButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(23, 21);
            this.browseButton.Text = "Browse";
            this.browseButton.Click += new System.EventHandler(this.BrowseButtonClick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // PluginUI
            // 
            this.Controls.Add(this.fileView);
            this.Controls.Add(this.toolStrip);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(280, 352);
            ((System.ComponentModel.ISupportInitialize)(this.watcher)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary> 
        /// We have to do final initialization here because we might 
        /// need to have a window handle to pre-populate the file list.
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.Initialize(null, null);
        }

        /// <summary>
        /// Shows the explorer shell menu
        /// </summary>
        private void ShowShellMenu(Object sender, EventArgs e)
        {
            Int32 count = this.fileView.SelectedItems.Count;
            FileInfo[] selectedPathsAndFiles = new FileInfo[count];
            ShellContextMenu scm = new ShellContextMenu();
            for (Int32 i = 0; i < count; i++)
            {
                String path = this.fileView.SelectedItems[i].Tag.ToString();
                selectedPathsAndFiles[i] = new FileInfo(path);
            }
            if (count == 0)
            {
                String path = this.selectedPath.Text;
                if (!Directory.Exists(path)) return;
                selectedPathsAndFiles = new FileInfo[1];
                selectedPathsAndFiles[0] = new FileInfo(path);
            }
            this.menu.Hide(); /* Hide default menu */
            Point location = new Point(this.menu.Bounds.Left, this.menu.Bounds.Top);
            scm.ShowContextMenu(selectedPathsAndFiles, location);
        }

        /// <summary>
        /// Creates and attaches the context menu
        /// </summary>
        private void InitializeContextMenu()
        {
            this.menu = new ContextMenuStrip();
            this.menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.RefreshView"), null, new EventHandler(this.RefreshFileView)));
            this.menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.CommandPromptHere"), null, new EventHandler(this.CommandPromptHere)));
            this.menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.ExploreHere"), null, new EventHandler(this.ExploreHere)));
            this.shellButton = new ToolStripMenuItem(TextHelper.GetString("Label.ShellMenu"), null, new EventHandler(this.ShowShellMenu));
            this.menu.Items.Add(this.shellButton);
            this.separatorItem1 = new ToolStripSeparator();
            this.menu.Items.Add(separatorItem1);
            this.runButton = new ToolStripMenuItem(TextHelper.GetString("Label.Run"), null, new EventHandler(this.OpenItem));
            this.renameButton = new ToolStripMenuItem(TextHelper.GetString("Label.Rename"), null, new EventHandler(this.RenameItem));
            this.deleteButton = new ToolStripMenuItem(TextHelper.GetString("Label.Delete"), null, new EventHandler(this.DeleteItems));
            this.menu.Items.Add(this.runButton);
            this.menu.Items.Add(this.renameButton);
            this.menu.Items.Add(this.deleteButton);
            this.menu.Font = PluginBase.Settings.DefaultFont;
            this.menu.Renderer = new DockPanelStripRenderer(false);
            this.fileView.ContextMenuStrip = this.menu;
        }

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        private void InitializeGraphics()
        {
            this.imageList = new ImageList();
            this.imageList.ImageSize = GetSmallIconSize();
            this.imageList.ColorDepth = ColorDepth.Depth32Bit;
            this.browseButton.Image = PluginBase.MainForm.FindImage("203");
            this.fileView.SmallImageList = this.imageList;
        }

        /// <summary>
        /// Applies localized texts to the control
        /// </summary>
        public void InitializeTexts()
        {
            this.fileHeader.Text = TextHelper.GetString("Header.Files");
            this.modifiedHeader.Text = TextHelper.GetString("Header.Modified");
            this.folderBrowserDialog.Description = TextHelper.GetString("Info.BrowseDescription");
            this.browseButton.ToolTipText = TextHelper.GetString("ToolTip.Browse");
            this.typeHeader.Text = TextHelper.GetString("Header.Type");
            this.sizeHeader.Text = TextHelper.GetString("Header.Size");
            this.modifiedHeader.Width = -2; // Extend last column
        }

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        private void InitializeLayout()
        {
            this.toolStrip.Renderer = new DockPanelStripRenderer();
        }

        /// <summary>
        /// Browses to the selected path
        /// </summary>
        public void BrowseTo(String path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                ErrorManager.ShowInfo("File path does not exist.");
                return;
            }
            autoSelectItem = path;
            if (!Directory.Exists(path)) path = Path.GetDirectoryName(path);
            if (String.Compare(path, this.selectedPath.Text, true) == 0)
                SelectAndScrollToItem();
            else
                this.PopulateFileView(path);
        }

        /// <summary>
        /// Gets the reference to the context menu
        /// </summary>
        public ContextMenuStrip GetContextMenu()
        {
            return this.menu;
        }

        /// <summary>
        /// Add the path to the combo box
        /// </summary>
        public void AddToMRU(String path)
        {
            if (Directory.Exists(path) && !this.selectedPath.Items.Contains(path))
            {
                this.selectedPath.Items.Add(path);
            }
        }

        /// <summary>
        /// List last open path on load
        /// </summary>
        private void Initialize(Object sender, System.EventArgs e)
        {
            String path = PathHelper.AppDir;
            String pathToCheck = this.pluginMain.Settings.FilePath;
            if (Directory.Exists(pathToCheck)) path = pathToCheck;
            this.listViewSorter.SortColumn = this.pluginMain.Settings.SortColumn;
            if (this.pluginMain.Settings.SortOrder == 0) this.listViewSorter.Order = SortOrder.Ascending;
            else this.listViewSorter.Order = SortOrder.Descending;
            this.watcher.Path = path; this.watcher.EnableRaisingEvents = true;
            this.fileView.ListViewItemSorter = this.listViewSorter;
            this.PopulateFileView(path);
        }

        /// <summary>
        /// Update files listview. If the path is invalid, use the last valid path
        /// </summary>
        private void PopulateFileView(String path)
        {
            try
            {
                if (!Directory.Exists(path)) return;
                if (this.updateInProgress) return;
                this.updateInProgress = true;
                this.fileView.ContextMenu = null;
                this.browseButton.Enabled = false;
                this.selectedPath.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                this.AddToMRU(path);
                path = PathHelper.GetPhysicalPathName(path);
                this.SetLastSelectedState(path);
                this.selectedPath.Text = path;
                this.pluginMain.Settings.FilePath = path;
                this.ClearImageList();
                this.fileView.ListViewItemSorter = null;
                this.fileView.Items.Clear();
                backgroundWorker1.RunWorkerAsync(path);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Asynchronous retrieving of file system info and updating UI
        /// </summary>
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            String path = e.Argument.ToString();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] infos = dir.GetFileSystemInfos();
            UpdateUI(path, dir, infos);
        }


        /// <summary>
        /// Updates the UI in the GUI thread
        /// </summary>
        private void UpdateUI(string path, DirectoryInfo directory, FileSystemInfo[] infos)
        {
            if (directory.Parent != null)
            {
                fileView.Invoke((MethodInvoker)delegate { AddFirstItem(directory); });
            }
            foreach (FileSystemInfo info in infos)
            {
                //System.Threading.Thread.Sleep(1000);
                DirectoryInfo subDir = info as DirectoryInfo;
                if (subDir != null && (subDir.Attributes & FileAttributes.Hidden) == 0)
                {
                    fileView.Invoke((MethodInvoker)delegate { AddDirectory(subDir); });
                }
                FileInfo file = info as FileInfo;
                if (file != null && (file.Attributes & FileAttributes.Hidden) == 0)
                {
                    fileView.Invoke((MethodInvoker)delegate { AddFile(file); });
                }
            }
        }

        void AddFirstItem(DirectoryInfo directory)
        {
            ListViewItem item = new ListViewItem("[..]", ExtractIconIfNecessary("/Folder/"));
            item.Tag = directory.Parent.FullName;
            item.SubItems.Add("-");
            item.SubItems.Add("-");
            item.SubItems.Add("-");
            this.fileView.Items.Add(item);
        }

        void AddDirectory(FileSystemInfo subDir)
        {
            ListViewItem item = new ListViewItem(subDir.Name, ExtractIconIfNecessary(subDir.FullName));
            item.Tag = subDir.FullName;
            item.SubItems.Add("-");
            item.SubItems.Add("-");
            item.SubItems.Add(subDir.LastWriteTime.ToString());
            this.fileView.Items.Add(item);
        }

        void AddFile(FileInfo file)
        {
            String kbs = TextHelper.GetString("Info.Kilobytes");
            ListViewItem item = new ListViewItem(file.Name, ExtractIconIfNecessary(file.FullName));
            item.Tag = file.FullName;
            if (file.Length / 1024 < 1) item.SubItems.Add("1 " + kbs);
            else item.SubItems.Add((file.Length / 1024).ToString() + " " + kbs);
            item.SubItems.Add(file.Extension.ToUpper().Replace(".", ""));
            item.SubItems.Add(file.LastWriteTime.ToString());
            this.fileView.Items.Add(item);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.Cursor = Cursors.Default;
            this.watcher.Path = this.selectedPath.Text;
            this.fileView.ListViewItemSorter = this.listViewSorter;
            this.fileView.ContextMenuStrip = this.menu;
            this.selectedPath.Enabled = true;
            this.browseButton.Enabled = true;
            this.updateInProgress = false;
            this.modifiedHeader.Width = -2; // Extend last column
            this.SelectAndScrollToItem();
        }

        /// <summary>
        /// Set last selected item and top item visible in file explorer
        /// </summary>
        private void SetLastSelectedState(String path)
        {
            if (String.Compare(path, this.selectedPath.Text, true) == 0)
            {
                topVisibleIndex = this.fileView.TopItem.Index;
                if (this.fileView.SelectedItems.Count > 0)
                {
                    ListViewItem lastItem = this.fileView.SelectedItems[0];
                    lastSelectedIndex = lastItem.Index;
                }
            }
        }

        /// <summary>
        /// Select and scroll to item in file explorer
        /// </summary>
        private void SelectAndScrollToItem()
        {
            if (autoSelectItem == null && topVisibleIndex == -1) return;
            if (autoSelectItem != null)
            {
                foreach (ListViewItem item in this.fileView.Items)
                {
                    if (String.Compare(item.Tag.ToString(), this.autoSelectItem, true) == 0)
                    {
                        HighlightSelectedItem(item);
                        this.fileView.EnsureVisible(item.Index);
                        break;
                    }
                }
            }
            else
            {
                if (this.fileView.Items.Count - 1 >= topVisibleIndex)
                    this.fileView.TopItem = this.fileView.Items[topVisibleIndex];
                else if (this.fileView.Items.Count > 0)
                    this.fileView.TopItem = this.fileView.Items[this.fileView.Items.Count - 1];

                if (lastSelectedIndex >= 0)
                {
                    if (this.fileView.Items.Count - 1 >= lastSelectedIndex)
                        HighlightSelectedItem(this.fileView.Items[lastSelectedIndex]);
                    else if (this.fileView.Items.Count > 0)
                        HighlightSelectedItem(this.fileView.Items[this.fileView.Items.Count - 1]);
                }
            }
            this.fileView.Focus();
            this.autoSelectItem = null;
            this.topVisibleIndex = -1;
            this.lastSelectedIndex = -1;
        }

        /// <summary>
        /// Open the folder browser dialog
        /// </summary>
        private void BrowseButtonClick(Object sender, System.EventArgs e)
        {
            try
            {
                if (Directory.Exists(this.selectedPath.Text))
                {
                    this.folderBrowserDialog.SelectedPath = this.selectedPath.Text;
                }
                if (this.folderBrowserDialog.ShowDialog((Form)PluginBase.MainForm) == DialogResult.OK)
                {
                    this.PopulateFileView(this.folderBrowserDialog.SelectedPath);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Repopulate when user changes the path from the combo box
        /// </summary>
        private void SelectedPathSelectedIndexChanged(Object sender, System.EventArgs e)
        {
            if (this.selectedPath.SelectedIndex != -1)
            {
                String path = this.selectedPath.SelectedItem.ToString();
                if (Directory.Exists(path)) this.PopulateFileView(path);
            }
        }

        /// <summary>
        /// Key pressed while editing the selected path
        /// </summary>
        private void SelectedPathKeyDown(Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                String path = this.selectedPath.Text;
                if (Directory.Exists(path))
                {
                    this.PopulateFileView(path);
                }
                else
                {
                    String directoryName = Path.GetDirectoryName(path);
                    if (Directory.Exists(directoryName))
                        this.PopulateFileView(directoryName);
                }
            }
        }

        /// <summary>
        /// Gets a list of currectly selected files
        /// </summary>
        private String[] GetSelectedFiles()
        {
            Int32 i = 0;
            if (this.fileView.SelectedItems.Count == 0) return null;
            String[] files = new String[this.fileView.SelectedItems.Count];
            foreach (ListViewItem item in this.fileView.SelectedItems)
            {
                files[i++] = item.Tag.ToString();
            }
            return files;
        }


        /// <summary>
        /// Checks if the path list contains only files
        /// </summary> 
        private Boolean ContainsOnlyFiles(String[] files)
        {
            for (Int32 i = 0; i < files.Length; i++)
            {
                if (Directory.Exists(files[i])) return false;
            }
            return true;
        }


        /// <summary>
        /// Opens the selected file or browses to a path
        /// </summary>
        private void FileViewItemActivate(Object sender, System.EventArgs e)
        {
            if (this.fileView.SelectedItems.Count == 0) return;
            String file = this.fileView.SelectedItems[0].Tag.ToString();
            if (Control.ModifierKeys == Keys.Shift) this.OpenItem(null, null);
            else
            {
                if (File.Exists(file))
                {
                    String extension = Path.GetExtension(file).ToLower();
                    if (PluginBase.Settings.FileTypesToOpen.Contains(extension))
                        PluginBase.MainForm.BrowseWebsite(file);
                    else
                        ProcessHelper.StartAsync(file);
                }
                else if (Directory.Exists(file))
                    this.PopulateFileView(file);
            }
        }

        /// <summary>
        /// Starts the dragging operation
        /// </summary>
        private void fileView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            String[] files = this.GetSelectedFiles();
            if (files != null && e.Button == MouseButtons.Left)
            {
                DataObject data = new DataObject(DataFormats.FileDrop, files);
                this.fileView.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        /// <summary>
        /// Creates the context menu on right button click
        /// </summary>
        private void FileViewMouseUp(Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.UpdateMenuItemVisibility();
            }
        }

        /// <summary>
        /// Updates the context menu item visibility
        /// </summary>
        private void UpdateMenuItemVisibility()
        {
            Boolean notFirstItem = true;
            Int32 selectedItems = this.fileView.SelectedItems.Count;
            if (selectedItems > 0) notFirstItem = (this.fileView.SelectedItems[0].Text.CompareTo("[..]") != 0);

            this.separatorItem1.Visible = (selectedItems > 0 && notFirstItem);
            this.renameButton.Visible = (selectedItems == 1 && notFirstItem);
            this.runButton.Visible = (selectedItems == 1 && notFirstItem);
            this.deleteButton.Visible = (selectedItems > 0 && notFirstItem);
        }

        /// <summary>
        /// Check whether the selected items are only files
        /// </summary> 
        private Boolean SelectedItemsAreOnlyFiles()
        {
            for (Int32 i = 0; i < this.fileView.SelectedItems.Count; i++)
            {
                String path = this.fileView.SelectedItems[i].Tag.ToString();
                if (Directory.Exists(path)) return false;
            }
            return true;
        }


        /// <summary>
        /// Refreshes the file view
        /// </summary>
        private void RefreshFileView(Object sender, System.EventArgs e)
        {
            String path = this.selectedPath.Text;
            this.PopulateFileView(path);
        }

        /// <summary>
        /// Opens Windows explorer in the current path
        /// </summary>
        private void ExploreHere(Object sender, System.EventArgs e)
        {
            String selectPath = this.selectedPath.Text;
            if (this.fileView.SelectedItems.Count > 0) selectPath = this.fileView.SelectedItems[0].Tag.ToString();
            DataEvent de = new DataEvent(EventType.Command, "FileExplorer.Explore", selectPath);
            EventManager.DispatchEvent(this, de);
        }

        /// <summary>
        /// Opens the command prompt in the current path
        /// </summary>
        private void CommandPromptHere(Object sender, System.EventArgs e)
        {
            DataEvent de = new DataEvent(EventType.Command, "FileExplorer.PromptHere", this.selectedPath.Text);
            EventManager.DispatchEvent(this, de);
        }

        /// <summary>
        /// Sorts items on user column click
        /// </summary>
        private void FileViewColumnClick(Object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            if (this.prevColumnClick == e.Column)
            {
                this.listViewSorter.Order = (this.listViewSorter.Order == SortOrder.Descending) ? SortOrder.Ascending : SortOrder.Descending;
            }
            else this.listViewSorter.Order = SortOrder.Ascending;
            if (this.listViewSorter.Order == SortOrder.Ascending)
            {
                this.pluginMain.Settings.SortOrder = 0;
            }
            else this.pluginMain.Settings.SortOrder = 1;
            this.prevColumnClick = e.Column;
            this.pluginMain.Settings.SortColumn = e.Column;
            this.listViewSorter.SortColumn = e.Column;
            this.fileView.Sort();
        }


        /// <summary>
        /// Deletes the selected items
        /// </summary>
        private void DeleteItems(Object sender, System.EventArgs e)
        {
            try
            {
                String message = TextHelper.GetString("Info.ConfirmDelete");
                String confirm = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                DialogResult result = MessageBox.Show(message, " " + confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    for (Int32 i = 0; i < this.fileView.SelectedItems.Count; i++)
                    {
                        String path = this.fileView.SelectedItems[i].Tag.ToString();
                        if (!FileHelper.Recycle(path))
                        {
                            String error = TextHelper.GetString("FlashDevelop.Info.CouldNotBeRecycled");
                            throw new Exception(error + " " + path);
                        }
                        DocumentManager.CloseDocuments(path);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Renames current file or directory
        /// </summary>
        private void RenameItem(Object sender, System.EventArgs e)
        {
            this.fileView.LabelEdit = true;
            this.fileView.SelectedItems[0].BeginEdit();
        }

        /// <summary>
        /// Disable labeling after editing
        /// </summary>
        private void fileView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            this.fileView.LabelEdit = false;
        }

        /// <summary>
        /// Opens the current file or directory with associated program 
        /// </summary>
        private void OpenItem(Object sender, System.EventArgs e)
        {
            try
            {
                if (this.fileView.SelectedItems.Count == 0) return;
                String file = this.fileView.SelectedItems[0].Tag.ToString();
                ProcessHelper.StartAsync(file);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Highlights the selected item
        /// </summary>
        private void HighlightSelectedItem(ListViewItem item)
        {
            foreach (ListViewItem selectedItem in this.fileView.SelectedItems)
            {
                selectedItem.Selected = false;
            }
            item.Selected = true;
        }

        /// <summary>
        /// The directory we're watching has changed - refresh!
        /// </summary>
        private void WatcherChanged(Object sender, FileSystemEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                long timestamp = DateTime.Now.Ticks;
                if (timestamp - this.lastUpdateTimeStamp < 500) return;
                this.lastUpdateTimeStamp = timestamp; // Store timestamp
                this.PopulateFileView(this.selectedPath.Text);
            });
        }

        /// <summary>
        /// The directory we're watching has changed - refresh!
        /// </summary>
        private void WatcherRenamed(Object sender, RenamedEventArgs e)
        {
            this.WatcherChanged(sender, null);
        }

        #endregion

        #region Icon Management

        /// <summary>
        /// Ask the shell to feed us the appropriate icon for the given file, but
        /// first try looking in our cache to see if we've already loaded it.
        /// </summary>
        private int ExtractIconIfNecessary(String path)
        {
            Icon icon;
            Size size = GetSmallIconSize();
            String ext = Path.GetExtension(path);
            if (String.IsNullOrEmpty(ext)) ext = "folder";
            if (this.imageList.Images[ext] != null)
                return this.imageList.Images.IndexOfKey(ext);
            else
            {
                if (File.Exists(path))
                    icon = IconExtractor.GetFileIcon(path, false, true);
                else
                    icon = IconExtractor.GetFolderIcon(path, false, true);
                Image image = ImageKonverter.ImageResize(icon.ToBitmap(), size.Width, size.Height);
                this.imageList.Images.Add(ext, image); icon.Dispose(); image.Dispose();
                return this.imageList.Images.Count - 1;
            }
        }

        /// <summary>
        /// Dispose all images entirely.
        /// </summary>
        private void ClearImageList()
        {
            this.imageList.Images.Clear();
            this.imageList.Dispose();
            this.imageList = new ImageList();
            this.imageList.ImageSize = GetSmallIconSize();
            this.imageList.ColorDepth = ColorDepth.Depth32Bit;
            this.fileView.SmallImageList = this.imageList;
        }

        /// <summary>
        /// Gets the small icon size. High dpi has slightly bigger icons
        /// </summary>
        /// <returns></returns>
        private Size GetSmallIconSize()
        {
            Size size = SystemInformation.SmallIconSize;
            if (size.Width > 16) return new Size(18, 18);
            else return size;
        }

        #endregion



    }

}
