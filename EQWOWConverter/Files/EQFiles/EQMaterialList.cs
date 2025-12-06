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

namespace EQWOWConverter.EQFiles
{
    internal class EQMaterialList
    {
        public List<List<Material>> MaterialsByTextureVariation = new List<List<Material>>();

        // Invisible Man (Race 127) has no textures, so need to create blank ones
        public void LoadForInvisibleMan()
        {
            Logger.WriteDebug(" - Creating EQ Material List Data for Invisible Man (IVM)...");
            MaterialsByTextureVariation.Add(new List<Material>());
            MaterialsByTextureVariation[0].Add(new Material("00", "00", 0, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("01", "01", 1, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("02", "02", 2, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("03", "03", 3, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("04", "04", 4, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("05", "05", 5, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("06", "06", 6, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("07", "07", 7, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("08", "08", 8, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("09", "09", 9, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("10", "10", 10, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("11", "11", 11, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("12", "12", 12, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("13", "13", 13, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("14", "14", 14, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("15", "15", 15, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            MaterialsByTextureVariation[0].Add(new Material("16", "16", 16, -1, MaterialType.TransparentAdditive, new List<string> { "clear" }, 0, 256, 256, false));
            Logger.WriteDebug(" - Done creating material list data for IVM");
        }

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDebug(" - Reading EQ Material List Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find material list file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load the core data
            string inputData = FileTool.ReadAllDataFromFile(fileFullPath);
            string[] inputRows = inputData.Split(Environment.NewLine);

            // Read in the number of variations by looking for any textures with more than 1 variation listing
            int variationCount = 0;
            foreach (string inputRow in inputRows)
            {
                // Skip Blank
                if (inputRow.Length == 0)
                    continue;
                // Skip Comments
                else if (inputRow.StartsWith("#"))
                    continue;
                string[] blocks = inputRow.Split(",");
                if (blocks.Length < 3)
                {
                    Logger.WriteError("Material data must be 3+ components");
                    continue;
                }
                UInt32 index = uint.Parse(blocks[0]);
                string[] variationParts = blocks[1].Split(';');
                if (variationParts.Length > variationCount)
                    variationCount = variationParts.Length;
            }

            // Block out the variations
            for (int i = 0; i < variationCount; i++)
            {
                MaterialsByTextureVariation.Add(new List<Material>());
            }

            // Process the data for each variation
            for (int variationIndex = 0; variationIndex < variationCount; ++variationIndex)
            {
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
                    string[] variationParts = blocks[1].Split(';');
                    int workingVariationIndex = 0;
                    if (variationParts.Length > variationIndex)
                    {
                        workingVariationIndex = variationIndex;
                        if (variationParts.Length > 1)
                            Logger.WriteDebug("There are at least '" + variationParts.Length + "' variations");
                    }
                    string[] nameAndTextureParts = variationParts[workingVariationIndex].Split(':');
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
                            if (subparts.Length > 1)
                                Logger.WriteInfo("TODO: Need to handle this before animations will work properly (" + fileFullPath + ")");
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

                    // Create and add it
                    Material newMaterial = new Material(name, name, index, -1, materialType, sourceTextureNameArray, animationDelayInMS,
                        sourceTextureWidth, sourceTextureHeight, false);
                    MaterialsByTextureVariation[variationIndex].Add(newMaterial);
                }
            }

            Logger.WriteDebug(" - Done reading EQ Material List Data from '" + fileFullPath + "'");
            return true;
        }
    }
}
