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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class GameObjectTemplateSQL
    {
        public class Row
        {
            public int EntryID;
            public GameObjectType Type = GameObjectType.GENERIC;
            public int DisplayID;
            public string Name = string.Empty; // 100 char max
            public string IconName = string.Empty; // 100 char max
            public string CastBarCaption = string.Empty; // 100 char max
            public string Unknown1 = string.Empty; // 100 char max
            public float Size = 1f;
            public int Data0 = 0;
            public int Data1 = 0;
            public int Data2 = 0;
            public int Data3 = 0;
            public int Data4 = 0;
            public int Data5 = 0;
            public int Data6 = 0;
            public int Data7 = 0;
            public int Data8 = 0;
            public int Data9 = 0;
            public int Data10 = 0;
            public int Data11 = 0;
            public int Data12 = 0;
            public int Data13 = 0;
            public int Data14 = 0;
            public int Data15 = 0;
            public int Data16 = 0;
            public int Data17 = 0;
            public int Data18 = 0;
            public int Data19 = 0;
            public int Data20 = 0;
            public int Data21 = 0;
            public int Data22 = 0;
            public int Data23 = 0;
            public string AIName = string.Empty; // 64 char max
            public string ScriptName = string.Empty; // 64 char max
            public int VerifiedBuild = 0;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int entryID, int displayID, string name)
        {
            if (name.Length > 100)
            {
                Logger.WriteError("Attempted to make a GameObjectTemplateSQL row with a name >100 characters named '" + name + "'");
                return;
            }

            Row newRow = new Row();
            newRow.EntryID = entryID;
            newRow.DisplayID = displayID;
            newRow.Name = name;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "GameObjectTemplateSQL.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM gameobject_template WHERE `entry` >= " + Configuration.CONFIG_DBCID_GAMEOBJECT_ID_START.ToString() + " AND `entry` <= " + Configuration.CONFIG_DBCID_GAMEOBJECT_ID_END + ";");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `gameobject_template` (`entry`, `type`, `displayId`, `name`, `IconName`, `castBarCaption`, `unk1`, `size`, `Data0`, `Data1`, `Data2`, `Data3`, `Data4`, `Data5`, `Data6`, `Data7`, `Data8`, `Data9`, `Data10`, `Data11`, `Data12`, `Data13`, `Data14`, `Data15`, `Data16`, `Data17`, `Data18`, `Data19`, `Data20`, `Data21`, `Data22`, `Data23`, `AIName`, `ScriptName`, `VerifiedBuild`) VALUES (");
                stringBuilder.Append(row.EntryID.ToString() + ", ");
                stringBuilder.Append(Convert.ToInt32(row.Type) + ", ");
                stringBuilder.Append(row.DisplayID.ToString() + ", ");
                stringBuilder.Append("'" + row.Name.ToString() + "', ");
                stringBuilder.Append("'" + row.IconName.ToString() + "', ");
                stringBuilder.Append("'" + row.CastBarCaption.ToString() + "', ");
                stringBuilder.Append("'" + row.Unknown1.ToString() + "', ");
                stringBuilder.Append(row.Size.ToString() + ", ");
                stringBuilder.Append(row.Data0.ToString() + ", ");
                stringBuilder.Append(row.Data1.ToString() + ", ");
                stringBuilder.Append(row.Data2.ToString() + ", ");
                stringBuilder.Append(row.Data3.ToString() + ", ");
                stringBuilder.Append(row.Data4.ToString() + ", ");
                stringBuilder.Append(row.Data5.ToString() + ", ");
                stringBuilder.Append(row.Data6.ToString() + ", ");
                stringBuilder.Append(row.Data7.ToString() + ", ");
                stringBuilder.Append(row.Data8.ToString() + ", ");
                stringBuilder.Append(row.Data9.ToString() + ", ");
                stringBuilder.Append(row.Data10.ToString() + ", ");
                stringBuilder.Append(row.Data11.ToString() + ", ");
                stringBuilder.Append(row.Data12.ToString() + ", ");
                stringBuilder.Append(row.Data13.ToString() + ", ");
                stringBuilder.Append(row.Data14.ToString() + ", ");
                stringBuilder.Append(row.Data15.ToString() + ", ");
                stringBuilder.Append(row.Data16.ToString() + ", ");
                stringBuilder.Append(row.Data17.ToString() + ", ");
                stringBuilder.Append(row.Data18.ToString() + ", ");
                stringBuilder.Append(row.Data19.ToString() + ", ");
                stringBuilder.Append(row.Data20.ToString() + ", ");
                stringBuilder.Append(row.Data21.ToString() + ", ");
                stringBuilder.Append(row.Data22.ToString() + ", ");
                stringBuilder.Append(row.Data23.ToString() + ", ");
                stringBuilder.Append("'" + row.AIName.ToString() + "', ");
                stringBuilder.Append("'" + row.ScriptName.ToString() + "', ");
                stringBuilder.AppendLine(row.VerifiedBuild.ToString() + ");");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
