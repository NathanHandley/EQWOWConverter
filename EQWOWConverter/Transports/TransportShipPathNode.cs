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
    internal class TransportShipPathNode : IComparable, IEquatable<TransportShipPathNode>
    {
        private static Dictionary<int, List<TransportShipPathNode>> TransportShipPathNodesByGroupID = new Dictionary<int, List<TransportShipPathNode>>();

        public int PathGroup = 0;
        public int WOWPathID = 0;
        public string MapShortName = string.Empty;
        public int StepNumber = 0;
        public float XPosition = 0;
        public float YPosition = 0;
        public float ZPosition = 0;
        public int PauseTimeInSec = 0;

        public static void SetPathIDForNodeGroup(int groupID, int wowPathID)
        {
            if (TransportShipPathNodesByGroupID.Count == 0)
                PopulatePathNodes();
            if (TransportShipPathNodesByGroupID.ContainsKey(groupID) == true)
                foreach(TransportShipPathNode node in TransportShipPathNodesByGroupID[groupID])
                    node.WOWPathID = wowPathID;
        }

        public static List<TransportShipPathNode> GetAllPathNodesSorted()
        {
            if (TransportShipPathNodesByGroupID.Count == 0)
                PopulatePathNodes();
            List<TransportShipPathNode> returnList = new List<TransportShipPathNode>();
            foreach (var transportShipPathNodes in TransportShipPathNodesByGroupID.Values)
                returnList.AddRange(transportShipPathNodes);
            returnList.Sort();
            return returnList;
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
                curNode.XPosition = float.Parse(columns["x"]) * Configuration.GENERATE_WORLD_SCALE;
                curNode.YPosition = float.Parse(columns["y"]) * Configuration.GENERATE_WORLD_SCALE;
                curNode.ZPosition = float.Parse(columns["z"]) * Configuration.GENERATE_WORLD_SCALE;
                curNode.PauseTimeInSec = int.Parse(columns["pause"]);            
                if (TransportShipPathNodesByGroupID.ContainsKey(curNode.PathGroup) == false)
                    TransportShipPathNodesByGroupID.Add(curNode.PathGroup, new List<TransportShipPathNode>());
                TransportShipPathNodesByGroupID[curNode.PathGroup].Add(curNode);
            }
        }

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            TransportShipPathNode? otherShipPathNode = obj as TransportShipPathNode;
            if (otherShipPathNode != null)
            {
                if (PathGroup != otherShipPathNode.PathGroup)
                    return PathGroup.CompareTo(otherShipPathNode.PathGroup);
                else return StepNumber.CompareTo(otherShipPathNode.StepNumber);
            }
            else
                throw new ArgumentException("Object is not a CreaturePathGridEntry");
        }

        public bool Equals(TransportShipPathNode? other)
        {
            if (other == null) return false;
            if (PathGroup != other.PathGroup) return false;
            if (StepNumber != other.StepNumber) return false;
            return true;
        }
    }
}
