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
        internal class TemplateMetadata
        {
            public List<string> BodyMeshNames = new List<string>();
            public List<string> HeadMeshNames = new List<string>();
            public int NumberOfTextures = 0;
        }

        public CreatureRace Race;
        public TemplateMetadata MaleTemplateMetadata;
        public TemplateMetadata FemaleTemplateMetadata;
        public TemplateMetadata NeutralTemplateMetadata;
        public List<CreatureModelVariation> ModelVariations;

        public CreatureModelTemplate(CreatureRace creatureRace)
        {
            Race = creatureRace;
            LoadTemplateMetadataForGender(Race.MaleSkeletonName, Race.MaleAnimationSupplementName, out MaleTemplateMetadata);
            LoadTemplateMetadataForGender(Race.FemaleSkeletonName, Race.FemaleAnimationSupplementName, out FemaleTemplateMetadata);
            LoadTemplateMetadataForGender(Race.NeutralSkeletonName, Race.NeutralAnimationSupplementName, out NeutralTemplateMetadata);
            ModelVariations = GetModelVariations();
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
            foreach(CreatureModelVariation modelVariation in ModelVariations)
            {
                // Get the skeleton names
                string animationSupplement = Race.GetAnimationSupplementNameForGender(modelVariation.GenderType);
                string skeletonName = Race.GetSkeletonNameForGender(modelVariation.GenderType);

                // Create the variation object
                ObjectModelProperties objectProperties = ObjectModelProperties.GetObjectPropertiesForObject(skeletonName);
                ObjectModel curObject = new ObjectModel(skeletonName, objectProperties, ObjectModelType.Skeletal, Race.ModelScale);
                curObject.LoadEQObjectData(charactersFolderRoot, modelVariation.GetMeshNames(), animationSupplement);

                // Convert to a WoW object
                float addedLift = Race.LiftMaleAndNeutral;
                if (modelVariation.GenderType == CreatureGenderType.Female)
                    addedLift = Race.LiftFemale;
                addedLift *= Configuration.CONFIG_GENERATE_WORLD_SCALE; // Modify scale by world scale
                curObject.PopulateObjectModelFromEQObjectModelData(modelVariation.BodyTextureIndex, addedLift);

                // Set fidget count for M2
                CreatureRaceSounds creatureRaceSounds = CreatureRaceSounds.GetSoundsByRaceIDAndGender(Race.ID, modelVariation.GenderType);
                if (creatureRaceSounds.SoundIdle2Name.ToLower() != "null24.wav")
                    curObject.NumOfFidgetSounds = 2;
                else if (creatureRaceSounds.SoundIdle1Name.ToLower() != "null24.wav")
                    curObject.NumOfFidgetSounds = 1;

                // Create the M2 and Skin
                M2 objectM2 = new M2(curObject, relativeMPQPath);
                modelVariation.ModelFileName = modelVariation.GenerateFileName(skeletonName);
                objectM2.WriteToDisk(modelVariation.ModelFileName, outputFullMPQPath);

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

        private List<CreatureModelVariation> GetModelVariations()
        {
            List<CreatureModelVariation> modelVariations = new List<CreatureModelVariation>();

            modelVariations.AddRange(GetModelVariationsFromMetadata(MaleTemplateMetadata, CreatureGenderType.Male));
            modelVariations.AddRange(GetModelVariationsFromMetadata(FemaleTemplateMetadata, CreatureGenderType.Female));
            modelVariations.AddRange(GetModelVariationsFromMetadata(NeutralTemplateMetadata, CreatureGenderType.Neutral));
            return modelVariations;
        }

        private List<CreatureModelVariation> GetModelVariationsFromMetadata(TemplateMetadata metadata, CreatureGenderType gender)
        {
            List<CreatureModelVariation> modelVariations = new List<CreatureModelVariation>();

            // Build for every combination of head mesh, body mesh, texture variation.  If there are no head or body meshes,
            // consider that index 0
            for (int curTextureIter = 0; curTextureIter < metadata.NumberOfTextures; curTextureIter++)
            {
                if (metadata.HeadMeshNames.Count == 0)
                {
                    for (int bodyMeshIter = 0; bodyMeshIter < metadata.BodyMeshNames.Count; bodyMeshIter++)
                    {
                        CreatureModelVariation newVariation = new CreatureModelVariation();
                        newVariation.GenderType = gender;
                        newVariation.BodyModelIndex = bodyMeshIter;
                        newVariation.BodyModelName = metadata.BodyMeshNames[bodyMeshIter];
                        newVariation.BodyTextureIndex = curTextureIter;
                        newVariation.HeadTextureIndex = curTextureIter;
                        newVariation.FaceTextureIndex = curTextureIter;
                        modelVariations.Add(newVariation);
                    }
                }
                else if (metadata.BodyMeshNames.Count == 0)
                {
                    for (int headMeshIter = 0; headMeshIter < metadata.HeadMeshNames.Count; headMeshIter++)
                    {
                        CreatureModelVariation newVariation = new CreatureModelVariation();
                        newVariation.GenderType = gender;
                        newVariation.HeadModelIndex = headMeshIter;
                        newVariation.HeadModelName = metadata.HeadMeshNames[headMeshIter];
                        newVariation.BodyTextureIndex = curTextureIter;
                        newVariation.HeadTextureIndex = curTextureIter;
                        newVariation.FaceTextureIndex = curTextureIter;
                        modelVariations.Add(newVariation);
                    }
                }
                else
                {
                    for (int headMeshIter = 0; headMeshIter < metadata.HeadMeshNames.Count; headMeshIter++)
                    {
                        for (int bodyMeshIter = 0; bodyMeshIter < metadata.BodyMeshNames.Count; bodyMeshIter++)
                        {
                            CreatureModelVariation newVariation = new CreatureModelVariation();
                            newVariation.GenderType = gender;
                            newVariation.BodyModelIndex = bodyMeshIter;
                            newVariation.BodyModelName = metadata.BodyMeshNames[bodyMeshIter];
                            newVariation.HeadModelIndex = headMeshIter;
                            newVariation.HeadModelName = metadata.HeadMeshNames[headMeshIter];
                            newVariation.BodyTextureIndex = curTextureIter;
                            newVariation.HeadTextureIndex = curTextureIter;
                            newVariation.FaceTextureIndex = curTextureIter;
                            modelVariations.Add(newVariation);
                        }
                    }
                }
            }

            return modelVariations;
        }

        private void LoadTemplateMetadataForGender(string skeletonName, string skeletonSupplementalName, out TemplateMetadata templateMetadata)
        {
            templateMetadata = new TemplateMetadata();
            if (skeletonName == string.Empty)
                return;

            // Load the EQ data
            ObjectModelEQData eqData = new ObjectModelEQData();
            string charactersFolderRoot = Path.Combine(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER, "characters");
            eqData.LoadSkeletalMetaDataFromDisk(skeletonName, charactersFolderRoot);

            // Mesh Names
            foreach(string meshName in eqData.SkeletonData.MeshNames)
            {
                if (meshName.Contains("he0"))
                    templateMetadata.HeadMeshNames.Add(meshName);
                else
                    templateMetadata.BodyMeshNames.Add(meshName);
            }
            foreach(string meshName in eqData.SkeletonData.SecondaryMeshNames)
            {
                if (meshName.Contains("he0"))
                    templateMetadata.HeadMeshNames.Add(meshName);
                else
                    templateMetadata.BodyMeshNames.Add(meshName);
            }

            // Textures
            templateMetadata.NumberOfTextures = eqData.MaterialsByTextureVariation.Count;
        }
    }
}
