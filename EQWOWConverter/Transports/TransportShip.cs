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

using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Transports
{
    internal class TransportShip
    {
        private static List<TransportShip> TransportShips = new List<TransportShip>();
        public static Dictionary<int, WMO> TransportShipWMOsByGameObjectDisplayInfoID = new Dictionary<int, WMO>();

        public int EQNPCID = 0;
        public int WOWGameObjectTemplateID = 0;
        public string Name = string.Empty;
        public int PathGroupID = 0;
        public float SpawnX = 0;
        public float SpawnY = 0;
        public float SpawnZ = 0;
        public int SpawnHeading = 0;
        public string SpawnZoneShortName = string.Empty;
        public string TouchedZones = string.Empty;
        public int EQRace = 0;
        public int EQGender = 0;
        public int EQTexture = 0;
        public float EQRunSpeed = 0;
        public float EQWalkSpeed = 0;
        public int FixedSpeed = -1;
        public string MeshName = string.Empty;
        public bool IsSkeletal = false;
        public int GameObjectDisplayInfoID = 0;
        public int TaxiPathID = 0;
        public int MapID = 0;
        public float Scale = 1f;
        public float ConvexVolumePlaneXMin = 0;
        public float ConvexVolumePlaneXMax = 0;
        public float ConvexVolumePlaneYMin = 0;
        public float ConvexVolumePlaneYMax = 0;
        public float ConvexVolumePlaneZMin = 0;
        public float ConvexVolumePlaneZMax = 0;
        public int TriggeredByGameObjectTemplateID = 0;
        public int TriggeredByStepNum = -1;
        public int TriggeredToStepNum = -1;

        public List<string> GetTouchedZonesSplitOut()
        {
            return TouchedZones.Split(",").ToList();
        }

        public static List<TransportShip> GetAllTransportShips()
        {
            if (TransportShips.Count == 0)
                PopulateTransportShipList();
            return TransportShips;
        }

        private static void PopulateTransportShipList()
        {
            // Load all of transport ships
            string transportShipsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TransportShips.csv");
            Logger.WriteDebug("Populating Transport Ships list via file '" + transportShipsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(transportShipsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip invalid expansions and those that just aren't enabled
                bool isEnabled = int.Parse(columns["enabled"]) == 1 ? true : false;
                if (isEnabled == false)
                    continue;
                int minExpansion = int.Parse(columns["min_expansion"]);
                if (minExpansion > Configuration.GENERATE_EQ_EXPANSION_ID_TRANSPORTS)
                    continue;

                // Load the data
                TransportShip curTransportShip = new TransportShip();
                curTransportShip.EQNPCID = int.Parse(columns["eq_npc_id"]);
                curTransportShip.WOWGameObjectTemplateID = int.Parse(columns["wow_gotemplate_id"]);
                curTransportShip.Name = columns["name"];
                curTransportShip.MeshName = columns["mesh"];
                curTransportShip.IsSkeletal = columns["is_skeletal"] == "1" ? true : false;
                curTransportShip.PathGroupID = int.Parse(columns["path_group"]);
                curTransportShip.SpawnZoneShortName = columns["spawn_zone"];
                curTransportShip.SpawnX = float.Parse(columns["spawn_x"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportShip.SpawnY = float.Parse(columns["spawn_y"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportShip.SpawnZ = float.Parse(columns["spawn_z"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportShip.SpawnHeading = int.Parse(columns["heading"]);
                curTransportShip.EQRace = int.Parse(columns["race"]);
                curTransportShip.EQGender = int.Parse(columns["gender"]);
                curTransportShip.EQTexture = int.Parse(columns["texture"]);
                curTransportShip.EQWalkSpeed = float.Parse(columns["walkspeed"]);
                curTransportShip.EQRunSpeed = float.Parse(columns["runspeed"]);
                curTransportShip.TouchedZones = columns["touchedzones"];
                curTransportShip.TaxiPathID = TaxiPathDBC.GenerateID();
                curTransportShip.MapID = int.Parse(columns["wow_mapID"]);
                curTransportShip.FixedSpeed = int.Parse(columns["fixed_speed"]);
                curTransportShip.Scale = float.Parse(columns["scale"]);
                curTransportShip.ConvexVolumePlaneXMin = float.Parse(columns["mcvp_x_min"]);
                curTransportShip.ConvexVolumePlaneXMax = float.Parse(columns["mcvp_x_max"]);
                curTransportShip.ConvexVolumePlaneYMin = float.Parse(columns["mcvp_y_min"]);
                curTransportShip.ConvexVolumePlaneYMax = float.Parse(columns["mcvp_y_max"]);
                curTransportShip.ConvexVolumePlaneZMin = float.Parse(columns["mcvp_z_min"]);
                curTransportShip.ConvexVolumePlaneZMax = float.Parse(columns["mcvp_z_max"]);
                curTransportShip.TriggeredByGameObjectTemplateID = int.Parse(columns["triggered_by_gotemplate_id"]);
                curTransportShip.TriggeredByStepNum = int.Parse(columns["triggered_by_step_num"]);
                curTransportShip.TriggeredToStepNum = int.Parse(columns["triggers_to_step_num"]);

                TransportShips.Add(curTransportShip);
            }

            // Fill the path node IDs
            foreach (TransportShip transportShip in TransportShips)
                if (transportShip.PathGroupID > 0)
                    TransportShipPathNode.SetPathIDForNodeGroup(transportShip.PathGroupID, transportShip.TaxiPathID);
        }
    }
}
