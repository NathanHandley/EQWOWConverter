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

        // Illusion version (FaceIndex 99) face data, populated during CreateModelFiles.  Face 0 head piece texture names are stored in head piece order, and per selectable face the display ID and texture variation names (one per head piece)
        public List<string> FaceHeadPieceTextureNames = new List<string>();
        public SortedDictionary<int, int> IllusionFaceDisplayIDsByFaceIndex = new SortedDictionary<int, int>();
        public SortedDictionary<int, List<string>> IllusionFaceTextureVariationsByFaceIndex = new SortedDictionary<int, List<string>>();

        public CreatureModelTemplate(CreatureRace creatureRace, CreatureGenderType genderType, int helmTextureID,
            int textureIndex, int faceIndex, int colorTintID, float modelTemplateScale)
        {
            string raceIDString = creatureRace.ID.ToString();
            string genderIDString = Convert.ToInt32(genderType).ToString();
            string scaleString = modelTemplateScale.ToString(CultureInfo.InvariantCulture);
            DBCCreatureModelDataID = IDGenerationTool.GenerateID("CreatureModelDataID", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
            DBCCreatureDisplayID = IDGenerationTool.GenerateID("CreatureDisplayInfoID", "modeltemplate", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);
            DBCCreatureSoundDataID = IDGenerationTool.GenerateID("CreatureSoundDataID", raceIDString, genderIDString, helmTextureID.ToString(), textureIndex.ToString(), faceIndex.ToString(), colorTintID.ToString(), scaleString);

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

        public bool DoBakeModelTemplateScaleIntoGeometry()
        {
            return FaceIndex == ILLUSION_REPLACEABLE_FACE_INDEX && ModelTemplateScale > Configuration.GENERATE_FLOAT_EPSILON;
        }

        private static int GetOrCreateIllusionFaceDisplayID(int raceID, CreatureGenderType genderType, int helmTextureID,
            int textureIndex, int colorTintID, int faceIndex)
        {
            return IDGenerationTool.GenerateID("CreatureDisplayInfoID", "illusionface", raceID.ToString(), Convert.ToInt32(genderType).ToString(), helmTextureID.ToString(), textureIndex.ToString(), colorTintID.ToString(), faceIndex.ToString());
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
            if (DoBakeModelTemplateScaleIntoGeometry() == true)
                objectProperties.AdditionalScaleMultiplier *= ModelTemplateScale;
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

        public static int CompareCreatureModelTemplatesByModelDataID(CreatureModelTemplate template1, CreatureModelTemplate template2)
        {
            return template1.DBCCreatureModelDataID.CompareTo(template2.DBCCreatureModelDataID);
        }
    }
}
