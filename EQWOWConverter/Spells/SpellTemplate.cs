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

namespace EQWOWConverter.Spells
{
    internal class SpellTemplate
    {
        private static int CUR_SPELL_DBCID = Configuration.CONFIG_DBCID_SPELL_ID_START;

        public int ID = 0;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public int SpellIconID = 0;

        public static int GenerateID()
        {
            int spellID = CUR_SPELL_DBCID;
            CUR_SPELL_DBCID++;
            return spellID;
        }
    }
}
