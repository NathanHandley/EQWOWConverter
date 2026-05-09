//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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
using System.Text;

namespace EQWOWConverter
{
    internal class ImageTool
    {
        private static readonly object PNGtoBLPLock = new object();

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

        public static (int spriteWidth, int spriteHeight) GetWidthAndHeightOfImage(string imageFullPath)
        {
            using (Bitmap image = new Bitmap(imageFullPath))
            {
                int frameWidth = image.Width;
                int frameHeight = image.Height;
                return (frameWidth, frameHeight);
            }
        }

        public static (int sideLength, int columns, int rows) CalculateSpriteSheetLayout(int spriteWidth, int spriteHeight, int numFrames, int minSize, int frameRepeat)
        {
            if (frameRepeat < 1) frameRepeat = 1;

            int effectiveFrames = numFrames * frameRepeat;

            int side = minSize;
            while (true)
            {
                int cols = side / spriteWidth;
                int rows = side / spriteHeight;

                if (cols > 0 && rows > 0)
                {
                    long totalCells = (long)cols * rows;
                    if (totalCells >= effectiveFrames)
                        return (side, cols, rows);
                }

                side *= 2;

                // I don't think sprites can be larger than this for WoW
                if (side > 2048)
                {
                    Logger.WriteError("Failed to calculate sprite sheet layout for spriteWidth ", spriteWidth.ToString(), ", spriteHeight ", spriteHeight.ToString(), ", numFrames ", numFrames.ToString());
                    return (0, 0, 0);
                }
            }
        }

        public static void GenerateSpriteSheetForSpriteChain(List<string> inputSpriteChain, string inputFolderName, string outputFolderName, string outputFileNameWithExt, int frameRepeat )
        {
            Logger.WriteDebug("Generating sprite sheet named '", outputFileNameWithExt, "'");

            // Get dimensions from first image
            string imageFullPath = Path.Combine(inputFolderName, inputSpriteChain[0] + ".png");
            var (frameWidth, frameHeight) = GetWidthAndHeightOfImage(imageFullPath);

            int numFrames = inputSpriteChain.Count;

            if (frameRepeat < 1) frameRepeat = 1;

            // Calculate layout (ensure min size to be 256)
            var (side, cols, rows) = CalculateSpriteSheetLayout(frameWidth, frameHeight, numFrames, 256, frameRepeat);

            // Create the output sprite sheet (always square)
            using (Bitmap outputImage = new Bitmap(side, side))
            {
                int curInputImageIndex = 0;
                int repeatCounter = 0;

                for (int yFrame = 0; yFrame < rows; yFrame++)
                {
                    for (int xFrame = 0; xFrame < cols; xFrame++)
                    {
                        string inputImageFileName = Path.Combine(inputFolderName, inputSpriteChain[curInputImageIndex] + ".png");

                        using (Bitmap inputImage = new Bitmap(inputImageFileName))
                        {
                            for (int inputYPos = 0; inputYPos < frameHeight; inputYPos++)
                            {
                                int yOutputPosition = (yFrame * frameHeight) + inputYPos;
                                for (int inputXPos = 0; inputXPos < frameWidth; inputXPos++)
                                {
                                    int xOutputPosition = (xFrame * frameWidth) + inputXPos;
                                    outputImage.SetPixel(xOutputPosition, yOutputPosition, inputImage.GetPixel(inputXPos, inputYPos));
                                }
                            }
                        }

                        // Handle frame repeating
                        repeatCounter++;
                        if (repeatCounter >= frameRepeat)
                        {
                            // Cycle through the input frames
                            repeatCounter = 0;
                            curInputImageIndex = (curInputImageIndex + 1) % numFrames;
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
            Particle,
            InGameMap,
            StaticObject
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
            // Make sure the tool is there
            string blpConverterFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "blpconverter", "BLPConverter.exe");
            if (File.Exists(blpConverterFullPath) == false)
            {
                Logger.WriteError("Failed to convert images files. '" + blpConverterFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
                return;
            }

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
                case ImageAssociationType.StaticObject:
                    {
                        formatArg = "/FBLP_DXT5";
                    } break;
                case ImageAssociationType.InGameMap:
                    {
                        formatArg = "/FBLP_DXT1_A1";
                    } break;
                default:
                    {
                        Logger.WriteError("ConvertPNGTexturesToBLP error. Unhandled image type of '" + imageType + "'");
                    } break;
            }

            bool moreToProcess = true;
            while (moreToProcess)
            {
                // Create the batch
                List<string> fileNameBatch = new List<string>();
                lock (PNGtoBLPLock)
                {
                    if (fullFileInputPaths.Count == 0)
                    {
                        moreToProcess = false;
                        continue;
                    }
                    else
                    {
                        int batchSize = Math.Min(Configuration.GENERATE_BLPCONVERTBATCHSIZE, fullFileInputPaths.Count);
                        fileNameBatch = fullFileInputPaths.Take(batchSize).ToList();
                        fullFileInputPaths.RemoveRange(0, batchSize);
                    }
                }

                // Convert to a parameter
                StringBuilder curFileArgListSB = new StringBuilder();
                for (int i = 0; i < fileNameBatch.Count; i++)
                {
                    string curFile = fileNameBatch[i];
                    curFileArgListSB.Append(" \"");
                    curFileArgListSB.Append(curFile);
                    curFileArgListSB.Append("\"");
                }

                // Convert them
                Logger.WriteDebug("Converting png files '" + curFileArgListSB.ToString() + "'");
                string args = string.Concat("/M ", formatArg, " ", curFileArgListSB.ToString());
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.Arguments = args;
                process.StartInfo.FileName = blpConverterFullPath;
                process.Start();
                //process.WaitForExit();
                Logger.WriteDebug(process.StandardOutput.ReadToEnd());
                Console.Title = "EverQuest to WoW Converter";
                curFileArgListSB.Clear();
                if (progressionCounter != null)
                    progressionCounter.Write(1);
            }
        }

        public static void SplitMapImageInto12Segments(string inputFilePath, string outputFolder, out List<string> outputImageFullPaths)
        {
            outputImageFullPaths = new List<string>();
            using (Bitmap inputImage = new Bitmap(inputFilePath))
            {
                string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                int outputWidth = 256;
                int outputHeight = 256;
                int fileNumber = 1;

                // Loop through 3 rows
                for (int row = 0; row < 3; row++)
                {
                    // Loop through 4 columns
                    for (int col = 0; col < 4; col++)
                    {
                        // Calculate source rectangle
                        Rectangle sourceRect = new Rectangle(col * outputWidth, row * outputHeight, outputWidth, outputHeight);

                        // Create new bitmap for output
                        using (Bitmap outputImage = new Bitmap(outputWidth, outputHeight))
                        {
                            using (Graphics graphics = Graphics.FromImage(outputImage))
                            {
                                graphics.DrawImage(inputImage, new Rectangle(0, 0, outputWidth, outputHeight), sourceRect, GraphicsUnit.Pixel);
                            }

                            // Generate and save
                            string outputPath = Path.Combine( outputFolder, string.Concat(fileName, fileNumber, ".png"));
                            outputImage.Save(outputPath, ImageFormat.Png);
                            fileNumber++;
                            outputImageFullPaths.Add(outputPath);
                        }
                    }
                }
            }
        }
    }
}
