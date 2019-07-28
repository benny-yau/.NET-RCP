using System;

namespace PluginCore
{
    [Flags]
    public enum EventType : long
    {
        // You can copy new values from:
        // http://stackoverflow.com/questions/1060760/what-to-do-when-bit-mask-flags-enum-gets-too-large
        RestoreSession = 1, // DataEvent (file, session)
        RestoreLayout = 2, // TextEvent (file)
        UIStarted = 4, // NotifyEvent
        UIRefresh = 8, // NotifyEvent
        UIClosing = 16, // NotifyEvent
        ApplySettings = 32, // NotifyEvent
        SettingChanged = 64, // TextEvent (setting)
        ProcessArgs = 128, // TextEvent (content)
        ProcessStart = 256, // NotifyEvent
        ProcessEnd = 512, // TextEvent (result)
        Shortcut = 1024, // DataEvent (id, keys)
        Command = 2048, // DataEvent (command)
        Trace = 4096, // NotifyEvent
        Keys = 8192, // KeyEvent (keys)
        FileEmpty = 16384, // NotifyEvent
        FileSwitch = 32768 // NotifyEvent
    }

    public enum SessionType
    {
        Startup = 0,
        Layout = 1,
        External = 2
    }

    public enum UiRenderMode
    {
        Professional,
        System
    }

    public enum HandlingPriority
    {
        High = 0,
        Normal = 1,
        Low = 2
    }

    public enum TraceType
    {
        ProcessStart = -1,
        ProcessEnd = -2,
        ProcessError = -3,
        Info = 0,
        Debug = 1,
        Warning = 2,
        Error = 3,
        Fatal = 4
    }

    public enum ViewMenu
    {
        FileExplorer,
        SearchManager,
        ReportingManager,
        LayoutManager,
        OutputPanel,
        StartPage,
        PropertiesPage
    }
}
