using System;
using System.Diagnostics;
using PluginCore.Managers;

namespace PluginCore.Helpers
{
	public class ProcessHelper
	{
		/// <summary>
		/// Starts a basic process asynchronously
		/// </summary>
		public static void StartAsync(String path)
		{
            StartDelegate1 dlgt = new StartDelegate1(Start);
            dlgt.BeginInvoke(path, null, null);
		}
        
        /// <summary>
        /// Starts a process with arguments asynchronously
		/// </summary>
		public static void StartAsync(String path, String arguments)
		{
            StartDelegate2 dlgt = new StartDelegate2(Start);
            dlgt.BeginInvoke(path, arguments, null, null);
		}

        /// <summary>
        /// Starts a process with start info asynchronously
        /// </summary>
        public static void StartAsync(ProcessStartInfo psi)
        {
            StartDelegate3 dlgt = new StartDelegate3(Start);
            dlgt.BeginInvoke(psi, null, null);
        }

        /// <summary>
        /// Runs a basic process
        /// </summary>
		private static void Start(String path)
		{
            try
            {
                Process.Start(path);
            }
            catch(Exception ex)
            {
                TraceManager.AddAsync(ex.Message);
            }
		}

        /// <summary>
        /// Runs a process with arguments
        /// </summary>
        private static void Start(String path, String arguments)
        {
            try
            {
                Process.Start(path, arguments);
            }
            catch (Exception ex)
            {
                TraceManager.AddAsync(ex.Message);
            }
        }

        /// <summary>
        /// Runs a process with start info
        /// </summary>
        private static void Start(ProcessStartInfo psi)
        {
            try
            {
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                TraceManager.AddAsync(ex.Message);
            }
        }

        /// <summary>
        /// Event delegates of the class
        /// </summary>
		private delegate void StartDelegate1(String path);
        private delegate void StartDelegate2(String path, String arguments);
        private delegate void StartDelegate3(ProcessStartInfo psi);

	}

}
