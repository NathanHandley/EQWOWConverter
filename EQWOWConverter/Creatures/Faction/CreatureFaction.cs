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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Creatures
{
    internal class CreatureFaction
    {
        public static int CURRENT_ID = Configuration.CONFIG_DBCID_FACTIONTEMPLATE_ID_START;

        private static Dictionary<int, CreatureFaction> CreatureFactionsByWOWFactionID = new Dictionary<int, CreatureFaction>();
        private static Dictionary<int, int> CreatureWOWFactionTemplateIDByEQFactionID = new Dictionary<int, int>();
        private static Dictionary<int, int> CreatureWOWFactionIDByEQFactionID = new Dictionary<int, int>();
        private static Dictionary<int, List<CreatureFactionKillReward>> CreatureFactionKillRewardsByEQNPCFactionID = new Dictionary<int, List<CreatureFactionKillReward>>();

        public int FactionID = 0;
        public int FactionTemplateID = -1;
        public int ParentFactionID = 0;
        public int ReputationIndex = 0;
        public string Name = string.Empty;
        public int BaseRep = 0;
        public string Description = string.Empty;

        public static int GetRootFactionParentWOWFactionID()
        {
            if (CreatureFactionsByWOWFactionID.Count == 0)
                PopulateFactionData();

            foreach (CreatureFaction creatureFaction in CreatureFactionsByWOWFactionID.Values)
            {
                if (creatureFaction.Name == Configuration.CONFIG_CREATURE_FACTION_ROOT_NAME)
                    return creatureFaction.FactionID;
            }

            Logger.WriteError("CreatureFaction - could not find faction with name 'Everquest'");
            return 0;
        }

        public static int GetWOWFactionTemplateIDForEQFactionID(int eqFactionID)
        {
            if (CreatureWOWFactionTemplateIDByEQFactionID.Count == 0)
                PopulateFactionData();
            if (CreatureWOWFactionTemplateIDByEQFactionID.ContainsKey(eqFactionID) == true)
                return CreatureWOWFactionTemplateIDByEQFactionID[eqFactionID];
            else
            {
                Logger.WriteDetail("Creature Faction - No wow faction template ID mapped to eq faction ID '" + eqFactionID.ToString() + "'");
                return -1;
            }
        }

        public static Dictionary<int, CreatureFaction> GetCreatureFactionsByFactionID()
        {
            if (CreatureFactionsByWOWFactionID.Count == 0)
                PopulateFactionData();
            return CreatureFactionsByWOWFactionID;
        }

        public static List<CreatureFactionKillReward> GetCreatureFactionKillRewards(int eqNPCFactionID)
        {
            if (CreatureFactionKillRewardsByEQNPCFactionID.Count == 0)
                PopulateFactionData();
            if (CreatureFactionKillRewardsByEQNPCFactionID.ContainsKey(eqNPCFactionID) == false)
                return new List<CreatureFactionKillReward>();
            else
            {
                // Condense the list so that there are no duplicates
                List<CreatureFactionKillReward> returnRewards = new List<CreatureFactionKillReward>();
                foreach(CreatureFactionKillReward killRewardCandidate in CreatureFactionKillRewardsByEQNPCFactionID[eqNPCFactionID])
                {
                    bool killRewardFound = false;
                    for (int i = returnRewards.Count - 1; i >= 0; i--)
                    {
                        // Take the highest or lowest faction reward
                        CreatureFactionKillReward killRewardStage = returnRewards[i];
                        if (killRewardStage.WOWFactionID == killRewardCandidate.WOWFactionID)
                        {
                            if (Math.Abs(killRewardCandidate.KillRewardValue) > Math.Abs(killRewardStage.KillRewardValue))
                                returnRewards.RemoveAt(i);
                            else
                            {
                                killRewardFound = true;
                                continue;
                            }
                        }
                    }
                    if (killRewardFound == false)
                        returnRewards.Add(killRewardCandidate);
                }
                return returnRewards;
            }   
        }

        private static void PopulateFactionData()
        {
            // Load in faction list
            string factionListFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureFactions.csv");
            Logger.WriteDetail("Populating creature factions via file '" + factionListFile + "'");
            List<Dictionary<string, string>> listRows = FileTool.ReadAllRowsFromFileWithHeader(factionListFile, "|");
            foreach (Dictionary<string, string> columns in listRows)
            {
                // Load the row
                CreatureFaction newCreatureFaction = new CreatureFaction();
                newCreatureFaction.FactionID = int.Parse(columns["FactionID"]);
                newCreatureFaction.ReputationIndex = int.Parse(columns["ReputationIndex"]);
                newCreatureFaction.Name = columns["Name"];
                newCreatureFaction.BaseRep = int.Parse(columns["Base"]);
                newCreatureFaction.Description = columns["Description"];

                // Generate a faction template ID if it's not the root
                if (newCreatureFaction.Name != Configuration.CONFIG_CREATURE_FACTION_ROOT_NAME)
                {
                    // Generate the unique ID
                    int curID = CURRENT_ID;
                    CURRENT_ID++;
                    newCreatureFaction.FactionTemplateID = curID;
                }

                CreatureFactionsByWOWFactionID.Add(newCreatureFaction.FactionID, newCreatureFaction);
            }

            // Update the parents for these factions
            int parentFactionID = GetRootFactionParentWOWFactionID();
            if (parentFactionID > 0)
            {
                foreach (CreatureFaction creatureFaction in CreatureFactionsByWOWFactionID.Values)
                    if (creatureFaction.FactionID != parentFactionID && creatureFaction.ReputationIndex > -1)
                        creatureFaction.ParentFactionID = parentFactionID;
            }
            else
                Logger.WriteError("Creature Faction - Could not assign parent ID to faction since no parent faction ID could be found");

            // Load the faction mappings
            string factionMapListFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureFactionMap.csv");
            Logger.WriteDetail("Populating creature factions map via file '" + factionMapListFile + "'");
            List<Dictionary<string, string>> mapRows = FileTool.ReadAllRowsFromFileWithHeader(factionMapListFile, "|");
            foreach (Dictionary<string, string> columns in mapRows)
            {
                // Load the row
                int eqfactionID = int.Parse(columns["EQFactionID"]);
                int wowFactionID = int.Parse(columns["WOWFactionID"]);
                if(CreatureFactionsByWOWFactionID.ContainsKey(wowFactionID) == false)
                {
                    Logger.WriteError("Creature Faction: Attempted to map an eq faction to wow, but there was no wowFactionID of '" + wowFactionID + "' in the CreatureFactionMap");
                    continue;
                }
                else
                {
                    CreatureFaction curFaction = CreatureFactionsByWOWFactionID[wowFactionID];
                    int wowFactionTemplateID = curFaction.FactionTemplateID;
                    if (CreatureWOWFactionIDByEQFactionID.ContainsKey(eqfactionID) == true)
                    {
                        Logger.WriteError("Creature Faction - Attempted to map eqFactionID of '" + eqfactionID + "' to wowFactionTemplateID of '" + wowFactionTemplateID + "' but a mapping already existed for the eqFactionID");
                        continue;
                    }
                    CreatureWOWFactionIDByEQFactionID.Add(eqfactionID, wowFactionID);
                    CreatureWOWFactionTemplateIDByEQFactionID.Add(eqfactionID, wowFactionTemplateID);
                }
            }

            // Load in faction kill reward list
            string factionKillRewardFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureFactionKillRewards.csv");
            Logger.WriteDetail("Populating creature faction kill rewards via file '" + factionKillRewardFile + "'");
            List<Dictionary<string, string>> factionKillRewardRows = FileTool.ReadAllRowsFromFileWithHeader(factionKillRewardFile, "|");
            foreach (Dictionary<string, string> columns in factionKillRewardRows)
            {
                // Load the row
                CreatureFactionKillReward creatureFactionKillReward = new CreatureFactionKillReward();
                creatureFactionKillReward.EQNPCFactionID = int.Parse(columns["npc_faction_id"]);
                creatureFactionKillReward.EQFactionID = int.Parse(columns["faction_id"]);
                creatureFactionKillReward.SortOrder = int.Parse(columns["sort_order"]);
                creatureFactionKillReward.KillRewardValue = int.Parse(columns["value"]);

                // Add it
                if (CreatureWOWFactionIDByEQFactionID.ContainsKey(creatureFactionKillReward.EQFactionID))
                {
                    creatureFactionKillReward.WOWFactionID = CreatureWOWFactionIDByEQFactionID[creatureFactionKillReward.EQFactionID];
                    if (CreatureFactionKillRewardsByEQNPCFactionID.ContainsKey(creatureFactionKillReward.EQNPCFactionID) == false)
                        CreatureFactionKillRewardsByEQNPCFactionID.Add(creatureFactionKillReward.EQNPCFactionID, new List<CreatureFactionKillReward>());
                    CreatureFactionKillRewardsByEQNPCFactionID[creatureFactionKillReward.EQNPCFactionID].Add(creatureFactionKillReward);
                }
            }
        }
    }
}
