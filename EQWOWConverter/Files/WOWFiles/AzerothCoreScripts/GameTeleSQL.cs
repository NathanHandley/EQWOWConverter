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
    internal class GameTeleSQL
    {
        public class Row
        {
            private static int CURRENT_ROWID = Configuration.CONFIG_SQL_GAMETELE_ROWID_START;

            public int ID;
            public float XPosition = 10;
            public float YPosition = 10;
            public float ZPosition = 10;
            public float Orientation = 0;
            public int MapID;
            public string Name = string.Empty;

            public Row()
            {
                ID = CURRENT_ROWID;
                CURRENT_ROWID++;
            }
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int mapID, string name, float x, float y, float z)
        {
            Row newRow = new Row();
            newRow.MapID = mapID;
            newRow.Name = name;
            newRow.XPosition = x;
            newRow.YPosition = y;
            newRow.ZPosition = z;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "UpdateGameTele.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM `game_tele` WHERE `id` >= " + Configuration.CONFIG_SQL_GAMETELE_ROWID_START + " AND `id` <= " + (Configuration.CONFIG_SQL_GAMETELE_ROWID_START + rows.Count) + ";");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `game_tele` (`id`, `position_x`, `position_y`, `position_z`, `orientation`, `map`, `name`) VALUES (");
                stringBuilder.Append(row.ID.ToString());
                stringBuilder.Append(", " + row.XPosition.ToString());
                stringBuilder.Append(", " + row.YPosition.ToString());
                stringBuilder.Append(", " + row.ZPosition.ToString());
                stringBuilder.Append(", " + row.Orientation.ToString());
                stringBuilder.Append(", " + row.MapID.ToString());
                stringBuilder.AppendLine(", '" + row.Name + "');");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
