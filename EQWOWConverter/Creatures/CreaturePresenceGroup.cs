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

namespace EQWOWConverter.Creatures
{
    internal class CreaturePresenceGroup
    {
        private static List<CreaturePresenceGroup> PresenceGroupList = new List<CreaturePresenceGroup>();
        private static HashSet<int> PresenceGroupMemberEQCreatureTemplateIDs = new HashSet<int>();
        private static readonly object PresenceGroupLock = new object();

        public const int MAX_OTHER_CREATURE_TEMPLATES = 4;

        public int ID;
        public string ZoneShortName = string.Empty;
        public int PrimaryEQCreatureTemplateID;
        public List<int> OtherEQCreatureTemplateIDs = new List<int>();
        public int CheckIntervalMS;
        public string Comment = string.Empty;

        public static bool IsCreatureInAPresenceGroup(int eqCreatureTemplateID)
        {
            lock (PresenceGroupLock)
            {
                if (PresenceGroupList.Count == 0)
                    PopulatePresenceGroupList();
                return PresenceGroupMemberEQCreatureTemplateIDs.Contains(eqCreatureTemplateID);
            }
        }

        public static List<CreaturePresenceGroup> GetPresenceGroupList()
        {
            lock (PresenceGroupLock)
            {
                if (PresenceGroupList.Count == 0)
                    PopulatePresenceGroupList();
                return PresenceGroupList;
            }
        }

        private static void PopulatePresenceGroupList()
        {
            string presenceGroupsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreaturePresenceGroups.csv");
            Logger.WriteDebug("Populating Creature Presence Group list via file '" + presenceGroupsFile + "'");
            if (File.Exists(presenceGroupsFile) == false)
            {
                Logger.WriteError("Could not load creature presence groups, file did not exist at '" + presenceGroupsFile + "'");
                return;
            }
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(presenceGroupsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                CreaturePresenceGroup newPresenceGroup = new CreaturePresenceGroup();
                newPresenceGroup.ID = int.Parse(columns["id"]);
                newPresenceGroup.ZoneShortName = columns["zone"].ToLower().Trim();
                newPresenceGroup.PrimaryEQCreatureTemplateID = int.Parse(columns["primary_eq_creature_template_id"]);
                for (int i = 1; i <= MAX_OTHER_CREATURE_TEMPLATES; i++)
                {
                    string idString = columns[string.Concat("other_eq_creature_template_id_", i)].Trim();
                    if (idString.Length == 0)
                        continue;
                    newPresenceGroup.OtherEQCreatureTemplateIDs.Add(int.Parse(idString));
                }
                newPresenceGroup.CheckIntervalMS = int.Parse(columns["check_interval_ms"]);
                newPresenceGroup.Comment = columns["comment"];
                if (newPresenceGroup.OtherEQCreatureTemplateIDs.Count == 0)
                {
                    Logger.WriteError("CreaturePresenceGroup row '" + newPresenceGroup.ID + "' names no other creature templates, so it would keep its primary up permanently");
                    continue;
                }
                if (newPresenceGroup.CheckIntervalMS <= 0)
                {
                    Logger.WriteError("CreaturePresenceGroup row '" + newPresenceGroup.ID + "' has a check_interval_ms of '" + newPresenceGroup.CheckIntervalMS + "', which would never fire");
                    continue;
                }
                PresenceGroupList.Add(newPresenceGroup);
                PresenceGroupMemberEQCreatureTemplateIDs.Add(newPresenceGroup.PrimaryEQCreatureTemplateID);
                foreach (int otherEQCreatureTemplateID in newPresenceGroup.OtherEQCreatureTemplateIDs)
                    PresenceGroupMemberEQCreatureTemplateIDs.Add(otherEQCreatureTemplateID);
            }
        }
    }
}
