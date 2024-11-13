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

using EQWOWConverter.WOWFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureModelInfoSQL
    {
        public class Row
        {
            public int DisplayID; // Reference to CreatureDisplayInfo.dbc
            public float BoundingRadius = 0; // Not currently used (?)
            public float CombatReach = 1.5f; // Lots of 0 as well, consider changing
            public int Gender = 2; // 0: Male, 1: Female, 2: None/Neutral
            public int DisplayIDOtherGender = 0; // Record that relates to the 'other' gender ID (if male, this is the female row...)
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int displayID, int gender)
        {
            Row newRow = new Row();
            newRow.DisplayID = displayID;
            newRow.Gender = gender;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "CreatureModelInfo.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM creature_model_info WHERE `DisplayID` >= " + Configuration.CONFIG_DBCID_CREATUREDISPLAYINFO_ID_START.ToString() + " AND `DisplayID` <= " + Configuration.CONFIG_DBCID_CREATUREDISPLAYINFO_ID_END.ToString() + " ;");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `creature_model_info` (`DisplayID`, `BoundingRadius`, `CombatReach`, `Gender`, `DisplayID_Other_Gender`) VALUES (");
                stringBuilder.Append(row.DisplayID.ToString());
                stringBuilder.Append(", " + row.BoundingRadius.ToString());
                stringBuilder.Append(", " + row.CombatReach.ToString());
                stringBuilder.Append(", " + row.Gender.ToString());
                stringBuilder.Append(", " + row.DisplayIDOtherGender.ToString());
                stringBuilder.AppendLine(");");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
