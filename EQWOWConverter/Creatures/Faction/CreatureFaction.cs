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
        private static Dictionary<int, CreatureFaction> CreatureFactionsByFactionID = new Dictionary<int, CreatureFaction>();
        private static Dictionary<int, int> CreatureWOWFactionIDByEQFactionID = new Dictionary<int, int>();

        int FactionID = 0;
        int ReputationIndex = 0;
        string Name = string.Empty;
        int BaseRep = 0;
        string Description = string.Empty;

        public static int GetRootFactionParentWOWFactionID()
        {
            if (CreatureFactionsByFactionID.Count == 0)
                PopulateFactionData();

            foreach (CreatureFaction creatureFaction in CreatureFactionsByFactionID.Values)
            {
                if (creatureFaction.Name == "Everquest")
                    return creatureFaction.FactionID;
            }

            Logger.WriteError("CreatureFaction - could not find faction with name 'Everquest'");
            return 0;
        }

        public static int GetWOWFactionIDForEQFactionID(int eqFactionID)
        {
            if (CreatureWOWFactionIDByEQFactionID.Count == 0)
                PopulateFactionData();
            if (CreatureWOWFactionIDByEQFactionID.ContainsKey(eqFactionID) == true)
                return CreatureWOWFactionIDByEQFactionID[eqFactionID];
            else
            {
                Logger.WriteDetail("Creature Faction - No wow faction ID mapped to eq faction ID '" + eqFactionID.ToString() + "'");
                return -1;
            }    
        }

        public static Dictionary<int, CreatureFaction> GetCreatureFactionsByFactionID()
        {
            if (CreatureFactionsByFactionID.Count == 0)
                PopulateFactionData();
            return CreatureFactionsByFactionID;
        }

        private static void PopulateFactionData()
        {
            CreatureFactionsByFactionID.Clear();
            CreatureWOWFactionIDByEQFactionID.Clear();

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
                CreatureFactionsByFactionID.Add(newCreatureFaction.FactionID, newCreatureFaction);
            }

            // Load the faction mappings
            string factionMapListFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureFactionMap.csv");
            Logger.WriteDetail("Populating creature factions map via file '" + factionMapListFile + "'");
            List<Dictionary<string, string>> mapRows = FileTool.ReadAllRowsFromFileWithHeader(factionMapListFile, "|");
            foreach (Dictionary<string, string> columns in mapRows)
            {
                // Load the row
                int eqfactionID = int.Parse(columns["EQFactionID"]);
                int wowFactionID = int.Parse(columns["WOWFactionID"]);
                if (CreatureWOWFactionIDByEQFactionID.ContainsKey(eqfactionID) == true)
                {
                    Logger.WriteError("Creature Faction - Attempted to map eqFactionID of '" + eqfactionID + "' to wowFactionID of '" + wowFactionID + "' but a mapping already existed for the eqFactionID");
                    continue;
                }
                CreatureWOWFactionIDByEQFactionID.Add(eqfactionID, wowFactionID);
            }
        }
    }
}
