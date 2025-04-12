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
        private static bool IsFirstCreate = true;

        private static Dictionary<string, List<Int64>> GeneratedArmorPartBySourceNameThenColorID = new Dictionary<string, List<long>>();

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

        private static void BuildAndCopyTexturesForArmorPart(string subfolderName, string fileNamePrefix, string armorIdentifier, string genderIdentifier, Int64 colorPacked)
        {
            // Build the subfolder if it doesn't exist
            string workingFolderName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedEquipmentTextures", subfolderName);
            if (Directory.Exists(workingFolderName) == false)
                Directory.CreateDirectory(workingFolderName);

            // Don't generate anything if it's already been generated
            string sourceFileNameNoExt = fileNamePrefix + "_" + armorIdentifier;
            if (genderIdentifier != "U")
                sourceFileNameNoExt += "_" + genderIdentifier;
            if (GeneratedArmorPartBySourceNameThenColorID.ContainsKey(sourceFileNameNoExt) == true)
                if (GeneratedArmorPartBySourceNameThenColorID[sourceFileNameNoExt].Contains(colorPacked) == true)
                    return;

            // Copy and tint
            string targetFileNameNoExt = fileNamePrefix + "_" + armorIdentifier + "_C" + colorPacked + "_" + genderIdentifier;
            TintAndCopyTexture(workingFolderName, sourceFileNameNoExt, targetFileNameNoExt, colorPacked);

            // Save to prevent regeneration
            if (GeneratedArmorPartBySourceNameThenColorID.ContainsKey(sourceFileNameNoExt) == false)
                GeneratedArmorPartBySourceNameThenColorID.Add(sourceFileNameNoExt, new List<long>());
            GeneratedArmorPartBySourceNameThenColorID[sourceFileNameNoExt].Add(colorPacked);
        }

        private static void TintAndCopyTexture(string workingFolderName, string sourceFileNameNoExt, string targetFileNameNoExt, Int64 colorPacked)
        {
            // Generate the color if needed
            ColorRGBA colorTint = new ColorRGBA();
            if (colorPacked != 0)
            {
                colorTint.A = (byte)((colorPacked >> 24) & 0xFF);
                colorTint.R = (byte)((colorPacked >> 16) & 0xFF);
                colorTint.G = (byte)((colorPacked >> 8) & 0xFF);
                colorTint.B = (byte)(colorPacked & 0xFF);
            }

            string sourceTextureFolder = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "CustomTextures", "item", "texturecomponents");
            string targetFileNameAndPathNoExt = Path.Combine(workingFolderName, targetFileNameNoExt);

            // Copy the texture, or generate a colored version
            if (colorPacked == 0)
            {
                string sourceFileNameAndPath = Path.Combine(sourceTextureFolder, sourceFileNameNoExt + ".png");
                FileTool.CopyFile(sourceFileNameAndPath, targetFileNameAndPathNoExt + ".png");
            }
            else
            {
                ImageTool.GenerateColoredTintedTexture(sourceTextureFolder, sourceFileNameNoExt, workingFolderName, targetFileNameAndPathNoExt,
                    colorTint, ImageTool.ImageAssociationType.Clothing, false);
            }
        }

        private static bool DoTexturesExist()
        {
            string sourceTextureTestName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "CustomTextures", "item", "texturecomponents", "EQ_Robe_Chest_TU_01.png");
            return File.Exists(sourceTextureTestName);
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

            // Make temp folder if it's not there yet
            string workingFolderPath = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedEquipmentTextures");
            if (Directory.Exists(workingFolderPath) == false)
                Directory.CreateDirectory(workingFolderPath);

            // Create the item display record
            ItemDisplayInfo newItemDisplayInfo = new ItemDisplayInfo();
            newItemDisplayInfo.IconFileNameNoExt = iconFileNameNoExt;
            ItemDisplayInfos.Add(newItemDisplayInfo);

            // Test for a texture file.  If none, the graphics will be blank
            if (DoTexturesExist() == false)
            {
                if (IsFirstCreate == true)
                {
                    Logger.WriteInfo("Note: There will be no armor textures, because Assets\\CustomTextures\\item\\texturecomponents is missing.  Install a texture pack here if you desire visable armor.");
                    IsFirstCreate = false;
                }
                return newItemDisplayInfo;
            }

            // If a robe, set the texture properties and copy the textures
            if (inventoryType == ItemWOWInventoryType.Chest && materialTypeID >= 10 && materialTypeID <= 16)
            {
                // Generate the robe geometry, if needed
                int robeID = materialTypeID - 9;
                string robeIDString = "0" + robeID.ToString();

                // Build the robe parts
                BuildAndCopyTexturesForArmorPart("ArmLowerTexture", "EQ_Robe_Sleeve_AL", robeIDString, "U", colorPacked);
                BuildAndCopyTexturesForArmorPart("ArmUpperTexture", "EQ_Robe_Sleeve_AU", robeIDString, "U", colorPacked);
                BuildAndCopyTexturesForArmorPart("LegLowerTexture", "EQ_Robe_Legs_LL", robeIDString, "U", colorPacked);
                BuildAndCopyTexturesForArmorPart("LegUpperTexture", "EQ_Robe_Legs_LU", robeIDString, "U", colorPacked);
                BuildAndCopyTexturesForArmorPart("TorsoLowerTexture", "EQ_Robe_Chest_TL", robeIDString, "U", colorPacked);
                BuildAndCopyTexturesForArmorPart("TorsoUpperTexture", "EQ_Robe_Chest_TU", robeIDString, "U", colorPacked);

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
            else if (IsHeld(inventoryType) == true)
            {
                ObjectModel objectModel = GetOrCreateModelForHeldItem(itemDisplayCommonName, inventoryType);
                newItemDisplayInfo.ModelName = objectModel.Name + ".mdx";
            }
            // Armor
            else if (IsVisableArmor(inventoryType) == true && (materialTypeID <= 4 || materialTypeID == 7 || (materialTypeID >= 17 && materialTypeID <= 23)))
            {
                int armorID = materialTypeID + 1;
                if (materialTypeID == 7)
                    armorID = 3; // Kunark Chain => Classic Chain
                else if (materialTypeID >= 17)
                {
                    // Set it 1:1 for velious or higher
                    if (Configuration.GENERATE_EQ_EXPANSION_ID >= 2)
                        armorID = materialTypeID - 11;
                    // Otherwise, remap any graphics to classic counterparts
                    else
                    {
                        switch (materialTypeID)
                        {
                            case 17: armorID = 2; break; // Velious Leather => Classic Leather
                            case 18: armorID = 3; break; // Velious Chain => Classic Chain
                            case 19: armorID = 4; break; // Velious Plate => Classic Plate
                            case 20: armorID = 2; break; // Velious Leather 2 => Classic Leather
                            case 21: armorID = 3; break; // Velious Chain 2 => Classic Chain
                            case 22: armorID = 4; break; // Velious Plate 2 => Classic Plate
                            case 23: armorID = 5; break; // Velious Monk => Classic Monk
                            default: armorID = 1; break; // Default to cloth
                        }
                    }
                }
                string armorIDString;
                if (armorID < 10)
                    armorIDString = "0" + armorID.ToString();
                else
                    armorIDString = armorID.ToString();
                switch (inventoryType)
                {
                    case ItemWOWInventoryType.Chest:
                        {
                            newItemDisplayInfo.ArmorTexture4 = "EQ_Armor_Chest_TU_" + armorIDString + "_C" + colorPacked;
                            BuildAndCopyTexturesForArmorPart("TorsoUpperTexture", "EQ_Armor_Chest_TU", armorIDString, "M", colorPacked);
                            BuildAndCopyTexturesForArmorPart("TorsoUpperTexture", "EQ_Armor_Chest_TU", armorIDString, "F", colorPacked);
                            newItemDisplayInfo.ArmorTexture5 = "EQ_Armor_Chest_TL_" + armorIDString + "_C" + colorPacked;
                            BuildAndCopyTexturesForArmorPart("TorsoLowerTexture", "EQ_Armor_Chest_TL", armorIDString, "M", colorPacked);
                            BuildAndCopyTexturesForArmorPart("TorsoLowerTexture", "EQ_Armor_Chest_TL", armorIDString, "F", colorPacked);

                            // TODO: See if it's possible to amke this work for shoulder
                            newItemDisplayInfo.ArmorTexture1 = "EQ_Armor_Arms_AU_" + armorIDString + "_C" + colorPacked;
                            BuildAndCopyTexturesForArmorPart("ArmUpperTexture", "EQ_Armor_Arms_AU", armorIDString, "M", colorPacked);
                            BuildAndCopyTexturesForArmorPart("ArmUpperTexture", "EQ_Armor_Arms_AU", armorIDString, "F", colorPacked);

                        } break;
                    case ItemWOWInventoryType.Legs:
                        {
                            newItemDisplayInfo.ArmorTexture6 = "EQ_Armor_Legs_LU_" + armorIDString + "_C" + colorPacked;
                            BuildAndCopyTexturesForArmorPart("LegUpperTexture", "EQ_Armor_Legs_LU", armorIDString, "M", colorPacked);
                            BuildAndCopyTexturesForArmorPart("LegUpperTexture", "EQ_Armor_Legs_LU", armorIDString, "F", colorPacked);
                            newItemDisplayInfo.ArmorTexture7 = "EQ_Armor_Legs_LL_" + armorIDString + "_C" + colorPacked;
                            BuildAndCopyTexturesForArmorPart("LegLowerTexture", "EQ_Armor_Legs_LL", armorIDString, "M", colorPacked);
                            BuildAndCopyTexturesForArmorPart("LegLowerTexture", "EQ_Armor_Legs_LL", armorIDString, "F", colorPacked);
                        } break;
                    case ItemWOWInventoryType.Feet:
                        {
                            newItemDisplayInfo.ArmorTexture7 = "EQ_Armor_Feet_LL_" + armorIDString + "_C" + colorPacked;
                            BuildAndCopyTexturesForArmorPart("LegLowerTexture", "EQ_Armor_Feet_LL", armorIDString, "M", colorPacked);
                            BuildAndCopyTexturesForArmorPart("LegLowerTexture", "EQ_Armor_Feet_LL", armorIDString, "F", colorPacked);
                            newItemDisplayInfo.ArmorTexture8 = "EQ_Armor_Feet_FO_" + armorIDString + "_C" + colorPacked;
                            BuildAndCopyTexturesForArmorPart("FootTexture", "EQ_Armor_Feet_FO", armorIDString, "M", colorPacked);
                            BuildAndCopyTexturesForArmorPart("FootTexture", "EQ_Armor_Feet_FO", armorIDString, "F", colorPacked);
                        } break;
                    case ItemWOWInventoryType.Hands:
                        {
                            newItemDisplayInfo.ArmorTexture3 = "EQ_Armor_Hand_HA_" + armorIDString + "_C" + colorPacked;
                            BuildAndCopyTexturesForArmorPart("HandTexture", "EQ_Armor_Hand_HA", armorIDString, "M", colorPacked);
                            BuildAndCopyTexturesForArmorPart("HandTexture", "EQ_Armor_Hand_HA", armorIDString, "F", colorPacked);
                        } break;
                    case ItemWOWInventoryType.Shoulder:
                        {
                            // Doesn't work.  Look for a solution
                            //newItemDisplayInfo.ArmorTexture1 = "EQ_Armor_Arms_AU_" + armorIDString + "_C" + colorPacked;
                            //BuildAndCopyTexturesForArmorPart("ArmUpperTexture", "EQ_Armor_Arms_AU", armorIDString, "M", colorPacked);
                            //BuildAndCopyTexturesForArmorPart("ArmUpperTexture", "EQ_Armor_Arms_AU", armorIDString, "F", colorPacked);
                        } break;
                    case ItemWOWInventoryType.Wrists:
                        {
                            newItemDisplayInfo.ArmorTexture2 = "EQ_Armor_Wrist_AL_" + armorIDString + "_C" + colorPacked;
                            BuildAndCopyTexturesForArmorPart("ArmLowerTexture", "EQ_Armor_Wrist_AL", armorIDString, "M", colorPacked);
                            BuildAndCopyTexturesForArmorPart("ArmLowerTexture", "EQ_Armor_Wrist_AL", armorIDString, "F", colorPacked);
                        } break;
                    default:
                        {
                            Logger.WriteError("Unable to set DisplayInfo for equipment with name '" + itemDisplayCommonName + "' due to unhandled inventory type of '" + inventoryType.ToString() + "'");
                        } break;              
                }
            }

            return newItemDisplayInfo;
        }

        static private ObjectModel GetOrCreateModelForHeldItem(string itemDisplayCommonName, ItemWOWInventoryType inventoryType)
        {
            // Make sure things are loaded
            string equipmentSourceBasePath = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "equipment");
            if (staticFileNamesByCommonName.Count == 0)
            {
                Logger.WriteDebug("Loading equipment actors_static.txt...");
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
                Logger.WriteDebug("Loading equipment actors_skeletal.txt...");
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
                Logger.WriteDebug("Creating equipment model object for '" + itemDisplayCommonName + "'...");

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
                        FileTool.CopyFile(inputTextureName, outputTextureName);
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

        static private bool IsVisableArmor(ItemWOWInventoryType inventoryType)
        {
            if (inventoryType != ItemWOWInventoryType.Shoulder && inventoryType != ItemWOWInventoryType.Chest && inventoryType != ItemWOWInventoryType.Legs &&
                inventoryType != ItemWOWInventoryType.Wrists && inventoryType != ItemWOWInventoryType.Hands && inventoryType != ItemWOWInventoryType.Feet)
            {
                return false;
            }
            return true;
        }
    }
}
