using System;
using System.IO;
using System.Text;
using LanternExtractor.Infrastructure.Logger;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Pfim;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace LanternExtractor.Infrastructure
{
    public static class ImageWriter
    {
        public static void WriteImageAsPng(byte[] bytes, string filePath, string fileName, bool isMasked,
            ILogger logger)
        {
            // https://docs.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide#dds-file-layout
            bool ddsMagic = Encoding.ASCII.GetString(bytes, 0, 4) == "DDS ";
            if (fileName.EndsWith(".bmp") && !ddsMagic)
            {
                WriteBmpAsPng(bytes, filePath, Path.GetFileNameWithoutExtension(fileName) + ".png", isMasked, false,
                    logger);
            }
            else
            {
                WriteDdsAsPng(bytes, filePath, Path.GetFileNameWithoutExtension(fileName) + ".png");
            }
        }

        private static void WriteBmpAsPng(byte[] bytes, string filePath, string fileName, bool isMasked, bool rotate,
            ILogger logger)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            Directory.CreateDirectory(filePath);

            EqBmp image;
            var byteStream = new MemoryStream(bytes);

            try
            {
                image = new EqBmp(byteStream);
            }
            catch (Exception e)
            {
                logger.LogError("Caught exception while creating bitmap: " + e);
                return;
            }

            // The filename is misspelled in the archive
            // It only works because there is a matching canwall1.png in the objects archive
            // If we find more like this, we can create a function to fix them.
            if (fileName == "canwall1a.png")
            {
                fileName = "canwall1.png";
            }

            if (image.PixelFormat == PixelFormat.Format8bppIndexed && isMasked)
            {
                var paletteIndex = GetPaletteIndex(fileName);
                image.MakePaletteTransparent(paletteIndex);
            }
            else
            {
                image.MakeMagentaTransparent();
            }

            image.WritePng(Path.Combine(filePath, fileName));
        }

        private static void WriteDdsAsPng(byte[] bytes, string filePath, string fileName)
        {
            using (IImage image = Pfim.Pfim.FromStream(new MemoryStream(bytes)))
            {
                PixelFormat format;

                // Convert from Pfim's backend agnostic image format into GDI+'s image format
                switch (image.Format)
                {
                    // Some PoP sky.s3d tga files
                    case Pfim.ImageFormat.Rgb24:
                        format = PixelFormat.Format24bppRgb;
                        break;
                    case Pfim.ImageFormat.Rgba32:
                        format = PixelFormat.Format32bppArgb;
                        break;
                    // case Pfim.ImageFormat.R5g5b5:
                    //     format = PixelFormat.Format16bppRgb555;
                    //     break;
                    // case Pfim.ImageFormat.R5g6b5:
                    //     format = PixelFormat.Format16bppRgb565;
                    //     break;
                    // case Pfim.ImageFormat.R5g5b5a1:
                    //     format = PixelFormat.Format16bppArgb1555;
                    //     break;
                    // case Pfim.ImageFormat.Rgb8:
                    //     format = PixelFormat.Format8bppIndexed;
                    //     break;
                    default:
                        return;
                }

                // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                // in this snippet but useful technique if the data was going to be used in
                // control like a picture box
                var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                try
                {
                    var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                    var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    Directory.CreateDirectory(filePath);
                    bitmap.Save(Path.Combine(filePath, fileName), ImageFormat.Png);
                }
                finally
                {
                    handle.Free();
                }
            }
        }

        private static int GetPaletteIndex(string fileName)
        {
            switch (fileName)
            {
                case "clhe0004.png":
                case "kahe0001.png":
                    return 255;
                case "furpile1.png":
                    return 250;
                case "bearrug.png":
                    return 47;
                default:
                    return 0;
            }
        }
    }
}
