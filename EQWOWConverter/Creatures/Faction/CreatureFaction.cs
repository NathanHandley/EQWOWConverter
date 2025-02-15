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
    internal class CreatureFaction
    {
        private static Dictionary<int, CreatureFaction> CreatureFactionsByWOWFactionID = new Dictionary<int, CreatureFaction>();
        private static Dictionary<int, int> CreatureWOWFactionTemplateIDByEQFactionID = new Dictionary<int, int>();
        private static Dictionary<int, int> CreatureWOWFactionIDByEQFactionID = new Dictionary<int, int>();
        private static Dictionary<int, List<CreatureFactionKillReward>> CreatureFactionKillRewardsByEQNPCFactionID = new Dictionary<int, List<CreatureFactionKillReward>>();
        private static int Good1ClassMask = 0;
        private static int Good1RaceMask = 0;        
        private static int Good2ClassMask = 0;
        private static int Good2RaceMask = 0;        
        private static int Evil1ClassMask = 0;
        private static int Evil1RaceMask = 0;        
        private static int Evil2ClassMask = 0;
        private static int Evil2RaceMask = 0;
        private int BaseRepOverride = 0;
        private int GoodBaseRep = 0;
        private int EvilBaseRep = 0;
        public int FactionID = 0;
        public int FactionTemplateID = -1;
        public int ParentFactionID = 0;
        public int ReputationIndex = 0;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public bool ForceAgro = false;
        public bool FleeAtLowLife = false;

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
                Logger.WriteDetail("Creature Faction - No wow faction template ID mapped to eq faction ID '" + eqFactionID.ToString() + "' so using default");
                return Configuration.CONFIG_CREATURE_FACTION_TEMPLATE_DEFAULT;
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
                newCreatureFaction.FactionTemplateID = int.Parse(columns["FactionTemplateID"]);
                newCreatureFaction.ReputationIndex = int.Parse(columns["ReputationIndex"]);
                newCreatureFaction.Name = columns["Name"];
                newCreatureFaction.BaseRepOverride = int.Parse(columns["BaseRepOverride"]);
                newCreatureFaction.GoodBaseRep = int.Parse(columns["BaseRepGood"]);
                newCreatureFaction.EvilBaseRep = int.Parse(columns["BaseRepEvil"]);
                newCreatureFaction.Description = columns["Description"];
                newCreatureFaction.FleeAtLowLife = int.Parse(columns["FleeLowLife"]) == 1 ? true : false;
                newCreatureFaction.ForceAgro = int.Parse(columns["ForceAgro"]) == 1 ? true : false;
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
                creatureFactionKillReward.KillRewardValue = int.Parse(columns["value"]) * Configuration.CONFIG_CREATURE_KILL_REWARD_REP_MULTIPLIER;

                // Add it
                if (CreatureWOWFactionIDByEQFactionID.ContainsKey(creatureFactionKillReward.EQFactionID))
                {
                    creatureFactionKillReward.WOWFactionID = CreatureWOWFactionIDByEQFactionID[creatureFactionKillReward.EQFactionID];
                    if (CreatureFactionKillRewardsByEQNPCFactionID.ContainsKey(creatureFactionKillReward.EQNPCFactionID) == false)
                        CreatureFactionKillRewardsByEQNPCFactionID.Add(creatureFactionKillReward.EQNPCFactionID, new List<CreatureFactionKillReward>());
                    CreatureFactionKillRewardsByEQNPCFactionID[creatureFactionKillReward.EQNPCFactionID].Add(creatureFactionKillReward);
                }
            }

            // Load in the class alignments
            string factionClassAlignmentFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureFactionClassAlignment.csv");
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
                        } break;                
                }
            }

            // Load in the race alignments
            string factionRaceAlignmentFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureFactionRaceAlignment.csv");
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

            // Calculate the masks
            GenerateGoodEvilClassRaceMasks(goodClasses, evilClasses, goodRaces, evilRaces);
        }

        private static void GenerateGoodEvilClassRaceMasks(HashSet<ClassType> goodClasses, HashSet<ClassType> evilClasses, HashSet<RaceType> goodRaces, HashSet<RaceType> evilRaces)
        {
            Logger.WriteDetail("Generating good and evil class race masks for factions");

            // Good 1: Class Perspective (Good Classes + Good or Neutral races)
            Good1ClassMask = 0;
            Good1RaceMask = 0;
            foreach (ClassType goodClass in goodClasses)
                Good1ClassMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(goodClass) - 1));
            foreach (RaceType race in Enum.GetValues(typeof(RaceType)))
                if (evilRaces.Contains(race) == false)
                    Good1RaceMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(race) - 1));

            // Good 2: Race Perspective (Good Races + Good or Neutral classes)
            Good2ClassMask = 0;
            Good2RaceMask = 0;
            foreach (RaceType goodRace in goodRaces)
                Good2RaceMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(goodRace) - 1));
            foreach (ClassType classType in Enum.GetValues(typeof(ClassType)))
                if (evilClasses.Contains(classType) == false)
                    Good2ClassMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(classType) - 1));

            // Evil 1: Class Perspective (Evil Classes + Evil or Neutral races)
            Evil1ClassMask = 0;
            Evil1RaceMask = 0;
            foreach (ClassType evilClass in evilClasses)
                Evil1ClassMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(evilClass) - 1));
            foreach (RaceType race in Enum.GetValues(typeof(RaceType)))
                if (goodRaces.Contains(race) == false)
                    Evil1RaceMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(race) - 1));

            // Evil 2: Race Perspective (Evil Races + Evil or Neutral classes)
            Evil2ClassMask = 0;
            Evil2RaceMask = 0;
            foreach (RaceType evilRace in evilRaces)
                Evil2RaceMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(evilRace) - 1));
            foreach (ClassType classType in Enum.GetValues(typeof(ClassType)))
                if (goodClasses.Contains(classType) == false)
                    Evil2ClassMask += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(classType) - 1));
        }

        // Accessors for the rep values
        public int GetGood1ClassMask()
        {
            if (BaseRepOverride == -1)
                return Good1ClassMask;
            else
                return 1791; // Taken from Netherwing
        }

        public int GetGood1RaceMask()
        {
            if (BaseRepOverride == -1)
                return Good1RaceMask;
            else
                return 1535; // Taken from Netherwing
        }

        public int GetGood1BaseRep()
        {
            if (BaseRepOverride == -1)
                return GoodBaseRep;
            else
                return BaseRepOverride;
        }

        public int GetGood2ClassMask()
        {
            if (BaseRepOverride == -1)
                return Good2ClassMask;
            else
                return 0;
        }

        public int GetGood2RaceMask()
        {
            if (BaseRepOverride == -1)
                return Good2RaceMask;
            else
                return 0;
        }

        public int GetGood2BaseRep()
        {
            if (BaseRepOverride == -1)
                return GoodBaseRep;
            else
                return 0;
        }

        public int GetEvil1ClassMask()
        {
            if (BaseRepOverride == -1)
                return Evil1ClassMask;
            else
                return 0;
        }

        public int GetEvil1RaceMask()
        {
            if (BaseRepOverride == -1)
                return Evil1RaceMask;
            else
                return 0;
        }

        public int GetEvil1BaseRep()
        {
            if (BaseRepOverride == -1)
                return EvilBaseRep;
            else
                return 0;
        }

        public int GetEvil2ClassMask()
        {
            if (BaseRepOverride == -1)
                return Evil2ClassMask;
            else
                return 0;
        }

        public int GetEvil2RaceMask()
        {
            if (BaseRepOverride == -1)
                return Evil2RaceMask;
            else
                return 0;
        }

        public int GetEvil2BaseRep()
        {
            if (BaseRepOverride == -1)
                return EvilBaseRep;
            else
                return 0;
        }
    }
}
