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

using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class InstanceTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM `instance_template` WHERE `map` >= " + Configuration.DBCID_MAP_ID_START + " AND `map` <= " + (ZoneProperties.CURRENT_MAPID) + ";";
        }

        public void AddRow(int mapID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("map", mapID);
            newRow.AddInt("parent", 0);
            newRow.AddString("script", 128, string.Empty);
            newRow.AddInt("allowMount", 1); // 0 no, 1 yes
            Rows.Add(newRow);
        }
    }
}
