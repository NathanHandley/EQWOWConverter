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

namespace EQWOWConverter.Files
{ 
    internal class PoolTemplateSQL
    {
        private class Row
        {
            public int EntryID;
            public int MaxLimit = 1;
            public string Description = string.Empty;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int entryID, string description)
        {
            if (description.Length > 255)
                description = description.Substring(0, 252) + "...";

            Row newRow = new Row();
            newRow.EntryID = entryID;
            newRow.Description = description;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "PoolTemplate.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM pool_template WHERE entry >= " + Configuration.CONFIG_SQL_POOL_TEMPLATE_ID_START.ToString() + " AND entry <= " + Configuration.CONFIG_SQL_POOL_TEMPLATE_ID_END.ToString() + " ;");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `pool_template` (`entry`, `max_limit`, `description`) VALUES (");
                stringBuilder.Append(row.EntryID.ToString() + ", ");
                stringBuilder.Append(row.MaxLimit.ToString() + ", ");
                stringBuilder.Append("'" + row.Description + "'");
                stringBuilder.AppendLine(");");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
