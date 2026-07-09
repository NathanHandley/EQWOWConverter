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
    internal class TaxiPathDBC : DBCFile
    {
        private static int CUR_ID = Configuration.DBCID_TAXIPATH_ID_START;
        private static readonly object TaxiPathLock = new object();
        private static Dictionary<string, int>? SavedIDsByContextKey = null;

        public void AddRow(int id)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(0); // FromTaxiNode
            newRow.AddInt32(0); // ToTaxiNode
            newRow.AddInt32(0); // Cost
            Rows.Add(newRow);
        }

        public static int GenerateID(string taxiName)
        {
            lock (TaxiPathLock)
            {
                LoadSavedIDsIfNeeded();
                if (SavedIDsByContextKey!.ContainsKey(taxiName) == true)
                    return SavedIDsByContextKey[taxiName];

                int id = CUR_ID;
                CUR_ID++;
                SavedIDsByContextKey.Add(taxiName, id);
                AppendSavedIDToFile(taxiName, id);
                return id;
            }
        }

        private static string GetSavedIDsFilePath()
        {
            return Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TaxiPathIDs.csv");
        }

        private static void LoadSavedIDsIfNeeded()
        {
            if (SavedIDsByContextKey != null)
                return;
            SavedIDsByContextKey = new Dictionary<string, int>();

            string savedIDsFilePath = GetSavedIDsFilePath();
            if (File.Exists(savedIDsFilePath) == false)
            {
                Logger.WriteDebug("No saved taxi path IDs file found at '" + savedIDsFilePath + "', so all IDs will be newly generated");
                return;
            }

            Logger.WriteDebug("Loading saved taxi path IDs via file '" + savedIDsFilePath + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(savedIDsFilePath, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                string contextKey = columns["contextkey"];
                if (SavedIDsByContextKey.ContainsKey(contextKey) == true)
                {
                    Logger.WriteError("Duplicate context key '" + contextKey + "' found in '" + savedIDsFilePath + "', skipping the duplicate row");
                    continue;
                }

                int taxiPathID = int.Parse(columns["taxipath_id"]);
                SavedIDsByContextKey.Add(contextKey, taxiPathID);

                // Ensure newly generated IDs never collide with previously saved ones
                if (taxiPathID >= CUR_ID)
                    CUR_ID = taxiPathID + 1;
            }
        }

        private static void AppendSavedIDToFile(string contextKey, int taxiPathID)
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            rowValues.Add("contextkey", contextKey);
            rowValues.Add("taxipath_id", taxiPathID.ToString());
            FileTool.AppendRowToFileWithHeader(GetSavedIDsFilePath(), "|", rowValues);
        }
    }
}
