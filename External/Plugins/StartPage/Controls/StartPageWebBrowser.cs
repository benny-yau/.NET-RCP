using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;

namespace StartPage.Controls
{
    public class StartPageWebBrowser : UserControl
    {
        private String rssUrl = String.Empty;
        private String pageUrl = String.Empty;
        private Boolean showingStartPage = false;
        private StartPageActions startPageActions;
        private WebBrowserEx webBrowser;
        private DragDropPanel dndPanel;

        public StartPageWebBrowser(String pageUrl, String rssUrl)
        {
            this.rssUrl = rssUrl;
            this.pageUrl = pageUrl;
            this.InitializeDragDrop();
            this.InitializeComponent();
            this.startPageActions = new StartPageActions();
            this.webBrowser.ObjectForScripting = this.startPageActions;
            this.startPageActions.DocumentCompleted += new EventHandler(WebBrowserDocumentCompleted);
            this.ShowStartPage();
        }
        
        #region Component Designer Generated Code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.webBrowser = new StartPage.Controls.WebBrowserEx();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(20, 20);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowserNavigating);
            this.webBrowser.NewWindow += new System.ComponentModel.CancelEventHandler(this.WebBrowserNewWindow);
            // 
            // StartPageWebBrowser
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.webBrowser);
            this.Name = "StartPageWebBrowser";
            this.Size = new System.Drawing.Size(0, 0);
            this.ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the drag and drop operation.
        /// </summary>
        private void InitializeDragDrop()
        {
            this.dndPanel = new DragDropPanel();
            this.Controls.Add(this.dndPanel);
        }

        /// <summary>
        /// Shows the start page
        /// </summary>
        public void ShowStartPage()
        {
            this.showingStartPage = true;
            this.webBrowser.Navigate(this.pageUrl);
        }
        
        /// <summary>
        /// Updates the rss feed
        /// </summary>
        private void WebBrowserDocumentCompleted(Object sender, EventArgs e)
        {
            this.webBrowser.Document.InvokeScript("handleXmlData", new String[] { this.rssUrl});
        }

        /// <summary>
        /// If the page tries to open a new window use a fd tab instead
        /// </summary>
        private void WebBrowserNewWindow(Object sender, CancelEventArgs e)
        {
            this.startPageActions.ShowURL(this.webBrowser.StatusText);
            e.Cancel = true;
        }

        /// <summary>
        /// If we're not about to show the start page and it isn't a javascript call then open a new fd tab
        /// </summary>
        private void WebBrowserNavigating(Object sender, WebBrowserNavigatingEventArgs e)
        {
            if (this.IsDownloadable(e.Url.ToString())) return;
            if (!this.showingStartPage && !e.Url.ToString().StartsWith("javascript"))
            {
                this.startPageActions.ShowURL(e.Url.ToString());
                e.Cancel = true;
            }
            this.showingStartPage = false;
        }

        /// <summary>
        /// Checks if the url is downloadable file
        /// </summary>
        private Boolean IsDownloadable(String url)
        {
            if (url.EndsWith(".exe") || url.EndsWith(".zip")) return true;
            return false;
        }

        #endregion

        #region Start Page Actions

        [ComVisible(true)]
        public class StartPageActions
        {
            public event EventHandler DocumentCompleted;

            private static String HOME_URL = "http://www.flashdevelop.org/";
            private static String RELEASE_NOTES_URL = Path.Combine(PathHelper.DocDir, "index.html");
            private static String DOCUMENTATION_URL = "http://www.flashdevelop.org/wikidocs/";

            public void PageReady()
            {
                if (DocumentCompleted != null) DocumentCompleted(this, null);
            }

            /// <summary>
            /// Called from webpage to browse any url in seperate browser control
            /// </summary>
            public void ShowURL(String url)
            {
                PluginBase.MainForm.CallCommand("Browse", url);
            }

            /// <summary>
            /// Called from webpage to browse FlashDevelop Homepage
            /// </summary>
            public void ShowHome()
            {
                this.ShowURL(HOME_URL);
            }

            /// <summary>
            /// Called from webpage to browse release notes
            /// </summary>
            public void ShowReleaseNotes()
            {
                this.ShowURL(RELEASE_NOTES_URL);
            }

            /// <summary>
            /// Called from webpage to browse documentation
            /// </summary>
            public void ShowDocumentation()
            {
                this.ShowURL(DOCUMENTATION_URL);
            }

            /// <summary>
            /// Called from webpage to show FlashDevelop About dialog
            /// </summary>
            public void ShowAbout()
            {
                PluginBase.MainForm.CallCommand("About", null);
            }

        }

        #endregion

    }

    #region WebBrowserEx

    class WebBrowserEx : WebBrowser
    {
        /// <summary>
        /// Redirect events to MainForm.
        /// </summary>
        public override Boolean PreProcessMessage(ref Message msg)
        {
            return ((Form)PluginBase.MainForm).PreProcessMessage(ref msg);
        }

    }

    #endregion

}
