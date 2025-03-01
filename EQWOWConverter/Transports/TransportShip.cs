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

using EQWOWConverter.ObjectModels;
using EQWOWConverter.Zones;

namespace EQWOWConverter.Transports
{
    internal class TransportShip
    {
        private static List<TransportShip> TransportShips = new List<TransportShip>();

        public int EQNPCID = 0;
        public int WOWGameObjectTemplateID = 0;
        public string Name = string.Empty;
        public int PathGroupID = 0;
        public float SpawnX = 0;
        public float SpawnY = 0;
        public float SpawnZ = 0;
        public int SpawnHeading = 0;
        public string SpawnZoneShortName = string.Empty;
        public int EQRace = 0;
        public int EQGender = 0;
        public int EQTexture = 0;
        public float EQRunSpeed = 0;
        public float EQWalkSpeed = 0;
        public string MeshName = string.Empty;
        public List<TransportShipPathNode> PathNodes = new List<TransportShipPathNode>();

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
            Logger.WriteDetail("Populating Transport Ships list via file '" + transportShipsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(transportShipsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip invalid expansions and those that just aren't enabled
                bool isEnabled = int.Parse(columns["enabled"]) == 1 ? true : false;
                if (isEnabled == false)
                    continue;
                int minExpansion = int.Parse(columns["min_expansion"]);
                if (minExpansion < Configuration.GENERATE_EQ_EXPANSION_ID)
                    continue;

                // Load the data
                TransportShip curTransportShip = new TransportShip();
                curTransportShip.EQNPCID = int.Parse(columns["eq_npc_id"]);
                curTransportShip.WOWGameObjectTemplateID = int.Parse(columns["wow_gotemplate_id"]);
                curTransportShip.Name = columns["name"];
                curTransportShip.MeshName = columns["mesh"];
                curTransportShip.PathGroupID = int.Parse(columns["path_group"]);
                curTransportShip.SpawnZoneShortName = columns["spawn_zone"];
                curTransportShip.SpawnX = float.Parse(columns["spawn_x"]);
                curTransportShip.SpawnY = float.Parse(columns["spawn_y"]);
                curTransportShip.SpawnZ = float.Parse(columns["spawn_z"]);
                curTransportShip.SpawnHeading = int.Parse(columns["heading"]);
                curTransportShip.EQRace = int.Parse(columns["race"]);
                curTransportShip.EQGender = int.Parse(columns["gender"]);
                curTransportShip.EQTexture = int.Parse(columns["texture"]);
                curTransportShip.EQWalkSpeed = float.Parse(columns["walkspeed"]);
                curTransportShip.EQRunSpeed = float.Parse(columns["runspeed"]);
                TransportShips.Add(curTransportShip);
            }

            // Fill the path nodes
            foreach (TransportShip transportShip in TransportShips)
                if (transportShip.PathGroupID > 0)
                    transportShip.PathNodes = TransportShipPathNode.GetPathNodesForGroup(transportShip.PathGroupID);
        }
    }
}
