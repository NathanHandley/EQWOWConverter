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

namespace EQWOWConverter.Transports
{
    internal class TransportLiftTrigger
    {
        private static List<TransportLiftTrigger> AllTransportLiftTriggers = new List<TransportLiftTrigger>();
        public static Dictionary<int, M2> ObjectModelM2ByMeshGameObjectDisplayID = new Dictionary<int, M2>();
        public static Dictionary<string, Sound> AllSoundsBySoundName = new Dictionary<string, Sound>();
        public string SpawnZoneShortName = string.Empty;
        public string Name = string.Empty;
        public string MeshName = string.Empty;
        public ActiveDoodadAnimType AnimationType = ActiveDoodadAnimType.SlideUpDownWithMod;
        public float SpawnX = 0;
        public float SpawnY = 0;
        public float SpawnZ = 0;
        public float Orientation = 0;
        public float AnimMod = 0;
        public int AnimTimeInMS = 0;
        public int ResetTimeInMS = 0;
        public int GameObjectGUID = 0;
        public int GameObjectTemplateID = 0;
        public int GameObjectDisplayInfoID = 0;
        public Sound? OpenSound = null;
        public Sound? CloseSound = null;

        public static List<TransportLiftTrigger> GetAllTransportLiftTriggers()
        {
            if (AllTransportLiftTriggers.Count == 0)
                PopulateTransportLiftTriggerList();
            return AllTransportLiftTriggers;
        }

        private static void PopulateTransportLiftTriggerList()
        {
            // Load all of transport lift triggers
            string transportLiftTriggersFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TransportLiftTriggers.csv");
            Logger.WriteDebug("Populating Transport Lift Triggers list via file '" + transportLiftTriggersFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(transportLiftTriggersFile, "|");
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
                TransportLiftTrigger curLiftTrigger = new TransportLiftTrigger();
                curLiftTrigger.SpawnZoneShortName = columns["spawn_zone"];
                switch (columns["anim_type"].ToLower().Trim())
                {
                    case "up_down": curLiftTrigger.AnimationType = ActiveDoodadAnimType.SlideUpDownWithMod; break;
                    case "rot_z": curLiftTrigger.AnimationType = ActiveDoodadAnimType.RotateAroundZClockwiseHalf; break;
                    default: Logger.WriteError("Unable to load transport lift trigger due to unhandled anim type of '" + columns["anim_type"] + "'"); continue;
                }
                curLiftTrigger.GameObjectTemplateID = int.Parse(columns["gotemplate_id"]);
                curLiftTrigger.Name = columns["name"];
                curLiftTrigger.MeshName = columns["mesh"];
                curLiftTrigger.SpawnX = float.Parse(columns["spawn_x"]) * Configuration.GENERATE_WORLD_SCALE;
                curLiftTrigger.SpawnY = float.Parse(columns["spawn_y"]) * Configuration.GENERATE_WORLD_SCALE;
                curLiftTrigger.SpawnZ = float.Parse(columns["spawn_z"]) * Configuration.GENERATE_WORLD_SCALE;
                curLiftTrigger.Orientation = float.Parse(columns["orientation"]);
                curLiftTrigger.AnimMod = float.Parse(columns["anim_mod"]);
                curLiftTrigger.AnimTimeInMS = int.Parse(columns["anim_time_in_ms"]);
                curLiftTrigger.ResetTimeInMS = int.Parse(columns["reset_in_ms"]);
                string openSoundName = columns["sound_open"].Trim();
                if (openSoundName.Length > 0)
                    curLiftTrigger.OpenSound = GetSound(openSoundName);
                string closeSoundName = columns["sound_close"].Trim();
                if (closeSoundName.Length > 0)
                    curLiftTrigger.CloseSound = GetSound(closeSoundName);
                curLiftTrigger.GameObjectGUID = GameObjectSQL.GenerateGUID();
                AllTransportLiftTriggers.Add(curLiftTrigger);
            }
        }
        
        private static Sound GetSound(string soundName)
        {
            if (AllSoundsBySoundName.ContainsKey(soundName.Trim()) == true)
                return AllSoundsBySoundName[soundName.Trim()];
            else
            {
                string name = "EQ TransportLiftTrigger " + Path.GetFileNameWithoutExtension(soundName);
                Sound returnSound = new Sound(name, Path.GetFileNameWithoutExtension(soundName), SoundType.GameObject, 8, 20, false);
                AllSoundsBySoundName.Add(soundName.Trim(), returnSound);
                return returnSound;
            }            
        }
    }
}
