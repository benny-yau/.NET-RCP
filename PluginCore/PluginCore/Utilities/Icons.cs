using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PluginCore.Utilities
{
    public class FDImage
    {
        public readonly Image Img;
        public readonly int Index;

        public FDImage(Image img, int index)
        {
            Img = img;
            Index = index;
        }

        public Icon Icon { get { return Icon.FromHandle((Img as Bitmap).GetHicon()); } }
    }

    /// <summary>
    /// Contains icons used by the other plugin projects
    /// </summary>
    public class Icons
    {
        // store all extension icons we've pulled from the file system
        static Dictionary<string, FDImage> extensionIcons = new Dictionary<string, FDImage>();

        private static IMainForm mainForm;
        private static ImageList imageList;

        public static FDImage Properties;
        public static FDImage FileExplorer;


        public static ImageList ImageList { get { return imageList; } }

        public static void Initialize(IMainForm mainForm)
        {
            Icons.mainForm = mainForm;

            imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(16, 16);
            imageList.TransparentColor = Color.Transparent;
            Properties = Get("242");
            FileExplorer = Get("209");
        }

        public static Image RetrieveFDImage(String data)
        {
            if (data == "Icons.Properties")
                return Properties.Img;
            else if (data == "Icons.FileExplorer")
                return FileExplorer.Img;
            else
                return null;
        }

        public static FDImage GetGray(string data)
        {
            Image image = (mainForm != null) ? mainForm.FindImage(data) : new Bitmap(16, 16);
            Image converted = ImageKonverter.ImageToGrayscale(image);
            imageList.Images.Add(converted);
            return new FDImage(converted, imageList.Images.Count - 1);
        }

        public static FDImage Get(int fdIndex)
        {
            Image image = (mainForm != null) ? mainForm.FindImage(fdIndex.ToString()) : new Bitmap(16, 16);
            imageList.Images.Add(image);
            return new FDImage(image, imageList.Images.Count - 1);
        }

        public static FDImage Get(string data)
        {
            Image image = (mainForm != null) ? mainForm.FindImage(data) : new Bitmap(16, 16);
            imageList.Images.Add(image);
            return new FDImage(image, imageList.Images.Count - 1);
        }

        public static FDImage GetResource(string resourceID)
        {
            string filename = System.IO.Path.GetFileNameWithoutExtension(resourceID);
            Image image;
            try
            {
                resourceID = "PluginCore.Resources." + resourceID;
                Assembly assembly = Assembly.GetExecutingAssembly();
                image = new Bitmap(assembly.GetManifestResourceStream(resourceID));
            }
            catch
            {
                image = new Bitmap(16, 16);
            }
            imageList.Images.Add(filename, image);
            return new FDImage(image, imageList.Images.Count - 1);
        }

        public static FDImage ExtractIconIfNecessary(string file)
        {
            string extension = Path.GetExtension(file);
            if (extensionIcons.ContainsKey(extension))
            {
                return extensionIcons[extension];
            }
            else
            {
                Icon icon = IconExtractor.GetFileIcon(file, true);
                Image image = ImageKonverter.ImageResize(icon.ToBitmap(), 16, 16);
                icon.Dispose(); imageList.Images.Add(image);
                int index = imageList.Images.Count - 1; // of the icon we just added
                FDImage fdImage = new FDImage(image, index);
                extensionIcons.Add(extension, fdImage);
                return fdImage;
            }
        }

        public static Image Overlay(Image image, Image overlay, int x, int y)
        {
            Bitmap composed = image.Clone() as Bitmap;
            using (Graphics destination = Graphics.FromImage(composed))
            {
                destination.DrawImage(overlay, new Rectangle(x, y, 16, 16), new Rectangle(0, 0, 16, 16), GraphicsUnit.Pixel);
            }
            return composed;
        }

    }
}
