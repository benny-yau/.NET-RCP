using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FlashDevelop.Dialogs;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;

namespace FlashDevelop.Utilities
{
	public class ArgsProcessor
	{
        /// <summary>
        /// Regexes for tab and var replacing
        /// </summary>
        private static Regex reTabs = new Regex("^\\t+", RegexOptions.Multiline | RegexOptions.Compiled);
        private static Regex reArgs = new Regex("\\$\\(([a-z$]+)\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        
        /// <summary>
        /// Regexes and variabled for enhanced arguments
        /// </summary>
        private static Dictionary<String, String> userArgs;
        private static Regex reUserArgs = new Regex("\\$\\$\\(([a-z0-9]+)\\=?([^\\)]+)?\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static Regex reSpecialArgs = new Regex("\\$\\$\\(\\#([a-z]+)\\#=?([^\\)]+)?\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static Regex reEnvArgs = new Regex("\\$\\$\\(\\%([a-z]+)\\%\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public static Regex reURL = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Compiled);
        /// <summary>
        /// Gets the FlashDevelop root directory
		/// </summary>
        public static String GetAppDir()
		{
			return PathHelper.AppDir;
		}

        /// <summary>
        /// Gets the users FlashDevelop directory
        /// </summary>
        public static String GetUserAppDir()
        {
            return PathHelper.UserAppDir;
        }

        /// <summary>
        /// Gets the data file directory
        /// </summary>
        public static String GetBaseDir()
        {
            return PathHelper.BaseDir;
        }

		/// <summary>
		/// Gets the current file
		/// </summary>
        public static String GetCurFile()
		{
            if (Globals.CurrentDocument == null ||!Globals.CurrentDocument.IsEditable) return String.Empty;
            else return Globals.CurrentDocument.FileName;
		}

		/// <summary>
		/// Gets the current file's path or last active path
		/// </summary>
        public static String GetCurDir()
		{
            if (Globals.CurrentDocument != null && !Globals.CurrentDocument.IsEditable) return Globals.MainForm.WorkingDirectory;
            else return Path.GetDirectoryName(GetCurFile());
		}
		
		/// <summary>
		/// Gets the name of the current file
		/// </summary>
        public static String GetCurFilename()
		{
            if (Globals.CurrentDocument != null && !Globals.CurrentDocument.IsEditable) return String.Empty;
            else return Path.GetFileName(GetCurFile());
        }

        /// <summary>
        /// Gets the name of the current file without extension
        /// </summary>
        public static String GetCurFilenameNoExt()
        {
            if (Globals.CurrentDocument != null && !Globals.CurrentDocument.IsEditable) return String.Empty;
            else return Path.GetFileNameWithoutExtension(GetCurFile());
        }

        /// <summary>
        /// Gets the timestamp
        /// </summary>
        public static String GetTimestamp()
        {
            return DateTime.Now.ToString("g");
        }
		
		/// <summary>
		/// Gets the desktop path
		/// </summary>
        public static String GetDesktopDir()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		}
		
		/// <summary>
		/// Gets the system path
		/// </summary>
        public static String GetSystemDir()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.System);
		}
		
		/// <summary>
		/// Gets the program files path
		/// </summary>
        public static String GetProgramsDir()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
		}
		
		/// <summary>
		/// Gets the users personal files path
		/// </summary>
        public static String GetPersonalDir()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}
		
		/// <summary>
		/// Gets the working directory
		/// </summary>
        public static String GetWorkingDir()
		{
            return Globals.MainForm.WorkingDirectory;
		}
		
		/// <summary>
		/// Gets the user selected file for opening
		/// </summary>
        public static String GetOpenFile()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = GetCurDir();
			ofd.Multiselect = false;
            if (ofd.ShowDialog(Globals.MainForm) == DialogResult.OK) return ofd.FileName;
            else return String.Empty;
		}
		
		/// <summary>
		/// Gets the user selected file for saving
		/// </summary>
        public static String GetSaveFile()
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.InitialDirectory = GetCurDir();
            if (sfd.ShowDialog(Globals.MainForm) == DialogResult.OK) return sfd.FileName;
            else return String.Empty;
		}
		
		/// <summary>
		/// Gets the user selected folder
		/// </summary>
        public static String GetOpenDir()
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog(Globals.MainForm) == DialogResult.OK) return fbd.SelectedPath;
            else return String.Empty;
		}
		
		/// <summary>
		/// Gets the clipboard text
		/// </summary>
        public static String GetClipboard()
		{
			IDataObject cbdata = Clipboard.GetDataObject();
			if (cbdata.GetDataPresent("System.String", true)) 
			{
				return cbdata.GetData("System.String", true).ToString();
			}
            else return String.Empty;
		}

		/// <summary>
		/// Processes the argument String variables
		/// </summary>
		public static String ProcessString(String args, Boolean dispatch)
		{
            try
            {
                String result = args;
                if (result == null) return String.Empty;
                result = reArgs.Replace(result, new MatchEvaluator(ReplaceVars));
                if (!dispatch || result.IndexOf('$') < 0) return result;
                TextEvent te = new TextEvent(EventType.ProcessArgs, result);
                EventManager.DispatchEvent(Globals.MainForm, te);
                result = ReplaceArgsWithGUI(te.Value);
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return String.Empty;
            }
		}

        /// <summary>
        /// Match evaluator for vars
        /// </summary>
        public static String ReplaceVars(Match match)
        {
            if (match.Groups.Count > 0)
            {
                string name = match.Groups[1].Value;
                switch (name)
                {
                    case "Quote" : return "\"";
                    case "AppDir" : return GetAppDir();
                    case "UserAppDir" : return GetUserAppDir();
                    case "BaseDir" : return GetBaseDir();
                    case "CurFilename": return GetCurFilename();
                    case "CurFilenameNoExt": return GetCurFilenameNoExt();
                    case "CurFile" : return GetCurFile();
                    case "CurDir" : return GetCurDir();
                    case "Timestamp" : return GetTimestamp();
                    case "OpenFile" : return GetOpenFile();
                    case "SaveFile" : return GetSaveFile();
                    case "OpenDir" : return GetOpenDir();
                    case "DesktopDir" : return GetDesktopDir();
                    case "SystemDir" : return GetSystemDir();
                    case "ProgramsDir" : return GetProgramsDir();
                    case "PersonalDir" : return GetPersonalDir();
                    case "WorkingDir" : return GetWorkingDir();
                    case "Clipboard": return GetClipboard();
                    case "Dollar": return "$";
                }
                foreach (Argument arg in ArgumentDialog.CustomArguments)
                {
                    if (name == arg.Key) return arg.Value;
                }
                return "$(" + name + ")";
            }
            else return match.Value;
        }
        
        /// <summary>
        /// Replaces the enchanced arguments with gui
        /// </summary>
        public static String ReplaceArgsWithGUI(String args)
        {
            if (args.IndexOf("$$(") < 0) return args;
            if (reEnvArgs.IsMatch(args)) // Environmental arguments
            {
                args = reEnvArgs.Replace(args, new MatchEvaluator(ReplaceEnvArgs));
            }
            if (reSpecialArgs.IsMatch(args)) // Special arguments
            {
                args = reSpecialArgs.Replace(args, new MatchEvaluator(ReplaceSpecialArgs));
            }
            if (reUserArgs.IsMatch(args)) // User arguments
			{
                ArgReplaceDialog rvd = new ArgReplaceDialog(args, reUserArgs);
                userArgs = rvd.Dictionary; // Save dictionary temporarily...
                if (rvd.ShowDialog() == DialogResult.OK)
                {
                    args = reUserArgs.Replace(args, new MatchEvaluator(ReplaceUserArgs));
                }
                else args = reUserArgs.Replace(args, new MatchEvaluator(ReplaceWithEmpty));
			}
            return args;
        }

        /// <summary>
        /// Match evaluator for to clear args
        /// </summary>
        public static String ReplaceWithEmpty(Match match)
        {
            return String.Empty;
        }

        /// <summary>
        /// Match evaluator for User Arguments
        /// </summary>
        public static String ReplaceUserArgs(Match match)
        {
            if (match.Groups.Count > 0) return userArgs[match.Groups[1].Value];
            else return match.Value;
        }

        /// <summary>
        /// Match evaluator for Environment Variables
        /// </summary>
        public static String ReplaceEnvArgs(Match match)
        {
            if (match.Groups.Count > 0) return Environment.GetEnvironmentVariable(match.Groups[1].Value);
            else return match.Value;
        }

        /// <summary>
        /// Match evaluator for Special Arguments
        /// </summary>
        public static String ReplaceSpecialArgs(Match match)
        {
            if (match.Groups.Count > 0)
            {
                switch (match.Groups[1].Value.ToUpper(System.Globalization.CultureInfo.InvariantCulture))
                {
                    case "DATETIME":
                    {
                        String dateFormat = "";
                        if (match.Groups.Count == 3) dateFormat = match.Groups[2].Value;
                        return (DateTime.Now.ToString(dateFormat));
                    }
                }
            }
            return match.Value;
        }

	}
	
}
