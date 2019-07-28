using System;
using System.IO;

namespace PluginCore.Managers
{
    public class DocumentManager
    {
        /// <summary>
        /// Activates the document specified by document index
        /// </summary>
        public static void ActivateDocument(Int32 index)
        {
            if (index < PluginBase.MainForm.Documents.Length && index >= 0)
            {
                PluginBase.MainForm.Documents[index].Activate();
            }
        }

        /// <summary>
        /// Finds the document by the file name
        /// </summary>
        public static ITabbedDocument FindDocument(String filename)
        {
            Int32 count = PluginBase.MainForm.Documents.Length;
            for (Int32 i = 0; i < count; i++)
            {
                ITabbedDocument current = PluginBase.MainForm.Documents[i];
                if (current.FileName == filename)
                {
                    return current;
                }
            }
            return null;
        }

        /// <summary>
        /// Closes all open files inside the given path
        /// </summary>
        public static void CloseDocuments(String path)
        {
            foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
            {
                if (document.IsBrowsable)
                {
                    path = Path.GetFullPath(path);
                    Char separator = Path.DirectorySeparatorChar;
                    String filename = document.FileName; // Path.GetFullPath(document.FileName);
                    if (filename == path || filename.StartsWith(path + separator))
                    {
                        document.Close();
                    }
                }
            }
        }
    }

}
