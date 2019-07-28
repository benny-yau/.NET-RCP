using System;
using System.Drawing;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Utilities;
using WeifenLuo.WinFormsUI.Docking;

namespace FlashDevelop.Controls
{
    public class PropertyPad
    {
        private static Image pluginImage;
        public static FilteredGrid propertiesUI;
        private static DockContent propertiesPanel;
        private static String propertiesGuid = "9b79609e-2b05-4e88-9430-21713aafc234";

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public static void Initialize()
        {
            pluginImage = Icons.Properties.Img;
            CreatePluginPanel();
            CreateMenuItem();
        }

        /// <summary>
        /// Creates a menu item for the plugin and registers the shortcut item
        /// </summary>
        public static void CreateMenuItem()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem propertyItem = new ToolStripMenuItem(TextHelper.GetString("Label.PropertiesWindow"), pluginImage, new EventHandler(OpenPanel));
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowPropertiesWindow", propertyItem);
            viewMenu.DropDownItems.Add(propertyItem);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public static void CreatePluginPanel()
        {
            propertiesUI = new FilteredGrid();
            propertiesUI.ToolbarVisible = false;
            propertiesUI.Text = TextHelper.GetString("Label.Properties");
            propertiesPanel = PluginBase.MainForm.CreateDockablePanel(propertiesUI, propertiesGuid, pluginImage, DockState.DockRight);
        }

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public static void OpenPanel(Object sender, System.EventArgs e)
        {
            propertiesPanel.Show();
        }
    }
}
