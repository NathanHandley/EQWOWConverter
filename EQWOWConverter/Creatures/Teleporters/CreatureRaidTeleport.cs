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

namespace EQWOWConverter.Creatures.Teleporters
{
    internal class CreatureRaidTeleport
    {
        public int CreatureTemplateWOWID;
        public int GossipMenuOptionID;
        public string OptionText = string.Empty;
        public string DestZoneShortName = string.Empty;
        public float DestXPosition;
        public float DestYPosition;
        public float DestZPosition;
        public float DestOrientation;

        private static SortedDictionary<int, List<CreatureRaidTeleport>> RaidTeleportsByCreatureTemplateWOWID = new SortedDictionary<int, List<CreatureRaidTeleport>>();
        public static readonly object RaidTeleportLock = new object();

        public static SortedDictionary<int, List<CreatureRaidTeleport>> GetRaidTeleportsByCreatureTemplateWOWID()
        {
            lock (RaidTeleportLock)
            {
                if (RaidTeleportsByCreatureTemplateWOWID.Count == 0)
                    LoadRaidTeleports();
                return RaidTeleportsByCreatureTemplateWOWID;
            }
        }

        private static void LoadRaidTeleports()
        {
            RaidTeleportsByCreatureTemplateWOWID.Clear();
            string raidTeleportsFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureRaidTeleports.csv");
            Logger.WriteDebug("Populating creature raid teleport list via file '" + raidTeleportsFileName + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(raidTeleportsFileName, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                CreatureRaidTeleport raidTeleport = new CreatureRaidTeleport();
                raidTeleport.CreatureTemplateWOWID = int.Parse(columns["CreatureTemplateWOWID"]);
                raidTeleport.GossipMenuOptionID = int.Parse(columns["GossipMenuOptionID"]);
                raidTeleport.OptionText = columns["OptionText"];
                raidTeleport.DestZoneShortName = columns["DestZoneShortName"].ToLower().Trim();
                raidTeleport.DestXPosition = float.Parse(columns["DestX"]) * Configuration.GENERATE_WORLD_SCALE;
                raidTeleport.DestYPosition = float.Parse(columns["DestY"]) * Configuration.GENERATE_WORLD_SCALE;
                raidTeleport.DestZPosition = float.Parse(columns["DestZ"]) * Configuration.GENERATE_WORLD_SCALE;
                raidTeleport.DestOrientation = float.Parse(columns["DestOrientation"]);
                if (RaidTeleportsByCreatureTemplateWOWID.ContainsKey(raidTeleport.CreatureTemplateWOWID) == false)
                    RaidTeleportsByCreatureTemplateWOWID.Add(raidTeleport.CreatureTemplateWOWID, new List<CreatureRaidTeleport>());
                foreach (CreatureRaidTeleport existingRaidTeleport in RaidTeleportsByCreatureTemplateWOWID[raidTeleport.CreatureTemplateWOWID])
                {
                    if (existingRaidTeleport.GossipMenuOptionID == raidTeleport.GossipMenuOptionID)
                        Logger.WriteError("CreatureRaidTeleports.csv has a duplicate GossipMenuOptionID of '" + raidTeleport.GossipMenuOptionID + "' for CreatureTemplateWOWID '" + raidTeleport.CreatureTemplateWOWID + "'");
                }
                RaidTeleportsByCreatureTemplateWOWID[raidTeleport.CreatureTemplateWOWID].Add(raidTeleport);
            }

            // Menu options render in option ID order, so keep the lists sorted that way
            foreach (var raidTeleportsForCreature in RaidTeleportsByCreatureTemplateWOWID)
                raidTeleportsForCreature.Value.Sort(CompareRaidTeleportsByGossipMenuOptionID);
        }

        private static int CompareRaidTeleportsByGossipMenuOptionID(CreatureRaidTeleport a, CreatureRaidTeleport b)
        {
            return a.GossipMenuOptionID.CompareTo(b.GossipMenuOptionID);
        }
    }
}
