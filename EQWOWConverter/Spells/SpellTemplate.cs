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
using EQWOWConverter.Creatures;
using EQWOWConverter.Items;
using EQWOWConverter.Tradeskills;
using EQWOWConverter.WOWFiles;
using EQWOWConverter.Zones;
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

        internal class ClickySpellParameters
        {
            public int WOWSpellID = -1;
            public bool IsForcedSelfOnly = false;
            public bool IsUsableWhileSilenced = false;
            public int FixedLevel = 0; // When > 0, effect values and aura duration are exactly this level (like tiered potions)
            protected int _SpellCastTimeDBCID = 1; // First row, instant cast
            public int SpellCastTimeDBCID { get { return _SpellCastTimeDBCID; } }
            protected int _CastTimeInMS = 0;
            public int CastTimeInMS
            {
                get { return _CastTimeInMS; }
                set
                {
                    if (SpellCastTimeDBCIDsByCastTime.ContainsKey(value) == false)
                        SpellCastTimeDBCIDsByCastTime.Add(value, IDGenerationTool.GenerateID("SpellCastTimeID", value.ToString()));
                    _SpellCastTimeDBCID = SpellCastTimeDBCIDsByCastTime[value];
                    _CastTimeInMS = value;
                }
            }
        }

        public static Dictionary<int, int> SpellCastTimeDBCIDsByCastTime = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellRangeDBCIDsBySpellRange = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellRadiusDBCIDsBySpellRadius = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellGroupStackRuleByGroup = new Dictionary<int, int>();
        private static Dictionary<int, int> SpellGroupIDByEffectStackKey = new Dictionary<int, int>();

        private static Dictionary<int, SpellTemplate> SpellTemplatesByEQID = new Dictionary<int, SpellTemplate>();
        private static Dictionary<(string, int, ItemFocusType, int), SpellTemplate> SpellTemplatesByFocusTypeAndValue = new Dictionary<(string, int, ItemFocusType, int), SpellTemplate>();
        private static readonly object SpellTemplateLock = new object();
        private static readonly object SpellEQIDLock = new object();
        private static int CUR_GENERATED_EQ_SPELL_ID = 5000;

        public class Reagent
        {
            public int WOWItemTemplateEntryID;
            public int Count;
            public int ParentWOWItemTemplateEntryID = 0;

            public Reagent(int wowItemTemplateEntryID, int count)
            {
                WOWItemTemplateEntryID = wowItemTemplateEntryID;
                Count = count;
            }
        }

        public int WOWSpellID = 0;
        public int WOWSpellIDProcAndGoodEffect = -1;
        public List<ClickySpellParameters> ClickySpellParatemers = new List<ClickySpellParameters>();
        public int EQSpellID = -1;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public string AuraDescription = string.Empty;
        public UInt32 Category = 1;
        public UInt32 CategoryRecoveryTimeInMS = 0; // When non-zero (with a shared Category), spells in the same category share a cooldown
        public UInt32 ChannelInterruptFlags = 0;
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
                    SpellCastTimeDBCIDsByCastTime.Add(value, IDGenerationTool.GenerateID("SpellCastTimeID", value.ToString()));
                _SpellCastTimeDBCID = SpellCastTimeDBCIDsByCastTime[value];
                _CastTimeInMS = value;
            }
        }
        public int CastTimeBeforeModsInMS = 0; // Original EQ cast time before any mods, required for aligning things like DPS
        protected int _SpellRangeDBCID = 1; // First row, self only (no range)
        public int SpellRangeDBCID { get { return _SpellRangeDBCID; } }
        protected int _SpellRange = 0;
        public int SpellRange
        {
            get { return _SpellRange; }
            set
            {
                if (SpellRangeDBCIDsBySpellRange.ContainsKey(value) == false)
                    SpellRangeDBCIDsBySpellRange.Add(value, IDGenerationTool.GenerateID("SpellRangeID", value.ToString()));
                _SpellRangeDBCID = SpellRangeDBCIDsBySpellRange[value];
                _SpellRange = value;
            }
        }
        public void SetSpellRangeToMeleeRange()
        {
            _SpellRangeDBCID = Configuration.DBCID_SPELLRANGE_MELEE_ID;
            _SpellRange = 5;
        }
        protected int _SpellRadiusDBCID = 0;
        public int SpellRadiusDBCID { get { return _SpellRadiusDBCID; } }
        protected int _SpellRadius = 0;
        public int SpellRadius
        {
            get { return _SpellRadius; }
            set
            {
                if (SpellRadiusDBCIDsBySpellRadius.ContainsKey(value) == false)
                    SpellRadiusDBCIDsBySpellRadius.Add(value, IDGenerationTool.GenerateID("SpellRadiusID", value.ToString()));
                _SpellRadiusDBCID = SpellRadiusDBCIDsBySpellRadius[value];
                _SpellRadius = value;
            }
        }
        public int EQAOERange = 0; // This is used as a data field for illusions
        public int EQBuffDurationInTicks = 0;
        public int EQBuffDurationFormula = 0;
        public SpellDuration AuraDuration = new SpellDuration();
        public float PeriodicDamageDurationCompensationMod = 1f;
        public List<int> SpellGroupStackingIDs = new List<int>();
        public List<int> WornSpellGroupStackingIDs = new List<int>(); // Worn auras only stack-compete with other worn auras, so they get separate groups
        private List<int> AuraStackEffectKeys = new List<int>();
        private int AuraStackKeyFlagBits = 0;
        public UInt32 RecoveryTimeInMS = 0; // Note that this may be zero for a player but not a creature.  See Configuration.SPELL_RECOVERY_TIME_MINIMUM_IN_MS
        public int EQSpellVisualEffectIndex = 0;
        public UInt32 SpellVisualID1 = 0;
        public UInt32 SpellVisualID2 = 0;
        public bool PlayerLearnableByClassTrainer = false; // Needed?
        public int MinimumPlayerLearnLevel = -1;
        public bool HasEffectBaseFormulaUsingSpellLevel = false;
        public int RequiredAreaIDs = -1;
        public bool IsGoodEffect = false;
        public UInt32 SchoolMask = 1;
        public UInt32 DispelType = 0;
        public UInt32 RequiredTotemID1 = 0;
        public UInt32 RequiredTotemID2 = 0;
        public UInt32 SpellFocusID = 0;
        public bool AllowCastInCombat = true;
        public List<Reagent> Reagents = new List<Reagent>();
        public int SkillLine = 0;
        public SpellEQSkillCategory EQSkillCategory = SpellEQSkillCategory.Unknown;
        public List<SpellEffectEQ> EQSpellEffects = new List<SpellEffectEQ>();
        public List<SpellEffectWOW> WOWSpellEffects = new List<SpellEffectWOW>();
        public UInt32 ManaCost = 0;
        public SpellEQTargetType EQTargetType = SpellEQTargetType.Single;
        public bool IsSelfCenteredAreaBreath = false; // Dragon breath
        public bool CanTargetBothFriendlyAndEnemy = false;
        public UInt32 TargetCreatureType = 0; // No specific creature type
        public bool CastOnCorpse = false;
        public Dictionary<ClassEQType, SpellLearnScrollProperties> LearnScrollPropertiesByEQClassType = new Dictionary<ClassEQType, SpellLearnScrollProperties>();
        public int HighestDirectHealAmountInSpellEffect = 0; // Used in spell priority calculations
        private string TargetDescriptionTextFragment = string.Empty;
        public bool BreakEffectOnNonAutoDirectDamage = false;
        public bool NoPartialImmunity = false;
        public int MaxCreatureTargetLevel = 0; // 0 = no limit
        public UInt32 DefenseType = 0; // 0 None, 1 Magic, 2 Melee, 3 Ranged
        public UInt32 PreventionType = 0; // 0 None, 1 Silence, 2 Pacify, 4 No Actions
        public int WeaponSpellItemEnchantmentDBCID = 0;
        public int WeaponItemEnchantProcSpellID = 0;
        public string WeaponItemEnchantSpellName = string.Empty;
        public UInt32 ProcChance = 101;
        public bool IsTransferEffectType = false;
        public int RecourseLinkEQSpellID = 0;
        public SpellTemplate? RecourseLinkSpellTemplate = null;
        public List<SpellTemplate> ChainedSpellTemplates = new List<SpellTemplate>();
        public bool ChainAppliesViaHitTrigger = false; // If a chained spell and this is 'true', it won't wear off when the initial spell aura fades
        public int ProcLinkEQSpellID = 0;
        public int WOWSpellIDCastOnMeleeAttacker = 0;
        public int ExcludeTargetAuraSpellID = 0; // If the target has this aura, the spell refuses to apply
        public bool HideCaster = false;
        public bool TriggersGlobalCooldown = true;
        public bool DoNotInterruptAutoActionsAndSwingTimers = false;
        public bool IsModelSizeChangeSpell = false;
        public bool IsChanneled = false;
        public bool ForceAsDebuff = false;
        public bool IsFarSight = false;
        public bool GenerateNoThreat = false;
        public bool IgnoreTargetRequirements = false;
        public bool ProcsOnMeleeAttacks = false;
        public SpellPet? SummonSpellPet = null;
        public int SummonPropertiesDBCID = 0;
        public int SummonCreatureTemplateID = 0;
        public bool IsBardSongAura = false;
        public bool IsCharmSpell = false;
        public bool HasAdditionalTickOnApply = false;
        public bool InterruptOnMovement = true;
        public bool InterruptOnSchoolLockdown = true;
        public bool InterruptOnPushback = true;
        public bool InterruptOnCast = true;
        public bool InterruptOnDamageTaken = false;
        public bool InterruptAuraOnMeleeAttack = false;
        public bool InterruptAuraOnSpellAttack = false;
        public bool InterruptAuraOnMount = false;
        public bool InterruptAuraOnCast = false;
        public bool InterruptAuraOnTakeDamage = false;
        public bool IsNegateIfCombat = false;
        public bool RemoveAuraWhenCasterCreatureInitsAgro = false;
        public bool PreventAuraClickOff = false;
        public bool AlwaysPersist = false;
        public SpellFocusBoostType FocusBoostType = SpellFocusBoostType.None;
        public bool IsFocusBoostableEffect = false;
        public bool IsToggleAura = false;
        public int PeriodicAuraWOWSpellID = 0;
        public int PeriodicAuraSpellRadius = 0;
        public SpellFailableType FailableType = SpellFailableType.None;
        public int EffectFailChancePercent = 0;
        public bool StunUsesBashKickChance = false;
        public int SpellIDCastOnTargetWhenStunLands = 0;
        public bool AuraStaysOnSecondaryClassSwitch = false;
        public bool ShowFocusBoostInDescriptionIfExists = false;
        public bool IsllusionSpellParent = false;
        public bool ForceHiddenFromDisplay = false;
        public bool CannotBeStolen = false;
        public SpellTemplate? IllusionSpellParent = null;
        public int MaleFormSpellTemplateID = 0;
        public int FemaleFormSpellTemplateID = 0;
        public bool CanMountWhileInForm = false;
        public bool AllowSpellPowerToInfluence = false;
        public bool InfluencedBySpellPower = false;
        public int EquippedItemClass = -1;
        public int EquippedItemSubClassMask = 0;
        public int EquippedItemInventoryTypeMask = 0;
        public bool UsesRangedWeaponSlot = false;
        public bool AllowInShapeshift = false;

        private List<SpellEffectBlock> _GroupedBaseSpellEffectBlocksForOutput = new List<SpellEffectBlock>();
        public List<SpellEffectBlock> GroupedBaseSpellEffectBlocksForOutput
        {
            get
            {
                if (_GroupedBaseSpellEffectBlocksForOutput.Count == 0)
                    GenerateOutputEffectBlocks();
                return _GroupedBaseSpellEffectBlocksForOutput;
            }
        }
        public List<List<SpellEffectBlock>> ItemWornSpellEffectBlockSets = new List<List<SpellEffectBlock>>();
        private List<SpellEffectBlock> _GroupedGoodProcSpellEffectBlocksForOutput = new List<SpellEffectBlock>();
        public List<SpellEffectBlock> GroupedGoodProcSpellEffectBlocksForOutput
        {
            get
            {
                if (_GroupedBaseSpellEffectBlocksForOutput.Count == 0)
                    GenerateOutputEffectBlocks();

                // WOWSpellIDProcAndGoodEffect can be assigned after the base blocks were generated, so build the good proc blocks if they are needed and missing
                GenerateMissingGoodProcOutputEffectBlocks();
                return _GroupedGoodProcSpellEffectBlocksForOutput;
            }
        }
        private List<List<SpellEffectBlock>> _GroupedClickySpellEffectBlocksForOutputBySpellParameters = new List<List<SpellEffectBlock>>();
        public List<List<SpellEffectBlock>> GroupedClickySpellEffectBlocksForOutputBySpellParameters
        {
            get
            {
                if (_GroupedBaseSpellEffectBlocksForOutput.Count == 0)
                    GenerateOutputEffectBlocks();

                // Clicky parameters can be registered after the base blocks were generated (worn effects on an earlier item forces generation), so fill in blocks for any parameters that don't have them yet
                if (_GroupedClickySpellEffectBlocksForOutputBySpellParameters.Count < ClickySpellParatemers.Count)
                    GenerateMissingClickyOutputEffectBlocks();
                return _GroupedClickySpellEffectBlocksForOutputBySpellParameters;
            }
        }

        public static Dictionary<int, SpellTemplate> GetSpellTemplatesByEQID()
        {
            lock (SpellTemplateLock)
            {
                if (SpellTemplatesByEQID.Count == 0)
                {
                    Logger.WriteError("GetSPellTemplatesByEQID called before LoadSpellTemplates");
                    return new Dictionary<int, SpellTemplate>();
                }
                return SpellTemplatesByEQID;
            }
        }

        public bool HasTeleportEffect()
        {
            foreach (SpellEffectEQ eqEffect in EQSpellEffects)
                if (eqEffect.EQEffectType == SpellEQEffectType.Gate || eqEffect.EQEffectType == SpellEQEffectType.Teleport || eqEffect.EQEffectType == SpellEQEffectType.Succor)
                    return true;
            return false;
        }

        public bool HasPetSummonEffect()
        {
            foreach (SpellEffectEQ eqEffect in EQSpellEffects)
                if (eqEffect.EQEffectType == SpellEQEffectType.SummonPet || eqEffect.EQEffectType == SpellEQEffectType.NecPet)
                    return true;
            return false;
        }

        public UInt32 GetCounterBasedDispelType()
        {
            // Detrimental spells with a positive counter are cured by the counter regardless of resist type
            foreach (SpellEffectEQ eqEffect in EQSpellEffects)
            {
                if (eqEffect.EQEffectType == SpellEQEffectType.DiseaseCounter && eqEffect.EQBaseValue > 0)
                    return 3; // Disease
                if (eqEffect.EQEffectType == SpellEQEffectType.PoisonCounter && eqEffect.EQBaseValue > 0)
                    return 4; // Poison
            }
            return 0;
        }

        public bool IsOffensiveDispell()
        {
            // Beneficial-flagged dispells (like Cancel Magic) can be aimed at enemies, so only self-only dispells are excluded
            if (EQTargetType == SpellEQTargetType.Self)
                return false;
            foreach (SpellEffectEQ eqEffect in EQSpellEffects)
                if (eqEffect.EQEffectType == SpellEQEffectType.CancelMagic)
                    return true;
            return false;
        }

        public static int GetCastTimeAfterConfigModsInMS(int castTimeInMS, bool isOffensiveDispell)
        {
            // Don't reduce anything below global cooldown
            if (castTimeInMS <= 500)
                return castTimeInMS;

            float castTime = Convert.ToSingle(castTimeInMS) * Configuration.SPELLS_CAST_TIME_MOD;

            // Never reduce below the floor, but casts starting at/below the floor keep their original cast time
            int configuredFloorInMS = Configuration.SPELLS_CAST_TIME_REDUCTION_FLOOR_IN_MS;
            if (isOffensiveDispell == true)
                configuredFloorInMS = Math.Max(configuredFloorInMS, Configuration.SPELLS_CAST_TIME_REDUCTION_FLOOR_OFFENSIVE_DISPELLS_IN_MS);
            float castTimeReductionFloor = Math.Min(castTimeInMS, configuredFloorInMS);
            castTime = Math.Max(castTime, castTimeReductionFloor);
            return (int)Math.Ceiling(castTime / 100f) * 100; // Round up for cleaner cast times
        }

        public ClickySpellParameters SetClickySpellParameters(int wowSpellID, int castTimeInMS, bool isForcedSelfOnly, int fixedLevel, bool isUsableWhileSilenced)
        {
            // Only make unique
            foreach (ClickySpellParameters clickySpell in this.ClickySpellParatemers)
            {
                if (clickySpell.CastTimeInMS == castTimeInMS && clickySpell.IsForcedSelfOnly == isForcedSelfOnly && clickySpell.FixedLevel == fixedLevel && clickySpell.IsUsableWhileSilenced == isUsableWhileSilenced)
                    return clickySpell;
            }
            ClickySpellParameters newClickySpell = new ClickySpellParameters();
            newClickySpell.WOWSpellID = wowSpellID;
            newClickySpell.IsForcedSelfOnly = isForcedSelfOnly;
            newClickySpell.CastTimeInMS = castTimeInMS;
            newClickySpell.FixedLevel = fixedLevel;
            newClickySpell.IsUsableWhileSilenced = isUsableWhileSilenced;
            this.ClickySpellParatemers.Add(newClickySpell);
            return newClickySpell;
        }

        public static void LoadSpellTemplates(SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID, Dictionary<string, ZoneProperties> zonePropertiesByShortName,
            ref Dictionary<int, CreatureTemplate> creatureTemplatesByEQID)
        {
            // Load the spell templates
            string spellTemplatesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpellTemplates.csv");
            Logger.WriteDebug(string.Concat("Loading spell templates via file '", spellTemplatesFile, "'"));
            List<Dictionary<string, string>> spellTemplateRows = FileTool.ReadAllRowsFromFileWithHeader(spellTemplatesFile, "|");

            // Spells cast by clicking an item need stacking rules even if they have no player spell category
            HashSet<int> itemClickSpellEQIDs = new HashSet<int>();
            foreach (ItemTemplate itemTemplate in itemTemplatesByEQDBID.Values)
                if (itemTemplate.EQClickSpellEffectID > 0)
                    itemClickSpellEQIDs.Add(itemTemplate.EQClickSpellEffectID);

            foreach (Dictionary<string, string> columns in spellTemplateRows)
            {
                // Skip invalid
                if (columns["enabled"] == "0")
                    continue;

                // Load the row
                SpellTemplate newSpellTemplate = new SpellTemplate();
                newSpellTemplate.EQSpellID = int.Parse(columns["eq_id"]);
                newSpellTemplate.WOWSpellID = int.Parse(columns["wow_id"]);
                newSpellTemplate.WOWSpellIDProcAndGoodEffect = int.Parse(columns["wow_good_proc_id"]);
                newSpellTemplate.Name = columns["name"];
                newSpellTemplate.SpellRange = Convert.ToInt32(float.Parse(columns["range"]) * Configuration.SPELLS_RANGE_MULTIPLIER);
                newSpellTemplate.EQAOERange = int.Parse(columns["aoerange"]);
                newSpellTemplate.SpellRadius = Convert.ToInt32(Convert.ToSingle(newSpellTemplate.EQAOERange) * Configuration.SPELLS_RANGE_MULTIPLIER);
                newSpellTemplate.Category = 0; // Temp / TODO: Figure out how/what to set here
                newSpellTemplate.RecourseLinkEQSpellID = int.Parse(columns["RecourseLink"]);

                // Recovery time (take highest)
                UInt32 eqCastRecoveryTime = UInt32.Parse(columns["cast_recovery_time"]);
                UInt32 eqInterruptRecoveryTime = UInt32.Parse(columns["interrupt_recovery_time"]);
                newSpellTemplate.RecoveryTimeInMS = Math.Max(eqCastRecoveryTime, eqInterruptRecoveryTime);

                // TODO: FacingCasterFlags

                // Spell effects
                for (int i = 1; i <= 12; i++)
                    PopulateEQSpellEffect(ref newSpellTemplate, i, columns);
                // Skip if there isn't an effect
                if (newSpellTemplate.EQSpellEffects.Count == 0)
                    continue;

                newSpellTemplate.IsGoodEffect = int.Parse(columns["goodEffect"]) != 0 ? true : false ; // 0 = detrimental, 1 = beneficial, 2 = beneficial group only.  Both 1 and 2 are non-detrimental.
                bool isDetrimental = !newSpellTemplate.IsGoodEffect;

                // Target type (needed before cast time so the offensive dispell floor can exclude self-only dispells)
                int eqTargetTypeID = int.Parse(columns["targettype"]);
                if (Enum.IsDefined(typeof(SpellEQTargetType), eqTargetTypeID) == true)
                    newSpellTemplate.EQTargetType = (SpellEQTargetType)eqTargetTypeID;

                // Cast time (teleport-adjacent and pet summoning spells keep their original cast time)
                newSpellTemplate.CastTimeBeforeModsInMS = int.Parse(columns["cast_time"]);
                if (newSpellTemplate.HasTeleportEffect() == true || newSpellTemplate.HasPetSummonEffect() == true)
                    newSpellTemplate.CastTimeInMS = newSpellTemplate.CastTimeBeforeModsInMS;
                else
                    newSpellTemplate.CastTimeInMS = GetCastTimeAfterConfigModsInMS(newSpellTemplate.CastTimeBeforeModsInMS, newSpellTemplate.IsOffensiveDispell());

                // Generic properties
                PopulateAllClassLearnScrollProperties(ref newSpellTemplate, columns);
                newSpellTemplate.ManaCost = Convert.ToUInt32(columns["mana"]);

                // Buff duration (if any)
                newSpellTemplate.EQBuffDurationInTicks = Convert.ToInt32(columns["buffduration"]);
                newSpellTemplate.EQBuffDurationFormula = Convert.ToInt32(columns["buffdurationformula"]);
                if (newSpellTemplate.EQBuffDurationFormula != 0 || newSpellTemplate.IsModelSizeChangeSpell == true)
                    newSpellTemplate.AuraDuration.CalculateAndSetAuraDuration(newSpellTemplate.MinimumPlayerLearnLevel, newSpellTemplate.EQBuffDurationFormula,
                        newSpellTemplate.EQBuffDurationInTicks, newSpellTemplate.IsModelSizeChangeSpell);

                // Focus / Bard / skill properties
                int skillID = int.Parse(columns["skill"]);
                newSpellTemplate.FocusBoostType = GetFocusBoostType(skillID);
                if ((skillID == 12 || skillID == 41 || skillID == 49 || skillID == 54 || skillID == 70) && newSpellTemplate.RecoveryTimeInMS == 0 && newSpellTemplate.EQBuffDurationInTicks != 0)
                    newSpellTemplate.IsBardSongAura = true;
                if (newSpellTemplate.IsBardSongAura == true)
                    newSpellTemplate.InterruptOnMovement = false; // Bards can play music while moving in EQ
                if (newSpellTemplate.FocusBoostType != SpellFocusBoostType.None && newSpellTemplate.IsBardSongAura == false)
                    newSpellTemplate.IsFocusBoostableEffect = true;
                
                if (skillID == 15 || skillID == 33 || skillID == 52)
                    newSpellTemplate.EQSkillCategory = SpellEQSkillCategory.Combat;
                else if (Enum.IsDefined(typeof(SpellEQSkillCategory), skillID) == true && skillID > 0)
                    newSpellTemplate.EQSkillCategory = (SpellEQSkillCategory)skillID;

                if (newSpellTemplate.EQSkillCategory != SpellEQSkillCategory.Unknown)
                    newSpellTemplate.SkillLine = SkillLineDBC.GetIDForSkillCatagory(newSpellTemplate.EQSkillCategory);

                // Icon
                int spellIconID = int.Parse(columns["icon"]);
                if (spellIconID >= 2500)
                    newSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(spellIconID - 2500);

                // Visual
                newSpellTemplate.EQSpellVisualEffectIndex = int.Parse(columns["SpellVisualEffectIndex"]);
                if (newSpellTemplate.EQSpellVisualEffectIndex >= 61 && newSpellTemplate.EQSpellVisualEffectIndex <= 64 // These are dragonbreath
                    && newSpellTemplate.CastTimeInMS < Configuration.SPELL_EFFECT_DRAGONBREATH_CAST_TIME_IN_MS)
                    newSpellTemplate.CastTimeInMS = Configuration.SPELL_EFFECT_DRAGONBREATH_CAST_TIME_IN_MS;
                if (newSpellTemplate.EQSpellVisualEffectIndex >= 0 && newSpellTemplate.EQSpellVisualEffectIndex < 255)
                {
                    SpellVisualType spellVisualType = SpellVisualType.Beneficial;
                    if (newSpellTemplate.IsBardSongAura == true)
                        spellVisualType = SpellVisualType.BardSong;
                    else if (isDetrimental == true)
                        spellVisualType = SpellVisualType.Detrimental;
                    newSpellTemplate.SpellVisualID1 = Convert.ToUInt32(SpellVisual.GetSpellVisual(newSpellTemplate.EQSpellVisualEffectIndex, spellVisualType).SpellVisualDBCID);
                }

                // School class
                int resistType = int.Parse(columns["resisttype"]);
                newSpellTemplate.SchoolMask = GetSchoolMaskForResistType(resistType);
                newSpellTemplate.DispelType = GetDispelTypeForResistType(resistType, isDetrimental, newSpellTemplate.AuraDuration.MaxDurationInMS > 0);
                if (isDetrimental == true && newSpellTemplate.AuraDuration.MaxDurationInMS > 0)
                {
                    UInt32 counterBasedDispelType = newSpellTemplate.GetCounterBasedDispelType();
                    if (counterBasedDispelType != 0)
                        newSpellTemplate.DispelType = counterBasedDispelType;
                }

                // Reagents (Components).  Bard instruments are required to play a song but not consumed, so they map to a
                // totem category requirement instead of a consumed reagent.  Everything else is a normal reagent.
                AddComponentAsReagentOrInstrumentRequirement(ref newSpellTemplate, int.Parse(columns["components1"]), int.Parse(columns["component_counts1"]), itemTemplatesByEQDBID);
                AddComponentAsReagentOrInstrumentRequirement(ref newSpellTemplate, int.Parse(columns["components2"]), int.Parse(columns["component_counts2"]), itemTemplatesByEQDBID);

                // Set defensive properties
                newSpellTemplate.DefenseType = 1; // Magic
                newSpellTemplate.PreventionType = 1; // Silence

                // Modify DoT and crowd control durations for non-bard sounds
                if (isDetrimental == true && newSpellTemplate.IsBardSongAura == false)
                    ApplyDurationModsToDoTAndCrowdControlDurations(ref newSpellTemplate);

                // Get targets and convert the spell effects
                List<SpellTemplate> effectGeneratedSpellTemplates = new List<SpellTemplate>();
                if (newSpellTemplate.IsBardSongAura == true)
                {
                    ConvertEQSpellEffectsIntoWOWEffectsForBardSongAura(ref newSpellTemplate, newSpellTemplate.SchoolMask, newSpellTemplate.AuraDuration,
                        eqTargetTypeID, newSpellTemplate.SpellRadius, newSpellTemplate.SpellRange, isDetrimental, newSpellTemplate.FocusBoostType, ref effectGeneratedSpellTemplates);
                }
                else
                {
                    List<SpellWOWTargetType> targets = CalculateTargets(ref newSpellTemplate, eqTargetTypeID, isDetrimental, newSpellTemplate.EQSpellEffects, newSpellTemplate.SpellRange, newSpellTemplate.SpellRadius);
                    string teleportZoneOrPetTypeName = columns["teleport_zone"];
                    ConvertEQSpellEffectsIntoWOWEffects(ref newSpellTemplate, newSpellTemplate.SchoolMask, newSpellTemplate.AuraDuration,
                        newSpellTemplate.CastTimeInMS, targets, newSpellTemplate.SpellRadiusDBCID, itemTemplatesByEQDBID, isDetrimental, teleportZoneOrPetTypeName, 
                        zonePropertiesByShortName, ref creatureTemplatesByEQID, ref effectGeneratedSpellTemplates);
                    newSpellTemplate.ShowFocusBoostInDescriptionIfExists = true;
                }

                // If there is no wow effect, skip it
                if (newSpellTemplate.WOWSpellEffects.Count == 0)
                    continue;

                // Scale mana cost to follow any change in total direct healing/damage caused by the cast time mod
                ApplyManaCostScalingForDirectOutputChange(ref newSpellTemplate);

                // Stacking rules
                bool isItemClickSpell = itemClickSpellEQIDs.Contains(newSpellTemplate.EQSpellID);
                SetAuraStackRule(ref newSpellTemplate, int.Parse(columns["spell_category"]), newSpellTemplate.IsBardSongAura, isDetrimental, isItemClickSpell);
                for (int i = 0; i < effectGeneratedSpellTemplates.Count; i++)
                {
                    SpellTemplate effectGeneratedSpellTemplate = effectGeneratedSpellTemplates[i];
                    SetAuraStackRule(ref effectGeneratedSpellTemplate, int.Parse(columns["spell_category"]), effectGeneratedSpellTemplate.IsBardSongAura, isDetrimental, isItemClickSpell);
                }

                // Add it, and any effect generated ones
                SpellTemplatesByEQID.Add(newSpellTemplate.EQSpellID, newSpellTemplate);
                foreach (SpellTemplate effectGeneratedSpellTemplate in effectGeneratedSpellTemplates)
                    SpellTemplatesByEQID.Add(effectGeneratedSpellTemplate.EQSpellID, effectGeneratedSpellTemplate);
            }

            // Set any post load grooming
            foreach (int eqSpellID in SpellTemplatesByEQID.Keys)
            {
                SpellTemplate spellTemplate = SpellTemplatesByEQID[eqSpellID];

                // Pull recourse spell template
                SpellTemplate? recourseSpellTemplate = null;
                if (spellTemplate.RecourseLinkEQSpellID != 0)
                {
                    if (SpellTemplatesByEQID.ContainsKey(spellTemplate.RecourseLinkEQSpellID) == false)
                        Logger.WriteError("Spell with eqid ", eqSpellID.ToString(), " has a linked recourse spell eqid ", spellTemplate.RecourseLinkEQSpellID.ToString(), " which did not exist");
                    else
                    {
                        recourseSpellTemplate = SpellTemplatesByEQID[spellTemplate.RecourseLinkEQSpellID];
                        spellTemplate.RecourseLinkSpellTemplate = recourseSpellTemplate;
                    }
                }

                // Pull proc link template
                SpellTemplate? procLinkSpellTemplate = null;
                if (spellTemplate.ProcLinkEQSpellID != 0)
                {
                    if (SpellTemplatesByEQID.ContainsKey(spellTemplate.ProcLinkEQSpellID) == false)
                        Logger.WriteError("Spell with eqid ", eqSpellID.ToString(), " has a linked proc spell eqid ", spellTemplate.ProcLinkEQSpellID.ToString(), " which did not exist");
                    else
                    {
                        procLinkSpellTemplate = SpellTemplatesByEQID[spellTemplate.ProcLinkEQSpellID];
                        foreach (SpellEffectWOW spellEffectWOW in spellTemplate.WOWSpellEffects)
                            if (spellEffectWOW.EffectTriggerSpell == spellTemplate.ProcLinkEQSpellID)
                                spellEffectWOW.EffectTriggerSpell = SpellTemplatesByEQID[spellTemplate.ProcLinkEQSpellID].WOWSpellID;
                    }
                }

                // Replicate form effects to child illusion effects and they must inherit the prarent's learn level so effec
                // values are right.  If this doesn't happen, things like wolf form multiplies the bonus
                if (spellTemplate.IllusionSpellParent != null)
                {
                    spellTemplate.MinimumPlayerLearnLevel = spellTemplate.IllusionSpellParent.MinimumPlayerLearnLevel;
                    foreach (SpellEffectWOW spellEffect in spellTemplate.IllusionSpellParent.WOWSpellEffects)
                    {
                        // Dummy is used for the form change trigger, so skip that
                        if (spellEffect.EffectType == SpellWOWEffectType.Dummy)
                            continue;
                        spellTemplate.WOWSpellEffects.Add(spellEffect.Clone());
                    }

                    // Add proper stacking rules for illusion spells
                    foreach (int parentStackGroupID in spellTemplate.IllusionSpellParent.SpellGroupStackingIDs)
                        if (spellTemplate.SpellGroupStackingIDs.Contains(parentStackGroupID) == false)
                            spellTemplate.SpellGroupStackingIDs.Add(parentStackGroupID);
                }

                // Set the spell and aura descriptions
                SetActionAndAuraDescriptions(ref spellTemplate, recourseSpellTemplate, procLinkSpellTemplate);
            }

            // Pull out all but the dummy to avoid too many icons showing for illusions
            foreach (SpellTemplate spellTemplate in SpellTemplatesByEQID.Values)
                if (spellTemplate.IsllusionSpellParent == true)
                    spellTemplate.WOWSpellEffects.RemoveAll(IsNonDummySpellEffect);
        }

        private static bool IsNonDummySpellEffect(SpellEffectWOW spellEffect)
        {
            return spellEffect.EffectType != SpellWOWEffectType.Dummy;
        }

        public void CalculateSpellPowerCoefficientsForBlock(SpellEffectBlock effectBlock, out float directCoefficient, out float dotCoefficient)
        {
            // Direct coefficient
            float standardCastTimeMS = Convert.ToSingle(Configuration.SPELL_SPELL_POWER_STANDARD_CAST_TIME_IN_MS);
            float clampedCastTimeMS = Math.Clamp(Convert.ToSingle(CastTimeInMS),
                Convert.ToSingle(Configuration.SPELL_SPELL_POWER_MIN_CAST_TIME_IN_MS), standardCastTimeMS);
            directCoefficient = clampedCastTimeMS / standardCastTimeMS;

            // DoT calculation.  Spread out coefficient over full duration (and it applies to channeled too, but I don't think there are any EQ channeled...)
            dotCoefficient = directCoefficient; // Fallback for blocks with no periodic effect (the core never reads it there)
            int tickPeriodMS = GetLongestSpellPowerPeriodicTickInMSForBlock(effectBlock);
            if (tickPeriodMS > 0)
            {
                if (IsChanneled == true)
                {
                    int tickCount = 1;
                    if (AuraDuration.MaxDurationInMS > 0)
                        tickCount = Math.Max(1, AuraDuration.MaxDurationInMS / tickPeriodMS);
                    dotCoefficient = 1.0f / Convert.ToSingle(tickCount);
                }
                else
                    dotCoefficient = Convert.ToSingle(tickPeriodMS) / Convert.ToSingle(Configuration.SPELL_SPELL_POWER_DOT_FULL_DURATION_IN_MS);
            }

            // Area spells influence at a reduced rate
            if (BlockIsAreaEffect(effectBlock) == true)
            {
                directCoefficient *= Configuration.SPELL_SPELL_POWER_AOE_MULTIPLIER;
                dotCoefficient *= Configuration.SPELL_SPELL_POWER_AOE_MULTIPLIER;
            }

            // Apply multiplier
            float influenceMultiplier = Configuration.SPELL_DEFAULT_SPELL_POWER_INFLUANCE_MOD;
            if (IsBardSongAura == true)
                influenceMultiplier = Configuration.SPELL_BARD_SPELL_POWER_INFLUANCE_MOD;
            directCoefficient *= influenceMultiplier;
            dotCoefficient *= influenceMultiplier;

            // Additional low-level mod (applies to both bard and default): a level-1 spell is reduced by the low-level
            // mod, ramping linearly back to 1.0 (no reduction) at/above the phaseout level
            float lowLevelMod = GetLowLevelSpellPowerMod();
            directCoefficient *= lowLevelMod;
            dotCoefficient *= lowLevelMod;
        }

        private float GetLowLevelSpellPowerMod()
        {
            float levelOneMod = Configuration.SPELL_SPELL_POWER_LOW_LEVEL_MOD;
            int phaseoutLevel = Configuration.SPELL_SPELL_POWER_LOW_LEVEL_MOD_PHASEOUT_LEVEL;

            // Disabled / no ramp
            if (levelOneMod == 1.0f || phaseoutLevel <= 1)
                return 1.0f;

            // Use the spell's lowest learn level as its level (unknown levels are treated as level 1)
            int spellLevel = MinimumPlayerLearnLevel;
            if (spellLevel < 1)
                spellLevel = 1;

            // Fully phased out at/above the phaseout level
            if (spellLevel >= phaseoutLevel)
                return 1.0f;

            // Linearly interpolate from level 1 to phaseout, in which this becomes 1.0f (no reduction)
            float phaseProgress = Convert.ToSingle(spellLevel - 1) / Convert.ToSingle(phaseoutLevel - 1);
            return levelOneMod + (phaseProgress * (1.0f - levelOneMod));
        }

        public int GetMinimumTargetLevel()
        {
            // TAKP has the buff restriction of beneficial buffs above a level threshold can only land on players "(learn level / 2) + 15" or higher
            int spellLevelThreshold = Configuration.SPELL_BUFF_MIN_TARGET_LEVEL_RESTRICTION_SPELL_LEVEL_THRESHOLD;
            if (spellLevelThreshold <= 0)
                return 0;
            if (IsGoodEffect == false || IsBardSongAura == true)
                return 0;
            if (AuraDuration.MaxDurationInMS == 0)
                return 0;
            if (MinimumPlayerLearnLevel <= spellLevelThreshold)
                return 0;
            return (MinimumPlayerLearnLevel / 2) + 15;
        }

        private static int GetLongestSpellPowerPeriodicTickInMSForBlock(SpellEffectBlock effectBlock)
        {
            int highestTickPeriodMS = 0;
            foreach (SpellEffectWOW spellEffect in effectBlock.SpellEffects)
            {
                if (IsSpellPowerPeriodicAuraType(spellEffect.EffectAuraType) == false)
                    continue;
                int tickPeriodMS = Convert.ToInt32(spellEffect.EffectAuraPeriod);
                if (tickPeriodMS > highestTickPeriodMS)
                    highestTickPeriodMS = tickPeriodMS;
            }
            return highestTickPeriodMS;
        }

        private static bool IsSpellPowerPeriodicAuraType(SpellWOWAuraType auraType)
        {
            if (auraType == SpellWOWAuraType.PeriodicDamage)
                return true;
            if (auraType == SpellWOWAuraType.PeriodicHeal)
                return true;
            if (auraType == SpellWOWAuraType.PeriodicLeech)
                return true;
            return false;
        }

        private static bool BlockIsAreaEffect(SpellEffectBlock effectBlock)
        {
            foreach (SpellEffectWOW spellEffect in effectBlock.SpellEffects)
            {
                if (IsAreaTargetType(spellEffect.ImplicitTargetA) == true)
                    return true;
                if (IsAreaTargetType(spellEffect.ImplicitTargetB) == true)
                    return true;
            }
            return false;
        }

        private static bool IsAreaTargetType(SpellWOWTargetType targetType)
        {
            if (targetType == SpellWOWTargetType.UnitSourceAreaEnemy)
                return true;
            if (targetType == SpellWOWTargetType.UnitDestinationAreaEnemy)
                return true;
            if (targetType == SpellWOWTargetType.UnitCasterAreaParty)
                return true;
            if (targetType == SpellWOWTargetType.UnitDestinationAreaAlly)
                return true;
            if (targetType == SpellWOWTargetType.UnitDestinationAreaParty)
                return true;
            return false;
        }

        public static void GenerateItemEnchantSpellIfNotCreated(string itemName, int procSpellEQID, int enchantSpellWOWID, out SpellTemplate? enchantSpellTemplate)
        {
            lock (SpellTemplateLock)
            {
                enchantSpellTemplate = null;

                // Don't do anything if it already exists
                foreach (SpellTemplate spellTemplate in SpellTemplatesByEQID.Values)
                {
                    if (spellTemplate.WOWSpellID == enchantSpellWOWID)
                        return;
                }
                if (SpellTemplatesByEQID.ContainsKey(procSpellEQID) == false)
                {
                    Logger.WriteDebug("Failed to create an item enchantement since triggered eq spell ID of ", procSpellEQID.ToString() ," did not exist");
                    return;
                }
                SpellTemplate procSpellTemplate = SpellTemplatesByEQID[procSpellEQID];

                // Generate a description
                StringBuilder descriptionSB = new StringBuilder();
                descriptionSB.Append("Coats a weapon with poison that lasts for ");
                descriptionSB.Append(GetTimeTextFromSeconds(Configuration.SPELL_ENCHANT_ROGUE_POISON_ENCHANT_DURATION_ON_WEAPON_TIME_IN_SECONDS));
                descriptionSB.Append(" with each strike having a ");
                descriptionSB.Append(Configuration.SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE);
                descriptionSB.Append("% chance of applying the following: ");
                descriptionSB.Append(procSpellTemplate.Description);

                // Generate an enchant ID
                int enchantID = IDGenerationTool.GenerateID("SpellItemEnchantmentID", itemName, procSpellTemplate.WOWSpellID.ToString());

                // Create the new spell
                SpellTemplate enchantSpell = new SpellTemplate();
                enchantSpell.Name = itemName;
                enchantSpell.WOWSpellID = enchantSpellWOWID;
                enchantSpell.Description = descriptionSB.ToString();
                enchantSpell.WeaponSpellItemEnchantmentDBCID = enchantID;
                enchantSpell.WeaponItemEnchantProcSpellID = procSpellTemplate.WOWSpellID;
                enchantSpell.WeaponItemEnchantSpellName = itemName;
                enchantSpell.WOWSpellEffects.Add(new SpellEffectWOW(SpellWOWEffectType.EnchantItemTemporary, 0, 0, 0, 1, 0, enchantID, 0));
                enchantSpell.ProcChance = Convert.ToUInt32(Configuration.SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE);
                enchantSpell.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(procSpellTemplate.SpellIconID);
                enchantSpell.SpellVisualID1 = Convert.ToUInt32(Configuration.SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_APPLYING_VISUAL_ID);
                enchantSpell.CastTimeInMS = Configuration.SPELL_ENCHANT_ROGUE_POISON_APPLY_TIME_IN_MS;

                enchantSpellTemplate = enchantSpell;
            }
        }

        public static void GenerateFocusSpellIfNotCreated(string itemName, int itemIconID, ItemFocusType focusType, int focusValue, out SpellTemplate? focusSpellTemplate,
            out bool isNewSpell)
        {
            lock (SpellTemplateLock)
            {
                focusSpellTemplate = null;
                (string, int, ItemFocusType, int) focusSpellKey = (itemName, itemIconID, focusType, focusValue);

                // Don't do anything if it already exists
                if (SpellTemplatesByFocusTypeAndValue.ContainsKey(focusSpellKey) == true)
                {
                    focusSpellTemplate =  SpellTemplatesByFocusTypeAndValue[focusSpellKey];
                    isNewSpell = false;
                    return;
                }
                isNewSpell = true;

                // Generate an aura description
                string description = string.Empty;
                SpellDummyType spellDummyType = SpellDummyType.None;
                switch (focusType)
                {
                    case ItemFocusType.BardBrassInstruments:
                        {
                            spellDummyType = SpellDummyType.BardFocusBrass;
                            description = string.Concat("Increases potency of songs using brass instruments by ", focusValue, "%");
                        } break;
                    case ItemFocusType.BardStringedInstruments:
                        {
                            spellDummyType = SpellDummyType.BardFocusString;
                            description = string.Concat("Increases potency of songs using string instruments by ", focusValue, "%");
                        } break;
                    case ItemFocusType.BardWindInstruments:
                        {
                            spellDummyType = SpellDummyType.BardFocusWind;
                            description = string.Concat("Increases potency of songs using wind instruments by ", focusValue, "%");
                        } break;
                    case ItemFocusType.BardPercussionInstruments:
                        {
                            spellDummyType = SpellDummyType.BardFocusPercussion;
                            description = string.Concat("Increases potency of songs using percussion instruments by ", focusValue, "%");
                        } break;
                    case ItemFocusType.BardAll:
                        {
                            spellDummyType = SpellDummyType.BardFocusAll;
                            description = string.Concat("Increases potency of all songs by ", focusValue, "%");
                        } break;
                    default:
                        {
                            Logger.WriteError("GenerateFocusSpellIfNotCreated failed, unhandled focusType ", focusType.ToString());
                            return;
                        }
                }

                focusSpellTemplate = new SpellTemplate();
                focusSpellTemplate.Name = string.Concat(itemName, " Focus");
                focusSpellTemplate.WOWSpellID = IDGenerationTool.GenerateID("SpellID", "focus", itemName.ToString(), itemIconID.ToString(), ((int)focusType).ToString(), focusValue.ToString());
                focusSpellTemplate.EQSpellID = GenerateUniqueEQSpellID();
                focusSpellTemplate.Description = description;
                focusSpellTemplate.AuraDescription = description;
                focusSpellTemplate.WOWSpellEffects.Add(new SpellEffectWOW(SpellWOWEffectType.ApplyAura, SpellWOWAuraType.Dummy, 0, 0, 0, 1, (int)spellDummyType, focusValue));
                focusSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForItemIconID(itemIconID);
                focusSpellTemplate.SpellVisualID1 = 0; // No visual
                focusSpellTemplate.PreventAuraClickOff = true;
                focusSpellTemplate.AlwaysPersist = true;
                focusSpellTemplate.AuraDuration = new SpellDuration();
                focusSpellTemplate.AuraDuration.IsInfinite = true;
                SpellTemplatesByFocusTypeAndValue.Add(focusSpellKey, focusSpellTemplate);
            }
        }

        private static void PopulateAllClassLearnScrollProperties(ref SpellTemplate spellTemplate, Dictionary<string, string> rowColumns)
        {
            // Potentially one for each class
            foreach (ClassEQType eqClass in Enum.GetValues(typeof(ClassEQType)))
            {
                if (eqClass == ClassEQType.All || eqClass == ClassEQType.None)
                    continue;

                // Pull details
                string className = eqClass.ToString().ToLower();
                int curClassLearnlevel = int.Parse(rowColumns[string.Concat(className, "_learn_level")]);
                if (curClassLearnlevel != -1 && curClassLearnlevel <= 100)
                {
                    SpellLearnScrollProperties spellLearnScrollProperties = new SpellLearnScrollProperties();
                    spellLearnScrollProperties.LearnLevel = curClassLearnlevel;
                    spellLearnScrollProperties.WOWItemTemplateID = int.Parse(rowColumns[string.Concat(className, "_learn_wowitemid")]);
                    spellTemplate.LearnScrollPropertiesByEQClassType.Add(eqClass, spellLearnScrollProperties);

                    // Also save it as the lowest level possible to learn for future formulas
                    if (spellTemplate.MinimumPlayerLearnLevel <= 0 || spellTemplate.MinimumPlayerLearnLevel > spellLearnScrollProperties.LearnLevel)
                        spellTemplate.MinimumPlayerLearnLevel = spellLearnScrollProperties.LearnLevel;
                }
            }
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

        public static UInt32 GetDispelTypeForResistType(int eqResistType, bool isDetrimental, bool hasSpellDuration)
        {
            // TODO: Honor 'nodispel'?
            if (hasSpellDuration == false)
                return 0;
            if (isDetrimental == false)
                return 1;
            switch (eqResistType)
            {
                case 1: return 1; // EQ Magic => MAGIC
                case 2: return 1; // EQ Fire => MAGIC
                case 3: return 1; // EQ Cold => MAGIC
                case 4: return 4; // EQ Poison => POISON
                case 5: return 3; // EQ Disease => DISEASE
                default: return 0; // None
            }
        }

        private static List<SpellWOWTargetType> CalculateTargets(ref SpellTemplate spellTemplate, int eqTargetTypeID, bool isDetrimental, List<SpellEffectEQ> eqSpellEffects, int range, int spellRadius)
        {
            List<SpellWOWTargetType> spellWOWTargetTypes = new List<SpellWOWTargetType>();

            // Capture the EQ Target type
            if (Enum.IsDefined(typeof(SpellEQTargetType), eqTargetTypeID) == false)
            {
                Logger.WriteError("SpellTemplate with EQID ", spellTemplate.EQSpellID.ToString(), " has unknown target type of ", eqTargetTypeID.ToString());
                spellWOWTargetTypes.Add(SpellWOWTargetType.UnitCaster);
                return spellWOWTargetTypes;
            }
            else
                spellTemplate.EQTargetType = (SpellEQTargetType)eqTargetTypeID;

            // Some spell effects allow targeting both friendly and enemy
            foreach (SpellEffectEQ effect in eqSpellEffects)
            {
                if (effect.EQEffectType == SpellEQEffectType.CancelMagic)
                    spellTemplate.CanTargetBothFriendlyAndEnemy = true;
                else if (effect.EQEffectType == SpellEQEffectType.BindSight)
                    spellTemplate.CanTargetBothFriendlyAndEnemy = true;
            }

            // Map the EQ target type to WOW
            switch (spellTemplate.EQTargetType)
            {
                case SpellEQTargetType.LineOfSight:
                    {
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single visible enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetAlly);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single visible friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.GroupV1:
                case SpellEQTargetType.GroupV2:
                    {
                        spellWOWTargetTypes.Add(SpellWOWTargetType.UnitCasterAreaParty);
                        if (spellRadius > 0)
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets the whole party within ", spellRadius, " yards around the caster");
                        else
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets the whole party");
                    }
                    break;
                case SpellEQTargetType.PointBlankAreaOfEffect:
                case SpellEQTargetType.AreaOfEffectUndead: // TODO, may not be needed.  See "Words of the Undead King"
                    {
                        string targetTypeDescriptionFragment = "";
                        if (spellTemplate.EQTargetType == SpellEQTargetType.AreaOfEffectUndead)
                        {
                            spellTemplate.TargetCreatureType = 32; // Undead, 0x0020
                            targetTypeDescriptionFragment = "undead ";
                        }

                        if (isDetrimental == true)
                        {
                            // Referenced from Blast Wave
                            spellWOWTargetTypes.Add(SpellWOWTargetType.SourceCaster);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitSourceAreaEnemy);
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets ", targetTypeDescriptionFragment, "enemies within ", spellRadius, " yards around the caster");
                        }
                        if (isDetrimental == false)
                        {
                            // Referenced from Circle of healing
                            spellWOWTargetTypes.Add(SpellWOWTargetType.SourceCaster);
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets ", targetTypeDescriptionFragment, "friendly units within ", spellRadius, " yards around the caster");
                        }

                    }
                    break;
                case SpellEQTargetType.Single:
                    {
                        if (spellTemplate.CanTargetBothFriendlyAndEnemy == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.DestinationTargetAny);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single enemy or friendly unit";
                        }
                        else if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetAlly);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.Self:
                    {
                        spellWOWTargetTypes.Add(SpellWOWTargetType.UnitCaster);
                        spellTemplate.TargetDescriptionTextFragment = "Targets self";
                    }
                    break;
                //case SpellEQTargetType.TargetedAreaOfEffectLifeTap: // Not used anywhere?
                case SpellEQTargetType.TargetedAreaOfEffect:
                    {
                        if (isDetrimental == true)
                        {
                            // Dragon breath (visual index 61-64) must NOT have a primary unit target
                            if (spellTemplate.EQSpellVisualEffectIndex >= 61 && spellTemplate.EQSpellVisualEffectIndex <= 64)
                            {
                                spellWOWTargetTypes.Add(SpellWOWTargetType.SourceCaster);
                                spellWOWTargetTypes.Add(SpellWOWTargetType.UnitSourceAreaEnemy);
                                spellTemplate.IsSelfCenteredAreaBreath = true;
                            }
                            else
                            {
                                spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                                spellWOWTargetTypes.Add(SpellWOWTargetType.UnitDestinationAreaEnemy);
                            }
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets an enemy and other enemies within ", spellRadius, " yards around the target");
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetAlly);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitDestinationAreaAlly);
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets a friendly unit and other friendly units within ", spellRadius, " yards around the target");
                        }
                    }
                    break;
                case SpellEQTargetType.Animal:
                    {
                        spellTemplate.TargetCreatureType = 1; // Beast, 0x0001
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single beast enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetAlly); // "lull" put into this for now
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single beast friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.Undead:
                    {
                        spellTemplate.TargetCreatureType = 32; // Undead, 0x0020
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single undead enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetAlly); // "lull" and heal undead put into this for now
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single undead friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.Summoned:
                    {
                        spellTemplate.TargetCreatureType = 8; // Elemental, 0x0008
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single elemental enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetAlly);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single elemental friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.AreaOfEffectSummoned:
                    {
                        spellTemplate.TargetCreatureType = 8; // Elemental, 0x0008
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitDestinationAreaEnemy);
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets an elemental enemy and other elemental enemies within ", spellRadius, " yards around the target");
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetAlly);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitDestinationAreaAlly);
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets an elemental friendly unit and other elemental friendly units within ", spellRadius, " yards around the target");
                        }
                    } break;
                case SpellEQTargetType.LifeTap:
                    {
                        spellTemplate.IsTransferEffectType = true;
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetAlly);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.Pet:
                    {
                        spellWOWTargetTypes.Add(SpellWOWTargetType.UnitPet);
                        spellTemplate.TargetDescriptionTextFragment = "Targets your pet";
                    }
                    break;
                case SpellEQTargetType.Corpse:
                    {
                        spellWOWTargetTypes.Add(SpellWOWTargetType.None);
                        spellTemplate.TargetDescriptionTextFragment = "Targets a friendly corpse";
                        spellTemplate.CastOnCorpse = true;
                    }
                    break;
                case SpellEQTargetType.Plant:
                    {
                        // Using "elemental" for now
                        spellTemplate.TargetCreatureType = 8; // Elemental, 0x0008
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single elemental enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.DestinationTargetAny); // "lull" and heal undead put into this for now
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single elemental friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.UberGiants:
                    {
                        spellTemplate.TargetCreatureType = 16; // Giant, 0x0010
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single giant enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.DestinationTargetAny); // "lull" and heal put into this for now
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single giant friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.UberDragons:
                    {
                        spellTemplate.TargetCreatureType = 2; // Dragonkin, 0x0002
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single dragonkin enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.DestinationTargetAny); // "lull" and heal put into this for now
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single dragonkin friendly unit";
                        }
                    }
                    break;
                default:
                    {
                        Logger.WriteError("Unable to map eq target type ", spellTemplate.EQTargetType.ToString(), " to WOW target type");
                        spellWOWTargetTypes.Add(SpellWOWTargetType.UnitCaster);
                        spellTemplate.TargetDescriptionTextFragment = "Targets self";
                    }
                    break;
            }

            return spellWOWTargetTypes;
        }

        private static SpellFocusBoostType GetFocusBoostType(int skillID)
        {
            // Pull out the bard songs from skill string
            switch (skillID)
            {
                case 12: return SpellFocusBoostType.BardBrassInstruments;
                case 41: return SpellFocusBoostType.BardSinging;
                case 49: return SpellFocusBoostType.BardStringedInstruments;
                case 54: return SpellFocusBoostType.BardWindInstruments;
                case 70: return SpellFocusBoostType.BardPercussionInstruments;
                default: break;
            }
            return SpellFocusBoostType.None;
        }

        private static void AddComponentAsReagentOrInstrumentRequirement(ref SpellTemplate spellTemplate, int componentEQID, int componentCount,
            SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID)
        {
            if (componentEQID <= 0 || componentCount <= 0)
                return;
            if (itemTemplatesByEQDBID.ContainsKey(componentEQID) == false)
            {
                Logger.WriteError("For Spell WOWID ", spellTemplate.WOWSpellID.ToString(), " a reagent could not be found with EQID ", componentEQID.ToString());
                return;
            }

            ItemTemplate componentItem = itemTemplatesByEQDBID[componentEQID];
            UInt32 instrumentTotemID = GetInstrumentRequiredTotemID(componentItem.FocusType);
            if (instrumentTotemID != 0)
            {
                // Required (not consumed) - any instrument of the matching totem category satisfies it
                if (spellTemplate.RequiredTotemID1 == 0)
                    spellTemplate.RequiredTotemID1 = instrumentTotemID;
                else if (spellTemplate.RequiredTotemID2 == 0)
                    spellTemplate.RequiredTotemID2 = instrumentTotemID;
                return;
            }

            spellTemplate.Reagents.Add(new Reagent(componentItem.WOWEntryID, componentCount));
        }

        private static UInt32 GetInstrumentRequiredTotemID(ItemFocusType focusType)
        {
            switch (focusType)
            {
                case ItemFocusType.BardWindInstruments: return Convert.ToUInt32(Configuration.ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_WIND);
                case ItemFocusType.BardStringedInstruments: return Convert.ToUInt32(Configuration.ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_STRING);
                case ItemFocusType.BardBrassInstruments: return Convert.ToUInt32(Configuration.ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_BRASS);
                case ItemFocusType.BardPercussionInstruments: return Convert.ToUInt32(Configuration.ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_PERCUSSION);
                default: return 0;
            }
        }

        private static void ApplyDurationModsToDoTAndCrowdControlDurations(ref SpellTemplate spellTemplate)
        {
            // Stun durations are defined in the the 'effect value', so they must be modified here
            if (Configuration.SPELLS_CROWD_CONTROL_DURATION_MOD != 1f)
            {
                foreach (SpellEffectEQ eqEffect in spellTemplate.EQSpellEffects)
                    if (eqEffect.EQEffectType == SpellEQEffectType.Stun && eqEffect.EQBaseValue > 1)
                        eqEffect.EQBaseValue = Convert.ToInt32(Convert.ToSingle(eqEffect.EQBaseValue) * Configuration.SPELLS_CROWD_CONTROL_DURATION_MOD);
            }

            if (spellTemplate.AuraDuration.IsInfinite == true || spellTemplate.AuraDuration.MaxDurationInMS <= 0)
                return;
            bool hasPeriodicDamage = false;
            bool hasCrowdControl = false;
            foreach (SpellEffectEQ eqEffect in spellTemplate.EQSpellEffects)
            {
                if (eqEffect.EQEffectType == SpellEQEffectType.CurrentHitPoints && eqEffect.EQBaseValue < 0)
                    hasPeriodicDamage = true;
                else if (eqEffect.EQEffectType == SpellEQEffectType.Charm || eqEffect.EQEffectType == SpellEQEffectType.Fear ||
                        eqEffect.EQEffectType == SpellEQEffectType.Mez || eqEffect.EQEffectType == SpellEQEffectType.Root)
                    hasCrowdControl = true;
            }

            // DoT gets priority over crowd control mods in order to keep the relative damage output the same
            if (hasPeriodicDamage == true)
            {
                if (Configuration.SPELLS_DOT_TIME_DURATION_MOD == 1f)
                    return;

                // Round up to the next periodic tick, otherwise we'll drop the last tick in rounding sometimes
                int tickPeriodInMS = Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW * 1000;
                int maxDurationBeforeModInMS = spellTemplate.AuraDuration.MaxDurationInMS;
                spellTemplate.AuraDuration.ScaleDuration(Configuration.SPELLS_DOT_TIME_DURATION_MOD, tickPeriodInMS);
                spellTemplate.PeriodicDamageDurationCompensationMod = Convert.ToSingle(maxDurationBeforeModInMS) / Convert.ToSingle(spellTemplate.AuraDuration.MaxDurationInMS);
            }
            else if (hasCrowdControl == true)
            {
                if (Configuration.SPELLS_CROWD_CONTROL_DURATION_MOD == 1f)
                    return;
                spellTemplate.AuraDuration.ScaleDuration(Configuration.SPELLS_CROWD_CONTROL_DURATION_MOD, 0);
            }
        }

        private static void ApplyManaCostScalingForDirectOutputChange(ref SpellTemplate spellTemplate)
        {
            // Only direct (non-periodic) heals and damage have their total output scaled by the cast time mod;
            if (spellTemplate.CastTimeBeforeModsInMS <= 0 || spellTemplate.CastTimeInMS == spellTemplate.CastTimeBeforeModsInMS)
                return;
            if (spellTemplate.ManaCost == 0)
                return;

            bool hasDirectHealOrDamage = false;
            foreach (SpellEffectWOW wowEffect in spellTemplate.WOWSpellEffects)
            {
                if (wowEffect.EffectAuraType != SpellWOWAuraType.None)
                    continue; // Periodic (DoT/HoT) effects carry an aura type and keep their total output
                if (wowEffect.EffectType == SpellWOWEffectType.Heal || wowEffect.EffectType == SpellWOWEffectType.SchoolDamage ||
                    wowEffect.EffectType == SpellWOWEffectType.HealthLeech)
                {
                    hasDirectHealOrDamage = true;
                    break;
                }
            }
            if (hasDirectHealOrDamage == false)
                return;

            // Total direct output scaled by the same ratio the direct amounts did (see SpellEffectWOW cast time scaling)
            float outputMod = Convert.ToSingle(spellTemplate.CastTimeInMS) / Convert.ToSingle(spellTemplate.CastTimeBeforeModsInMS);
            spellTemplate.ManaCost = Convert.ToUInt32(Math.Max(1.0, Math.Round(Convert.ToDouble(spellTemplate.ManaCost) * outputMod)));
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
            curEffect.EQEffectSlot = slotID;
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
                    default: curEffect.EQBaseValueFormulaType = SpellEQBaseValueFormulaType.UnknownUseBaseOrMaxWhicheverHigher; break;
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

            // Teleports and succors use fixed columns for values
            if (curEffect.EQEffectType == SpellEQEffectType.Teleport || curEffect.EQEffectType == SpellEQEffectType.Succor)
            {
                curEffect.EQTelePosition = new Vector3(float.Parse(rowColumns["effect_base_value1"]),
                    float.Parse(rowColumns["effect_base_value2"]),
                    float.Parse(rowColumns["effect_base_value3"]));
                curEffect.EQTeleHeading = int.Parse(rowColumns["effect_base_value4"]);
            }

            // Model change spells need to be identified for a default effect duration
            if (curEffect.EQEffectType == SpellEQEffectType.ModelSize)
                spellTemplate.IsModelSizeChangeSpell = true;

            // Add it
            spellTemplate.EQSpellEffects.Add(curEffect);
        }

        private static void ConvertEQSpellEffectsIntoWOWEffectsForBardSongAura(ref SpellTemplate spellTemplate, UInt32 schoolMask, SpellDuration auraDuration,
            int eqTargetType, int spellRadius, int spellRange, bool isDetrimental, SpellFocusBoostType focusBoostType, ref List<SpellTemplate> effectGeneratedSpellTemplates)
        {
            // Some spell effects allow targeting both friendly and enemy
            foreach (SpellEffectEQ effect in spellTemplate.EQSpellEffects)
            {
                if (effect.EQEffectType == SpellEQEffectType.CancelMagic)
                    spellTemplate.CanTargetBothFriendlyAndEnemy = true;
                else if (effect.EQEffectType == SpellEQEffectType.BindSight)
                    spellTemplate.CanTargetBothFriendlyAndEnemy = true;
            }

            // Use targets to determine the dummy type
            SpellDummyType dummyType = SpellDummyType.None;
            List<SpellWOWTargetType> effectedSpellTargets = new List<SpellWOWTargetType>();
            int effectSpellRange = spellRange;
            switch (eqTargetType)
            {
                case 3: // GroupV1
                case 41: // GroupV2
                    {
                        effectedSpellTargets.Add(SpellWOWTargetType.UnitTargetAlly);
                        dummyType = SpellDummyType.BardSongFriendlyParty;
                        spellTemplate.TargetDescriptionTextFragment = string.Concat("Applies the effect every ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds to all party members within ", spellRadius, " yards");
                        effectSpellRange = spellRadius;
                    } break;
                case 4: // PointBlankAreaOfEffect
                    {
                        effectedSpellTargets.Add(SpellWOWTargetType.UnitTargetEnemy);
                        dummyType = SpellDummyType.BardSongEnemyArea;
                        spellTemplate.TargetDescriptionTextFragment = string.Concat("Applies the effect every ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds to all enemies within ", spellRadius, " yards");
                        effectSpellRange = spellRadius;
                    } break;
                case 5: // Single
                    {
                        if (spellTemplate.CanTargetBothFriendlyAndEnemy == true)
                        {
                            effectedSpellTargets.Add(SpellWOWTargetType.DestinationTargetAny);
                            dummyType = SpellDummyType.BardSongAnySingle;
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Applies the effect every ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds to a single target within ", spellRange, " yards");
                        }
                        else if (isDetrimental == true)
                        {
                            effectedSpellTargets.Add(SpellWOWTargetType.UnitTargetEnemy);
                            dummyType = SpellDummyType.BardSongEnemySingle;
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Applies the effect every ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds to a single enemy target within ", spellRange, " yards");
                        }
                        else
                        {
                            effectedSpellTargets.Add(SpellWOWTargetType.UnitTargetAlly);
                            dummyType = SpellDummyType.BardSongFriendlySingle;
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Applies the effect every ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds to a single friendly target within ", spellRange, " yards");
                        }                        
                    } break;
                case 6: // Self
                    {
                        effectedSpellTargets.Add(SpellWOWTargetType.UnitCaster);
                        dummyType = SpellDummyType.BardSongSelf;
                        spellTemplate.TargetDescriptionTextFragment = string.Concat("Applies the effect every ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds to self");
                    } break;
                default:
                    {
                        Logger.WriteError("ConvertEQSpellEffectsIntoWOWEffectsForBardSongAura error, unhandled eqTargetType of ", eqTargetType.ToString());
                    } break;
            }

            // Generate the effect spell, and move many of the properties over to it
            SpellTemplate effectGeneratedSpellTemplate = new SpellTemplate();
            effectGeneratedSpellTemplate.Name = string.Concat(spellTemplate.Name, " Effect");
            effectGeneratedSpellTemplate.WOWSpellID = IDGenerationTool.GenerateID("SpellID", "bardsongeffect", spellTemplate.EQSpellID.ToString());
            effectGeneratedSpellTemplate.EQSpellID = GenerateUniqueEQSpellID();
            effectGeneratedSpellTemplate.SpellIconID = spellTemplate.SpellIconID;
            effectGeneratedSpellTemplate.DoNotInterruptAutoActionsAndSwingTimers = true;
            effectGeneratedSpellTemplate.TriggersGlobalCooldown = false;
            effectGeneratedSpellTemplate.EQSpellEffects = spellTemplate.EQSpellEffects;
            effectGeneratedSpellTemplate.MinimumPlayerLearnLevel = spellTemplate.MinimumPlayerLearnLevel;
            effectGeneratedSpellTemplate.SpellRadius = 0;
            effectGeneratedSpellTemplate.SpellRange = effectSpellRange;
            effectGeneratedSpellTemplate.IsFocusBoostableEffect = true;
            effectGeneratedSpellTemplate.FocusBoostType = focusBoostType;
            effectGeneratedSpellTemplate.AuraDuration = auraDuration;
            Dictionary<int, CreatureTemplate> discardCreatureTemplates = new Dictionary<int, CreatureTemplate>();
            ConvertEQSpellEffectsIntoWOWEffects(ref effectGeneratedSpellTemplate, schoolMask, effectGeneratedSpellTemplate.AuraDuration, 0, effectedSpellTargets,
                spellTemplate.SpellRadiusDBCID, new SortedDictionary<int, ItemTemplate>(), isDetrimental, string.Empty, new Dictionary<string, ZoneProperties>(),
                ref discardCreatureTemplates, ref effectGeneratedSpellTemplates);
            if (spellTemplate.EQSpellVisualEffectIndex >= 0)
                effectGeneratedSpellTemplate.SpellVisualID1 = Convert.ToUInt32(SpellVisual.GetSpellVisual(spellTemplate.EQSpellVisualEffectIndex, SpellVisualType.BardTick).SpellVisualDBCID);
            SetActionAndAuraDescriptions(ref effectGeneratedSpellTemplate, null, null);

            // Update properties for this song aura
            SpellEffectWOW auraEffect = new SpellEffectWOW();
            auraEffect.EffectType = SpellWOWEffectType.ApplyAura;
            auraEffect.EffectAuraType = SpellWOWAuraType.PeriodicDummy;
            auraEffect.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
            auraEffect.EffectAuraPeriod = (Convert.ToUInt32(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW) * 1000) + Convert.ToUInt32(Configuration.SPELL_PERIODIC_BARD_TICK_BUFFER_IN_MS);
            auraEffect.EffectMiscValueA = Convert.ToInt32(dummyType);
            StringBuilder descriptionSB = new StringBuilder();
            descriptionSB.Append(effectGeneratedSpellTemplate.Description);
            if (descriptionSB.Length > 0)
                descriptionSB[0] = char.ToLower(descriptionSB[0]);
            if (descriptionSB.ToString().EndsWith("."))
                descriptionSB.Length--;
            string description = string.Concat("every ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds ", descriptionSB.ToString());
            auraEffect.ActionDescription = description;
            auraEffect.AuraDescription = description;

            spellTemplate.WOWSpellEffects.Add(auraEffect);
            if (Configuration.SPELL_EFFECT_BARD_ADDITIONAL_TICK_ON_CAST == true)
                spellTemplate.HasAdditionalTickOnApply = true;
            spellTemplate.InterruptOnMovement = false;
            spellTemplate.InterruptOnPushback = false;
            spellTemplate.AuraDuration = new SpellDuration();
            spellTemplate.AuraDuration.IsInfinite = true;
            spellTemplate.IsToggleAura = true;
            spellTemplate.PeriodicAuraWOWSpellID = effectGeneratedSpellTemplate.WOWSpellID;
            spellTemplate.PeriodicAuraSpellRadius = spellRadius;
            spellTemplate.ShowFocusBoostInDescriptionIfExists = true;

            effectGeneratedSpellTemplates.Add(effectGeneratedSpellTemplate);
        }

        private static void ConvertEQSpellEffectsIntoWOWEffects(ref SpellTemplate spellTemplate, UInt32 schoolMask, SpellDuration auraDuration, 
            int spellCastTimeInMS, List<SpellWOWTargetType> targets, int spellRadiusIndex, SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID,
            bool isDetrimental, string teleportZoneOrPetTypeName, Dictionary<string, ZoneProperties> zonePropertiesByShortName, 
            ref Dictionary<int, CreatureTemplate> creatureTemplatesByEQID, ref List<SpellTemplate> effectGeneratedSpellTemplates)
        {
            bool hasSpellDuration = (auraDuration.IsInfinite || auraDuration.BaseDurationInMS > 0);

            // Process all spell effects
            List<SpellEffectWOW> newSpellEffects = new List<SpellEffectWOW>();
            foreach (SpellEffectEQ eqEffect in spellTemplate.EQSpellEffects)
            {
                // This is temporary logic until "Splurt" is implemented
                // Re-enable "Greenmist Recourse" if another solution isn't found
                if (eqEffect.EQFormulaTypeValue == 122 || eqEffect.EQFormulaTypeValue == 120) // 122 is splurt, 120 is something else though....
                {
                    if (isDetrimental)
                        eqEffect.EQBaseValue *= -1;
                }

                if (spellTemplate.IsTransferEffectType == true)
                {
                    SpellWOWTargetType? otherTarget = null;
                    switch (eqEffect.EQEffectType)
                    {
                        case SpellEQEffectType.Charisma:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                else
                                    Logger.WriteError("Transfer spell effect type Charisma with base value > 0 not implemented.");
                            } break;
                        case SpellEQEffectType.CurrentMana:
                            {
                                if (hasSpellDuration == true)
                                {
                                    Logger.WriteError("Unimplemented Mana Leach Aura effect for EQSpellID of ", spellTemplate.EQSpellID.ToString());
                                    continue;
                                }
                                if (eqEffect.EQBaseValue > 0)
                                {
                                    Logger.WriteError("Unimplemented Mana Leach Reverse effect for EQSpellID of ", spellTemplate.EQSpellID.ToString());
                                    continue;
                                }
                                int preFormulaEffectAmount = Math.Abs(eqEffect.EQBaseValue);
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ManaUpDirect", SpellEffectWOWConversionScaleType.CastTime, castTimeBeforeModsInMS: spellTemplate.CastTimeBeforeModsInMS);
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.PowerDrain;
                                newSpellEffectWOW.EffectMiscValueA = 0; // Power Type = Mana
                                newSpellEffectWOW.EffectMultipleValue = 1;
                                newSpellEffectWOW.ActionDescription = string.Concat("drain ", newSpellEffectWOW.GetFormattedEffectActionString(false), " mana from the target and add it to your own");
                                spellTemplate.InfluencedBySpellPower = true;
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.CurrentHitPoints:
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
                                if (hasSpellDuration == false)
                                {
                                    SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                    if (eqEffect.EQBaseValue > 0)
                                    {
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HealDirectHPS", SpellEffectWOWConversionScaleType.CastTime, castTimeBeforeModsInMS: spellTemplate.CastTimeBeforeModsInMS);
                                        newSpellEffectWOW.EffectType = SpellWOWEffectType.Heal;
                                        newSpellEffectWOW.ActionDescription = string.Concat("transfer ", newSpellEffectWOW.GetFormattedEffectActionString(false), " of your life to the target");
                                        newSpellEffects.Add(newSpellEffectWOW);

                                        // Add a second for damage to self
                                        SpellEffectWOW newSpellEffectWOW2 = newSpellEffectWOW.Clone();
                                        newSpellEffectWOW2.EffectType = SpellWOWEffectType.SchoolDamage;
                                        newSpellEffectWOW2.ActionDescription = string.Empty;
                                        newSpellEffectWOW2.AuraDescription = string.Empty;
                                        newSpellEffects.Add(newSpellEffectWOW2);
                                        otherTarget = SpellWOWTargetType.UnitCaster;
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageDirectDPS", SpellEffectWOWConversionScaleType.CastTime, castTimeBeforeModsInMS: spellTemplate.CastTimeBeforeModsInMS);
                                        newSpellEffectWOW.EffectType = SpellWOWEffectType.HealthLeech;
                                        if (elementalSchoolName.Length > 0)
                                            newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " ", elementalSchoolName, " damage and return it as life to yourself");
                                        else
                                            newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " damage and return it as life to yourself");
                                        newSpellEffectWOW.EffectMultipleValue = 1;
                                        newSpellEffects.Add(newSpellEffectWOW);
                                    }
                                }
                                else
                                {
                                    SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                    newSpellEffectWOW.EffectAuraPeriod = Convert.ToUInt32(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW) * 1000;
                                    if (eqEffect.EQBaseValue > 0)
                                    {
                                        Logger.WriteWarning("Unimplemented Life Leach Aura effect for EQSpellID of ", spellTemplate.EQSpellID.ToString());
                                        continue;
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicLeech;
                                        if (elementalSchoolName.Length > 0)
                                        {
                                            newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageOverTimeDPS", SpellEffectWOWConversionScaleType.Periodic, spellTemplate.PeriodicDamageDurationCompensationMod);
                                            newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds and return it as life to yourself");
                                            newSpellEffectWOW.SetAuraDescription("suffering", false, " ", string.Concat(" ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds and return it as life to yourself"));
                                        }
                                        else
                                        {
                                            newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageOverTimeDPS", SpellEffectWOWConversionScaleType.Periodic, spellTemplate.PeriodicDamageDurationCompensationMod);
                                            newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds and return it as life to yourself");
                                            newSpellEffectWOW.SetAuraDescription("suffering", false, " ", string.Concat(" damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds and return it as life to yourself"));
                                        }
                                    }
                                    newSpellEffectWOW.EffectMultipleValue = 1;
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
                                spellTemplate.InfluencedBySpellPower = true;
                            } break;
                        case SpellEQEffectType.Strength:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                                newSpellEffectWOW.EffectMiscValueA = 0; // Strength
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    Logger.WriteWarning("Unimplemented strength leach aura effect for EQSpellID of ", spellTemplate.EQSpellID.ToString());
                                    continue;
                                }
                               
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "StrengthDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("steal ", newSpellEffectWOW.GetFormattedEffectActionString(false), " strength from the target");
                                newSpellEffectWOW.SetAuraDescription("", false, "", "strength stolen by the caster");
                                newSpellEffects.Add(newSpellEffectWOW);

                                // Make a second for the buff on the caster
                                SpellEffectWOW casterStrBuffEffect = newSpellEffectWOW.Clone();
                                casterStrBuffEffect.Invert();
                                casterStrBuffEffect.ClearDescriptions();
                                newSpellEffects.Add(casterStrBuffEffect);
                            } break;
                        case SpellEQEffectType.Agility:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                                newSpellEffectWOW.EffectMiscValueA = 1; // Strength
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    Logger.WriteError("Unimplemented agility leach aura effect for EQSpellID of ", spellTemplate.EQSpellID.ToString());
                                    continue;
                                }

                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "AgilityDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("steal ", newSpellEffectWOW.GetFormattedEffectActionString(false), " agility from the target");
                                newSpellEffectWOW.SetAuraDescription("", false, "", "agility stolen by the caster");
                                newSpellEffects.Add(newSpellEffectWOW);

                                // Make a second for the buff on the caster
                                SpellEffectWOW casterAglBuffEffect = newSpellEffectWOW.Clone();
                                casterAglBuffEffect.Invert();
                                casterAglBuffEffect.ClearDescriptions();
                                newSpellEffects.Add(casterAglBuffEffect);
                            } break;
                        case SpellEQEffectType.ArmorClass:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                                newSpellEffectWOW.EffectMiscValueA = 1; // Armor
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    Logger.WriteWarning("Unimplemented armor leach aura effect for EQSpellID of ", spellTemplate.EQSpellID.ToString());
                                    continue;
                                }

                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArmorClassDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("steal ", newSpellEffectWOW.GetFormattedEffectActionString(false), " armor from the target");
                                newSpellEffectWOW.SetAuraDescription("", false, "", "armor stolen by the caster");
                                newSpellEffects.Add(newSpellEffectWOW);

                                // Make a second for the buff on the caster
                                SpellEffectWOW casterArmorBuffEffect = newSpellEffectWOW.Clone();
                                casterArmorBuffEffect.Invert();
                                casterArmorBuffEffect.ClearDescriptions();
                                newSpellEffects.Add(casterArmorBuffEffect);
                            } break;
                        default:
                            {
                                Logger.WriteError("Unhandled Transfer type SpellTemplate EQEffectType of ", eqEffect.EQEffectType.ToString(), " for eq spell id ", spellTemplate.EQSpellID.ToString());
                                continue;
                            }
                    }
                    if (newSpellEffects.Count > 0)
                    {
                        newSpellEffects[0].ImplicitTargetA = targets[0];
                        if (newSpellEffects.Count > 1 && otherTarget != null)
                            newSpellEffects[1].ImplicitTargetA = (SpellWOWTargetType)otherTarget;
                        foreach (SpellEffectWOW newSpellEffect in newSpellEffects)
                            spellTemplate.WOWSpellEffects.Add(newSpellEffect);
                    }
                }
                else
                {
                    switch (eqEffect.EQEffectType)
                    {
                        case SpellEQEffectType.CurrentHitPoints: // Fallthrough
                        case SpellEQEffectType.CurrentHitPointsOnce:
                        case SpellEQEffectType.HealOverTime:
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
                                if (hasSpellDuration == false || eqEffect.EQEffectType == SpellEQEffectType.CurrentHitPointsOnce)
                                {
                                    SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                    if (eqEffect.EQBaseValue > 0)
                                    {
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HealDirectHPS", SpellEffectWOWConversionScaleType.CastTime, castTimeBeforeModsInMS: spellTemplate.CastTimeBeforeModsInMS);
                                        newSpellEffectWOW.EffectType = SpellWOWEffectType.Heal;
                                        newSpellEffectWOW.ActionDescription = string.Concat("heal for ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                        spellTemplate.HighestDirectHealAmountInSpellEffect = Math.Max(spellTemplate.HighestDirectHealAmountInSpellEffect, newSpellEffectWOW.CalcEffectHighLevelValue);
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageDirectDPS", SpellEffectWOWConversionScaleType.CastTime, castTimeBeforeModsInMS: spellTemplate.CastTimeBeforeModsInMS);
                                        newSpellEffectWOW.EffectType = SpellWOWEffectType.SchoolDamage;
                                        if (elementalSchoolName.Length > 0)
                                            newSpellEffectWOW.ActionDescription = string.Concat("strike for ", newSpellEffectWOW.GetFormattedEffectActionString(false), " ", elementalSchoolName, " damage");
                                        else
                                            newSpellEffectWOW.ActionDescription = string.Concat("strike for ", newSpellEffectWOW.GetFormattedEffectActionString(false), " damage");
                                    }
                                    spellTemplate.InfluencedBySpellPower = true;
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
                                else
                                {
                                    SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                    newSpellEffectWOW.EffectAuraPeriod = Convert.ToUInt32(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW) * 1000;
                                    spellTemplate.InfluencedBySpellPower = true;
                                    if (eqEffect.EQBaseValue > 0)
                                    {
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HealOverTimeHPS", SpellEffectWOWConversionScaleType.Periodic);
                                        newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicHeal;
                                        newSpellEffectWOW.ActionDescription = string.Concat("regenerate ", newSpellEffectWOW.GetFormattedEffectActionString(false), " health per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                        newSpellEffectWOW.SetAuraDescription("regenerating", false, " ", string.Concat(" health per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds"));
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicDamage;
                                        if (elementalSchoolName.Length > 0)
                                        {
                                            newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageOverTimeDPS", SpellEffectWOWConversionScaleType.Periodic, spellTemplate.PeriodicDamageDurationCompensationMod);
                                            newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                            newSpellEffectWOW.SetAuraDescription("suffering", false, " ", string.Concat(" ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds"));
                                        }
                                        else
                                        {
                                            newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageOverTimeDPS", SpellEffectWOWConversionScaleType.Periodic, spellTemplate.PeriodicDamageDurationCompensationMod);
                                            newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                            newSpellEffectWOW.SetAuraDescription("suffering", false, " ", string.Concat(" damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds"));
                                        }
                                    }
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
                            } break;
                        case SpellEQEffectType.CompleteHeal:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;

                                // Heals 7500 x the base value when it lands (Lookup SE_CompleteHeal in TAKP)
                                int preFormulaEffectAmount = Math.Abs(eqEffect.EQBaseValue) * 7500;
                                SpellEffectWOW healSpellEffectWOW = new SpellEffectWOW();
                                healSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                healSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, 0, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HealDirectHPS", SpellEffectWOWConversionScaleType.CastTime, castTimeBeforeModsInMS: spellTemplate.CastTimeBeforeModsInMS);
                                healSpellEffectWOW.EffectType = SpellWOWEffectType.Heal;
                                healSpellEffectWOW.ActionDescription = string.Concat("heal for ", healSpellEffectWOW.GetFormattedEffectActionString(false));
                                spellTemplate.HighestDirectHealAmountInSpellEffect = Math.Max(spellTemplate.HighestDirectHealAmountInSpellEffect, healSpellEffectWOW.CalcEffectHighLevelValue);
                                spellTemplate.InfluencedBySpellPower = true;
                                newSpellEffects.Add(healSpellEffectWOW);

                                // Residual buff that regenerates 1 mana per tick and blocks another complete heal until it wears off
                                SpellTemplate effectGeneratedSpellTemplate = new SpellTemplate();
                                effectGeneratedSpellTemplate.Name = string.Concat(spellTemplate.Name, " Residual");
                                effectGeneratedSpellTemplate.WOWSpellID = IDGenerationTool.GenerateID("SpellID", "completehealresidual", spellTemplate.EQSpellID.ToString(), eqEffect.EQEffectSlot.ToString());
                                effectGeneratedSpellTemplate.EQSpellID = GenerateUniqueEQSpellID();
                                effectGeneratedSpellTemplate.SpellIconID = spellTemplate.SpellIconID;
                                effectGeneratedSpellTemplate.DoNotInterruptAutoActionsAndSwingTimers = true;
                                effectGeneratedSpellTemplate.TriggersGlobalCooldown = false;
                                effectGeneratedSpellTemplate.SpellRange = spellTemplate.SpellRange;
                                effectGeneratedSpellTemplate.AuraDuration = new SpellDuration();
                                effectGeneratedSpellTemplate.AuraDuration.SetFixedDuration(auraDuration.MaxDurationInMS);

                                SpellEffectWOW residualSpellEffectWOW = new SpellEffectWOW();
                                residualSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                residualSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicEnergize;
                                residualSpellEffectWOW.EffectMiscValueA = 0; // Power Type = Mana
                                residualSpellEffectWOW.EffectAuraPeriod = Convert.ToUInt32(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW) * 1000;
                                residualSpellEffectWOW.SetEffectAmountValues(1, 0, spellTemplate.MinimumPlayerLearnLevel, SpellEQBaseValueFormulaType.BaseValue, spellCastTimeInMS, "ManaUpOverTimeMPS", SpellEffectWOWConversionScaleType.Periodic);
                                residualSpellEffectWOW.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
                                residualSpellEffectWOW.ActionDescription = string.Concat("recover ", residualSpellEffectWOW.GetFormattedEffectActionString(false), " mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds and block additional complete heals from similar items");
                                residualSpellEffectWOW.SetAuraDescription("recovering", false, " ", string.Concat(" mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds and blocking additional complete heals from similar items"));
                                effectGeneratedSpellTemplate.WOWSpellEffects.Add(residualSpellEffectWOW);

                                // Prevent application if the buff is on
                                effectGeneratedSpellTemplates.Add(effectGeneratedSpellTemplate);
                                spellTemplate.ChainedSpellTemplates.Add(effectGeneratedSpellTemplate);
                                spellTemplate.ExcludeTargetAuraSpellID = effectGeneratedSpellTemplate.WOWSpellID;
                                spellTemplate.PreventAuraClickOff = true;
                                
                                // Lock the duration
                                spellTemplate.AuraDuration.SetFixedDuration(spellTemplate.AuraDuration.MaxDurationInMS);
                            } break;
                        case SpellEQEffectType.CurrentMana:
                        case SpellEQEffectType.CurrentManaOnce:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                spellTemplate.InfluencedBySpellPower = true;

                                int preFormulaEffectAmount = Math.Abs(eqEffect.EQBaseValue);
                                if (hasSpellDuration == false || eqEffect.EQEffectType == SpellEQEffectType.CurrentManaOnce)
                                {
                                    SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                    newSpellEffectWOW.EffectMiscValueA = 0; // Power Type = Mana
                                    if (eqEffect.EQBaseValue > 0)
                                    {
                                        newSpellEffectWOW.EffectType = SpellWOWEffectType.Energize;
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ManaUpDirect", SpellEffectWOWConversionScaleType.None);
                                        newSpellEffectWOW.ActionDescription = string.Concat("restore ", newSpellEffectWOW.GetFormattedEffectActionString(false), " mana");
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.EffectType = SpellWOWEffectType.PowerBurn;
                                        newSpellEffectWOW.EffectMultipleValue = 0;
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ManaDownDirect", SpellEffectWOWConversionScaleType.None);
                                        newSpellEffectWOW.ActionDescription = string.Concat("reduce ", newSpellEffectWOW.GetFormattedEffectActionString(false), " mana");
                                    }
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
                                else
                                {
                                    SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                    newSpellEffectWOW.EffectMiscValueA = 0; // Power Type = Mana
                                    newSpellEffectWOW.EffectAuraPeriod = Convert.ToUInt32(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW) * 1000;
                                    if (eqEffect.EQBaseValue > 0)
                                    {
                                        newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicEnergize;
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ManaUpOverTimeMPS", SpellEffectWOWConversionScaleType.Periodic);
                                        newSpellEffectWOW.ActionDescription = string.Concat("recover ", newSpellEffectWOW.GetFormattedEffectActionString(false), " mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                        newSpellEffectWOW.SetAuraDescription("recovering", false, " ", string.Concat(" mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds"));
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PowerBurn;
                                        newSpellEffectWOW.EffectMultipleValue = 0;
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ManaDownOvertimeMPS", SpellEffectWOWConversionScaleType.Periodic);
                                        newSpellEffectWOW.ActionDescription = string.Concat("reduce ", newSpellEffectWOW.GetFormattedEffectActionString(false), " mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                        newSpellEffectWOW.SetAuraDescription("reducing", false, " ", string.Concat(" mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds"));
                                    }
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
                            } break;
                        case SpellEQEffectType.ArmorClass:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                                newSpellEffectWOW.EffectMiscValueA = 1; // Armor
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArmorClassBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase armor by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("armor increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArmorClassDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease armor by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("armor decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Attack:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModAttackPower;
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "AttackBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase attack power by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("attack power increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "AttackDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease attack power by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("attack power decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);

                                // Add a second for ranged attack power
                                SpellEffectWOW newSpellEffectWOW2 = newSpellEffectWOW.Clone();
                                newSpellEffectWOW2.ClearDescriptions();
                                newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModRangedAttackPower;
                                newSpellEffects.Add(newSpellEffectWOW2);
                            } break;
                        case SpellEQEffectType.MovementSpeed:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModIncreaseSpeed;
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase non-mounted movement speed by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.SetAuraDescription("non-mounted movement speed increased", true, " by ", "");
                                }
                                else
                                {
                                    int eqMaxValue = eqEffect.EQMaxValue;
                                    if (eqMaxValue == 0 || eqMaxValue < Configuration.SPELLS_SLOWEST_MOVE_SPEED_EFFECT_VALUE)
                                        eqMaxValue = Configuration.SPELLS_SLOWEST_MOVE_SPEED_EFFECT_VALUE;
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModDecreaseSpeed;
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease movement speed by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.SetAuraDescription("movement speed decreased", true, " by ", "");
                                    newSpellEffectWOW.EffectMechanic = SpellMechanicType.Snared;
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.TotalHP:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModMaximumHealth;
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "MaxHPBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase maximum health by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("maximum health increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease maximum health by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("maximum health decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Strength:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                                newSpellEffectWOW.EffectMiscValueA = 0; // Strength
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "StrengthBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase strength by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("strength increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "StrengthDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease strength by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("strength decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
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
                                    }
                                }
                                if (ignoreAsAglEffectExistsAndIsStronger == true)
                                    continue;

                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                                newSpellEffectWOW.EffectMiscValueA = 1; // Agility
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "AgilityBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase agility by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("agility increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "AgilityDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease agility by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("agility decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Stamina:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                                newSpellEffectWOW.EffectMiscValueA = 2; // Stamina
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "StaminaBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase stamina by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("stamina increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "StaminaDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease stamina by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("stamina decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Intelligence:
                        case SpellEQEffectType.TotalMana:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;

                                if (eqEffect.EQBaseValue > 0)
                                {
                                    SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                    if (eqEffect.EQEffectType == SpellEQEffectType.TotalMana)
                                        newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "MaxManaToIntBuff", SpellEffectWOWConversionScaleType.None);
                                    else
                                        newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "IntellectBuff", SpellEffectWOWConversionScaleType.None);

                                    // EQ Intelligence and EQ Total Mana are both mapped to WOW int, so use the higher of the two and reuse if one exists
                                    bool ignoreAsIntEffectExistsAndIsStronger = false;
                                    foreach (SpellEffectWOW wowEffect in spellTemplate.WOWSpellEffects)
                                    {
                                        if (wowEffect.EffectAuraType == SpellWOWAuraType.ModStat && wowEffect.EffectMiscValueA == 3)
                                            if (wowEffect.EffectBasePoints > 0 && wowEffect.EffectBasePoints >= newSpellEffectWOW.EffectBasePoints)
                                                ignoreAsIntEffectExistsAndIsStronger = true;
                                    }
                                    if (ignoreAsIntEffectExistsAndIsStronger == true)
                                        continue;

                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                                    newSpellEffectWOW.EffectMiscValueA = 3; // Intellect                                
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase intellect by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("intellect increased", false, " by ", "");
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
                                else
                                {
                                    SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                    if (eqEffect.EQEffectType == SpellEQEffectType.TotalMana)
                                        newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "MaxManaToIntDebuff", SpellEffectWOWConversionScaleType.None);
                                    else
                                        newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "IntellectDebuff", SpellEffectWOWConversionScaleType.None);

                                    // EQ Intelligence and EQ Total Mana are both mapped to WOW int, so use the higher of the two and reuse if one exists
                                    bool ignoreAsIntEffectExistsAndIsStronger = false;
                                    foreach (SpellEffectWOW wowEffect in spellTemplate.WOWSpellEffects)
                                    {
                                        if (wowEffect.EffectAuraType == SpellWOWAuraType.ModStat && wowEffect.EffectMiscValueA == 3)
                                        {
                                            if (wowEffect.EffectBasePoints < 0 && wowEffect.EffectBasePoints <= newSpellEffectWOW.EffectBasePoints)
                                                ignoreAsIntEffectExistsAndIsStronger = true;
                                        }
                                    }
                                    if (ignoreAsIntEffectExistsAndIsStronger == true)
                                        continue;

                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                                    newSpellEffectWOW.EffectMiscValueA = 3; // Intellect
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease intellect by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("intellect decreased", false, " by ", "");
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
                            } break;
                        case SpellEQEffectType.Wisdom:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                                newSpellEffectWOW.EffectMiscValueA = 4; // Spirit
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "SpiritBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase spirit by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("spirit increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "SpiritDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease spirit by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("spirit decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Charisma:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModHitChance;
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HitPctBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase hit chance by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.SetAuraDescription("hit chance increased", true, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HitPctDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease hit chance by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.SetAuraDescription("hit chance decreased", true, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);

                                // Add a second for spell hit, since ModHitChance only covers melee and ranged
                                SpellEffectWOW newSpellEffectWOW2 = newSpellEffectWOW.Clone();
                                newSpellEffectWOW2.ClearDescriptions();
                                newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModSpellHitChance;
                                newSpellEffects.Add(newSpellEffectWOW2);
                            } break;
                        case SpellEQEffectType.AttackSpeed:
                        case SpellEQEffectType.AttackSpeed2:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModMeleeHaste;

                                // Baseline for attack speed is 100, so above that is increase and below that is decrease
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue - 100, eqEffect.EQMaxValue - 100, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "", SpellEffectWOWConversionScaleType.None);
                                if (newSpellEffectWOW.EffectBasePoints >= 0)
                                {
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase attack speed by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.SetAuraDescription("attack speed increased", true, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease attack speed by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.SetAuraDescription("attack speed decreased", true, " by ", "");
                                    newSpellEffectWOW.EffectMechanic = SpellMechanicType.Slowed;
                                }
                                newSpellEffects.Add(newSpellEffectWOW);

                                // Add a second for ranged attack speed
                                SpellEffectWOW newSpellEffectWOW2 = newSpellEffectWOW.Clone();
                                newSpellEffectWOW2.ActionDescription = string.Empty;
                                newSpellEffectWOW2.AuraDescription = string.Empty;
                                newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModRangedHaste;
                                if (newSpellEffectWOW2.EffectBasePoints < 0)
                                    newSpellEffectWOW2.EffectMechanic = SpellMechanicType.Slowed;
                                newSpellEffects.Add(newSpellEffectWOW2);
                            } break;
                        case SpellEQEffectType.InvisibilityUnstable:
                        case SpellEQEffectType.Invisibility:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModInvisibility;
                                newSpellEffectWOW.ActionDescription = string.Concat("grants invisibility");
                                newSpellEffectWOW.AuraDescription = string.Concat("shrouded by invisibility");
                                spellTemplate.InterruptAuraOnCast = true;
                                spellTemplate.InterruptAuraOnMeleeAttack = true;
                                spellTemplate.InterruptAuraOnMount = true;
                                spellTemplate.InterruptAuraOnTakeDamage = true;
                                spellTemplate.InterruptAuraOnSpellAttack = true;
                                spellTemplate.RemoveAuraWhenCasterCreatureInitsAgro = true;
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.InvisVsUndead:
                        case SpellEQEffectType.InvisVsUndead2:
                            {
                                // Invis vs Undead in done through a unique wow invis group. All non-undead creatures (and players)
                                // get an aura that lets them see through it
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModInvisibility;
                                newSpellEffectWOW.EffectMiscValueA = Configuration.SPELL_INVIS_VS_UNDEAD_INVIS_TYPE;
                                newSpellEffectWOW.ActionDescription = string.Concat("grants invisibility to undead");
                                newSpellEffectWOW.AuraDescription = string.Concat("shrouded by invisibility to undead");
                                spellTemplate.InterruptAuraOnCast = true;
                                spellTemplate.InterruptAuraOnMeleeAttack = true;
                                spellTemplate.InterruptAuraOnMount = true;
                                spellTemplate.InterruptAuraOnTakeDamage = true;
                                spellTemplate.InterruptAuraOnSpellAttack = true;
                                spellTemplate.RemoveAuraWhenCasterCreatureInitsAgro = true;
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.SeeInvisibility:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModInvisibilityDetect;
                                newSpellEffectWOW.EffectBasePoints = 1000;
                                newSpellEffectWOW.ActionDescription = string.Concat("grants ability to see invisibility");
                                newSpellEffectWOW.AuraDescription = string.Concat("able to see through invisibility");
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.WaterBreathing:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.WaterBreathing;
                                newSpellEffectWOW.ActionDescription = string.Concat("grants ability to breath underwater");
                                newSpellEffectWOW.AuraDescription = string.Concat("able to breath underwater");
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        // Not happy with this
                        //case SpellEQEffectType.Blind:
                        //    {
                        //        SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                        //        newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                        //        newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModAttackerMeleeHitChance;
                        //        newSpellEffectWOW.EffectBasePoints = -200;
                        //        newSpellEffectWOW.ActionDescription = string.Concat("cause blindness which makes all hits miss and stops movement");
                        //        newSpellEffectWOW.AuraDescription = string.Concat("unable to land hits or move due to blindness");
                        //        newSpellEffects.Add(newSpellEffectWOW);

                        //        // Add a second for stopping movement
                        //        SpellEffectWOW newSpellEffectWOW2 = newSpellEffectWOW.Clone();
                        //        newSpellEffectWOW2.ActionDescription = string.Empty;
                        //        newSpellEffectWOW2.AuraDescription = string.Empty;
                        //        newSpellEffectWOW2.EffectBasePoints = 0;
                        //        newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModRoot;
                        //        newSpellEffects.Add(newSpellEffectWOW2);
                        //    } break;
                        case SpellEQEffectType.Stun:
                            {
                                // Skip zero stuns (and for now, stuns with value of 1)
                                // NOTE: This will break the bard song "Denon's Breavement" if it's disabled
                                if (eqEffect.EQBaseValue <= 1)
                                    continue;

                                // Stuns become their own spell since the stun duration can differ from a parent aura duration
                                SpellTemplate effectGeneratedSpellTemplate = new SpellTemplate();
                                effectGeneratedSpellTemplate.Name = string.Concat(spellTemplate.Name, " Stunning Effect");
                                effectGeneratedSpellTemplate.WOWSpellID = IDGenerationTool.GenerateID("SpellID", "stun", spellTemplate.EQSpellID.ToString(), eqEffect.EQEffectSlot.ToString());
                                effectGeneratedSpellTemplate.EQSpellID = GenerateUniqueEQSpellID();
                                effectGeneratedSpellTemplate.SpellIconID = spellTemplate.SpellIconID;
                                effectGeneratedSpellTemplate.DoNotInterruptAutoActionsAndSwingTimers = true;
                                effectGeneratedSpellTemplate.TriggersGlobalCooldown = false;
                                effectGeneratedSpellTemplate.EQSpellEffects = new List<SpellEffectEQ>() { eqEffect };
                                effectGeneratedSpellTemplate.SpellRadius = spellTemplate.SpellRadius;
                                effectGeneratedSpellTemplate.SpellRange = spellTemplate.SpellRange;
                                effectGeneratedSpellTemplate.IsFocusBoostableEffect = spellTemplate.IsFocusBoostableEffect;
                                effectGeneratedSpellTemplate.FocusBoostType = spellTemplate.FocusBoostType;
                                effectGeneratedSpellTemplate.AuraDuration = new SpellDuration();
                                effectGeneratedSpellTemplate.AuraDuration.SetFixedDuration(eqEffect.EQBaseValue);
                                effectGeneratedSpellTemplate.SpellVisualID1 = spellTemplate.SpellVisualID1;
                                if (eqEffect.EQMaxValue > 0)
                                    effectGeneratedSpellTemplate.MaxCreatureTargetLevel = eqEffect.EQMaxValue;
                                else
                                    effectGeneratedSpellTemplate.MaxCreatureTargetLevel = Configuration.SPELL_STUN_MAX_CREATURE_TARGET_LEVEL_DEFAULT;

                                SpellEffectWOW stunSpellEffectWOW = new SpellEffectWOW();
                                stunSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                stunSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStun;
                                stunSpellEffectWOW.EffectMechanic = SpellMechanicType.Stunned;
                                stunSpellEffectWOW.ActionDescription = string.Concat("stuns");
                                stunSpellEffectWOW.AuraDescription = string.Concat("stunned");
                                stunSpellEffectWOW.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
                                effectGeneratedSpellTemplate.WOWSpellEffects.Add(stunSpellEffectWOW);

                                // Chain it
                                effectGeneratedSpellTemplates.Add(effectGeneratedSpellTemplate);
                                spellTemplate.ChainedSpellTemplates.Add(effectGeneratedSpellTemplate);

                                // Make a dummy for descriptions
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.Dummy;
                                newSpellEffectWOW.ActionDescription = string.Concat("stuns for ", effectGeneratedSpellTemplate.AuraDuration.GetTimeText());
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Fear:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModFear;
                                newSpellEffectWOW.ActionDescription = string.Concat("run away in fear");
                                newSpellEffectWOW.AuraDescription = string.Concat("running in fear");
                                newSpellEffectWOW.EffectMechanic = SpellMechanicType.Fleeing;
                                spellTemplate.BreakEffectOnNonAutoDirectDamage = true;
                                spellTemplate.NoPartialImmunity = true;
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Root:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModRoot;
                                newSpellEffectWOW.ActionDescription = string.Concat("roots in place stops movement by rooting in place");
                                newSpellEffectWOW.AuraDescription = string.Concat("rooted");
                                newSpellEffectWOW.EffectMechanic = SpellMechanicType.Rooted;
                                spellTemplate.BreakEffectOnNonAutoDirectDamage = true;
                                spellTemplate.NoPartialImmunity = true;
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.CancelMagic:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.Dispel;
                                newSpellEffectWOW.EffectMiscValueA = 1; // 1 = Magic
                                newSpellEffectWOW.EffectDieSides = 1;
                                int numOfOtherDispels = 0;
                                foreach (SpellEffectWOW wowEffect in spellTemplate.WOWSpellEffects)
                                {
                                    if (wowEffect.EffectType == SpellWOWEffectType.Dispel && wowEffect.EffectMiscValueA == 1)
                                    {
                                        wowEffect.AuraDescription = string.Empty;
                                        wowEffect.ActionDescription = string.Empty;
                                        numOfOtherDispels++;
                                    }
                                }
                                int totalDispelCount = numOfOtherDispels + 1;
                                newSpellEffectWOW.ActionDescription = string.Concat("dispels ", totalDispelCount, " beneficial (enemy) or detrimental (friendly) magic effect");
                                if (totalDispelCount > 1)
                                    newSpellEffectWOW.ActionDescription = string.Concat(newSpellEffectWOW.ActionDescription, "s");
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.DiseaseCounter:
                            {
                                // Positive values are associated with other disease effects and 'counters' don't exist in WoW, so just ignore them
                                if (eqEffect.EQBaseValue >= 0)
                                    continue;

                                // Calculate a potency
                                int totalDispels = 1;
                                if (eqEffect.EQBaseValue < -1 && eqEffect.EQBaseValue > -10)
                                    totalDispels = 2;
                                else if (eqEffect.EQBaseValue <= -10)
                                    totalDispels = 3;

                                // Update the true count by counting other ones
                                int numOfOtherDispels = 0;
                                foreach (SpellEffectWOW wowEffect in spellTemplate.WOWSpellEffects)
                                {
                                    if (wowEffect.EffectType == SpellWOWEffectType.Dispel && wowEffect.EffectMiscValueA == 3) // 3 = Disease
                                    {
                                        wowEffect.AuraDescription = string.Empty;
                                        wowEffect.ActionDescription = string.Empty;
                                        numOfOtherDispels++;
                                    }
                                }
                                int totalDispelCount = totalDispels + numOfOtherDispels;

                                // Create one for each count
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.Dispel;
                                newSpellEffectWOW.EffectMiscValueA = 3; // 3 = Disease
                                newSpellEffectWOW.EffectDieSides = 1;
                                for (int i = 1; i < totalDispels; i++)
                                {
                                    SpellEffectWOW newSpellEffectWOWAdditonal = newSpellEffectWOW.Clone();
                                    newSpellEffectWOWAdditonal.ActionDescription = string.Empty;
                                    newSpellEffectWOWAdditonal.AuraDescription = string.Empty;
                                    newSpellEffects.Add(newSpellEffectWOWAdditonal);
                                }

                                // Set the in the display
                                newSpellEffectWOW.ActionDescription = string.Concat("cures ", totalDispelCount, " disease");
                                if (totalDispels > 1)
                                    newSpellEffectWOW.ActionDescription = string.Concat(newSpellEffectWOW.ActionDescription, "s");
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.PoisonCounter:
                            {
                                // Positive values are associated with other poison effects and 'counters' don't exist in WoW, so just ignore them
                                if (eqEffect.EQBaseValue >= 0)
                                    continue;

                                // Calculate a potency
                                int totalDispels = 1;
                                if (eqEffect.EQBaseValue < -1 && eqEffect.EQBaseValue > -10)
                                    totalDispels = 2;
                                else if (eqEffect.EQBaseValue <= -10)
                                    totalDispels = 3;

                                // Update the true count by counting other ones
                                int numOfOtherDispels = 0;
                                foreach (SpellEffectWOW wowEffect in spellTemplate.WOWSpellEffects)
                                {
                                    if (wowEffect.EffectType == SpellWOWEffectType.Dispel && wowEffect.EffectMiscValueA == 4) // 4 = Poison
                                    {
                                        wowEffect.AuraDescription = string.Empty;
                                        wowEffect.ActionDescription = string.Empty;
                                        numOfOtherDispels++;
                                    }
                                }
                                int totalDispelCount = totalDispels + numOfOtherDispels;

                                // Create one for each count
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.Dispel;
                                newSpellEffectWOW.EffectMiscValueA = 4; // 4 = Poison
                                newSpellEffectWOW.EffectDieSides = 1;
                                for (int i = 1; i < totalDispels; i++)
                                {
                                    SpellEffectWOW newSpellEffectWOWAdditonal = newSpellEffectWOW.Clone();
                                    newSpellEffectWOWAdditonal.ActionDescription = string.Empty;
                                    newSpellEffectWOWAdditonal.AuraDescription = string.Empty;
                                    newSpellEffects.Add(newSpellEffectWOWAdditonal);
                                }

                                // Set the in the display
                                newSpellEffectWOW.ActionDescription = string.Concat("cures ", totalDispelCount, " poison");
                                if (totalDispels > 1)
                                    newSpellEffectWOW.ActionDescription = string.Concat(newSpellEffectWOW.ActionDescription, "s");
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.SummonItems:
                            {
                                if (itemTemplatesByEQDBID.ContainsKey(eqEffect.EQBaseValue) == false)
                                {
                                    Logger.WriteWarning("Failed to summon items with eq id ", eqEffect.EQBaseValue.ToString(), " for eq spell id ", spellTemplate.EQSpellID.ToString(), " as it was not found in the item list");
                                    continue;
                                }

                                int itemCount = Math.Max(eqEffect.EQMaxValue, 1);
                                SpellEffectWOW spellEffectWOW = new SpellEffectWOW();
                                spellEffectWOW.EffectType = SpellWOWEffectType.CreateItem;
                                spellEffectWOW.EffectBasePoints = itemCount;
                                spellEffectWOW.EffectItemType = Convert.ToUInt32(itemTemplatesByEQDBID[eqEffect.EQBaseValue].WOWEntryID);
                                itemTemplatesByEQDBID[eqEffect.EQBaseValue].IsCreatedBySpell = true;
                                string itemName = itemTemplatesByEQDBID[eqEffect.EQBaseValue].Name;
                                if (itemCount == 1)
                                    spellEffectWOW.ActionDescription = string.Concat("Conjure ", itemCount, " ", itemName, ".\n\nConjured items disappear if logged out for more than 15 minutes");
                                else
                                    spellEffectWOW.ActionDescription = string.Concat("Conjure ", itemCount, " ", itemName, "s.\n\nConjured items disappear if logged out for more than 15 minutes");
                                newSpellEffects.Add(spellEffectWOW);
                            } break;
                        case SpellEQEffectType.ResistFire:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                                newSpellEffectWOW.EffectMiscValueA = 4; // Fire
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "FireResistanceBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase fire resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("fire resistance increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "FireResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease fire resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("fire resistance decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.ResistCold:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                                newSpellEffectWOW.EffectMiscValueA = 16; // Frost
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "FrostResistanceBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase frost resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("frost resistance increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "FrostResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease frost resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("frost resistance decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.ResistPoison:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                                newSpellEffectWOW.EffectMiscValueA = 8; // Nature
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "NatureResistanceBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase nature resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("nature resistance increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "NatureResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease nature resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("nature resistance decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.ResistDisease:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                                newSpellEffectWOW.EffectMiscValueA = 32; // Shadow
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ShadowResistanceBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase shadow resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("shadow resistance increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ShadowResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease shadow resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("shadow resistance decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.ResistMagic:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                                newSpellEffectWOW.EffectMiscValueA = 64; // Arcane
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArcaneResistanceBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase arcane resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("arcane resistance increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArcaneResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease arcane resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("arcane resistance decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.ResistAll:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                                newSpellEffectWOW.EffectMiscValueA = 124; // All resistances
                                if (eqEffect.EQBaseValue >= 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArcaneResistanceBuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("increase all resistances by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("all resistances increased", false, " by ", "");
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArcaneResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease all resistances by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.SetAuraDescription("all resistances decreased", false, " by ", "");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Revive:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ResurrectNew;
                                int amountRestored = Math.Max(Configuration.SPELL_EFFECT_REVIVE_EXPPCT_TO_HPMP_MULTIPLIER * eqEffect.EQBaseValue, 1);
                                newSpellEffectWOW.EffectBasePoints = amountRestored;
                                newSpellEffectWOW.ActionDescription = string.Concat("brings a dead player back to life with ", amountRestored, " health and ", amountRestored, " mana");
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Gate:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                if (Configuration.SPELLS_GATE_TETHER_ENABLED == true)
                                {
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.Dummy;
                                    newSpellEffectWOW.ActionDescription = "opens a magical portal that returns you to your bind point in norrath, and you will have 30 minutes where you can return to your gate point after casting it";
                                    newSpellEffectWOW.AuraDescription = "you are tethered to the location where you gated and may return there if you click it off before the buff wears off, but it will fail in combat";
                                    newSpellEffectWOW.EffectMiscValueA = (int)SpellDummyType.Gate;
                                    spellTemplate.AuraDuration.SetFixedDuration(1800000); // 30 minutes
                                    spellTemplate.AuraStaysOnSecondaryClassSwitch = true;
                                }
                                else
                                {
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.Dummy;
                                    newSpellEffectWOW.ActionDescription = "opens a magical portal that returns you to your bind point in norrath";
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                    newSpellEffectWOW.EffectMiscValueA = (int)SpellDummyType.Gate;
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.BindAffinity:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.Dummy;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.Dummy;
                                newSpellEffectWOW.EffectMiscValueA = (int)SpellDummyType.BindAny;
                                newSpellEffectWOW.ActionDescription = string.Concat("binds the soul of the target to their current location, which only works in norrath");
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Teleport:
                            {
                                if (zonePropertiesByShortName.ContainsKey(teleportZoneOrPetTypeName) == false)
                                {
                                    Logger.WriteDebug("Could not convert teleport spell effect for eq spell id ", spellTemplate.EQSpellID.ToString(), " since there is no output zone properties loaded for zone short name ", teleportZoneOrPetTypeName);
                                    continue;
                                }
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.ImplicitTargetB = SpellWOWTargetType.DestinationDatabaseForTeleport;
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.TeleportUnits;
                                newSpellEffectWOW.EffectDieSides = 1;
                                newSpellEffectWOW.EffectBasePoints = -1;
                                newSpellEffectWOW.ActionDescription = string.Concat("teleports the affected to ", zonePropertiesByShortName[teleportZoneOrPetTypeName].DescriptiveName);
                                newSpellEffectWOW.TeleMapID = zonePropertiesByShortName[teleportZoneOrPetTypeName].DBCMapID;

                                // Position
                                Vector3 telePosition = new Vector3(eqEffect.EQTelePosition);
                                telePosition.X *= Configuration.GENERATE_WORLD_SCALE;
                                telePosition.Y *= Configuration.GENERATE_WORLD_SCALE;
                                telePosition.Z *= Configuration.GENERATE_WORLD_SCALE;
                                newSpellEffectWOW.TelePosition = telePosition;

                                // Orientation
                                // Note: "Heading" in EQ was 0-512 instead of 0-360, and the result needs to rotate 180 degrees due to y axis difference
                                float orientation;
                                if (eqEffect.EQTeleHeading == 0)
                                    orientation = MathF.PI;
                                else
                                {
                                    float orientationInDegrees = (eqEffect.EQTeleHeading / 512) * 360;
                                    float orientationInRadians = orientationInDegrees * MathF.PI / 180.0f;
                                    orientation = orientationInRadians + MathF.PI;
                                }
                                newSpellEffectWOW.TeleOrientation = orientation;

                                if (targets.Count > 1)
                                    Logger.WriteError("Teleport for eq spell id ", spellTemplate.EQSpellID.ToString(), " will not properly hit targets since there is > 2 targets");

                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Succor:
                            {
                                // In-zone succor has "same" in this field, which the mod side of the code will have to figure it out
                                if (teleportZoneOrPetTypeName.Trim().ToLower() == "same")
                                {
                                    SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.Dummy;
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                    newSpellEffectWOW.EffectMiscValueA = (int)SpellDummyType.Succor;
                                    newSpellEffectWOW.ActionDescription = "returns the affected to a safe location in their current zone";
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
                                // Other-zone succor, which is mostly a normal teleport
                                else
                                {
                                    if (zonePropertiesByShortName.ContainsKey(teleportZoneOrPetTypeName) == false)
                                    {
                                        Logger.WriteDebug("Could not convert succor spell effect for eq spell id ", spellTemplate.EQSpellID.ToString(), " since there is no output zone properties loaded for zone short name ", teleportZoneOrPetTypeName);
                                        continue;
                                    }
                                    ZoneProperties destinationZoneProperties = zonePropertiesByShortName[teleportZoneOrPetTypeName];
                                    SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                    newSpellEffectWOW.ImplicitTargetB = SpellWOWTargetType.DestinationDatabaseForTeleport;
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.TeleportUnits;
                                    newSpellEffectWOW.EffectDieSides = 1;
                                    newSpellEffectWOW.EffectBasePoints = -1;
                                    newSpellEffectWOW.ActionDescription = string.Concat("returns the affected to ", destinationZoneProperties.DescriptiveName);
                                    newSpellEffectWOW.TeleMapID = destinationZoneProperties.DBCMapID;

                                    // Cross-zone succor teleports to the spell coordinates (TAKP also has a range, not replicated here)
                                    Vector3 telePosition = new Vector3(eqEffect.EQTelePosition);
                                    telePosition.X *= Configuration.GENERATE_WORLD_SCALE;
                                    telePosition.Y *= Configuration.GENERATE_WORLD_SCALE;
                                    telePosition.Z *= Configuration.GENERATE_WORLD_SCALE;
                                    newSpellEffectWOW.TelePosition = telePosition;

                                    // Orientation
                                    // Note: "Heading" in EQ was 0-512 instead of 0-360, and the result needs to rotate 180 degrees due to y axis difference
                                    float orientation;
                                    if (eqEffect.EQTeleHeading == 0)
                                        orientation = MathF.PI;
                                    else
                                    {
                                        float orientationInDegrees = (eqEffect.EQTeleHeading / 512) * 360;
                                        float orientationInRadians = orientationInDegrees * MathF.PI / 180.0f;
                                        orientation = orientationInRadians + MathF.PI;
                                    }
                                    newSpellEffectWOW.TeleOrientation = orientation;

                                    if (targets.Count > 1)
                                        Logger.WriteError("Succor for eq spell id ", spellTemplate.EQSpellID.ToString(), " will not properly hit targets since there is > 2 targets");
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
                            } break;
                        case SpellEQEffectType.Silence:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModSilence;
                                newSpellEffectWOW.ActionDescription = string.Concat("casting interrupted and unable to cast spells");
                                newSpellEffectWOW.AuraDescription = string.Concat("silenced");
                                newSpellEffectWOW.EffectMechanic = SpellMechanicType.Silenced;
                                spellTemplate.NoPartialImmunity = true;
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Levitate:
                            {
                                // FeatherFall
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.FeatherFall;
                                newSpellEffectWOW.ActionDescription = string.Concat("grants levitation");
                                newSpellEffectWOW.AuraDescription = string.Concat("levitating");
                                newSpellEffects.Add(newSpellEffectWOW);

                                // + Waterwalk
                                SpellEffectWOW newSpellEffectWOW2 = new SpellEffectWOW();
                                newSpellEffectWOW2.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.WaterWalk;
                                newSpellEffectWOW2.ActionDescription = string.Empty;
                                newSpellEffectWOW2.AuraDescription = string.Empty;
                                newSpellEffects.Add(newSpellEffectWOW2);

                                // + Hover
                                SpellEffectWOW newSpellEffectWOW3 = new SpellEffectWOW();
                                newSpellEffectWOW3.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW3.EffectAuraType = SpellWOWAuraType.Hover;
                                newSpellEffectWOW3.ActionDescription = string.Empty;
                                newSpellEffectWOW3.AuraDescription = string.Empty;
                                newSpellEffects.Add(newSpellEffectWOW3);
                            } break;
                        case SpellEQEffectType.DamageShield:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                if (eqEffect.EQBaseValue > 0)
                                {
                                    // Create a healing spell for this
                                    SpellTemplate effectGeneratedSpellTemplate = new SpellTemplate();
                                    effectGeneratedSpellTemplate.Name = string.Concat(spellTemplate.Name, " Heal Effect");
                                    effectGeneratedSpellTemplate.WOWSpellID = IDGenerationTool.GenerateID("SpellID", "heal", spellTemplate.EQSpellID.ToString(), eqEffect.EQEffectSlot.ToString());
                                    effectGeneratedSpellTemplate.EQSpellID = GenerateUniqueEQSpellID();
                                    effectGeneratedSpellTemplate.SpellIconID = spellTemplate.SpellIconID;
                                    effectGeneratedSpellTemplate.SpellVisualID1 = 5560; // Lesser Heal visual, like Judgement of Light
                                    effectGeneratedSpellTemplate.SchoolMask = 2; // Holy
                                    effectGeneratedSpellTemplate.HideCaster = true;
                                    effectGeneratedSpellTemplate.DoNotInterruptAutoActionsAndSwingTimers = true;
                                    effectGeneratedSpellTemplate.TriggersGlobalCooldown = false;
                                    SpellEffectEQ healEQEffect = new SpellEffectEQ();
                                    healEQEffect.EQEffectType = SpellEQEffectType.CurrentHitPoints;
                                    healEQEffect.EQBaseValue = eqEffect.EQBaseValue;
                                    healEQEffect.EQBaseValueFormulaType = eqEffect.EQBaseValueFormulaType;
                                    healEQEffect.EQFormulaTypeValue = eqEffect.EQFormulaTypeValue;
                                    healEQEffect.EQLimitValue = eqEffect.EQLimitValue;
                                    healEQEffect.EQMaxValue = eqEffect.EQMaxValue;
                                    effectGeneratedSpellTemplate.EQSpellEffects.Add(healEQEffect);
                                    effectGeneratedSpellTemplates.Add(effectGeneratedSpellTemplate);
                                    ConvertEQSpellEffectsIntoWOWEffects(ref effectGeneratedSpellTemplate, schoolMask, new SpellDuration(), 0, new List<SpellWOWTargetType>() { SpellWOWTargetType.UnitTargetAny },
                                        0, itemTemplatesByEQDBID, false, string.Empty, zonePropertiesByShortName, ref creatureTemplatesByEQID, ref effectGeneratedSpellTemplates);

                                    // Proc effect for the heal
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.Dummy;
                                    newSpellEffectWOW.ActionDescription = string.Concat("applies a shield that heals melee attackers for ", effectGeneratedSpellTemplate.WOWSpellEffects[0].GetFormattedEffectActionString(false));
                                    // Rendered immediately: the amount is measured on the child heal effect but stored here, so it cannot be deferred.
                                    newSpellEffectWOW.AuraDescription = string.Concat("healing melee attackers", effectGeneratedSpellTemplate.WOWSpellEffects[0].GetFormattedEffectAuraString(false, " for ", ""));
                                    spellTemplate.WOWSpellIDCastOnMeleeAttacker = effectGeneratedSpellTemplate.WOWSpellID;
                                }
                                else
                                {
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
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.DamageShield;
                                    newSpellEffectWOW.SetEffectAmountValues(Math.Abs(eqEffect.EQBaseValue), Math.Abs(eqEffect.EQMaxValue), spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageReflectShield", SpellEffectWOWConversionScaleType.None);
                                    
                                    if (elementalSchoolName.Length > 0)
                                    {
                                        newSpellEffectWOW.ActionDescription = string.Concat("grants a damage shield that reflects ", newSpellEffectWOW.GetFormattedEffectActionString(false), " ", elementalSchoolName, " damage back melee attackers");
                                        newSpellEffectWOW.SetAuraDescription("reflecting", false, " ", string.Concat(" ", elementalSchoolName, " damage back at melee attackers"));
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.ActionDescription = string.Concat("grants a damage shield that reflects ", newSpellEffectWOW.GetFormattedEffectActionString(false), " damage back melee attackers");
                                        newSpellEffectWOW.SetAuraDescription("reflecting", false, " ", " damage back at melee attackers");
                                    }
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Mez:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStun;
                                newSpellEffectWOW.ActionDescription = string.Concat("mesmerises the target which stops all actions until damage is taken");
                                newSpellEffectWOW.AuraDescription = string.Concat("mesmerized");
                                newSpellEffectWOW.EffectMechanic = SpellMechanicType.Incapacitated;
                                if (eqEffect.EQMaxValue > 0 && (spellTemplate.MaxCreatureTargetLevel == 0 || eqEffect.EQMaxValue < spellTemplate.MaxCreatureTargetLevel))
                                    spellTemplate.MaxCreatureTargetLevel = eqEffect.EQMaxValue;
                                spellTemplate.InterruptAuraOnTakeDamage = true;
                                spellTemplate.NoPartialImmunity = true;

                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Charm:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModCharm;
                                newSpellEffectWOW.ActionDescription = string.Concat("charms the target which makes it fight for you");
                                newSpellEffectWOW.AuraDescription = string.Concat("charmed");
                                newSpellEffectWOW.EffectMechanic = SpellMechanicType.Charmed;
                                if (eqEffect.EQMaxValue > 0 && (spellTemplate.MaxCreatureTargetLevel == 0 || eqEffect.EQMaxValue < spellTemplate.MaxCreatureTargetLevel))
                                    spellTemplate.MaxCreatureTargetLevel = eqEffect.EQMaxValue;
                                spellTemplate.NoPartialImmunity = true;
                                spellTemplate.GenerateNoThreat = true; // This avoids being stuck in combat
                                spellTemplate.IsCharmSpell = true; // This too
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Rune:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                spellTemplate.InfluencedBySpellPower = true;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.SchoolAbsorb;
                                newSpellEffectWOW.EffectMiscValueA = 1; // Physical
                                newSpellEffectWOW.SetEffectAmountValues(Math.Abs(eqEffect.EQBaseValue), Math.Abs(eqEffect.EQMaxValue), spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "PhysicalAbsorb", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("applies a shield that absorbs ", newSpellEffectWOW.GetFormattedEffectActionString(false), " physical damage before breaking");
                                newSpellEffectWOW.SetAuraDescription("absorbing", false, " ", " physical damage");
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.ModelSize:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModScale;
                                int modelScale = eqEffect.EQBaseValue - 100; // 100 is 'same size'
                                newSpellEffectWOW.SetEffectAmountValues(modelScale, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "", SpellEffectWOWConversionScaleType.None);
                                if (modelScale > 0)
                                {
                                    newSpellEffectWOW.ActionDescription = string.Concat("grows the size of the target by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.SetAuraDescription("grown by ", true, string.Empty, string.Empty);
                                }
                                else
                                {
                                    newSpellEffectWOW.ActionDescription = string.Concat("shrink the size of the target by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.SetAuraDescription("shrunk by ", true, string.Empty, string.Empty);
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.BindSight:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.BindSight;
                                newSpellEffectWOW.ActionDescription = string.Concat("allows the caster to see through the target's eyes");
                                newSpellEffectWOW.AuraDescription = string.Concat("sight granted through target's eyes");
                                newSpellEffects.Add(newSpellEffectWOW);

                                SpellEffectWOW newSpellEffectWOW2 = new SpellEffectWOW();
                                newSpellEffectWOW2.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModStalked;
                                newSpellEffects.Add(newSpellEffectWOW2);

                                spellTemplate.IsChanneled = true;
                                spellTemplate.ForceAsDebuff = true;
                                spellTemplate.IsFarSight = true;
                                spellTemplate.GenerateNoThreat = true;
                                spellTemplate.IgnoreTargetRequirements = true;
                                spellTemplate.InterruptOnCast = false;
                                spellTemplate.InterruptOnPushback = false;
                                spellTemplate.ChannelInterruptFlags = 31772;
                            } break;
                        case SpellEQEffectType.DivineAura:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.SchoolImmunity;
                                newSpellEffectWOW.ActionDescription = string.Concat("grants invulnerability but stops any ability to attack or cast");
                                newSpellEffectWOW.AuraDescription = string.Concat("invulnerable");
                                newSpellEffectWOW.EffectMiscValueA = 127;
                                newSpellEffects.Add(newSpellEffectWOW);

                                SpellEffectWOW newSpellEffectWOW2 = new SpellEffectWOW();
                                newSpellEffectWOW2.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModPacify;
                                newSpellEffects.Add(newSpellEffectWOW2);

                                SpellEffectWOW newSpellEffectWOW3 = new SpellEffectWOW();
                                newSpellEffectWOW2.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModPacifySilence;
                                newSpellEffects.Add(newSpellEffectWOW3);
                            } break;
                        case SpellEQEffectType.WeaponProc:
                            {
                                if (spellTemplate.ProcLinkEQSpellID != 0)
                                {
                                    Logger.WriteError("Proc effect already bound to eq spell id ", spellTemplate.EQSpellID.ToString());
                                    continue;
                                }
                                spellTemplate.ProcLinkEQSpellID = eqEffect.EQBaseValue;
                                spellTemplate.ProcsOnMeleeAttacks = true;
                                spellTemplate.ProcChance = Convert.ToUInt32(Configuration.SPELLS_ENCHANT_SPELL_IMBUE_PROC_CHANGE);

                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ProcTriggerSpell;
                                newSpellEffectWOW.ActionDescription = string.Concat("imbues the caster's attacks");
                                newSpellEffectWOW.EffectTriggerSpell = eqEffect.EQBaseValue;

                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.SummonPet:
                        case SpellEQEffectType.NecPet:
                            {
                                SpellPet? spellPet = SpellPet.GetSpellPetByTypeName(teleportZoneOrPetTypeName);
                                if (spellPet == null)
                                {
                                    Logger.WriteError("Could not assign pet for eq spell id ", spellTemplate.EQSpellID.ToString(), " as there was no typename of ", teleportZoneOrPetTypeName);
                                    continue;
                                }
                                if (creatureTemplatesByEQID.ContainsKey(spellPet.EQCreatureTemplateID) == false)
                                {
                                    Logger.WriteError("Could not assign pet for eq spell id ", spellTemplate.EQSpellID.ToString(), " as there was no eq creature template ID of ", spellPet.EQCreatureTemplateID.ToString());
                                    continue;
                                }

                                // Set names for this creature template
                                switch (spellPet.NamingType)
                                {
                                    case SpellPetNamingType.Pet: creatureTemplatesByEQID[spellPet.EQCreatureTemplateID].Name = "Pet"; break;
                                    case SpellPetNamingType.Familiar: creatureTemplatesByEQID[spellPet.EQCreatureTemplateID].Name = "Familiar"; break;
                                    case SpellPetNamingType.Warder: creatureTemplatesByEQID[spellPet.EQCreatureTemplateID].Name = "Warder"; break;
                                    case SpellPetNamingType.Random: creatureTemplatesByEQID[spellPet.EQCreatureTemplateID].Name = "Random"; break;
                                }

                                spellTemplate.SummonSpellPet = spellPet;
                                spellTemplate.SummonCreatureTemplateID = creatureTemplatesByEQID[spellPet.EQCreatureTemplateID].WOWCreatureTemplateID;

                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectMiscValueA = creatureTemplatesByEQID[spellPet.EQCreatureTemplateID].WOWCreatureTemplateID;                                

                                // Description differs if undead or not
                                if (eqEffect.EQEffectType == SpellEQEffectType.NecPet)
                                    newSpellEffectWOW.ActionDescription = "summon an undead companion to fight by your side";
                                else
                                    newSpellEffectWOW.ActionDescription = "summon a companion to fight by your side";

                                // Different summon types based on the config
                                if (Configuration.SPELL_EFFECT_SUMMON_PETS_USE_EQ_LEVEL_AND_BEHAVIOR == true)
                                {
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.Summon;
                                    spellTemplate.SummonPropertiesDBCID = IDGenerationTool.GenerateID("SummonPropertiesID", spellTemplate.EQSpellID.ToString(), eqEffect.EQEffectSlot.ToString());
                                    newSpellEffectWOW.EffectMiscValueB = spellTemplate.SummonPropertiesDBCID;
                                }
                                else
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.SummonPet;
                                newSpellEffects.Add(newSpellEffectWOW);

                                // This scaling is to ensure that equipment looks the right size on creatures
                                CreatureRace creatureRace = creatureTemplatesByEQID[spellPet.EQCreatureTemplateID].Race;
                                creatureTemplatesByEQID[spellPet.EQCreatureTemplateID].ModelTemplateScale = creatureRace.Height * creatureRace.SpawnSizeMod
                                    * (Configuration.GENERATE_CREATURE_SCALE / Configuration.GENERATE_EQUIPMENT_SCALE);
                                creatureTemplatesByEQID[spellPet.EQCreatureTemplateID].IsPet = true;
                            } break;
                        case SpellEQEffectType.Illusion:
                            {
                                // Any value over 196 is an illusion for an expansion that will not be supported
                                if (eqEffect.EQBaseValue > 196)
                                {
                                    Logger.WriteWarning("SpellTemplate with wow spell template id ", spellTemplate.WOWSpellID.ToString(), " has an illusion effect greater than 196, so it's being skipped");
                                    continue;
                                }
                                if (eqEffect.EQBaseValue < 0)
                                {
                                    Logger.WriteError("SpellTemplate with wow spell template id ", spellTemplate.WOWSpellID.ToString(), " has an illusion effect < 0, so it's being skipped");
                                    continue;
                                }

                                int textureID = 0;
                                if (spellTemplate.EQAOERange < 10) // Why is aoerange the textureID? 
                                    textureID = spellTemplate.EQAOERange;

                                // Temp faction value.  Need to change this on a per-form basis
                                int wowFactionTemplateID = 35; // Friendly

                                // Male form
                                SpellTemplate maleFormSpellTemplate = new SpellTemplate();
                                maleFormSpellTemplate.Name = string.Concat(spellTemplate.Name);
                                maleFormSpellTemplate.WOWSpellID = IDGenerationTool.GenerateID("SpellID", "maleform", spellTemplate.EQSpellID.ToString(), eqEffect.EQEffectSlot.ToString());
                                maleFormSpellTemplate.EQSpellID = GenerateUniqueEQSpellID();
                                maleFormSpellTemplate.SpellIconID = spellTemplate.SpellIconID;
                                SpellEffectWOW maleFormSpellEffectWOW = new SpellEffectWOW();
                                maleFormSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                maleFormSpellEffectWOW.EffectAuraType = SpellWOWAuraType.Transform;
                                maleFormSpellEffectWOW.ImplicitTargetA = targets[0];
                                if (targets.Count == 2)
                                    maleFormSpellEffectWOW.ImplicitTargetB = targets[1];
                                maleFormSpellEffectWOW.EffectRadiusIndex = Convert.ToUInt32(spellRadiusIndex);
                                CreatureRace? creatureRaceMale = CreatureRace.GetRaceForRaceGenderVariant(eqEffect.EQBaseValue, CreatureGenderType.Male, 0, true);
                                if (creatureRaceMale == null)
                                {
                                    Logger.WriteError("SpellTemplate with wow spell template id ", spellTemplate.WOWSpellID.ToString(), " has an illusion effect but could not find a raceid with id ", eqEffect.EQBaseValue.ToString());
                                    continue;
                                }
                                maleFormSpellTemplate.CanMountWhileInForm = creatureRaceMale.CanMount;
                                float scaleMale = creatureRaceMale.Height * creatureRaceMale.SpawnSizeMod * (Configuration.GENERATE_CREATURE_SCALE / Configuration.GENERATE_EQUIPMENT_SCALE);
                                CreatureTemplate maleCreatureTemplate = CreatureTemplate.GenerateCreatureTemplate(maleFormSpellTemplate.Name, creatureRaceMale, creatureRaceMale.Gender, 0, textureID, 0, 0, scaleMale, wowFactionTemplateID, maleFormSpellTemplate.WOWSpellID);
                                maleFormSpellEffectWOW.EffectMiscValueA = maleCreatureTemplate.WOWCreatureTemplateID;
                                string raceName = creatureRaceMale.Name;
                                string textParticle = "a";
                                if (raceName.ToLower().StartsWith("a") || raceName.ToLower().StartsWith("e") || raceName.ToLower().StartsWith("o"))
                                    textParticle = "an";
                                maleFormSpellEffectWOW.ActionDescription = string.Concat("changes the form to ", textParticle, " ", raceName);
                                maleFormSpellEffectWOW.AuraDescription = string.Concat("appear as ", textParticle, " ", raceName);
                                maleFormSpellTemplate.WOWSpellEffects.Add(maleFormSpellEffectWOW);
                                maleFormSpellTemplate.AuraDuration = spellTemplate.AuraDuration;
                                maleFormSpellTemplate.IllusionSpellParent = spellTemplate;
                                spellTemplate.MaleFormSpellTemplateID = maleFormSpellTemplate.WOWSpellID;
                                effectGeneratedSpellTemplates.Add(maleFormSpellTemplate);
                                if (creatureRaceMale.CanShowEquipInIllusion == true)
                                    CreatureIllusionVersionRegistry.RegisterFormSpell(maleFormSpellTemplate.WOWSpellID, creatureRaceMale, creatureRaceMale.Gender, scaleMale);

                                // Female form
                                SpellTemplate femaleFormSpellTemplate = new SpellTemplate();
                                femaleFormSpellTemplate.Name = string.Concat(spellTemplate.Name);
                                femaleFormSpellTemplate.WOWSpellID = IDGenerationTool.GenerateID("SpellID", "femaleform", spellTemplate.EQSpellID.ToString(), eqEffect.EQEffectSlot.ToString());
                                femaleFormSpellTemplate.EQSpellID = GenerateUniqueEQSpellID();
                                femaleFormSpellTemplate.SpellIconID = spellTemplate.SpellIconID;
                                SpellEffectWOW femaleFormSpellEffectWOW = new SpellEffectWOW();
                                femaleFormSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                femaleFormSpellEffectWOW.EffectAuraType = SpellWOWAuraType.Transform;
                                femaleFormSpellEffectWOW.ImplicitTargetA = targets[0];
                                if (targets.Count == 2)
                                    femaleFormSpellEffectWOW.ImplicitTargetB = targets[1];
                                femaleFormSpellEffectWOW.EffectRadiusIndex = Convert.ToUInt32(spellRadiusIndex);
                                CreatureRace? creatureRaceFemale = CreatureRace.GetRaceForRaceGenderVariant(eqEffect.EQBaseValue, CreatureGenderType.Female, 0, true);
                                if (creatureRaceFemale == null)
                                {
                                    Logger.WriteError("SpellTemplate with wow spell template id ", spellTemplate.WOWSpellID.ToString(), " has an illusion effect but could not find a raceid with id ", eqEffect.EQBaseValue.ToString());
                                    continue;
                                }
                                femaleFormSpellTemplate.CanMountWhileInForm = creatureRaceFemale.CanMount;
                                float scaleFemale = creatureRaceFemale.Height * creatureRaceFemale.SpawnSizeMod * (Configuration.GENERATE_CREATURE_SCALE / Configuration.GENERATE_EQUIPMENT_SCALE);
                                CreatureTemplate femaleCreatureTemplate = CreatureTemplate.GenerateCreatureTemplate(femaleFormSpellTemplate.Name, creatureRaceFemale, creatureRaceFemale.Gender, 0, textureID, 0, 0, scaleFemale, wowFactionTemplateID, femaleFormSpellTemplate.WOWSpellID);
                                femaleFormSpellEffectWOW.EffectMiscValueA = femaleCreatureTemplate.WOWCreatureTemplateID;
                                raceName = creatureRaceFemale.Name;
                                textParticle = "a";
                                if (raceName.ToLower().StartsWith("a") || raceName.ToLower().StartsWith("e") || raceName.ToLower().StartsWith("o"))
                                    textParticle = "an";
                                femaleFormSpellEffectWOW.ActionDescription = string.Concat("changes the form to ", textParticle, " ", raceName);
                                femaleFormSpellEffectWOW.AuraDescription = string.Concat("appear as ", textParticle, " ", raceName);
                                femaleFormSpellTemplate.WOWSpellEffects.Add(femaleFormSpellEffectWOW);
                                femaleFormSpellTemplate.AuraDuration = spellTemplate.AuraDuration;
                                spellTemplate.FemaleFormSpellTemplateID = femaleFormSpellTemplate.WOWSpellID;
                                femaleFormSpellTemplate.IllusionSpellParent = spellTemplate;
                                effectGeneratedSpellTemplates.Add(femaleFormSpellTemplate);
                                if (creatureRaceFemale.CanShowEquipInIllusion == true)
                                    CreatureIllusionVersionRegistry.RegisterFormSpell(femaleFormSpellTemplate.WOWSpellID, creatureRaceFemale, creatureRaceFemale.Gender, scaleFemale);

                                // Parent illusion spell
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.Dummy;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                newSpellEffectWOW.EffectMiscValueA = (int)SpellDummyType.IllusionParent;
                                newSpellEffectWOW.ActionDescription = string.Concat("changes the form to ", textParticle, " ", raceName);
                                newSpellEffectWOW.AuraDescription = string.Concat("appear as ", textParticle, " ", raceName);
                                newSpellEffects.Add(newSpellEffectWOW);
                                spellTemplate.IsllusionSpellParent = true;
                            } break;
                        case SpellEQEffectType.FeignDeath:
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.FeignDeath;
                                newSpellEffectWOW.ActionDescription = string.Concat("feigns death, removing you from combat and dropping aggro");
                                newSpellEffectWOW.AuraDescription = string.Concat("feigning death");
                                newSpellEffects.Add(newSpellEffectWOW);

                                // Needs to have a root effect and no wear-off
                                SpellEffectWOW rootSpellEffectWOW = new SpellEffectWOW();
                                rootSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                rootSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModRoot;
                                newSpellEffects.Add(rootSpellEffectWOW);
                                spellTemplate.AuraDuration.IsInfinite = true;
                                spellTemplate.EffectFailChancePercent = Configuration.SPELL_FEIGN_DEATH_FAIL_CHANCE_PERCENT;
                                spellTemplate.FailableType = SpellFailableType.FeignDeath;
                                spellTemplate.InterruptAuraOnCast = true;
                                spellTemplate.InterruptAuraOnMeleeAttack = true;
                            } break;
                        case SpellEQEffectType.NegateIfCombat:
                            {
                                // Break when the wearer casts a spell or attacks
                                spellTemplate.InterruptAuraOnCast = true;
                                spellTemplate.InterruptAuraOnMeleeAttack = true;
                                spellTemplate.IsNegateIfCombat = true;
                            } break;
                        default:
                            {
                                Logger.WriteError("Unhandled SpellTemplate EQEffectType of ", eqEffect.EQEffectType.ToString(), " for eq spell id ", spellTemplate.EQSpellID.ToString());
                                continue;
                            }
                    }
                }
            }

            // Add targets and radius
            foreach (SpellEffectWOW spellEffect in newSpellEffects)
            {
                if (targets.Count == 0)
                {
                    Logger.WriteError("Too few targets for spell effect");
                    continue;
                }
                if (targets.Count > 2)
                {
                    Logger.WriteError("Too many targets for spell effect");
                    continue;
                }
                spellEffect.ImplicitTargetA = targets[0];
                if (targets.Count == 2)
                    spellEffect.ImplicitTargetB = targets[1];

                spellEffect.EffectRadiusIndex = Convert.ToUInt32(spellRadiusIndex);

                // Don't need to add leach transfer effects because they already were
                if (spellTemplate.IsTransferEffectType == false)
                    spellTemplate.WOWSpellEffects.Add(spellEffect);
            }

            // Sort them so the aura effects are last
            spellTemplate.WOWSpellEffects.Sort();
        }

        private static void SetAuraStackRule(ref SpellTemplate spellTemplate, int eqSpellCategory, bool isBardSongAura, bool isDetrimental, bool isItemClickSpell)
        {
            // Spells attached to item clicks always take a buff slot in EQ, so they stack-compete even when they have no player spell category (the potion/poison-only spells are all -99)
            if (eqSpellCategory < 0 && isItemClickSpell == false) // NPC = -99, AA Procs = -1
                return;
            if (eqSpellCategory > 250) // AA Abilities = 999
                return;

            // Stacking only matters for spells that leave a lasting aura on the target
            if (spellTemplate.AuraDuration.MaxDurationInMS <= 0 && spellTemplate.AuraDuration.IsInfinite == false)
                return;

            // Collect the stacking-relevant effect keys this spell carries
            HashSet<int> effectStackKeys = new HashSet<int>();
            foreach (SpellEffectEQ eqEffect in spellTemplate.EQSpellEffects)
                AddStackingEffectKeys(eqEffect, effectStackKeys);
            if (effectStackKeys.Count == 0)
                return;

            // EQ compares spell effects slot by slot (in the effects list) which is why those
            // CHA spacers are in there.  Azerothcore does allow a spell be a multiple groups.
            foreach (int effectStackKey in effectStackKeys)
            {
                // Consider bard songs differently since they only conflict with each other, and split detrimental from
                // beneficial since buffs and debuffs of the same effect
                int compositeKey = (effectStackKey * 8) + (isBardSongAura ? 2 : 0) + (isDetrimental ? 1 : 0);
                spellTemplate.SpellGroupStackingIDs.Add(GetOrCreateSpellGroupID(compositeKey));
                spellTemplate.AuraStackEffectKeys.Add(effectStackKey);
            }
            spellTemplate.AuraStackKeyFlagBits = (isBardSongAura ? 2 : 0) + (isDetrimental ? 1 : 0);
        }

        private static int GetOrCreateSpellGroupID(int compositeKey)
        {
            if (SpellGroupIDByEffectStackKey.TryGetValue(compositeKey, out int groupStackingID) == false)
            {
                groupStackingID = IDGenerationTool.GenerateID("SpellGroupID", compositeKey.ToString());
                SpellGroupIDByEffectStackKey[compositeKey] = groupStackingID;
                SpellGroupStackRuleByGroup[groupStackingID] = 4; // SPELL_GROUP_STACK_RULE_EXCLUSIVE_HIGHEST
            }
            return groupStackingID;
        }

        // Combine similar EQ effect type with a raw slot for slot calculations, to factor for spacers (CHA placeholders)
        private static int MakeEffectStackKey(SpellEQEffectType effectType, int slot)
        {
            return ((int)effectType * 16) + slot; // slot is 1-12
        }

        // Only add stack effect keys for an effect if it's something we want stack control on
        private static void AddStackingEffectKeys(SpellEffectEQ eqEffect, HashSet<int> effectStackKeys)
        {
            int slot = eqEffect.EQEffectSlot;
            switch (eqEffect.EQEffectType)
            {
                // Non-Binary Buffs/Debuffs. Zero values are spacers for slot comparisons
                case SpellEQEffectType.ArmorClass:
                case SpellEQEffectType.Attack:
                case SpellEQEffectType.MovementSpeed:    // Run speed + / - compete
                case SpellEQEffectType.Strength:
                case SpellEQEffectType.Dexterity:
                case SpellEQEffectType.Agility:
                case SpellEQEffectType.Stamina:
                case SpellEQEffectType.Intelligence:
                case SpellEQEffectType.Wisdom:
                case SpellEQEffectType.Charisma:
                case SpellEQEffectType.AttackSpeed:      // Haste vs slow compete
                case SpellEQEffectType.AttackSpeed2:     // This actually stacks with AttackSpeed 1
                case SpellEQEffectType.ResistFire:
                case SpellEQEffectType.ResistCold:
                case SpellEQEffectType.ResistPoison:
                case SpellEQEffectType.ResistDisease:
                case SpellEQEffectType.ResistMagic:
                case SpellEQEffectType.Rune:
                case SpellEQEffectType.DamageShield:
                case SpellEQEffectType.TotalHP:
                case SpellEQEffectType.TotalMana:
                case SpellEQEffectType.HealOverTime:
                    if (eqEffect.EQBaseValue != 0)
                        effectStackKeys.Add(MakeEffectStackKey(eqEffect.EQEffectType, slot));
                    break;
                case SpellEQEffectType.CurrentHitPoints:
                case SpellEQEffectType.CurrentMana:
                    if (eqEffect.EQBaseValue > 0)
                        effectStackKeys.Add(MakeEffectStackKey(eqEffect.EQEffectType, slot));
                    break;
                // Binary effects
                case SpellEQEffectType.InvisibilityUnstable:
                case SpellEQEffectType.SeeInvisibility:
                case SpellEQEffectType.WaterBreathing:
                case SpellEQEffectType.Invisibility:
                case SpellEQEffectType.InvisVsUndead:
                case SpellEQEffectType.InvisVsUndead2:
                case SpellEQEffectType.Levitate:
                case SpellEQEffectType.Illusion:
                case SpellEQEffectType.ModelSize:
                    effectStackKeys.Add(MakeEffectStackKey(eqEffect.EQEffectType, slot));
                    break;
                // Resist All overwrites specific resists
                case SpellEQEffectType.ResistAll:
                    if (eqEffect.EQBaseValue != 0)
                    {
                        effectStackKeys.Add(MakeEffectStackKey(SpellEQEffectType.ResistFire, slot));
                        effectStackKeys.Add(MakeEffectStackKey(SpellEQEffectType.ResistCold, slot));
                        effectStackKeys.Add(MakeEffectStackKey(SpellEQEffectType.ResistPoison, slot));
                        effectStackKeys.Add(MakeEffectStackKey(SpellEQEffectType.ResistDisease, slot));
                        effectStackKeys.Add(MakeEffectStackKey(SpellEQEffectType.ResistMagic, slot));
                    }
                    break;
                // All else are things should probably be skipped.  Nukes, pets, blah blah
                default:
                    break;
            }
        }

        private static void SetActionAndAuraDescriptions(ref SpellTemplate spellTemplate, SpellTemplate? recourseSpellTemplate, SpellTemplate? procLinkSpellTemplate)
        {
            // Action Description
            spellTemplate.Description = GenerateActionDescription(spellTemplate);
            if (recourseSpellTemplate != null)
                spellTemplate.Description = string.Concat(spellTemplate.Description, "\n\nOn success also cast:\n", recourseSpellTemplate.Name, "\n", GenerateActionDescription(recourseSpellTemplate));
            if (procLinkSpellTemplate != null)
                spellTemplate.Description = string.Concat(spellTemplate.Description, "\n\nSometimes on hit cast:\n", procLinkSpellTemplate.Name, "\n", GenerateActionDescription(procLinkSpellTemplate));
            bool focusBoostWasAddedToDescription = false;
            if (spellTemplate.ShowFocusBoostInDescriptionIfExists == true)
            {
                string songSkillTypeString = string.Empty;
                switch (spellTemplate.FocusBoostType)
                {
                    case SpellFocusBoostType.BardBrassInstruments: songSkillTypeString = "brass instruments"; break;
                    case SpellFocusBoostType.BardStringedInstruments: songSkillTypeString = "string instruments"; break;
                    case SpellFocusBoostType.BardWindInstruments: songSkillTypeString = "wind instruments"; break;
                    case SpellFocusBoostType.BardPercussionInstruments: songSkillTypeString = "percussion instruments"; break;
                    case SpellFocusBoostType.BardSinging: songSkillTypeString = "singing aids"; break;
                    default: break;
                }
                if (songSkillTypeString.Length > 0)
                {
                    spellTemplate.Description = string.Concat(spellTemplate.Description, "\n\nEnhanced by ", songSkillTypeString, ".");
                    focusBoostWasAddedToDescription = true;
                }
            }

            // Aura Description
            spellTemplate.AuraDescription = GenerateAuraDescription(spellTemplate);
            if (procLinkSpellTemplate != null)
            {
                if (spellTemplate.AuraDescription.Length == 0)
                    spellTemplate.AuraDescription = string.Concat("Sometimes casts ", procLinkSpellTemplate.Name, " on strike.");
                else
                    spellTemplate.AuraDescription = string.Concat(spellTemplate.AuraDescription, " Sometimes casts ", procLinkSpellTemplate.Name, " on strike.");
            }

            // Add bard song aura limit to aura description and action description
            if (spellTemplate.IsBardSongAura == true && Configuration.SPELL_MAX_CONCURRENT_BARD_SONGS != 0)
            {
                // Make the string
                string songOrSongs = "song";
                if (Configuration.SPELL_MAX_CONCURRENT_BARD_SONGS > 1)
                    songOrSongs = "songs";
                string concurrentString = string.Concat("Only ", Configuration.SPELL_MAX_CONCURRENT_BARD_SONGS.ToString(), " ", songOrSongs,
                    " can play at once, and the oldest is canceled if exceeded.");

                // Action Description
                if (focusBoostWasAddedToDescription == false)
                    spellTemplate.Description = string.Concat(spellTemplate.Description, "\n\n");
                else
                    spellTemplate.Description = string.Concat(spellTemplate.Description, " ");
                spellTemplate.Description = string.Concat(spellTemplate.Description, concurrentString);

                // Aura Description
                spellTemplate.AuraDescription = string.Concat(spellTemplate.AuraDescription, "\n\n", concurrentString);
            }
        }

        private static string GenerateActionDescription(SpellTemplate spellTemplate)
        {
            StringBuilder descriptionSB = new StringBuilder();

            // Songs should have something
            if (spellTemplate.IsBardSongAura)
                descriptionSB.Append("Perform a continuous song that ");

            // Add the description blocks
            bool descriptionTextHasBeenAddedToAction = false;
            for (int i = 0; i < spellTemplate.WOWSpellEffects.Count; i++)
            {
                SpellEffectWOW spellEffectWOW = spellTemplate.WOWSpellEffects[i];
                if (spellEffectWOW.ActionDescription.Length > 0)
                {
                    if (descriptionTextHasBeenAddedToAction == true)
                        descriptionSB.Append(", ");
                    descriptionSB.Append(spellTemplate.WOWSpellEffects[i].ActionDescription);
                    descriptionTextHasBeenAddedToAction = true;
                }
            }
            if (descriptionTextHasBeenAddedToAction == false)
                return string.Empty;
            descriptionSB.Append('.');

            // Control capitalization
            descriptionSB[0] = char.ToUpper(descriptionSB[0]);

            if (spellTemplate.TargetDescriptionTextFragment.Length > 0)
            {
                descriptionSB.Append(" ");
                descriptionSB.Append(spellTemplate.TargetDescriptionTextFragment);
                descriptionSB.Append(".");
            }
            descriptionSB.Append(GetTimeDurationStringFromMSWithLeadingSpace(spellTemplate.AuraDuration.MaxDurationInMS, spellTemplate.AuraDuration.GetTimeText()));

            // Add any additional fragments to descriptions
            if (spellTemplate.BreakEffectOnNonAutoDirectDamage == true)
                descriptionSB.Append(" May break on direct damage.");
            if (spellTemplate.IsNegateIfCombat == true)
                descriptionSB.Append(" Breaks if you cast a spell or attack.");

            // Capitalize Norrath
            descriptionSB.Replace("norrath", "Norrath");

            return descriptionSB.ToString();
        }

        private static string GenerateAuraDescription(SpellTemplate spellTemplate)
        {
            string auraDescription = BuildAuraDescriptionFromEffects(spellTemplate.WOWSpellEffects);
            if (auraDescription.Length == 0)
                return string.Empty;

            // Bard song auras need target information
            if (spellTemplate.IsBardSongAura && spellTemplate.TargetDescriptionTextFragment.Length > 0)
                auraDescription = string.Concat(auraDescription, " ", spellTemplate.TargetDescriptionTextFragment, ".");

            return auraDescription;
        }

        private static string BuildAuraDescriptionFromEffects(IEnumerable<SpellEffectWOW> spellEffects)
        {
            StringBuilder auraSB = new StringBuilder();
            bool descriptionTextHasBeenAddedToAura = false;
            foreach (SpellEffectWOW spellEffectWOW in spellEffects)
            {
                string effectAuraDescription = spellEffectWOW.RenderAuraDescription();
                if (effectAuraDescription.Length > 0)
                {
                    if (descriptionTextHasBeenAddedToAura == true)
                        auraSB.Append(", ");
                    auraSB.Append(effectAuraDescription);
                    descriptionTextHasBeenAddedToAura = true;
                }
            }
            if (auraSB.Length == 0)
                return string.Empty;

            // Store and control capitalization
            auraSB.Append('.');
            auraSB[0] = char.ToUpper(auraSB[0]);

            return auraSB.ToString();
        }

        private static string GetTimeDurationStringFromMSWithLeadingSpace(int durationInMS, string timeFragment)
        {
            // Skip no duration
            if (durationInMS < 1000)
                return string.Empty;

            // Get the time string
            return string.Concat(" Lasts ", timeFragment, ".");
        }

        private static string GetTimeTextFromSeconds(int inputSeconds)
        {
            StringBuilder timeSB = new StringBuilder();
            int hours = inputSeconds / 3600;
            if (hours > 0)
            {
                timeSB.Append(hours);
                timeSB.Append(" hour");
                if (hours != 1)
                    timeSB.Append("s");
            }
            int minutes = (inputSeconds % 3600) / 60;
            if (minutes > 0)
            {
                if (hours > 0)
                    timeSB.Append(" ");
                timeSB.Append(minutes);
                timeSB.Append(" minute");
                if (minutes != 1)
                    timeSB.Append("s");
            }
            int seconds = (inputSeconds % 60);
            if (seconds > 0)
            {
                if (hours > 0 || minutes > 0)
                    timeSB.Append(" ");
                timeSB.Append(seconds);
                timeSB.Append(" second");
                if (seconds != 1)
                    timeSB.Append("s");
            }
            return timeSB.ToString();
        }

        private static bool BlockHasAura(SpellEffectBlock effectBlock)
        {
            foreach (SpellEffectWOW spellEffect in effectBlock.SpellEffects)
                if (spellEffect.IsAuraType() == true)
                    return true;
            return false;
        }

        public int GenerateWornSpellVariant(int primaryWornWOWSpellID, int wornFixedLevel)
        {
            if (primaryWornWOWSpellID <= 0)
            {
                Logger.WriteError("GenerateWornSpellVariant called with an invalid primary worn spell ID for spell eq id ", EQSpellID.ToString());
                return 0;
            }

            // Ensure the base blocks are generated before cloning from them
            List<SpellEffectBlock> baseBlocks = GroupedBaseSpellEffectBlocksForOutput;

            // Worn effects are item bonuses in EQ that never conflict with cast buffs, so they get their own stack groups where they only compete with other worn copies of the same effect
            if (WornSpellGroupStackingIDs.Count == 0)
                foreach (int effectStackKey in AuraStackEffectKeys)
                    WornSpellGroupStackingIDs.Add(GetOrCreateSpellGroupID((effectStackKey * 8) + 4 + AuraStackKeyFlagBits));

            List<SpellEffectBlock> wornBlocks = new List<SpellEffectBlock>();
            List<SpellEffectWOW> wornEffectsForDescription = new List<SpellEffectWOW>();
            foreach (SpellEffectBlock baseBlock in baseBlocks)
            {
                SpellEffectBlock wornBlock = new SpellEffectBlock();
                if (wornBlocks.Count == 0)
                    wornBlock.WOWSpellID = primaryWornWOWSpellID;
                else
                    wornBlock.WOWSpellID = IDGenerationTool.GenerateID("SpellID", "wornblock", primaryWornWOWSpellID.ToString(), wornBlocks.Count.ToString());
                wornBlock.SpellName = string.Concat(baseBlock.SpellName, " (from gear)");
                wornBlock.ForceVisibleSplitAura = baseBlock.ForceVisibleSplitAura;
                foreach (SpellEffectWOW baseEffect in baseBlock.SpellEffects)
                {
                    SpellEffectWOW wornEffect = baseEffect.Clone();
                    if (wornFixedLevel != 0)
                        wornEffect.FixValueToLevel(wornFixedLevel);
                    wornBlock.SpellEffects.Add(wornEffect);
                    wornEffectsForDescription.Add(wornEffect);
                }
                wornBlocks.Add(wornBlock);
            }

            // Fixed level worn items don't scale, so make a fixed description
            if (wornFixedLevel != 0)
            {
                string fixedAuraDescription = BuildAuraDescriptionFromEffects(wornEffectsForDescription);
                if (fixedAuraDescription.Length > 0)
                    wornBlocks[0].AuraDescriptionOverride = fixedAuraDescription;
            }

            ItemWornSpellEffectBlockSets.Add(wornBlocks);
            return primaryWornWOWSpellID;
        }

        private void GenerateOutputEffectBlocks()
        {
            if (_GroupedBaseSpellEffectBlocksForOutput.Count != 0)
            {
                Logger.WriteError("Attempted to call GenerateOutputEffectBlocks more than once for spell eq id ", EQSpellID.ToString());
                return;
            }

            // Blank out if no spell effects
            if (WOWSpellEffects.Count == 0)
            {
                SpellEffectBlock blankEffectBlock = new SpellEffectBlock();
                blankEffectBlock.WOWSpellID = WOWSpellID;
                for (int i = 0; i < 3; i++)
                    blankEffectBlock.SpellEffects.Add(new SpellEffectWOW());
                _GroupedBaseSpellEffectBlocksForOutput.Add(blankEffectBlock);
                return;
            }

            // Pre-group by 'max' levels so that spell value calculations work properly
            Dictionary<int, List<SpellEffectWOW>> spellEffectsByMaxLevel = new Dictionary<int, List<SpellEffectWOW>>();
            foreach (SpellEffectWOW spellEffect in WOWSpellEffects)
            {
                if (spellEffectsByMaxLevel.ContainsKey(spellEffect.CalcEffectHighLevel) == false)
                    spellEffectsByMaxLevel.Add(spellEffect.CalcEffectHighLevel, new List<SpellEffectWOW>());
                spellEffectsByMaxLevel[spellEffect.CalcEffectHighLevel].Add(spellEffect);
            }

            // Otherwise, group in batches of 3 adding blanks to pad the space
            // Group these by maximum level of related effects
            foreach (var curEffectList in spellEffectsByMaxLevel.Values)
            {
                int numOfExtractedEffects = 0;
                while (numOfExtractedEffects < curEffectList.Count)
                {
                    SpellEffectBlock baseEffectBlock = new SpellEffectBlock();
                    for (int i = 0; i < 3; i++)
                    {
                        if (numOfExtractedEffects >= curEffectList.Count)
                            baseEffectBlock.SpellEffects.Add(new SpellEffectWOW());
                        else
                            baseEffectBlock.SpellEffects.Add(curEffectList[numOfExtractedEffects]);
                        numOfExtractedEffects += 1;
                    }
                    if (_GroupedBaseSpellEffectBlocksForOutput.Count == 0)
                    {
                        baseEffectBlock.SpellName = Name;
                        baseEffectBlock.WOWSpellID = WOWSpellID;
                    }
                    else
                    {
                        // When a primary block has no aura (like a damage spell), the split has the aura so we need to display the icon
                        if (BlockHasAura(_GroupedBaseSpellEffectBlocksForOutput[0]) == false && BlockHasAura(baseEffectBlock) == true)
                        {
                            baseEffectBlock.SpellName = Name;
                            baseEffectBlock.ForceVisibleSplitAura = true;
                        }
                        else
                            baseEffectBlock.SpellName = string.Concat(Name, " Split ", _GroupedBaseSpellEffectBlocksForOutput.Count.ToString());
                        baseEffectBlock.WOWSpellID = IDGenerationTool.GenerateID("SpellID", "basesplit", WOWSpellID.ToString(), _GroupedBaseSpellEffectBlocksForOutput.Count.ToString());
                    }
                    _GroupedBaseSpellEffectBlocksForOutput.Add(baseEffectBlock);
                }
            }

            // Good proc and clicky spells each need copies of the base blocks
            GenerateMissingGoodProcOutputEffectBlocks();
            GenerateMissingClickyOutputEffectBlocks();
        }

        private void GenerateMissingGoodProcOutputEffectBlocks()
        {
            if (WOWSpellIDProcAndGoodEffect <= 0 || _GroupedGoodProcSpellEffectBlocksForOutput.Count > 0)
                return;
            if (WOWSpellEffects.Count == 0)
                return;

            foreach (SpellEffectBlock baseEffectBlock in _GroupedBaseSpellEffectBlocksForOutput)
            {
                SpellEffectBlock goodProcEffectBlock = new SpellEffectBlock();
                if (_GroupedGoodProcSpellEffectBlocksForOutput.Count == 0)
                    goodProcEffectBlock.WOWSpellID = WOWSpellIDProcAndGoodEffect;
                else
                    goodProcEffectBlock.WOWSpellID = IDGenerationTool.GenerateID("SpellID", "goodprocblock", WOWSpellIDProcAndGoodEffect.ToString(), _GroupedGoodProcSpellEffectBlocksForOutput.Count.ToString());
                foreach (SpellEffectWOW spellEffect in baseEffectBlock.SpellEffects)
                {
                    // Override the implicit target since the user is the only one it can be used on
                    SpellEffectWOW effectClone = spellEffect.Clone();
                    effectClone.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
                    goodProcEffectBlock.SpellEffects.Add(effectClone);
                }
                goodProcEffectBlock.SpellName = baseEffectBlock.SpellName;
                _GroupedGoodProcSpellEffectBlocksForOutput.Add(goodProcEffectBlock);
            }
        }

        private void GenerateMissingClickyOutputEffectBlocks()
        {
            // Spells with no effects don't get clicky blocks (matches the base block blank-out behavior)
            if (WOWSpellEffects.Count == 0)
                return;

            for (int clickyIndex = _GroupedClickySpellEffectBlocksForOutputBySpellParameters.Count; clickyIndex < ClickySpellParatemers.Count; clickyIndex++)
            {
                ClickySpellParameters clickySpellParameters = ClickySpellParatemers[clickyIndex];
                List<SpellEffectBlock> clickyBlocks = new List<SpellEffectBlock>();

                // Collects fixed-level clicky (tiered potion) effects so exact-value descriptions can be built
                List<SpellEffectWOW> clickyEffectsForDescription = new List<SpellEffectWOW>();
                foreach (SpellEffectBlock baseEffectBlock in _GroupedBaseSpellEffectBlocksForOutput)
                {
                    SpellEffectBlock clickEffectBlock = new SpellEffectBlock();
                    if (clickyBlocks.Count == 0)
                        clickEffectBlock.WOWSpellID = clickySpellParameters.WOWSpellID;
                    else
                        clickEffectBlock.WOWSpellID = IDGenerationTool.GenerateID("SpellID", "clickyblock", clickySpellParameters.WOWSpellID.ToString(), clickyBlocks.Count.ToString());
                    foreach (SpellEffectWOW spellEffect in baseEffectBlock.SpellEffects)
                    {
                        SpellEffectWOW effectClone = spellEffect.Clone();
                        if (clickySpellParameters.IsForcedSelfOnly == true)
                            effectClone.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
                        if (clickySpellParameters.FixedLevel > 0)
                        {
                            effectClone.FixValueToLevel(clickySpellParameters.FixedLevel);
                            clickyEffectsForDescription.Add(effectClone);
                        }
                        clickEffectBlock.SpellEffects.Add(effectClone);
                    }
                    clickEffectBlock.SpellName = string.Concat(baseEffectBlock.SpellName);
                    clickyBlocks.Add(clickEffectBlock);
                }
                _GroupedClickySpellEffectBlocksForOutputBySpellParameters.Add(clickyBlocks);

                // Fixed level clickies (tiered potions) don't scale, so make fixed descriptions like fixed worn items get
                if (clickyEffectsForDescription.Count > 0)
                {
                    string fixedAuraDescription = BuildAuraDescriptionFromEffects(clickyEffectsForDescription);
                    if (fixedAuraDescription.Length > 0)
                    {
                        SpellEffectBlock firstClickyBlock = clickyBlocks[0];
                        firstClickyBlock.AuraDescriptionOverride = fixedAuraDescription;

                        // Item tooltips read the click spell's description, so rebuild it with the exact values and pinned duration
                        StringBuilder actionDescriptionSB = new StringBuilder(fixedAuraDescription);
                        if (TargetDescriptionTextFragment.Length > 0)
                        {
                            actionDescriptionSB.Append(" ");
                            actionDescriptionSB.Append(TargetDescriptionTextFragment);
                            actionDescriptionSB.Append(".");
                        }
                        int fixedDurationInMS = AuraDuration.GetBuffDurationForLevel(clickySpellParameters.FixedLevel);
                        actionDescriptionSB.Append(GetTimeDurationStringFromMSWithLeadingSpace(fixedDurationInMS, AuraDuration.GetTimeTextForDurationInMS(fixedDurationInMS)));
                        firstClickyBlock.ActionDescriptionOverride = actionDescriptionSB.ToString();
                    }
                }
            }
        }
    
        public static int GenerateUniqueEQSpellID()
        {
            lock (SpellEQIDLock)
            {
                int returnID = CUR_GENERATED_EQ_SPELL_ID;
                CUR_GENERATED_EQ_SPELL_ID++;
                return returnID;
            }
        }
    }
}
