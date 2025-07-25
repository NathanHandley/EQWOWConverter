﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

using EQWOWConverter.ObjectModels.Properties;
using EQWOWConverter.ObjectModels;
using System.Text;
using EQWOWConverter.WOWFiles;
namespace EQWOWConverter.Creatures
{
    internal class CreatureModelTemplate
    {
        public static Dictionary<int, List<CreatureModelTemplate>> AllTemplatesByRaceID = new Dictionary<int, List<CreatureModelTemplate>>();

        public CreatureRace Race;
        public CreatureGenderType GenderType = CreatureGenderType.Neutral;
        public int TextureIndex = 0;
        public int HelmTextureIndex = 0;
        public int FaceIndex = 0;
        public int ColorTintID = 0;
        public CreatureTemplateColorTint? ColorTint = null;

        private static int CURRENT_DBCID_CREATUREMODELDATAID = Configuration.DBCID_CREATUREMODELDATA_ID_START;
        private static int CURRENT_DBCID_CREATUREDISPLAYINFOID = Configuration.DBCID_CREATUREDISPLAYINFO_ID_START;
        private static int CURRENT_DBCID_CREATURESOUNDDATAID = Configuration.DBCID_CREATURESOUNDDATA_ID_START;
        public int DBCCreatureModelDataID;
        public int DBCCreatureDisplayID;
        public int DBCCreatureSoundDataID;
        private static readonly object CreatureLock = new object();

        public CreatureModelTemplate(CreatureRace creatureRace, CreatureTemplate creatureTemplate)
        {
            lock(CreatureLock)
            {
                DBCCreatureModelDataID = CURRENT_DBCID_CREATUREMODELDATAID;
                CURRENT_DBCID_CREATUREMODELDATAID++;
                DBCCreatureDisplayID = CURRENT_DBCID_CREATUREDISPLAYINFOID;
                CURRENT_DBCID_CREATUREDISPLAYINFOID++;
                DBCCreatureSoundDataID = CURRENT_DBCID_CREATURESOUNDDATAID;
                CURRENT_DBCID_CREATURESOUNDDATAID++;
            }

            Race = creatureRace;
            GenderType = creatureTemplate.GenderType;
            TextureIndex = creatureTemplate.TextureID;
            HelmTextureIndex = creatureTemplate.HelmTextureID;
            FaceIndex = creatureTemplate.FaceID;

            if (creatureTemplate.ColorTintID != 0)
            {
                Dictionary<int, CreatureTemplateColorTint> colorTints = CreatureTemplateColorTint.GetCreatureTemplateColorTints();
                if (colorTints.ContainsKey(creatureTemplate.ColorTintID) == false)
                    Logger.WriteError("No color tint ID of '" + creatureTemplate.ColorTintID + "' found");
                else
                {
                    ColorTint = colorTints[creatureTemplate.ColorTintID];
                    ColorTintID = creatureTemplate.ColorTintID;
                }
            }
        }

        public static void CreateAllCreatureModelTemplates(List<CreatureTemplate> creatureTemplates)
        {
            // Clear the old list
            AllTemplatesByRaceID.Clear();

            // Generate model templates in response to creature templates
            foreach(CreatureTemplate creatureTemplate in creatureTemplates)
            {
                // They are grouped by race
                if (AllTemplatesByRaceID.ContainsKey(creatureTemplate.Race.ID) == false)
                    AllTemplatesByRaceID.Add(creatureTemplate.Race.ID, new List<CreatureModelTemplate>());

                // Create for any templates
                CreatureModelTemplate? existingModel = null;
                foreach(CreatureModelTemplate modelTemplate in AllTemplatesByRaceID[creatureTemplate.Race.ID])
                {
                    // Skip if this model template already exists
                    if (modelTemplate.GenderType == creatureTemplate.GenderType && 
                        modelTemplate.HelmTextureIndex == creatureTemplate.HelmTextureID &&
                        modelTemplate.TextureIndex == creatureTemplate.TextureID &&
                        modelTemplate.FaceIndex == creatureTemplate.FaceID &&
                        modelTemplate.ColorTintID == creatureTemplate.ColorTintID)
                    {
                        existingModel = modelTemplate;
                        break;
                    }
                }
                creatureTemplate.ModelTemplate = existingModel;
                if (existingModel != null)
                    continue;

                // Create the new template
                CreatureModelTemplate newModelTemplate = new CreatureModelTemplate(creatureTemplate.Race, creatureTemplate);
                creatureTemplate.ModelTemplate = newModelTemplate;
                AllTemplatesByRaceID[creatureTemplate.Race.ID].Add(newModelTemplate);
            }
        }

        public void CreateModelFiles(string charactersFolderRoot, string inputObjectTextureFolder, string exportAnimatedObjectsFolder,
            string generatedTexturesFolderPath)
        {
            string objectName = String.Concat(Race.Name, " ", GenerateFileName());
            Logger.WriteDebug(String.Concat("For creature template '", objectName , "', creating the object files"));

            // Get the skeleton name
            string skeletonName = Race.SkeletonName;

            // Only operate if there is a skeleton name
            if (skeletonName.Trim().Length == 0)
            {
                Logger.WriteDebug("Skipping creature template due to no skeleton name");
                return;
            }

            // Base paths
            string outputObjectFolderName = GetCreatureModelFolderName();        
            string relativeMPQPath = Path.Combine("Creature", "Everquest", outputObjectFolderName);
            string outputFullMPQPath = Path.Combine(exportAnimatedObjectsFolder, outputObjectFolderName);

            // Create folder if it doesn't exist
            if (Directory.Exists(outputFullMPQPath) == false)
                Directory.CreateDirectory(outputFullMPQPath);

            // Load in an object
            float lift = Race.Lift;
            ObjectModelProperties objectProperties = new ObjectModelProperties(ObjectModelProperties.GetObjectPropertiesForObject(skeletonName));
            objectProperties.CreatureModelTemplate = this;
            objectProperties.ModelScalePreWorldScale = Race.ModelScale;
            objectProperties.ModelLiftPreWorldScale = lift;
            ObjectModel curObject = new ObjectModel(skeletonName, objectProperties, ObjectModelType.Creature, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
            curObject.LoadEQObjectFromFile(charactersFolderRoot, skeletonName);
            StringBuilder nameSB = new StringBuilder();
            nameSB.Append(Race.Name);
            nameSB.Append(" ");
            nameSB.Append(GenerateFileName());
            curObject.Name = nameSB.ToString();

            // Set fidget count for M2
            if (Race.SoundIdle2Name.ToLower() != "null24.wav")
                curObject.NumOfFidgetSounds = 2;
            else if (Race.SoundIdle1Name.ToLower() != "null24.wav")
                curObject.NumOfFidgetSounds = 1;

            // Create the M2 and Skin
            M2 objectM2 = new M2(curObject, relativeMPQPath);
            objectM2.WriteToDisk(GenerateFileName(), outputFullMPQPath);

            // Place the related textures
            foreach (ObjectModelTexture texture in curObject.ModelTextures)
            {
                string inputTextureNameInCharTextureFolder = Path.Combine(inputObjectTextureFolder, texture.TextureName + ".blp");
                string inputTextureNameInGeneratedTextureFolder = Path.Combine(generatedTexturesFolderPath, texture.TextureName + ".blp");
                string outputTextureName = Path.Combine(outputFullMPQPath, texture.TextureName + ".blp");
                if (Path.Exists(inputTextureNameInCharTextureFolder) == true)
                {
                    FileTool.CopyFile(inputTextureNameInCharTextureFolder, outputTextureName);
                    Logger.WriteDebug(String.Concat("- [", curObject.Name, "]: Texture named '", texture.TextureName, ".blp' copied"));
                }
                else if (Path.Exists(inputTextureNameInGeneratedTextureFolder) == true)
                {
                    FileTool.CopyFile(inputTextureNameInGeneratedTextureFolder, outputTextureName);
                    Logger.WriteDebug("- [" + curObject.Name + "]: Texture named '" + texture.TextureName + ".blp' copied");
                }
                else
                {
                    Logger.WriteError("- [" + curObject.Name + "]: Error Texture named '" + texture.TextureName + ".blp' not found.  Did you run blpconverter?");
                    return;
                }
            }

            Logger.WriteDebug(String.Concat("For creature template '", objectName, "', completed creating the object files"));
        }

        public string GenerateFileName()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Race.SkeletonName);
            switch (GenderType)
            {
                case CreatureGenderType.Male: sb.Append("_M_"); break;
                case CreatureGenderType.Female: sb.Append("_F_"); break;
                default: sb.Append("_N_"); break;
            }
            sb.Append("h" + HelmTextureIndex);
            sb.Append("t" + TextureIndex);
            sb.Append("f" + FaceIndex);
            sb.Append("c" + ColorTintID);
            return sb.ToString();
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
    }
}
