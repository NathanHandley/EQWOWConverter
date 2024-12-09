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
    internal class CreatureAddonSQL
    {
        private class Row
        {
            public int GUID = 0;
            public int PathID = 0;
            public int Mount = 0;
            public int Bytes1 = 0;
            public int Bytes2 = 0;
            public int Emote = 0;
            public int AIAnimKit = 0;
            public int MovementAnimKit = 0;
            public int MeleeAnimKit = 0;
            public int VisibilityDistanceType = 0;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int guid, int pathID)
        {
            Row newRow = new Row();
            newRow.GUID = guid;
            newRow.PathID = pathID;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "CreatureAddon.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM creature_addon WHERE `guid` >= " + Configuration.CONFIG_SQL_CREATURE_GUID_LOW.ToString() + " AND `guid` <= " + Configuration.CONFIG_SQL_CREATURE_GUID_HIGH + " ;");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `creature_addon` (`guid`, `path_id`, `mount`, `bytes1`, `bytes2`, `emote`, `visibilityDistanceType`, `auras`) VALUES (");
                stringBuilder.Append(row.GUID.ToString() + ", ");
                stringBuilder.Append(row.PathID.ToString() + ", ");
                stringBuilder.Append(row.Mount.ToString() + ", ");
                stringBuilder.Append(row.Bytes1.ToString() + ", ");
                stringBuilder.Append(row.Bytes2.ToString() + ", ");
                stringBuilder.Append(row.Emote.ToString() + ", ");
                stringBuilder.Append(row.VisibilityDistanceType.ToString() + ", ");
                stringBuilder.Append("NULL"); // Auras
                stringBuilder.AppendLine(");");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
