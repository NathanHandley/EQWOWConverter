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

using EQWOWConverter.Creatures;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using System.Net.NetworkInformation;
using System;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestCharacterHomebindSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_character_homebind` (");
            stringBuilder.AppendLine("`guid` INT(10) UNSIGNED NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`mapId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`zoneId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`posX` FLOAT NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`posY` FLOAT NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`posZ` FLOAT NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("PRIMARY KEY(`guid`) USING BTREE);");
            return stringBuilder.ToString();
        }

        public void AddRow()
        {
            //SQLRow newRow = new SQLRow();
            //newRow.AddInt("guid", 0);
            //newRow.AddInt("mapId", 0);
            //newRow.AddInt("zoneId", 0);
            //newRow.AddFloat("posX", 0);
            //newRow.AddFloat("posY", 0);
            //newRow.AddFloat("posZ", 0);
            //Rows.Add(newRow);
        }
    }
}
