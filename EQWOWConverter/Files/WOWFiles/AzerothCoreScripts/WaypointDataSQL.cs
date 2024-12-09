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
    internal class WaypointDataSQL
    {
        private class Row
        {
            public int ID = 0;
            public int Point = 0;
            public float PositionX = 0;
            public float PositionY = 0;
            public float PositionZ = 0;
            public int DelayInMS = 0;
            public int MoveType = 0; // 0 = walk, 1 = run, 2 = fly
            public int Action = 0;
            public int ActionChance = 100;
            public int WPGuid = 0; // Do not use, let the core use it

            public string GenerateInsertValueBlock()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("(");
                stringBuilder.Append(ID.ToString() + ", ");
                stringBuilder.Append(Point.ToString() + ", ");
                stringBuilder.Append(PositionX.ToString() + ", ");
                stringBuilder.Append(PositionX.ToString() + ", ");
                stringBuilder.Append(PositionZ.ToString() + ", ");
                stringBuilder.Append("NULL , "); // Orientation
                stringBuilder.Append(DelayInMS.ToString() + ", ");
                stringBuilder.Append(MoveType.ToString() + ", ");
                stringBuilder.Append(Action.ToString() + ", ");
                stringBuilder.Append(ActionChance.ToString() + ", ");
                stringBuilder.Append(WPGuid.ToString());
                stringBuilder.AppendLine(")");
                return stringBuilder.ToString();
            }
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int id, int point, float positionX, float positionY, float positionZ, int delayInMS)
        {
            Row newRow = new Row();
            newRow.ID = id;
            newRow.Point = point;
            newRow.PositionX = positionX;
            newRow.PositionY = positionY;
            newRow.PositionZ = positionZ;
            newRow.DelayInMS = delayInMS;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);

            // Break rows into groups to avoid overloading it
            int rowsPerBatch = 50000;
            int batches = rows.Count / rowsPerBatch + 1;
            for (int i = 0; i < batches; i++)
            {
                string fullFilePath = Path.Combine(baseFolderPath, "WaypointData" + i + ".sql");
                StringBuilder stringBuilder = new StringBuilder();

                // On first batch, put the delete statement
                if (i == 0)
                {
                    int idLow = Configuration.CONFIG_SQL_CREATURE_GUID_LOW * 1000;
                    int idHigh = Configuration.CONFIG_SQL_CREATURE_GUID_HIGH * 1000;
                    stringBuilder.AppendLine("DELETE FROM waypoint_data WHERE `id` >= " + idLow.ToString() + " AND `id` <= " + idHigh + " ;");
                }

                // Calculate what rows to output
                int startRowIter = i * rowsPerBatch;
                int endRowIter = ((i + 1) * rowsPerBatch) - 1;
                if (endRowIter >= rows.Count)
                    endRowIter = rows.Count - 1;

                // Output the rows
                for (int rowIter = startRowIter; rowIter < endRowIter; ++rowIter)
                {
                    Row row = rows[rowIter];
                    stringBuilder.Append("INSERT INTO `waypoint_data` (`id`, `point`, `position_x`, `position_y`, `position_z`, `orientation`, `delay`, `move_type`, `action`, `action_chance`, `wpguid`) VALUES ");
                    stringBuilder.Append(row.GenerateInsertValueBlock());
                    stringBuilder.AppendLine(";");
                }

                // Output it
                File.WriteAllText(fullFilePath, stringBuilder.ToString());
            }
        }
    }
}
