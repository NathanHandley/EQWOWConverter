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

using EQWOWConverter.Common;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestZoneSafePointSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_zone_safe_point`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_zone_safe_point` ( ");
            stringBuilder.AppendLine("`MapID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`X` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Y` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Z` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Orientation` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY (`MapID`) USING BTREE); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int mapID, Vector3 position)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("MapID", mapID);
            newRow.AddFloat("X", position.X);
            newRow.AddFloat("Y", position.Y);
            newRow.AddFloat("Z", position.Z);
            newRow.AddFloat("Orientation", 0); // TBD if this will map to something, doesn't seem neccessary
            Rows.Add(newRow);
        }
    }
}
