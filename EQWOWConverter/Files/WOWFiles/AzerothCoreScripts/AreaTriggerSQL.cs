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
    internal class AreaTriggerSQL
    {
        public class Row
        {
            public int Entry;
            public int MapID;
            public float PositionX;
            public float PositionY;
            public float PositionZ;
            public float BoxLength;
            public float BoxWidth;
            public float BoxHeight;
            public float BoxOrientation;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int areaTriggerID, int mapID, float positionX, float positionY, float positionZ,
            float boxLength, float boxWidth, float boxHeight, float boxOrientation)
        {
            Row newRow = new Row();
            newRow.Entry = areaTriggerID;
            newRow.MapID = mapID;
            newRow.PositionX = positionX;
            newRow.PositionY = positionY;
            newRow.PositionZ = positionZ;
            newRow.BoxLength = boxLength;
            newRow.BoxWidth = boxWidth;
            newRow.BoxHeight = boxHeight;
            newRow.BoxOrientation = boxOrientation;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "UpdateAreaTrigger.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM areatrigger WHERE `entry` >= " + Configuration.CONFIG_DBCID_AREATRIGGERID_START.ToString() + ";"); // TODO: Put a max on here
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `areatrigger` (`entry`, `map`, `x`, `y`, `z`, `radius`, `length`, `width`, `height`, `orientation`) VALUES (");
                stringBuilder.Append(row.Entry.ToString());
                stringBuilder.Append(", " + row.MapID.ToString());
                stringBuilder.Append(", " + row.PositionX.ToString());
                stringBuilder.Append(", " + row.PositionY.ToString());
                stringBuilder.Append(", " + row.PositionZ.ToString());
                stringBuilder.Append(", 0"); // Radius
                stringBuilder.Append(", " + row.BoxLength.ToString());
                stringBuilder.Append(", " + row.BoxWidth.ToString());
                stringBuilder.Append(", " + row.BoxHeight.ToString());
                stringBuilder.AppendLine(", " + row.BoxOrientation.ToString() + ");");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
