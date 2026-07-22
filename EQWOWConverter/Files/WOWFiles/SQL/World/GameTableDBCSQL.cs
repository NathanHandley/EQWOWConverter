//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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
    // Azerothcore reads these "gt" (game table) datas from the database, not the dbc files
    internal class GameTableDBCSQL : SQLFile
    {
        private string TableName;
        private List<int> RowIDs = new List<int>();

        public GameTableDBCSQL(string tableName)
        {
            TableName = tableName;
        }

        public override string DeleteRowSQL()
        {
            // Only delete rows that are being updated
            if (RowIDs.Count == 0)
                return string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("DELETE FROM `");
            stringBuilder.Append(TableName);
            stringBuilder.Append("` WHERE `ID` IN (");
            for (int i = 0; i < RowIDs.Count; i++)
            {
                stringBuilder.Append(RowIDs[i]);
                if (i < RowIDs.Count - 1)
                    stringBuilder.Append(",");
            }
            stringBuilder.Append(");");
            return stringBuilder.ToString();
        }

        public void AddRow(int id, float data)
        {
            RowIDs.Add(id);
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", id);
            newRow.AddFloat("Data", data);
            Rows.Add(newRow);
        }
    }
}
