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
    internal class ZoneGameObject
    {
        protected static Dictionary<string, List<ZoneGameObject>> ZoneGameObjectsByZoneShortname = new Dictionary<string, List<ZoneGameObject>>();
        protected static readonly object ZoneGameObjectsLock = new object();

        public int ID;
        public int DoorID;
        public ZoneGameObjectType ObjectType = ZoneGameObjectType.Unknown;
        public ZoneGameObjectOpenType OpenType = ZoneGameObjectOpenType.Unknown;
        public string ZoneShortName = string.Empty;
        public string ModelName = string.Empty;
        public Vector3 Position = new Vector3();
        public float Orientation;

        public static Dictionary<string, List<ZoneGameObject>> GetAllZoneGameObjectsByZoneShortNames()
        {
            lock (ZoneGameObjectsLock)
            {
                if (ZoneGameObjectsByZoneShortname.Count == 0)
                    LoadZoneGameObjects();
                return ZoneGameObjectsByZoneShortname;
            }
        }

        private static void LoadZoneGameObjects()
        {
            // Load the file first
            string zoneGameObjectsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ZoneGameObjects.csv");
            Logger.WriteDebug(string.Concat("Populating Zone Game Object list via file '", zoneGameObjectsFile, "'"));
            List<Dictionary<string, string>> gameObjectsRows = FileTool.ReadAllRowsFromFileWithHeader(zoneGameObjectsFile, "|");

            // Process the rows
            foreach(Dictionary<string, string> gameObjectsRow in gameObjectsRows)
            {
                ZoneGameObject newGameObject = new ZoneGameObject();
                newGameObject.ID = int.Parse(gameObjectsRow["id"]);
                newGameObject.DoorID = int.Parse(gameObjectsRow["doorid"]);
                newGameObject.ObjectType = GetType(gameObjectsRow["type"]);
                newGameObject.OpenType = GetOpenType(int.Parse(gameObjectsRow["opentype"]));
                newGameObject.ZoneShortName = gameObjectsRow["zone"];
                newGameObject.ModelName = gameObjectsRow["name"];
                float xPosition = float.Parse(gameObjectsRow["pos_x"]) * Configuration.GENERATE_WORLD_SCALE;
                float yPosition = float.Parse(gameObjectsRow["pos_y"]) * Configuration.GENERATE_WORLD_SCALE;
                float zPosition = float.Parse(gameObjectsRow["pos_z"]) * Configuration.GENERATE_WORLD_SCALE;
                newGameObject.Position = new Vector3(xPosition, yPosition, zPosition);
                float eqHeading = float.Parse(gameObjectsRow["heading"]);
                float wowHeading = 0;
                if (eqHeading != 0)
                    wowHeading = eqHeading / (256f / 360f);
                newGameObject.Orientation = wowHeading;

                // Add it
                if (ZoneGameObjectsByZoneShortname.ContainsKey(newGameObject.ZoneShortName) == false)
                    ZoneGameObjectsByZoneShortname.Add(newGameObject.ZoneShortName, new List<ZoneGameObject>());
                ZoneGameObjectsByZoneShortname[newGameObject.ZoneShortName].Add(newGameObject);
            }
        }

        private static ZoneGameObjectOpenType GetOpenType(int openTypeID)
        {
            switch (openTypeID)
            {
                case 0: return ZoneGameObjectOpenType.TYPE0;
                case 1: return ZoneGameObjectOpenType.TYPE1;
                case 2: return ZoneGameObjectOpenType.TYPE2;
                case 5: return ZoneGameObjectOpenType.TYPE5;
                case 6: return ZoneGameObjectOpenType.TYPE6;
                case 7: return ZoneGameObjectOpenType.TYPE7;
                case 10: return ZoneGameObjectOpenType.TYPE10;
                case 12: return ZoneGameObjectOpenType.TYPE12;
                case 15: return ZoneGameObjectOpenType.TYPE15;
                case 16: return ZoneGameObjectOpenType.TYPE16;
                case 17: return ZoneGameObjectOpenType.TYPE17;
                case 21: return ZoneGameObjectOpenType.TYPE21;
                case 22: return ZoneGameObjectOpenType.TYPE22;
                case 25: return ZoneGameObjectOpenType.TYPE25;
                case 26: return ZoneGameObjectOpenType.TYPE26;
                case 30: return ZoneGameObjectOpenType.TYPE30;
                case 35: return ZoneGameObjectOpenType.TYPE35;
                case 40: return ZoneGameObjectOpenType.TYPE40;
                case 45: return ZoneGameObjectOpenType.TYPE45;
                case 53: return ZoneGameObjectOpenType.TYPE53;
                case 54: return ZoneGameObjectOpenType.TYPE54;
                case 55: return ZoneGameObjectOpenType.TYPE55;
                case 56: return ZoneGameObjectOpenType.TYPE56;
                case 57: return ZoneGameObjectOpenType.TYPE57;
                case 58: return ZoneGameObjectOpenType.TYPE58;
                case 59: return ZoneGameObjectOpenType.TYPE59;
                case 60: return ZoneGameObjectOpenType.TYPE60;
                case 61: return ZoneGameObjectOpenType.TYPE61;
                case 65: return ZoneGameObjectOpenType.TYPE65;
                case 66: return ZoneGameObjectOpenType.TYPE66;
                case 70: return ZoneGameObjectOpenType.TYPE70;
                case 71: return ZoneGameObjectOpenType.TYPE71;
                case 72: return ZoneGameObjectOpenType.TYPE72;
                case 74: return ZoneGameObjectOpenType.TYPE74;
                case 75: return ZoneGameObjectOpenType.TYPE75;
                case 76: return ZoneGameObjectOpenType.TYPE76;
                case 77: return ZoneGameObjectOpenType.TYPE77;
                case 100: return ZoneGameObjectOpenType.TYPE100;
                case 101: return ZoneGameObjectOpenType.TYPE101;
                case 105: return ZoneGameObjectOpenType.TYPE105;
                case 106: return ZoneGameObjectOpenType.TYPE106;
                case 120: return ZoneGameObjectOpenType.TYPE120;
                case 125: return ZoneGameObjectOpenType.TYPE125;
                case 130: return ZoneGameObjectOpenType.TYPE130;
                case 140: return ZoneGameObjectOpenType.TYPE140;
                case 145: return ZoneGameObjectOpenType.TYPE145;
                case 156: return ZoneGameObjectOpenType.TYPE156;
                default:
                    {
                        Logger.WriteError("Can't determine GameObjectOpenType due to an unmapped open type id of " + openTypeID.ToString());
                        return ZoneGameObjectOpenType.Unknown;
                    }
            }
        }

        private static ZoneGameObjectType GetType(string typeNameValue)
        {
            switch (typeNameValue.Trim().ToLower())
            {
                case "noninteract": return ZoneGameObjectType.NonInteract;
                case "door": return ZoneGameObjectType.Door;
                case "boat": return ZoneGameObjectType.Boat;
                case "bridge": return ZoneGameObjectType.Bridge;
                case "lever": return ZoneGameObjectType.Lever;
                case "teleport": return ZoneGameObjectType.Teleport;
                case "trap": return ZoneGameObjectType.Trap;
                default:
                    {
                        Logger.WriteError("Can't determine GameObjectType due to an unmapped open type name value of " + typeNameValue);
                        return ZoneGameObjectType.Unknown;
                    }

            }
        }
    }
}
