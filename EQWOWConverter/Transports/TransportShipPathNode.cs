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

namespace EQWOWConverter.Transports
{
    internal class TransportShipPathNode
    {
        private static Dictionary<int, List<TransportShipPathNode>> TransportShipPathNodesByGroupID = new Dictionary<int, List<TransportShipPathNode>>();

        public int PathGroup = 0;
        public string MapShortName = string.Empty;
        public int StepNumber = 0;
        public float XPosition = 0;
        public float YPosition = 0;
        public float ZPosition = 0;
        public int PauseTimeInSec = 0;

        public static List<TransportShipPathNode> GetPathNodesForGroup(int groupID)
        {
            if (TransportShipPathNodesByGroupID.Count == 0)
                PopulatePathNodes();
            if (TransportShipPathNodesByGroupID.ContainsKey(groupID) == false)
                return new List<TransportShipPathNode>();
            else
                return TransportShipPathNodesByGroupID[groupID];
        }

        private static void PopulatePathNodes()
        {
            // Load all of transport ship nodes
            string transportShipPathNodesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TransportShipPathNodes.csv");
            Logger.WriteDetail("Populating Transport Ship Path Nodes list via file '" + transportShipPathNodesFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(transportShipPathNodesFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // TODO: Here
                TransportShipPathNode curNode = new TransportShipPathNode();
                curNode.PathGroup = int.Parse(columns["path_group"]);
                curNode.MapShortName = columns["map_short_name"];
                curNode.StepNumber = int.Parse(columns["step_num"]);
                curNode.XPosition = float.Parse(columns["x"]);
                curNode.YPosition = float.Parse(columns["y"]);
                curNode.ZPosition = float.Parse(columns["z"]);
                curNode.PauseTimeInSec = int.Parse(columns["pause"]);
                if (TransportShipPathNodesByGroupID.ContainsKey(curNode.PathGroup) == false)
                    TransportShipPathNodesByGroupID.Add(curNode.PathGroup, new List<TransportShipPathNode>());
                TransportShipPathNodesByGroupID[curNode.PathGroup].Add(curNode);
            }
        }
    }
}
