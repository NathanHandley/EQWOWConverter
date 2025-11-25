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

namespace EQWOWConverter.Zones
{
    internal class ZoneContinent
    {
        public class MapLink
        {
            public string LinkedZoneShortName = string.Empty;
            public int Left;
            public int Top;
            public int Width;
            public int Height;

            public MapLink(string linkedZoneShortName, int left, int top, int width, int height)
            {
                LinkedZoneShortName = linkedZoneShortName;
                Left = left;
                Top = top;
                Width = width;
                Height = height;
            }
        }

        public int DBCMapID;
        public int DBCWorldMapAreaID;
        public string ShortName = string.Empty;
        public string DescriptiveName = string.Empty;
        public ZoneContinentType ContinentType;
        public int ExpansionID = 0;

        private static readonly object ZONE_CONTINENT_DATA_LOCK = new object();
        private static Dictionary<string, List<MapLink>> MapLinksByContinentShortname = new Dictionary<string, List<MapLink>>();
        private static List<ZoneContinent> ZoneContinents = new List<ZoneContinent>();

        public static List<ZoneContinent> GetZoneContinents()
        {
            lock (ZONE_CONTINENT_DATA_LOCK)
            {
                if (ZoneContinents.Count == 0)
                    LoadZoneContinents();
                return ZoneContinents;
            }
        }

        public static List<MapLink> GetMapLinksForContinent(string continentShortName)
        {
            lock (ZONE_CONTINENT_DATA_LOCK)
            {
                if (MapLinksByContinentShortname.Count == 0)
                    LoadMapLinks();
                if (MapLinksByContinentShortname.ContainsKey(continentShortName) == true)
                    return MapLinksByContinentShortname[continentShortName];
                else
                    return new List<MapLink>();
            }
        }

        private static void LoadMapLinks()
        {
            string mapLinkListFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ZoneDisplayMapContinentLinks.csv");
            Logger.WriteDebug("Populating Zone Display Continent Map Links via file '" + mapLinkListFile + "'");
            List<Dictionary<string, string>> mapLinkFileRows = FileTool.ReadAllRowsFromFileWithHeader(mapLinkListFile, "|");
            foreach (Dictionary<string, string> mapLinkFileColumns in mapLinkFileRows)
            {
                string ownerZoneShortName = mapLinkFileColumns["OwnerZoneShortName"];
                string linkedZoneShortName = mapLinkFileColumns["LinkedZoneShortName"];
                int left = Convert.ToInt32(mapLinkFileColumns["Left"]);
                int top = Convert.ToInt32(mapLinkFileColumns["Top"]);
                int width = Convert.ToInt32(mapLinkFileColumns["Width"]);
                int height = Convert.ToInt32(mapLinkFileColumns["Height"]);

                MapLink newMapLink = new MapLink(linkedZoneShortName, left, top, width, height);
                if (MapLinksByContinentShortname.ContainsKey(ownerZoneShortName) == false)
                    MapLinksByContinentShortname.Add(ownerZoneShortName, new List<MapLink>());
                MapLinksByContinentShortname[ownerZoneShortName].Add(newMapLink);
            }
        }

        private static void LoadZoneContinents()
        {
            string zoneContinentsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ZoneContinents.csv");
            Logger.WriteDebug("Populating Zone Continents via file '" + zoneContinentsFile + "'");
            List<Dictionary<string, string>> zoneContinentRows = FileTool.ReadAllRowsFromFileWithHeader(zoneContinentsFile, "|");
            foreach (Dictionary<string, string> zoneContinentColumns in zoneContinentRows)
            {
                ZoneContinent newContinent = new ZoneContinent();
                newContinent.ShortName = zoneContinentColumns["ShortName"];
                newContinent.DBCMapID = int.Parse(zoneContinentColumns["WOWMapID"]);
                newContinent.DBCWorldMapAreaID = int.Parse(zoneContinentColumns["WorldMapAreaID"]);
                newContinent.DescriptiveName = zoneContinentColumns["DescriptiveName"];
                newContinent.ContinentType = (ZoneContinentType)int.Parse(zoneContinentColumns["ContinentID"]);
                newContinent.ExpansionID = int.Parse(zoneContinentColumns["ExpansionID"]);
                ZoneContinents.Add(newContinent);
            }
        }
    }
}
