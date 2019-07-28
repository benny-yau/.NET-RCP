using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FlashDevelop.Controls;
using FlashDevelop.Managers;
using PluginCore;
using PluginCore.Utilities;
using WeifenLuo.WinFormsUI.Docking;

namespace FlashDevelop.Docking
{
    public class TabbedDocument : DockContent, ITabbedDocument
	{
        private Boolean useCustomIcon;
        private String fileName;

        public TabbedDocument()
		{
            this.ControlAdded += new ControlEventHandler(this.DocumentControlAdded);
            this.DockPanel = Globals.MainForm.DockPanel;
            this.Font = Globals.Settings.DefaultFont;
            this.DockAreas = DockAreas.Document;
            this.BackColor = Color.White;
            this.useCustomIcon = false;
		}

        /// <summary>
        /// Disables the automatic update of the icon
        /// </summary>
        public Boolean UseCustomIcon
        {
            get { return this.useCustomIcon; }
            set { this.useCustomIcon = value; }
        }

        /// <summary>
        /// Path of the document
        /// </summary>
        public String FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                this.fileName = value;
                this.UpdateToolTipText();
            }
        }

        /// <summary>
        /// Do we contain a Browser control?
        /// </summary>
        public Boolean IsBrowsable
        {
            get
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Browser) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Is it an editable document?
        /// </summary>
        public Boolean IsEditable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Updates context menus
        /// </summary>
        public new void Activate()
        {
            ButtonManager.UpdateFlaggedButtons();
            base.Activate();
        }

        /// <summary>
        /// Automaticly updates the document icon
        /// </summary>
        private void UpdateDocumentIcon(String file)
        {
            if (this.useCustomIcon) return;
            if (!this.IsBrowsable) this.Icon = IconExtractor.GetFileIcon(file, true);
            else
            {
                Image image = Globals.MainForm.FindImage("480");
                this.Icon = ImageKonverter.ImageToIcon(image);
                this.useCustomIcon = true;
            }
        }

        /// <summary>
        /// Updates the document's tooltip
        /// </summary>
        private void UpdateToolTipText()
        {
            this.ToolTipText = this.FileName;
        }

        /// <summary>
        /// Updates the document icon when a control is added
        /// </summary>
        private void DocumentControlAdded(Object sender, ControlEventArgs e)
        {
            this.UpdateToolTipText();
            this.UpdateDocumentIcon(this.FileName);
        }

    }
	
}
