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

using EQWOWConverter.Spells;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestPetSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_pet`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_pet` ( ");
            stringBuilder.AppendLine("`CreatingSpellID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`NamingType` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`CreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`SummonPropertiesID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY (`CreatingSpellID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int creatingSpellID, SpellPetNamingType petNamingType,
            int creatureTemplateID, int summonPropertiesID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("CreatingSpellID", creatingSpellID);
            newRow.AddInt("NamingType", (int)petNamingType);
            newRow.AddInt("CreatureTemplateID", creatureTemplateID);
            newRow.AddInt("SummonPropertiesID", summonPropertiesID);
            Rows.Add(newRow);
        }
    }
}
