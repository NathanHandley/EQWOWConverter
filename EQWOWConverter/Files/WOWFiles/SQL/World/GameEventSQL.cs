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

namespace EQWOWConverter.WOWFiles
{
    internal class GameEventSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return string.Concat("DELETE FROM game_event WHERE `eventEntry` IN (", Configuration.SQL_GAMEEVENT_ID_DAY, ",", Configuration.SQL_GAMEEVENT_ID_NIGHT, ");");
        }

        public void AddRow(int id, DateTime? startTime, DateTime? endTime, int occurence, int length, string description)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("eventEntry", id);
            newRow.AddDateTime("start_time", startTime);
            newRow.AddDateTime("end_time", endTime);
            newRow.AddInt("occurence", occurence);
            newRow.AddInt("length", length);
            newRow.AddInt("holiday", 0);
            newRow.AddInt("holidayStage", 0);
            newRow.AddString("description", 255, description);
            newRow.AddInt("world_event", 0);
            newRow.AddInt("announce", 2);
            Rows.Add(newRow);
        }
    }
}
