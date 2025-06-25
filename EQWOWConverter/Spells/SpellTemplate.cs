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
        public static Dictionary<int, int> SpellCastTimeDBCIDsByCastTime = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellRangeDBCIDsBySpellRange = new Dictionary<int, int>();

        private static Dictionary<int, SpellTemplate> SpellTemplatesByEQID = new Dictionary<int, SpellTemplate>();
        private static readonly object SpellTemplateLock = new object();

        public class Reagent
        {
            public int WOWItemTemplateEntryID;
            public int Count;

            public Reagent(int wowItemTemplateEntryID, int count)
            {
                WOWItemTemplateEntryID = wowItemTemplateEntryID;
                Count = count;
            }
        }

        public int WOWSpellID = 0;
        public int EQSpellID = -1;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public string AuraDescription = string.Empty;
        public UInt32 Category = 1;
        public UInt32 InterruptFlags = 15;
        public int SpellIconID = 0;
        public TradeskillRecipe? TradeskillRecipe = null;
        protected int _SpellCastTimeDBCID = 1; // First row, instant cast
        public int SpellCastTimeDBCID { get { return _SpellCastTimeDBCID; } }
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
        protected int _SpellRangeDBCID = 1; // First row, self only (no range)
        public int SpellRangeDBCID { get { return _SpellRangeDBCID; } }
        protected int _SpellRange = 0;
        public int SpellRange
        {
            get { return _SpellRange; }
            set
            {
                if (SpellRangeDBCIDsBySpellRange.ContainsKey(value) == false)
                    SpellRangeDBCIDsBySpellRange.Add(value, SpellRangeDBC.GenerateDBCID());
                _SpellRange = SpellRangeDBCIDsBySpellRange[value];
            }
        }        
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

        public static Dictionary<int, SpellTemplate> GetSpellTemplatesByEQID()
        {
            lock (SpellTemplateLock)
            {
                if (SpellTemplatesByEQID.Count == 0)
                    LoadSpellTemplates();
                return SpellTemplatesByEQID;
            }
        }

        private static void LoadSpellTemplates()
        {
            // Load the spell templates
            string spellTemplatesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpellTemplates.csv");
            Logger.WriteDebug(string.Concat("Loading spell templates via file '", spellTemplatesFile, "'"));
            List<Dictionary<string, string>> spellTemplateRows = FileTool.ReadAllRowsFromFileWithHeader(spellTemplatesFile, "|");
            foreach (Dictionary<string, string> columns in spellTemplateRows)
            {
                // Load the row
                SpellTemplate newSpellTemplate = new SpellTemplate();
                newSpellTemplate.EQSpellID = int.Parse(columns["eq_id"]);
                newSpellTemplate.WOWSpellID = int.Parse(columns["wow_id"]);
                newSpellTemplate.Name = columns["Name"];
                newSpellTemplate.AuraDescription = newSpellTemplate.Name; // TODO: Find strings for these
                newSpellTemplate.Description = newSpellTemplate.Name; // TODO: Find strings for these
                newSpellTemplate.SpellRange = Convert.ToInt32(float.Parse(columns["range"]) * Configuration.SPELLS_RANGE_MULTIPLIER);
                newSpellTemplate.RecoveryTimeInMS = UInt32.Parse(columns["recovery_time"]);
                // TODO: AOE range?

                // Determine the effect, and currently only load in spells that modify hitpoints




                // Add it
                SpellTemplatesByEQID.Add(newSpellTemplate.EQSpellID, newSpellTemplate);
            }
        }
    }
}
