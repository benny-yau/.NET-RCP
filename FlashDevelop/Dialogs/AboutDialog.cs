using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FlashDevelop.Helpers;

namespace FlashDevelop.Dialogs
{
    public class AboutDialog : Form
	{
        private System.Windows.Forms.Label copyLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.PictureBox imageBox;

		public AboutDialog()
		{
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
            this.InitializeGraphics();
		}

		#region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
		public void InitializeComponent() 
        {
            this.imageBox = new System.Windows.Forms.PictureBox();
            this.copyLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox
            // 
            this.imageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBox.Location = new System.Drawing.Point(0, 0);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(452, 228);
            this.imageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imageBox.TabIndex = 0;
            this.imageBox.TabStop = false;
            this.imageBox.Click += new System.EventHandler(this.DialogCloseClick);
            // 
            // copyLabel
            // 
            this.copyLabel.AutoSize = true;
            this.copyLabel.BackColor = System.Drawing.Color.White;
            this.copyLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.copyLabel.ForeColor = System.Drawing.Color.Black;
            this.copyLabel.Location = new System.Drawing.Point(25, 191);
            this.copyLabel.Name = "copyLabel";
            this.copyLabel.Size = new System.Drawing.Size(199, 13);
            this.copyLabel.TabIndex = 0;
            this.copyLabel.Text = "Derived from FlashDevelop version 4.4.2";
            this.copyLabel.Click += new System.EventHandler(this.DialogCloseClick);
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.BackColor = System.Drawing.Color.White;
            this.versionLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.versionLabel.ForeColor = System.Drawing.Color.Black;
            this.versionLabel.Location = new System.Drawing.Point(25, 173);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(216, 13);
            this.versionLabel.TabIndex = 0;
            this.versionLabel.Text = ".NET RCP version 1.0 for Microsoft.NET 4.0";
            this.versionLabel.Click += new System.EventHandler(this.DialogCloseClick);
            // 
            // AboutDialog
            // 
            this.ClientSize = new System.Drawing.Size(452, 228);
            this.Controls.Add(this.copyLabel);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.imageBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " About .NET RCP";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DialogKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		#region Methods And Event Handlers

        /// <summary>
        /// Attaches the image to the imagebox
        /// </summary>
        private void InitializeGraphics()
        {
            Stream stream = ResourceHelper.GetStream("splash_rcp.jpg");
            this.imageBox.Image = Image.FromStream(stream);
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            //this.Text = " " + TextHelper.GetString("Title.AboutDialog");
            this.versionLabel.Font = new Font(this.Font, FontStyle.Bold);
            //this.versionLabel.Text = Application.ProductName;
        }

        /// <summary>
        /// Closes the about dialog
        /// </summary>
        private void DialogKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

		/// <summary>
		/// Closes the about dialog
		/// </summary>
        private void DialogCloseClick(Object sender, EventArgs e)
		{
			this.Close();
		}

        /// <summary>
        /// Shows the about dialog
        /// </summary>
        public static new void Show()
        {
            AboutDialog aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog();
        }

		#endregion

	}
	
}
