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
using EQWOWConverter.ObjectModels;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Items
{
    internal class ItemDisplayInfo
    {
        public static List<ItemDisplayInfo> ItemDisplayInfos = new List<ItemDisplayInfo>();
        private static int CURRENT_DBCID_ITEMDISPLAYINFO = Configuration.DBCID_ITEMDISPLAYINFO_START;
        private static Dictionary<string, ObjectModel> ObjectModelsByEQItemDisplayFileName = new Dictionary<string, ObjectModel>();
        private static Dictionary<string, string> staticFileNamesByCommonName = new Dictionary<string, string>();
        private static Dictionary<string, string> skeletalFileNamesByCommonName = new Dictionary<string, string>();
        private static Dictionary<int, List<Int64>> GeneratedRobesByIDThenColor = new Dictionary<int, List<Int64>>();

        public int ItemDisplayInfoDBCID = 0;
        public string IconFileNameNoExt = string.Empty;
        public string ModelName = string.Empty;
        public string ModelTexture1 = string.Empty;
        public string ModelTexture2 = string.Empty;
        public int GeosetGroup1 = 0;
        public int GeosetGroup2 = 0;
        public int GeosetGroup3 = 0;
        public string ArmorTexture1 = string.Empty;
        public string ArmorTexture2 = string.Empty;
        public string ArmorTexture3 = string.Empty;
        public string ArmorTexture4 = string.Empty;
        public string ArmorTexture5 = string.Empty;
        public string ArmorTexture6 = string.Empty;
        public string ArmorTexture7 = string.Empty;
        public string ArmorTexture8 = string.Empty;
        public ObjectModel? EquipmentModel = null;

        public ItemDisplayInfo()
        {
            ItemDisplayInfoDBCID = CURRENT_DBCID_ITEMDISPLAYINFO;
            CURRENT_DBCID_ITEMDISPLAYINFO++;
        }

        private static void BuildAndCopyTexturesForRobe(int robeID, Int64 colorPacked)
        {
            // Don't do anything if this was already generated
            if (GeneratedRobesByIDThenColor.ContainsKey(robeID) == true)
                if (GeneratedRobesByIDThenColor[robeID].Contains(colorPacked) == true)
                    return;

            // Make temp folder if it's not there yet
            string workingFolderPath = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedEquipmentTextures");
            if (Directory.Exists(workingFolderPath) == false)
                Directory.CreateDirectory(workingFolderPath);

            // Build the robe parts
            BuildAndCopyTexturesForRobePart("ArmLowerTexture", "EQ_Robe_Sleeve_AL", robeID, false, colorPacked);
            BuildAndCopyTexturesForRobePart("ArmUpperTexture", "EQ_Robe_Sleeve_AU", robeID, false, colorPacked);
            BuildAndCopyTexturesForRobePart("LegLowerTexture", "EQ_Robe_Legs_LL", robeID, false, colorPacked);
            BuildAndCopyTexturesForRobePart("LegUpperTexture", "EQ_Robe_Legs_LU", robeID, false, colorPacked);
            BuildAndCopyTexturesForRobePart("TorsoLowerTexture", "EQ_Robe_Chest_TL", robeID, true, colorPacked);
            BuildAndCopyTexturesForRobePart("TorsoUpperTexture", "EQ_Robe_Chest_TU", robeID, true, colorPacked);

            // Add it as generated
            if (GeneratedRobesByIDThenColor.ContainsKey(robeID) == false)
                GeneratedRobesByIDThenColor.Add(robeID, new List<Int64>());
            GeneratedRobesByIDThenColor[robeID].Add(colorPacked);
        }

        private static void BuildAndCopyTexturesForRobePart(string subfolderName, string fileNamePrefix, int robeID, 
            bool doGenerateBothMaleAndFemale, Int64 colorPacked)
        {
            // Build the subfolder if it doesn't exist
            string workingFolderName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedEquipmentTextures", subfolderName);
            if (Directory.Exists(workingFolderName) == false)
                Directory.CreateDirectory(workingFolderName);

            // Generate the color if needed
            ColorRGBA colorTint = new ColorRGBA();
            if (colorPacked != 0)
            {
                colorTint.A = (byte)((colorPacked >> 24) & 0xFF);
                colorTint.R = (byte)((colorPacked >> 16) & 0xFF);
                colorTint.G = (byte)((colorPacked >> 8) & 0xFF);
                colorTint.B = (byte)(colorPacked & 0xFF);
            }

            // Depending on the config, either 1 or 2 needs to be generated
            string sourceFileNameNoExt = fileNamePrefix + "_0" + robeID.ToString();
            if (doGenerateBothMaleAndFemale == true)
            {
                TintAndCopyTexture(workingFolderName, sourceFileNameNoExt, "M", colorPacked, colorTint);
                TintAndCopyTexture(workingFolderName, sourceFileNameNoExt, "F", colorPacked, colorTint);
            }
            else
                TintAndCopyTexture(workingFolderName, sourceFileNameNoExt, "U", colorPacked, colorTint);
        }

        private static void TintAndCopyTexture(string workingFolderName, string sourceFileNameNoExt, string genderIdentifier, Int64 colorPacked, ColorRGBA colorTint)
        {
            string sourceTextureFolder = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "CustomTextures", "item", "texturecomponents");
            string targetFileNameAndPathNoExt = Path.Combine(workingFolderName, sourceFileNameNoExt + "_C" + colorPacked + "_" + genderIdentifier);

            // Copy the texture, or generate a colored version
            if (colorPacked == 0)
            {
                string sourceFileNameAndPath = Path.Combine(sourceTextureFolder, sourceFileNameNoExt + ".png");
                File.Copy(sourceFileNameAndPath, targetFileNameAndPathNoExt + ".png", true);
            }
            else
            {
                ImageTool.GenerateColoredTintedTexture(sourceTextureFolder, sourceFileNameNoExt, workingFolderName, targetFileNameAndPathNoExt,
                    colorTint, ImageTool.ImageAssociationType.Clothing, false);
            }
        }

        public static ItemDisplayInfo CreateItemDisplayInfo(string itemDisplayCommonName, string iconFileNameNoExt, 
            ItemWOWInventoryType inventoryType, int materialTypeID, Int64 colorPacked)
        {
            // Pull if it already exists
            string modelFileName = itemDisplayCommonName + ".mdx";
            foreach(ItemDisplayInfo itemDisplayInfo in ItemDisplayInfos)
            {
                if (itemDisplayInfo.IconFileNameNoExt == iconFileNameNoExt && itemDisplayInfo.ModelName == modelFileName)
                    return itemDisplayInfo;
            }

            // Create the item display record
            ItemDisplayInfo newItemDisplayInfo = new ItemDisplayInfo();
            newItemDisplayInfo.IconFileNameNoExt = iconFileNameNoExt;
            ItemDisplayInfos.Add(newItemDisplayInfo);

            // If a robe, set the texture properties and copy the textures
            if (inventoryType == ItemWOWInventoryType.Chest && materialTypeID >= 10 && materialTypeID <= 16)
            {
                // Generate the robe geometry, if needed
                int robeID = materialTypeID - 9;
                BuildAndCopyTexturesForRobe(robeID, colorPacked);
                string robeIDString = "0" + robeID.ToString();

                // Robes use geosets 1 and 3
                newItemDisplayInfo.GeosetGroup1 = 1;
                newItemDisplayInfo.GeosetGroup3 = 1;

                // Set the armor texture names
                newItemDisplayInfo.ArmorTexture1 = "EQ_Robe_Sleeve_AU_" + robeIDString + "_C" + colorPacked;
                newItemDisplayInfo.ArmorTexture2 = "EQ_Robe_Sleeve_AL_" + robeIDString + "_C" + colorPacked;
                newItemDisplayInfo.ArmorTexture4 = "EQ_Robe_Chest_TU_" + robeIDString + "_C" + colorPacked;
                newItemDisplayInfo.ArmorTexture5 = "EQ_Robe_Chest_TL_" + robeIDString + "_C" + colorPacked;
                newItemDisplayInfo.ArmorTexture6 = "EQ_Robe_Legs_LU_" + robeIDString + "_C" + colorPacked;
                newItemDisplayInfo.ArmorTexture7 = "EQ_Robe_Legs_LL_" + robeIDString + "_C" + colorPacked;
            }

            // Held objects have models
            if (IsHeld(inventoryType) == true)
            {
                ObjectModel objectModel = GetOrCreateModelForHeldItem(itemDisplayCommonName, inventoryType);
                newItemDisplayInfo.ModelName = objectModel.Name + ".mdx";
            }
            return newItemDisplayInfo;
        }

        static private ObjectModel GetOrCreateModelForHeldItem(string itemDisplayCommonName, ItemWOWInventoryType inventoryType)
        {
            // Make sure things are loaded
            string equipmentSourceBasePath = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "equipment");
            if (staticFileNamesByCommonName.Count == 0)
            {
                Logger.WriteDetail("Loading equipment actors_static.txt...");
                string staticListFileName = Path.Combine(equipmentSourceBasePath, "actors_static.txt");
                foreach (string line in FileTool.ReadAllStringLinesFromFile(staticListFileName, false, true))
                {
                    string[] blocks = line.Split(',');
                    string curCommonName = "eq_" + blocks[0].Trim().ToLower();
                    string curFileName = blocks[1].Trim().ToLower();
                    staticFileNamesByCommonName.Add(curCommonName, curFileName);
                }
            }
            if (skeletalFileNamesByCommonName.Count == 0)
            {
                Logger.WriteDetail("Loading equipment actors_skeletal.txt...");
                string skeletalListFileName = Path.Combine(equipmentSourceBasePath, "actors_skeletal.txt");
                foreach (string line in FileTool.ReadAllStringLinesFromFile(skeletalListFileName, false, true))
                {
                    string[] blocks = line.Split(',');
                    string curCommonName = "eq_" + blocks[0].Trim().ToLower();
                    string curFileName = blocks[1].Trim().ToLower();
                    skeletalFileNamesByCommonName.Add(curCommonName, curFileName);
                }
            }

            // Fallback if it's not held
            if (IsHeld(inventoryType) == false)
                itemDisplayCommonName = "eq_it63";

            // Get or load the model
            if (ObjectModelsByEQItemDisplayFileName.ContainsKey(itemDisplayCommonName) == true)
                return ObjectModelsByEQItemDisplayFileName[itemDisplayCommonName];
            else
            {
                Logger.WriteDetail("Creating equipment model object for '" + itemDisplayCommonName + "'...");

                // Fallback the name if it's not known, or it's not a held type
                string eqAssetFileName = "it63"; // Fallback/default
                ObjectModelType modelType = ObjectModelType.EquipmentHeld;
                if (staticFileNamesByCommonName.ContainsKey(itemDisplayCommonName) == true)
                {
                    eqAssetFileName = staticFileNamesByCommonName[itemDisplayCommonName];
                    modelType = ObjectModelType.EquipmentHeld;
                }
                if (skeletalFileNamesByCommonName.ContainsKey(itemDisplayCommonName) == true)
                {
                    eqAssetFileName = skeletalFileNamesByCommonName[itemDisplayCommonName];
                    modelType = ObjectModelType.EquipmentHeld;
                }

                // Create a new model
                ObjectModel equipmentModel = new ObjectModel(itemDisplayCommonName, new ObjectModels.Properties.ObjectModelProperties(), modelType);
                equipmentModel.LoadEQObjectFromFile(equipmentSourceBasePath, eqAssetFileName);

                // Determine the output folder based on item type
                string relativeMPQFolderName = string.Empty;
                if (inventoryType == ItemWOWInventoryType.Shield)
                    relativeMPQFolderName = Path.Combine("ITEM", "OBJECTCOMPONENTS", "Shield");
                else
                    relativeMPQFolderName = Path.Combine("ITEM", "OBJECTCOMPONENTS", "WEAPON");
                string fullOutputFolderName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", relativeMPQFolderName);

                // Create the output files
                M2 objectM2 = new M2(equipmentModel, relativeMPQFolderName);
                objectM2.WriteToDisk(itemDisplayCommonName, fullOutputFolderName);

                // Copy the textures
                foreach (ObjectModelTexture texture in equipmentModel.ModelTextures)
                {
                    string inputTextureName = Path.Combine(equipmentSourceBasePath, "Textures", texture.TextureName + ".blp");
                    string outputTextureName = Path.Combine(fullOutputFolderName, texture.TextureName + ".blp");
                    if (Path.Exists(inputTextureName) == false)
                        Logger.WriteError("Error copying texture '" + inputTextureName + "' for '" + itemDisplayCommonName + "', as it could not be found. Did you do the 'convert png to blp' step?");
                    else
                        File.Copy(inputTextureName, outputTextureName, true);
                }

                // Save it on the list and return it
                ObjectModelsByEQItemDisplayFileName.Add(itemDisplayCommonName, equipmentModel);
                return equipmentModel;
            }
        }

        static private bool IsHeld(ItemWOWInventoryType inventoryType)
        {
            // Fallback if it's not held
            if (inventoryType != ItemWOWInventoryType.OneHand && inventoryType != ItemWOWInventoryType.Ranged && inventoryType != ItemWOWInventoryType.Shield &&
                inventoryType != ItemWOWInventoryType.TwoHand && inventoryType != ItemWOWInventoryType.HeldInOffHand && inventoryType != ItemWOWInventoryType.RangedRight &&
                inventoryType != ItemWOWInventoryType.Thrown)
            {
                return false;
            }
            return true;
        }
    }
}
