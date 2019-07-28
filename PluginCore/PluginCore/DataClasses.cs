using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace PluginCore
{
    #region Data Objects

    /// <summary>
    /// Menus items
    /// </summary>
    public class ItemData
    {
        public String Id = String.Empty;
        public String Tag = String.Empty;
        public String Flags = String.Empty;

        public ItemData(String id, String tag, String flags)
        {
            if (id != null) this.Id = id;
            if (tag != null) this.Tag = tag;
            if (flags != null) this.Flags = flags;
        }

    }

    /// <summary>
    /// User custom arguments
    /// </summary>
    [Serializable]
    public class Argument
    {
        private String key = String.Empty;
        private String value = String.Empty;

        public Argument() { }
        public Argument(String key, String value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Gets and sets the key
        /// </summary> 
        public String Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// Gets and sets the value
        /// </summary> 
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public override string ToString()
        {
            return String.IsNullOrEmpty(this.key) ? "New argument" : "$(" + this.key + ")";
        }
    }

    #endregion


}
