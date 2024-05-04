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
            public int AllowMountBool = 0; // 0 no, 1 yes
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
            stringBuilder.AppendLine("DELETE FROM `instance_template` WHERE `map` >= 750 AND `map` <= 900;");
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
