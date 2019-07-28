using FlashDevelop.Settings;
using PluginCore;

namespace FlashDevelop
{
    public class Globals
    {
        /// <summary>
        /// Quick reference to MainForm 
        /// </summary> 
        public static MainForm MainForm
        {
            get { return MainForm.Instance; }
        }

        /// <summary>
        /// Quick reference to CurrentDocument 
        /// </summary>
        public static ITabbedDocument CurrentDocument 
        {
            get { return MainForm.CurrentDocument; }
        }

        /// <summary>
        /// Quick reference to Settings 
        /// </summary>
        public static SettingObject Settings
        {
            get { return MainForm.AppSettings; }
        }

    }

}
