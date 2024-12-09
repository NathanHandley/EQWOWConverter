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
    internal class CreatureAddonSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM creature_addon WHERE `guid` >= " + Configuration.CONFIG_SQL_CREATURE_GUID_LOW.ToString() + " AND `guid` <= " + Configuration.CONFIG_SQL_CREATURE_GUID_HIGH + ";";
        }

        public void AddRow(int guid, int pathID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("guid", guid);
            newRow.AddInt("path_id", pathID);
            newRow.AddInt("mount", 0);
            newRow.AddInt("bytes1", 0);
            newRow.AddInt("bytes2", 0);
            newRow.AddInt("emote", 0);
            newRow.AddInt("visibilityDistanceType", 0);
            newRow.AddString("auras", null);
            Rows.Add(newRow);
        }
    }
}
