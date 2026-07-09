//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

using System.Text;

namespace EQWOWConverter
{
    internal class IDGenerationTool
    {
        private class IDGenerationCategory
        {
            public string Name = string.Empty;
            public int StartID = 0;
            public int EndID = 0;
            public int CurrentID = 0;
            public Dictionary<string, int> SavedIDsByContextKey = new Dictionary<string, int>();
        }

        private static readonly object IDGenerationLock = new object();
        private static Dictionary<string, IDGenerationCategory> CategoriesByName = new Dictionary<string, IDGenerationCategory>();
        private static bool SavedIDsAreLoaded = false;

        public static int GenerateID(string idName, params string[] contextKeyParts)
        {
            lock (IDGenerationLock)
            {
                InitializeIfNeeded();
                if (CategoriesByName.ContainsKey(idName) == false)
                    throw new Exception("IDGenerationTool error, unknown ID of name '" + idName + "'");
                if (contextKeyParts.Length == 0)
                    throw new Exception("IDGenerationTool error, for name '" + idName + "' there were no contextKeyParts");
                IDGenerationCategory category = CategoriesByName[idName];

                string contextKey = GenerateContextKey(contextKeyParts);
                if (category.SavedIDsByContextKey.ContainsKey(contextKey) == true)
                    return category.SavedIDsByContextKey[contextKey];

                int returnID = category.CurrentID;
                if (returnID > category.EndID)
                    Logger.WriteError("Generated '" + idName + "' ID of '" + returnID.ToString() + "' is above the category end ID of '" + category.EndID.ToString() + "' which will cause issues");
                category.CurrentID++;
                category.SavedIDsByContextKey.Add(contextKey, returnID);
                AppendSavedIDToFile(category, contextKey, returnID);
                return returnID;
            }
        }

        private static void InitializeIfNeeded()
        {
            if (CategoriesByName.Count > 0)
                return;
            Initialize("SpellID", Configuration.DBCID_SPELL_ID_GENERATED_START, Configuration.DBCID_SPELL_ID_END);
            Initialize("ItemDisplayInfoID", Configuration.DBCID_ITEMDISPLAYINFO_START, int.MaxValue);
            Initialize("GameObjectDisplayInfoID", Configuration.DBCID_GAMEOBJECTDISPLAYINFO_ID_START, int.MaxValue);
            Initialize("TotemCategoryID", Convert.ToInt32(Configuration.DBCID_TOTEMCATEGORY_ID_START), int.MaxValue);
            Initialize("TaxiPathID", Configuration.DBCID_TAXIPATH_ID_START, int.MaxValue);
            Initialize("SoundEntryID", Configuration.DBCID_SOUNDENTRIES_ID_START, int.MaxValue);
            Initialize("CreatureModelDataID", Configuration.DBCID_CREATUREMODELDATA_ID_START, int.MaxValue);
            Initialize("CreatureDisplayInfoID", Configuration.DBCID_CREATUREDISPLAYINFO_ID_START, Configuration.DBCID_CREATUREDISPLAYINFO_ID_END);
            Initialize("CreatureSoundDataID", Configuration.DBCID_CREATURESOUNDDATA_ID_START, int.MaxValue);
            Initialize("ZoneWMOGroupID", Convert.ToInt32(Configuration.DBCID_WMOAREATABLE_WMOGROUPID_START), int.MaxValue);
            LoadSavedIDs();
        }

        private static void Initialize(string idName, int startID, int endID)
        {
            if (CategoriesByName.ContainsKey(idName) == true)
            {
                Logger.WriteError("IDGenerationTool tried to initialize ID name '" + idName + "' more than once");
                return;
            }
            IDGenerationCategory category = new IDGenerationCategory();
            category.Name = idName;
            category.StartID = startID;
            category.EndID = endID;
            category.CurrentID = startID;
            CategoriesByName.Add(idName, category);
        }

        private static string GenerateContextKey(string[] contextKeyParts)
        {
            StringBuilder keySB = new StringBuilder();
            for (int i = 0; i < contextKeyParts.Length; i++)
            {
                if (i > 0)
                    keySB.Append("~");
                keySB.Append(contextKeyParts[i]);
            }

            // Trailing blank parts are trimmed so optional context key parts don't change the key
            return keySB.ToString().TrimEnd('~');
        }

        private static string GetSavedIDsFilePath()
        {
            return Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", Configuration.IDGENERATION_FILE_NAME);
        }

        private static void LoadSavedIDs()
        {
            if (SavedIDsAreLoaded == true)
                return;
            SavedIDsAreLoaded = true;

            string savedIDsFilePath = GetSavedIDsFilePath();
            if (File.Exists(savedIDsFilePath) == false)
            {
                Logger.WriteDebug("No saved generated IDs file found at '" + savedIDsFilePath + "', so all IDs will be newly generated");
                return;
            }

            Logger.WriteDebug("Loading saved generated IDs via file '" + savedIDsFilePath + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(savedIDsFilePath, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                string categoryName = columns["category"];
                if (CategoriesByName.ContainsKey(categoryName) == false)
                {
                    Logger.WriteError("Unknown category '" + categoryName + "' found in '" + savedIDsFilePath + "', skipping the row");
                    continue;
                }
                IDGenerationCategory category = CategoriesByName[categoryName];

                string contextKey = columns["contextkey"].TrimEnd('~');
                if (category.SavedIDsByContextKey.ContainsKey(contextKey) == true)
                {
                    Logger.WriteError("Duplicate '" + categoryName + "' context key '" + contextKey + "' found in '" + savedIDsFilePath + "', skipping the duplicate row");
                    continue;
                }

                int savedID = int.Parse(columns["id"]);
                category.SavedIDsByContextKey.Add(contextKey, savedID);

                // Ensure newly generated IDs never collide with previously saved ones
                if (savedID >= category.CurrentID)
                    category.CurrentID = savedID + 1;
            }
        }

        private static void AppendSavedIDToFile(IDGenerationCategory category, string contextKey, int id)
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            rowValues.Add("category", category.Name);
            rowValues.Add("contextkey", contextKey);
            rowValues.Add("id", id.ToString());
            FileTool.AppendRowToFileWithHeader(GetSavedIDsFilePath(), "|", rowValues);
        }
    }
}
