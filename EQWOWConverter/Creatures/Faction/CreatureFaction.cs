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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Creatures
{
    // WOW Faction Bands:
    // - Hated: -42,000 to -6,000
    // - Hostile: -5,999 to -3,000
    // - Unfriendly: -2,999 to -1
    // - Neutral: 0 to 2,999
    // - Friendly: 3,000 to 5,999
    // - Honored: 6,000 to 11,999
    // - Revered: 12,000 to 20,999
    // - Exalted: 21,000+
    // EQ Faction Bands:
    // - Scowling (KOS - Kill on Sight): -2000 to -751
    // - Threateningly: -750 to -501
    // - Dubious: -500 to -101
    // - Apprehensive: -100 to -1
    // - Indifferent: 0 to 99
    // - Amiable: 100 to 499
    // - Kindly: 500 to 749
    // - Warmly: 750 to 1099
    // - Ally: 1100+

    internal class CreatureFaction
    {
        private static Dictionary<int, CreatureFaction> CreatureFactionsByWOWFactionID = new Dictionary<int, CreatureFaction>();
        private static Dictionary<int, int> CreatureWOWFactionTemplateIDByEQFactionID = new Dictionary<int, int>();
        private static Dictionary<int, int> CreatureWOWFactionIDByEQFactionID = new Dictionary<int, int>();
        private static Dictionary<int, List<CreatureFactionKillReward>> CreatureFactionKillRewardsByEQNPCFactionID = new Dictionary<int, List<CreatureFactionKillReward>>();
        private static int GoodClassesMask = 0;
        private static int GoodRacesMask = 0;
        private static int EvilClassesMask = 0;
        private static int EvilRacesMask = 0;
        private static int NeutralClassesMask = 0;
        private static int NeutralRacesMask = 0;
        private int BaseRepOverride = 0;
        private int BaseRepGoodClasses = 0;
        private int BaseRepEvilClasses = 0;
        private int BaseRepAlignedRaces = 0;
        private int BaseRepUnalignedRaces = 0;
        private int AlignedRacesMask = 0;
        private int UnalignedRacesMask = 0;
        public int FactionID = 0;
        public int FactionTemplateID = -1;
        public int ParentFactionID = 0;
        public int ReputationIndex = 0;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public bool ForceAgro = false;
        public int EnemyFaction1 = 0;
        public int EnemyFaction2 = 0;
        public int EnemyFaction3 = 0;
        public int EnemyFaction4 = 0;

        public static int GetRootFactionParentWOWFactionID()
        {
            if (CreatureFactionsByWOWFactionID.Count == 0)
                PopulateFactionData();

            foreach (CreatureFaction creatureFaction in CreatureFactionsByWOWFactionID.Values)
            {
                if (creatureFaction.Name == Configuration.CREATURE_FACTION_ROOT_NAME)
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
                Logger.WriteDetail("Creature Faction - No wow faction template ID mapped to eq faction ID '" + eqFactionID.ToString() + "' so using default");
                return Configuration.CREATURE_FACTION_TEMPLATE_DEFAULT;
            }
        }

        public static bool CanFactionAssistPlayer(int eqFactionID)
        {
            if (CreatureWOWFactionTemplateIDByEQFactionID.Count == 0)
                PopulateFactionData();
            if (CreatureWOWFactionIDByEQFactionID.ContainsKey(eqFactionID) == true)
            {
                int wowFactionID = CreatureWOWFactionIDByEQFactionID[eqFactionID];
                if (CreatureFactionsByWOWFactionID.ContainsKey(wowFactionID) == true)
                {
                    if (CreatureFactionsByWOWFactionID[wowFactionID].ReputationIndex > -1)
                        return true;
                }                
            }
            return false;
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
            // Load in the class alignments
            string factionClassAlignmentFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureFactionClassAlignment.csv");
            Logger.WriteDetail("Populating creature faction class alignments via file '" + factionClassAlignmentFile + "'");
            List<Dictionary<string, string>> classAlignmentRows = FileTool.ReadAllRowsFromFileWithHeader(factionClassAlignmentFile, "|");
            HashSet<ClassType> evilClasses = new HashSet<ClassType>();
            HashSet<ClassType> goodClasses = new HashSet<ClassType>();
            foreach (Dictionary<string, string> columns in classAlignmentRows)
            {
                ClassType classType = (ClassType)int.Parse(columns["ClassID"]);
                string alignmentString = columns["Alignment"].Trim().ToLower();
                switch (alignmentString)
                {
                    case "evil": evilClasses.Add(classType); break;
                    case "good": goodClasses.Add(classType); break;
                    case "neutral": break; // do nothing for neutral
                    default:
                        {
                            Logger.WriteError("Class alignment error, as the alignment string '" + alignmentString + "' has no mapping");
                        }
                        break;
                }
            }

            // Generate the class bitmasks
            foreach (ClassType classType in Enum.GetValues(typeof(ClassType)))
            {
                if (classType == ClassType.All || classType == ClassType.None)
                    continue;
                if (evilClasses.Contains(classType) == true)
                    EvilClassesMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(classType) - 1));
                else if (goodClasses.Contains(classType) == true)
                    GoodClassesMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(classType) - 1));
                else
                    NeutralClassesMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(classType) - 1));
            }

            // Load in the race alignments
            string factionRaceAlignmentFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureFactionRaceAlignment.csv");
            Logger.WriteDetail("Populating creature faction race alignments via file '" + factionRaceAlignmentFile + "'");
            List<Dictionary<string, string>> raceAlignmentRows = FileTool.ReadAllRowsFromFileWithHeader(factionRaceAlignmentFile, "|");
            HashSet<RaceType> evilRaces = new HashSet<RaceType>();
            HashSet<RaceType> goodRaces = new HashSet<RaceType>();
            foreach (Dictionary<string, string> columns in raceAlignmentRows)
            {
                RaceType raceType = (RaceType)int.Parse(columns["RaceID"]);
                string alignmentString = columns["Alignment"].Trim().ToLower();
                switch (alignmentString)
                {
                    case "evil": evilRaces.Add(raceType); break;
                    case "good": goodRaces.Add(raceType); break;
                    case "neutral": break; // do nothing for neutral
                    default:
                        {
                            Logger.WriteError("Race alignment error, as the alignment string '" + alignmentString + "' has no mapping");
                        }
                        break;
                }
            }

            // Generate the race bitmasks
            foreach (RaceType raceType in Enum.GetValues(typeof(RaceType)))
            {
                if (evilRaces.Contains(raceType) == true)
                    EvilRacesMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(raceType) - 1));
                else if (goodRaces.Contains(raceType) == true)
                    GoodRacesMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(raceType) - 1));
                else
                    NeutralRacesMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(raceType) - 1));
            }

            // Load in faction list
            string factionListFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureFactions.csv");
            Logger.WriteDetail("Populating creature factions via file '" + factionListFile + "'");
            List<Dictionary<string, string>> listRows = FileTool.ReadAllRowsFromFileWithHeader(factionListFile, "|");
            foreach (Dictionary<string, string> columns in listRows)
            {
                // Load the row
                CreatureFaction newCreatureFaction = new CreatureFaction();
                newCreatureFaction.FactionID = int.Parse(columns["FactionID"]);
                newCreatureFaction.FactionTemplateID = int.Parse(columns["FactionTemplateID"]);
                newCreatureFaction.ReputationIndex = int.Parse(columns["ReputationIndex"]);
                newCreatureFaction.Name = columns["Name"];
                newCreatureFaction.BaseRepOverride = int.Parse(columns["BaseRepOverride"]);
                newCreatureFaction.BaseRepGoodClasses = int.Parse(columns["BaseRepGoodClass"]);
                newCreatureFaction.BaseRepEvilClasses = int.Parse(columns["BaseRepEvilClass"]);
                newCreatureFaction.BaseRepAlignedRaces = int.Parse(columns["BaseRepAlignedRace"]);
                newCreatureFaction.BaseRepUnalignedRaces = int.Parse(columns["BaseRepUnalignedRace"]);
                newCreatureFaction.Description = columns["Description"];
                newCreatureFaction.ForceAgro = int.Parse(columns["ForceAgro"]) == 1 ? true : false;
                if (int.Parse(columns["AlignedRaceGood"]) == 1)
                    newCreatureFaction.AlignedRacesMask += GoodRacesMask;
                if (int.Parse(columns["AlignedRaceNeutral"]) == 1)
                    newCreatureFaction.AlignedRacesMask += NeutralRacesMask;
                if (int.Parse(columns["AlignedRaceEvil"]) == 1)
                    newCreatureFaction.AlignedRacesMask += EvilRacesMask;
                newCreatureFaction.UnalignedRacesMask = 1791 - newCreatureFaction.AlignedRacesMask;
                newCreatureFaction.EnemyFaction1 = int.Parse(columns["EnemyFaction1"]);
                newCreatureFaction.EnemyFaction2 = int.Parse(columns["EnemyFaction2"]);
                newCreatureFaction.EnemyFaction3 = int.Parse(columns["EnemyFaction3"]);
                newCreatureFaction.EnemyFaction4 = int.Parse(columns["EnemyFaction4"]);
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
            string factionMapListFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureFactionMap.csv");
            Logger.WriteDetail("Populating creature factions map via file '" + factionMapListFile + "'");
            List<Dictionary<string, string>> mapRows = FileTool.ReadAllRowsFromFileWithHeader(factionMapListFile, "|");
            foreach (Dictionary<string, string> columns in mapRows)
            {
                // Load the row
                int eqfactionID = int.Parse(columns["EQFactionID"]);
                int wowFactionID = int.Parse(columns["WOWFactionID"]);
                if (CreatureFactionsByWOWFactionID.ContainsKey(wowFactionID) == false)
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
            string factionKillRewardFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureFactionKillRewards.csv");
            Logger.WriteDetail("Populating creature faction kill rewards via file '" + factionKillRewardFile + "'");
            List<Dictionary<string, string>> factionKillRewardRows = FileTool.ReadAllRowsFromFileWithHeader(factionKillRewardFile, "|");
            foreach (Dictionary<string, string> columns in factionKillRewardRows)
            {
                // Load the row
                CreatureFactionKillReward creatureFactionKillReward = new CreatureFactionKillReward();
                creatureFactionKillReward.EQNPCFactionID = int.Parse(columns["npc_faction_id"]);
                creatureFactionKillReward.EQFactionID = int.Parse(columns["faction_id"]);
                creatureFactionKillReward.SortOrder = int.Parse(columns["sort_order"]);
                creatureFactionKillReward.KillRewardValue = int.Parse(columns["value"]) * Configuration.CREATURE_KILL_REWARD_REP_MULTIPLIER;

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

        // Accessors for the rep values
        public int GetClassMask1()
        {
            if (BaseRepOverride == -1)
                return GoodClassesMask;
            else
                return 1535; // All
        }

        public int GetRaceMask1()
        {
            return 1791; // All
        }

        public int GetBaseRep1()
        {
            if (BaseRepOverride == -1)
                return BaseRepGoodClasses;
            else
                return BaseRepOverride;
        }

        public int GetClassMask2()
        {
            if (BaseRepOverride == -1)
                return EvilClassesMask;
            else
                return 0;
        }

        public int GetRaceMask2()
        {
            if (BaseRepOverride == -1)
                return 1791; // All
            else
                return 0;
        }

        public int GetBaseRep2()
        {
            if (BaseRepOverride == -1)
                return BaseRepEvilClasses;
            else
                return 0;
        }

        public int GetClassMask3()
        {
            if (BaseRepOverride == -1)
                return NeutralClassesMask;
            else
                return 0;
        }

        public int GetRaceMask3()
        {
            if (BaseRepOverride == -1)
                return AlignedRacesMask;
            else
                return 0;
        }

        public int GetBaseRep3()
        {
            if (BaseRepOverride == -1)
                return BaseRepAlignedRaces;
            else
                return 0;
        }

        public int GetClassMask4()
        {
            if (BaseRepOverride == -1)
                return NeutralClassesMask;
            else
                return 0;
        }

        public int GetRaceMask4()
        {
            if (BaseRepOverride == -1)
                return UnalignedRacesMask;
            else
                return 0;
        }

        public int GetBaseRep4()
        {
            if (BaseRepOverride == -1)
                return BaseRepUnalignedRaces;
            else
                return 0;
        }
    }
}
