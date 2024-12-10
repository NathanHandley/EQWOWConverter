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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Creatures
{
    internal class CreaturePathGridEntry
    {
        private static List<CreaturePathGridEntry> PathGridEntries = new List<CreaturePathGridEntry>();

        public int GridID = 0;
        public string ZoneShortName = string.Empty;
        public int Number = 0;
        public float NodeX = 0;
        public float NodeY = 0;
        public float NodeZ = 0;
        public int PauseInSec = 0;

        public static List<CreaturePathGridEntry> GetPathGridEntries()
        {
            if (PathGridEntries.Count == 0)
                PopulatePathGridEntries();
            return PathGridEntries;
        }

        private static void PopulatePathGridEntries()
        {
            PathGridEntries.Clear();

            string pathGridEntriesFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "PathGridEntries.csv");
            Logger.WriteDetail("Populating Path Grid Entires list via file '" + pathGridEntriesFile + "'");
            string inputData = File.ReadAllText(pathGridEntriesFile);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("Path Grid Entries list via file '" + pathGridEntriesFile + "' did not have enough rows");
                return;
            }

            // Load all of the data
            bool headerRow = true;
            foreach (string row in inputRows)
            {
                // Handle first row
                if (headerRow == true)
                {
                    headerRow = false;
                    continue;
                }

                // Skip blank rows
                if (row.Trim().Length == 0)
                    continue;

                // Load the row
                string[] rowBlocks = row.Split(",");
                CreaturePathGridEntry newPathGridEntry = new CreaturePathGridEntry();
                newPathGridEntry.GridID = int.Parse(rowBlocks[0]);
                newPathGridEntry.ZoneShortName = rowBlocks[1];
                newPathGridEntry.Number = int.Parse(rowBlocks[2]);
                float nodeX = float.Parse(rowBlocks[3]);
                float nodeY = float.Parse(rowBlocks[4]);
                float nodeZ = float.Parse(rowBlocks[5]);
                newPathGridEntry.PauseInSec = int.Parse(rowBlocks[6]);

                // Modify by world scale
                // Note: X and Y were reversed when converting the data
                newPathGridEntry.NodeX = nodeX * Configuration.CONFIG_GENERATE_WORLD_SCALE;
                newPathGridEntry.NodeY = nodeY * Configuration.CONFIG_GENERATE_WORLD_SCALE;
                newPathGridEntry.NodeZ = nodeZ * Configuration.CONFIG_GENERATE_WORLD_SCALE;

                PathGridEntries.Add(newPathGridEntry);
            }
        }
    }
}
