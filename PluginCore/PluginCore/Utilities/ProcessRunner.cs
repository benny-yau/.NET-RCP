using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using PluginCore.Managers;

namespace PluginCore.Utilities
{
	public class ProcessRunner
	{
		Process process;
		Boolean isRunning;
		StreamReader outputReader;
		StreamReader errorReader;
		Int32 tasksFinished;
		
		public event LineOutputHandler Output;
		public event LineOutputHandler Error;
		public event ProcessEndedHandler ProcessEnded;

        public String WorkingDirectory;
        public Process HostedProcess { get { return process; } }
        public Boolean IsRunning { get { return isRunning; } }
        public Boolean RedirectInput;

        public void Run(String fileName, String arguments)
        {
            Run(fileName, arguments, false);
        }

        public void Run(String fileName, String arguments, Boolean shellCommand)
		{
			if (isRunning) throw new Exception("This ProcessRunner is already running a process.");
            if (!shellCommand && !File.Exists(fileName)) throw new FileNotFoundException("The program '" + fileName + "' was not found.", fileName);

			isRunning = true;
			process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = RedirectInput;
			process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardOutputEncoding = Encoding.Default;
            process.StartInfo.StandardErrorEncoding = Encoding.Default;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = fileName;
			process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = WorkingDirectory ?? PluginBase.MainForm.WorkingDirectory;
			process.Start();
			
			outputReader = process.StandardOutput;
			errorReader = process.StandardError;
			
			// we need to wait for all 3 threadpool operations 
            // to finish (processexit, readoutput, readerror)
			tasksFinished = 0;
			
			ThreadStart waitForExitDel = new ThreadStart(process.WaitForExit);
			waitForExitDel.BeginInvoke(new AsyncCallback(TaskFinished), null);
			
			ThreadStart readOutputDel = new ThreadStart(ReadOutput);
			ThreadStart readErrorDel = new ThreadStart(ReadError);
			
			readOutputDel.BeginInvoke(new AsyncCallback(TaskFinished), null);
			readErrorDel.BeginInvoke(new AsyncCallback(TaskFinished), null);
		}
		
		public void KillProcess()
		{
			try
			{
				process.Kill();
			}
			catch (Exception ex)
			{
				ErrorManager.ShowError(ex);
			}
		}
		
		private void ReadOutput()
		{
			while (true)
			{
				string line = outputReader.ReadLine();
				if (line == null) break;
				if (Output != null) Output(this, line);
			}
		}
		
		private void ReadError()
		{
			while (true)
			{
				string line = errorReader.ReadLine();
				if (line == null) break;
				if (Error != null) Error(this, line);
			}
		}
		
		private void TaskFinished(IAsyncResult result)
		{
			lock (this) 
            {
				if (++tasksFinished >= 3) 
                {
					isRunning = false;
					if (ProcessEnded != null) ProcessEnded(this, process.ExitCode);
				}
			}
		}
	}
	
	public delegate void LineOutputHandler(Object sender, String line);
    public delegate void ProcessEndedHandler(Object sender, Int32 exitCode);
    public delegate void ProcessOutputHandler(Object sender, String line);
	
}
