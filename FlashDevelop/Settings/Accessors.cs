using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Serialization;
using PluginCore;
using PluginCore.Localization;

namespace FlashDevelop.Settings
{
    public partial class SettingObject : ISettings
    {

        #region Application Mode
        [DisplayName("Stand Alone Mode")]
        [Category("Application Mode")]
        [Description("Configured for single user or multiple users, defined by the ¡§.local¡¨ file in the application directory")]
        public Boolean StandAloneMode
        {
            get { return Globals.MainForm.StandaloneMode; }
        }

        [DisplayName("Multi Instance Mode")]
        [Category("Application Mode")]
        [Description("Configured for single instance or multiple instances of the application, defined by the ¡§.multi¡¨ file in the application directory")]
        public Boolean MultiInstanceMode
        {
            get { return Globals.MainForm.MultiInstanceMode; }
        }

        #endregion

        #region Display

        [DefaultValue(true)]
        [DisplayName("View ToolBar")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewToolBar")]
        public Boolean ViewToolBar
        {
            get { return this.viewToolBar; }
            set { this.viewToolBar = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View StatusBar")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewStatusBar")]
        public Boolean ViewStatusBar
        {
            get { return this.viewStatusBar; }
            set { this.viewStatusBar = value; }
        }
        [DefaultValue(false)]
        [DisplayName("Use System UI Colors")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.UseSystemColors")]
        public Boolean UseSystemColors
        {
            get { return this.useSystemColors; }
            set { this.useSystemColors = value; }
        }
        [DisplayName("UI Render Mode")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.RenderMode")]
        [DefaultValue(UiRenderMode.Professional)]
        public UiRenderMode RenderMode
        {
            get { return this.uiRenderMode; }
            set { this.uiRenderMode = value; }
        }
        
        [XmlIgnore]
        [DisplayName("UI Console Font")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ConsoleFont")]
        [DefaultValue(typeof(Font), "Courier New, 8.75pt")]
        public Font ConsoleFont
        {
            get { return this.consoleFont; }
            set { this.consoleFont = value; }
        }

        [XmlIgnore]
        [DisplayName("UI Default Font")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.DefaultFont")]
        [DefaultValue(typeof(Font), "Tahoma, 8.25pt")]
        public Font DefaultFont
        {
            get { return this.defaultFont; }
            set { this.defaultFont = value; }
        }
        #endregion

        #region Locale

        [DisplayName("Selected Locale")]
        [DefaultValue(LocaleVersion.en_US)]
        [LocalizedCategory("FlashDevelop.Category.Locale")]
        [LocalizedDescription("FlashDevelop.Description.LocaleVersion")]
        public LocaleVersion LocaleVersion
        {
            get { 
                return this.localeVersion; 
            }
            set { 
                this.localeVersion = value;
            }
        }

        #endregion
        
        #region Features
        [DefaultValue(false)]
        [DisplayName("Use Sequential Tabbing")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.SequentialTabbing")]
        public Boolean SequentialTabbing
        {
            get { return this.sequentialTabbing; }
            set { this.sequentialTabbing = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Restore File Session")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.RestoreFileSession")]
        public Boolean RestoreFileSession
        {
            get { return this.restoreFileSession; }
            set { this.restoreFileSession = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Confirm On Exit")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.ConfirmOnExit")]
        public Boolean ConfirmOnExit
        {
            get { return this.confirmOnExit; }
            set { this.confirmOnExit = value; }
        }


        [DisplayName("File Types To Open"), Description("File types to open as documents in the application (eg .htm, .html, .txt). Other file types will be opened in their default applications.")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<String> FileTypesToOpen
        {
            get { return this.fileTypes; }
            set { this.fileTypes = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Save Last Visited Urls"), Description("Save last visited urls from the browser in the application")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        public Boolean SaveLastVisitedUrls
        {
            get { return this.saveLastVisitedUrls; }
            set { this.saveLastVisitedUrls = value; }
        }
        #endregion

        #region State
        [DisplayName("Last Active Path")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.LatestDialogPath")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public String LatestDialogPath
        {
            get { return this.latestDialogPath; }
            set {
                MainForm.Instance.WorkingDirectory = value;
                this.latestDialogPath = value; 
            }
        }

        [DisplayName("Window Size")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowSize")]
        [ReadOnly(true)]
        public Size WindowSize
        {
            get { return this.windowSize; }
            set { this.windowSize = value; }
        }

        [DisplayName("Window State")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowState")]
        [ReadOnly(true)]
        public FormWindowState WindowState
        {
            get { return this.windowState; }
            set { this.windowState = value; }
        }

        [DisplayName("Window Position")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowPosition")]
        [ReadOnly(true)]
        public Point WindowPosition
        {
            get { return this.windowPosition; }
            set { this.windowPosition = value; }
        }

       

        [DisplayName("Disabled Plugins")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.DisabledPlugins")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<String> DisabledPlugins
        {
            get { return this.disabledPlugins; }
            set { this.disabledPlugins = value; }
        }


        [DisplayName("Last Visited Urls"), Description("List of last visited urls")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<String> LastVisitedUrls
        {
            get { return this.lastVisitedUrls; }
            set { this.lastVisitedUrls = value; }
        }
        #endregion

    }

}
