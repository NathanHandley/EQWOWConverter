//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

using EQWOWConverter.ObjectModels.Properties;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Creatures
{
    internal class CreatureRace
    {
        private static Dictionary<int, CreatureRace> CreatureRacesByRaceID = new Dictionary<int, CreatureRace>();

        public int ID;
        public string Name = string.Empty;
        public string MaleSkeletonName = string.Empty;
        public string FemaleSkeletonName = string.Empty;
        public string NeutralSkeletonName = string.Empty;
        public float BoundaryRadius;
        public float LiftMaleAndNeutral = 0;
        public float LiftFemale = 0;

        public static CreatureRace GetCreatureRaceByID(int id)
        {
            if (CreatureRacesByRaceID.Count == 0)
                PopulateCreatureRaceList();
            if (CreatureRacesByRaceID.ContainsKey(id) == false)
                return new CreatureRace();
            else
                return CreatureRacesByRaceID[id];
        }

        public static List<CreatureRace> GetAllCreatureRaces()
        {
            if (CreatureRacesByRaceID.Count == 0)
                PopulateCreatureRaceList();
            List<CreatureRace> creatureRaces = new List<CreatureRace>();
            foreach(var creatureRace in CreatureRacesByRaceID.Values)
                creatureRaces.Add(creatureRace);
            return creatureRaces;
        }

        private static void PopulateCreatureRaceList()
        {
            CreatureRacesByRaceID.Clear();

            // Load in base race data
            string raceDataFileName = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureRaces.csv");
            Logger.WriteDetail("Populating CreatureRace list via file '" + raceDataFileName + "'");
            string inputData = File.ReadAllText(raceDataFileName);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("CreatureRace list via file '" + raceDataFileName + "' did not have enough lines");
                return;
            }

            // Load all of the data
            bool firstRow = true;
            foreach(string row in inputRows)
            {
                // Handle first row
                if (firstRow == true)
                {
                    firstRow = false;
                    continue;
                }

                // Make sure data size is correct
                string[] rowBlocks = row.Split(",");
                if (rowBlocks.Length != 8)
                {
                    Logger.WriteError("CreatureRace list via file '" + raceDataFileName + "' has an invalid row which doesn't have a length of 8");
                    continue;
                }

                // Load the row
                CreatureRace newCreatureRace = new CreatureRace();
                newCreatureRace.ID = int.Parse(rowBlocks[0]);
                newCreatureRace.Name = rowBlocks[1].Trim();
                newCreatureRace.MaleSkeletonName = rowBlocks[2].Trim();
                newCreatureRace.FemaleSkeletonName = rowBlocks[3].Trim();
                newCreatureRace.NeutralSkeletonName = rowBlocks[4].Trim();
                newCreatureRace.BoundaryRadius = float.Parse(rowBlocks[5]);
                newCreatureRace.LiftMaleAndNeutral = float.Parse(rowBlocks[6]);
                newCreatureRace.LiftFemale = float.Parse(rowBlocks[7]);
                if (CreatureRacesByRaceID.ContainsKey(newCreatureRace.ID))
                {
                    Logger.WriteError("CreatureRace list via file '" + raceDataFileName + "' has an duplicate row with id '" + newCreatureRace.ID + "'");
                    continue;
                }
                CreatureRacesByRaceID.Add(newCreatureRace.ID, newCreatureRace);
            }
        }
    }
}
