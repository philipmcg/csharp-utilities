using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Utilities
{
    public class Screenshot
    {
        public static void Capture(string path, string formatStr)
        {
            ImageFormat format = ImageFormat.MemoryBmp;

            formatStr = formatStr.ToLower();
            switch (formatStr)
            {
                case "gif":
                    format = ImageFormat.Gif;
                    break;
                case "jpg":
                case "jpeg":
                    format = ImageFormat.Jpeg;
                    break;
                case "bmp":
                    format = ImageFormat.Bmp;
                    break;
                case "exif":
                    format = ImageFormat.Exif;
                    break;
                case "emf":
                    format = ImageFormat.Emf;
                    break;
                case "wmf":
                    format = ImageFormat.Wmf;
                    break;
                case "png":
                    format = ImageFormat.Png;
                    break;
                case "tif":
                case "tiff":
                    format = ImageFormat.Tiff;
                    break;
            }

            if (format == ImageFormat.MemoryBmp)
            {
                return;
            }

            Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

            Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

            bmpScreenshot.Save(path, format);
        }
    }
}
