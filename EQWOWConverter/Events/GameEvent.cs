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

namespace EQWOWConverter.Events
{
    internal class GameEvent
    {
        private static int CurGameEventsSQLID = Configuration.SQL_GAME_EVENTS_ID_START;
        private static readonly object GameEventSQLIDLock = new object();
        private static List<GameEvent> GameEvents = new List<GameEvent>();
        private static Dictionary<string, Dictionary<int, GameEvent>> GameEventsByZoneAndConditionID = new Dictionary<string, Dictionary<int, GameEvent>>();
        private static readonly object GameEventsLock = new object();

        public int GameEventsSQLID = -1;
        public List<string> ZoneShortNames = new List<string>();
        public int ZoneEventID = 0;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public GameEventNormalizeType NormalizeType = GameEventNormalizeType.None;
        public bool IsScheduled = true; // If it should run on a schedule, or require some event to trigger
        public int TriggerHour = 0;
        public int DurationInMinutes = 0;
        public int DurationInHours = 0;
        public DateTime? StartTime = null;
        public DateTime? EndTime = null;
        public int Occurrance = 1440; // This default means daily

        public GameEvent()
        {

        }

        public GameEvent(GameEvent other)
        {
            GameEventsSQLID = other.GameEventsSQLID;
            ZoneShortNames = other.ZoneShortNames;
            ZoneEventID = other.ZoneEventID;
            Name = other.Name;
            Description = other.Description;
            NormalizeType = other.NormalizeType;
            IsScheduled = other.IsScheduled;
            TriggerHour = other.TriggerHour;
            DurationInMinutes = other.DurationInMinutes;
            DurationInHours = other.DurationInHours;
            StartTime = other.StartTime;
            EndTime = other.EndTime;
            Occurrance = other.Occurrance;
        }

        public static int GenerateEventSQLID()
        {
            lock (GameEventSQLIDLock)
            {
                int returnEventID = CurGameEventsSQLID;
                CurGameEventsSQLID++;
                if (CurGameEventsSQLID > Configuration.SQL_GAME_EVENTS_ID_END)
                    Logger.WriteError("game_event ID exceeded ", Configuration.SQL_GAME_EVENTS_ID_END.ToString());
                return returnEventID;
            }
        }

        public static List<GameEvent> GetGameEventsList()
        {
            lock (GameEventsLock)
            {
                if (GameEvents.Count == 0)
                    PopulateGameEventsList();
                return GameEvents;
            }
        }

        public static GameEvent? GetGameEvent(string zoneShortName, int conditionID)
        {
            lock (GameEventsLock)
            {
                if (GameEvents.Count == 0)
                    PopulateGameEventsList();

                if (GameEventsByZoneAndConditionID.ContainsKey(zoneShortName) == false)
                    return null;
                if (GameEventsByZoneAndConditionID[zoneShortName].ContainsKey(conditionID) == false)
                    return null;
                else
                    return GameEventsByZoneAndConditionID[zoneShortName][conditionID];
            }
        }

        private static void PopulateGameEventsList()
        {
            GameEvents.Clear();

            // If normalizing, create the day/night records
            if (Configuration.EVENTS_DO_NORMALIZE_GAME_EVENTS == true)
            {
                // Day
                GameEvent dayEvent = new GameEvent();
                dayEvent.Name = "Day";
                dayEvent.TriggerHour = Configuration.EVENTS_NORMALIZED_DAY_SPAWN_START_HOUR;
                dayEvent.DurationInHours = Configuration.EVENTS_NORMALIZED_DAY_SPAWN_LENGTH_IN_HOUR;
                dayEvent.NormalizeType = GameEventNormalizeType.Day;
                dayEvent.ZoneEventID = 2; // Must always be 2
                dayEvent.IsScheduled = true;
                GameEvents.Add(dayEvent);

                // Night
                GameEvent nightEvent = new GameEvent();
                nightEvent.Name = "Night";
                nightEvent.TriggerHour = Configuration.EVENTS_NORMALIZED_NIGHT_SPAWN_START_HOUR;
                nightEvent.DurationInHours = Configuration.EVENTS_NORMALIZED_NIGHT_SPAWN_LENGTH_IN_HOUR;
                nightEvent.NormalizeType = GameEventNormalizeType.Night;
                nightEvent.ZoneEventID = 1; // Must always be 1
                nightEvent.IsScheduled = true;
                GameEvents.Add(nightEvent);

                // Dead Coldain
                GameEvent deadColdainEvent = new GameEvent();
                deadColdainEvent.Name = "Dead Coldain";
                deadColdainEvent.TriggerHour = 0;
                deadColdainEvent.DurationInHours = 2;
                deadColdainEvent.NormalizeType = GameEventNormalizeType.DeadColdain;
                deadColdainEvent.ZoneEventID = 3; // Must always be 3
                deadColdainEvent.IsScheduled = false;
                GameEvents.Add(nightEvent);
            }

            string gameEventsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "GameEvents.csv");
            Logger.WriteDebug("Populating Game Events list via file '" + gameEventsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(gameEventsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                GameEvent spawnEvent = new GameEvent();
                spawnEvent.ZoneShortNames.Add(columns["zone"].Trim().ToLower());
                spawnEvent.ZoneEventID = int.Parse(columns["zone_event"]);
                spawnEvent.Name = columns["name"];
                spawnEvent.IsScheduled = columns["is_scheduled"] == "1" ? true : false;
                spawnEvent.Description = string.Concat("EQ ", spawnEvent.ZoneShortNames[0], " ", spawnEvent.Name);
                switch (columns["normalize_type"].ToLower().Trim())
                {
                    case "day": spawnEvent.NormalizeType = GameEventNormalizeType.Day; break;
                    case "night": spawnEvent.NormalizeType = GameEventNormalizeType.Night; break;
                    case "deadcoldain": spawnEvent.NormalizeType = GameEventNormalizeType.DeadColdain; break;
                    case "none": spawnEvent.NormalizeType = GameEventNormalizeType.None; break;
                    default:
                        {
                            Logger.WriteError("GameEvent named '", spawnEvent.Name, "' has an unhandled normalize_type of '", columns["normalize_type"].ToLower().Trim(), "'");
                        }
                        break;
                }
                spawnEvent.TriggerHour = int.Parse(columns["trigger_hour"]);
                spawnEvent.DurationInHours = int.Parse(columns["duration_in_hour"]);

                // Group events since there's an upper limit in AzerothCore
                bool groupExists = false;
                if (spawnEvent.IsScheduled == true)
                {   
                    foreach (GameEvent groupEvent in GameEvents)
                    {
                        if (Configuration.EVENTS_DO_NORMALIZE_GAME_EVENTS == true)
                        {
                            if (spawnEvent.NormalizeType != GameEventNormalizeType.None)
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
                        if (groupEvent.ZoneEventID != spawnEvent.ZoneEventID)
                            continue;
                        groupExists = true;
                        groupEvent.ZoneShortNames.Add(spawnEvent.ZoneShortNames[0]);
                        break;
                    }
                }
                if (groupExists == false)
                    GameEvents.Add(new GameEvent(spawnEvent));
            }

            // Event post-load processing
            foreach (GameEvent gameEvent in GameEvents)
            {
                gameEvent.GameEventsSQLID = GenerateEventSQLID();

                // Clean up description
                string zonePluralFragment = string.Empty;
                if (gameEvent.ZoneShortNames.Count > 1)
                    zonePluralFragment = string.Concat(gameEvent.ZoneShortNames.Count.ToString(), " zones");
                else
                    zonePluralFragment = gameEvent.ZoneShortNames[0];
                string triggeredFragment = "Triggered";
                if (gameEvent.IsScheduled == true)
                    triggeredFragment = "Scheduled";
                gameEvent.Description = string.Concat("EQ ", triggeredFragment, " ", gameEvent.Name, " for ", zonePluralFragment);

                // Create lookups by zone and condition
                foreach (string zoneName in gameEvent.ZoneShortNames)
                {
                    if (GameEventsByZoneAndConditionID.ContainsKey(zoneName) == false)
                        GameEventsByZoneAndConditionID.Add(zoneName, new Dictionary<int, GameEvent>());
                    GameEventsByZoneAndConditionID[zoneName].Add(gameEvent.ZoneEventID, gameEvent);
                }

                // Set remaining derived elements
                gameEvent.StartTime = new DateTime(2000, 10, 29, gameEvent.TriggerHour, 0, 0);
                gameEvent.EndTime = new DateTime(Configuration.EVENTS_MAX_DATETIME_YEAR, 12, 30, 23, 0, 0);
                if (gameEvent.IsScheduled == false)
                    gameEvent.DurationInMinutes = 2592000;
                else
                    gameEvent.DurationInMinutes = gameEvent.DurationInHours * 60;
            }
        }
    }
}
