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

namespace EQWOWConverter.Items
{
    internal class ItemKeyException
    {
        private static HashSet<int> DisabledKeyWOWItemTemplateIDs = new HashSet<int>();
        private static bool IsPopulated = false;
        private static readonly object KeyExceptionLock = new object();

        public static HashSet<int> GetDisabledKeyWOWItemTemplateIDs()
        {
            lock (KeyExceptionLock)
            {
                if (IsPopulated == false)
                    PopulateDisabledKeyWOWItemTemplateIDs();
                return DisabledKeyWOWItemTemplateIDs;
            }
        }

        public static bool IsKeyDisabled(int wowItemTemplateID)
        {
            if (Configuration.ITEMS_LOCK_KEY_DISABLED_EXCEPTIONS_ENABLED == false)
                return false;
            if (wowItemTemplateID <= 0)
                return false;
            return GetDisabledKeyWOWItemTemplateIDs().Contains(wowItemTemplateID);
        }

        private static void PopulateDisabledKeyWOWItemTemplateIDs()
        {
            DisabledKeyWOWItemTemplateIDs.Clear();

            string keyExceptionFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ItemKeyExceptions.csv");
            Logger.WriteDebug(string.Concat("Populating Item Key Exceptions via file '", keyExceptionFileName, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(keyExceptionFileName, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                if (columns["Enabled"].Trim() != "1")
                    continue;
                int wowItemTemplateID = int.Parse(columns["WOWItemTemplateID"].Trim());
                if (wowItemTemplateID <= 0)
                {
                    Logger.WriteError("ItemKeyException had a row with an invalid WOWItemTemplateID of '", columns["WOWItemTemplateID"], "', so the row was skipped");
                    continue;
                }
                DisabledKeyWOWItemTemplateIDs.Add(wowItemTemplateID);
            }

            IsPopulated = true;
        }
    }
}
