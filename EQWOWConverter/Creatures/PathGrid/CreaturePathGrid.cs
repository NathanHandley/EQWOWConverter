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

// Note: This is ignored for now until I figure out what Type and Type2 mean...
namespace EQWOWConverter.Creatures
{
    internal class CreaturePathGrid
    {
        private static List<CreaturePathGrid> PathGrids = new List<CreaturePathGrid>();

        public int GridID = 0; 
        public int ZoneID = 0;
        public int Type = 0;
        public int Type2 = 0;

        public static List<CreaturePathGrid> GetPathGrids()
        {
            if (PathGrids.Count == 0)
                PopulatePathGrids();
            return PathGrids;
        }

        private static void PopulatePathGrids()
        {
            PathGrids.Clear();

            string pathGridsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "PathGrids.csv");
            Logger.WriteDetail("Populating Path Grids list via file '" + pathGridsFile + "'");
            string inputData = FileTool.ReadAllDataFromFile(pathGridsFile);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("Path Grids list via file '" + pathGridsFile + "' did not have enough rows");
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
                string[] rowBlocks = row.Split("|");
                CreaturePathGrid newPathGrid = new CreaturePathGrid();
                newPathGrid.GridID = int.Parse(rowBlocks[0]);
                newPathGrid.ZoneID = int.Parse(rowBlocks[1]);
                newPathGrid.Type = int.Parse(rowBlocks[2]);
                newPathGrid.Type2 = int.Parse(rowBlocks[3]);

                PathGrids.Add(newPathGrid);
            }
        }
    }
}
