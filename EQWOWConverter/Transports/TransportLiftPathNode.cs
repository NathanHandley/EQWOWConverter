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
    internal class TransportLiftPathNode : IComparable, IEquatable<TransportLiftPathNode>
    {
        private static List<TransportLiftPathNode> TransportLiftPathNodes = new List<TransportLiftPathNode>();

        public int PathGroup = 0;
        public string ZoneShortName = string.Empty;
        public float XPositionOffset = 0;
        public float YPositionOffset = 0;
        public float ZPositionOffset = 0;
        public int TimestampInMS = 0;
        public int AnimationSequenceID = 0; // 0 = Stand
        public int GameObjectTemplateEntryID = 0;
        
        public static List<TransportLiftPathNode> GetAllPathNodesSorted()
        {
            if (TransportLiftPathNodes.Count == 0)
                PopulatePathNodes();
            TransportLiftPathNodes.Sort();
            return TransportLiftPathNodes;
        }

        public static void UpdateNodesWithGameObjectEntryIDByPathGroup(int pathGroup, int gameObjectTemplateEntryID)
        {
            if (TransportLiftPathNodes.Count == 0)
                PopulatePathNodes();
            foreach (TransportLiftPathNode node in TransportLiftPathNodes)
                if (node.PathGroup == pathGroup)
                    node.GameObjectTemplateEntryID = gameObjectTemplateEntryID;
        }

        private static void PopulatePathNodes()
        {
            // Load all of transport Lift nodes
            string transportLiftPathNodesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TransportLiftPathNodes.csv");
            Logger.WriteDebug("Populating Transport Lift Path Nodes list via file '" + transportLiftPathNodesFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(transportLiftPathNodesFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // TODO: Here
                TransportLiftPathNode curNode = new TransportLiftPathNode();
                curNode.PathGroup = int.Parse(columns["path_group"]);
                curNode.ZoneShortName = columns["zone_shortname"];
                curNode.XPositionOffset = float.Parse(columns["x"]) * Configuration.GENERATE_WORLD_SCALE;
                curNode.YPositionOffset = float.Parse(columns["y"]) * Configuration.GENERATE_WORLD_SCALE;
                curNode.ZPositionOffset = float.Parse(columns["z"]) * Configuration.GENERATE_WORLD_SCALE;
                curNode.TimestampInMS = int.Parse(columns["timestamp"]);
                curNode.AnimationSequenceID = int.Parse(columns["anim_seq_id"]);
                
                TransportLiftPathNodes.Add(curNode);
            }
        }

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            TransportLiftPathNode? otherLiftPathNode = obj as TransportLiftPathNode;
            if (otherLiftPathNode != null)
            {
                if (GameObjectTemplateEntryID != otherLiftPathNode.GameObjectTemplateEntryID)
                    return GameObjectTemplateEntryID.CompareTo(otherLiftPathNode.GameObjectTemplateEntryID);
                else return TimestampInMS.CompareTo(otherLiftPathNode.TimestampInMS);
            }
            else
                throw new ArgumentException("Object is not a CreaturePathGridEntry");
        }

        public bool Equals(TransportLiftPathNode? other)
        {
            if (other == null) return false;
            if (PathGroup != other.PathGroup) return false;
            if (TimestampInMS != other.TimestampInMS) return false;
            if (GameObjectTemplateEntryID != other.GameObjectTemplateEntryID) return false;
            return true;
        }
    }
}
