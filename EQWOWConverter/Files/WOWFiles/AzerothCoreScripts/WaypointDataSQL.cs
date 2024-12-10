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
using System.Web;

namespace EQWOWConverter.WOWFiles
{
    internal class WaypointDataSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            int idLow = Configuration.CONFIG_SQL_CREATURE_GUID_LOW * 1000;
            int idHigh = Configuration.CONFIG_SQL_CREATURE_GUID_HIGH * 1000;
            return "DELETE FROM waypoint_data WHERE `id` >= " + idLow.ToString() + " AND `id` <= " + idHigh + ";";
        }

        public void AddRow(int id, int point, float positionX, float positionY, float positionZ, int delayInMS)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("id", id);
            newRow.AddInt("point", point);
            newRow.AddFloat("position_x", positionX);
            newRow.AddFloat("position_y", positionY);
            newRow.AddFloat("position_z", positionZ);
            newRow.AddFloat("orientation", null);
            newRow.AddInt("delay", delayInMS);
            newRow.AddInt("move_type", 0); // 0 = walk, 1 = run, 2 = fly
            newRow.AddInt("action", 0);
            newRow.AddInt("action_chance", 0);
            newRow.AddInt("wpguid", 0); // Do not use, let the core use it
            Rows.Add(newRow);
        }
    }
}
