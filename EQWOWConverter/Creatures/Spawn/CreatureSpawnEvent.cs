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
        private static List<CreatureSpawnEvent> IndividualSpawnEvents = new List<CreatureSpawnEvent>();
        private static List<CreatureSpawnEvent> GroupedSpawnEvents = new List<CreatureSpawnEvent>();
        private static int CurGameEventsSQLID = Configuration.SQL_GAME_EVENTS_ID_START;
        private static readonly object SpawnEventsLock = new object();

        public List<int> EQIDs = new List<int>();
        public List<string> ZoneShortNames = new List<string>();
        public int ConditionID = 0;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public CreatureSpawnEventNormalizeType NormalizeType = CreatureSpawnEventNormalizeType.None;
        public int TriggerHour = 0;
        public int DurationInHours = 0;
        public int DurationInMinutes = 0;
        public int GameEventsSQLID = -1;
        public DateTime? StartTime = null;
        public DateTime? EndTime = null;

        public CreatureSpawnEvent()
        {

        }

        public CreatureSpawnEvent(CreatureSpawnEvent other)
        {
            EQIDs = other.EQIDs;
            ZoneShortNames = other.ZoneShortNames;
            ConditionID = other.ConditionID;
            Name = other.Name;
            Description = other.Description;
            NormalizeType = other.NormalizeType;
            TriggerHour = other.TriggerHour;
            DurationInHours = other.DurationInHours;
            DurationInMinutes = other.DurationInMinutes;
            GameEventsSQLID = other.GameEventsSQLID;
            StartTime = other.StartTime;
            EndTime = other.EndTime;
        }

        public static List<CreatureSpawnEvent> GetGroupSpawnEventsList()
        {
            lock (SpawnEventsLock)
            {
                if (GroupedSpawnEvents.Count == 0)
                    PopulateSpawnEventsList();
                return GroupedSpawnEvents;
            }
        }

        private static void PopulateSpawnEventsList()
        {
            GroupedSpawnEvents.Clear();
            IndividualSpawnEvents.Clear();

            // If normalizing, create the day/night records
            if (Configuration.EVENTS_DO_NORMALIZE_DAYNIGHT_SPAWN_EVENTS == true)
            {
                // Day
                CreatureSpawnEvent dayEvent = new CreatureSpawnEvent();
                dayEvent.Name = "Day";
                dayEvent.StartTime = new DateTime(2000, 10, 29, Configuration.EVENTS_NORMALIZED_DAY_SPAWN_START_HOUR, 0, 0);
                dayEvent.TriggerHour = Configuration.EVENTS_NORMALIZED_DAY_SPAWN_START_HOUR;
                dayEvent.EndTime = new DateTime(Configuration.EVENTS_MAX_DATETIME_YEAR, 12, 30, 23, 0, 0);
                dayEvent.DurationInHours = Configuration.EVENTS_NORMALIZED_DAY_SPAWN_LENGTH_IN_HOUR;
                dayEvent.DurationInMinutes = Configuration.EVENTS_NORMALIZED_DAY_SPAWN_LENGTH_IN_HOUR * 60;
                dayEvent.NormalizeType = CreatureSpawnEventNormalizeType.Day;
                dayEvent.GameEventsSQLID = GenerateEventID();
                GroupedSpawnEvents.Add(dayEvent);

                // Night
                CreatureSpawnEvent nightEvent = new CreatureSpawnEvent();
                nightEvent.Name = "Night";
                nightEvent.StartTime = new DateTime(2000, 10, 29, Configuration.EVENTS_NORMALIZED_NIGHT_SPAWN_START_HOUR, 0, 0);
                nightEvent.TriggerHour = Configuration.EVENTS_NORMALIZED_NIGHT_SPAWN_START_HOUR;
                nightEvent.EndTime = new DateTime(Configuration.EVENTS_MAX_DATETIME_YEAR, 12, 30, 23, 0, 0);
                nightEvent.DurationInHours = Configuration.EVENTS_NORMALIZED_NIGHT_SPAWN_LENGTH_IN_HOUR;
                nightEvent.DurationInMinutes = Configuration.EVENTS_NORMALIZED_NIGHT_SPAWN_LENGTH_IN_HOUR * 60;
                nightEvent.NormalizeType = CreatureSpawnEventNormalizeType.Night;
                nightEvent.GameEventsSQLID = GenerateEventID();
                GroupedSpawnEvents.Add(nightEvent);
            }

            string spawnEventsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpawnEvents.csv");
            Logger.WriteDebug("Populating Spawn Events list via file '" + spawnEventsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(spawnEventsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                CreatureSpawnEvent spawnEvent = new CreatureSpawnEvent();
                spawnEvent.EQIDs.Add(int.Parse(columns["id"]));
                spawnEvent.ZoneShortNames.Add(columns["zone"].Trim().ToLower());
                spawnEvent.ConditionID = int.Parse(columns["cond_id"]);
                spawnEvent.Name = columns["name"];
                spawnEvent.Description = string.Concat("EQ ", spawnEvent.ZoneShortNames[0], " ", spawnEvent.Name);
                switch (columns["normalize_type"].ToLower().Trim())
                {
                    case "day": spawnEvent.NormalizeType = CreatureSpawnEventNormalizeType.Day; break;
                    case "night": spawnEvent.NormalizeType = CreatureSpawnEventNormalizeType.Night; break;
                    case "none": spawnEvent.NormalizeType = CreatureSpawnEventNormalizeType.None; break;
                    default:
                        {
                            Logger.WriteError("SpawnEvent named '", spawnEvent.Name, "' has an unhandled normalize_type of '", columns["normalize_type"].ToLower().Trim(), "'");
                        } break;
                }
                spawnEvent.TriggerHour = int.Parse(columns["trigger_hour"]);
                spawnEvent.DurationInHours = int.Parse(columns["duration_in_hour"]);
                spawnEvent.DurationInMinutes = spawnEvent.DurationInHours * 60;
                spawnEvent.StartTime = new DateTime(2000, 10, 29, spawnEvent.TriggerHour, 0, 0);
                spawnEvent.EndTime = new DateTime(Configuration.EVENTS_MAX_DATETIME_YEAR, 12, 30, 23, 0, 0);
                IndividualSpawnEvents.Add(spawnEvent);

                // Also add to the grouped events since there's an upper limit in AzerothCore
                bool groupExists = false;
                foreach (CreatureSpawnEvent groupEvent in GroupedSpawnEvents)
                {
                    if (Configuration.EVENTS_DO_NORMALIZE_DAYNIGHT_SPAWN_EVENTS == true)
                    {
                        if (spawnEvent.NormalizeType != CreatureSpawnEventNormalizeType.None)
                        {
                            if (spawnEvent.NormalizeType == groupEvent.NormalizeType)
                            {
                                groupExists = true;
                                groupEvent.ZoneShortNames.Add(spawnEvent.ZoneShortNames[0]);
                                break;
                            }
                        }
                    }

                    if (groupEvent.Name != spawnEvent.Name)
                        continue;
                    if (groupEvent.TriggerHour != spawnEvent.TriggerHour)
                        continue;
                    if (groupEvent.DurationInHours != spawnEvent.DurationInHours)
                        continue;
                    if (groupEvent.ConditionID != spawnEvent.ConditionID)
                        continue;
                    groupExists = true;
                    groupEvent.ZoneShortNames.Add(spawnEvent.ZoneShortNames[0]);
                    break;
                }
                if (groupExists == false)
                    GroupedSpawnEvents.Add(new CreatureSpawnEvent(spawnEvent));
            }

            // Clean up descriptions of groups / Create IDs
            foreach (CreatureSpawnEvent groupEvent in GroupedSpawnEvents)
            {
                string zonePluralString = string.Empty;
                if (groupEvent.ZoneShortNames.Count > 1)
                    zonePluralString = string.Concat(groupEvent.ZoneShortNames.Count.ToString(), " zones");
                else
                    zonePluralString = groupEvent.ZoneShortNames[0];
                groupEvent.Description = string.Concat("EQ ", groupEvent.Name, " for ", zonePluralString);
                groupEvent.GameEventsSQLID = GenerateEventID();
            }
        }

        private static int GenerateEventID()
        {
            int returnEventID = CurGameEventsSQLID;
            CurGameEventsSQLID++;
            if (CurGameEventsSQLID > Configuration.SQL_GAME_EVENTS_ID_END)
                Logger.WriteError("game_event ID exceeded ", Configuration.SQL_GAME_EVENTS_ID_END.ToString());
            return returnEventID;
        }
    }
}
