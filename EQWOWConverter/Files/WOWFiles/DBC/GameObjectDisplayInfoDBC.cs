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

namespace EQWOWConverter.WOWFiles
{
    internal class GameObjectDisplayInfoDBC : DBCFile
    {
        private static int CUR_ID = Configuration.DBCID_GAMEOBJECTDISPLAYINFO_ID_START;
        private static readonly object GameObjectDisplayLock = new object();
        private static Dictionary<string, int>? SavedIDsByContextKey = null;

        public void AddRow(int id, string modelNameAndRelativePath, BoundingBox boundingBox, int openSoundEntryID = 0, int closeSoundEntryID = 0)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id);
            newRow.AddString(modelNameAndRelativePath);
            newRow.AddInt32(0); // Stand SoundEntries.ID
            newRow.AddInt32(openSoundEntryID); // Open SoundEntries.ID
            newRow.AddInt32(0); // Loop SoundEntries.ID
            newRow.AddInt32(closeSoundEntryID); // Close SoundEntries.ID
            newRow.AddInt32(0); // Destroy SoundEntries.ID
            newRow.AddInt32(0); // Opened SoundEntries.ID
            newRow.AddInt32(0); // Custom0 SoundEntries.ID
            newRow.AddInt32(0); // Custom1 SoundEntries.ID
            newRow.AddInt32(0); // Custom2 SoundEntries.ID
            newRow.AddInt32(0); // Custom3 SoundEntries.ID
            newRow.AddFloat(boundingBox.BottomCorner.X); // GeoBox Min X
            newRow.AddFloat(boundingBox.BottomCorner.Y); // GeoBox Min Y
            newRow.AddFloat(boundingBox.BottomCorner.Z); // GeoBox Min Z
            newRow.AddFloat(boundingBox.TopCorner.X); // GeoBox Max X
            newRow.AddFloat(boundingBox.TopCorner.Y); // GeoBox Max Y
            newRow.AddFloat(boundingBox.TopCorner.Z); // GeoBox Max Z
            newRow.AddInt32(0); // ObjectEffectPackageID (?)
            Rows.Add(newRow);
        }

        public static int GenerateID(string contextKeyPart1, string contextKeyPart2, string contextKeyPart3 = "", string contextKeyPart4 = "", string contextKeyPart5 = "")
        {
            string contextKey = string.Concat(contextKeyPart1, "~", contextKeyPart2, "~", contextKeyPart3, "~", contextKeyPart4, "~", contextKeyPart5);
            lock (GameObjectDisplayLock)
            {
                LoadSavedIDsIfNeeded();
                if (SavedIDsByContextKey!.ContainsKey(contextKey) == true)
                    return SavedIDsByContextKey[contextKey];

                int id = CUR_ID;
                CUR_ID++;
                SavedIDsByContextKey.Add(contextKey, id);
                AppendSavedIDToFile(contextKey, id);
                return id;
            }
        }

        private static string GetSavedIDsFilePath()
        {
            return Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "GameObjectDisplayInfoIDs.csv");
        }

        private static void LoadSavedIDsIfNeeded()
        {
            if (SavedIDsByContextKey != null)
                return;
            SavedIDsByContextKey = new Dictionary<string, int>();

            string savedIDsFilePath = GetSavedIDsFilePath();
            if (File.Exists(savedIDsFilePath) == false)
            {
                Logger.WriteDebug("No saved game object display info IDs file found at '" + savedIDsFilePath + "', so all IDs will be newly generated");
                return;
            }

            Logger.WriteDebug("Loading saved game object display info IDs via file '" + savedIDsFilePath + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(savedIDsFilePath, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                string contextKey = columns["contextkey"];
                if (SavedIDsByContextKey.ContainsKey(contextKey) == true)
                {
                    Logger.WriteError("Duplicate context key '" + contextKey + "' found in '" + savedIDsFilePath + "', skipping the duplicate row");
                    continue;
                }

                int displayInfoID = int.Parse(columns["gameobjectdisplayinfo_id"]);
                SavedIDsByContextKey.Add(contextKey, displayInfoID);

                // Ensure newly generated IDs never collide with previously saved ones
                if (displayInfoID >= CUR_ID)
                    CUR_ID = displayInfoID + 1;
            }
        }

        private static void AppendSavedIDToFile(string contextKey, int displayInfoID)
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            rowValues.Add("contextkey", contextKey);
            rowValues.Add("gameobjectdisplayinfo_id", displayInfoID.ToString());
            FileTool.AppendRowToFileWithHeader(GetSavedIDsFilePath(), "|", rowValues);
        }
    }
}
