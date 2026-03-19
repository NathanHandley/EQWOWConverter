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
        private static readonly object EventsLock = new object();

        public int GameEventsSQLID = -1;
        public DateTime? StartTime = null;
        public DateTime? EndTime = null;
        public int DurationInMinutes = 0;
        public string Description = string.Empty;
        public int Occurrance = 1440; // This default means daily

        public static int GenerateEventSQLID()
        {
            lock (EventsLock)
            {
                int returnEventID = CurGameEventsSQLID;
                CurGameEventsSQLID++;
                if (CurGameEventsSQLID > Configuration.SQL_GAME_EVENTS_ID_END)
                    Logger.WriteError("game_event ID exceeded ", Configuration.SQL_GAME_EVENTS_ID_END.ToString());
                return returnEventID;
            }
        }
    }
}
