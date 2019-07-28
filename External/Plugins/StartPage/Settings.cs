using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace StartPage
{
    [Serializable]
    public class Settings
    {
        private Boolean showStartPageInsteadOfUntitled = false;
        private ShowStartPageOnStartupEnum showStartPageOnStartup = ShowStartPageOnStartupEnum.NotRestoringSession;
        private Boolean useCustomStartPage = false;
        private Boolean useCustomRssFeed = false;
        private String customStartPage = "";
        private String customRssFeed = "";

        #region Common Settings

        /// <summary>
        /// Gets and sets showStartPageInsteadOfUntitled
        /// </summary>
        [DisplayName("Show Start Page Instead Of Nothing")]
        [LocalizedDescription("StartPage.Description.ShowStartPageInsteadOfUntitled")]
        [LocalizedCategory("StartPage.Category.Common")]
        [DefaultValue(false)]
        public Boolean ShowStartPageInsteadOfUntitled
        {
            get { return showStartPageInsteadOfUntitled; }
            set { this.showStartPageInsteadOfUntitled = value; }
        }

        /// <summary>
        /// Gets and sets showStartPageOnStartup
        /// </summary>
        [DisplayName("Show Start Page On Startup")]
        [LocalizedCategory("StartPage.Category.Common")]
        [LocalizedDescription("StartPage.Description.ShowStartPageOnStartup")]
        [DefaultValue(ShowStartPageOnStartupEnum.NotRestoringSession)]
        public ShowStartPageOnStartupEnum ShowStartPageOnStartup
        {
            get { return showStartPageOnStartup; }
            set { this.showStartPageOnStartup = value; }
        }

        #endregion

        #region Custom Settings

        /// <summary>
        /// Gets and sets customStartPage
        /// </summary>
        [DisplayName("Custom Start Page")]
        [LocalizedDescription("StartPage.Description.CustomStartPage")]
        [LocalizedCategory("StartPage.Category.Custom")]
        [DefaultValue("")]
        public String CustomStartPage
        {
            get { return customStartPage; }
            set { this.customStartPage = value; }
        }

        /// <summary>
        /// Gets and sets useCustomStartPage
        /// </summary>
        [DisplayName("Use Custom Start Page")]
        [LocalizedDescription("StartPage.Description.UseCustomStartPage")]
        [LocalizedCategory("StartPage.Category.Custom")]
        [DefaultValue(false)]
        public Boolean UseCustomStartPage
        {
            get { return useCustomStartPage; }
            set { this.useCustomStartPage = value; }
        }

        /// <summary>
        /// Gets and sets customRssFeed
        /// </summary>
        [DisplayName("Custom RSS Feed")]
        [LocalizedDescription("StartPage.Description.CustomRssFeed")]
        [LocalizedCategory("StartPage.Category.Custom")]
        [DefaultValue("")]
        public String CustomRssFeed
        {
            get { return customRssFeed; }
            set { this.customRssFeed = value; }
        }

        /// <summary>
        /// Gets and sets useCustomRssFeed
        /// </summary>
        [DisplayName("Use Custom RSS Feed")]
        [LocalizedDescription("StartPage.Description.UseCustomRssFeed")]
        [LocalizedCategory("StartPage.Category.Custom")]
        [DefaultValue(false)]
        public Boolean UseCustomRssFeed
        {
            get { return useCustomRssFeed; }
            set { this.useCustomRssFeed = value; }
        }

        #endregion

    }

    #region Enums

    public enum ShowStartPageOnStartupEnum
    {
        Always,
        NotRestoringSession,
        Never
    }

    #endregion

}
