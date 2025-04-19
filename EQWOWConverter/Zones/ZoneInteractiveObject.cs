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
    internal class ZoneInteractiveObject
    {
        protected static Dictionary<string, List<ZoneInteractiveObject>> ZoneInteractiveObjectsByZoneShortname = new Dictionary<string, List<ZoneInteractiveObject>>();
        protected static readonly object ZoneInteractiveObjectsLock = new object();

        public int ID;
        public int DoorID;
        public ZoneInteractiveObjectType ObjectType = ZoneInteractiveObjectType.Unknown;
        public ZoneInteractiveObjectOpenType OpenType = ZoneInteractiveObjectOpenType.Unknown;
        public string ZoneShortName = string.Empty;
        public string ModelName = string.Empty;
        public Vector3 Position = new Vector3();
        public float Orientation;

        public static Dictionary<string, List<ZoneInteractiveObject>> GetAllZoneInteractiveObjectsByZoneShortNames()
        {
            lock (ZoneInteractiveObjectsLock)
            {
                if (ZoneInteractiveObjectsByZoneShortname.Count == 0)
                    LoadZoneInteractiveObjects();
                return ZoneInteractiveObjectsByZoneShortname;
            }
        }

        private static void LoadZoneInteractiveObjects()
        {
            // Load the file first
            string zoneInteractiveObjectsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ZoneInteractiveObjects.csv");
            Logger.WriteDebug(string.Concat("Populating Zone Interactive Object list via file '", zoneInteractiveObjectsFile, "'"));
            List<Dictionary<string, string>> interactiveObjectsRows = FileTool.ReadAllRowsFromFileWithHeader(zoneInteractiveObjectsFile, "|");

            // Process the rows
            foreach(Dictionary<string, string> interactiveObjectsRow in interactiveObjectsRows)
            {
                ZoneInteractiveObject newInteractiveObject = new ZoneInteractiveObject();
                newInteractiveObject.ID = int.Parse(interactiveObjectsRow["id"]);
                newInteractiveObject.DoorID = int.Parse(interactiveObjectsRow["doorid"]);
                newInteractiveObject.ObjectType = GetType(interactiveObjectsRow["type"]);
                newInteractiveObject.OpenType = GetOpenType(int.Parse(interactiveObjectsRow["opentype"]));
                newInteractiveObject.ZoneShortName = interactiveObjectsRow["zone"];
                newInteractiveObject.ModelName = interactiveObjectsRow["name"];
                float xPosition = float.Parse(interactiveObjectsRow["pos_x"]) * Configuration.GENERATE_WORLD_SCALE;
                float yPosition = float.Parse(interactiveObjectsRow["pos_y"]) * Configuration.GENERATE_WORLD_SCALE;
                float zPosition = float.Parse(interactiveObjectsRow["pos_z"]) * Configuration.GENERATE_WORLD_SCALE;
                newInteractiveObject.Position = new Vector3(xPosition, yPosition, zPosition);
                float eqHeading = float.Parse(interactiveObjectsRow["heading"]);
                float wowHeading = 0;
                if (eqHeading != 0)
                    wowHeading = eqHeading / (256f / 360f);
                newInteractiveObject.Orientation = wowHeading;

                // Add it
                if (ZoneInteractiveObjectsByZoneShortname.ContainsKey(newInteractiveObject.ZoneShortName) == false)
                    ZoneInteractiveObjectsByZoneShortname.Add(newInteractiveObject.ZoneShortName, new List<ZoneInteractiveObject>());
                ZoneInteractiveObjectsByZoneShortname[newInteractiveObject.ZoneShortName].Add(newInteractiveObject);
            }
        }

        private static ZoneInteractiveObjectOpenType GetOpenType(int openTypeID)
        {
            switch (openTypeID)
            {
                case 0: return ZoneInteractiveObjectOpenType.TYPE0;
                case 1: return ZoneInteractiveObjectOpenType.TYPE1;
                case 2: return ZoneInteractiveObjectOpenType.TYPE2;
                case 5: return ZoneInteractiveObjectOpenType.TYPE5;
                case 6: return ZoneInteractiveObjectOpenType.TYPE6;
                case 7: return ZoneInteractiveObjectOpenType.TYPE7;
                case 10: return ZoneInteractiveObjectOpenType.TYPE10;
                case 12: return ZoneInteractiveObjectOpenType.TYPE12;
                case 15: return ZoneInteractiveObjectOpenType.TYPE15;
                case 16: return ZoneInteractiveObjectOpenType.TYPE16;
                case 17: return ZoneInteractiveObjectOpenType.TYPE17;
                case 21: return ZoneInteractiveObjectOpenType.TYPE21;
                case 22: return ZoneInteractiveObjectOpenType.TYPE22;
                case 25: return ZoneInteractiveObjectOpenType.TYPE25;
                case 26: return ZoneInteractiveObjectOpenType.TYPE26;
                case 30: return ZoneInteractiveObjectOpenType.TYPE30;
                case 35: return ZoneInteractiveObjectOpenType.TYPE35;
                case 40: return ZoneInteractiveObjectOpenType.TYPE40;
                case 45: return ZoneInteractiveObjectOpenType.TYPE45;
                case 53: return ZoneInteractiveObjectOpenType.TYPE53;
                case 54: return ZoneInteractiveObjectOpenType.TYPE54;
                case 55: return ZoneInteractiveObjectOpenType.TYPE55;
                case 56: return ZoneInteractiveObjectOpenType.TYPE56;
                case 57: return ZoneInteractiveObjectOpenType.TYPE57;
                case 58: return ZoneInteractiveObjectOpenType.TYPE58;
                case 59: return ZoneInteractiveObjectOpenType.TYPE59;
                case 60: return ZoneInteractiveObjectOpenType.TYPE60;
                case 61: return ZoneInteractiveObjectOpenType.TYPE61;
                case 65: return ZoneInteractiveObjectOpenType.TYPE65;
                case 66: return ZoneInteractiveObjectOpenType.TYPE66;
                case 70: return ZoneInteractiveObjectOpenType.TYPE70;
                case 71: return ZoneInteractiveObjectOpenType.TYPE71;
                case 72: return ZoneInteractiveObjectOpenType.TYPE72;
                case 74: return ZoneInteractiveObjectOpenType.TYPE74;
                case 75: return ZoneInteractiveObjectOpenType.TYPE75;
                case 76: return ZoneInteractiveObjectOpenType.TYPE76;
                case 77: return ZoneInteractiveObjectOpenType.TYPE77;
                case 100: return ZoneInteractiveObjectOpenType.TYPE100;
                case 101: return ZoneInteractiveObjectOpenType.TYPE101;
                case 105: return ZoneInteractiveObjectOpenType.TYPE105;
                case 106: return ZoneInteractiveObjectOpenType.TYPE106;
                case 120: return ZoneInteractiveObjectOpenType.TYPE120;
                case 125: return ZoneInteractiveObjectOpenType.TYPE125;
                case 130: return ZoneInteractiveObjectOpenType.TYPE130;
                case 140: return ZoneInteractiveObjectOpenType.TYPE140;
                case 145: return ZoneInteractiveObjectOpenType.TYPE145;
                case 156: return ZoneInteractiveObjectOpenType.TYPE156;
                default:
                    {
                        Logger.WriteError("Can't determine InteractiveObjectOpenType due to an unmapped open type id of " + openTypeID.ToString());
                        return ZoneInteractiveObjectOpenType.Unknown;
                    }
            }
        }

        private static ZoneInteractiveObjectType GetType(string typeNameValue)
        {
            switch (typeNameValue.Trim().ToLower())
            {
                case "noninteract": return ZoneInteractiveObjectType.NonInteract;
                case "door": return ZoneInteractiveObjectType.Door;
                case "boat": return ZoneInteractiveObjectType.Boat;
                case "bridge": return ZoneInteractiveObjectType.Bridge;
                case "lever": return ZoneInteractiveObjectType.Lever;
                case "teleport": return ZoneInteractiveObjectType.Teleport;
                case "trap": return ZoneInteractiveObjectType.Trap;
                default:
                    {
                        Logger.WriteError("Can't determine InteractiveObjectType due to an unmapped open type name value of " + typeNameValue);
                        return ZoneInteractiveObjectType.Unknown;
                    }

            }
        }
    }
}
