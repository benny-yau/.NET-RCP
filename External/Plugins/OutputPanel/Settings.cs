using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace OutputPanel
{
    [Serializable]
    public class Settings
    {
        private Boolean showOnOutput = false;
        private Boolean showOnProcessEnd = false;
        private Boolean wrapOutput = false;

        /// <summary> 
        /// Get and sets the showOnOutput
        /// </summary>
        [DisplayName("Show On Output")] 
        [LocalizedDescription("OutputPanel.Description.ShowOnOutput"), DefaultValue(false)]
        public Boolean ShowOnOutput 
        {
            get { return this.showOnOutput; }
            set { this.showOnOutput = value; }
        }

        /// <summary> 
        /// Get and sets the showOnProcessEnd
        /// </summary>
        [DisplayName("Show On Process End")] 
        [LocalizedDescription("OutputPanel.Description.ShowOnProcessEnd"), DefaultValue(false)]
        public Boolean ShowOnProcessEnd 
        {
            get { return this.showOnProcessEnd; }
            set { this.showOnProcessEnd = value; }
        }

        /// <summary> 
        /// Get and sets the wrapOutput
        /// </summary>
        [DisplayName("Wrap Output")]
        [LocalizedDescription("OutputPanel.Description.WrapOutput"), DefaultValue(false)]
        public Boolean WrapOutput
        {
            get { return this.wrapOutput; }
            set { this.wrapOutput = value; }
        }

    }

}
