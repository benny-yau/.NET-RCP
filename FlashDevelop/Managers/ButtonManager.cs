using System;
using System.Windows.Forms;
using PluginCore;

namespace FlashDevelop.Managers
{
    public class ButtonManager
    {
        /// <summary>
        /// Updates the flagged buttons
        /// </summary>
        public static void UpdateFlaggedButtons()
        {
            Int32 count = StripBarManager.Items.Count;
            for (Int32 i = 0; i < count; i++)
            {
                ToolStripItem item = (ToolStripItem)StripBarManager.Items[i];
                String[] actions = ((ItemData)item.Tag).Flags.Split('+');
                for (Int32 j = 0; j < actions.Length; j++)
                {
                    Boolean value = ValidateFlagAction(item, actions[j]);
                    ExecuteFlagAction(item, actions[j], value);
                }
            }
        }

        /// <summary>
        /// Checks the flagged CommandBar item
        /// </summary>
        public static Boolean ValidateFlagAction(ToolStripItem item, String action)
        {
            ITabbedDocument document = Globals.CurrentDocument;
            if (action.Contains("!ProcessIsRunning"))
            {
                if (Globals.MainForm.ProcessIsRunning) return false;
            }
            else if (action.Contains("ProcessIsRunning"))
            {
                if (!Globals.MainForm.ProcessIsRunning) return false;
            }
            if (action.Contains("!StandaloneMode"))
            {
                if (Globals.MainForm.StandaloneMode) return false;
            }
            else if (action.Contains("StandaloneMode"))
            {
                if (!Globals.MainForm.StandaloneMode) return false;
            }
            if (action.Contains("!MultiInstanceMode"))
            {
                if (Globals.MainForm.MultiInstanceMode) return false;
            }
            else if (action.Contains("MultiInstanceMode"))
            {
                if (!Globals.MainForm.MultiInstanceMode) return false;
            }
            
            if (action.Contains("!IsFullScreen"))
            {
                if (MainForm.Instance.IsFullScreen) return false;
            }
            else if (action.Contains("IsFullScreen"))
            {
                if (!MainForm.Instance.IsFullScreen) return false;
            }
            if (action.Contains("TracksBoolean"))
            {
                Boolean value = (Boolean)Globals.Settings.GetValue(((ItemData)item.Tag).Tag);
                if (!value) return false;
            }
            return true;
        }

        /// <summary>
        /// Modifies the specified CommandBar item
        /// </summary>
        public static void ExecuteFlagAction(ToolStripItem item, String action, Boolean value)
        {
            if (action.StartsWith("Check:"))
            {
                if (item is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem)item).Checked = value;
                }
            }
            else if (action.StartsWith("Uncheck:"))
            {
                if (item is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem)item).Checked = !value;
                }
            }
            else if (action.StartsWith("Enable:"))
            {
                item.Enabled = value;
            }
            else if (action.StartsWith("Disable:"))
            {
                item.Enabled = !value;
            }
        }

    }

}
