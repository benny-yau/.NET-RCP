using System.Windows.Forms;

namespace PluginCore
{
    public class PluginBase
    {
        private static IMainForm instance;
        
        /// <summary>
        /// Activates if the sender is MainForm
        /// </summary>
        public static void Initialize(IMainForm sender)
        {
            if (sender.GetType().ToString() == "FlashDevelop.MainForm")
            {
                instance = sender;
            }
        }

        /// <summary>
        /// Gets the instance of the Settings
        /// </summary>
        public static ISettings Settings
        {
            get { return instance.Settings; }
        }

        /// <summary>
        /// Gets the instance of the MainForm
        /// </summary>
        public static IMainForm MainForm
        {
            get { return instance; }
        }

        /// <summary>
        /// Run action on UI thread
        /// </summary>
        /// <param name="action"></param>
        public static void RunAsync(MethodInvoker action)
        {
            Form ui = MainForm as Form;
            if (ui != null && ui.InvokeRequired) ui.BeginInvoke(action);
            else action.Invoke();
        }
    }

}
