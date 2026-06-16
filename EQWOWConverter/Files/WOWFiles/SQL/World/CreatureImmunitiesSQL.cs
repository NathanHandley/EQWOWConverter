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
using EQWOWConverter.Spells;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureImmunitiesSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM creature_immunities WHERE `ID` >= " + Configuration.SQL_CREATUREIMMUNITIES_ID_START.ToString() + " AND `ID` <= " + Configuration.SQL_CREATUREIMMUNITIES_ID_END.ToString() + ";";
        }

        public void AddRow(int id, long mechanicsMask)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", id);
            newRow.AddInt("SchoolMask", 0);
            newRow.AddInt("DispelTypeMask", 0);
            newRow.AddInt("MechanicsMask", Convert.ToInt32(mechanicsMask));
            newRow.AddString("Effects", string.Empty);
            newRow.AddString("Auras", string.Empty);
            newRow.AddInt("ImmuneAoE", 0);
            newRow.AddInt("ImmuneChain", 0);
            newRow.AddString("Comment", GenerateComment(mechanicsMask));
            Rows.Add(newRow);
        }

        private static string GenerateComment(long mechanicsMask)
        {
            StringBuilder mechanicNames = new StringBuilder();
            for (int bit = 0; bit < 32; bit++)
            {
                if ((mechanicsMask & (1L << bit)) == 0)
                    continue;
                if (mechanicNames.Length > 0)
                    mechanicNames.Append("|");
                mechanicNames.Append(GetMechanicName(bit));
            }
            return string.Concat("EQ immune to ", mechanicNames.ToString());
        }

        private static string GetMechanicName(int mechanicBit)
        {
            switch ((SpellMechanicType)mechanicBit)
            {
                case SpellMechanicType.Charmed: return "Charm";
                case SpellMechanicType.Fleeing: return "Fear";
                case SpellMechanicType.Rooted: return "Root";
                case SpellMechanicType.Slowed: return "Slow";
                case SpellMechanicType.Snared: return "Snare";
                case SpellMechanicType.Stunned: return "Stun";
                case SpellMechanicType.Incapacitated: return "Mez";
                default: return string.Concat("Mechanic", mechanicBit.ToString());
            }
        }
    }
}
