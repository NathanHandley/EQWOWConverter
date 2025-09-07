//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
    internal class ModEverquestSystemConfigsSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_systemconfigs`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_systemconfigs` ( ");
            stringBuilder.AppendLine("`Key` VARCHAR(100) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`Value` VARCHAR(100) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("PRIMARY KEY (`Key`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(string key, string value)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddString("Key", key);
            newRow.AddString("Value", value);
            Rows.Add(newRow);
        }
    }
}
