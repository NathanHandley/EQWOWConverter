//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

using EQWOWConverter.Events;

namespace EQWOWConverter.Creatures
{
    internal class CreatureSpawnCondition
    {
        private static List<CreatureSpawnCondition> SpawnConditions = new List<CreatureSpawnCondition>();
        private static readonly object SpawnConditionLock = new object();

        public string ZoneShortName = string.Empty;
        public string Name = string.Empty;
        public int ID = 0;
        public int Value = 0;
        public int OnChange = 0;
        public GameEvent? LinkedGameEvent = null;

        public static List<CreatureSpawnCondition> GetSpawnConditionList()
        {
            lock (SpawnConditionLock)
            {
                if (SpawnConditions.Count == 0)
                    PopulateSpawnConditionList();
                return SpawnConditions;
            }
        }

        private static void PopulateSpawnConditionList()
        {
            SpawnConditions.Clear();

            string spawnConditionsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpawnConditions.csv");
            Logger.WriteDebug("Populating Spawn Conditions list via file '" + spawnConditionsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(spawnConditionsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                CreatureSpawnCondition spawnCondition = new CreatureSpawnCondition();
                spawnCondition.ZoneShortName = columns["zone"];
                spawnCondition.Name = columns["name"];
                spawnCondition.ID = int.Parse(columns["id"]);
                spawnCondition.Value = int.Parse(columns["value"]);
                spawnCondition.OnChange = int.Parse(columns["onchange"]);
                SpawnConditions.Add(spawnCondition);
            }
        }
    }
}
