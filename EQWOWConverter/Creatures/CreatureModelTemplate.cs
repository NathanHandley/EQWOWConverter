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

using EQWOWConverter.ObjectModels;
using System.Globalization;
using System.Text;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Creatures
{
    internal class CreatureModelTemplate
    {
        // FaceIndex of 99 marks an illusion version model with replaceable face textures (real faces are only 0-9).  These get their own M2 files
        // (separate from any NPC-shared templates) where the head/face textures are typed as creature skins, so the face is selected at runtime through CreatureDisplayInfo texture variations instead of being baked in
        public const int ILLUSION_REPLACEABLE_FACE_INDEX = 99;

        public static Dictionary<int, List<CreatureModelTemplate>> AllTemplatesByRaceID = new Dictionary<int, List<CreatureModelTemplate>>();

        public CreatureRace Race;
        public CreatureGenderType GenderType = CreatureGenderType.Neutral;
        public int TextureIndex = 0;
        public int HelmTextureIndex = 0;
        public int FaceIndex = 0;
        public int ColorTintID = 0;
        public CreatureTemplateColorTint? ColorTint = null;
        public float ModelTemplateScale = 1.0f; // Used for form changes

        private static int CURRENT_DBCID_CREATUREMODELDATAID = Configuration.DBCID_CREATUREMODELDATA_ID_START;
        private static int CURRENT_DBCID_CREATUREDISPLAYINFOID = -1;
        private static int CURRENT_DBCID_CREATURESOUNDDATAID = Configuration.DBCID_CREATURESOUNDDATA_ID_START;
        private static readonly object CreatureIDLock = new object();
        public int DBCCreatureModelDataID;
        public int DBCCreatureDisplayID;
        public int DBCCreatureSoundDataID;
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

        // Values are [DBCCreatureModelDataID, DBCCreatureDisplayID, DBCCreatureSoundDataID] keyed by the generating context
        private static Dictionary<string, int[]>? SavedDBCIDsByContextKey = null;

        // Illusion version face displays, keyed by the generating context with values of CreatureDisplayInfo IDs
        private static Dictionary<string, int>? SavedIllusionFaceDisplayIDsByContextKey = null;

        // Illusion version (FaceIndex 99) face data, populated during CreateModelFiles.  Face 0 head piece texture names are stored in head piece order, and per selectable face the display ID and texture variation names (one per head piece)
        public List<string> FaceHeadPieceTextureNames = new List<string>();
        public SortedDictionary<int, int> IllusionFaceDisplayIDsByFaceIndex = new SortedDictionary<int, int>();
        public SortedDictionary<int, List<string>> IllusionFaceTextureVariationsByFaceIndex = new SortedDictionary<int, List<string>>();

        public CreatureModelTemplate(CreatureRace creatureRace, CreatureGenderType genderType, int helmTextureID,
            int textureIndex, int faceIndex, int colorTintID, float modelTemplateScale)
        {
            lock (CreatureLock)
            {
                LoadSavedDBCIDsIfNeeded();
                string contextKey = GenerateDBCIDsContextKey(creatureRace.ID, genderType, helmTextureID, textureIndex,
                    faceIndex, colorTintID, modelTemplateScale);
                if (SavedDBCIDsByContextKey!.ContainsKey(contextKey) == true)
                {
                    int[] savedDBCIDs = SavedDBCIDsByContextKey[contextKey];
                    DBCCreatureModelDataID = savedDBCIDs[0];
                    DBCCreatureDisplayID = savedDBCIDs[1];
                    DBCCreatureSoundDataID = savedDBCIDs[2];
                }
                else
                {
                    DBCCreatureModelDataID = CURRENT_DBCID_CREATUREMODELDATAID;
                    CURRENT_DBCID_CREATUREMODELDATAID++;
                    DBCCreatureDisplayID = GenerateDisplayInfoID();
                    DBCCreatureSoundDataID = CURRENT_DBCID_CREATURESOUNDDATAID;
                    CURRENT_DBCID_CREATURESOUNDDATAID++;
                    SavedDBCIDsByContextKey.Add(contextKey, new int[] { DBCCreatureModelDataID, DBCCreatureDisplayID, DBCCreatureSoundDataID });
                    AppendSavedDBCIDsToFile(creatureRace.ID, genderType, helmTextureID, textureIndex, faceIndex,
                        colorTintID, modelTemplateScale, DBCCreatureModelDataID, DBCCreatureDisplayID, DBCCreatureSoundDataID);
                }
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

        public static int GenerateDisplayInfoID()
        {
            lock (CreatureIDLock)
            {
                if (CURRENT_DBCID_CREATUREDISPLAYINFOID == -1)
                    CURRENT_DBCID_CREATUREDISPLAYINFOID = Configuration.DBCID_CREATUREDISPLAYINFO_ID_START;
                int returnID = CURRENT_DBCID_CREATUREDISPLAYINFOID;
                if (returnID > Configuration.DBCID_CREATUREDISPLAYINFO_ID_END)
                    Logger.WriteError(string.Concat("Generated CreatureDisplayInfo ID '", returnID.ToString(), "' is above DBCID_CREATUREDISPLAYINFO_ID_END of '", Configuration.DBCID_CREATUREDISPLAYINFO_ID_END.ToString(), "', and this will crash the world server"));
                CURRENT_DBCID_CREATUREDISPLAYINFOID++;
                return returnID;
            }
        }

        private static string GetSavedDBCIDsFilePath()
        {
            return Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureModelTemplateIDs.csv");
        }

        private static string GetSavedIllusionFaceDisplayIDsFilePath()
        {
            return Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureIllusionFaceDisplayIDs.csv");
        }

        private static string GenerateIllusionFaceDisplayIDContextKey(int raceID, CreatureGenderType genderType, int helmTextureID,
            int textureIndex, int colorTintID, int faceIndex)
        {
            StringBuilder keySB = new StringBuilder();
            keySB.Append(raceID);
            keySB.Append("|");
            keySB.Append(Convert.ToInt32(genderType));
            keySB.Append("|");
            keySB.Append(helmTextureID);
            keySB.Append("|");
            keySB.Append(textureIndex);
            keySB.Append("|");
            keySB.Append(colorTintID);
            keySB.Append("|");
            keySB.Append(faceIndex);
            return keySB.ToString();
        }

        private static string GenerateDBCIDsContextKey(int raceID, CreatureGenderType genderType, int helmTextureID,
            int textureIndex, int faceIndex, int colorTintID, float modelTemplateScale)
        {
            StringBuilder keySB = new StringBuilder();
            keySB.Append(raceID);
            keySB.Append("|");
            keySB.Append(Convert.ToInt32(genderType));
            keySB.Append("|");
            keySB.Append(helmTextureID);
            keySB.Append("|");
            keySB.Append(textureIndex);
            keySB.Append("|");
            keySB.Append(faceIndex);
            keySB.Append("|");
            keySB.Append(colorTintID);
            keySB.Append("|");
            keySB.Append(modelTemplateScale.ToString(CultureInfo.InvariantCulture));
            return keySB.ToString();
        }

        private static void LoadSavedDBCIDsIfNeeded()
        {
            if (SavedDBCIDsByContextKey != null)
                return;
            SavedDBCIDsByContextKey = new Dictionary<string, int[]>();
            if (CURRENT_DBCID_CREATUREDISPLAYINFOID == -1)
                CURRENT_DBCID_CREATUREDISPLAYINFOID = Configuration.DBCID_CREATUREDISPLAYINFO_ID_START;
            LoadSavedIllusionFaceDisplayIDs();

            string savedDBCIDsFilePath = GetSavedDBCIDsFilePath();
            if (File.Exists(savedDBCIDsFilePath) == false)
            {
                Logger.WriteDebug("No saved creature model template DBC IDs file found at '" + savedDBCIDsFilePath + "', so all IDs will be newly generated");
                return;
            }

            Logger.WriteDebug("Loading saved creature model template DBC IDs via file '" + savedDBCIDsFilePath + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(savedDBCIDsFilePath, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                StringBuilder keySB = new StringBuilder();
                keySB.Append(columns["raceid"]);
                keySB.Append("|");
                keySB.Append(columns["genderid"]);
                keySB.Append("|");
                keySB.Append(columns["helmtextureid"]);
                keySB.Append("|");
                keySB.Append(columns["textureindex"]);
                keySB.Append("|");
                keySB.Append(columns["faceindex"]);
                keySB.Append("|");
                keySB.Append(columns["colortintid"]);
                keySB.Append("|");
                keySB.Append(columns["modeltemplatescale"]);
                string contextKey = keySB.ToString();
                if (SavedDBCIDsByContextKey.ContainsKey(contextKey) == true)
                {
                    Logger.WriteError("Duplicate context key '" + contextKey + "' found in '" + savedDBCIDsFilePath + "', skipping the duplicate row");
                    continue;
                }

                int modelDataDBCID = int.Parse(columns["creaturemodeldata_dbcid"]);
                int displayDBCID = int.Parse(columns["creaturedisplay_dbcid"]);
                int soundDataDBCID = int.Parse(columns["creaturesounddata_dbcid"]);
                SavedDBCIDsByContextKey.Add(contextKey, new int[] { modelDataDBCID, displayDBCID, soundDataDBCID });

                // Ensure newly generated IDs never collide with previously saved ones
                if (modelDataDBCID >= CURRENT_DBCID_CREATUREMODELDATAID)
                    CURRENT_DBCID_CREATUREMODELDATAID = modelDataDBCID + 1;
                if (displayDBCID >= CURRENT_DBCID_CREATUREDISPLAYINFOID)
                    CURRENT_DBCID_CREATUREDISPLAYINFOID = displayDBCID + 1;
                if (soundDataDBCID >= CURRENT_DBCID_CREATURESOUNDDATAID)
                    CURRENT_DBCID_CREATURESOUNDDATAID = soundDataDBCID + 1;
            }
        }

        private static void LoadSavedIllusionFaceDisplayIDs()
        {
            SavedIllusionFaceDisplayIDsByContextKey = new Dictionary<string, int>();
            string savedFaceDisplayIDsFilePath = GetSavedIllusionFaceDisplayIDsFilePath();
            if (File.Exists(savedFaceDisplayIDsFilePath) == false)
            {
                Logger.WriteDebug("No saved illusion face display IDs file found at '" + savedFaceDisplayIDsFilePath + "', so all IDs will be newly generated");
                return;
            }

            Logger.WriteDebug("Loading saved illusion face display IDs via file '" + savedFaceDisplayIDsFilePath + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(savedFaceDisplayIDsFilePath, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                StringBuilder keySB = new StringBuilder();
                keySB.Append(columns["raceid"]);
                keySB.Append("|");
                keySB.Append(columns["genderid"]);
                keySB.Append("|");
                keySB.Append(columns["helmtextureid"]);
                keySB.Append("|");
                keySB.Append(columns["textureindex"]);
                keySB.Append("|");
                keySB.Append(columns["colortintid"]);
                keySB.Append("|");
                keySB.Append(columns["faceindex"]);
                string contextKey = keySB.ToString();
                if (SavedIllusionFaceDisplayIDsByContextKey.ContainsKey(contextKey) == true)
                {
                    Logger.WriteError("Duplicate context key '" + contextKey + "' found in '" + savedFaceDisplayIDsFilePath + "', skipping the duplicate row");
                    continue;
                }

                int displayDBCID = int.Parse(columns["creaturedisplay_dbcid"]);
                SavedIllusionFaceDisplayIDsByContextKey.Add(contextKey, displayDBCID);

                // Ensure newly generated IDs never collide with previously saved ones
                if (displayDBCID >= CURRENT_DBCID_CREATUREDISPLAYINFOID)
                    CURRENT_DBCID_CREATUREDISPLAYINFOID = displayDBCID + 1;
            }
        }

        private static int GetOrCreateIllusionFaceDisplayID(int raceID, CreatureGenderType genderType, int helmTextureID,
            int textureIndex, int colorTintID, int faceIndex)
        {
            lock (CreatureLock)
            {
                LoadSavedDBCIDsIfNeeded();
                string contextKey = GenerateIllusionFaceDisplayIDContextKey(raceID, genderType, helmTextureID, textureIndex,
                    colorTintID, faceIndex);
                if (SavedIllusionFaceDisplayIDsByContextKey!.ContainsKey(contextKey) == true)
                    return SavedIllusionFaceDisplayIDsByContextKey[contextKey];

                int displayDBCID = GenerateDisplayInfoID();
                SavedIllusionFaceDisplayIDsByContextKey.Add(contextKey, displayDBCID);
                Dictionary<string, string> rowValues = new Dictionary<string, string>();
                rowValues.Add("raceid", raceID.ToString());
                rowValues.Add("genderid", Convert.ToInt32(genderType).ToString());
                rowValues.Add("helmtextureid", helmTextureID.ToString());
                rowValues.Add("textureindex", textureIndex.ToString());
                rowValues.Add("colortintid", colorTintID.ToString());
                rowValues.Add("faceindex", faceIndex.ToString());
                rowValues.Add("creaturedisplay_dbcid", displayDBCID.ToString());
                FileTool.AppendRowToFileWithHeader(GetSavedIllusionFaceDisplayIDsFilePath(), "|", rowValues);
                return displayDBCID;
            }
        }

        private static void AppendSavedDBCIDsToFile(int raceID, CreatureGenderType genderType, int helmTextureID,
            int textureIndex, int faceIndex, int colorTintID, float modelTemplateScale, int modelDataDBCID,
            int displayDBCID, int soundDataDBCID)
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            rowValues.Add("raceid", raceID.ToString());
            rowValues.Add("genderid", Convert.ToInt32(genderType).ToString());
            rowValues.Add("helmtextureid", helmTextureID.ToString());
            rowValues.Add("textureindex", textureIndex.ToString());
            rowValues.Add("faceindex", faceIndex.ToString());
            rowValues.Add("colortintid", colorTintID.ToString());
            rowValues.Add("modeltemplatescale", modelTemplateScale.ToString(CultureInfo.InvariantCulture));
            rowValues.Add("creaturemodeldata_dbcid", modelDataDBCID.ToString());
            rowValues.Add("creaturedisplay_dbcid", displayDBCID.ToString());
            rowValues.Add("creaturesounddata_dbcid", soundDataDBCID.ToString());
            FileTool.AppendRowToFileWithHeader(GetSavedDBCIDsFilePath(), "|", rowValues);
        }

        public static CreatureModelTemplate CreateCreatureModelTemplateForWaypointDebugging()
        {
            lock (CreatureLock)
            {
                // Otherwise create a new one
                CreatureRace debugRace = new CreatureRace(1, CreatureGenderType.Male, 0, "Debug Male", "HUM", "ELM", 3, 1, 6, 0.2f, 1.96078f, 0, 7, false);
                CreatureModelTemplate newModelTemplate = new CreatureModelTemplate(debugRace, 0, 0, 0, 0, 0, 1);
                AllTemplatesByRaceID.Add(1, new List<CreatureModelTemplate>());
                AllTemplatesByRaceID[1].Add(newModelTemplate);
                return newModelTemplate;
            }
        }

        public static CreatureModelTemplate GetOrCreateCreatureModelTemplate(CreatureRace creatureRace, CreatureGenderType genderType, int helmTextureID,
            int textureIndex, int faceIndex, int colorTintID, float modelTemplateScale)
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
                        modelTemplate.ModelTemplateScale == modelTemplateScale)
                    {
                        return modelTemplate;
                    }
                }

                // Otherwise create a new one
                CreatureModelTemplate newModelTemplate = new CreatureModelTemplate(creatureRace, genderType, helmTextureID,
                    textureIndex, faceIndex, colorTintID, modelTemplateScale);
                AllTemplatesByRaceID[creatureRace.ID].Add(newModelTemplate);
                return newModelTemplate;
            }
        }

        public static void CreateCreatureModelTemplatesFromCreatureTemplates(List<CreatureTemplate> creatureTemplates)
        {
            // Clear the old list
            AllTemplatesByRaceID.Clear();

            // Generate model templates in response to creature templates
            foreach(CreatureTemplate creatureTemplate in creatureTemplates)
            {
                CreatureModelTemplate curModelTemplate = GetOrCreateCreatureModelTemplate(creatureTemplate.Race,
                    creatureTemplate.GenderType, creatureTemplate.HelmTextureID, creatureTemplate.TextureID, creatureTemplate.FaceID,
                    creatureTemplate.ColorTintID, creatureTemplate.ModelTemplateScale);
                creatureTemplate.ModelTemplate = curModelTemplate;
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
            ObjectModelProperties objectProperties = new ObjectModelProperties(ObjectModelProperties.GetObjectPropertiesForObject(skeletonName.ToLower()));
            objectProperties.CreatureModelTemplate = this;
            objectProperties.ModelScalePreWorldScale = Race.ModelScale;
            objectProperties.ModelLiftPreWorldScale = lift;
            ObjectModel curObject = new ObjectModel(skeletonName, objectProperties, ObjectModelType.Creature);
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
            lock (GetOutputFolderLock(Path.Combine(outputObjectFolderName, GenerateFileName())))
                objectM2.WriteToDisk(GenerateFileName(), outputFullMPQPath);

            // Place the related textures. Serialized per shared race output folder because every model template of a race copies into the same folder
            lock (GetOutputFolderLock(outputObjectFolderName))
            {
                foreach (ObjectModelTexture texture in curObject.ModelTextures)
                {
                    // Replaceable textures (illusion version faces) have no baked filename in the M2, and the actual face textures get copied in GenerateIllusionFaceData below
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

            // Illusion versions get replaceable face textures and per-face display IDs
            if (FaceIndex == ILLUSION_REPLACEABLE_FACE_INDEX)
                GenerateIllusionFaceData(curObject, inputObjectTextureFolder, outputFullMPQPath);

            Logger.WriteDebug(String.Concat("For creature template '", objectName, "', completed creating the object files"));
        }

        private void GenerateIllusionFaceData(ObjectModel curObject, string inputObjectTextureFolder, string outputFullMPQPath)
        {
            FaceHeadPieceTextureNames.Clear();
            IllusionFaceDisplayIDsByFaceIndex.Clear();
            IllusionFaceTextureVariationsByFaceIndex.Clear();

            // Collect the face texture names in head piece order, which were typed CreatureSkin1/2/3 during the object model build
            SortedDictionary<int, string> pieceTextureNamesBySkinSlot = new SortedDictionary<int, string>();
            foreach (ObjectModelTexture modelTexture in curObject.ModelTextures)
            {
                if (modelTexture.Type == ObjectModelTextureType.CreatureSkin1)
                    pieceTextureNamesBySkinSlot[0] = modelTexture.TextureName;
                else if (modelTexture.Type == ObjectModelTextureType.CreatureSkin2)
                    pieceTextureNamesBySkinSlot[1] = modelTexture.TextureName;
                else if (modelTexture.Type == ObjectModelTextureType.CreatureSkin3)
                    pieceTextureNamesBySkinSlot[2] = modelTexture.TextureName;
            }
            foreach (var pieceTextureNameBySkinSlot in pieceTextureNamesBySkinSlot)
                FaceHeadPieceTextureNames.Add(pieceTextureNameBySkinSlot.Value);

            // Helm-tier versions (HelmTextureIndex > 0) have no face materials, so they get no face displays
            if (FaceHeadPieceTextureNames.Count == 0)
                return;

            // Copy every face texture that exists for the head pieces (faces 0-9)
            lock (GetOutputFolderLock(GetCreatureModelFolderName()))
            {
                foreach (string faceZeroTextureName in FaceHeadPieceTextureNames)
                {
                    for (int faceIndex = 0; faceIndex <= 9; faceIndex++)
                    {
                        string faceTextureName = GetFaceTextureName(faceZeroTextureName, faceIndex);
                        string inputTexturePath = Path.Combine(inputObjectTextureFolder, faceTextureName + ".blp");
                        string outputFaceTexturePath = Path.Combine(outputFullMPQPath, faceTextureName + ".blp");
                        if (File.Exists(inputTexturePath) == true && File.Exists(outputFaceTexturePath) == false)
                            FileTool.CopyFile(inputTexturePath, outputFaceTexturePath);
                    }
                }
            }

            // Build a display per selectable face index
            for (int faceIndex = 1; faceIndex <= 9; faceIndex++)
            {
                bool faceTextureExistsForAnyPiece = false;
                List<string> variationTextureNames = new List<string>();
                foreach (string faceZeroTextureName in FaceHeadPieceTextureNames)
                {
                    string faceTextureName = GetFaceTextureName(faceZeroTextureName, faceIndex);
                    if (File.Exists(Path.Combine(inputObjectTextureFolder, faceTextureName + ".blp")) == true)
                    {
                        faceTextureExistsForAnyPiece = true;
                        variationTextureNames.Add(faceTextureName);
                    }
                    else
                        variationTextureNames.Add(faceZeroTextureName);
                }
                if (faceTextureExistsForAnyPiece == false)
                    continue;

                int faceDisplayID = GetOrCreateIllusionFaceDisplayID(Race.ID, GenderType, HelmTextureIndex, TextureIndex, ColorTintID, faceIndex);
                IllusionFaceDisplayIDsByFaceIndex.Add(faceIndex, faceDisplayID);
                IllusionFaceTextureVariationsByFaceIndex.Add(faceIndex, variationTextureNames);
            }
        }

        // Face textures are named '{skeleton}he00{faceIndex}{headPieceDigit}', so swap the second-to-last character
        private static string GetFaceTextureName(string faceZeroTextureName, int faceIndex)
        {
            return string.Concat(faceZeroTextureName.Substring(0, faceZeroTextureName.Length - 2), faceIndex.ToString(),
                faceZeroTextureName[faceZeroTextureName.Length - 1]);
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
