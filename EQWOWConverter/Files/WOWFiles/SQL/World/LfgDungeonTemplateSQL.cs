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
    internal class LfgDungeonTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return string.Concat("DELETE FROM `lfg_dungeon_template` WHERE `dungeonId` >= ", Configuration.DBCID_LFGDUNGEONS_ID_START, ";");
        }

        public void AddRow(int dungeonID, string name, float positionX, float positionY, float positionZ, float orientation)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("dungeonId", dungeonID);
            newRow.AddString("name", 255, name);
            newRow.AddFloat("position_x", positionX);
            newRow.AddFloat("position_y", positionY);
            newRow.AddFloat("position_z", positionZ);
            newRow.AddFloat("orientation", orientation);
            Rows.Add(newRow);
        }
    }
}
