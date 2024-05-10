using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles.DBC
{
    internal class LoadingScreensDBC
    {
        public class Row
        {
            public int Id;
            public string Name = string.Empty;
            public string FileName = string.Empty;
            public int HasWideScreen = 0;
        }

        private List<Row> rows = new List<Row>();

        public void AddRow(int id, string name, string fileName)
        {
            Row newRow = new Row();
            newRow.Id = id;
            newRow.Name = name;
            newRow.FileName = fileName;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "LoadingScreensDBC.csv");

            // Add each row of data
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Row row in rows)
            {
                stringBuilder.Append("\"" + row.Id.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Name + "\"");
                stringBuilder.Append(",\"" + row.FileName + "\"");
                stringBuilder.AppendLine(",\"" + row.HasWideScreen.ToString() + "\"");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
