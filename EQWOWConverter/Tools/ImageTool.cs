﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter
{
    internal class ImageTool
    {
        private static bool FirstColorTextureGeneration = true;
        private static string GeneratedFolderPath = string.Empty;
        private static string InputCharacterTextureFolderPath = string.Empty;
        private static string BLPConverterFullPath = string.Empty;

        public static void GenerateItemIconImagesFromFile(string inputImageToCutUp, int numberOfIconImagesToMake, int startingIconImageIndex, string outputFolderPath)
        {
            Logger.WriteDetail("Generating item icons from '" + inputImageToCutUp +"' started...");

            // Read in the backdrop data first
            List<List<Color>> backdropPixels = new List<List<Color>>();
            string backdropTextureFullPath = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "CustomTextures", "item", "ItemIconBackdrop.png");
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

            using (Bitmap inputIconsMosaic = new Bitmap(inputImageToCutUp))
            {
                for (int i = 0; i < numberOfIconImagesToMake; i++)
                {
                    // Calculate the start position for this image
                    int inputPixelStartX = (i / 12) * 40;
                    int inputPixelStartY = (i % 12) * 40;

                    // Create it
                    using (Bitmap outputImage = new Bitmap(64, 64))
                    {
                        for (int y = 0; y < 64; y++)
                        {
                            for (int x = 0; x < 64; x++)
                            {
                                // Always output the backdrop
                                outputImage.SetPixel(x, y, backdropPixels[x][y]);

                                // Only copy the icon images if it's the center block
                                if (x >= 11 && y >= 11 && x <= 50 && y <= 50)
                                {
                                    int curInputPixelStartX = inputPixelStartX + (x - 11);
                                    int curInputPixelStartY = inputPixelStartY + (y - 11);

                                    // Get the current pixel's color
                                    Color pixelColor = inputIconsMosaic.GetPixel(curInputPixelStartX, curInputPixelStartY);

                                    // Output only non-alpha pixels
                                    if (pixelColor.A > 0)
                                        outputImage.SetPixel(x, y, pixelColor);
                                }
                            }
                        }
                        string outputFileName = Path.Combine(outputFolderPath, "INV_EQ_" + (i + startingIconImageIndex).ToString() + ".png");
                        outputImage.Save(outputFileName);
                    }
                }
            }

            Logger.WriteDetail("Generating item icons from '" + inputImageToCutUp + "' completed.");
        }

        public static void GenerateColoredCreatureTexture(string inputTextureFileNameNoExt, string outputTextureFileNameNoExt, ColorRGBA color)
        {
            Logger.WriteDetail("Generating colored texture from '" + inputTextureFileNameNoExt + "' to '" + outputTextureFileNameNoExt + "'");

            // If first run, clear the directory and generate paths
            if (FirstColorTextureGeneration == true)
            {
                // Generate folder paths
                GeneratedFolderPath = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "GeneratedCreatureTextures");
                InputCharacterTextureFolderPath = Path.Combine(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER, "characters", "Textures");
                BLPConverterFullPath = Path.Combine(Configuration.CONFIG_PATH_TOOLS_FOLDER, "blpconverter", "BLPConverter.exe");

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
            if (File.Exists(BLPConverterFullPath) == false)
            {
                Logger.WriteError("Failed to create '" + outputBLPFullPath + "' because the BLPConverter could not be found here '" + BLPConverterFullPath + "'");
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
            process.StartInfo.FileName = BLPConverterFullPath;
            process.Start();
            //process.WaitForExit();
            Logger.WriteDetail(process.StandardOutput.ReadToEnd());
            Console.Title = "EverQuest to WoW Converter";

            Logger.WriteDetail("Generating colored texture completed for '" + outputBLPFullPath + "'");
        }
    }
}