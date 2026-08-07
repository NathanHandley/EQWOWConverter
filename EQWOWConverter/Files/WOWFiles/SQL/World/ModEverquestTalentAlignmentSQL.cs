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
    internal class ModEverquestTalentAlignmentSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_talent_alignment`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_talent_alignment` ( ");
            stringBuilder.AppendLine("`SpellID` INT(10) UNSIGNED NOT NULL DEFAULT '0', "); // Talent rank 1 spell ID or a non-talent (proc buff) spell ID
            stringBuilder.AppendLine("`EffectIndex` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`WOWClassName` VARCHAR(24) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`SpellName` VARCHAR(64) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`SchoolMask` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AffectsDamage` TINYINT(1) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AffectsHealing` TINYINT(1) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`SpellRestriction` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0', "); // 0 = none, 1 = direct damage, 2 = area damage, 3 = point blank area damage
            stringBuilder.AppendLine("`MaxBaseCastTimeMS` INT(10) UNSIGNED NOT NULL DEFAULT '0', "); // If > 0, only EverQuest spells with a base cast time under this are affected
            stringBuilder.AppendLine("PRIMARY KEY (`SpellID`, `EffectIndex`, `SchoolMask`, `SpellRestriction`) USING BTREE); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int spellID, int effectIndex, string wowClassName, string spellName, int schoolMask, bool affectsDamage,
            bool affectsHealing, int spellRestriction, int maxBaseCastTimeInMS)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("SpellID", spellID);
            newRow.AddInt("EffectIndex", effectIndex);
            newRow.AddString("WOWClassName", 24, wowClassName);
            newRow.AddString("SpellName", 64, spellName);
            newRow.AddInt("SchoolMask", schoolMask);
            newRow.AddInt("AffectsDamage", affectsDamage == true ? 1 : 0);
            newRow.AddInt("AffectsHealing", affectsHealing == true ? 1 : 0);
            newRow.AddInt("SpellRestriction", spellRestriction);
            newRow.AddInt("MaxBaseCastTimeMS", maxBaseCastTimeInMS);
            Rows.Add(newRow);
        }
    }
}
