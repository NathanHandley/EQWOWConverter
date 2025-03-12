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
    internal class TransportLift
    {
        private static List<TransportLift> TransportLifts = new List<TransportLift>();
        public static Dictionary<int, M2> ObjectModelM2ByMeshGameObjectDisplayID = new Dictionary<int, M2>();

        public string SpawnZoneShortName = string.Empty;
        public TransportLiftTriggerType TriggerType = TransportLiftTriggerType.Automatic;
        public string Name = string.Empty;
        public string MeshName = string.Empty;
        public int PathGroupID = 0;
        public float SpawnX = 0;
        public float SpawnY = 0;
        public float SpawnZ = 0;
        public float Orientation = 0;
        public int GameObjectGUID = 0;
        public int GameObjectTemplateID = 0;
        public int GameObjectDisplayInfoID = 0;


        public static List<TransportLift> GetAllTransportLifts()
        {
            if (TransportLifts.Count == 0)
                PopulateTransportLiftList();
            return TransportLifts;
        }

        public static int GetTransportLiftGameObjectGUIDByTemplateID(int gameObjectTemplateID)
        {
            foreach(TransportLift transportLift in TransportLifts)
            {
                if (transportLift.GameObjectTemplateID == gameObjectTemplateID)
                    return transportLift.GameObjectGUID;
            }
            Logger.WriteError("Failed to get GameObject GUID for transport lift with template id '" + gameObjectTemplateID + "'");
            return 0;
        }

        private static void PopulateTransportLiftList()
        {
            // Load all of transport lifts
            string transportLiftsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TransportLifts.csv");
            Logger.WriteDetail("Populating Transport Lifts list via file '" + transportLiftsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(transportLiftsFile, "|");
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
                TransportLift curTransportLift = new TransportLift();
                curTransportLift.SpawnZoneShortName = columns["spawn_zone"];
                switch (columns["trigger_type"].ToLower().Trim())
                {
                    case "automatic": curTransportLift.TriggerType = TransportLiftTriggerType.Automatic; break;
                    case "toggle": curTransportLift.TriggerType = TransportLiftTriggerType.Toggle; break;
                    default: Logger.WriteError("Unable to load transport lift due to unhandled trigger type of '" + columns["trigger_type"] + "'"); continue;
                }
                curTransportLift.GameObjectTemplateID = int.Parse(columns["gotemplate_id"]);
                curTransportLift.Name = columns["name"];
                curTransportLift.MeshName = columns["mesh"];
                curTransportLift.PathGroupID = int.Parse(columns["path_group"]);                
                curTransportLift.SpawnX = float.Parse(columns["spawn_x"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportLift.SpawnY = float.Parse(columns["spawn_y"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportLift.SpawnZ = float.Parse(columns["spawn_z"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportLift.Orientation = float.Parse(columns["orientation"]);
                curTransportLift.GameObjectGUID = GameObjectSQL.GenerateGUID();
                TransportLifts.Add(curTransportLift);
            }
        }
    }
}
