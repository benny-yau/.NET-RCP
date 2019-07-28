
#region Imports

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CSScriptLibrary;
using FlashDevelop.Controls;
using FlashDevelop.Dialogs;
using FlashDevelop.Docking;
using FlashDevelop.Helpers;
using FlashDevelop.Managers;
using FlashDevelop.Settings;
using FlashDevelop.Utilities;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using WeifenLuo.WinFormsUI.Docking;
#endregion

namespace FlashDevelop
{
    public class MainForm : Form, IMainForm
    {
        #region Constructor

        public MainForm()
        {
            MainForm.Instance = this;
            this.InitializeGraphics();
            PluginBase.Initialize(this);
            this.InitializeErrorLog();
            this.InitializeSettings();
            this.InitializeLocalization();
            this.InitializeRendering();
            this.InitializeComponents();
            this.InitializeProcessRunner();
            this.InitializeMainForm();
        }

        /// <summary>
        /// Initializes some extra error logging
        /// </summary>
        private void InitializeErrorLog()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);
        }

        /// <summary>
        /// Handles the catched unhandled exception and logs it
        /// </summary>
        private void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = new Exception(e.ExceptionObject.ToString());
            ErrorManager.AddToLog("Unhandled exception: ", exception);
        }

        #endregion

        #region Private Properties

        /* Components */
        private DockPanel dockPanel;
        private ToolStrip toolStrip;
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripPanel toolStripPanel;
        private ToolStripProgressBar toolStripProgressBar;
        private ToolStripStatusLabel toolStripProgressLabel;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ProcessRunner processRunner;
        
        /* Settings */
        private SettingObject appSettings;

        /* Context Menus */
        public ContextMenuStrip tabMenu;

        /* Working Dir */
        String workingDirectory = String.Empty;

        /* Form State */
        private FormState formState;
        private Hashtable fullScreenDocks;
        private Boolean isFullScreen = false;
        private Boolean panelIsActive = false;
        private Boolean processingContents = false;
        private Boolean restoringContents = false;
        private Boolean closingEntirely = false;
        private Boolean closeAllCanceled = false;
        private Boolean restartRequested = false;
        private Boolean closingAll = false;
        
        /* Singleton */
        public static Boolean IsFirst;
        public static MainForm Instance;
        public static String[] Arguments;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the DockPanel
        /// </summary> 
        public DockPanel DockPanel
        {
            get { return this.dockPanel; }
        }

        /// <summary>
        /// Gets the menu strip
        /// </summary>
        public MenuStrip MenuStrip
        {
            get { return this.menuStrip; }
        }

        /// <summary>
        /// Gets the tool strip
        /// </summary>
        public ToolStrip ToolStrip
        {
            get { return this.toolStrip; }
        }

        /// <summary>
        /// Gets the tool strip panel
        /// </summary>
        public ToolStripPanel ToolStripPanel
        {
            get { return this.toolStripPanel; }
        }

        /// <summary>
        /// Gets the toolStripStatusLabel
        /// </summary>
        public ToolStripStatusLabel StatusLabel
        {
            get { return this.toolStripStatusLabel; }
        }

        /// <summary>
        /// Gets the toolStripProgressLabel
        /// </summary>
        public ToolStripStatusLabel ProgressLabel
        {
            get { return this.toolStripProgressLabel; }
        }

        /// <summary>
        /// Gets the toolStripProgressBar
        /// </summary>
        public ToolStripProgressBar ProgressBar
        {
            get { return this.toolStripProgressBar; }
        }

        /// <summary>
        /// Gets the TabMenu
        /// </summary>
        public ContextMenuStrip TabMenu
        {
            get { return this.tabMenu; }
        }

        /// <summary>
        /// Gets the StatusStrip
        /// </summary>
        public StatusStrip StatusStrip
        {
            get { return this.statusStrip; }
        }

        /// <summary>
        /// Gets the IgnoredKeys
        /// </summary>
        public List<Keys> IgnoredKeys
        {
            get { return ShortcutManager.AllShortcuts; }
        }

        /// <summary>
        /// Gets the Settings interface
        /// </summary>
        public ISettings Settings
        {
            get { return (ISettings)this.appSettings; }
        }

        /// <summary>
        /// Gets or sets the actual Settings
        /// </summary>
        public SettingObject AppSettings
        {
            get { return this.appSettings; }
            set { this.appSettings = value; }
        }

        /// <summary>
        /// Gets the CurrentDocument
        /// </summary>
        public ITabbedDocument CurrentDocument
        {
            get { return this.dockPanel.ActiveDocument as ITabbedDocument; }
        }

        /// <summary>
        /// Is FlashDevelop closing?
        /// </summary>
        public Boolean ClosingEntirely
        {
            get { return this.closingEntirely; }
        }

        /// <summary>
        /// Is this first MainForm instance?
        /// </summary>
        public Boolean IsFirstInstance
        {
            get
            {
                return MainForm.IsFirst;
            }
        }

        /// <summary>
        /// Is FlashDevelop in multi instance mode?
        /// </summary>
        public Boolean MultiInstanceMode
        {
            get
            {
                return Program.MultiInstanceMode;
            }
        }

        /// <summary>
        /// Is FlashDevelop in standalone mode?
        /// </summary>
        public Boolean StandaloneMode
        {
            get 
            {
                String file = Path.Combine(PathHelper.AppDir, ".local");
                return File.Exists(file); 
            }
        }

        /// <summary>
        /// Gets the all available documents
        /// </summary> 
        public ITabbedDocument[] Documents
        {
            get
            {
                List<ITabbedDocument> documents = new List<ITabbedDocument>();
                foreach (DockPane pane in Globals.MainForm.DockPanel.Panes)
                {
                    if (pane.DockState == DockState.Document)
                    {
                        foreach (IDockContent content in pane.Contents)
                        {
                            if (content is TabbedDocument)
                            {
                                documents.Add(content as TabbedDocument);
                            }
                        }
                    }
                }
                return documents.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the WorkingDirectory
        /// </summary>
        public String WorkingDirectory
        {
            get
            {
                if (!Directory.Exists(this.workingDirectory))
                {
                    this.workingDirectory = PathHelper.AppDir;
                }
                return this.workingDirectory;
            }
            set { this.workingDirectory = value; }
        }

        /// <summary>
        /// Gets or sets the ProcessIsRunning
        /// </summary>
        public Boolean ProcessIsRunning
        {
            get { return this.processRunner.IsRunning; }
        }

        /// <summary>
        /// Gets the panelIsActive
        /// </summary>
        public Boolean PanelIsActive
        {
            get { return this.panelIsActive; }
        }

        /// <summary>
        /// Gets the isFullScreen
        /// </summary>
        public Boolean IsFullScreen
        {
            get { return this.isFullScreen; }
        }

        /// <summary>
        /// Gets or sets the CloseAllCanceled
        /// </summary>
        public Boolean CloseAllCanceled
        {
            get { return this.closeAllCanceled; }
            set { this.closeAllCanceled = value; }
        }

        /// <summary>
        /// Gets or sets the ProcessingContents
        /// </summary>
        public Boolean ProcessingContents
        {
            get { return this.processingContents; }
            set { this.processingContents = value; }
        }

        /// <summary>
        /// Gets or sets the RestoringContents
        /// </summary>
        public Boolean RestoringContents
        {
            get { return this.restoringContents; }
            set { this.restoringContents = value; }
        }

        /// <summary>
        /// Gets or sets the RestartRequested
        /// </summary>
        public Boolean RestartRequested
        {
            get { return this.restartRequested; }
            set { this.restartRequested = value; }
        }

        /// <summary>
        /// Gets the application start args
        /// </summary>
        public String[] StartArguments
        {
            get { return MainForm.Arguments; }
        }

        /// <summary>
        /// Gets the application custom args
        /// </summary>
        public List<Argument> CustomArguments
        {
            get { return ArgumentDialog.CustomArguments; }
        }

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public new String ProductVersion
        {
            get { return Application.ProductVersion; }
        }

        /// <summary>
        /// Gets the full human readable version string
        /// </summary>
        public new String ProductName
        {
            get { return Application.ProductName; }
        }

        /// <summary>
        /// Gets the version of the operating system
        /// </summary>
        public Version OSVersion
        {
            get { return Environment.OSVersion.Version; }
        }

        #endregion

        #region Component Creation
       
        /// <summary>
        /// Creates a new custom document
        /// </summary>
        public DockContent CreateCustomDocument(Control ctrl)
        {
            try
            {
                TabbedDocument tabbedDocument = new TabbedDocument();
                tabbedDocument.Closing += new System.ComponentModel.CancelEventHandler(this.OnDocumentClosing);
                tabbedDocument.Closed += new System.EventHandler(this.OnDocumentClosed);
                tabbedDocument.Text = TextHelper.GetString("Title.CustomDocument");
                tabbedDocument.TabPageContextMenuStrip = this.tabMenu;
                tabbedDocument.Controls.Add(ctrl);
                tabbedDocument.Show();
                return tabbedDocument;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }


        /// <summary>
		/// Creates a floating panel for the plugin
		/// </summary>
        public DockContent CreateDockablePanel(Control ctrl, String guid, Image image, DockState defaultDockState)
        {
            try
            {
                DockablePanel dockablePanel = new DockablePanel(ctrl, guid);
                if (image != null) dockablePanel.Icon = ImageKonverter.ImageToIcon(image);
                dockablePanel.DockState = defaultDockState;
                LayoutManager.PluginPanels.Add(dockablePanel);
                return dockablePanel;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }


        #endregion

        #region Construct Components
       
        /// <summary>
        /// Initializes the graphics
        /// </summary>
        private void InitializeGraphics()
        {
            Icons.Initialize(this);
            this.Icon = new Icon(ResourceHelper.GetStream("RCP.ico"));
        }

        /// <summary>
        /// Initializes the UI rendering
        /// </summary>
        private void InitializeRendering()
        {
            if (Globals.Settings.RenderMode == UiRenderMode.System)
            {
                ToolStripManager.VisualStylesEnabled = true;
                ToolStripManager.RenderMode = ToolStripManagerRenderMode.System;
            }
            else if (Globals.Settings.RenderMode == UiRenderMode.Professional)
            {
                ToolStripManager.VisualStylesEnabled = false;
                ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;
            }
        }

        /// <summary>
        /// Initializes the application settings
        /// </summary>
        private void InitializeSettings()
        {
            this.appSettings = SettingObject.GetDefaultSettings();
            if (File.Exists(FileNameHelper.SettingData))
            {
                Object obj = ObjectSerializer.Deserialize(FileNameHelper.SettingData, this.appSettings, false);
                this.appSettings = (SettingObject)obj;
            }
            SettingObject.EnsureValidity(this.appSettings);
        }

        /// <summary>
        /// Set culture info and initialize the localization from .locale file
        /// </summary>
        private void InitializeLocalization()
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(this.appSettings.LocaleVersion.ToString().Replace('_', '-'));
                String filePath = Path.Combine(PathHelper.BaseDir, ".locale");
                if (File.Exists(filePath))
                {
                    String enumData = File.ReadAllText(filePath).Trim();
                    LocaleVersion localeVersion = (LocaleVersion)Enum.Parse(typeof(LocaleVersion), enumData);
                    this.appSettings.LocaleVersion = localeVersion;
                    File.Delete(filePath);
                }
            }
            catch {} // No errors...
        }

        /// <summary>
        /// Initializes the process runner
        /// </summary>
        public void InitializeProcessRunner()
        {
            this.processRunner = new ProcessRunner();
            this.processRunner.RedirectInput = true;
            this.processRunner.ProcessEnded += ProcessEnded;
            this.processRunner.Output += ProcessOutput;
            this.processRunner.Error += ProcessError;
        }

        /// <summary>
        /// Initializes the plugins, restores the layout and sets a fixed position
        /// </summary>
        public void InitializeMainForm()
        {
            try
            {
                this.formState = new FormState();
                Point position = this.appSettings.WindowPosition;
                if (position.X < -4 || position.Y < -4) this.Location = new Point(0, 0);
                else this.Location = position; // Set zero position if window is hidden
                String pluginDir = PathHelper.PluginDir; // Plugins of all users
                PropertyPad.Initialize();
                if (Directory.Exists(pluginDir)) PluginServices.FindPlugins(pluginDir);
                SortViewMenuItems();
                LayoutManager.BuildLayoutSystems(FileNameHelper.LayoutData);
                ShortcutManager.LoadCustomShortcuts();
                ArgumentDialog.LoadCustomArguments();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Initializes the form components
        /// </summary>
        private void InitializeComponents()
        {
            this.dockPanel = new DockPanel();
            this.statusStrip = new StatusStrip();
            this.toolStripPanel = new ToolStripPanel();
            this.toolStrip = StripBarManager.GetToolStrip(FileNameHelper.ToolBar);
            this.menuStrip = StripBarManager.GetMenuStrip(FileNameHelper.MainMenu);
            this.tabMenu = StripBarManager.GetContextMenu(FileNameHelper.TabMenu);
            this.toolStripStatusLabel = new ToolStripStatusLabel();
            this.toolStripProgressLabel = new ToolStripStatusLabel();
            this.toolStripProgressBar = new ToolStripProgressBar();
            this.SuspendLayout();
            //
            // toolStripPanel
            //
            this.toolStripPanel.Dock = DockStyle.Top;
            this.toolStripPanel.Controls.Add(this.toolStrip);
            this.toolStripPanel.Controls.Add(this.menuStrip);
            this.tabMenu.Font = Globals.Settings.DefaultFont;
            this.toolStrip.Font = Globals.Settings.DefaultFont;
            this.menuStrip.Font = Globals.Settings.DefaultFont;
            this.tabMenu.Renderer = new DockPanelStripRenderer(false);
            this.menuStrip.Renderer = new DockPanelStripRenderer(false);
            this.toolStrip.Renderer = new DockPanelStripRenderer(false);
            this.toolStrip.Padding = new Padding(0, 1, 0, 0);
            this.toolStrip.Size = new Size(500, 26);
            this.toolStrip.Stretch = true;
            // 
            // dockPanel
            //
            this.dockPanel.TabIndex = 2;
            this.dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            this.dockPanel.Dock = DockStyle.Fill;
            this.dockPanel.Name = "dockPanel";
            //
            // toolStripStatusLabel
            //
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.toolStripStatusLabel.Spring = true;
            //
            // toolStripProgressLabel
            //
            this.toolStripProgressLabel.AutoSize = true;
            this.toolStripProgressLabel.Name = "toolStripProgressLabel";
            this.toolStripProgressLabel.TextAlign = ContentAlignment.MiddleRight;
            this.toolStripProgressLabel.Visible = false;
            //
            // toolStripProgressBar
            //
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.ControlAlign = ContentAlignment.MiddleRight;
            this.toolStripProgressBar.ProgressBar.Width = 120;
            this.toolStripProgressBar.Visible = false;
            // 
            // statusStrip
            //
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Items.Add(this.toolStripStatusLabel);
            this.statusStrip.Items.Add(this.toolStripProgressLabel);
            this.statusStrip.Items.Add(this.toolStripProgressBar);
            this.statusStrip.Font = Globals.Settings.DefaultFont;
            this.statusStrip.Renderer = new DockPanelStripRenderer(false);
            this.statusStrip.Stretch = true;
            // 
            // MainForm
            //
            this.AllowDrop = true;
            this.Name = "MainForm";
            this.Text = ".NET RCP";
            this.Size = new Size(800, 600);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.toolStripPanel);
            this.Controls.Add(this.statusStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Size = this.appSettings.WindowSize;
            this.Font = this.appSettings.DefaultFont;
            this.StartPosition = FormStartPosition.Manual;
            this.Closing += new CancelEventHandler(this.OnMainFormClosing);
            this.Activated += new EventHandler(this.OnMainFormActivate);
            this.Shown += new EventHandler(this.OnMainFormShow);
            this.Load += new EventHandler(this.OnMainFormLoad);
            this.LocationChanged += new EventHandler(this.OnMainFormLocationChange);
            this.GotFocus += new EventHandler(this.OnMainFormGotFocus);
            this.Resize += new EventHandler(this.OnMainFormResize);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        #region Event Handlers

        /// <summary>
		/// Checks the file changes and activates
		/// </summary>
        private void OnMainFormActivate(Object sender, System.EventArgs e)
        {
            if (this.CurrentDocument == null) return;
            this.CurrentDocument.Activate(); // Activate the current document
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Checks the file changes when recieving focus
        /// </summary>
        private void OnMainFormGotFocus(Object sender, System.EventArgs e)
        {
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Initalizes the windows state after show is called and
        /// check if we need to notify user for recovery files
        /// </summary>
        private void OnMainFormShow(Object sender, System.EventArgs e)
        {
            this.WindowState = this.appSettings.WindowState;
        }

        /// <summary>
        /// Saves the window size as it's being resized
        /// </summary>
        private void OnMainFormResize(Object sender, System.EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized && this.WindowState != FormWindowState.Minimized)
            {
                this.appSettings.WindowSize = this.Size;
            }
        }

        /// <summary>
        /// Saves the window location as it's being moved
        /// </summary>
        private void OnMainFormLocationChange(Object sender, System.EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized && this.WindowState != FormWindowState.Minimized)
            {
                this.appSettings.WindowSize = this.Size;
                this.appSettings.WindowPosition = this.Location;
            }
        }

        /// <summary>
        /// Setups misc stuff when MainForm is loaded
        /// </summary>
        private void OnMainFormLoad(Object sender, System.EventArgs e)
        {
            // DockPanel events
            this.dockPanel.ActiveDocumentChanged += new EventHandler(this.OnActiveDocumentChanged);
            // Populate menus and check buttons 
            ButtonManager.UpdateFlaggedButtons();
            this.workingDirectory = this.appSettings.LatestDialogPath;
            // Open document[s] in startup 
            if (Arguments != null && Arguments.Length != 0)
            {
                this.ProcessParameters(Arguments);
                Arguments = null;
            }
            else if (this.appSettings.RestoreFileSession)
            {
                String file = FileNameHelper.SessionData;
                SessionManager.RestoreSession(file, SessionType.Startup);
            }
            if (this.Documents.Length == 0)
            {
                NotifyEvent ne = new NotifyEvent(EventType.FileEmpty);
                EventManager.DispatchEvent(this, ne);
            }
            // Load and apply current active theme
            String currentTheme = Path.Combine(PathHelper.ThemesDir, "CURRENT");
            if (File.Exists(currentTheme)) ThemeManager.LoadTheme(currentTheme);
            // Notify plugins that the application is ready
            EventManager.DispatchEvent(this, new NotifyEvent(EventType.UIStarted));
            // Apply all settings to all documents
            this.ApplyAllSettings();
            // Remove splash screen
            SplashScreenForm.SplashScreen.Dispose();
        }

        /// <summary>
        /// Save all settings when the MainForm is closing
        /// </summary>
        public void OnMainFormClosing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.closingEntirely = true;
            Session session = SessionManager.GetCurrentSession();
            NotifyEvent ne = new NotifyEvent(EventType.UIClosing);
            EventManager.DispatchEvent(this, ne);
            if (ne.Handled)
            {
                this.closingEntirely = false;
                e.Cancel = true;
            }
            if (!RestoringContents && !e.Cancel && Globals.Settings.ConfirmOnExit)
            {
                String title = TextHelper.GetString("Title.ConfirmDialog");
                String message = TextHelper.GetString("Info.AreYouSureToExit");
                DialogResult result = MessageBox.Show(Globals.MainForm, message, " " + title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) e.Cancel = true;
            }
            if (!e.Cancel) this.CloseAllDocuments(false);
            if (this.closeAllCanceled)
            {
                this.closeAllCanceled = false;
                this.closingEntirely = false;
                e.Cancel = true;
            }
            if (!e.Cancel && this.isFullScreen)
            {
                this.ToggleFullScreen(null, null);
            }
            if (!e.Cancel)
            {
                String file = FileNameHelper.SessionData;
                SessionManager.SaveSession(file, session);
                ShortcutManager.SaveCustomShortcuts();
                ArgumentDialog.SaveCustomArguments();
                PluginServices.DisposePlugins();
                this.SaveAllSettings();
            }
        }

        /// <summary>
        /// Handles the application shortcuts
        /// </summary>
        protected override Boolean ProcessCmdKey(ref Message msg, Keys keyData)
        {
            /**
            * Notify plugins. Don't notify ControlKey or ShiftKey as it polls a lot
            */
            KeyEvent ke = new KeyEvent(EventType.Keys, keyData);
            Keys keyCode = keyData & Keys.KeyCode;
            if ((keyCode != Keys.ControlKey) && (keyCode != Keys.ShiftKey))
            {
                EventManager.DispatchEvent(this, ke);
            }
            if (!ke.Handled)
            {
                /**
                * Process special key combinations and allow "chaining" of 
                * Ctrl-Tab commands if you keep holding control down.
                */
                if ((keyData & Keys.Control) != 0)
                {
                    Boolean sequentialTabbing = this.appSettings.SequentialTabbing;
                    if ((keyData == (Keys.Control | Keys.Next)) || (keyData == (Keys.Control | Keys.Tab)))
                    {
                        TabbingManager.TabTimer.Enabled = true;
                        if (keyData == (Keys.Control | Keys.Next) || sequentialTabbing)
                        {
                            TabbingManager.NavigateTabsSequentially(1);
                        }
                        else TabbingManager.NavigateTabHistory(1);
                        return true;
                    }
                    if ((keyData == (Keys.Control | Keys.Prior)) || (keyData == (Keys.Control | Keys.Shift | Keys.Tab)))
                    {
                        TabbingManager.TabTimer.Enabled = true;
                        if (keyData == (Keys.Control | Keys.Prior) || sequentialTabbing)
                        {
                            TabbingManager.NavigateTabsSequentially(-1);
                        }
                        else TabbingManager.NavigateTabHistory(-1);
                        return true;
                    }
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }
            return true;
        }

        /// <summary>
        /// Checks if last document is closing. 
        /// </summary>
        public void OnDocumentClosing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Documents.Length == 1 && !e.Cancel && !this.closingEntirely && !this.restoringContents)
            {
                NotifyEvent ne = new NotifyEvent(EventType.FileEmpty);
                EventManager.DispatchEvent(this, ne);
            }
        }

        /// <summary>
        /// Activates the previous document when document is closed
        /// </summary>
        public void OnDocumentClosed(Object sender, System.EventArgs e)
        {
            ITabbedDocument document = sender as ITabbedDocument;
            TabbingManager.TabHistory.Remove(document);
            if (this.appSettings.SequentialTabbing)
            {
                if (TabbingManager.SequentialIndex == 0) this.Documents[0].Activate();
                else TabbingManager.NavigateTabsSequentially(-1);
            }
            else TabbingManager.NavigateTabHistory(0);
            ButtonManager.UpdateFlaggedButtons();
        }


        /// <summary>
        /// Refreshes the statusbar display and updates the important edit buttons
        /// </summary>
        public void OnUpdateControl() //OnScintillaControlUpdateControl
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.OnUpdateControl(); });
                return;
            }
            
            this.toolStripStatusLabel.Text = " ";
            ButtonManager.UpdateFlaggedButtons();
            NotifyEvent ne = new NotifyEvent(EventType.UIRefresh);
            EventManager.DispatchEvent(this, ne);
        }

        #endregion

        #region General Methods
        /// <summary>
        /// Finds the specified plugin
        /// </summary>
        public IPlugin FindPlugin(String guid)
        {
            AvailablePlugin plugin = PluginServices.Find(guid);
            return plugin.Instance;
        }

        /// <summary>
        /// Themes the controls from the parent
        /// </summary>
        public void ThemeControls(Object parent)
        {
            ThemeManager.WalkControls(parent);
        }

        /// <summary>
        /// Gets a theme property color
        /// </summary>
        public Color GetThemeColor(String id)
        {
            return ThemeManager.GetThemeColor(id);
        }

        /// <summary>
        /// Gets a theme property value
        /// </summary>
        public String GetThemeValue(String id)
        {
            return ThemeManager.GetThemeValue(id);
        }

        /// <summary>
        /// Finds the specified menu item by name
        /// </summary>
        public ToolStripItem FindMenuItem(String name)
        {
            return StripBarManager.FindMenuItem(name);
        }

        /// <summary>
        /// Finds the menu items that have the specified name
        /// </summary>
        public List<ToolStripItem> FindMenuItems(String name)
        {
            return StripBarManager.FindMenuItems(name);
        }


        /// <summary>
        /// Registers a new menu item with the shortcut manager
        /// </summary>
        public void RegisterShortcutItem(String id, ToolStripMenuItem item)
        {
            ShortcutManager.RegisterItem(id, item);
        }

        /// <summary>
        /// Finds the specified composed/ready image
        /// </summary>
        public Image FindImage(String data)
        {
            try
            {
                lock (this)
                {
                    if (data.StartsWith("Icons."))
                        return Icons.RetrieveFDImage(data);
                    else
                        return ImageManager.GetComposedBitmap(data);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Shows the settings dialog
        /// </summary>
        public void ShowSettingsDialog(String itemName)
        {
            SettingDialog.Show(itemName, "");
        }
        public void ShowSettingsDialog(String itemName, String filter)
        {
            SettingDialog.Show(itemName, filter);
        }
        /// <summary>
        /// Shows the error dialog if the sender is ErrorManager
        /// </summary>
        public void ShowErrorDialog(Object sender, Exception ex)
        {
            if (sender.GetType().ToString() != "PluginCore.Managers.ErrorManager")
            {
                String message = TextHelper.GetString("Info.OnlyErrorManager");
                ErrorDialog.Show(new Exception(message));
            }
            else ErrorDialog.Show(ex);
        }

        /// <summary>
        /// Refreshes the main form
        /// </summary>
        public void RefreshUI()
        {
            this.OnUpdateControl();
        }
        /// <summary>
        /// Processes the argument string variables
        /// </summary>
        public String ProcessArgString(String args)
        {
            return ArgsProcessor.ProcessString(args, true);
        }

        /// <summary>
        /// Processes the incoming arguments 
        /// </summary> 
        public void ProcessParameters(String[] args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.ProcessParameters(args); });
                return;
            }
            this.Activate(); this.Focus();
            if (args != null && args.Length != 0)
            {
                for (Int32 i = 0; i < args.Length; i++)
                {
                    if (ArgsProcessor.reURL.IsMatch(args[i])) this.BrowseWebsite(args[i]);
                }
            }
            Win32.RestoreWindow(this.Handle);
        }

        /// <summary>
		/// Closes all open documents with an option: exceptCurrent
		/// </summary>
        public void CloseAllDocuments(Boolean exceptCurrent)
        {
            Int32 closeIndex = 0;
            Int32 documentCount = this.Documents.Length;
            ITabbedDocument current = this.CurrentDocument;
            this.closeAllCanceled = false; this.closingAll = true;
            for (Int32 i = 0; i < documentCount; i++)
            {
                ITabbedDocument document = this.Documents[closeIndex];
                if (exceptCurrent)
                {
                    if (document != current) document.Close();
                    else closeIndex = 1;
                }
                else document.Close();
            }
            this.closingAll = false;
        }

        /// <summary>
        /// Updates all needed settings after modification
        /// </summary>
        public void ApplyAllSettings()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.ApplyAllSettings(); });
                return;
            }
            ShortcutManager.ApplyAllShortcuts();
            EventManager.DispatchEvent(this, new NotifyEvent(EventType.ApplySettings));

            this.statusStrip.Visible = this.appSettings.ViewStatusBar;
            this.toolStrip.Visible = this.isFullScreen ? false : this.appSettings.ViewToolBar;
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Saves all settings of the FlashDevelop
        /// </summary>
        public void SaveAllSettings()
        {
            try
            {
                this.appSettings.WindowState = this.WindowState;
                this.appSettings.LatestDialogPath = this.workingDirectory;
                if (this.WindowState != FormWindowState.Maximized && this.WindowState != FormWindowState.Minimized)
                {
                    this.appSettings.WindowSize = this.Size;
                    this.appSettings.WindowPosition = this.Location;
                }
                if (!File.Exists(FileNameHelper.SettingData))
                {
                    String folder = Path.GetDirectoryName(FileNameHelper.SettingData);
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                }
                ObjectSerializer.Serialize(FileNameHelper.SettingData, this.appSettings);
                try {
                    if (!RestoringContents) this.dockPanel.SaveAsXml(FileNameHelper.LayoutData);
                }
                catch (Exception ex2)
                {
                    // Ignore errors on multi instance full close...
                    if (this.MultiInstanceMode && this.ClosingEntirely) return;
                    else throw ex2;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        #endregion

        #region Click Handlers


        /// <summary>
        /// Restores the specified panel layout
        /// </summary>
        public void RestoreLayout(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String file = ((ItemData)button.Tag).Tag;
                TextEvent te = new TextEvent(EventType.RestoreLayout, file);
                EventManager.DispatchEvent(Globals.MainForm, te);
                if (!te.Handled)
                {
                    File.Copy(file, FileNameHelper.LayoutData, true);
                    RestoringContents = true;
                    this.restartRequested = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Closes the current document
        /// </summary>
        public void Close(Object sender, System.EventArgs e)
        {
            this.CurrentDocument.Close();
        }

        /// <summary>
        /// Closes all open documents
        /// </summary>
        public void CloseAll(Object sender, System.EventArgs e)
        {
            this.CloseAllDocuments(false);
        }

        /// <summary>
        /// Closes all open documents exept the current
        /// </summary>
        public void CloseOthers(Object sender, System.EventArgs e)
        {
            this.CloseAllDocuments(true);
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        public void Exit(Object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Opens website in default browser
        /// </summary>
        /// 
        public void OpenDefaultBrowser(Object sender, System.EventArgs e)
        {
            String url = MainForm.FindUrl(this.CurrentDocument as TabbedDocument);
            if (!String.IsNullOrEmpty(url)) ProcessHelper.StartAsync(url);
        }

        /// <summary>
        /// Save full website in mht file
        /// </summary>
        public void SaveWebsiteAs(Object sender, System.EventArgs e)
        {
            TabbedDocument document = this.CurrentDocument as TabbedDocument;
            Control ctrl = document.Controls[0];
            if (ctrl is Browser) (ctrl as Browser).webBrowser.ShowSaveAsDialog();
        }

        /// <summary>
        /// Duplicates the current document
        /// </summary>
        public void Duplicate(Object sender, System.EventArgs e)
        {
            TabbedDocument document = this.CurrentDocument as TabbedDocument;
            if (document.IsBrowsable)
            {
                string url = FindUrl(document);
                if (!String.IsNullOrEmpty(url))
                    Globals.MainForm.BrowseWebsite(url);

            }
        }

        /// <summary>
        /// Retrieves the url of the browser document
        /// </summary>
        public static string FindUrl(TabbedDocument document)
        {
            string url = String.Empty;
            Control ctrl = document.Controls[0];
            if (ctrl is Browser)
            {
                if ((ctrl as Browser).webBrowser.Url == null) return null;
                url = (ctrl as Browser).webBrowser.Url.ToString();
            }
            return url;
        }

        /// <summary>
        /// Opens the edit shortcut dialog
        /// </summary>
        public void EditShortcuts(Object sender, System.EventArgs e)
        {
            ShortcutDialog.Show();
        }

        /// <summary>
        /// Opens the about dialog
        /// </summary>
        public void About(Object sender, System.EventArgs e)
        {
            AboutDialog.Show();
        }

        /// <summary>
        /// Opens the settings dialog
        /// </summary>
        public void ShowSettings(Object sender, System.EventArgs e)
        {
            SettingDialog.Show("FlashDevelop", "");
        }

        /// <summary>
        /// Shows the application in fullscreen or normal mode
        /// </summary>
        public void ToggleFullScreen(Object sender, System.EventArgs e)
        {
            if (this.isFullScreen)
            {
                this.formState.Restore(this);
                if (this.appSettings.ViewToolBar) this.toolStrip.Visible = true;
                foreach (DockPane pane in this.dockPanel.Panes)
                {
                    if (this.fullScreenDocks[pane] != null)
                    {
                        pane.DockState = (DockState)this.fullScreenDocks[pane];
                    }
                }
                this.isFullScreen = false;
            } 
            else 
            {
                this.formState.Maximize(this);
                this.toolStrip.Visible = false;
                this.fullScreenDocks = new Hashtable();
                foreach (DockPane pane in this.dockPanel.Panes)
                {
                    this.fullScreenDocks[pane] = pane.DockState;
                    switch (pane.DockState)
                    {
                        case DockState.DockLeft:
                            pane.DockState = DockState.DockLeftAutoHide;
                            break;
                        case DockState.DockRight:
                            pane.DockState = DockState.DockRightAutoHide;
                            break;
                        case DockState.DockBottom:
                            pane.DockState = DockState.DockBottomAutoHide;
                            break;
                        case DockState.DockTop:
                            pane.DockState = DockState.DockTopAutoHide;
                            break;
                    }
                }
                this.isFullScreen = true;
            }
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Opens the browser from the menu
        /// </summary>
        public void Browse(Object sender, System.EventArgs e)
        {
            String url = "";
            if (sender != null)
            {
                ToolStripItem button = (ToolStripItem)sender;
                url = this.ProcessArgString(((ItemData)button.Tag).Tag);
            }
            BrowseWebsite(url);
        }

        /// <summary>
        /// Opens the browser with the specified url
        /// </summary>
        public void BrowseWebsite(String url)
        {
            Browser browser = new Browser();
            browser.Dock = DockStyle.Fill;
            DockContent document = MainForm.Instance.CreateCustomDocument(browser);
            (document as TabbedDocument).FileName = url;
            if (url.Trim() != "") browser.WebBrowser.Navigate(url);
            else browser.WebBrowser.GoHome();
        }

        /// <summary>
        /// Opens the arguments dialog
        /// </summary>
        public void ShowArguments(Object sender, System.EventArgs e)
        {
            ArgumentDialog.Show();
        }

        /// <summary>
        /// Lets user browse for an theme file
        /// </summary>
        public void SelectTheme(Object sender, System.EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = PathHelper.ThemesDir;
            ofd.Title = " " + TextHelper.GetString("Title.OpenFileDialog");
            ofd.Filter = TextHelper.GetString("Info.ThemesFilter");
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                String ext = Path.GetExtension(ofd.FileName).ToLower();
                if (ext == ".fdi") ThemeManager.LoadTheme(ofd.FileName);
            }
        }
        /// <summary>
        /// Invokes the specified registered menu item
        /// </summary>
        public void InvokeMenuItem(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String registeredItem = ((ItemData)button.Tag).Tag;
                ShortcutItem item = ShortcutManager.GetRegisteredItem(registeredItem);
                if (item.Item != null) item.Item.PerformClick();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Calls a custom plugin command
        /// </summary>
        public void PluginCommand(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String[] args = ((ItemData)button.Tag).Tag.Split(';');
                String action = args[0]; // Action of the command
                String data = (args.Length > 1) ? args[1] : null;
                DataEvent de = new DataEvent(EventType.Command, action, data);
                EventManager.DispatchEvent(this, de);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Calls a normal MainForm method
        /// </summary>
        public Boolean CallCommand(String name, String tag)
        {
            try
            {
                Type mfType = this.GetType();
                System.Reflection.MethodInfo method = mfType.GetMethod(name);
                if (method == null) throw new MethodAccessException();
                ToolStripMenuItem button = new ToolStripMenuItem();
                button.Tag = new ItemData(null, tag, null); // Tag is used for args
                Object[] parameters = new Object[2];
                parameters[0] = button; parameters[1] = null;
                method.Invoke(this, parameters);
                return true;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return false;
            }
        }

        public void OnActiveDocumentChanged(Object sender, System.EventArgs e)
        {
            try
            {
                if (this.CurrentDocument == null) return;
                ((TabbedDocument)this.CurrentDocument).Activate();
                this.OnUpdateControl();
                /**
                * Bring this newly active document to the top of the tab history
                * unless you're currently cycling through tabs with the keyboard
                */
                TabbingManager.UpdateSequentialIndex(this.CurrentDocument);
                if (!TabbingManager.TabTimer.Enabled)
                {
                    TabbingManager.TabHistory.Remove(this.CurrentDocument);
                    TabbingManager.TabHistory.Insert(0, this.CurrentDocument);
                }

                NotifyEvent ne = new NotifyEvent(EventType.FileSwitch);
                EventManager.DispatchEvent(this, ne);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Runs a simple process
        /// </summary>
        public void RunProcess(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String args = this.ProcessArgString(((ItemData)button.Tag).Tag);
                Int32 position = args.IndexOf(';'); // Position of the arguments
                NotifyEvent ne = new NotifyEvent(EventType.ProcessStart);
                EventManager.DispatchEvent(this, ne);
                if (position > -1)
                {
                    String message = TextHelper.GetString("Info.RunningProcess");
                    TraceManager.Add(message + " " + args.Substring(0, position) + " " + args.Substring(position + 1), (Int32)TraceType.ProcessStart);
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.WorkingDirectory = this.WorkingDirectory;
                    psi.Arguments = args.Substring(position + 1);
                    psi.FileName = args.Substring(0, position);
                    ProcessHelper.StartAsync(psi);
                }
                else
                {
                    String message = TextHelper.GetString("Info.RunningProcess");
                    TraceManager.Add(message + " " + args, (Int32)TraceType.ProcessStart);
                    if (args.ToLower().EndsWith(".bat"))
                    {
                        Process bp = new Process();
                        bp.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        bp.StartInfo.FileName = @args;
                        bp.Start();
                    }
                    else
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(args);
                        psi.WorkingDirectory = this.WorkingDirectory;
                        ProcessHelper.StartAsync(psi);
                    }
                }
                ButtonManager.UpdateFlaggedButtons();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Runs a process and tracks its progress
        /// </summary>
        public void RunProcessCaptured(Object sender, System.EventArgs e)
        {
            try
            {
                if (this.processRunner.IsRunning)
                {
                    String message = TextHelper.GetString("Info.ProcessAlreadyRunning");
                    TraceManager.Add(message, (Int32)TraceType.Error);
                    return;
                }
                ToolStripItem button = (ToolStripItem)sender;
                String args = this.ProcessArgString(((ItemData)button.Tag).Tag);
                Int32 position = args.IndexOf(';'); // Position of the arguments
                NotifyEvent ne = new NotifyEvent(EventType.ProcessStart);
                EventManager.DispatchEvent(this, ne);
                String message2 = TextHelper.GetString("Info.RunningProcess");
                TraceManager.Add(message2 + " " + args.Substring(0, position) + " " + args.Substring(position + 1), (Int32)TraceType.ProcessStart);
                this.processRunner.Run(args.Substring(0, position), args.Substring(position + 1));
                ButtonManager.UpdateFlaggedButtons();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Handles the incoming info output
        /// </summary>
        private void ProcessOutput(Object sender, String line)
        {
            TraceManager.AddAsync(line, (Int32)TraceType.Info);
        }

        /// <summary>
        /// Handles the incoming error output
        /// </summary> 
        private void ProcessError(Object sender, String line)
        {
            TraceManager.AddAsync(line, (Int32)TraceType.ProcessError);
        }

        /// <summary>
        /// Handles the ending of a process
        /// </summary>
        private void ProcessEnded(Object sender, Int32 exitCode)
        {
            if (this.InvokeRequired) this.BeginInvoke((MethodInvoker)delegate { this.ProcessEnded(sender, exitCode); });
            else
            {
                String result = String.Format("Done({0})", exitCode);
                TraceManager.Add(result, (Int32)TraceType.ProcessEnd);
                TextEvent te = new TextEvent(EventType.ProcessEnd, result);
                EventManager.DispatchEvent(this, te);
                ButtonManager.UpdateFlaggedButtons();
            }
        }

        /// <summary>
        /// Stop the currently running process
        /// </summary>
        public void KillProcess(Object sender, System.EventArgs e)
        {
            if (this.processRunner.IsRunning)
            {
                this.processRunner.KillProcess();
            }
        }

        /// <summary>
        /// Executes the specified C# Script file
        /// </summary>
        public void ExecuteScript(Object sender, EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            String file = this.ProcessArgString(((ItemData)button.Tag).Tag);
            try
            {
                Host host = new Host();
                String[] args = file.Split(new Char[1] { ';' });
                if (args[1] == String.Empty) return;
                if (args[0] == "Internal") host.ExecuteScriptInternal(args[1], false);
                else if (args[0] == "Development") host.ExecuteScriptInternal(args[1], true);
                else host.ExecuteScriptExternal(file);
            }
            catch (Exception ex)
            {
                String message = TextHelper.GetString("Info.CouldNotExecuteScript");
                ErrorManager.ShowWarning(message + "\r\n" + ex.Message, null);
            }
        }

        /// <summary>
        /// Outputs the supplied argument string
        /// </summary>
        public void Debug(Object sender, EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            String args = this.ProcessArgString(((ItemData)button.Tag).Tag);
            if (args == String.Empty) ErrorManager.ShowError(new Exception("Debug"));
            else ErrorManager.ShowInfo(args);
        }

        /// <summary>
        /// Restarts FlashDevelop
        /// </summary>
        public void Restart(Object sender, EventArgs e)
        {
            this.restartRequested = true;
            this.Close();
        }

        #endregion

        #region Sort Menu
        public void SortViewMenuItems()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            ArrayList listMenus = new ArrayList(viewMenu.DropDownItems);
            listMenus.Sort(2, viewMenu.DropDownItems.Count - 2, new MenuComparer());
            viewMenu.DropDownItems.Clear();
            foreach (ToolStripItem item in listMenus)
            {
                viewMenu.DropDownItems.Add(item);
            }
        }

        public class MenuComparer : IComparer
        {
            /// <summary>
            /// Compares the menus and returns the comparison index
            /// </summary>
            public int Compare(ToolStripMenuItem menu1, ToolStripMenuItem menu2)
            {
                if (menu1.Tag == null && menu2.Tag == null)
                    return menu1.Text.CompareTo(menu2.Text);
                else if (menu1.Tag == null && menu2.Tag != null)
                    return 1;
                else if (menu2.Tag == null && menu1.Tag != null)
                    return -1;
                else
                    return Convert.ToInt32(menu1.Tag).CompareTo(Convert.ToInt32(menu2.Tag));
            }

            public int Compare(object menu1, object menu2)
            {
                return Compare(menu1 as ToolStripMenuItem, menu2 as ToolStripMenuItem);
            }
        }
        #endregion

        #region Script Host

        public class Host : MarshalByRefObject
        {
            /// <summary>
            /// Executes the script in a seperate appdomain and then unloads it
            /// NOTE: This is more suitable for one pass processes
            /// </summary>
            public void ExecuteScriptExternal(String script)
            {
                if (!File.Exists(script)) throw new FileNotFoundException();
                using (AsmHelper helper = new AsmHelper(CSScript.Compile(script, null, true), null, true))
                {
                    helper.Invoke("*.Execute");
                }
            }

            /// <summary>
            /// Executes the script and adds it to the current app domain
            /// NOTE: This locks the assembly script file
            /// </summary>
            public void ExecuteScriptInternal(String script, Boolean random)
            {
                if (!File.Exists(script)) throw new FileNotFoundException();
                String file = random ? Path.GetTempFileName() : null;
                AsmHelper helper = new AsmHelper(CSScript.Load(script, file, false, null));
                helper.Invoke("*.Execute");
            }

        }

        #endregion
    }


}
