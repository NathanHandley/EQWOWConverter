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

using EQWOWConverter.Events;

namespace EQWOWConverter.WOWFiles
{
    internal class GameEventSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return string.Concat("DELETE FROM game_event WHERE `eventEntry` >= ", Configuration.SQL_GAME_EVENTS_ID_START, " AND `eventEntry` <= ", Configuration.SQL_GAME_EVENTS_ID_END, ";");
        }

        public void AddRow(GameEvent gameEvent)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("eventEntry", gameEvent.GameEventsSQLID);
            newRow.AddDateTime("start_time", gameEvent.StartTime);
            newRow.AddDateTime("end_time", gameEvent.EndTime);
            newRow.AddInt("occurence", gameEvent.Occurrance);
            newRow.AddInt("length", gameEvent.DurationInMinutes);
            newRow.AddInt("holiday", 0);
            newRow.AddInt("holidayStage", 0);
            newRow.AddString("description", 255, gameEvent.Description);
            newRow.AddInt("world_event", 0);
            newRow.AddInt("announce", 2);
            Rows.Add(newRow);
        }
    }
}
