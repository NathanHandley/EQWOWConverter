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
using EQWOWConverter.ObjectModels;
using System.Globalization;
using System.Text;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Creatures
{
    internal class CreatureModelTemplate
    {
        // FaceIndex of 99 marks an illusion version model with replaceable face textures (real faces are only 0-9).  These get their own M2 files

        public static Dictionary<int, List<CreatureModelTemplate>> AllTemplatesByRaceID = new Dictionary<int, List<CreatureModelTemplate>>();

        public CreatureRace Race;
        public CreatureGenderType GenderType = CreatureGenderType.Neutral;
        public int TextureIndex = 0;
        public int HelmTextureIndex = 0;
        public int FaceIndex = 0;
        public int ColorTintID = 0;
        public CreatureTemplateColorTint? ColorTint = null;
        public float ModelTemplateScale = 1.0f; // Used for form changes
        public bool IsCompanionPetVersion = false;
        public bool IsIllusionFormVersion = false;
        public bool IsPetVersion = false;
        public bool IsPlayerCharacterVersion = false;
        public bool IsCharacterBasedVersion = false;
        public CreatureModelTemplate? CharacterBaseModelTemplate = null;
        public int DBCCreatureDisplayInfoExtraID = 0;
        public int CharacterHelmTintIndex = 0; // 0 = untinted
        public string CreatureSkinBakeName = string.Empty; // BakeName of the CreatureDisplayInfoExtra row (Textures\BakedNpcTextures\{name}.blp)
        private static HashSet<string> BakedCreatureSkinNames = new HashSet<string>();
        private static readonly object BakedCreatureSkinLock = new object();
        public float ModelStandingHeight = 0; // Z extent of the stand-posed geometry in final (rendered) model space
        public BoundingBox ModelStandingGeometryBox = new BoundingBox();
        public BoundingBox ModelClickBoundingBox = new BoundingBox(); // The finished clickable box that went into the M2
        public float SmallestCreatureWorldSpawnScale = 0f; // Smallest world scale any creature spawns this model at, 0 when no creature template uses it
        public float ModelCameraAnchorHeight = 0;

        public int DBCCreatureModelDataID;
        public int DBCCreatureDisplayID;
        public int DBCCreatureSoundDataID;

        // Silent-fidget twin rows, only generated for tameable (hunter pet) races
        public int DBCSilentTamedPetCreatureModelDataID = 0;
        public int DBCSilentTamedPetCreatureDisplayID = 0;
        public int DBCSilentTamedPetCreatureSoundDataID = 0;
        private static readonly object CreatureLock = new object();
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, object> OutputFolderLocksByName = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();

        private static object GetOutputFolderLock(string outputFolderName)
        {
            return OutputFolderLocksByName.GetOrAdd(outputFolderName, CreateOutputFolderLock);
        }

        private static object CreateOutputFolderLock(string outputFolderName)
        {
            return new object();
        }


        public CreatureModelTemplate(CreatureRace creatureRace, CreatureGenderType genderType, int helmTextureID,
            int textureIndex, int faceIndex, int colorTintID, float modelTemplateScale, bool isCompanionPetVersion, bool isIllusionFormVersion,
            bool isPetVersion, bool isPlayerCharacterVersion = false)
        {
            string raceIDString = creatureRace.ID.ToString();
            string genderIDString = Convert.ToInt32(genderType).ToString();
            string scaleString = modelTemplateScale.ToString(CultureInfo.InvariantCulture);
            IsCompanionPetVersion = isCompanionPetVersion;
            IsIllusionFormVersion = isIllusionFormVersion;

            // Summoned pet versions get no ID of their own, only their sound row differ (no fidget sounds)
            IsPetVersion = isPetVersion;
            IsPlayerCharacterVersion = isPlayerCharacterVersion;
            if (isPlayerCharacterVersion == true)
            {
                // Player character versions key separately from every other template type
                DBCCreatureModelDataID = IDGenerationTool.GenerateID("CreatureModelDataID", "playercharacter", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
                DBCCreatureDisplayID = IDGenerationTool.GenerateID("CreatureDisplayInfoID", "playercharacter", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
                DBCCreatureSoundDataID = IDGenerationTool.GenerateID("CreatureSoundDataID", "playercharacter", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
            }
            else if (isCompanionPetVersion == true)
            {
                // Companion pet versions key separately from the shared NPC templates
                DBCCreatureModelDataID = IDGenerationTool.GenerateID("CreatureModelDataID", "companionpet", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
                DBCCreatureDisplayID = IDGenerationTool.GenerateID("CreatureDisplayInfoID", "companionpet", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
                DBCCreatureSoundDataID = 0; // Make them silent
            }
            else if (isIllusionFormVersion == true)
            {
                // Illusion form versions key separately from the pet/NPC-shared templates so their sound data can differ (no fidget sounds)
                DBCCreatureModelDataID = IDGenerationTool.GenerateID("CreatureModelDataID", "illusionform", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
                DBCCreatureDisplayID = IDGenerationTool.GenerateID("CreatureDisplayInfoID", "illusionform", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
                DBCCreatureSoundDataID = IDGenerationTool.GenerateID("CreatureSoundDataID", "illusionform", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
            }
            else
            {
                DBCCreatureModelDataID = IDGenerationTool.GenerateID("CreatureModelDataID", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
                DBCCreatureDisplayID = IDGenerationTool.GenerateID("CreatureDisplayInfoID", "modeltemplate", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
                DBCCreatureSoundDataID = IDGenerationTool.GenerateID("CreatureSoundDataID", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
            }

            // Tamed (hunter) pets keep the display of the world creature they came from, so tameable races also get a parallel set of rows that only differ by having no fidget sounds
            if (DoGenerateSilentTamedPetVersionForProperties(creatureRace, faceIndex, isCompanionPetVersion, isIllusionFormVersion, isPetVersion) == true)
            {
                DBCSilentTamedPetCreatureModelDataID = IDGenerationTool.GenerateID("CreatureModelDataID", "tamedpetsilent", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
                DBCSilentTamedPetCreatureDisplayID = IDGenerationTool.GenerateID("CreatureDisplayInfoID", "tamedpetsilent", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
                DBCSilentTamedPetCreatureSoundDataID = IDGenerationTool.GenerateID("CreatureSoundDataID", "tamedpetsilent", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
            }

            Race = creatureRace;
            GenderType = genderType;
            TextureIndex = textureIndex;
            HelmTextureIndex = helmTextureID;
            FaceIndex = faceIndex;
            ModelTemplateScale = modelTemplateScale;

            if (colorTintID != 0)
            {
                Dictionary<int, CreatureTemplateColorTint> colorTints = CreatureTemplateColorTint.GetCreatureTemplateColorTints();
                if (colorTints.ContainsKey(colorTintID) == false)
                    Logger.WriteError("No color tint ID of '" + colorTintID + "' found");
                else
                {
                    ColorTint = colorTints[colorTintID];
                    ColorTintID = colorTintID;
                }
            }

            ModelTemplateScale = modelTemplateScale;
        }

        public bool DoUseOwnModelFiles()
        {
            return IsCompanionPetVersion == true || IsIllusionFormVersion == true || IsPlayerCharacterVersion == true;
        }

        private static bool DoGenerateSilentTamedPetVersionForProperties(CreatureRace creatureRace, int faceIndex, bool isCompanionPetVersion,
            bool isIllusionFormVersion, bool isPetVersion)
        {
            // A tamed pet copies the display of a world creature, so only the world NPC templates need a silent version
            if (isCompanionPetVersion == true || isIllusionFormVersion == true || isPetVersion == true)
                return false;
            if (creatureRace.WOWCreatureType != 1) // Only beasts are tameable
                return false;

            // Races with no walking sound get no CreatureSoundData row at all, so they already play no fidget sounds
            if (creatureRace.SoundWalkingName.Trim().Length == 0)
                return false;
            return true;
        }

        public bool DoGenerateSilentTamedPetVersion()
        {
            return DBCSilentTamedPetCreatureDisplayID != 0;
        }

        public void EnsureSilentFidgetAltIDsGenerated()
        {
            if (DBCSilentTamedPetCreatureDisplayID != 0)
                return;
            if (IsCompanionPetVersion == true)
                return;
            if (Race.SoundWalkingName.Trim().Length == 0)
                return;

            string raceIDString = Race.ID.ToString();
            string genderIDString = Convert.ToInt32(GenderType).ToString();
            string scaleString = ModelTemplateScale.ToString(CultureInfo.InvariantCulture);
            DBCSilentTamedPetCreatureModelDataID = IDGenerationTool.GenerateID("CreatureModelDataID", "tamedpetsilent", raceIDString, genderIDString, HelmTextureIndex.ToString(), TextureIndex.ToString(), FaceIndex.ToString(), ColorTintID.ToString(), scaleString);
            DBCSilentTamedPetCreatureDisplayID = IDGenerationTool.GenerateID("CreatureDisplayInfoID", "tamedpetsilent", raceIDString, genderIDString, HelmTextureIndex.ToString(), TextureIndex.ToString(), FaceIndex.ToString(), ColorTintID.ToString(), scaleString);
            DBCSilentTamedPetCreatureSoundDataID = IDGenerationTool.GenerateID("CreatureSoundDataID", "tamedpetsilent", raceIDString, genderIDString, HelmTextureIndex.ToString(), TextureIndex.ToString(), FaceIndex.ToString(), ColorTintID.ToString(), scaleString);
        }

        public bool DoPlayFidgetSounds()
        {
            // Forms that a player controls (illusions, summoned pets, and dressable player character models) stay quiet while idle
            if (IsIllusionFormVersion == true || IsPetVersion == true || IsPlayerCharacterVersion == true)
                return false;
            return true;
        }

        public float GetDBCDisplayScale()
        {
            // Client grows/shrinks unit's attached models by object scale and CreatureDisplayInfo scale.
            if (IsPlayerCharacterVersion == true)
                return 1f;
            if (ModelTemplateScale <= Configuration.GENERATE_FLOAT_EPSILON)
                return 1f;
            return ModelTemplateScale;
        }

        public float GetSmallestWorldScaleForClickBox()
        {
            if (SmallestCreatureWorldSpawnScale <= Configuration.GENERATE_FLOAT_EPSILON)
                return 0f;
            return GetDBCDisplayScale() * SmallestCreatureWorldSpawnScale;
        }

        public BoundingBox GenerateClickBoundingBox(BoundingBox geometryBoundingBox)
        {
            BoundingBox clickBoundingBox = BoundingBox.GetExpandedBox(geometryBoundingBox, Configuration.GENERATE_CREATURE_CLICKBOX_SIZE_MULTIPLIER,
                Configuration.GENERATE_CREATURE_CLICKBOX_ADDED_SIZE, Configuration.GENERATE_CREATURE_CLICKBOX_MIN_SIZE);

            // Tiny creatures shrink the box along with the model, so enforce the floor in world units and convert it back into model space
            float worldScale = GetSmallestWorldScaleForClickBox();
            if (Configuration.GENERATE_CREATURE_CLICKBOX_MIN_WORLD_SIZE > Configuration.GENERATE_FLOAT_EPSILON && worldScale > Configuration.GENERATE_FLOAT_EPSILON)
            {
                float minSizeInModelSpace = Configuration.GENERATE_CREATURE_CLICKBOX_MIN_WORLD_SIZE / worldScale;
                clickBoundingBox.ExpandToMinimumSizeWithGrowthCap(minSizeInModelSpace, Configuration.GENERATE_CREATURE_CLICKBOX_MIN_WORLD_SIZE_MAX_MULTIPLIER);
            }
            return clickBoundingBox;
        }

        public bool DoSuppressHeldItemAttachments()
        {
            return Race.CanHoldVisualItems == false;
        }

        public bool DoSuppressHeldShieldAttachment()
        {
            return Race.CanHoldVisualItems == false || Race.CanHoldVisualShields == false;
        }

        public static CreatureModelTemplate CreateCreatureModelTemplateForWaypointDebugging()
        {
            lock (CreatureLock)
            {
                // Otherwise create a new one
                CreatureRace debugRace = new CreatureRace(1, CreatureGenderType.Male, 0, "Debug Male", "HUM", "ELM", 3, 1, 6, 0.2f, 1.96078f, 0, 7, false);
                CreatureModelTemplate newModelTemplate = new CreatureModelTemplate(debugRace, 0, 0, 0, 0, 0, 1, false, false, false);
                AllTemplatesByRaceID.Add(1, new List<CreatureModelTemplate>());
                AllTemplatesByRaceID[1].Add(newModelTemplate);
                return newModelTemplate;
            }
        }

        public static CreatureModelTemplate GetOrCreateCreatureModelTemplate(CreatureRace creatureRace, CreatureGenderType genderType, int helmTextureID,
            int textureIndex, int faceIndex, int colorTintID, float modelTemplateScale, bool isCompanionPetVersion, bool isIllusionFormVersion,
            bool isPetVersion, bool isPlayerCharacterVersion = false)
        {
            lock (CreatureLock)
            {
                // They are grouped by race
                if (AllTemplatesByRaceID.ContainsKey(creatureRace.ID) == false)
                    AllTemplatesByRaceID.Add(creatureRace.ID, new List<CreatureModelTemplate>());

                // Return existing, if it exists
                foreach (CreatureModelTemplate modelTemplate in AllTemplatesByRaceID[creatureRace.ID])
                {
                    // Skip if this model template already exists
                    if (modelTemplate.GenderType == genderType &&
                        modelTemplate.HelmTextureIndex == helmTextureID &&
                        modelTemplate.TextureIndex == textureIndex &&
                        modelTemplate.FaceIndex == faceIndex &&
                        modelTemplate.ColorTintID == colorTintID &&
                        modelTemplate.ModelTemplateScale == modelTemplateScale &&
                        modelTemplate.IsCompanionPetVersion == isCompanionPetVersion &&
                        modelTemplate.IsIllusionFormVersion == isIllusionFormVersion &&
                        modelTemplate.IsPetVersion == isPetVersion &&
                        modelTemplate.IsPlayerCharacterVersion == isPlayerCharacterVersion)
                    {
                        return modelTemplate;
                    }
                }

                // Otherwise create a new one
                CreatureModelTemplate newModelTemplate = new CreatureModelTemplate(creatureRace, genderType, helmTextureID,
                    textureIndex, faceIndex, colorTintID, modelTemplateScale, isCompanionPetVersion, isIllusionFormVersion, isPetVersion, isPlayerCharacterVersion);

                // Pet versions share an ID context with the non-pet templates
                foreach (CreatureModelTemplate existingModelTemplate in AllTemplatesByRaceID[creatureRace.ID])
                {
                    if (existingModelTemplate.DBCCreatureModelDataID != newModelTemplate.DBCCreatureModelDataID)
                        continue;
                    Logger.WriteError(string.Concat("Creature model template for race '", creatureRace.Name, "' generated the model data ID '",
                        newModelTemplate.DBCCreatureModelDataID.ToString(), "' twice (isPetVersion of '", isPetVersion.ToString(), "'), so pet versions need their own ID context"));
                }

                AllTemplatesByRaceID[creatureRace.ID].Add(newModelTemplate);
                return newModelTemplate;
            }
        }

        public static void CreateCreatureModelTemplatesFromCreatureTemplates(List<CreatureTemplate> creatureTemplates)
        {
            // Clear the old list
            AllTemplatesByRaceID.Clear();
            HashSet<int> druidFormCreatureTemplateIDs = CreatureDruidFormOption.GetCreatureTemplateIDs();

            // Generate model templates in response to creature templates
            foreach(CreatureTemplate creatureTemplate in creatureTemplates)
            {
                CreatureModelTemplate curModelTemplate = GetOrCreateCreatureModelTemplate(creatureTemplate.Race,
                    creatureTemplate.GenderType, creatureTemplate.HelmTextureID, creatureTemplate.TextureID, creatureTemplate.FaceID,
                    creatureTemplate.ColorTintID, creatureTemplate.ModelTemplateScale, creatureTemplate.IsCompanionPet, creatureTemplate.IsIllusionForm,
                    creatureTemplate.IsPet);
                creatureTemplate.ModelTemplate = curModelTemplate;

                // A druid in one of the Norrath forms wears this creature's display, and a player worn form plays no fidget sounds
                if (druidFormCreatureTemplateIDs.Contains(creatureTemplate.WOWCreatureTemplateID) == true)
                    curModelTemplate.EnsureSilentFidgetAltIDsGenerated();

                // Track how small this model ever spawns, since the click box has to stay usable for the smallest creature having it
                if (creatureTemplate.IsCompanionPet == false)
                {
                    float creatureWorldSpawnScale = creatureTemplate.GetWorldSpawnScale();
                    if (creatureWorldSpawnScale > Configuration.GENERATE_FLOAT_EPSILON &&
                        (curModelTemplate.SmallestCreatureWorldSpawnScale <= Configuration.GENERATE_FLOAT_EPSILON || creatureWorldSpawnScale < curModelTemplate.SmallestCreatureWorldSpawnScale))
                    {
                        curModelTemplate.SmallestCreatureWorldSpawnScale = creatureWorldSpawnScale;
                    }
                }
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
            if (IsPlayerCharacterVersion == true)
            {
                string clientFileString = CreatureIllusionCharacterRegistry.GetClientFileStringForRace(Race);
                string genderFolderName = CreatureIllusionCharacterRegistry.GetGenderFolderName(GenderType);
                outputObjectFolderName = Path.Combine(clientFileString, genderFolderName);
                relativeMPQPath = Path.Combine("Character", clientFileString, genderFolderName);
                outputFullMPQPath = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", "Character", clientFileString, genderFolderName);
            }

            // Looks of a race with a player character model render that model instead of files of their own
            CreatureIllusionCharacterRegistry.IllusionCharacterEntry? characterEntry = null;
            if (IsPlayerCharacterVersion == false)
            {
                // Neutral gender looks (the same skeleton as the male) render on the male character model
                CreatureGenderType characterGenderType = (GenderType == CreatureGenderType.Female) ? CreatureGenderType.Female : CreatureGenderType.Male;
                characterEntry = CreatureIllusionCharacterRegistry.GetEntry(Race.ID, characterGenderType);
                if (characterEntry != null && (characterEntry.ModelTemplate == null || characterEntry.CharacterComposite == null || characterEntry.Race.SkeletonName != Race.SkeletonName))
                    characterEntry = null;
                if (characterEntry != null)
                {
                    IsCharacterBasedVersion = true;
                    CharacterBaseModelTemplate = characterEntry.ModelTemplate;
                }
            }

            // Create folder if it doesn't exist
            if (IsCharacterBasedVersion == false && Directory.Exists(outputFullMPQPath) == false)
                Directory.CreateDirectory(outputFullMPQPath);

            // Load in an object
            float lift = Race.Lift;
            ObjectModelProperties objectProperties = new ObjectModelProperties(ObjectModelProperties.GetObjectPropertiesForObject(skeletonName.ToLower()));
            objectProperties.CreatureModelTemplate = this;
            objectProperties.ModelScalePreWorldScale = Race.ModelScale;
            objectProperties.ModelLiftPreWorldScale = lift;
            ObjectModel curObject = new ObjectModel(skeletonName, objectProperties, ObjectModelType.Creature);
            curObject.LoadEQObjectFromFile(charactersFolderRoot, skeletonName);
            // GeometryBoundingBox is the stand-posed vertices in the same space the client renders
            ModelStandingHeight = curObject.GeometryBoundingBox.TopCorner.Z - curObject.GeometryBoundingBox.BottomCorner.Z;
            ModelStandingGeometryBox = new BoundingBox(curObject.GeometryBoundingBox);
            ModelClickBoundingBox = new BoundingBox(curObject.InteractionBoundingBox);
            ModelCameraAnchorHeight = curObject.GetAnchorAttachmentPositionModelSpace(ObjectModelAttachmentType.MouthBreath).Z;
            StringBuilder nameSB = new StringBuilder();
            nameSB.Append(Race.Name);
            nameSB.Append(" ");
            nameSB.Append(GenerateFileName());
            curObject.Name = nameSB.ToString();

            // Character based versions only needed the bounds above as their look is a baked skin on the shared character model
            if (IsCharacterBasedVersion == true && characterEntry != null && characterEntry.CharacterComposite != null)
            {
                GenerateCharacterBasedDisplayData(characterEntry.CharacterComposite);
                Logger.WriteDebug(String.Concat("For creature template '", objectName, "', completed generating the character based display data"));
                return;
            }

            // Set fidget count for M2 (companion pet versions are silent, so they keep zero fidget sounds)
            if (IsCompanionPetVersion == false)
            {
                if (Race.SoundIdle2Name.ToLower() != "null24.wav")
                    curObject.NumOfFidgetSounds = 2;
                else if (Race.SoundIdle1Name.ToLower() != "null24.wav")
                    curObject.NumOfFidgetSounds = 1;
            }

            // Create the M2 and Skin
            M2 objectM2 = new M2(curObject, relativeMPQPath);
            lock (GetOutputFolderLock(Path.Combine(outputObjectFolderName, GenerateFileName())))
                objectM2.WriteToDisk(GenerateFileName(), outputFullMPQPath);

            // The client's glue screens preload a model for every ChrRaces row through the naming convention Character\{ClientFileString}\{Sex}\{ClientFileString}{Sex}.m2,
            // and a missing file there intermittently crashes the client on an async loader thread, so player character models also get alias copies under that name
            if (IsPlayerCharacterVersion == true)
            {
                string aliasBaseName = string.Concat(CreatureIllusionCharacterRegistry.GetClientFileStringForRace(Race),
                    CreatureIllusionCharacterRegistry.GetGenderFolderName(GenderType));
                lock (GetOutputFolderLock(Path.Combine(outputObjectFolderName, aliasBaseName)))
                {
                    FileTool.CopyFile(Path.Combine(outputFullMPQPath, GenerateFileName() + ".m2"), Path.Combine(outputFullMPQPath, aliasBaseName + ".m2"));
                    for (int lodIndex = 0; lodIndex < 4; lodIndex++)
                    {
                        string lodSuffix = lodIndex.ToString("00");
                        FileTool.CopyFile(Path.Combine(outputFullMPQPath, GenerateFileName() + lodSuffix + ".skin"),
                            Path.Combine(outputFullMPQPath, aliasBaseName + lodSuffix + ".skin"));
                    }
                }
            }

            // Place the related textures. Serialized per shared race output folder because every model template of a race copies into the same folder
            lock (GetOutputFolderLock(outputObjectFolderName))
            {
                foreach (ObjectModelTexture texture in curObject.ModelTextures)
                {
                    // Only hardcoded textures have a file to copy
                    if (texture.Type != ObjectModelTextureType.Hardcoded)
                        continue;

                    string inputTextureNameInCharTextureFolder = Path.Combine(inputObjectTextureFolder, texture.TextureName + ".blp");
                    string inputTextureNameInGeneratedTextureFolder = Path.Combine(generatedTexturesFolderPath, texture.TextureName + ".blp");
                    string outputTextureName = Path.Combine(outputFullMPQPath, texture.TextureName + ".blp");

                    if (File.Exists(outputTextureName) == true)
                        continue;

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
            }

            // Player character versions bake their composite skin and head atlas textures, and record their output data for DBC/SQL generation
            if (IsPlayerCharacterVersion == true)
                GeneratePlayerCharacterData(curObject);

            Logger.WriteDebug(String.Concat("For creature template '", objectName, "', completed creating the object files"));
        }

        private void GenerateCharacterBasedDisplayData(ObjectModelCharacterComposite characterComposite)
        {
            string genderCode = (GenderType == CreatureGenderType.Female) ? "F" : "M";
            CreatureSkinBakeName = string.Concat("EQ_", Race.SkeletonName.ToUpper(), "_", genderCode, "_h", HelmTextureIndex.ToString(), "t", TextureIndex.ToString(),
                "f", FaceIndex.ToString(), "c", ColorTintID.ToString());
            DBCCreatureDisplayInfoExtraID = IDGenerationTool.GenerateID("CreatureDisplayInfoExtraID", CreatureSkinBakeName);
            if (HelmTextureIndex > 0 && ColorTint != null)
                CharacterHelmTintIndex = characterComposite.GetHelmTintIndex(ColorTint.HelmColor);

            lock (BakedCreatureSkinLock)
            {
                if (BakedCreatureSkinNames.Contains(CreatureSkinBakeName) == true)
                    return;
                BakedCreatureSkinNames.Add(CreatureSkinBakeName);
            }
            string workingFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedCreatureTextures", "BakedNpcTextures");
            string outputFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", "Textures", "BakedNpcTextures");
            lock (BakedCreatureSkinLock)
            {
                if (Directory.Exists(workingFolder) == false)
                    Directory.CreateDirectory(workingFolder);
                if (Directory.Exists(outputFolder) == false)
                    Directory.CreateDirectory(outputFolder);
            }
            string pngPath = Path.Combine(workingFolder, CreatureSkinBakeName + ".png");
            if (characterComposite.BakeCreatureSkin(TextureIndex, FaceIndex, HelmTextureIndex, ColorTint, pngPath) == false)
            {
                Logger.WriteError(string.Concat("Creature model template '", GenerateFileName(), "' could not bake its character skin"));
                return;
            }
            ImageTool.ConvertPNGTexturesToBLP(new List<string>() { pngPath }, ImageTool.ImageAssociationType.CharacterSkin);
            string blpPath = Path.ChangeExtension(pngPath, ".blp");
            if (File.Exists(blpPath) == false)
            {
                Logger.WriteError(string.Concat("Creature model template '", GenerateFileName(), "' baked skin BLP was not generated at '", blpPath, "'"));
                return;
            }
            FileTool.CopyFile(blpPath, Path.Combine(outputFolder, CreatureSkinBakeName + ".blp"));
        }

        private void GeneratePlayerCharacterData(ObjectModel curObject)
        {
            ObjectModelCharacterComposite? characterComposite = curObject.EQObjectModelData.CharacterComposite;
            if (characterComposite == null)
            {
                Logger.WriteError(string.Concat("Player character model template '", GenerateFileName(), "' has no character composite, so no textures or race data were generated"));
                return;
            }
            string workingRootFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedCharacterTextures");
            string outputFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", "Character",
                CreatureIllusionCharacterRegistry.GetClientFileStringForRace(Race), CreatureIllusionCharacterRegistry.GetGenderFolderName(GenderType));
            string blankTextureOutputFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", "Character", "EverQuest");
            if (Directory.Exists(workingRootFolder) == false)
                Directory.CreateDirectory(workingRootFolder);
            if (Directory.Exists(blankTextureOutputFolder) == false)
                Directory.CreateDirectory(blankTextureOutputFolder);
            // NPC helm colors of this race's looks become the tinted helm textures (hair colors 1..)
            List<ColorRGBA> helmColors = new List<ColorRGBA>();
            lock (CreatureLock)
            {
                if (AllTemplatesByRaceID.ContainsKey(Race.ID) == true)
                {
                    foreach (CreatureModelTemplate otherTemplate in AllTemplatesByRaceID[Race.ID])
                    {
                        CreatureGenderType otherGender = (otherTemplate.GenderType == CreatureGenderType.Female) ? CreatureGenderType.Female : CreatureGenderType.Male;
                        if (otherGender != GenderType || otherTemplate.HelmTextureIndex <= 0 || otherTemplate.ColorTint == null || otherTemplate.ColorTint.HelmColor == null)
                            continue;
                        helmColors.Add((ColorRGBA)otherTemplate.ColorTint.HelmColor);
                    }
                }
            }
            characterComposite.SetHelmTintPalette(helmColors);
            characterComposite.GenerateBakedTextures(workingRootFolder, outputFolder, blankTextureOutputFolder);
            CreatureIllusionCharacterRegistry.SetModelOutputData(Race.ID, GenderType, characterComposite);

            // Worn armor components in this race's own EQ art, which the armor item display infos get native variants of (see ItemDisplayInfo.CreateNativeVariants) for the mod to swap in when dressing this race's illusion
            int nativeComponentCount = characterComposite.GenerateNativeArmorComponentTextures(ObjectModelCharacterComposite.GetNativeComponentFolder());
            if (nativeComponentCount > 0)
            {
                int chrRacesID = CreatureIllusionCharacterRegistry.GetChrRacesID(Race.ID, GenderType);
                if (chrRacesID > 0)
                    Items.ItemDisplayInfo.RegisterNativeComponentRace(characterComposite.SkeletonName, chrRacesID, Convert.ToInt32(GenderType));
                else
                    Logger.WriteError(string.Concat("Player character model template '", GenerateFileName(), "' has no ChrRaces ID, so its native armor components are unreferenced"));
            }
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
            if (IsCompanionPetVersion == true)
                sb.Append("cp");
            else if (IsIllusionFormVersion == true)
                sb.Append("il");
            else if (IsPlayerCharacterVersion == true)
                sb.Append("pc");
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

        public static int CompareCreatureModelTemplatesByModelDataID(CreatureModelTemplate template1, CreatureModelTemplate template2)
        {
            return template1.DBCCreatureModelDataID.CompareTo(template2.DBCCreatureModelDataID);
        }
    }
}
