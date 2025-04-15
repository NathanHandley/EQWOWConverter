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

namespace EQWOWConverter.Player
{
    internal class PlayerClassRaceProperties
    {
        private static Dictionary<(int, int), PlayerClassRaceProperties> PlayerClassRacePropertiesByRaceAndClassIDs = new Dictionary<(int, int), PlayerClassRaceProperties>();
        private static readonly object PropertiesReadLock = new object();
        public int RaceID;
        public int ClassID;
        public string StartZoneShortName = string.Empty;
        public float StartPositionX;
        public float StartPositionY;
        public float StartPositionZ;
        public float StartOrientation;

        public static Dictionary<(int, int), PlayerClassRaceProperties> GetClassRacePropertiesByRaceAndClassID()
        {
            lock (PropertiesReadLock)
            {
                if (PlayerClassRacePropertiesByRaceAndClassIDs.Count == 0)
                    PopulateClassRaceProperties();
                return PlayerClassRacePropertiesByRaceAndClassIDs;
            }            
        }

        private static void PopulateClassRaceProperties()
        {
            string propertiesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "PlayerClassRaceProperties.csv");
            Logger.WriteDebug(string.Concat("Populating player class race properties via file '", propertiesFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(propertiesFile, "|");
            foreach(Dictionary<string, string> columns in rows)
            {
                // Load the row
                PlayerClassRaceProperties curProperties = new PlayerClassRaceProperties();
                curProperties.RaceID = int.Parse(columns["RaceID"]);
                curProperties.ClassID = int.Parse(columns["ClassID"]);
                curProperties.StartZoneShortName = columns["StartZoneShortName"].Trim().ToLower();
                curProperties.StartPositionX = float.Parse(columns["StartPosX"]) * Configuration.GENERATE_WORLD_SCALE;
                curProperties.StartPositionY = float.Parse(columns["StartPosY"]) * Configuration.GENERATE_WORLD_SCALE;
                curProperties.StartPositionZ = float.Parse(columns["StartPosZ"]) * Configuration.GENERATE_WORLD_SCALE;
                curProperties.StartOrientation = float.Parse(columns["StartOrientation"]);

                // Add if unique
                if (PlayerClassRacePropertiesByRaceAndClassIDs.ContainsKey((curProperties.RaceID, curProperties.ClassID)) == true)
                {
                    Logger.WriteError(string.Concat("Failed to read in a player class race properties row since there was a duplicate row with race ", curProperties.RaceID, " and class ", curProperties.ClassID));
                    continue;
                }
                PlayerClassRacePropertiesByRaceAndClassIDs.Add((curProperties.RaceID, curProperties.ClassID), curProperties);
            }
        }
    }
}
