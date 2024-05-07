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
            private static int CURRENT_ROWID = Configuration.CONFIG_GAMETELE_ROWID_START;

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
            stringBuilder.AppendLine("DELETE FROM `game_tele` WHERE `id` >= 2000 AND `id` <= 2200;");
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
