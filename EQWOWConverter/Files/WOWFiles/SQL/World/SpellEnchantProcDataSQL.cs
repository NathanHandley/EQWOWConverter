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
    internal class SpellEnchantProcDataSQL : SQLFile
    {
        private List<int> GeneratedEnchantIDs = new List<int>();

        public override string DeleteRowSQL()
        {
            // Only clear the generated rows, since the stock rogue poison and weapon enchant rows still need to work
            if (GeneratedEnchantIDs.Count == 0)
                return string.Empty;
            List<string> enchantIDStrings = new List<string>();
            foreach (int generatedEnchantID in GeneratedEnchantIDs)
                enchantIDStrings.Add(generatedEnchantID.ToString());
            return string.Concat("DELETE FROM spell_enchant_proc_data WHERE `entry` IN (", string.Join(",", enchantIDStrings), ");");
        }

        public void AddRowForRogueWeaponProc(int enchantID, float procsPerMinute)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entry", enchantID);
            newRow.AddInt("customChance", 0); // Unused, since a PPM is set
            newRow.AddFloat("PPMChance", procsPerMinute);
            newRow.AddInt("procEx", 0); // No hit type requirement beyond damage being dealt, matching the stock rogue poisons
            newRow.AddInt("attributeMask", 0); // Not restricted to white hits, so special attacks apply poison too (again matching stock rogue poisons)
            Rows.Add(newRow);
            GeneratedEnchantIDs.Add(enchantID);
        }
    }
}
