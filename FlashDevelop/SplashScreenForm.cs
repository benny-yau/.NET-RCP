using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FlashDevelop.Helpers;

namespace FlashDevelop
{
    public partial class SplashScreenForm : Form
    {
        static SplashScreenForm splashScreen;

        public static SplashScreenForm SplashScreen
        {
            get
            {
                return splashScreen;
            }
            set
            {
                splashScreen = value;
            }
        }

        /// <summary>
        /// Initialize splash screen and display picture as background image
        /// </summary> 
        public SplashScreenForm()
        {
            InitializeComponent();
            Stream stream = ResourceHelper.GetStream("splash_rcp.jpg");
            BackgroundImage = Image.FromStream(stream);
        }

        /// <summary>
        /// Show splash screen
        /// </summary> 
        public static void ShowSplashScreen()
        {
            splashScreen = new SplashScreenForm();
            splashScreen.Show();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
