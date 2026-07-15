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
    internal class ModEverquestCreatureMovementSoundSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_creature_movement_sound`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_creature_movement_sound` ( ");
            stringBuilder.AppendLine("`DisplayID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`WalkSoundEntryIDs` VARCHAR(128) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`WalkSoundDurationsMS` VARCHAR(128) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`RunSoundEntryIDs` VARCHAR(128) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`RunSoundDurationsMS` VARCHAR(128) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`MaxHearingDistance` FLOAT NOT NULL DEFAULT '20', ");
            stringBuilder.AppendLine("PRIMARY KEY (`DisplayID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int displayID, string walkSoundEntryIDs, string walkSoundDurationsMS, string runSoundEntryIDs, string runSoundDurationsMS, float maxHearingDistance)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("DisplayID", displayID);
            newRow.AddString("WalkSoundEntryIDs", 128, walkSoundEntryIDs);
            newRow.AddString("WalkSoundDurationsMS", 128, walkSoundDurationsMS);
            newRow.AddString("RunSoundEntryIDs", 128, runSoundEntryIDs);
            newRow.AddString("RunSoundDurationsMS", 128, runSoundDurationsMS);
            newRow.AddFloat("MaxHearingDistance", maxHearingDistance);
            Rows.Add(newRow);
        }
    }
}
