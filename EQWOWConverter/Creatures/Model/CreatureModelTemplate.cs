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

using EQWOWConverter.ObjectModels.Properties;
using EQWOWConverter.ObjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EQWOWConverter.WOWFiles;
using System.Reflection;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace EQWOWConverter.Creatures
{
    internal class CreatureModelTemplate
    {
        private static Dictionary<int, List<CreatureModelVariation>> AllModelVariationsByRaceID = new Dictionary<int, List<CreatureModelVariation>>();

        public CreatureRace Race;

        public CreatureModelTemplate(CreatureRace creatureRace)
        {
            Race = creatureRace;
        }

        public void CreateModelFiles()
        {
            Logger.WriteDetail("For creature template '" + Race.Name + "', creating the object files");

            // Make sure the source folder path exists
            string eqExportsConditionedPath = Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER;
            string charactersFolderRoot = Path.Combine(eqExportsConditionedPath, "characters");
            if (Directory.Exists(charactersFolderRoot) == false)
            {
                Logger.WriteError("Can not read in EQ Creatures (skeletal objects) data, because folder did not exist at '" + charactersFolderRoot + "'");
                return;
            }

            // Base paths
            string wowExportPath = Configuration.CONFIG_PATH_EXPORT_FOLDER;
            string outputObjectFolderName = GetCreatureModelFolderName();
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            string exportAnimatedObjectsFolder = Path.Combine(exportMPQRootFolder, "Creature", "Everquest");
            string relativeMPQPath = Path.Combine("Creature", "Everquest", outputObjectFolderName);
            string outputFullMPQPath = Path.Combine(exportAnimatedObjectsFolder, outputObjectFolderName);
            string inputObjectTextureFolder = Path.Combine(charactersFolderRoot, "Textures");

            // Make a model for every variation
            List<CreatureModelVariation> modelVariations = GetModelVariationsForRace(Race.ID);
            foreach(CreatureModelVariation modelVariation in modelVariations)
            {
                // Skip everything except bear
                //if (Race.ID != 43 || modelVariation.HelmTextureIndex != 1 || modelVariation.TextureIndex != 1)
                //    continue;

                // Get the skeleton names
                string skeletonName = Race.GetSkeletonNameForGender(modelVariation.GenderType);

                // Create the variation object
                ObjectModelProperties objectProperties = ObjectModelProperties.GetObjectPropertiesForObject(skeletonName);
                ObjectModel curObject = new ObjectModel(skeletonName, objectProperties, ObjectModelType.Skeletal, Race.ModelScale);
                curObject.LoadAnimateEQObjectFromFile(charactersFolderRoot, this, modelVariation);
                curObject.Name = Race.Name + " " + modelVariation.GenerateFileName(skeletonName);

                // Set fidget count for M2
                CreatureRaceSounds creatureRaceSounds = CreatureRaceSounds.GetSoundsByRaceIDAndGender(Race.ID, modelVariation.GenderType);
                if (creatureRaceSounds.SoundIdle2Name.ToLower() != "null24.wav")
                    curObject.NumOfFidgetSounds = 2;
                else if (creatureRaceSounds.SoundIdle1Name.ToLower() != "null24.wav")
                    curObject.NumOfFidgetSounds = 1;

                // Create the M2 and Skin
                M2 objectM2 = new M2(curObject, relativeMPQPath);
                objectM2.WriteToDisk(modelVariation.GenerateFileName(skeletonName), outputFullMPQPath);

                // Place the related textures
                foreach (ObjectModelTexture texture in curObject.ModelTextures)
                {
                    string inputTextureName = Path.Combine(inputObjectTextureFolder, texture.TextureName + ".blp");
                    string outputTextureName = Path.Combine(outputFullMPQPath, texture.TextureName + ".blp");
                    if (Path.Exists(inputTextureName) == false)
                    {
                        Logger.WriteError("- [" + curObject.Name + "]: Error Texture named '" + texture.TextureName + ".blp' not found.  Did you run blpconverter?");
                        return;
                    }
                    File.Copy(inputTextureName, outputTextureName, true);
                    Logger.WriteDetail("- [" + curObject.Name + "]: Texture named '" + texture.TextureName + ".blp' copied");
                }
            }

            Logger.WriteDetail("For creature template '" + Race.Name + "', completed creating the object files");
        }

        public string GetCreatureModelFolderName()
        {
            string raceName = Race.Name.Trim();
            raceName = raceName.Replace("/", "And");
            raceName = raceName.Replace(" ", "");
            raceName = raceName.Replace("-", "");

            string raceID = string.Empty;
            if (Race.ID < 10)
                raceID = "00" + Race.ID.ToString();
            else if (Race.ID < 100)
                raceID = "0" + Race.ID.ToString();
            else
                raceID = Race.ID.ToString();

            return raceID + raceName;
        }

        public static List<CreatureModelVariation> GetModelVariationsForRace(int raceID)
        {
            if (AllModelVariationsByRaceID.Count == 0)
                PopulateAllModelVariations();

            if (AllModelVariationsByRaceID.ContainsKey(raceID))
                return AllModelVariationsByRaceID[raceID];
            else
                return new List<CreatureModelVariation>();
        }

        private static void PopulateAllModelVariations()
        {
            AllModelVariationsByRaceID.Clear();

            // Load in the file
            string variationsDataFileName = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureVariations.csv");
            Logger.WriteDetail("Populating Creature Variations list via file '" + variationsDataFileName + "'");
            string inputData = File.ReadAllText(variationsDataFileName);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("Creature Variation list via file '" + variationsDataFileName + "' did not have enough lines");
                return;
            }

            // Load all of the data
            bool headerRow = true;
            foreach (string row in inputRows)
            {
                // Handle first row
                if (headerRow == true)
                {
                    headerRow = false;
                    continue;
                }

                // Skip blank rows
                if (row.Trim().Length == 0)
                    continue;

                // Load the row
                string[] rowBlocks = row.Split(",");
                int raceID = int.Parse(rowBlocks[0]);
                CreatureModelVariation newVariation = new CreatureModelVariation();
                int genderID = int.Parse(rowBlocks[1]);
                switch (genderID)
                {
                    case 0: newVariation.GenderType = CreatureGenderType.Male; break;
                    case 1: newVariation.GenderType = CreatureGenderType.Female; break;
                    default: newVariation.GenderType = CreatureGenderType.Neutral; break;
                }
                newVariation.TextureIndex = int.Parse(rowBlocks[2]);
                newVariation.HelmTextureIndex = int.Parse(rowBlocks[3]);
                if (AllModelVariationsByRaceID.ContainsKey(raceID) == false)
                    AllModelVariationsByRaceID.Add(raceID, new List<CreatureModelVariation>());
                AllModelVariationsByRaceID[raceID].Add(newVariation);
            }

            Logger.WriteDetail("Populating Creature Variations list via file complete.");
        }
    }
}
