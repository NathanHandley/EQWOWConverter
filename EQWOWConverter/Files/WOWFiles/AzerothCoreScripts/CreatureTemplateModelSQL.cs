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
    internal class CreatureTemplateModelSQL
    {
        public class Row
        {
            public int CreatureID;
            public int Idx = 0;
            public int CreatureDisplayID;
            public int DisplayScale = 1;
            public int Probability = 1;
            public int VerifiedBuild = 1;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int creatureID, int creatureDisplayID)
        {
            Row newRow = new Row();
            newRow.CreatureID = creatureID;
            newRow.CreatureDisplayID = creatureDisplayID;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "CreatureTemplateModel.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM creature_template_model WHERE `CreatureID` >= " + Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `CreatureID` <= " + Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_HIGH + " ;");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `creature_template_model` (`CreatureID`, `Idx`, `CreatureDisplayID`, `DisplayScale`, `Probability`, `VerifiedBuild`) VALUES (190010, 0, 19646, 1, 1, 0);");
                stringBuilder.Append(row.CreatureID.ToString() + ", ");
                stringBuilder.Append(row.Idx.ToString() + ", ");
                stringBuilder.Append(row.CreatureDisplayID.ToString() + ", ");
                stringBuilder.Append(row.DisplayScale.ToString() + ", ");
                stringBuilder.Append(row.Probability.ToString() + ", ");
                stringBuilder.AppendLine(row.VerifiedBuild.ToString() + ");");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
