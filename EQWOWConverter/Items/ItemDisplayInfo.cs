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

        public int ItemDisplayInfoDBCID = 0;
        public string IconFileNameNoExt = string.Empty;
        public string ModelName = string.Empty;
        public string ModelTexture1 = string.Empty;
        public ObjectModel? EquipmentModel = null;

        public ItemDisplayInfo()
        {
            ItemDisplayInfoDBCID = CURRENT_DBCID_ITEMDISPLAYINFO;
            CURRENT_DBCID_ITEMDISPLAYINFO++;
        }

        public static ItemDisplayInfo CreateItemDisplayInfo(string itemDisplayCommonName, string iconFileNameNoExt, ItemWOWInventoryType inventoryType)
        {
            // Pull if it already exists
            string modelFileName = itemDisplayCommonName + ".mdx";
            foreach(ItemDisplayInfo itemDisplayInfo in ItemDisplayInfos)
            {
                if (itemDisplayInfo.IconFileNameNoExt == iconFileNameNoExt && itemDisplayInfo.ModelName == modelFileName)
                    return itemDisplayInfo;
            }

            // Grab the model
            ObjectModel objectModel = GetOrCreateModelForHeldItem(itemDisplayCommonName, inventoryType);

            // Create the item display record
            ItemDisplayInfo newItemDisplayInfo = new ItemDisplayInfo();
            newItemDisplayInfo.IconFileNameNoExt = iconFileNameNoExt;
            newItemDisplayInfo.ModelName = modelFileName;
            ItemDisplayInfos.Add(newItemDisplayInfo);
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
            if (inventoryType != ItemWOWInventoryType.OneHand && inventoryType != ItemWOWInventoryType.Ranged && inventoryType != ItemWOWInventoryType.Shield &&
                inventoryType != ItemWOWInventoryType.TwoHand && inventoryType != ItemWOWInventoryType.HeldInOffHand && inventoryType != ItemWOWInventoryType.RangedRight &&
                inventoryType != ItemWOWInventoryType.Thrown)
            {
                itemDisplayCommonName = "eq_it63";
            }

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
    }
}
