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

namespace EQWOWConverter.Creatures
{
    // Note: These always repeat daily
    internal class CreatureSpawnEvent
    {
        private static List<CreatureSpawnEvent> SpawnEvents = new List<CreatureSpawnEvent>();
        private static readonly object SpawnEventsLock = new object();

        public int ID = 0;
        public string ZoneShortName = string.Empty;
        public int ConditionID = 0;
        public string Name = string.Empty;
        public int NextMinute = 0;
        public int NextHour = 0;
        public int Arguement = 0;

        public static List<CreatureSpawnEvent> GetSpawnEventsList()
        {
            lock (SpawnEventsLock)
            {
                if (SpawnEvents.Count == 0)
                    PopulateSpawnEventsList();
                return SpawnEvents;
            }
        }

        private static void PopulateSpawnEventsList()
        {
            SpawnEvents.Clear();

            string spawnEventsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpawnEvents.csv");
            Logger.WriteDebug("Populating Spawn Events list via file '" + spawnEventsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(spawnEventsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                CreatureSpawnEvent spawnEvent = new CreatureSpawnEvent();
                spawnEvent.ID = int.Parse(columns["id"]);
                spawnEvent.ZoneShortName = columns["zone"];
                spawnEvent.ConditionID = int.Parse(columns["cond_id"]);
                spawnEvent.Name = columns["name"];
                spawnEvent.NextMinute = int.Parse(columns["next_minute"]);
                spawnEvent.NextHour = int.Parse(columns["next_hour"]);
                spawnEvent.Arguement = int.Parse(columns["argument"]);
                SpawnEvents.Add(spawnEvent);
            }
        }
    }
}
