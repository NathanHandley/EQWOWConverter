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

using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureTemplateAddonSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder sb = new StringBuilder();
            if (Configuration.CONFIGONLY_CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true)
                sb.AppendLine("DELETE FROM creature_template_addon WHERE `entry` >= " + Configuration.CONFIGONLY_SQL_CREATURETEMPLATE_DEBUG_ENTRY_LOW.ToString() + " AND `entry` <= " + Configuration.CONFIGONLY_SQL_CREATURETEMPLATE_DEBUG_ENTRY_HIGH + ";");
            sb.Append("DELETE FROM creature_template_addon WHERE `entry` >= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `entry` <= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH + ";");
            return sb.ToString();
        }

        public void AddRow(int creatureTemplateID, string auras)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entry", creatureTemplateID);
            newRow.AddInt("path_id", 0);
            newRow.AddInt("mount", 0);
            newRow.AddInt("bytes1", 0);
            newRow.AddInt("bytes2", 1);
            newRow.AddInt("emote", 0);
            newRow.AddInt("visibilityDistanceType", 0);
            newRow.AddString("auras", auras);
            Rows.Add(newRow);
        }
    }
}
