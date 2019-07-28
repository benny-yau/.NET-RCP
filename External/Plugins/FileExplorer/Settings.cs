using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using PluginCore;
using PluginCore.Localization;

namespace FileExplorer
{
    [Serializable]
    public class Settings
    {
        private Int32 sortOrder = 0;
        private Int32 sortColumn = 0;

        /// <summary> 
        /// Get and sets the filePath.
        /// </summary>
        [DisplayName("Active Path")]
        [LocalizedDescription("FileExplorer.Description.FilePath"), DefaultValue("C:\\"), ReadOnly(true)]
        public String FilePath
        {
            get { return PluginBase.MainForm.Settings.LatestDialogPath; }
            set { PluginBase.MainForm.Settings.LatestDialogPath = value; }
        }

        /// <summary> 
        /// Get and sets the sortColumn.
        /// </summary>
        [DisplayName("Active Column")]
        [LocalizedDescription("FileExplorer.Description.SortColumn"), DefaultValue(0), ReadOnly(true)]
        public Int32 SortColumn
        {
            get { return this.sortColumn; }
            set { this.sortColumn = value; }
        }

        /// <summary> 
        /// Get and sets the sortOrder.
        /// </summary>
        [DisplayName("Sort Order")]
        [LocalizedDescription("FileExplorer.Description.SortOrder"), DefaultValue(0), ReadOnly(true)]
        public Int32 SortOrder
        {
            get { return this.sortOrder; }
            set { this.sortOrder = value; }
        }

    }

}
