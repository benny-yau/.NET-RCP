using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;

namespace FlashDevelop.Settings
{
    [Serializable]
    [DefaultProperty("AutoFilterList")]
    public partial class SettingObject : ISettings
    {
        #region Display
        private Boolean viewToolBar = true;
        private Boolean viewStatusBar = true;
        private Boolean useSystemColors = false;
        private UiRenderMode uiRenderMode = UiRenderMode.Professional;
        private Font consoleFont = new Font("Courier New", 8.75F);
        private Font defaultFont = new Font("Tahoma", 8.25F);
        #endregion

        #region Locale
        private LocaleVersion localeVersion = LocaleVersion.en_US;
        #endregion

        #region Features
        private Boolean sequentialTabbing = false;
        private Boolean restoreFileSession = true;
        private Boolean confirmOnExit = false;
        private List<String> fileTypes = new List<String>();
        private Boolean saveLastVisitedUrls = false;
        #endregion

        #region State
        private String latestDialogPath = Application.StartupPath;
        private FormWindowState windowState = FormWindowState.Maximized;
        private Point windowPosition = new Point(100, 100);
        private Size windowSize = new Size(800, 600);
        private List<String> disabledPlugins = new List<String>();
        private List<String> lastVisitedUrls = new List<String>();
        #endregion

    }

}
