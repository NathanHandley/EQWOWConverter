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

namespace EQWOWConverter.Zones
{
    internal class ZonePropertiesLightArea
    {
        public static Dictionary<string, List<ZonePropertiesLightArea>> LightAreasByZoneShortName = new Dictionary<string, List<ZonePropertiesLightArea>>();
        private static readonly object LightAreasLock = new object();

        public string ZoneShortName = string.Empty;
        public bool IsSkyVisible = false;
        public byte AmbientRed = 165;
        public byte AmbientGreen = 165;
        public byte AmbientBlue = 165;
        public ZoneFogType FogType = ZoneFogType.Light;
        public byte FogRed = 165;
        public byte FogGreen = 165;
        public byte FogBlue = 165;
        public Vector3 CenterPosition = new Vector3();
        public Vector3 OuterEdgePosition = new Vector3();
        public Vector3 InnerEdgePosition = new Vector3();

        public static List<ZonePropertiesLightArea> GetLightAreasForZone(string zoneShortName)
        {
            lock (LightAreasLock)
            {
                if (LightAreasByZoneShortName.Count == 0)
                    PopulateLightAreaData();
                if (LightAreasByZoneShortName.ContainsKey(zoneShortName) == true)
                    return LightAreasByZoneShortName[zoneShortName];
                else
                    return new List<ZonePropertiesLightArea>();
            }
        }

        private static void PopulateLightAreaData()
        {
            lock (LightAreasLock)
            {
                LightAreasByZoneShortName.Clear();
                string lightAreasFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ZoneLightAreas.csv");
                Logger.WriteDebug("Populating zone light areas via file '" + lightAreasFile + "'");
                List<Dictionary<string, string>> lightAreasRows = FileTool.ReadAllRowsFromFileWithHeader(lightAreasFile, "|");
                foreach (Dictionary<string, string> columns in lightAreasRows)
                {
                    ZonePropertiesLightArea lightArea = new ZonePropertiesLightArea();
                    lightArea.ZoneShortName = columns["ZoneShortName"];
                    lightArea.IsSkyVisible = columns["IsSkyVisible"].Trim() == "1" ? true : false;
                    lightArea.AmbientRed = byte.Parse(columns["AmbientRed"]);
                    lightArea.AmbientGreen = byte.Parse(columns["AmbientGreen"]);
                    lightArea.AmbientBlue = byte.Parse(columns["AmbientBlue"]);
                    switch (columns["FogType"].Trim().ToLower())
                    {
                        case "light": lightArea.FogType = ZoneFogType.Light; break;
                        case "medium": lightArea.FogType = ZoneFogType.Medium; break;
                        case "heavy": lightArea.FogType = ZoneFogType.Heavy; break;
                        default:
                            {
                                Logger.WriteError("For Light Area, invalid fog type of '", columns["FogType"], "' for zone '", lightArea.ZoneShortName, "' ");
                            } break;
                    }
                    lightArea.FogRed = byte.Parse(columns["FogRed"]);
                    lightArea.FogGreen = byte.Parse(columns["FogGreen"]);
                    lightArea.FogBlue = byte.Parse(columns["FogBlue"]);
                    lightArea.CenterPosition.X = float.Parse(columns["CenterX"]);
                    lightArea.CenterPosition.Y = float.Parse(columns["CenterY"]);
                    lightArea.CenterPosition.Z = float.Parse(columns["CenterZ"]);
                    lightArea.OuterEdgePosition.X = float.Parse(columns["OuterEdgeX"]);
                    lightArea.OuterEdgePosition.Y = float.Parse(columns["OuterEdgeY"]);
                    lightArea.OuterEdgePosition.Z = float.Parse(columns["OuterEdgeZ"]);
                    lightArea.InnerEdgePosition.X = float.Parse(columns["InnerEdgeX"]);
                    lightArea.InnerEdgePosition.Y = float.Parse(columns["InnerEdgeY"]);
                    lightArea.InnerEdgePosition.Z = float.Parse(columns["InnerEdgeZ"]);
                    if (LightAreasByZoneShortName.ContainsKey(lightArea.ZoneShortName) == false)
                        LightAreasByZoneShortName.Add(lightArea.ZoneShortName, new List<ZonePropertiesLightArea>());
                    LightAreasByZoneShortName[lightArea.ZoneShortName].Add(lightArea);
                }
            }
        }
    }
}
