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

using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class SmartScriptsSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM smart_scripts WHERE COMMENT LIKE 'EQ%';";
        }

        public void AddRow()
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entryorguid", 0);
            newRow.AddInt("source_type", 0);
            newRow.AddInt("id", 0);
            newRow.AddInt("link", 0);
            newRow.AddInt("event_type", 0);
            newRow.AddInt("event_phase_mask", 0);
            newRow.AddInt("event_chance", 0);
            newRow.AddInt("event_flags", 0);
            newRow.AddInt("event_param1", 0);
            newRow.AddInt("event_param2", 0);
            newRow.AddInt("event_param3", 0);
            newRow.AddInt("event_param4", 0);
            newRow.AddInt("event_param5", 0);
            newRow.AddInt("event_param6", 0);
            newRow.AddInt("action_type", 0);
            newRow.AddInt("action_param1", 0);
            newRow.AddInt("action_param2", 0);
            newRow.AddInt("action_param3", 0);
            newRow.AddInt("action_param4", 0);
            newRow.AddInt("action_param5", 0);
            newRow.AddInt("action_param6", 0);
            newRow.AddInt("target_type", 0);
            newRow.AddInt("target_param1", 0);
            newRow.AddInt("target_param2", 0);
            newRow.AddInt("target_param3", 0);
            newRow.AddInt("target_param4", 0);
            newRow.AddFloat("target_x", 0);
            newRow.AddFloat("target_y", 0);
            newRow.AddFloat("target_z", 0);
            newRow.AddFloat("target_o", 0);
            newRow.AddString("comment", string.Empty);
            Rows.Add(newRow);
        }
    }
}
