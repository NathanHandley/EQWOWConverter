//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using EQWOWConverter.Common;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace EQWOWConverter
{
    internal class ImageTool
    {
        private static bool FirstColorTextureGeneration = true;
        private static string GeneratedFolderPath = string.Empty;
        private static string InputCharacterTextureFolderPath = string.Empty;

        public enum IconSeriesDirection
        {
            AlongX,
            AlongY
        }

        public static void GenerateItemIconImagesFromFile(string inputImageToCutUp, int numberOfIconImagesToMake, int valueToAddToOutputIndexNumber, string outputFolderPath,
            int iconInitialWidth, int iconInitialHeight, IconSeriesDirection iconSeriesDirection, int iconsInSeries, string filePrefix, int startIndex)
        {
            Logger.WriteDetail("Generating item icons from '" + inputImageToCutUp +"' started...");

            // Read in the backdrop data first
            List<List<Color>> backdropPixels = new List<List<Color>>();
            string backdropTextureFullPath = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "CustomTextures", "item", "icons", "ItemIconBackdrop.png");
            if (Path.Exists(backdropTextureFullPath) == false)
            {
                Logger.WriteError("Failed to generate item icons since the backdrop image of '" + backdropTextureFullPath + "' did not exist");
                return;
            }
            using (Bitmap backdrop = new Bitmap(backdropTextureFullPath))
            {
                for (int x = 0; x < 64; x++)
                {
                    backdropPixels.Add(new List<Color>());
                    for (int y = 0; y < 64; y++)
                        backdropPixels[x].Add(backdrop.GetPixel(x, y));
                }
            }

            // Used for drawing the border
            Color borderPixelColor = Color.FromArgb(64, 64, 64);
            Color borderPixelColor2 = Color.FromArgb(32, 32, 32);
            List<List<int>> outputMask = new List<List<int>>();
            for (int x = 0; x < 64; x++)
            {
                outputMask.Add(new List<int>());
                for (int y = 0; y < 64; y++)
                    outputMask[x].Add(0);
            }
            
            // Output the image
            using (Bitmap inputIconsMosaic = new Bitmap(inputImageToCutUp))
            {
                int numOfMade = 0;
                for (int i = startIndex; i < numberOfIconImagesToMake + startIndex; i++)
                {
                    // Reset the mask
                    for (int x = 0; x < 64; x++)
                        for (int y = 0; y < 64; y++)
                            outputMask[x][y] = 0;

                    // Calculate the start position for this image
                    int inputPixelStartX;
                    int inputPixelStartY;
                    if (iconSeriesDirection == IconSeriesDirection.AlongY)
                    {
                        inputPixelStartX = (i / iconsInSeries) * iconInitialWidth;
                        inputPixelStartY = (i % iconsInSeries) * iconInitialHeight;
                    }
                    else
                    {
                        inputPixelStartX = (i % iconsInSeries) * iconInitialWidth;
                        inputPixelStartY = (i / iconsInSeries) * iconInitialHeight;
                    }

                    // Create an image representing only the source image slice
                    using (Bitmap sourceIconImage = new Bitmap(iconInitialWidth, iconInitialHeight))
                    {
                        // Copy the pixel data into the source icon image
                        for (int y = 0; y < iconInitialHeight; y++)
                        {
                            for (int x = 0; x < iconInitialWidth; x++)
                            {
                                int curInputPixelStartX = inputPixelStartX + x;
                                int curInputPixelStartY = inputPixelStartY + y;

                                // Add some offsets for specific icons that are not well centered
                                int iconID = i + valueToAddToOutputIndexNumber;
                                int addedYOffset = 0;
                                if (filePrefix == "INV_EQ_" && (iconID == 13 || iconID == 33)) // Wrist and a Belt
                                    addedYOffset = 8;

                                // Output only non-alpha pixels
                                Color pixelColor = inputIconsMosaic.GetPixel(curInputPixelStartX, curInputPixelStartY);
                                if (pixelColor.A != 0)
                                {
                                    sourceIconImage.SetPixel(x, y + addedYOffset, inputIconsMosaic.GetPixel(curInputPixelStartX, curInputPixelStartY));
                                    outputMask[x][y + addedYOffset] = 1;
                                }
                            }
                        }

                        // Create the border (layer 1)
                        for (int x = 1; x < iconInitialWidth-1; x++)
                        {
                            for (int y = 1; y < iconInitialHeight-1; y++)
                            {
                                // Skip written-to
                                if (outputMask[x][y] == 1)
                                    continue;

                                // Mark all alpha locations that touch the item paremeter
                                if (outputMask[x - 1][y] == 1 || outputMask[x + 1][y] == 1 || outputMask[x][y - 1] == 1 || outputMask[x][y + 1] == 1)
                                {
                                    sourceIconImage.SetPixel(x, y, borderPixelColor);
                                    outputMask[x][y] = 2;
                                }
                            }
                        }

                        // Create the border (layer 2)
                        for (int x = 1; x < iconInitialWidth-1; x++)
                        {
                            for (int y = 1; y <= iconInitialHeight-1; y++)
                            {
                                // Skip written-to
                                if (outputMask[x][y] > 0)
                                    continue;

                                // Mark all alpha locations that touch the item border paremeter
                                if (outputMask[x - 1][y] == 2 || outputMask[x + 1][y] == 2 || outputMask[x][y - 1] == 2 || outputMask[x][y + 1] == 2)
                                {
                                    sourceIconImage.SetPixel(x, y, borderPixelColor2);
                                }
                            }
                        }

                        // Create a scaled image 
                        using (Bitmap scaledIconImage = new Bitmap(58, 58))
                        {
                            using (Graphics graphics = Graphics.FromImage(scaledIconImage))
                            {
                                // Set interpolation mode to Linear
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                                // Draw the scaled image
                                graphics.DrawImage(sourceIconImage, 0, 0, 58, 58);
                            }

                            // Create a final image with the background put on first, then the scaled image
                            using (Bitmap outputImage = new Bitmap(64, 64))
                            {
                                // Output the backdrop first
                                for (int y = 0; y < 64; y++)
                                {
                                    for (int x = 0; x < 64; x++)
                                    {
                                        // Always output the backdrop
                                        outputImage.SetPixel(x, y, backdropPixels[x][y]);
                                    }
                                }

                                // Output the scaled image
                                for (int y = 0; y < 58; y++)
                                {
                                    for (int x = 0; x < 58; x++)
                                    {
                                        // Determine output spot
                                        int curPixelOutputX = x + 3;
                                        int curPixelOutputY = y + 3;

                                        // Skip the corner blocks
                                        if ((x == 0 || x == 57) && (y < 3 || y > 55))
                                            continue;
                                        if ((x == 1 || x == 56) && (y < 2 || y > 56))
                                            continue;
                                        if ((x == 2 || x == 55) && (y < 1 || y > 57))
                                            continue;
                                        if ((y == 0 || y == 57) && (x < 3 || x > 55))
                                            continue;
                                        if ((y == 1 || y == 56) && (x < 2 || x > 56))
                                            continue;
                                        if ((y == 2 || y == 55) && (x < 1 || x > 57))
                                            continue;

                                        // Get the current pixel's color;
                                        Color scaledPixelColor = scaledIconImage.GetPixel(x, y);
                                        Color targetPixelColor = outputImage.GetPixel(curPixelOutputX, curPixelOutputY);

                                        // Output factoring for alpha values
                                        float scaledAlpha = Convert.ToSingle(scaledPixelColor.A) / 255f;
                                        float targetAlpha = Convert.ToSingle(targetPixelColor.A) / 255f;
                                        float blendedAlpha = scaledAlpha + targetAlpha * (1 - scaledAlpha);
                                        int blendedR = (int)((scaledPixelColor.R * scaledAlpha) + (targetPixelColor.R * targetAlpha * (1 - scaledAlpha)));
                                        int blendedG = (int)((scaledPixelColor.G * scaledAlpha) + (targetPixelColor.G * targetAlpha * (1 - scaledAlpha)));
                                        int blendedB = (int)((scaledPixelColor.B * scaledAlpha) + (targetPixelColor.B * targetAlpha * (1 - scaledAlpha)));
                                        outputImage.SetPixel(curPixelOutputX, curPixelOutputY, Color.FromArgb((int)(blendedAlpha * 255), blendedR, blendedG, blendedB));
                                    }
                                }

                                // Output the image
                                string outputFileName = Path.Combine(outputFolderPath, filePrefix + (numOfMade + valueToAddToOutputIndexNumber).ToString() + ".png");
                                numOfMade++;
                                outputImage.Save(outputFileName);
                            }
                        }
                    }
                }
            }

            Logger.WriteDetail("Generating item icons from '" + inputImageToCutUp + "' completed.");
        }

        // TODO: Look for solution to image operations.  These image methods are why this project is windows only.
        public static void GenerateResizedImage(string inputFilePath, string outputFilePath, int newWidth, int newHeight)
        {
            // Resize the image to the passed parameters
            Bitmap inputImage = new Bitmap(inputFilePath);
            Bitmap outputImage = new Bitmap(newWidth, newHeight);
            outputImage.SetResolution(outputImage.HorizontalResolution, outputImage.VerticalResolution);
            using (var graphics = Graphics.FromImage(outputImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    Rectangle outputRectangle = new Rectangle(0, 0, newWidth, newHeight);
                    graphics.DrawImage(inputImage, outputRectangle, 0, 0, inputImage.Width, inputImage.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            inputImage.Dispose();
            if (File.Exists(outputFilePath) == true)
                File.Delete(outputFilePath);
            outputImage.Save(outputFilePath);
            outputImage.Dispose();
        }

        public static bool GenerateNewTransparentImage(string outputFilePath, int width, int height)
        {
            // Resize the image to the passed parameters
            Bitmap outputImage = new Bitmap(width, height);
            outputImage.SetResolution(outputImage.HorizontalResolution, outputImage.VerticalResolution);
            using (var graphics = Graphics.FromImage(outputImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
            }
            outputImage.Save(outputFilePath);
            outputImage.Dispose();

            return true;
        }

        public static void GenerateColoredCreatureTexture(string inputTextureFileNameNoExt, string outputTextureFileNameNoExt, ColorRGBA color)
        {
            Logger.WriteDetail("Generating colored texture from '" + inputTextureFileNameNoExt + "' to '" + outputTextureFileNameNoExt + "'");

            // If first run, clear the directory and generate paths
            if (FirstColorTextureGeneration == true)
            {
                // Generate folder paths
                GeneratedFolderPath = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedCreatureTextures");
                InputCharacterTextureFolderPath = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "characters", "Textures");

                // Clear directory
                Logger.WriteDetail("Clearing the generation folder located at '" + GeneratedFolderPath + "'");
                if (Directory.Exists(GeneratedFolderPath))
                    Directory.Delete(GeneratedFolderPath, true);
                Directory.CreateDirectory(GeneratedFolderPath);

                FirstColorTextureGeneration = false;
            }

            // Generate names
            string inputPNGName = inputTextureFileNameNoExt + ".png";
            string inputPNGFullPath = Path.Combine(InputCharacterTextureFolderPath, inputPNGName);
            string outputPNGName = outputTextureFileNameNoExt + ".png";
            string outputPNGFullPath = Path.Combine(GeneratedFolderPath, outputPNGName);
            string outputBLPName = outputTextureFileNameNoExt + ".blp";
            string outputBLPFullPath = Path.Combine(GeneratedFolderPath, outputBLPName);

            // Do existance checks for early exits
            if (File.Exists(outputBLPFullPath) == true)
            {
                Logger.WriteDetail("No need to created '" + outputBLPFullPath + "' as it already exists");
                return;
            }
            if (File.Exists(inputPNGFullPath) == false)
            {
                Logger.WriteError("Failed to create '" + outputBLPFullPath + "' because the input texture '" + inputPNGFullPath + "' did not exist");
                return;
            }
            string blpConverterFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "blpconverter", "BLPConverter.exe");
            if (File.Exists(blpConverterFullPath) == false)
            {
                Logger.WriteError("Failed to create '" + outputBLPFullPath + "' because the BLPConverter could not be found here '" + blpConverterFullPath + "'");
                return;
            }

            // Build a color-changed image
            // TODO: Look for solution to image operations.  These image methods are part of why this project is windows only.
            using (Bitmap inputImage = new Bitmap(inputPNGFullPath))
            {
                using (Bitmap outputImage = new Bitmap(inputImage.Width, inputImage.Height))
                {
                    for (int y = 0; y < outputImage.Height; y++)
                    {
                        for (int x = 0; x < outputImage.Width; x++)
                        {
                            // Get the current pixel's color
                            Color pixelColor = inputImage.GetPixel(x, y);

                            // Apply vertex color as a multiplier to the RGB channels
                            int blendedR = ((pixelColor.R * color.R) / 255);
                            int blendedG = ((pixelColor.G * color.G) / 255);
                            int blendedB = ((pixelColor.B * color.B) / 255);
                            Color blendedColor = Color.FromArgb(pixelColor.A, blendedR, blendedG, blendedB);

                            // Set the blended color to the output image
                            outputImage.SetPixel(x, y, blendedColor);
                        }
                    }

                    // Save the output image
                    outputImage.Save(outputPNGFullPath, ImageFormat.Png);
                }
            }

            // Generate the BLP file
            string args = "/M /FBLP_DXT5 \"" + outputPNGFullPath + "\"";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = args;
            process.StartInfo.FileName = blpConverterFullPath;
            process.Start();
            //process.WaitForExit();
            Logger.WriteDetail(process.StandardOutput.ReadToEnd());
            Console.Title = "EverQuest to WoW Converter";

            Logger.WriteDetail("Generating colored texture completed for '" + outputBLPFullPath + "'");
        }

        public enum ImageSourceType
        {
            Creature,
            Clothing        
        }

        static public void ConvertPNGTexturesToBLP(List<string> fullFileInputPaths, ImageSourceType imageType)
        {
            string formatArg = "/FBLP_DXT5";
            switch (imageType)
            {
                case ImageSourceType.Clothing:
                    {
                        formatArg = "/FBLP_PAL_A8";
                    } break;
                case ImageSourceType.Creature:
                    {
                        formatArg = "/FBLP_DXT5"; // TODO: Look into this, it may be the wrong format
                    } break;
                default:
                    {
                        Logger.WriteError("ConvertPNGTexturesToBLP failed. Unhandled image type of '" + imageType + "'");
                    } break;
            }

            // Output a BLP file for each passed path
            string blpConverterFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "blpconverter", "BLPConverter.exe");
            foreach (string file in fullFileInputPaths)
            {
                // Generate the BLP files
                string args = "/M " + formatArg + " \"" + file + "\"";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.Arguments = args;
                process.StartInfo.FileName = blpConverterFullPath;
                process.Start();
                Logger.WriteDetail(process.StandardOutput.ReadToEnd());
                Console.Title = "EverQuest to WoW Converter";
            }
        }
    }
}
