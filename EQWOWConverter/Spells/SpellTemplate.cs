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

using EQWOWConverter.Tradeskills;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Spells
{
    internal class SpellTemplate
    {
        public class Reagent
        {
            public int ItemID;
            public int Count;

            public Reagent(int itemID, int count)
            {
                ItemID = itemID;
                Count = count;
            }
        }

        public static Dictionary<int, int> SpellCastTimeDBCIDsByCastTime = new Dictionary<int, int>();

        public int ID = 0;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public string AuraDescription = string.Empty;
        public UInt32 Category = 1;
        public UInt32 InterruptFlags = 15;
        public int SpellIconID = 0;
        public TradeskillRecipe? TradeskillRecipe = null;
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
        public bool PlayerLearnableByClassTrainer = false; // Needed?
        public UInt32 DurationIndex = 0;
        public Int32 Effect1 = 0; // 6 = SPELL_EFFECT_APPLY_AURA
        public UInt32 EffectAura1 = 0; // 4 = SPELL_AURA_DUMMY
        public UInt32 EffectItemType1 = 0;
        public Int32 EffectDieSides1 = 0;
        public Int32 EffectBasePoints1 = 0;
        public int EffectMiscValue1 = 0;
        public int RequiredAreaIDs = -1;
        public UInt32 SchoolMask = 0;
        public UInt32 RequiredTotemID1 = 0;
        public UInt32 RequiredTotemID2 = 0;
        public UInt32 SpellFocusID = 0;
        public bool AllowCastInCombat = true;
        public List<Reagent> Reagents = new List<Reagent>();
        public int SkillLine = 0;
    }
}
