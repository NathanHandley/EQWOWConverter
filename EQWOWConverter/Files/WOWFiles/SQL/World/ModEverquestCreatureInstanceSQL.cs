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

using EQWOWConverter.Creatures;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestCreatureInstanceSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_creature_instance`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_creature_instance` ( ");
            stringBuilder.AppendLine("`CreatureGUID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`WanderType` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PauseType` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`MapID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`WaypointID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`DoesRoam` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`RoamMinX` FLOAT NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`RoamMaxX` FLOAT NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`RoamMinY` FLOAT NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`RoamMaxY` FLOAT NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("PRIMARY KEY (`CreatureGUID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int creatureGUID, CreaturePathGridWanderType wanderType, int pauseType, int mapID, int waypointID, bool doesRoam,
            float roamMinX, float roamMaxX, float roamMinY, float roamMaxY)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("CreatureGUID", creatureGUID);
            newRow.AddInt("WanderType", (int)wanderType);
            newRow.AddInt("PauseType", pauseType);
            newRow.AddInt("MapID", mapID);
            newRow.AddInt("WaypointID", waypointID);
            newRow.AddInt("DoesRoam", doesRoam == true ? 1 : 0);
            newRow.AddFloat("RoamMinX", roamMinX);
            newRow.AddFloat("RoamMaxX", roamMaxX);
            newRow.AddFloat("RoamMinY", roamMinY);
            newRow.AddFloat("RoamMaxY", roamMaxY);
            Rows.Add(newRow);
        }
    }
}
