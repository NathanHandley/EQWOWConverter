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
        public enum IconSeriesDirection
        {
            AlongX,
            AlongY
        }

        public static void GenerateItemIconImagesFromFile(string inputImageToCutUp, int numberOfIconImagesToMake, int valueToAddToOutputIndexNumber, string outputFolderPath,
            int iconInitialWidth, int iconInitialHeight, IconSeriesDirection iconSeriesDirection, int iconsInSeries, string filePrefix, int startIndex)
        {
            Logger.WriteDebug("Generating item icons from '" + inputImageToCutUp +"' started...");

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

            Logger.WriteDebug("Generating item icons from '" + inputImageToCutUp + "' completed.");
        }

        public static void GenerateSpriteSheetForSpriteChain(List<string> inputSpriteChain, string inputFolderName, string outputFolderName, string outputFileNameWithExt)
        {
            Logger.WriteDebug("Generating sprite sheet named '", outputFileNameWithExt, "'");

            // The first file will give the base dimensions to use
            string imageFullPath = Path.Combine(inputFolderName, inputSpriteChain[0] + ".png");
            Bitmap image = new Bitmap(imageFullPath);
            int inputImageHeight = image.Height;
            int inputImageWidth = image.Width;
            image.Dispose();

            // Create the output image, reading in the images in chain and making copies wherever neccessary
            // There will always be 1, 2 or 8 in a chain, which we need to bring up to 64 (8x8)
            using (Bitmap outputImage = new Bitmap(inputImageHeight * 8, inputImageWidth * 8))
            {
                int curInputImageIndex = 0;
                int remainingInputImageCopies = 4; // And to slow down the animation speed to be more EQ-like, each frame is quadrupled
                for (int yFrame = 0; yFrame < 8; yFrame++)
                {
                    for (int xFrame = 0; xFrame < 8; xFrame++)
                    {
                        // Open the next image and copy in the pixels
                        string inputImageFileName = Path.Combine(inputFolderName, inputSpriteChain[curInputImageIndex] + ".png");
                        using (Bitmap inputImage = new Bitmap(inputImageFileName))
                        {
                            for (int inputYPos = 0; inputYPos < inputImageHeight; inputYPos++)
                            {
                                int yOutputPosition = (yFrame * inputImageHeight) + inputYPos;
                                for (int inputXPos = 0; inputXPos < inputImageWidth; inputXPos++)
                                {
                                    int xOutputPosition = (xFrame * inputImageWidth) + inputXPos;
                                    outputImage.SetPixel(xOutputPosition, yOutputPosition, inputImage.GetPixel(inputXPos, inputYPos));
                                }
                            }
                        }
                        remainingInputImageCopies--;
                        if (remainingInputImageCopies == 0)
                        {
                            remainingInputImageCopies = 4;
                            curInputImageIndex++;

                            // When the number of frames has cycled, just start over
                            if (curInputImageIndex >= inputSpriteChain.Count)
                                curInputImageIndex = 0;
                        }
                    }
                }

                string outputFileFullPath = Path.Combine(outputFolderName, outputFileNameWithExt);
                outputImage.Save(outputFileFullPath);
            }

            Logger.WriteDebug("Generating sprite sheet complete.");
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

        public enum ImageAssociationType
        {
            Creature,
            Clothing,
            SpellParticle
        }

        public static void GenerateColoredTintedTexture(string inputTextureFolder, string inputTextureFileNameNoExt, string workingDirectory,
            string outputTextureFileNameNoExt, ColorRGBA color, ImageAssociationType imageAssociationType, bool doGenerateBLP)
        {
            Logger.WriteDebug("Generating colored texture from '" + inputTextureFileNameNoExt + "' to '" + outputTextureFileNameNoExt + "'");

            // Generate names
            string inputPNGName = inputTextureFileNameNoExt + ".png";
            string inputPNGFullPath = Path.Combine(inputTextureFolder, inputPNGName);
            string outputPNGName = outputTextureFileNameNoExt + ".png";
            string outputPNGFullPath = Path.Combine(workingDirectory, outputPNGName);
            string outputBLPName = outputTextureFileNameNoExt + ".blp";
            string outputBLPFullPath = Path.Combine(workingDirectory, outputBLPName);

            // Do existance checks for early exits
            if (File.Exists(outputBLPFullPath) == true)
            {
                Logger.WriteDebug("No need to created '" + outputBLPFullPath + "' as it already exists");
                return;
            }
            if (File.Exists(inputPNGFullPath) == false)
            {
                Logger.WriteError("Failed to create '" + outputBLPFullPath + "' because the input texture '" + inputPNGFullPath + "' did not exist");
                return;
            }

            // Build a color-changed image
            using (Bitmap inputImage = new Bitmap(inputPNGFullPath))
            using (Bitmap outputImage = new Bitmap(inputImage.Width, inputImage.Height, PixelFormat.Format32bppArgb))
            {
                // Lock the input and output bitmaps bits for faster operation
                BitmapData inputData = inputImage.LockBits(new Rectangle(0, 0, inputImage.Width, inputImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData outputData = outputImage.LockBits(new Rectangle(0, 0, outputImage.Width, outputImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                try
                {
                    unsafe
                    {
                        // Pointers to the pixel data
                        byte* inputPtr = (byte*)inputData.Scan0;
                        byte* outputPtr = (byte*)outputData.Scan0;

                        // Bytes per pixel (4 for ARGB: A, R, G, B)
                        int bytesPerPixel = 4;
                        int height = inputImage.Height;
                        int widthInBytes = inputImage.Width * bytesPerPixel;

                        for (int y = 0; y < height; y++)
                        {
                            byte* currentInputLine = inputPtr + (y * inputData.Stride);
                            byte* currentOutputLine = outputPtr + (y * outputData.Stride);

                            for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                            {
                                // Extract ARGB from input
                                byte blue = currentInputLine[x];
                                byte green = currentInputLine[x + 1];
                                byte red = currentInputLine[x + 2];
                                byte alpha = currentInputLine[x + 3];

                                // Apply blend (multiply RGB by color, preserve alpha)
                                byte blendedR = (byte)((red * color.R) / 255);
                                byte blendedG = (byte)((green * color.G) / 255);
                                byte blendedB = (byte)((blue * color.B) / 255);

                                // Write to output
                                currentOutputLine[x] = blendedB;
                                currentOutputLine[x + 1] = blendedG;
                                currentOutputLine[x + 2] = blendedR;
                                currentOutputLine[x + 3] = alpha;
                            }
                        }
                    }
                }
                finally
                {
                    // Unlock the bits
                    inputImage.UnlockBits(inputData);
                    outputImage.UnlockBits(outputData);
                }

                // Save the output image
                outputImage.Save(outputPNGFullPath, ImageFormat.Png);
            }

            // Generate the BLP file, if needed
            if (doGenerateBLP == true)
                ConvertPNGTexturesToBLP(new List<string>() { outputPNGFullPath }, imageAssociationType);
            Logger.WriteDebug("Generating colored texture completed for '" + outputBLPFullPath + "'");
        }

        public static void ConvertPNGTexturesToBLP(List<string> fullFileInputPaths, ImageAssociationType imageType, LogCounter? progressionCounter = null)
        {
            string formatArg = "/FBLP_DXT5";
            switch (imageType)
            {
                case ImageAssociationType.Clothing:
                    {
                        formatArg = "/FBLP_PAL_A8";
                    } break;
                case ImageAssociationType.Creature:
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
                Logger.WriteDebug(process.StandardOutput.ReadToEnd());
                Console.Title = "EverQuest to WoW Converter";

                if (progressionCounter != null)
                    progressionCounter.Write(1);
            }
        }
    }
}
