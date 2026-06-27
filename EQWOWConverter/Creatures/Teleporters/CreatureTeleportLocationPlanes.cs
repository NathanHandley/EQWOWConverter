//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

namespace EQWOWConverter.Creatures.Teleporters
{
    internal class CreatureTeleportLocationPlanes
    {
        public string ZoneShortName = string.Empty;
        public float XPosition;
        public float YPosition;
        public float ZPosition;
        public float Orientation;
        public string MenuItemText = string.Empty;

        private static List<CreatureTeleportLocationPlanes> TeleportLocations = new List<CreatureTeleportLocationPlanes>();
        public static readonly object TeleporterLock = new object();

        public static List<CreatureTeleportLocationPlanes> GetAllTeleportLocations()
        {
            lock (TeleporterLock)
            {
                if (TeleportLocations.Count == 0)
                    LoadTeleportLocations();
                return TeleportLocations;
            }
        }

        private static void LoadTeleportLocations()
        {
            TeleportLocations.Clear();
            string teleportLocationFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TeleportLocationsToPlanes.csv");
            Logger.WriteDebug("Populating Planes teleport locations list via file '" + teleportLocationFileName + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(teleportLocationFileName, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                CreatureTeleportLocationPlanes teleportLocation = new CreatureTeleportLocationPlanes();
                teleportLocation.ZoneShortName = columns["ZoneShortName"];
                teleportLocation.XPosition = float.Parse(columns["X"]) * Configuration.GENERATE_WORLD_SCALE;
                teleportLocation.YPosition = float.Parse(columns["Y"]) * Configuration.GENERATE_WORLD_SCALE;
                teleportLocation.ZPosition = float.Parse(columns["Z"]) * Configuration.GENERATE_WORLD_SCALE;
                teleportLocation.Orientation = float.Parse(columns["O"]);
                teleportLocation.MenuItemText = columns["MenuItemText"];
                TeleportLocations.Add(teleportLocation);
            }
        }
    }
}
