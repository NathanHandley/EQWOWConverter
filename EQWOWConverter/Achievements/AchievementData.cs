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

using EQWOWConverter.Creatures;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Achievements
{
    internal class AchievementData
    {
        private static SortedDictionary<int, AchievementData> AchievementsByAchievementID = new SortedDictionary<int, AchievementData>();
        private static Dictionary<string, AchievementData> InstanceClearAchievementsByZoneAndInstanceType = new Dictionary<string, AchievementData>();
        private static readonly object AchievementReadLock = new object();

        public int AchievementID;
        public int ParentCategoryID; // Achievement_Category.dbc ID
        public AchievementType Type = AchievementType.Unknown;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public int Points;
        public int IconSpellEQID = -1;
        public int IconItemEQID = -1;
        public string ZoneShortName = string.Empty; // Only used by zone-specific types (like InstanceClear)
        public string Data1 = string.Empty; // Meaning varies by type
        public List<int> CriteriaCreatureWOWIDs = new List<int>(); // Exracted from Data2+ fields for some achievemetns
        public List<CreatureTemplate> CriteriaCreatureTemplates = new List<CreatureTemplate>(); // Resolved creature templates from above

        public static SortedDictionary<int, AchievementData> GetAchievementsByAchievementID()
        {
            lock (AchievementReadLock)
            {
                if (AchievementsByAchievementID.Count == 0)
                    PopulateAchievementList();
                return AchievementsByAchievementID;
            }
        }

        public static AchievementData? GetInstanceClearAchievement(string zoneShortName, string instanceType)
        {
            lock (AchievementReadLock)
            {
                if (AchievementsByAchievementID.Count == 0)
                    PopulateAchievementList();
                string lookupKey = string.Concat(zoneShortName.ToLower().Trim(), "~", instanceType);
                if (InstanceClearAchievementsByZoneAndInstanceType.ContainsKey(lookupKey) == false)
                    return null;
                return InstanceClearAchievementsByZoneAndInstanceType[lookupKey];
            }
        }

        public int GetIconDBCID()
        {
            // Can use either type of icon
            if (IconItemEQID >= 0)
                return SpellIconDBC.GetDBCIDForItemIconID(IconItemEQID);
            if (IconSpellEQID >= 0)
                return SpellIconDBC.GetDBCIDForSpellIconID(IconSpellEQID);
            Logger.WriteError("AchievementData with ID '" + AchievementID + "' has no populated icon column, so the achievement icon will be invalid");
            return SpellIconDBC.GetDBCIDForItemIconID(0);
        }

        public static void PopulateCriteriaCreatureTemplates()
        {
            SortedDictionary<int, AchievementData> achievementsByID = GetAchievementsByAchievementID();
            Dictionary<int, CreatureTemplate> creatureTemplatesByWOWID = CreatureTemplate.GetCreatureTemplateListByWOWID();
            foreach (AchievementData achievement in achievementsByID.Values)
            {
                foreach (int creatureWOWID in achievement.CriteriaCreatureWOWIDs)
                {
                    if (creatureTemplatesByWOWID.ContainsKey(creatureWOWID) == false)
                    {
                        Logger.WriteError("AchievementData with ID '" + achievement.AchievementID + "' references creature template WOW ID '" + creatureWOWID + "' which does not exist, so it is skipped");
                        continue;
                    }
                    achievement.AddCriteriaCreatureTemplateIfNew(creatureTemplatesByWOWID[creatureWOWID]);
                }

                if (achievement.Type == AchievementType.InstanceClear)
                {
                    // Boss kills grant the raid instance lock through the core's CREATURE_FLAG_EXTRA_INSTANCE_BIND
                    foreach (CreatureTemplate creatureTemplate in achievement.CriteriaCreatureTemplates)
                        creatureTemplate.BindsRaidInstanceOnKill = true;
                    if (achievement.CriteriaCreatureTemplates.Count == 0)
                        Logger.WriteError("AchievementData with ID '" + achievement.AchievementID + "' has no criteria creatures in its Data columns, so the achievement can never complete");
                }
            }
        }

        private void AddCriteriaCreatureTemplateIfNew(CreatureTemplate creatureTemplate)
        {
            foreach (CreatureTemplate existingTemplate in CriteriaCreatureTemplates)
                if (existingTemplate.WOWCreatureTemplateID == creatureTemplate.WOWCreatureTemplateID)
                    return;
            CriteriaCreatureTemplates.Add(creatureTemplate);
        }

        private static void PopulateAchievementList()
        {
            string achievementsFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "AchievementData.csv");
            Logger.WriteDebug("Populating achievement data list via file '" + achievementsFileName + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(achievementsFileName, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                AchievementData achievement = new AchievementData();
                achievement.AchievementID = int.Parse(columns["AchievementID"]);
                achievement.ParentCategoryID = int.Parse(columns["ParentCategoryID"]);
                achievement.Name = columns["Name"];
                achievement.Description = columns["Description"];
                achievement.Points = int.Parse(columns["Points"]);
                achievement.IconSpellEQID = int.Parse(columns["IconSpellEQID"]);
                achievement.IconItemEQID = int.Parse(columns["IconItemEQID"]);
                achievement.ZoneShortName = columns["ZoneShortName"].ToLower().Trim();
                achievement.Data1 = columns["Data1"].Trim();
                for (int dataIndex = 2; columns.ContainsKey("Data" + dataIndex.ToString()) == true; dataIndex++)
                {
                    string dataValue = columns["Data" + dataIndex.ToString()].Trim();
                    if (dataValue.Length > 0)
                        achievement.CriteriaCreatureWOWIDs.Add(int.Parse(dataValue));
                }

                string achievementTypeValue = columns["AchievementType"].Trim();
                switch (achievementTypeValue)
                {
                    case "InstanceClear": achievement.Type = AchievementType.InstanceClear; break;
                    default:
                        {
                            Logger.WriteError("AchievementData with ID '" + achievement.AchievementID + "' has unhandled AchievementType of '" + achievementTypeValue + "', so the row is skipped");
                            continue;
                        }
                }

                if (AchievementsByAchievementID.ContainsKey(achievement.AchievementID) == true)
                {
                    Logger.WriteError("AchievementData.csv has more than one row with AchievementID '" + achievement.AchievementID + "', so extra rows are skipped");
                    continue;
                }
                AchievementsByAchievementID.Add(achievement.AchievementID, achievement);

                if (achievement.Type == AchievementType.InstanceClear)
                {
                    string lookupKey = string.Concat(achievement.ZoneShortName, "~", achievement.Data1);
                    if (InstanceClearAchievementsByZoneAndInstanceType.ContainsKey(lookupKey) == true)
                    {
                        Logger.WriteError("AchievementData.csv has more than one InstanceClear row for zone '" + achievement.ZoneShortName + "' and instance type '" + achievement.Data1 + "', so extra rows are ignored for zone lookups");
                        continue;
                    }
                    InstanceClearAchievementsByZoneAndInstanceType.Add(lookupKey, achievement);
                }
            }
        }
    }
}
