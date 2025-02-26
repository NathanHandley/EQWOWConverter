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

using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Spells
{
    internal class SpellTemplate
    {
        private static int CUR_SPELL_DBCID = Configuration.CONFIG_DBCID_SPELL_ID_START;
        public static Dictionary<int, int> SpellCastTimeDBCIDsByCastTime = new Dictionary<int, int>();

        public int ID = 0;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public int SpellIconID = 0;
        protected int _CastTimeInMS = 0;
        public int CastTimeInMS
        {
            get { return _CastTimeInMS; }
            set
            {
                if (SpellCastTimeDBCIDsByCastTime.ContainsKey(value) == false)
                    SpellCastTimeDBCIDsByCastTime.Add(value, SpellCastTimesDBC.GenerateDBCID());
                _SpellCastTimeDBCID = SpellCastTimeDBCIDsByCastTime[value];
            }
        }
        protected int _SpellCastTimeDBCID = 1; // First row, instant cast
        public int SpellCastTimeDBCID { get { return _SpellCastTimeDBCID; } }
        public int RangeIndexDBCID = 1; // 1 = self for now
        public UInt32 RecoveryTimeInMS = 0;
        public SpellTargetType TargetType = SpellTargetType.SelfSingle;
        public UInt32 SpellVisualID1 = 0;
        public UInt32 SpellVisualID2 = 0;
        public bool PlayerLearnableByClassTrainer = false;

        public static int GenerateID()
        {
            int spellID = CUR_SPELL_DBCID;
            CUR_SPELL_DBCID++;
            return spellID;
        }
    }
}
