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
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Zones
{
    internal class ZonePropertiesZoneLineBox
    {
        private static Dictionary<string, List<ZonePropertiesZoneLineBox>> ZoneLineBoxesBySourceZoneShortName = new Dictionary<string, List<ZonePropertiesZoneLineBox>>();
        private static readonly object ZoneLineBoxesLock = new object();

        public int AreaTriggerID;
        public string TargetZoneShortName = string.Empty;
        public Vector3 TargetZonePosition = new Vector3();
        public float TargetZoneOrientation = 0f;
        public Vector3 BoxPosition = new Vector3();
        public float BoxLength;
        public float BoxWidth;
        public float BoxHeight;
        public float BoxOrientation = 0.0f;

        private static void PopulateZoneLineBoxesFromFile()
        {
            ZoneLineBoxesBySourceZoneShortName.Clear();
            string zoneLineBoxesFilePath = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ZoneLineBoxes.csv");
            Logger.WriteDebug("Populating Zone Line Boxes via file '" + zoneLineBoxesFilePath + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(zoneLineBoxesFilePath, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                string sourceZoneShortName = columns["SourceZoneShortName"];
                string targetZoneShortName = columns["TargetZoneShortName"];
                float targetZonePositionX = Convert.ToSingle(columns["TargetPosX"]);
                float targetZonePositionY = Convert.ToSingle(columns["TargetPosY"]);
                float targetZonePositionZ = Convert.ToSingle(columns["TargetPosZ"]);
                float boxTopNorthwestX = Convert.ToSingle(columns["SourceBoxTopNW_X"]);
                float boxTopNorthwestY = Convert.ToSingle(columns["SourceBoxTopNW_Y"]);
                float boxTopNorthwestZ = Convert.ToSingle(columns["SourceBoxTopNW_Z"]);
                float boxBottomSoutheastX = Convert.ToSingle(columns["SourceBoxBottomSE_X"]);
                float boxBottomSoutheastY = Convert.ToSingle(columns["SourceBoxBottomSE_Y"]);
                float boxBottomSoutheastZ = Convert.ToSingle(columns["SourceBoxBottomSE_Z"]);
                ZoneLineOrientationType targetZoneOrientation = ZoneLineOrientationType.North;
                switch (columns["TargetPosOrientation"].ToLower().Trim())
                {
                    case "north": targetZoneOrientation = ZoneLineOrientationType.North; break;
                    case "south": targetZoneOrientation = ZoneLineOrientationType.South; break;
                    case "east": targetZoneOrientation = ZoneLineOrientationType.East; break;
                    case "west": targetZoneOrientation = ZoneLineOrientationType.West; break;
                    case "northwest": targetZoneOrientation = ZoneLineOrientationType.NorthWest; break;
                    default:
                        {
                            Logger.WriteError("PopulateZoneLineBoxesFromFile found unhandled orientation of '", columns["TargetPosOrientation"], "'");
                        } break;
                }
                ZonePropertiesZoneLineBox zoneLineBox = new ZonePropertiesZoneLineBox(targetZoneShortName, targetZonePositionX,
                    targetZonePositionY, targetZonePositionZ, targetZoneOrientation, boxTopNorthwestX, boxTopNorthwestY, boxTopNorthwestZ,
                    boxBottomSoutheastX, boxBottomSoutheastY, boxBottomSoutheastZ);
                if (ZoneLineBoxesBySourceZoneShortName.ContainsKey(sourceZoneShortName) == false)
                    ZoneLineBoxesBySourceZoneShortName.Add(sourceZoneShortName, new List<ZonePropertiesZoneLineBox>());
                ZoneLineBoxesBySourceZoneShortName[sourceZoneShortName].Add(zoneLineBox);
            }
        }

        public static List<ZonePropertiesZoneLineBox> GetZoneLineBoxesForSourceZone(string zoneShortName)
        {
            lock (ZoneLineBoxesLock)
            {
                if (ZoneLineBoxesBySourceZoneShortName.Count == 0)
                    PopulateZoneLineBoxesFromFile();
                if (ZoneLineBoxesBySourceZoneShortName.ContainsKey(zoneShortName) == false)
                    return new List<ZonePropertiesZoneLineBox>();
                else
                    return ZoneLineBoxesBySourceZoneShortName[zoneShortName];
            }
        }

        public ZonePropertiesZoneLineBox(string targetZoneShortName, float targetZonePositionX, float targetZonePositionY,
            float targetZonePositionZ, ZoneLineOrientationType targetZoneOrientation, float boxTopNorthwestX, float boxTopNorthwestY,
            float boxTopNorthwestZ, float boxBottomSoutheastX, float boxBottomSoutheastY, float boxBottomSoutheastZ)
        {
            AreaTriggerID = AreaTriggerDBC.GetGeneratedAreaTriggerID();

            // Scale input values
            targetZonePositionX *= Configuration.GENERATE_WORLD_SCALE;
            targetZonePositionY *= Configuration.GENERATE_WORLD_SCALE;
            targetZonePositionZ *= Configuration.GENERATE_WORLD_SCALE;
            boxTopNorthwestX *= Configuration.GENERATE_WORLD_SCALE;
            boxTopNorthwestY *= Configuration.GENERATE_WORLD_SCALE;
            boxTopNorthwestZ *= Configuration.GENERATE_WORLD_SCALE;
            boxBottomSoutheastX *= Configuration.GENERATE_WORLD_SCALE;
            boxBottomSoutheastY *= Configuration.GENERATE_WORLD_SCALE;
            boxBottomSoutheastZ *= Configuration.GENERATE_WORLD_SCALE;

            // Create the box base values
            TargetZoneShortName = targetZoneShortName;
            TargetZonePosition = new Vector3(targetZonePositionX, targetZonePositionY, targetZonePositionZ);
            switch (targetZoneOrientation)
            {
                case ZoneLineOrientationType.North: TargetZoneOrientation = 0; break;
                case ZoneLineOrientationType.South: TargetZoneOrientation = Convert.ToSingle(Math.PI); break;
                case ZoneLineOrientationType.West: TargetZoneOrientation = Convert.ToSingle(Math.PI * 0.5); break;
                case ZoneLineOrientationType.East: TargetZoneOrientation = Convert.ToSingle(Math.PI * 1.5); break;
                case ZoneLineOrientationType.NorthWest: TargetZoneOrientation = Convert.ToSingle(Math.PI * 0.25); break;
            }

            // Calculate the dimensions in the form needed by a wow trigger zone
            BoundingBox zoneLineBoxBounding = new BoundingBox(boxBottomSoutheastX, boxBottomSoutheastY, boxBottomSoutheastZ,
                boxTopNorthwestX, boxTopNorthwestY, boxTopNorthwestZ);
            BoxPosition = zoneLineBoxBounding.GetCenter();
            BoxWidth = zoneLineBoxBounding.GetYDistance();
            BoxLength = zoneLineBoxBounding.GetXDistance();
            BoxHeight = zoneLineBoxBounding.GetZDistance();
        }
    }
}
