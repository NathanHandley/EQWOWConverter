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
        private static Dictionary<string, Dictionary<int, CreaturePathGrid>> PathGridByZoneNameAndGridID = new Dictionary<string, Dictionary<int, CreaturePathGrid>>();

        public int GridID = 0;
        public string ZoneShortName = string.Empty;
        public CreaturePathGridWanderType WanderType = CreaturePathGridWanderType.None;
        public CreaturePathGridPauseType PauseType = CreaturePathGridPauseType.RandomHalf;
        public bool DisableGroundContour = false;

        public static Dictionary<string, Dictionary<int, CreaturePathGrid>> GetAllPathGridsByZoneNameAndGridID()
        {
            if (PathGridByZoneNameAndGridID.Count == 0)
                PopulatePathGrids();
            return PathGridByZoneNameAndGridID;
        }

        private static void PopulatePathGrids()
        {
            PathGridByZoneNameAndGridID.Clear();

            string pathGridsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "PathGrids.csv");
            Logger.WriteDebug("Populating Path Grids list via file '" + pathGridsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(pathGridsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Load the row
                CreaturePathGrid newPathGrid = new CreaturePathGrid();
                newPathGrid.GridID = int.Parse(columns["id"]);
                newPathGrid.ZoneShortName = columns["short_name"];
                newPathGrid.WanderType = (CreaturePathGridWanderType)int.Parse(columns["wander_type"]);
                newPathGrid.PauseType = (CreaturePathGridPauseType)int.Parse(columns["pause_type"]);
                newPathGrid.DisableGroundContour = columns["disable_ground_contour"].Trim() == "1" ? true : false;

                if (PathGridByZoneNameAndGridID.ContainsKey(newPathGrid.ZoneShortName) == false)
                    PathGridByZoneNameAndGridID.Add(newPathGrid.ZoneShortName, new Dictionary<int, CreaturePathGrid>());
                if (PathGridByZoneNameAndGridID[newPathGrid.ZoneShortName].ContainsKey(newPathGrid.GridID) == false)
                    PathGridByZoneNameAndGridID[newPathGrid.ZoneShortName].Add(newPathGrid.GridID, newPathGrid);
            }
        }
    }
}
