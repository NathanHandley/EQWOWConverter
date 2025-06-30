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
        public static Dictionary<int, int> SpellDurationDBCIDsByDurationInMS = new Dictionary<int, int>();

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
                _SpellRangeDBCID = SpellRangeDBCIDsBySpellRange[value];
            }
        }
        protected int _SpellDurationDBCID = 21; // "Infinite" by default"
        public int SpellDurationDBCID { get { return _SpellDurationDBCID; } }
        protected int _SpellDurationInMS = -1;
        public int SpellDurationInMS
        {
            get { return _SpellDurationInMS; }
            set
            {
                if (SpellDurationDBCIDsByDurationInMS.ContainsKey(value) == false)
                    SpellDurationDBCIDsByDurationInMS.Add(value, SpellDurationDBC.GenerateDBCID());
                _SpellDurationInMS = SpellDurationDBCIDsByDurationInMS[value];
            }
        }
        public UInt32 RecoveryTimeInMS = 0;
        public SpellWOWTargetType WOWTargetType = SpellWOWTargetType.Self;
        public UInt32 SpellVisualID1 = 0;
        public UInt32 SpellVisualID2 = 0;
        public bool PlayerLearnableByClassTrainer = false; // Needed?
        public SpellWOWEffectType EffectType1 = SpellWOWEffectType.None;
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
        public List<SpellEffect> SpellEffects = new List<SpellEffect>();
        public UInt32 ManaCost = 0;
        public SpellEQTargetType EQTargetType = SpellEQTargetType.Single;
        public UInt32 TargetCreatureType = 0; // No specific creature type

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
                // Skip invalid
                if (columns["enabled"] == "0")
                    continue;

                // Load the row
                SpellTemplate newSpellTemplate = new SpellTemplate();
                newSpellTemplate.EQSpellID = int.Parse(columns["eq_id"]);
                newSpellTemplate.WOWSpellID = int.Parse(columns["wow_id"]);
                newSpellTemplate.Name = columns["name"];
                newSpellTemplate.AuraDescription = newSpellTemplate.Name; // TODO: Find strings for these
                newSpellTemplate.Description = newSpellTemplate.Name; // TODO: Find strings for these
                newSpellTemplate.SpellRange = Convert.ToInt32(float.Parse(columns["range"]) * Configuration.SPELLS_RANGE_MULTIPLIER);
                // TODO: AOE range?
                newSpellTemplate.RecoveryTimeInMS = UInt32.Parse(columns["recast_time"]); // "recovery_time" is if interrupted 
                newSpellTemplate.Category = 0; // Temp / TODO: Figure out how/what to set here
                newSpellTemplate.CastTimeInMS = int.Parse(columns["cast_time"]);
                // TODO: FacingCasterFlags
                PopulateSpellEffect(ref newSpellTemplate, 1, columns);
                // Skip if there isn't an effect
                if (newSpellTemplate.SpellEffects.Count == 0)
                    continue;
                PopulateSpellEffect(ref newSpellTemplate, 2, columns);
                PopulateSpellEffect(ref newSpellTemplate, 3, columns);
                PopulateSpellEffect(ref newSpellTemplate, 4, columns);
                PopulateSpellEffect(ref newSpellTemplate, 5, columns);
                PopulateSpellEffect(ref newSpellTemplate, 6, columns);
                PopulateSpellEffect(ref newSpellTemplate, 7, columns);
                PopulateSpellEffect(ref newSpellTemplate, 8, columns);
                PopulateSpellEffect(ref newSpellTemplate, 9, columns);
                PopulateSpellEffect(ref newSpellTemplate, 10, columns);
                PopulateSpellEffect(ref newSpellTemplate, 11, columns);
                PopulateSpellEffect(ref newSpellTemplate, 12, columns);
                newSpellTemplate.ManaCost = Convert.ToUInt32(columns["mana"]);
                int buffDuration = Convert.ToInt32(columns["buffduration"]);
                if (buffDuration > 0)
                    newSpellTemplate.SpellDurationInMS = buffDuration * 1000;

                // Setting the spell effect type (TODO: Refine this)
                if (newSpellTemplate.SpellEffects[0].EQEffectType == SpellEQEffectType.CurrentHitPoints)
                {
                    if (newSpellTemplate.SpellEffects[0].EQBaseValue < 0)
                        newSpellTemplate.EffectType1 = SpellWOWEffectType.SchoolDamage;
                    else if (newSpellTemplate.SpellEffects[0].EQBaseValue > 0)
                        newSpellTemplate.EffectType1 = SpellWOWEffectType.Heal;
                }

                // Icon
                int spellIconID = int.Parse(columns["icon"]);
                if (spellIconID >= 2500)
                    newSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(spellIconID - 2500);

                // Target
                int eqTargetTypeID = int.Parse(columns["targettype"]);
                bool isDetrimental = int.Parse(columns["goodEffect"]) == 0 ? true : false; // "2" should be non-detrimental group only (not caster).  Ignoring that for now.
                PopulateTarget(ref newSpellTemplate, eqTargetTypeID, isDetrimental);

                // Add it
                SpellTemplatesByEQID.Add(newSpellTemplate.EQSpellID, newSpellTemplate);
            }
        }

        private static void PopulateTarget(ref SpellTemplate spellTemplate, int eqTargetTypeID, bool isDetrimental)
        {
            // Capture the EQ Target type
            if (Enum.IsDefined(typeof(SpellEQTargetType), eqTargetTypeID) == false)
            {
                Logger.WriteError("SpellTemplate with EQID ", spellTemplate.EQSpellID.ToString(), " has unknown target type of ", eqTargetTypeID.ToString());
                spellTemplate.WOWTargetType = SpellWOWTargetType.Self;
                return;
            }
            else
                spellTemplate.EQTargetType = (SpellEQTargetType)eqTargetTypeID;

            // Map the EQ target type to WOW
            switch (spellTemplate.EQTargetType)
            {
                case SpellEQTargetType.LineOfSight:
                    {
                        if (isDetrimental == true)
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetEnemy;
                        else
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetFriendly;
                    } break;
                case SpellEQTargetType.GroupV1:
                case SpellEQTargetType.GroupV2:
                    {
                        spellTemplate.WOWTargetType = SpellWOWTargetType.CasterParty;
                    } break;
                case SpellEQTargetType.PointBlankAreaOfEffect:
                case SpellEQTargetType.AreaOfEffectUndead: // TODO, may not be needed.  See "Words of the Undead King"
                    {
                        // This is from Circle of Healing and Thunderclap.  May have differences for good vs bad effects
                        spellTemplate.WOWTargetType = SpellWOWTargetType.AreaAroundCaster;
                    } break;
                case SpellEQTargetType.Single:
                    {
                        if (isDetrimental == true)
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetEnemy;
                        else
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetFriendly;
                    } break;
                case SpellEQTargetType.Self:
                    {
                        spellTemplate.WOWTargetType = SpellWOWTargetType.Self;
                    } break;
                case SpellEQTargetType.TargetedAreaOfEffect:
                case SpellEQTargetType.TargetedAreaOfEffectLifeTap:
                {
                        if (isDetrimental == true)
                            spellTemplate.WOWTargetType = SpellWOWTargetType.AreaAroundTargetEnemy;
                        else
                            spellTemplate.WOWTargetType = SpellWOWTargetType.AreaAroundTargetAlly;
                    } break;
                case SpellEQTargetType.Animal:
                    {
                        spellTemplate.TargetCreatureType = 1; // Beast, 0x0001
                        if (isDetrimental == true)
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetEnemy;
                        else
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetAny; // "lull" put into this for now
                    } break;
                case SpellEQTargetType.Undead:
                    {
                        spellTemplate.TargetCreatureType = 32; // Undead, 0x0020
                        if (isDetrimental == true)
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetEnemy;
                        else
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetAny; // "lull" and heal undead put into this for now
                    } break;
                case SpellEQTargetType.Summoned:
                    {
                        spellTemplate.TargetCreatureType = 8; // Elemental, 0x0008
                        spellTemplate.WOWTargetType = SpellWOWTargetType.TargetAny; // "Any" is probably wrong, figure out good vs bad
                    } break;
                case SpellEQTargetType.LifeTap:
                    {
                        if (isDetrimental == true)
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetEnemy;
                        else
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetFriendly;
                    } break;
                case SpellEQTargetType.Pet:
                    {
                        spellTemplate.WOWTargetType = SpellWOWTargetType.Pet;
                    } break;
                case SpellEQTargetType.Corpse:
                    {
                        // TODO: Make only work on corpses
                        spellTemplate.WOWTargetType = SpellWOWTargetType.Corpse;
                    } break;
                case SpellEQTargetType.Plant:
                    {
                        // Using "elemental" for now
                        spellTemplate.TargetCreatureType = 8; // Elemental, 0x0008
                        if (isDetrimental == true)
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetEnemy;
                        else
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetAny; // "lull" and heal undead put into this for now
                    } break;
                case SpellEQTargetType.UberGiants:
                    {
                        spellTemplate.TargetCreatureType = 16; // Giant, 0x0010
                        if (isDetrimental == true)
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetEnemy;
                        else
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetAny; // "lull" and heal undead put into this for now
                    } break;
                case SpellEQTargetType.UberDragons:
                    {
                        spellTemplate.TargetCreatureType = 2; // Dragonkin, 0x0002
                        if (isDetrimental == true)
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetEnemy;
                        else
                            spellTemplate.WOWTargetType = SpellWOWTargetType.TargetAny; // "lull" and heal undead put into this for now
                    } break;

                default:
                    {
                        Logger.WriteError("Unable to map eq target type ", spellTemplate.EQTargetType.ToString(), " to WOW target type");
                        spellTemplate.WOWTargetType = SpellWOWTargetType.Self;
                    } break;
            }
        }


        private static void PopulateSpellEffect(ref SpellTemplate spellTemplate, int slotID, Dictionary<string, string> rowColumns)
        {
            // Skip non and unmapped effects
            int effectIDRaw = int.Parse(rowColumns[string.Concat("effectid", slotID)]);          
            if (effectIDRaw == 254)
                return;
            if (Enum.IsDefined(typeof(SpellEQEffectType), effectIDRaw) == false)
            {
                Logger.WriteDebug(string.Concat("Skipping population of SpellEffect with EQID of ", spellTemplate.EQSpellID, " as the type ID ", effectIDRaw, " is not mapped"));
                return;
            }

            // Fill in the effect details
            SpellEffect curEffect = new SpellEffect();
            curEffect.EQEffectType = (SpellEQEffectType)effectIDRaw;
            if (slotID < 12)
            {
                int formulaRaw = int.Parse(rowColumns[string.Concat("formula", slotID)]);
                if (Enum.IsDefined(typeof(SpellEQBaseValueFormulaType), formulaRaw) == true)
                    curEffect.EQBaseValueFormulaType = (SpellEQBaseValueFormulaType)formulaRaw;
                else switch (formulaRaw)
                {
                    case 101: curEffect.EQBaseValueFormulaType = SpellEQBaseValueFormulaType.BaseAddLevelDivideTwo; break;
                    case 115: curEffect.EQBaseValueFormulaType = SpellEQBaseValueFormulaType.BaseAddSixTimesLevelMinusSpellLevel; break;
                    case 116: curEffect.EQBaseValueFormulaType = SpellEQBaseValueFormulaType.BaseAddEightTimesLevelMinusSpellLevel; break;
                    case 121: curEffect.EQBaseValueFormulaType = SpellEQBaseValueFormulaType.BaseAddLevelDivideThree; break;
                    default: curEffect.EQBaseValueFormulaType = SpellEQBaseValueFormulaType.BaseValue; break;
                }
                curEffect.EQFormulaTypeValue = formulaRaw;
            }
            curEffect.EQBaseValue = int.Parse(rowColumns[string.Concat("effect_base_value", slotID)]);
            if (slotID < 4)
                curEffect.EQLimitValue = int.Parse(rowColumns[string.Concat("effect_limit_value", slotID)]);
            if (slotID < 11)
                curEffect.EQMaxValue = int.Parse(rowColumns[string.Concat("max", slotID)]);

            // Set the effect amount
            switch (curEffect.EQEffectType)
            {
                case SpellEQEffectType.CurrentHitPoints:
                    {
                        // TODO: Formulas
                        spellTemplate.EffectDieSides1 = 1;
                        spellTemplate.EffectBasePoints1 = Math.Abs(curEffect.EQBaseValue);
                    } break;
                default:
                    {
                        Logger.WriteError(string.Concat("Unhandled SpellTemplate EQEffectType of ", curEffect.EQEffectType, " for eq spell id ", spellTemplate.EQSpellID));
                        return;
                    }
            }

            // Add it
            spellTemplate.SpellEffects.Add(curEffect);
        }
    }
}
