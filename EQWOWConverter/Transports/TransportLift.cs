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
        TransportLiftTriggerType TriggerType = TransportLiftTriggerType.Automatic;

        public int LiftWOWGameObjectTemplateID = 0;
        public string LiftName = string.Empty;
        public int LiftPathGroupID = 0;
        public float LiftSpawnX = 0;
        public float LiftSpawnY = 0;
        public float LiftSpawnZ = 0;
        public float LiftOrientation = 0;        
        public string LiftMeshName = string.Empty;
        public int LiftGameObjectDisplayInfoID = 0;

        public int TriggerWOWGameObjectTemplateID = 0;
        public string TriggerName = string.Empty;
        public int TriggerPathGroupID = 0;
        public float TriggerSpawnX = 0;
        public float TriggerSpawnY = 0;
        public float TriggerSpawnZ = 0;
        public float TriggerOrientation = 0;
        public string TriggerMeshName = string.Empty;
        public int TriggerGameObjectDisplayInfoID = 0;

        public static List<TransportLift> GetAllTransportLifts()
        {
            if (TransportLifts.Count == 0)
                PopulateTransportLiftList();
            return TransportLifts;
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
                    case "toggle": curTransportLift.TriggerType = TransportLiftTriggerType.Automatic; break;
                    default: Logger.WriteError("Unable to load transport lift due to unhandled trigger type of '" + columns["trigger_type"] + "'"); continue;
                }
                curTransportLift.LiftWOWGameObjectTemplateID = int.Parse(columns["lift_gotemplate_id"]);
                curTransportLift.LiftName = columns["lift_name"];
                curTransportLift.LiftMeshName = columns["lift_mesh"];
                curTransportLift.LiftPathGroupID = int.Parse(columns["lift_path_group"]);                
                curTransportLift.LiftSpawnX = float.Parse(columns["lift_spawn_x"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportLift.LiftSpawnY = float.Parse(columns["lift_spawn_y"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportLift.LiftSpawnZ = float.Parse(columns["lift_spawn_z"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportLift.LiftOrientation = float.Parse(columns["lift_orientation"]);
                curTransportLift.TriggerWOWGameObjectTemplateID = int.Parse(columns["trigger_gotemplate_id"]);
                curTransportLift.TriggerName = columns["trigger_name"];
                curTransportLift.TriggerMeshName = columns["trigger_mesh"];
                curTransportLift.TriggerPathGroupID = int.Parse(columns["trigger_path_group"]);
                curTransportLift.TriggerSpawnX = float.Parse(columns["trigger_spawn_x"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportLift.TriggerSpawnY = float.Parse(columns["trigger_spawn_y"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportLift.TriggerSpawnZ = float.Parse(columns["trigger_spawn_z"]) * Configuration.GENERATE_WORLD_SCALE;
                curTransportLift.TriggerOrientation = float.Parse(columns["trigger_orientation"]);
                TransportLifts.Add(curTransportLift);
            }
        }
    }
}
