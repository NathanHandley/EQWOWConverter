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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.EQFiles
{
    internal class EQMaterialList
    {
        public List<Material> Materials = new List<Material>();

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDetail(" - Reading EQ Material List Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find material list file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load the core data
            string inputData = File.ReadAllText(fileFullPath);
            string[] inputRows = inputData.Split(Environment.NewLine);
            foreach (string inputRow in inputRows)
            {
                // Nothing for blank lines
                if (inputRow.Length == 0)
                    continue;

                // # = comment
                else if (inputRow.StartsWith("#"))
                    continue;

                string[] blocks = inputRow.Split(",");
                if (blocks.Length < 3)
                {
                    Logger.WriteError("Material data must be 3+ components");
                    continue;
                }

                // Always Index first
                UInt32 index = uint.Parse(blocks[0]);

                // Next is material name and the array of textures
                string[] nameAndTextureParts = blocks[1].Split(':');
                string name = nameAndTextureParts[0];

                // If the first part has an underscore, it tells what type of material it is
                MaterialType materialType = MaterialType.Diffuse;
                if (nameAndTextureParts[0].Contains("_"))
                {
                    string[] nameAttributeSplit = nameAndTextureParts[0].Split('_');
                    switch (nameAttributeSplit[0])
                    {
                        case "d": materialType = MaterialType.Diffuse; break;
                        case "i": materialType = MaterialType.Invisible; break;
                        case "b": materialType = MaterialType.Boundary; break;
                        case "t25": materialType = MaterialType.Transparent25Percent; break;
                        case "t50": materialType = MaterialType.Transparent50Percent; break;
                        case "t75": materialType = MaterialType.Transparent75Percent; break;
                        case "ta": materialType = MaterialType.TransparentAdditive; break;
                        case "tau": materialType = MaterialType.TransparentAdditiveUnlit; break;
                        case "tm": materialType = MaterialType.TransparentMasked; break;
                        case "ds": materialType = MaterialType.DiffuseSkydome; break;
                        case "ts": materialType = MaterialType.TransparentSkydome; break;
                        case "taus": materialType = MaterialType.Boundary; break;
                        default:
                            {
                                Logger.WriteError("Material had a name of " + nameAndTextureParts[0] + " which doesn't map to a type.  Setting to diffuse.");
                                materialType = MaterialType.Diffuse;
                            }
                            break;
                    }
                }

                // Any following segments from : are the texture names
                List<string> sourceTextureNameArray = new List<string>();
                if (nameAndTextureParts.Length > 1)
                    for (int i = 1; i < nameAndTextureParts.Length; i++)
                    {
                        // Some files have an extra section after a semicolon, so control for that
                        string[] subparts = nameAndTextureParts[i].Split(';');
                        sourceTextureNameArray.Add(subparts[0]);
                    }

                // Animation Delay
                UInt32 animationDelayInMS = uint.Parse(blocks[2]);

                // If there are more than 4, then the next two are the dimensions
                int sourceTextureWidth = 0;
                int sourceTextureHeight = 0;
                if (blocks.Length >= 4)
                {   
                    sourceTextureWidth = int.Parse(blocks[3]);
                    sourceTextureHeight = int.Parse(blocks[4]);
                }

                // If there's a 6th, it's the combined texture name when it's animated by texture transformation
                string combinedTransformAnimationTextureName = string.Empty;
                if (blocks.Length == 6)
                    combinedTransformAnimationTextureName = blocks[5];

                // Create and add it
                Material newMaterial = new Material(name, index, materialType, sourceTextureNameArray, animationDelayInMS, 
                    sourceTextureWidth, sourceTextureHeight, combinedTransformAnimationTextureName);
                Materials.Add(newMaterial);
            }

            Logger.WriteDetail(" - Done reading EQ Material List Data from '" + fileFullPath + "'");
            return true;
        }
    }
}
