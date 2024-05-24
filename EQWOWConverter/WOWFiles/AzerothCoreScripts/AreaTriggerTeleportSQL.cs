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
    internal class AreaTriggerTeleportSQL
    {
        public class Row
        {
            public int AreaTriggerDBCID;
            public string DescriptiveName = string.Empty;
            public int TargetMapID;
            public float TargetPositionX;
            public float TargetPositionY;
            public float TargetPositionZ;
            public float TargetOrientation;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int areaTriggerDBCID, string descriptiveName, int targetMapID, float targetPositionX, float targetPositionY, float targetPositionZ, float targetOrientation)
        {
            Row row = new Row();
            row.AreaTriggerDBCID = areaTriggerDBCID;
            row.DescriptiveName = descriptiveName;
            row.TargetMapID = targetMapID;
            row.TargetPositionX = targetPositionX;
            row.TargetPositionY = targetPositionY;
            row.TargetPositionZ = targetPositionZ;
            row.TargetOrientation = targetOrientation;
            rows.Add(row);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "UpdateAreaTrigger_Teleport.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM areatrigger_teleport WHERE `Name` LIKE 'EQ %';");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `areatrigger_teleport` (`ID`, `Name`, `target_map`, `target_position_x`, `target_position_y`, `target_position_z`, `target_orientation`) VALUES (");
                stringBuilder.Append(row.AreaTriggerDBCID.ToString());
                stringBuilder.Append(", '" + row.DescriptiveName + "'");
                stringBuilder.Append(", " + row.TargetMapID.ToString());
                stringBuilder.Append(", " + row.TargetPositionX.ToString());
                stringBuilder.Append(", " + row.TargetPositionY.ToString());
                stringBuilder.Append(", " + row.TargetPositionZ.ToString());
                stringBuilder.AppendLine(", " + row.TargetOrientation.ToString() + ");");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
