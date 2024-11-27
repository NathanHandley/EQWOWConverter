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

namespace EQWOWConverter.Creatures
{
    internal class CreatureModelTemplate
    {
        public CreatureRace Race;
        public List<CreatureModelVariation> ModelVariations = new List<CreatureModelVariation>();

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

            // Build for all 3 gender versions
            if (Race.MaleSkeletonName != string.Empty)
                CreateModelFilesForGender(CreatureGenderType.Male, Race.MaleSkeletonName);
            if (Race.FemaleSkeletonName != string.Empty)
                CreateModelFilesForGender(CreatureGenderType.Female, Race.FemaleSkeletonName);
            if (Race.NeutralSkeletonName != string.Empty)
                CreateModelFilesForGender(CreatureGenderType.Neutral, Race.NeutralSkeletonName);
            Logger.WriteDetail("For creature template '" + Race.Name + "', completed creating the object files");
        }

        private void CreateModelFilesForGender(CreatureGenderType gender, string skeletonName)
        {
            Logger.WriteDetail("For creature template '" + Race.Name + "' gender '" + gender.ToString() + "', creating the object files");

            // Base paths
            string eqExportsConditionedPath = Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER;
            string wowExportPath = Configuration.CONFIG_PATH_EXPORT_FOLDER;
            string charactersFolderRoot = Path.Combine(eqExportsConditionedPath, "characters");
            string outputObjectFolderName = GetCreatureModelFolderName();
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            string exportAnimatedObjectsFolder = Path.Combine(exportMPQRootFolder, "Creature", "Everquest");
            string relativeMPQPath = Path.Combine("Creature", "Everquest", outputObjectFolderName);
            string outputFullMPQPath = Path.Combine(exportAnimatedObjectsFolder, outputObjectFolderName);
            string inputObjectTextureFolder = Path.Combine(charactersFolderRoot, "Textures");

            // Init the root object
            ObjectModelProperties objectProperties = ObjectModelProperties.GetObjectPropertiesForObject(skeletonName);
            ObjectModel rootObject = new ObjectModel(skeletonName, objectProperties, ObjectModelType.Skeletal);
            rootObject.LoadEQObjectData(charactersFolderRoot);

            // For now, create variations based on texture indexes
            for (int variationIndex = 0; variationIndex < rootObject.EQObjectModelData.MaterialsByTextureVariation.Count; variationIndex++)
                AddVariation(gender, 0, 0, variationIndex, variationIndex);

            // Create for any matching model variations
            for (int variationIndex = 0; variationIndex < ModelVariations.Count; variationIndex++)
            {
                CreatureModelVariation modelVariation = ModelVariations[variationIndex];
                if (modelVariation.GenderType != gender)
                    continue;

                // Get any animation supplement
                string animationSupplement = Race.GetAnimationSupplementNameForGender(gender);

                // Create the variation object
                ObjectModel curObject = new ObjectModel(skeletonName, objectProperties, ObjectModelType.Skeletal, Race.ModelScale);
                curObject.LoadEQObjectData(charactersFolderRoot, animationSupplement);

                // Convert to a WoW object
                float addedLift = Race.LiftMaleAndNeutral;
                if (gender == CreatureGenderType.Female)
                    addedLift = Race.LiftFemale;
                addedLift *= Configuration.CONFIG_GENERATE_WORLD_SCALE; // Modify scale by world scale
                curObject.PopulateObjectModelFromEQObjectModelData(modelVariation.BodyTextureIndex, addedLift);

                // Set fidget count for M2
                CreatureRaceSounds creatureRaceSounds = CreatureRaceSounds.GetSoundsByRaceIDAndGender(Race.ID, gender);
                if (creatureRaceSounds.SoundIdle2Name.ToLower() != "null24.wav")
                    curObject.NumOfFidgetSounds = 2;
                else if (creatureRaceSounds.SoundIdle1Name.ToLower() != "null24.wav")
                    curObject.NumOfFidgetSounds = 1;

                // Create the M2 and Skin
                M2 objectM2 = new M2(curObject, relativeMPQPath);
                string outputM2Name = skeletonName + "v" + variationIndex;
                objectM2.WriteToDisk(outputM2Name, outputFullMPQPath);
                modelVariation.ModelFileName = outputM2Name;

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

            Logger.WriteDetail("For creature template '" + Race.Name + "' gender '" + gender.ToString() + "', completed creating the object files");
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

        public void AddVariation(CreatureGenderType gender, int bodyModelIndex, int headModelIndex, int bodyTextureIndex, int headTextureIndex)
        {
            CreatureModelVariation modelVariation = new CreatureModelVariation();
            modelVariation.GenderType = gender;
            modelVariation.BodyModelIndex = bodyModelIndex;
            modelVariation.HeadModelIndex = headModelIndex;
            modelVariation.BodyTextureIndex = bodyTextureIndex;
            modelVariation.HeadTextureIndex = headTextureIndex;
            ModelVariations.Add(modelVariation);
        }
    }
}
