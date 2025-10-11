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
    internal class ModEverquestSpellSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_spell`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_spell` ( ");
            stringBuilder.AppendLine("`SpellID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AuraDurationBaseInMS` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AuraDurationAddPerLevelInMS` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AuraDurationMaxInMS` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AuraDurationCalcMinLevel` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AuraDurationCalcMaxLevel` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`RecourseSpellID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`SpellIDCastOnMeleeAttacker` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`FocusBoostType` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PeriodicAuraSpellID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PeriodicAuraSpellRadius` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`MaleFormSpellID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`FemaleFormSpellID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY (`SpellID`) USING BTREE ); ");            
            return stringBuilder.ToString();
        }

        public void AddRow(SpellTemplate spellTemplate, int spellID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("SpellID", spellID);
            newRow.AddInt("AuraDurationBaseInMS", spellTemplate.AuraDuration.BaseDurationInMS);
            newRow.AddInt("AuraDurationAddPerLevelInMS", spellTemplate.AuraDuration.DurationInMSPerLevel);
            newRow.AddInt("AuraDurationMaxInMS", spellTemplate.AuraDuration.MaxDurationInMS);
            newRow.AddInt("AuraDurationCalcMinLevel", spellTemplate.AuraDuration.MinLevel);
            newRow.AddInt("AuraDurationCalcMaxLevel", spellTemplate.AuraDuration.MaxLevel);
            if (spellTemplate.RecourseLinkSpellTemplate != null)
                newRow.AddInt("RecourseSpellID", spellTemplate.RecourseLinkSpellTemplate.WOWSpellID);
            else
                newRow.AddInt("RecourseSpellID", 0);
            newRow.AddInt("SpellIDCastOnMeleeAttacker", spellTemplate.WOWSpellIDCastOnMeleeAttacker);
            newRow.AddInt("FocusBoostType", (int)spellTemplate.FocusBoostType);
            newRow.AddInt("PeriodicAuraSpellID", spellTemplate.PeriodicAuraWOWSpellID);
            newRow.AddInt("PeriodicAuraSpellRadius", spellTemplate.PeriodicAuraSpellRadius);
            newRow.AddInt("MaleFormSpellID", spellTemplate.MaleFormSpellTemplateID);
            newRow.AddInt("FemaleFormSpellID", spellTemplate.FemaleFormSpellTemplateID);
            Rows.Add(newRow);
        }
    }
}
