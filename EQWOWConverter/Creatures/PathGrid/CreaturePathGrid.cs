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

namespace EQWOWConverter.Creatures
{
    internal class CreaturePathGrid
    {
        private static List<CreaturePathGrid> PathGrids = new List<CreaturePathGrid>();

        public int GridID = 0;
        public string ZoneShortName = string.Empty;
        public CreaturePathGridWanderType WanderType = CreaturePathGridWanderType.GridCircular;
        public CreaturePathGridPauseType PauseType = CreaturePathGridPauseType.RandomHalf;

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
            Logger.WriteDebug("Populating Path Grids list via file '" + pathGridsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(pathGridsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Load the row
                CreaturePathGrid newPathGrid = new CreaturePathGrid();
                newPathGrid.GridID = int.Parse(columns["id"]);
                newPathGrid.ZoneShortName = columns["zone_short_name"];
                newPathGrid.WanderType = (CreaturePathGridWanderType)int.Parse(columns["wander_type"]);
                newPathGrid.PauseType = (CreaturePathGridPauseType)int.Parse(columns["pause_type"]);
                PathGrids.Add(newPathGrid);
            }
        }
    }
}
