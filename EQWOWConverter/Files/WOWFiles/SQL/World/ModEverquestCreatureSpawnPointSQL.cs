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
    internal class ModEverquestCreatureSpawnPointSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_creature_spawn_point`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_creature_spawn_point` ( ");
            stringBuilder.AppendLine("`CreatureGUID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`MapID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`SpawnPointID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`SpawnGroupID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`SpawnGroupLimit` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY (`CreatureGUID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int creatureGUID, int mapID, int spawnPointID, int spawnGroupID, int spawnGroupLimit)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("CreatureGUID", creatureGUID);
            newRow.AddInt("MapID", mapID);
            newRow.AddInt("SpawnPointID", spawnPointID);
            newRow.AddInt("SpawnGroupID", spawnGroupID);
            newRow.AddInt("SpawnGroupLimit", spawnGroupLimit);
            Rows.Add(newRow);
        }
    }
}
