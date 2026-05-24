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

namespace EQWOWConverter.Forage
{
    internal class ForageZoneItem
    {
        private static List<ForageZoneItem> ForageZoneItems = new List<ForageZoneItem>();
        private static readonly object ForageLock = new object();

        public string ZoneShortName = string.Empty;
        public ForageZoneItemType ForageType = ForageZoneItemType.Other;
        public int WOWMapID = -1;
        public int EQItemID = 0;
        public int WOWItemTemplateID = -1;
        public int Chance = 100;

        public static List<ForageZoneItem> GetAllZoneItems()
        {
            lock (ForageLock)
            {
                if (ForageZoneItems.Count == 0)
                    PopulateZoneItemList();
                return ForageZoneItems;
            }
        }

        private static void PopulateZoneItemList()
        {
            ForageZoneItems.Clear();

            string zoneItemsListFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ForageZoneItems.csv");
            Logger.WriteDebug("Populating Zone Items List via file '" + zoneItemsListFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(zoneItemsListFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip any invalid expansion rows
                int minExpansion = int.Parse(columns["min_expansion"]);
                int maxExpansion = int.Parse(columns["max_expansion"]);
                if (minExpansion != -1 && minExpansion > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;
                if (maxExpansion != -1 && maxExpansion < Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;

                ForageZoneItem zoneItem = new ForageZoneItem();
                zoneItem.ZoneShortName = columns["zone_short_name"];
                zoneItem.EQItemID = int.Parse(columns["eq_itemid"]);
                zoneItem.Chance = int.Parse(columns["chance"]);
                ForageZoneItems.Add(zoneItem);
            }

        }
    }
}
