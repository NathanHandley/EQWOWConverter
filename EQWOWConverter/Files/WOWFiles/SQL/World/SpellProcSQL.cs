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

namespace EQWOWConverter.WOWFiles
{
    internal class SpellProcSQL : SQLFile
    {
        private List<int> OverriddenSpellIDs = new List<int>();

        public override string DeleteRowSQL()
        {
            if (OverriddenSpellIDs.Count == 0)
                return string.Empty;
            List<string> spellIDStrings = new List<string>();
            foreach (int overriddenSpellID in OverriddenSpellIDs)
                spellIDStrings.Add(overriddenSpellID.ToString());
            return string.Concat("DELETE FROM spell_proc WHERE `SpellId` IN (", string.Join(",", spellIDStrings), ");");
        }

        public void AddRow(int spellID, int schoolMask, int spellFamilyName, int procFlags, int spellTypeMask, int spellPhaseMask, int hitMask, int attributesMask, int cooldown)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("SpellId", spellID);
            newRow.AddInt("SchoolMask", schoolMask);
            newRow.AddInt("SpellFamilyName", spellFamilyName);
            newRow.AddInt("SpellFamilyMask0", 0);
            newRow.AddInt("SpellFamilyMask1", 0);
            newRow.AddInt("SpellFamilyMask2", 0);
            newRow.AddInt("ProcFlags", procFlags);
            newRow.AddInt("SpellTypeMask", spellTypeMask);
            newRow.AddInt("SpellPhaseMask", spellPhaseMask);
            newRow.AddInt("HitMask", hitMask);
            newRow.AddInt("AttributesMask", attributesMask);
            newRow.AddInt("DisableEffectsMask", 0);
            newRow.AddFloat("ProcsPerMinute", 0);
            newRow.AddFloat("Chance", 0);
            newRow.AddInt("Cooldown", cooldown);
            newRow.AddInt("Charges", 0);
            Rows.Add(newRow);
            OverriddenSpellIDs.Add(spellID);
        }

        public void AddFamilyWideProcOverrideRows()
        {
            if (Configuration.SPELL_WOW_TALENT_INTERACTION_ENABLED == false)
                return;

            // Rewrites the proc conditions of every talent that procs off "any spell of my class" so the family check is dropped, leaving only the school and hit
            // conditions.  Values are the stock AzerothCore spell_proc rows with SpellFamilyName set to zero and nothing else touched.  Only entries whose SpellFamilyMask
            // was already zero appear here (a masked entry is keyed to particular spells), and clearing the family there would drop the mask filter with it and make the
            // talent proc off everything.  Negative spell id covers every rank in the talent's chain

            // Mage
            AddRow(-11119, 4, 0, 0, 1, 2, 2, 0, 0);  // Ignite (fire damage crit)
            AddRow(-11213, 0, 0, 0, 1, 2, 0, 0, 0);  // Arcane Concentration (any damaging spell hit)
            AddRow(-11180, 16, 0, 0, 0, 2, 3, 0, 0); // Winter's Chill (frost damage hit or crit)
            AddRow(-29074, 20, 0, 0, 0, 2, 2, 0, 8); // Master of Elements (fire or frost crit)
            AddRow(-11095, 4, 0, 0, 0, 2, 0, 0, 0);  // Improved Scorch (fire damage hit applies the crit-taken debuff)
            AddRow(-44557, 16, 0, 0, 0, 2, 0, 0, 6000);   // Enduring Winter (frost damage hit grants Replenishment)
            AddRow(-44449, 0, 0, 0, 0, 2, 2, 0, 8);  // Burnout (any spell critical costs extra mana)
            AddRow(-44445, 4, 0, 0, 1, 2, 0, 0, 0);  // Hot Streak (fire damage; the mod's replacement script filters the spells)
            AddRow(-44546, 16, 0, 69632, 0, 2, 0, 0, 0);  // Brain Freeze (frost damage; the mod's replacement script filters the spells)

            // REQ_SPELLMOD attribute (8) means the charge only drops when the buff's modifier actually applied to the cast
            AddRow(12043, 0, 0, 0, 7, 1, 0, 8, 0);   // Presence of Mind
            AddRow(12536, 0, 0, 0, 0, 1, 0, 12, 0);  // Clearcasting (Arcane Concentration's buff)
            AddRow(57529, 0, 0, 0, 0, 1, 0, 8, 0);   // Arcane Potency crit buff (rank 1)
            AddRow(57531, 0, 0, 0, 0, 1, 0, 8, 0);   // Arcane Potency crit buff (rank 2)
            AddRow(57761, 4, 0, 65536, 1, 1, 0, 8, 0);    // Fireball! (Brain Freeze's instant+free buff)
            AddRow(74396, 126, 0, 65536, 0, 3, 0, 2, 0);  // Fingers of Frost charge tracker
            AddRow(11129, 4, 0, 0, 1, 2, 0, 0, 0);   // Combustion (fire damage hits count stacks and crits; charges come from the spell data)

            // Priest
            AddRow(-14892, 0, 0, 0, 2, 2, 2, 2, 0);  // Inspiration (any direct critical heal)
            AddRow(-34753, 0, 0, 0, 2, 2, 2, 2, 1);  // Holy Concentration (critical heals; the mod's added script filters the spells)
            AddRow(-47516, 0, 0, 0, 0, 2, 0, 2, 0);  // Grace (heal cast; the mod's added script filters the spells)
            AddRow(-15337, 0, 0, 0, 1, 2, 2, 2, 0);  // Improved Spirit Tap (damage criticals; the mod's replacement script filters the spells)

            // Shaman
            AddRow(-51525, 0, 0, 0, 1, 2, 0, 0, 0);  // Static Shock (any damaging spell hit)
            AddRow(16166, 0, 0, 0, 7, 1, 0, 8, 0);   // Elemental Mastery charge consumption
            AddRow(16246, 0, 0, 0, 0, 1, 0, 12, 0);  // Elemental Focus Clearcasting charge consumption

            // Druid
            AddRow(-16880, 0, 0, 0, 0, 3, 2, 0, 0);  // Nature's Grace (any spell critical; the haste buff itself is school-wide already)
            AddRow(16870, 0, 0, 0, 0, 1, 0, 12, 0);  // Omen of Clarity Clearcasting charge consumption
            AddRow(17116, 0, 0, 0, 7, 1, 0, 8, 0);   // Nature's Swiftness charge consumption

            // Death Knight
            AddRow(49796, 16, 0, 0, 0, 4, 0, 8, 0);  // Deathchill charge consumption (frost cast that used the crit modifier)
            // (DK core script requires blood runes to be on cooldown, so this only widens which casts are considered, not when the talent is allowed to fire)
            AddRow(-49182, 0, 0, 0, 0, 1, 0, 0, 0);  // Blade Barrier (on cast)

            // Warlock
            AddRow(-18094, 32, 0, 0, 1, 2, 0, 0, 6000);   // Nightfall (shadow damage; the mod's added script filters the spells)
            AddRow(-32385, 0, 0, 0, 1, 2, 0, 0, 0);      // Shadow Embrace (damage hit; the mod's added script filters the spells)
            AddRow(-47195, 0, 0, 0, 1, 2, 0, 0, 0);      // Eradication (periodic damage tick; the mod's added script filters the spells)
            AddRow(18708, 0, 0, 0, 0, 1, 0, 8, 0);       // Fel Domination charge consumption (the summon cast that used the modifiers)
            AddRow(17941, 104, 0, 65536, 1, 1, 0, 8, 0);  // Shadow Trance (Nightfall's instant Shadow Bolt buff; shadow, arcane and nature cover the EQ direct spells it reaches)
            AddRow(34936, 108, 0, 65536, 1, 1, 0, 8, 0);  // Backlash's instant cast buff (fire and shadow for WOW, plus arcane and nature for the EQ direct spells it reaches)

            // Warrior (its own core script still excludes the extra-attack spells it generates)
            AddRow(12328, 0, 0, 0, 1, 2, 0, 2, 0);   // Sweeping Strikes (any damaging spell hit) - Example, Bash wouldn't work on other targets without this
        }
    }
}
