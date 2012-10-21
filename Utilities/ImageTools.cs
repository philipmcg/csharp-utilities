using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing;

using System.IO;

namespace Utilities
{

    public sealed class ImageTools
    {
        public static Bitmap[] SplitBMP(Bitmap bmp, int num)
        {
            Bitmap[] bmps = new Bitmap[num];
            int w = bmp.Width / num;
            int h = bmp.Height;
            for (int k = 0; k < num; k++)
            {
                bmps[k] = CropBitmap(bmp, k * w, 0, w, h);
            }
            return bmps;
        }
        public static Bitmap CropBitmap(Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropped = bitmap.Clone(rect, bitmap.PixelFormat);
            return cropped;
        }

        public static Bitmap CreateBitmapFromBytes(byte[] byteArray, Size size)
        {
            unsafe
            {
                fixed (byte* ptr = byteArray)
                {
                    return new Bitmap(size.Width, size.Height, size.Width * 3, PixelFormat.Format24bppRgb, new IntPtr(ptr));
                }
            }
        }

        public static Bitmap CreateGrayscaleBitmapFromBytes(byte[] byteArray, Size size)
        {
            unsafe
            {
                fixed (byte* ptr = byteArray)
                {
                    return new Bitmap(size.Width, size.Height, size.Width, PixelFormat.Format8bppIndexed, new IntPtr(ptr));
                }
            }
        }

        /// <summary>
        /// Uses the red-value of 32 bit image
        /// </summary>
        public static byte[] Convert32bppTo8bpp(Bitmap bmp)
        {
            byte[] bytes = BmpToBytes_Unsafe(bmp, PixelFormat.Format32bppArgb);
            byte[] dest = new byte[bytes.Length / 4];

            for (int i = 0, k = 0; i < bytes.Length; i += 4, k++)
            {
                dest[k] = bytes[i];
            }
            return dest;
        }

        /// <summary>
        /// Uses the red-value of 24 bit image
        /// </summary>
        public static byte[] Convert24bppTo8bpp(Bitmap bmp)
        {
            byte[] bytes = BmpToBytes_Unsafe(bmp, PixelFormat.Format24bppRgb);
            byte[] dest = new byte[bytes.Length / 3];

            for (int i = 0, k = 0; i < bytes.Length; i += 3, k++)
            {
                dest[k] = bytes[i];
            }
            return dest;
        }

        public static byte[,] Convert32bppTo8bpp2d(Bitmap bmp)
        {
            byte[] Array1d = Convert32bppTo8bpp(bmp);
            byte[,] dest = new byte[bmp.Width, bmp.Height];
            for (int r = 0; r < bmp.Height; r++)
			{
                for (int c = 0; c < bmp.Width; c++)
			    {
                    dest[c, r] = Array1d[r * bmp.Width + c];
			    }
			}
            return dest;
        }

        public static unsafe byte[] BmpToBytes_Unsafe(Bitmap bmp)
        {
            return BmpToBytes_Unsafe(bmp, PixelFormat.Format32bppRgb);
        }

        public static unsafe byte[] BmpToBytes_Unsafe(Bitmap bmp, PixelFormat format)
        {
            BitmapData bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size),
                ImageLockMode.ReadOnly,
                format);
            // number of bytes in the bitmap
            int byteCount = bData.Stride * bmp.Height;
            byte[] bmpBytes = new byte[byteCount];

            // Copy the locked bytes from memory
            Marshal.Copy(bData.Scan0, bmpBytes, 0, byteCount);

            // don't forget to unlock the bitmap!!
            bmp.UnlockBits(bData);

            return bmpBytes;
        }

        /// <summary>
        /// Gets all image data bytes from a bitmap
        /// </summary>
        /// <param name="path">Path to bitmap file</param>
        /// <param name="size">Size of bitmap</param>
        /// <param name="bytespp">Bytes per pixel</param>
        public static byte[] LoadBitmapBytes(string path, Size size, int bytespp)
        {
            int length = size.Width * size.Height * bytespp;
            byte[] fileContents = LoadFile(path);
            byte[] imageData = new byte[length];
            Array.Copy(fileContents, fileContents.Length - length, imageData, 0, length);
            return imageData;
        }

        /// <summary>
        /// Gets all image data bytes from a bitmap
        /// </summary>
        public static byte[] LoadBitmapBytes(string path, PixelFormat format, out Size size)
        {
            using (Bitmap bmp = new Bitmap(path))
            {
                size = bmp.Size;
                if (format == PixelFormat.Format8bppIndexed)
                {
                    if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
                        return Convert24bppTo8bpp(bmp);
                    else if (bmp.PixelFormat == PixelFormat.Format32bppRgb)
                        return Convert32bppTo8bpp(bmp);
                    else if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
                        return BmpToBytes_Unsafe(bmp, format);
                    else throw new NotSupportedException("Unsupported conversion to 8-bit bitmap");
                }
                else return BmpToBytes_Unsafe(bmp, format);
            }
        }

        /// <summary>
        /// Gets all bytes from a binary file
        /// </summary>
        static byte[] LoadFile(string path)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                byte[] bytes = reader.ReadBytes((int)reader.BaseStream.Length);
                return bytes;
            }
        }
        
        /// <summary>
        /// A quick lookup for getting image encoders
        /// </summary>
        private static Dictionary<string, ImageCodecInfo> encoders = null;

        /// <summary>
        /// A quick lookup for getting image encoders
        /// </summary>
        public static Dictionary<string, ImageCodecInfo> Encoders
        {
            //get accessor that creates the dictionary on demand
            get
            {
                //if the quick lookup isn't initialised, initialise it
                if (encoders == null)
                {
                    encoders = new Dictionary<string, ImageCodecInfo>();
                }

                //if there are no codecs, try loading them
                if (encoders.Count == 0)
                {
                    //get all the codecs
                    foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                    {
                        //add each codec to the quick lookup
                        encoders.Add(codec.MimeType.ToLower(), codec);
                    }
                }

                //return the lookup
                return encoders;
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            //a holder for the result
            Bitmap result = new Bitmap(width, height);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }

        /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path">Path to which the image would be saved.</param> 
        /// <param name="quality">An integer from 0 to 100, with 100 being the 
        /// highest quality</param> 
        /// <exception cref="ArgumentOutOfRangeException">
        /// An invalid value was entered for image quality.
        /// </exception>
        public static void SaveJpeg(string path, Image image, int quality)
        {
            //ensure the quality is within the correct range
            if ((quality < 0) || (quality > 100))
            {
                //create the error message
                string error = string.Format("Jpeg image quality must be between 0 and 100, with 100 being the highest quality.  A value of {0} was specified.", quality);
                //throw a helpful exception
                throw new ArgumentOutOfRangeException(error);
            }

            //create an encoder parameter for the image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            //get the jpeg codec
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

            //create a collection of all parameters that we will pass to the encoder
            EncoderParameters encoderParams = new EncoderParameters(1);
            //set the quality parameter for the codec
            encoderParams.Param[0] = qualityParam;
            //save the image using the codec and the parameters
            image.Save(path, jpegCodec, encoderParams);
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            //do a case insensitive search for the mime type
            string lookupKey = mimeType.ToLower();

            //the codec to return, default to null
            ImageCodecInfo foundCodec = null;

            //if we have the encoder, get it to return
            if (Encoders.ContainsKey(lookupKey))
            {
                //pull the codec from the lookup
                foundCodec = Encoders[lookupKey];
            }

            return foundCodec;
        } 
    }


}
