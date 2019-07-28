using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Localization;
using WeifenLuo.WinFormsUI.Docking;

namespace PluginCore
{
    public interface IPlugin : IEventHandler
    {
	    #region IPlugin Methods
		
	    void Dispose();
	    void Initialize();
		
	    #endregion
		
	    #region IPlugin Properties

        Int32 Api { get; }
        String Name { get; }
        String Guid { get; }
        String Help { get; }
        String Author { get; }
        String Description { get; }
        Object Settings { get; }
        Boolean CanDisable { get; }

        // List of valid API levels:
        // FlashDevelop 4.0 = 1
		
	    #endregion
    }

    public interface IEventHandler
	{
        #region IEventHandler Methods

        void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority);

        #endregion
	}

    public interface ITabbedDocument
    {
        #region ITabbedDocument Properties

        Icon Icon { get; set; }
        String FileName { get; }
        String Text { get; set; }
        Boolean UseCustomIcon { get; set; }
        Control.ControlCollection Controls { get; }
        Boolean IsBrowsable { get; }
        Boolean IsEditable { get; }

        #endregion

        #region ITabbedDocument Methods

        void Close();
        void Activate();

        #endregion
    }

    public interface IMainForm : IWin32Window
    {
        #region IMainForm Methods

        void RefreshUI();
        void ShowSettingsDialog(String itemName);
        void ShowErrorDialog(Object sender, Exception ex);
        void ShowSettingsDialog(String itemName, String filter);
        void RegisterShortcutItem(String id, ToolStripMenuItem item);
        DockContent CreateCustomDocument(Control ctrl);
		DockContent CreateDockablePanel(Control form, String guid, Image image, DockState defaultDockState);
        Boolean CallCommand(String command, String arguments);
        List<ToolStripItem> FindMenuItems(String name);
        ToolStripItem FindMenuItem(String name);
        String ProcessArgString(String args);
        String GetThemeValue(String id);
        Color GetThemeColor(String id);
        IPlugin FindPlugin(String guid);
        Image FindImage(String data);
        void Browse(Object sender, System.EventArgs e);
        void BrowseWebsite(String url);
        #endregion

        #region IMainForm Properties

        ISettings Settings { get; }
        ToolStrip ToolStrip { get; }
        MenuStrip MenuStrip { get; }
        DockPanel DockPanel { get; }
        String[] StartArguments { get; }
        List<Argument> CustomArguments { get; }
        StatusStrip StatusStrip { get; }
        String WorkingDirectory { get; set; }
        ToolStripPanel ToolStripPanel { get; }
        ToolStripStatusLabel StatusLabel { get; }
        ToolStripStatusLabel ProgressLabel { get; }
        ToolStripProgressBar ProgressBar { get; }
        Control.ControlCollection Controls { get; }
        ContextMenuStrip TabMenu { get; }
        ITabbedDocument CurrentDocument { get; }
        ITabbedDocument[] Documents { get; }
        Boolean ClosingEntirely { get; }
        Boolean ProcessIsRunning { get; }
        Boolean ProcessingContents { get; }
        Boolean RestoringContents { get; }
        Boolean PanelIsActive { get; }
        Boolean IsFullScreen { get; }
        Boolean StandaloneMode { get; }
        Boolean MultiInstanceMode { get; }
        Boolean IsFirstInstance { get; }
        Boolean RestartRequested { get; }
        List<Keys> IgnoredKeys { get; }
        String ProductVersion { get; }
        String ProductName { get; }

        #endregion
    }


    public interface ISettings
    {
        #region ISettings Properties

        Font DefaultFont { get; set; }
        Font ConsoleFont { get; set; }
        List<String> DisabledPlugins { get; set; }
        LocaleVersion LocaleVersion { get; set; }
        UiRenderMode RenderMode { get; set; }
        String LatestDialogPath { get; set; }
        Boolean ConfirmOnExit { get; set; }
        Boolean UseSystemColors { get; set; }
        Boolean ViewToolBar { get; set; }
        Boolean ViewStatusBar { get; set; }
        Size WindowSize { get; set; }
        FormWindowState WindowState { get; set; }
        Point WindowPosition { get; set; }
        List<String> FileTypesToOpen { get; set; }
        #endregion
    }

    public interface ISession
    {
        #region ISession Properties

        Int32 Index { get; set; }
        List<String> Files { get; set; }
        SessionType Type { get; set; }

        #endregion
    }

}