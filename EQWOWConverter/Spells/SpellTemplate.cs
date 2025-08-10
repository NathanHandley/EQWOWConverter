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

using EQWOWConverter.Common;
using EQWOWConverter.Tradeskills;
using EQWOWConverter.WOWFiles;
using System.Text;

namespace EQWOWConverter.Spells
{
    internal class SpellTemplate
    {
        internal class SpellLearnScrollProperties
        {
            public int LearnLevel = -1;
            public int WOWItemTemplateID = -1;
        }

        public static Dictionary<int, int> SpellCastTimeDBCIDsByCastTime = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellRangeDBCIDsBySpellRange = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellDurationDBCIDsByDurationInMS = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellGroupStackRuleByGroup = new Dictionary<int, int>();

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
                _SpellRange = value;
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
                _SpellDurationDBCID = SpellDurationDBCIDsByDurationInMS[value];
                _SpellDurationInMS = value;
            }
        }
        public int SpellGroupStackingID = -1;
        public int SpellGroupStackingRule = 0;
        public UInt32 RecoveryTimeInMS = 0;
        public SpellWOWTargetType WOWTargetType = SpellWOWTargetType.Self;
        public UInt32 SpellVisualID1 = 0;
        public UInt32 SpellVisualID2 = 0;
        public bool PlayerLearnableByClassTrainer = false; // Needed?
        public int MinimumPlayerLearnLevel = -1;
        public bool HasEffectBaseFormulaUsingSpellLevel = false;
        public int RequiredAreaIDs = -1;
        public UInt32 SchoolMask = 1;
        public UInt32 RequiredTotemID1 = 0;
        public UInt32 RequiredTotemID2 = 0;
        public UInt32 SpellFocusID = 0;
        public bool AllowCastInCombat = true;
        public List<Reagent> Reagents = new List<Reagent>();
        public int SkillLine = 0;
        public List<SpellEffectEQ> EQSpellEffects = new List<SpellEffectEQ>();
        public List<SpellEffectWOW> WOWSpellEffects = new List<SpellEffectWOW>();
        public UInt32 ManaCost = 0;
        public SpellEQTargetType EQTargetType = SpellEQTargetType.Single;
        public UInt32 TargetCreatureType = 0; // No specific creature type
        public Dictionary<ClassType, SpellLearnScrollProperties> LearnScrollPropertiesByClassType = new Dictionary<ClassType, SpellLearnScrollProperties>();
        public int HighestDirectHealAmountInSpellEffect = 0; // Used in spell priority calculations

        public static Dictionary<int, SpellTemplate> GetSpellTemplatesByEQID()
        {
            lock (SpellTemplateLock)
            {
                if (SpellTemplatesByEQID.Count == 0)
                    LoadSpellTemplates();
                return SpellTemplatesByEQID;
            }
        }

        public static void LoadSpellTemplates()
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
                //newSpellTemplate.AuraDescription = newSpellTemplate.Name; // TODO: Find strings for these
                //newSpellTemplate.Description = newSpellTemplate.Name; // TODO: Find strings for these
                newSpellTemplate.SpellRange = Convert.ToInt32(float.Parse(columns["range"]) * Configuration.SPELLS_RANGE_MULTIPLIER);
                // TODO: AOE range?
                newSpellTemplate.RecoveryTimeInMS = UInt32.Parse(columns["cast_recovery_time"]);
                newSpellTemplate.Category = 0; // Temp / TODO: Figure out how/what to set here
                newSpellTemplate.CastTimeInMS = int.Parse(columns["cast_time"]);
                // TODO: FacingCasterFlags
                for (int i = 1; i <= 12; i++)
                    PopulateEQSpellEffect(ref newSpellTemplate, i, columns);
                // Skip if there isn't an effect
                if (newSpellTemplate.EQSpellEffects.Count == 0)
                    continue;
                PopulateAllClassLearnScrollProperties(ref newSpellTemplate, columns);
                newSpellTemplate.ManaCost = Convert.ToUInt32(columns["mana"]);
                int buffDurationInTicks = Convert.ToInt32(columns["buffduration"]);
                if (buffDurationInTicks > 0)
                {
                    // TODO: Formulas 50 and 51 are perm effects and should have no duration
                    int buffDurationFormula = Convert.ToInt32(columns["buffdurationformula"]);
                    newSpellTemplate.SpellDurationInMS = GetBuffDurationInMS(buffDurationInTicks, buffDurationFormula);
                }

                // Icon
                int spellIconID = int.Parse(columns["icon"]);
                if (spellIconID >= 2500)
                    newSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(spellIconID - 2500);

                // Target
                int eqTargetTypeID = int.Parse(columns["targettype"]);
                bool isDetrimental = int.Parse(columns["goodEffect"]) == 0 ? true : false; // "2" should be non-detrimental group only (not caster).  Ignoring that for now.
                PopulateTarget(ref newSpellTemplate, eqTargetTypeID, isDetrimental);

                // Visual
                int spellVisualEffectIndex = int.Parse(columns["SpellVisualEffectIndex"]);
                if (spellVisualEffectIndex >= 0 && spellVisualEffectIndex < 52)
                    newSpellTemplate.SpellVisualID1 = Convert.ToUInt32(SpellVisual.GetSpellVisual(spellVisualEffectIndex, !isDetrimental).SpellVisualDBCID);

                // School class
                int resistType = int.Parse(columns["resisttype"]);
                newSpellTemplate.SchoolMask = GetSchoolMaskForResistType(resistType);

                // Convert the spell effects
                ConvertEQSpellEffectsIntoWOWEffects(ref newSpellTemplate, newSpellTemplate.SchoolMask, newSpellTemplate.SpellDurationInMS, newSpellTemplate.CastTimeInMS);

                // If there is no wow effect, skip it
                if (newSpellTemplate.WOWSpellEffects.Count == 0)
                    continue;

                // Set the spell and aura descriptions
                SetMainAndAuraDescriptions(ref newSpellTemplate);

                // Stacking rules
                SetAuraStackRule(ref newSpellTemplate, int.Parse(columns["spell_category"]));

                // Add it
                SpellTemplatesByEQID.Add(newSpellTemplate.EQSpellID, newSpellTemplate);
            }
        }

        private static void PopulateAllClassLearnScrollProperties(ref SpellTemplate spellTemplate, Dictionary<string, string> rowColumns)
        {
            // In EQ, a scroll can be learned by multiple classes at different levels
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Warrior, "warrior", "bard");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Paladin, "paladin");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Hunter, "ranger", "beastlord");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Rogue, "monk", "rogue");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Priest, "cleric");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.DeathKnight, "shadowknight");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Shaman, "shaman");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Mage, "magician", "wizard");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Warlock, "necromancer", "enchanter");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Druid, "druid");
        }

        private static UInt32 GetSchoolMaskForResistType(int eqResistType)
        {
            switch (eqResistType)
            {
                case 1: return 64; // EQ Magic => WOW Arcane
                case 2: return 4; // EQ Fire => WOW Fire
                case 3: return 16; // EQ Cold => WOW Frost
                case 4: return 8; // EQ Poison => WOW Nature
                case 5: return 32; // EQ Disease => WOW Shadow
                default: return 1; // Physical by default
            }
        }

        private static void PopulateClassLearnScrollProperties(ref SpellTemplate spellTemplate, Dictionary<string, string> rowColumns, ClassType wowClassType, params string[] eqClassNames)
        {
            SpellLearnScrollProperties spellLearnScrollProperties = new SpellLearnScrollProperties();
            
            // Use the lowest learn level id properties
            foreach(string className in eqClassNames)
            {
                int curClassLearnlevel = int.Parse(rowColumns[string.Concat(className, "_learn_level")]);
                if (curClassLearnlevel != -1 && curClassLearnlevel <= 100 && (curClassLearnlevel < spellLearnScrollProperties.LearnLevel || spellLearnScrollProperties.LearnLevel == -1))
                {
                    spellLearnScrollProperties.LearnLevel = curClassLearnlevel;
                    spellLearnScrollProperties.WOWItemTemplateID = int.Parse(rowColumns[string.Concat(className, "_learn_wowitemid")]);
                }
            }

            // Only save it if a valid one was found
            if (spellLearnScrollProperties.LearnLevel > -1)
            {
                spellTemplate.LearnScrollPropertiesByClassType[wowClassType] = spellLearnScrollProperties;

                // Also save it as the lowest level possible to learn for future formulas
                if (spellTemplate.MinimumPlayerLearnLevel <= 0 || spellTemplate.MinimumPlayerLearnLevel > spellLearnScrollProperties.LearnLevel)
                    spellTemplate.MinimumPlayerLearnLevel = spellLearnScrollProperties.LearnLevel;
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

        private static int GetBuffDurationInMS(int eqBuffDurationInTicks, int eqBuffDurationFormula)
        {
            int buffTicks = eqBuffDurationInTicks;

            // TODO: There are level-based formulas, but they are ignored for now since spellpower is a 'thing' and balance
            // is already achieved
            switch (eqBuffDurationFormula)
            {
                case 4: buffTicks = Math.Min(50, eqBuffDurationInTicks); break;
                case 5: buffTicks = Math.Min(2, eqBuffDurationInTicks); break;
                default: break;
            }

            return eqBuffDurationInTicks * Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_EQ * 1000;
        }

        private static void PopulateEQSpellEffect(ref SpellTemplate spellTemplate, int slotID, Dictionary<string, string> rowColumns)
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
            SpellEffectEQ curEffect = new SpellEffectEQ();
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

            // Some spells have an effect formula based on the spell level, so they must have their "spell level" set for formulas
            if ((int)curEffect.EQBaseValueFormulaType >= 111 || (int)curEffect.EQBaseValueFormulaType <= 118)
                spellTemplate.HasEffectBaseFormulaUsingSpellLevel = true;

            // Add it
            spellTemplate.EQSpellEffects.Add(curEffect);
        }

        private static void ConvertEQSpellEffectsIntoWOWEffects(ref SpellTemplate spellTemplate, UInt32 schoolMask, int spellDurationInMS, int spellCastTimeInMS)
        {
            // Process all spell effects
            foreach (SpellEffectEQ eqEffect in spellTemplate.EQSpellEffects)
            {
                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                SpellEffectWOW? newSpellEffectWOW2 = null;
                switch (eqEffect.EQEffectType)
                {
                    case SpellEQEffectType.CurrentHitPoints: // Fallthrough
                    case SpellEQEffectType.CurrentHitPointsOnce:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            string elementalSchoolName = string.Empty;
                            switch (schoolMask)
                            {
                                case 4: elementalSchoolName = "fire"; break;
                                case 8: elementalSchoolName = "nature"; break;
                                case 16: elementalSchoolName = "frost"; break;
                                case 32: elementalSchoolName = "shadow"; break;
                                case 64: elementalSchoolName = "arcane"; break;
                                default: break;
                            }

                            int preFormulaEffectAmount = Math.Abs(eqEffect.EQBaseValue);
                            if (spellDurationInMS <= 0 || eqEffect.EQEffectType == SpellEQEffectType.CurrentHitPointsOnce)
                            {
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                if (eqEffect.EQBaseValue > 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "HealDirectHPS");
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.Heal;
                                    newSpellEffectWOW.ActionDescription = string.Concat("healed for ", newSpellEffectWOW.EffectBasePoints);
                                    spellTemplate.HighestDirectHealAmountInSpellEffect = Math.Max(spellTemplate.HighestDirectHealAmountInSpellEffect, newSpellEffectWOW.EffectBasePoints);
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "DamageDirectDPS");
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.SchoolDamage;
                                    if (elementalSchoolName.Length > 0)
                                        newSpellEffectWOW.ActionDescription = string.Concat("struck for ", newSpellEffectWOW.EffectBasePoints, " ", elementalSchoolName, " damage");
                                    else
                                        newSpellEffectWOW.ActionDescription = string.Concat("struck for ", newSpellEffectWOW.EffectBasePoints, " damage");
                                }
                            }
                            else
                            {
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                float effectAmountMod = Convert.ToSingle(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW) / Convert.ToSingle(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_EQ);
                                preFormulaEffectAmount = Math.Max((int)Math.Round(Convert.ToSingle(preFormulaEffectAmount) * effectAmountMod), 1);
                                newSpellEffectWOW.EffectAuraPeriod = Convert.ToUInt32(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW) * 1000;
                                if (eqEffect.EQBaseValue > 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "HealOverTimeHPS");
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicHeal;
                                    newSpellEffectWOW.ActionDescription = string.Concat("regenerates ", newSpellEffectWOW.EffectBasePoints, " health per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                    newSpellEffectWOW.AuraDescription = string.Concat("regenerating ", newSpellEffectWOW.EffectBasePoints, " health per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                }
                                else
                                {
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicDamage;
                                    if (elementalSchoolName.Length > 0)
                                    {
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "DamageOverTimeDPS");
                                        newSpellEffectWOW.ActionDescription = string.Concat("inflicts ", newSpellEffectWOW.EffectBasePoints, " ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                        newSpellEffectWOW.AuraDescription = string.Concat("suffering ", newSpellEffectWOW.EffectBasePoints, " ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "DamageOverTimeDPS");
                                        newSpellEffectWOW.ActionDescription = string.Concat("inflicts ", newSpellEffectWOW.EffectBasePoints, " ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                        newSpellEffectWOW.AuraDescription = string.Concat("suffering ", newSpellEffectWOW.EffectBasePoints, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                    }
                                }
                            }
                        } break;
                    case SpellEQEffectType.ArmorClass:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                            newSpellEffectWOW.EffectMiscValueA = 1; // Armor
                            if (newSpellEffectWOW.EffectBasePoints >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "ArmorClassBuff");
                                newSpellEffectWOW.ActionDescription = string.Concat("increases armor by ", newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.AuraDescription = string.Concat("armor increased by ", newSpellEffectWOW.EffectBasePoints);
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "ArmorClassDebuff");
                                int reductionAmount = Math.Abs(newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.ActionDescription = string.Concat("decreases armor by ", reductionAmount);
                                newSpellEffectWOW.AuraDescription = string.Concat("armor decreased by ", reductionAmount);
                            }
                        } break;
                    case SpellEQEffectType.Attack:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModAttackPower;
                            if (newSpellEffectWOW.EffectBasePoints >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "AttackBuff");
                                newSpellEffectWOW.ActionDescription = string.Concat("increases attack power by ", newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.AuraDescription = string.Concat("attack power increased by ", newSpellEffectWOW.EffectBasePoints);
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "AttackDebuff");
                                int reductionAmount = Math.Abs(newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.ActionDescription = string.Concat("decreases attack power by ", reductionAmount);
                                newSpellEffectWOW.AuraDescription = string.Concat("attack power decreased by ", reductionAmount);
                            }

                            // Add a second for ranged attack power
                            newSpellEffectWOW2 = newSpellEffectWOW.Clone();
                            newSpellEffectWOW2.ActionDescription = string.Empty;
                            newSpellEffectWOW2.AuraDescription = string.Empty;
                            newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModRangedAttackPower;
                        } break;
                    case SpellEQEffectType.MovementSpeed:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "");
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModIncreaseSpeed;
                                newSpellEffectWOW.ActionDescription = string.Concat("increases non-mounted movement speed by ", newSpellEffectWOW.EffectBasePoints, "% ");
                                newSpellEffectWOW.AuraDescription = string.Concat("non-mounted movement speed increased by ", newSpellEffectWOW.EffectBasePoints, "% ");
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "");
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModDecreaseSpeed;
                                newSpellEffectWOW.ActionDescription = string.Concat("decreases movement speed by ", Math.Abs(newSpellEffectWOW.EffectBasePoints), "% ");
                                newSpellEffectWOW.AuraDescription = string.Concat("movement speed decreased by ", Math.Abs(newSpellEffectWOW.EffectBasePoints), "% ");
                                newSpellEffectWOW.EffectMechanic = SpellMechanicType.Snared;
                            }
                        } break;
                    case SpellEQEffectType.TotalHP:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModMaximumHealth;
                            if (newSpellEffectWOW.EffectBasePoints >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "MaxHPBuff");
                                newSpellEffectWOW.ActionDescription = string.Concat("increases maximum health by ", newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.AuraDescription = string.Concat("maximum health increased by ", newSpellEffectWOW.EffectBasePoints);
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "");
                                int reductionAmount = Math.Abs(newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.ActionDescription = string.Concat("decreases maximum health by ", reductionAmount);
                                newSpellEffectWOW.AuraDescription = string.Concat("maximum health decreased by ", reductionAmount);
                            }
                        } break;
                    case SpellEQEffectType.Strength:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                            newSpellEffectWOW.EffectMiscValueA = 0; // Strength
                            if (newSpellEffectWOW.EffectBasePoints >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "StrengthBuff");
                                newSpellEffectWOW.ActionDescription = string.Concat("increases maximum strength by ", newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.AuraDescription = string.Concat("maximum strength increased by ", newSpellEffectWOW.EffectBasePoints);
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "StrengthDebuff");
                                int reductionAmount = Math.Abs(newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.ActionDescription = string.Concat("decreases maximum strength by ", reductionAmount);
                                newSpellEffectWOW.AuraDescription = string.Concat("maximum strength decreased by ", reductionAmount);
                            }
                        } break;
                    case SpellEQEffectType.Dexterity: // Fallthrough
                    case SpellEQEffectType.Agility:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            // EQ Dexterity and EQ Agility are both mapped WOW agility, so use the higher of the two and reuse if one exists
                            bool ignoreAsAglEffectExistsAndIsStronger = false;
                            foreach (SpellEffectWOW wowEffect in spellTemplate.WOWSpellEffects)
                            {
                                if (wowEffect.EffectAuraType == SpellWOWAuraType.ModStat && wowEffect.EffectMiscValueA == 1)
                                {
                                    if (wowEffect.EffectBasePoints > 0 && wowEffect.EffectBasePoints >= eqEffect.EQBaseValue)
                                        ignoreAsAglEffectExistsAndIsStronger = true;
                                    if (wowEffect.EffectBasePoints < 0 && wowEffect.EffectBasePoints <= eqEffect.EQBaseValue)
                                        ignoreAsAglEffectExistsAndIsStronger = true;
                                    newSpellEffectWOW = wowEffect;
                                }
                            }
                            if (ignoreAsAglEffectExistsAndIsStronger == true)
                                continue;

                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                            newSpellEffectWOW.EffectMiscValueA = 1; // Agility
                            if (newSpellEffectWOW.EffectBasePoints >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "AgilityBuff");
                                newSpellEffectWOW.ActionDescription = string.Concat("increases agility by ", newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.AuraDescription = string.Concat("agility increased by ", newSpellEffectWOW.EffectBasePoints);
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "AgilityDebuff");
                                int reductionAmount = Math.Abs(newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.ActionDescription = string.Concat("decreases agility by ", reductionAmount);
                                newSpellEffectWOW.AuraDescription = string.Concat("agility decreased by ", reductionAmount);
                            }
                        } break;
                    case SpellEQEffectType.Stamina:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                            newSpellEffectWOW.EffectMiscValueA = 2; // Stamina
                            if (newSpellEffectWOW.EffectBasePoints >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "StaminaBuff");
                                newSpellEffectWOW.ActionDescription = string.Concat("increases stamina by ", newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.AuraDescription = string.Concat("stamina increased by ", newSpellEffectWOW.EffectBasePoints);
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "StaminaDebuff");
                                int reductionAmount = Math.Abs(newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.ActionDescription = string.Concat("decreases stamina by ", reductionAmount);
                                newSpellEffectWOW.AuraDescription = string.Concat("stamina decreased by ", reductionAmount);
                            }
                        } break;
                    case SpellEQEffectType.Intelligence:
                        {
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                            newSpellEffectWOW.EffectMiscValueA = 3; // Intellect
                            if (newSpellEffectWOW.EffectBasePoints >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "IntellectBuff");
                                newSpellEffectWOW.ActionDescription = string.Concat("increases intellect by ", newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.AuraDescription = string.Concat("intellect increased by ", newSpellEffectWOW.EffectBasePoints);
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "IntellectDebuff");
                                int reductionAmount = Math.Abs(newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.ActionDescription = string.Concat("decreases intellect by ", reductionAmount);
                                newSpellEffectWOW.AuraDescription = string.Concat("intellect decreased by ", reductionAmount);
                            }
                        } break;
                    case SpellEQEffectType.Wisdom:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                            newSpellEffectWOW.EffectMiscValueA = 4; // Spirit
                            if (newSpellEffectWOW.EffectBasePoints >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "SpiritBuff");
                                newSpellEffectWOW.ActionDescription = string.Concat("increases spirit by ", newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.AuraDescription = string.Concat("spirit increased by ", newSpellEffectWOW.EffectBasePoints);
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "SpiritDebuff");
                                int reductionAmount = Math.Abs(newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.ActionDescription = string.Concat("decreases spirit by ", reductionAmount);
                                newSpellEffectWOW.AuraDescription = string.Concat("spirit decreased by ", reductionAmount);
                            }
                        } break;
                    case SpellEQEffectType.Charisma:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModHitChance;
                            if (newSpellEffectWOW.EffectBasePoints >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "HitPctBuff");
                                newSpellEffectWOW.ActionDescription = string.Concat("increases hit chance by ", newSpellEffectWOW.EffectBasePoints, "%");
                                newSpellEffectWOW.AuraDescription = string.Concat("hit chance increased by ", newSpellEffectWOW.EffectBasePoints, "%");
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "HitPctDebuff");
                                int reductionAmount = Math.Abs(newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.ActionDescription = string.Concat("decreases hit chance by ", reductionAmount, "%");
                                newSpellEffectWOW.AuraDescription = string.Concat("hit chance decreased by ", reductionAmount, "%");
                            }
                        } break;
                    case SpellEQEffectType.AttackSpeed:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModMeleeHaste;

                            // Baseline for attack speed is 100, so above that is increase and below that is decrease
                            newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue - 100, eqEffect.EQMaxValue - 100, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, true, "");
                            if (newSpellEffectWOW.EffectBasePoints >= 0)
                            {   
                                newSpellEffectWOW.ActionDescription = string.Concat("increases attack speed by ", newSpellEffectWOW.EffectBasePoints, "%");
                                newSpellEffectWOW.AuraDescription = string.Concat("attack speed increased by ", newSpellEffectWOW.EffectBasePoints, "%");
                            }
                            else
                            {
                                int reductionAmount = Math.Abs(newSpellEffectWOW.EffectBasePoints);
                                newSpellEffectWOW.ActionDescription = string.Concat("decreases attack speed by ", reductionAmount, "%");
                                newSpellEffectWOW.AuraDescription = string.Concat("attack speed decreased by ", reductionAmount, "%");
                            }

                            // Add a second for ranged attack speed
                            newSpellEffectWOW2 = newSpellEffectWOW.Clone();
                            newSpellEffectWOW2.ActionDescription = string.Empty;
                            newSpellEffectWOW2.AuraDescription = string.Empty;
                            newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModRangedHaste;
                        }
                        break;
                    default:
                        {
                            Logger.WriteError("Unhandled SpellTemplate EQEffectType of ", eqEffect.EQEffectType.ToString(), " for eq spell id ", spellTemplate.EQSpellID.ToString());
                            continue;
                        }
                }                       
                spellTemplate.WOWSpellEffects.Add(newSpellEffectWOW);
                if (newSpellEffectWOW2 != null)
                    spellTemplate.WOWSpellEffects.Add(newSpellEffectWOW2);
            }

            // TODO: Collapse multi-stat effects into 1 where possible

            // Sort them so the aura effects are last
            spellTemplate.WOWSpellEffects.Sort();
        }

        private static void SetAuraStackRule(ref SpellTemplate spellTemplate, int eqSpellCategory)
        {
            // This is exactly how EQ does spell stacking, but it seems like an appropriate approximation that works well for wow
            if (eqSpellCategory < 0) // NPC = -99, AA Procs = -1
                return;
            if (eqSpellCategory > 250) // AA Abilities = 999
                return;

            // Damage detrimental effects should stack from multiple sources / spells
            if (eqSpellCategory == 7 || // Damage over time (magic)
                eqSpellCategory == 8 || // Damage over time (undead)
                eqSpellCategory == 9 || // Damage over time (life taps)
                eqSpellCategory == 129 || // Damage over time (fire)
                eqSpellCategory == 130 || // Damage over time (ice)
                eqSpellCategory == 131 || // Damage over time (poison)
                eqSpellCategory == 132) // Damage over time (disease)
                spellTemplate.SpellGroupStackingRule = 2; // SPELL_GROUP_STACK_FLAG_NOT_SAME_CASTER
            else
                spellTemplate.SpellGroupStackingRule |= 8; // SPELL_GROUP_STACK_FLAG_NEVER_STACK

            // Calculate the category
            int groupStackingID = Configuration.SQL_SPELL_GROUP_ID_START + eqSpellCategory;
            if (SpellGroupStackRuleByGroup.ContainsKey(groupStackingID) == false)
                SpellGroupStackRuleByGroup[groupStackingID] = spellTemplate.SpellGroupStackingRule;
            spellTemplate.SpellGroupStackingID = groupStackingID;
        }

        private static void SetMainAndAuraDescriptions(ref SpellTemplate spellTemplate)
        {
            StringBuilder descriptionSB = new StringBuilder();
            StringBuilder auraSB = new StringBuilder();
            descriptionSB.Append("Target ");
            for (int i = 0; i < spellTemplate.WOWSpellEffects.Count; i++)
            {
                if (i != 0)
                {
                    if (spellTemplate.WOWSpellEffects[i].ActionDescription.Length > 0)
                        descriptionSB.Append(", ");
                    if (spellTemplate.WOWSpellEffects[i].AuraDescription.Length > 0 && auraSB.Length > 0)
                        auraSB.Append(", ");
                }
                descriptionSB.Append(spellTemplate.WOWSpellEffects[i].ActionDescription);
                auraSB.Append(spellTemplate.WOWSpellEffects[i].AuraDescription);
            }

            // Store and control capitalization
            descriptionSB.Append('.');
            auraSB.Append('.');
            spellTemplate.Description = descriptionSB.ToString().ToLower();
            if (spellTemplate.Description.Length > 0)
                spellTemplate.Description = string.Concat(char.ToUpper(spellTemplate.Description[0]), spellTemplate.Description.Substring(1));
            spellTemplate.AuraDescription = auraSB.ToString().ToLower();
            if (spellTemplate.AuraDescription.Length > 0)
                spellTemplate.AuraDescription = string.Concat(char.ToUpper(spellTemplate.AuraDescription[0]), spellTemplate.AuraDescription.Substring(1));

            // Add the time duration
            spellTemplate.Description = string.Concat(spellTemplate.Description, GetTimeDurationStringFromMSWithLeadingSpace(spellTemplate.SpellDurationInMS));
        }

        private static string GetTimeDurationStringFromMSWithLeadingSpace(int durationInMS)
        {
            // Skip no duration
            if (durationInMS <= 0)
                return string.Empty;

            // Pull out the time values
            int wholeMinutes = durationInMS / 60000;
            int remainderSeconds = (durationInMS % 60000) / 1000;

            // Different text depending on minutes, seconds, or both
            if (wholeMinutes > 0)
            {
                if (remainderSeconds > 0)
                    return (string.Concat(" Lasts ", wholeMinutes, " min ", remainderSeconds, " sec."));
                else
                    return (string.Concat(" Lasts ", wholeMinutes, " min."));
            }
            else // Just seconds
                return (string.Concat(" Lasts ", remainderSeconds, " sec."));
        }

        public List<List<SpellEffectWOW>> GetWOWEffectsInBlocksOfThree()
        {
            List<List<SpellEffectWOW>> returnList = new List<List<SpellEffectWOW>> ();
           
            // Blank out if no spell effects
            if (WOWSpellEffects.Count == 0)
            {
                List<SpellEffectWOW> curBlockSpellEffects = new List<SpellEffectWOW>();
                for (int i = 0; i < 3; i++)
                    curBlockSpellEffects.Add(new SpellEffectWOW());
                returnList.Add(curBlockSpellEffects);
                return returnList;
            }

            // Otherwise, group in batches of 3 adding blanks to pad the space
            int numOfExtractedEffects = 0;
            while (numOfExtractedEffects < WOWSpellEffects.Count)
            {
                List<SpellEffectWOW> curBlockSpellEffects = new List<SpellEffectWOW>();
                for (int i = 0; i < 3; i++)
                {
                    if (numOfExtractedEffects >= WOWSpellEffects.Count)
                        curBlockSpellEffects.Add(new SpellEffectWOW());
                    else
                        curBlockSpellEffects.Add(WOWSpellEffects[numOfExtractedEffects]);
                    numOfExtractedEffects += 1;
                }
                returnList.Add(curBlockSpellEffects);
            }

            return returnList;
        }


    }
}
