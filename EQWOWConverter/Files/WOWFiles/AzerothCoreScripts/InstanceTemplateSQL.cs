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

using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class InstanceTemplateSQL
    {
        public class Row
        {
            public int MapID;
            public string ScriptName = string.Empty;
            public int AllowMountBool = 1; // 0 no, 1 yes
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int mapID)
        {
            Row newRow = new Row();
            newRow.MapID = mapID;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "UpdateInstanceTemplate.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM `instance_template` WHERE `map` >= " + Configuration.CONFIG_DBCID_MAP_ID_START + " AND `map` <= " + (ZoneProperties.CURRENT_MAPID) + ";");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `instance_template` (`map`, `parent`, `script`, `allowMount`) VALUES (");
                stringBuilder.Append(row.MapID.ToString());
                stringBuilder.Append(", 0"); // Parent
                stringBuilder.Append(", '" + row.ScriptName + "'");
                stringBuilder.AppendLine(", '" + row.AllowMountBool + "');");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
