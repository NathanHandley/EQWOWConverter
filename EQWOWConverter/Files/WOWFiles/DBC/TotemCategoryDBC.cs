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

namespace EQWOWConverter.WOWFiles
{
    internal class TotemCategoryDBC : DBCFile
    {
        private static UInt32 CUR_ID = Configuration.DBCID_TOTEMCATEGORY_ID_START;
        private static readonly object TotemCategoryLock = new object();
        private static Dictionary<string, UInt32>? SavedIDsByContextKey = null;

        public void AddRow(UInt32 id, string name, int categoryID, int categoryMask)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddUInt32(id); // ID;
            newRow.AddStringLang(name); // Name_Lang
            newRow.AddInt32(categoryID); // TotemCategoryType
            newRow.AddInt32(categoryMask); // TotemCategoryMask
            Rows.Add(newRow);
        }

        public static UInt32 GetUniqueID(string itemName)
        {
            lock (TotemCategoryLock)
            {
                LoadSavedIDsIfNeeded();
                if (SavedIDsByContextKey!.ContainsKey(itemName) == true)
                    return SavedIDsByContextKey[itemName];

                UInt32 newID = CUR_ID;
                CUR_ID++;
                SavedIDsByContextKey.Add(itemName, newID);
                AppendSavedIDToFile(itemName, newID);
                return newID;
            }
        }

        private static string GetSavedIDsFilePath()
        {
            return Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TotemCategoryIDs.csv");
        }

        private static void LoadSavedIDsIfNeeded()
        {
            if (SavedIDsByContextKey != null)
                return;
            SavedIDsByContextKey = new Dictionary<string, UInt32>();

            string savedIDsFilePath = GetSavedIDsFilePath();
            if (File.Exists(savedIDsFilePath) == false)
            {
                Logger.WriteDebug("No saved totem category IDs file found at '" + savedIDsFilePath + "', so all IDs will be newly generated");
                return;
            }

            Logger.WriteDebug("Loading saved totem category IDs via file '" + savedIDsFilePath + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(savedIDsFilePath, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                string contextKey = columns["contextkey"];
                if (SavedIDsByContextKey.ContainsKey(contextKey) == true)
                {
                    Logger.WriteError("Duplicate context key '" + contextKey + "' found in '" + savedIDsFilePath + "', skipping the duplicate row");
                    continue;
                }

                UInt32 totemCategoryID = UInt32.Parse(columns["totemcategory_id"]);
                SavedIDsByContextKey.Add(contextKey, totemCategoryID);

                // Ensure newly generated IDs never collide with previously saved ones
                if (totemCategoryID >= CUR_ID)
                    CUR_ID = totemCategoryID + 1;
            }
        }

        private static void AppendSavedIDToFile(string contextKey, UInt32 totemCategoryID)
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            rowValues.Add("contextkey", contextKey);
            rowValues.Add("totemcategory_id", totemCategoryID.ToString());
            FileTool.AppendRowToFileWithHeader(GetSavedIDsFilePath(), "|", rowValues);
        }
    }
}
