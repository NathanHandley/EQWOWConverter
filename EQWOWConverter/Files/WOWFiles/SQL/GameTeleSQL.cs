//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class GameTeleSQL : SQLFile
    {
        private static int CURRENT_ROWID = Configuration.CONFIG_SQL_GAMETELE_ROWID_START;

        public override string DeleteRowSQL()
        {
            // TODO: Make this have a high value
            return "DELETE FROM `game_tele` WHERE `id` >= " + Configuration.CONFIG_SQL_GAMETELE_ROWID_START + " AND `id` <= " + (Configuration.CONFIG_SQL_GAMETELE_ROWID_START + Rows.Count) + ";";
        }

        public void AddRow(int mapID, string name, float x, float y, float z)
        {
            int ID = CURRENT_ROWID;
            CURRENT_ROWID++;

            SQLRow newRow = new SQLRow();
            newRow.AddInt("id", ID);
            newRow.AddFloat("position_x", x);
            newRow.AddFloat("position_y", y);
            newRow.AddFloat("position_z", z);
            newRow.AddFloat("orientation", 0);
            newRow.AddInt("map", mapID);
            newRow.AddString("name", 100, name);
            Rows.Add(newRow);
        }
    }
}
