﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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

using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal abstract class SQLFile
    {
        public class SQLRow
        {
            public class SQLColumn
            {
                public bool IsString = false;
                public bool IsNull = false;
                public string Name = string.Empty;
                public string Value = string.Empty;

                public string GetValueForManualOutput()
                {
                    if (IsNull == true)
                        return "NULL";
                    else if (IsString == true)
                        return "\"" + Value + "\"";
                    else
                        return Value;
                }
            }

            public void AddInt(string columnName, Int32? value)
            {
                SQLColumn newColumn = new SQLColumn();
                newColumn.Name = columnName;
                if (value.HasValue)
                    newColumn.Value = value.GetValueOrDefault().ToString();
                else
                    newColumn.IsNull = true;
                SQLColumns.Add(newColumn);
            }

            public void AddFloat(string columnName, float? value)
            {
                SQLColumn newColumn = new SQLColumn();
                newColumn.Name = columnName;
                if (value.HasValue)
                    newColumn.Value = value.GetValueOrDefault().ToString();
                else
                    newColumn.IsNull = true;
                SQLColumns.Add(newColumn);
            }

            public void AddString(string columnName, int maxLength, string? value)
            {
                SQLColumn newColumn = new SQLColumn();
                newColumn.IsString = true;
                newColumn.Name = columnName;
                if (value != null)
                {
                    if (maxLength > 3 && value.Length > maxLength)
                        value = value.Substring(0, maxLength-3) + "...";
                    newColumn.Value = value;
                }
                else
                    newColumn.IsNull = true;
                SQLColumns.Add(newColumn);
            }

            public void AddString(string columnName, string? value)
            {
                AddString(columnName, 65535, value);
            }

            public void AddDateTime(string columnName, DateTime? dateTime)
            {
                SQLColumn newColumn = new SQLColumn();
                newColumn.Name = columnName;
                if (dateTime != null)
                {
                    newColumn.Value = string.Concat("'", dateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), "'");
                }
                else
                    newColumn.IsNull = true;
                SQLColumns.Add(newColumn);
            }

            public string GetValuesStringInSQL()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("(");
                for (int i = 0; i < SQLColumns.Count; i++)
                {
                    stringBuilder.Append(SQLColumns[i].GetValueForManualOutput());
                    if (i < SQLColumns.Count - 1)
                        stringBuilder.Append(",");
                }
                stringBuilder.Append(")");
                return stringBuilder.ToString();
            }

            public List<SQLColumn> SQLColumns = new List<SQLColumn>();
        }

        protected List<SQLRow> Rows = new List<SQLRow>();

        public abstract string DeleteRowSQL();

        public void SaveToDisk(string tableName, SQLFileType fileType)
        {
            Logger.WriteDebug("Saving SQL Scripts for '" + tableName + "' started...");

            // Determine the path and create the folder if needed
            string outputFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "SQLScripts", fileType.ToString());
            FileTool.CreateBlankDirectory(outputFolder, true);

            // Generate the leading part of the SQL statement
            string fieldsSegment = GenerateFieldsSegment();

            // Break rows into groups to avoid making SQL files that are too big to import
            int rowsPerBatch = Configuration.GENERATE_SQL_FILE_BATCH_SIZE;
            int batches = Rows.Count / rowsPerBatch + 1;
            for (int i = 0; i < batches; i++)
            {
                string fullFilePath;
                if (i < 10)
                    fullFilePath = Path.Combine(outputFolder, tableName + "_0" + i.ToString() + ".sql");
                else
                    fullFilePath = Path.Combine(outputFolder, tableName + "_" + i.ToString() + ".sql");
                StringBuilder stringBuilder = new StringBuilder();

                // If configured to do so, put a unique ID
                if (Configuration.GENERATE_FORCE_SQL_UPDATES == true)
                {
                    string uniqueStamp = string.Concat("-- Unique Stamp: ", DateTime.Now.ToString("hh:mm:ss tt"));
                    stringBuilder.AppendLine(uniqueStamp);
                }

                // On first batch, put the delete statement
                if (i == 0)
                    stringBuilder.AppendLine(DeleteRowSQL());

                // Calculate what rows to output
                int startRowIter = i * rowsPerBatch;
                int endRowIter = ((i + 1) * rowsPerBatch) - 1;
                if (endRowIter > Rows.Count)
                    endRowIter = Rows.Count;

                // Output the rows
                int curInsertRowIter = 0;
                for (int rowIter = startRowIter; rowIter < endRowIter; ++rowIter)
                {
                    SQLRow row = Rows[rowIter];

                    // Rows are added as value blocks based on configuration, so add until a limit is hit and reset
                    // Start row insertions with the fields list
                    if (curInsertRowIter == 0)
                    {
                        stringBuilder.Append("INSERT INTO `" + tableName + "` ");
                        stringBuilder.Append(fieldsSegment);
                        stringBuilder.Append(" VALUES ");
                    }

                    // Grab the fields
                    stringBuilder.Append(row.GetValuesStringInSQL());

                    // Cap off the last rows, otherwise add a comma
                    if (curInsertRowIter == Configuration.GENERATE_SQL_FILE_INLINE_INSERT_ROWCOUNT_SIZE - 1 || rowIter == endRowIter - 1)
                        stringBuilder.AppendLine(";");
                    else
                        stringBuilder.Append(",");

                    // Add to the insert block iter, and cycle back if the end is hit
                    curInsertRowIter++;
                    if (curInsertRowIter == Configuration.GENERATE_SQL_FILE_INLINE_INSERT_ROWCOUNT_SIZE)
                        curInsertRowIter = 0;
                }

                // Output it
                File.WriteAllText(fullFilePath, stringBuilder.ToString());
            }

            Logger.WriteDebug("Saving SQL Scripts for '" + tableName + "' completed");
        }

        private string GenerateFieldsSegment()
        {
            if (Rows.Count == 0)
                return string.Empty;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            SQLRow firstRow = Rows[0];
            for (int i = 0; i < firstRow.SQLColumns.Count; i++)
            {
                stringBuilder.Append("`");
                stringBuilder.Append(Rows[0].SQLColumns[i].Name);
                stringBuilder.Append("`");
                if (i < firstRow.SQLColumns.Count - 1)
                    stringBuilder.Append(",");
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }
    }
}
