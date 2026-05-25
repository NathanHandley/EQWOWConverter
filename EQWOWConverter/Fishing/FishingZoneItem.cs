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

using EQWOWConverter.Items;
using EQWOWConverter.Zones;

namespace EQWOWConverter.Fishing
{
    internal class FishingZoneItem
    {
        private static Dictionary<string, List<FishingZoneItem>> FishingZoneItemsByZoneShortName = new Dictionary<string, List<FishingZoneItem>>();
        private static Dictionary<string, int> FishingWOWSkillLevelByZoneShortName = new Dictionary<string, int>();
        private static readonly object FishingLock = new object();

        public string ZoneShortName = string.Empty;
        public int SkillLevelEQ = 0;
        public int SkillLevelWOW = 0;
        public int EQItemID = 0;
        public int WOWItemTemplateID = -1;
        public int ChanceRelative = 100;
        public float ChanceAbsolute = 0;

        public static Dictionary<string, List<FishingZoneItem>> GetFishingZoneItemsByZoneShortName()
        {
            lock (FishingLock)
            {
                if (FishingZoneItemsByZoneShortName.Count == 0)
                    PopulateZoneItemList();
                return FishingZoneItemsByZoneShortName;
            }
        }

        public static Dictionary<string, int> GetWOWFishingLevelByZoneShortName()
        {
            lock (FishingLock)
            {
                if (FishingWOWSkillLevelByZoneShortName.Count == 0)
                    PopulateZoneItemList();
                return FishingWOWSkillLevelByZoneShortName;
            }
        }

        private static void PopulateZoneItemList()
        {
            FishingZoneItemsByZoneShortName.Clear();
            FishingWOWSkillLevelByZoneShortName.Clear();

            // Used to skip any bad items
            SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID = ItemTemplate.GetItemTemplatesByEQDBIDs();

            // Used to skip any unloaded zones
            Dictionary<string, ZoneProperties> zonePropertiesByShortName = ZoneProperties.GetZonePropertyListByShortName();

            // Load the list
            string zoneItemsListFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "FishingZoneItems.csv");
            Logger.WriteDebug("Populating Fishing Zone Items List via file '" + zoneItemsListFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(zoneItemsListFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip any invalid expansion rows
                int minExpansion = int.Parse(columns["min_expansion"]);
                if (minExpansion != -1 && minExpansion > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;

                FishingZoneItem zoneItem = new FishingZoneItem();
                zoneItem.ZoneShortName = columns["zone_short_name"].Trim().ToLower();
                if (zonePropertiesByShortName.ContainsKey(zoneItem.ZoneShortName) == false)
                {
                    Logger.WriteDebug("PopulateZoneItemList for FishingZoneItem skipping EQItemID with ID '", zoneItem.EQItemID.ToString(), "' because zone with short name '", zoneItem.ZoneShortName, "' doesn't exist");
                    continue;
                }
                zoneItem.EQItemID = int.Parse(columns["eq_itemid"]);
                if (itemTemplatesByEQDBID.ContainsKey(zoneItem.EQItemID) == false)
                {
                    Logger.WriteError("PopulateZoneItemList for FishingZoneItem failed because no EQItemID existed with ID '", zoneItem.EQItemID.ToString(), "'");
                    continue;
                }
                itemTemplatesByEQDBID[zoneItem.EQItemID].IsFished = true;
                zoneItem.WOWItemTemplateID = itemTemplatesByEQDBID[zoneItem.EQItemID].WOWEntryID;
                zoneItem.SkillLevelEQ = int.Parse(columns["skill_level"]);
                zoneItem.SkillLevelWOW = Math.Min(Math.Max(Convert.ToInt32((float)zoneItem.SkillLevelEQ * Configuration.FISHING_SKILL_CONVERSION_MOD), 1), 450);
                zoneItem.ChanceRelative = int.Parse(columns["chance"]);
                if (FishingZoneItemsByZoneShortName.ContainsKey(zoneItem.ZoneShortName) == false)
                {
                    FishingZoneItemsByZoneShortName.Add(zoneItem.ZoneShortName, new List<FishingZoneItem>());
                    FishingWOWSkillLevelByZoneShortName.Add(zoneItem.ZoneShortName, zoneItem.SkillLevelWOW);
                }
                FishingZoneItemsByZoneShortName[zoneItem.ZoneShortName].Add(zoneItem);
                FishingWOWSkillLevelByZoneShortName[zoneItem.ZoneShortName] = Math.Max(FishingWOWSkillLevelByZoneShortName[zoneItem.ZoneShortName], zoneItem.SkillLevelWOW);
            }

            // Fill any zone gaps
            foreach (string zoneShortName in zonePropertiesByShortName.Keys)
            {
                if (FishingZoneItemsByZoneShortName.ContainsKey(zoneShortName) == false)
                {
                    FishingZoneItemsByZoneShortName.Add(zoneShortName, new List<FishingZoneItem>());
                    FishingWOWSkillLevelByZoneShortName.Add(zoneShortName, Convert.ToInt32(40f * Configuration.FISHING_SKILL_CONVERSION_MOD));
                }
            }

            // Add the common fishing items in all zones
            foreach (var fishingZoneItemsByZoneShortName in FishingZoneItemsByZoneShortName)
            {
                // Fresh Fish
                FishingZoneItem freshFishFishingZoneItem = new FishingZoneItem();
                freshFishFishingZoneItem.ZoneShortName = fishingZoneItemsByZoneShortName.Key;
                freshFishFishingZoneItem.EQItemID = 13019;
                freshFishFishingZoneItem.WOWItemTemplateID = itemTemplatesByEQDBID[freshFishFishingZoneItem.EQItemID].WOWEntryID;
                freshFishFishingZoneItem.SkillLevelEQ = 0;
                freshFishFishingZoneItem.SkillLevelWOW = 0;
                freshFishFishingZoneItem.ChanceRelative = 40;
                fishingZoneItemsByZoneShortName.Value.Add(freshFishFishingZoneItem);
                itemTemplatesByEQDBID[freshFishFishingZoneItem.EQItemID].IsFished = true;

                // Fish Scales
                FishingZoneItem fishScalesFishingZoneItem = new FishingZoneItem();
                fishScalesFishingZoneItem.ZoneShortName = fishingZoneItemsByZoneShortName.Key;
                fishScalesFishingZoneItem.EQItemID = 13076;
                fishScalesFishingZoneItem.WOWItemTemplateID = itemTemplatesByEQDBID[fishScalesFishingZoneItem.EQItemID].WOWEntryID;
                fishScalesFishingZoneItem.SkillLevelEQ = 0;
                fishScalesFishingZoneItem.SkillLevelWOW = 0;
                fishScalesFishingZoneItem.ChanceRelative = 20;
                fishingZoneItemsByZoneShortName.Value.Add(fishScalesFishingZoneItem);
                itemTemplatesByEQDBID[fishScalesFishingZoneItem.EQItemID].IsFished = true;
            }

            // Calculate the absolute drop rates for output rows
            foreach (List<FishingZoneItem> fishingZoneItems in FishingZoneItemsByZoneShortName.Values)
            {
                int totalRelativeChance = 0;
                foreach (FishingZoneItem fishingZoneItem in fishingZoneItems)
                    totalRelativeChance += fishingZoneItem.ChanceRelative;
                float relativeToAbsoluteMod = 100f / (float)totalRelativeChance;
                float remainingAbsoluteMod = 100f;
                for (int i = 0; i < fishingZoneItems.Count; i++)
                {
                    if (i < fishingZoneItems.Count-1)
                    {
                        fishingZoneItems[i].ChanceAbsolute = float.Round(fishingZoneItems[i].ChanceRelative * relativeToAbsoluteMod, 2);
                        remainingAbsoluteMod -= fishingZoneItems[i].ChanceAbsolute;
                    }
                    else
                        fishingZoneItems[i].ChanceAbsolute = float.Round(remainingAbsoluteMod, 2);
                }
            }
        }
    }
}
