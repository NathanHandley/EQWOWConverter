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

namespace EQWOWConverter.WOWFiles
{
    internal class GameEventPoolSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return string.Concat("DELETE FROM game_event_pool WHERE `eventEntry` >= ", Configuration.SQL_GAME_EVENTS_ID_START, " AND `eventEntry` <= ", Configuration.SQL_GAME_EVENTS_ID_END, ";");
        }

        public void AddRow(int eventID, int poolEntry, bool doSpawn)
        {
            SQLRow newRow = new SQLRow();
            if (doSpawn == true)
                newRow.AddInt("eventEntry", eventID);
            else
                newRow.AddInt("eventEntry", -1 * eventID); // Negative event IDs mean that the creature should despawn when the event starts
            newRow.AddInt("pool_entry", poolEntry);
            Rows.Add(newRow);
        }
    }
}
