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

            // Shaman
            AddRow(-51525, 0, 0, 0, 1, 2, 0, 0, 0);  // Static Shock (any damaging spell hit)

            // Death Knight (its own core script still requires blood runes to be on cooldown, so this only widens which casts are considered, not when the talent is allowed to fire)
            AddRow(-49182, 0, 0, 0, 0, 1, 0, 0, 0);  // Blade Barrier (on cast)

            // Warrior (its own core script still excludes the extra-attack spells it generates)
            AddRow(12328, 0, 0, 0, 1, 2, 0, 2, 0);   // Sweeping Strikes (any damaging spell hit)
        }
    }
}
