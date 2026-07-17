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
    internal class ModEverquestCreatureSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_creature`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_creature` ( ");
            stringBuilder.AppendLine("`CreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`CanShowHeldLootItems` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`CanShowHeldLootShields` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`SpawnLimit` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`RangedAttackEnabled` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`RangedAttackMinRange` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`RangedAttackMaxRange` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`RangedAttackDamageModPct` INT(11) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AgroSocialDistanceMod` FLOAT NOT NULL DEFAULT '1', ");
            stringBuilder.AppendLine("`EnrageEnabled` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`EnrageHPPct` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`EnrageDurationInMS` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`EnrageCooldownInMS` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`FlurryEnabled` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`FlurryChancePct` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`RampageEnabled` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`RampageChancePct` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`RampageRange` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`RampageDamagePct` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`WildRampageEnabled` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`WildRampageChancePct` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`WildRampageMaxTargets` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`WildRampageDamagePct` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AttackRoundTimeInMS` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY (`CreatureTemplateID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int creatureTemplateID, bool canShowHeldLootItems, bool canShowHeldLootShields, int spawnLimit,
            bool rangedAttackEnabled, int rangedAttackMinRange, int rangedAttackMaxRange, int rangedAttackDamageModPct,
            float agroSocialDistanceMod, bool enrageEnabled, int enrageHPPct, int enrageDurationInMS, int enrageCooldownInMS,
            bool flurryEnabled, int flurryChancePct, bool rampageEnabled, int rampageChancePct, int rampageRange, int rampageDamagePct,
            bool wildRampageEnabled, int wildRampageChancePct, int wildRampageMaxTargets, int wildRampageDamagePct, int attackRoundTimeInMS)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("CreatureTemplateID", creatureTemplateID);
            newRow.AddInt("CanShowHeldLootItems", canShowHeldLootItems == true ? 1 : 0);
            newRow.AddInt("CanShowHeldLootShields", canShowHeldLootShields == true ? 1 : 0);
            newRow.AddInt("SpawnLimit", spawnLimit);
            newRow.AddInt("RangedAttackEnabled", rangedAttackEnabled == true ? 1 : 0);
            newRow.AddInt("RangedAttackMinRange", rangedAttackMinRange);
            newRow.AddInt("RangedAttackMaxRange", rangedAttackMaxRange);
            newRow.AddInt("RangedAttackDamageModPct", rangedAttackDamageModPct);
            newRow.AddFloat("AgroSocialDistanceMod", agroSocialDistanceMod);
            newRow.AddInt("EnrageEnabled", enrageEnabled == true ? 1 : 0);
            newRow.AddInt("EnrageHPPct", enrageHPPct);
            newRow.AddInt("EnrageDurationInMS", enrageDurationInMS);
            newRow.AddInt("EnrageCooldownInMS", enrageCooldownInMS);
            newRow.AddInt("FlurryEnabled", flurryEnabled == true ? 1 : 0);
            newRow.AddInt("FlurryChancePct", flurryChancePct);
            newRow.AddInt("RampageEnabled", rampageEnabled == true ? 1 : 0);
            newRow.AddInt("RampageChancePct", rampageChancePct);
            newRow.AddInt("RampageRange", rampageRange);
            newRow.AddInt("RampageDamagePct", rampageDamagePct);
            newRow.AddInt("WildRampageEnabled", wildRampageEnabled == true ? 1 : 0);
            newRow.AddInt("WildRampageChancePct", wildRampageChancePct);
            newRow.AddInt("WildRampageMaxTargets", wildRampageMaxTargets);
            newRow.AddInt("WildRampageDamagePct", wildRampageDamagePct);
            newRow.AddInt("AttackRoundTimeInMS", attackRoundTimeInMS);
            Rows.Add(newRow);
        }
    }
}
