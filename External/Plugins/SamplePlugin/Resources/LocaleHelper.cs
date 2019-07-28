using System;
using System.Reflection;
using System.Resources;
using PluginCore.Localization;

namespace SamplePlugin.Resources
{
    class LocaleHelper
    {
        private static ResourceManager resources = null;

        /// <summary>
        /// Initializes the localization of the plugin
        /// </summary>
        public static void Initialize(LocaleVersion locale)
        {
            String path = "SamplePlugin.Resources." + locale.ToString();
            resources = new ResourceManager(path, Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Loads a string from the internal resources
        /// </summary>
        public static String GetString(String identifier)
        {
            return resources.GetString(identifier);
        }

    }

}
