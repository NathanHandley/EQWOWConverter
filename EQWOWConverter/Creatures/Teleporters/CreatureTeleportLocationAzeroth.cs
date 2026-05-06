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

using EQWOWConverter.Common;

namespace EQWOWConverter.Creatures.Teleporters
{
    internal class CreatureTeleportLocationAzeroth
    {
        public RaceType Race;
        public int MapID;
        public int AreaID;
        public float XPosition;
        public float YPosition;
        public float ZPosition;
        public float Orientation;
        public string MenuItemText;

        private static List<CreatureTeleportLocationAzeroth> TeleportLocations = new List<CreatureTeleportLocationAzeroth>();

        public static readonly object TeleporterLock = new object();

        public static List<CreatureTeleportLocationAzeroth> GetAllTeleportLocations()
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
            string teleportLocationFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TeleportLocationsToAzeroth.csv");
            Logger.WriteDebug("Populating Azeroth teleport locations list via file '" + teleportLocationFileName + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(teleportLocationFileName, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                int raceID = int.Parse(columns["RaceID"]);
                if (Enum.IsDefined(typeof(RaceType), raceID) == false)
                {
                    Logger.WriteError("Error in LoadTeleportLocations, as there was no RaceType that maps to int value '", raceID.ToString(), "'");
                    continue;
                }
                CreatureTeleportLocationAzeroth teleportLocation = new CreatureTeleportLocationAzeroth();
                teleportLocation.Race = (RaceType)raceID;
                teleportLocation.MapID = int.Parse(columns["MapID"]);
                teleportLocation.AreaID = int.Parse(columns["AreaID"]);
                teleportLocation.XPosition = float.Parse(columns["X"]);
                teleportLocation.YPosition = float.Parse(columns["Y"]);
                teleportLocation.ZPosition = float.Parse(columns["Z"]);
                teleportLocation.Orientation = float.Parse(columns["O"]);
                teleportLocation.MenuItemText = columns["MenuItemText"];
                TeleportLocations.Add(teleportLocation);
            }
        }
    }
}
