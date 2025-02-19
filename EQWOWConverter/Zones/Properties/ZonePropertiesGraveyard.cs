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

namespace EQWOWConverter.Zones
{
    internal class ZonePropertiesGraveyard
    {
        private static Dictionary<int, ZonePropertiesGraveyard> GraveyardsByID = new Dictionary<int, ZonePropertiesGraveyard>();
        private static Dictionary<string, ZonePropertiesGraveyard> GraveyardsByShortName = new Dictionary<string, ZonePropertiesGraveyard>();

        public int ID = 0;
        public string ShortName = string.Empty;
        public float RespawnX = 0;
        public float RespawnY = 0;
        public float RespawnZ = 0;
        public float RespawnOrientation = 0;
        public float SpiritHealerX = 0;
        public float SpiritHealerY = 0;
        public float SpiritHealerZ = 0;
        public float SpiritHealerOrientation = 0;

        public static ZonePropertiesGraveyard GetGraveyardByID(int ID)
        {
            if (GraveyardsByID.Count == 0)
                PopulateGraveyardData();
            if (GraveyardsByID.ContainsKey(ID) == false)
            {
                Logger.WriteError("Unable to find graveyard with ID '" + ID + "', so returning default");
                return GraveyardsByID[Configuration.CONFIG_ZONE_DEFAULT_GRAVEYARD_ID];
            }
            else
                return GraveyardsByID[ID];
        }

        public static ZonePropertiesGraveyard GetGraveyardByShortName(string shortName)
        {
            if (GraveyardsByShortName.Count == 0)
                PopulateGraveyardData();
            if (GraveyardsByShortName.ContainsKey(shortName) == false)
            {
                Logger.WriteError("Unable to find graveyard with shortname reference of '" + shortName + "', so returning default");
                return GraveyardsByID[Configuration.CONFIG_ZONE_DEFAULT_GRAVEYARD_ID];
            }
            else
                return GraveyardsByShortName[shortName];
        }

        public static List<ZonePropertiesGraveyard> GetAllGraveyards()
        {
            if (GraveyardsByShortName.Count == 0)
                PopulateGraveyardData();
            return GraveyardsByID.Values.ToList();
        }

        private static void PopulateGraveyardData()
        {
            // Load the graveyards
            string graveyardFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "ZoneGraveyards.csv");
            Logger.WriteDetail("Populating zone graveyards via file '" + graveyardFile + "'");
            List<Dictionary<string, string>> zoneGraveyardRows = FileTool.ReadAllRowsFromFileWithHeader(graveyardFile, "|");
            foreach (Dictionary<string, string> columns in zoneGraveyardRows)
            {
                ZonePropertiesGraveyard graveyard = new ZonePropertiesGraveyard();
                graveyard.ID = int.Parse(columns["ID"]);
                graveyard.ShortName = columns["ZoneShortName"];
                graveyard.RespawnX = int.Parse(columns["RespawnX"]);
                graveyard.RespawnY = int.Parse(columns["RespawnY"]);
                graveyard.RespawnZ = int.Parse(columns["RespawnZ"]);
                graveyard.RespawnOrientation = int.Parse(columns["RespawnOrientation"]);
                graveyard.SpiritHealerX = int.Parse(columns["SpiritHealerX"]);
                graveyard.SpiritHealerY = int.Parse(columns["SpiritHealerY"]);
                graveyard.SpiritHealerZ = int.Parse(columns["SpiritHealerZ"]);
                graveyard.SpiritHealerOrientation = int.Parse(columns["SpiritHealerOrientation"]);
                GraveyardsByID.Add(graveyard.ID, graveyard);
            }

            // Load the zone mapping
            string graveyardMapFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "ZoneGraveyardMap.csv");
            Logger.WriteDetail("Populating zone graveyard map via file '" + graveyardMapFile + "'");
            List<Dictionary<string, string>> zoneGraveyardMapRows = FileTool.ReadAllRowsFromFileWithHeader(graveyardMapFile, "|");
            foreach (Dictionary<string, string> columns in zoneGraveyardMapRows)
            {
                string zoneShortName = columns["ZoneShortName"];
                int graveyardID = int.Parse(columns["GraveyardID"]);
                GraveyardsByShortName.Add(zoneShortName, GetGraveyardByID(graveyardID));
            }
        }
    }
}
