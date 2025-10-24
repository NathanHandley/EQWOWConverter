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

        public static Dictionary<int, int> SpellCastTimeDBCIDsByCastTime = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellRangeDBCIDsBySpellRange = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellRadiusDBCIDsBySpellRadius = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellGroupStackRuleByGroup = new Dictionary<int, int>();

        private static Dictionary<int, SpellTemplate> SpellTemplatesByEQID = new Dictionary<int, SpellTemplate>();
        private static Dictionary<(ItemFocusType, int), SpellTemplate> SpellTemplatesByFocusTypeAndValue = new Dictionary<(ItemFocusType, int), SpellTemplate>();
        private static readonly object SpellTemplateLock = new object();
        private static int CUR_GENERATED_WOW_SPELL_ID = Configuration.DBCID_SPELL_ID_GENERATED_START;
        private static readonly object SpellEQIDLock = new object();
        private static int CUR_GENERATED_EQ_SPELL_ID = 5000;

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
        public int WOWSpellIDWorn = -1;
        public int EQSpellID = -1;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public string AuraDescription = string.Empty;
        public UInt32 Category = 1;
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
                    SpellCastTimeDBCIDsByCastTime.Add(value, SpellCastTimesDBC.GenerateDBCID());
                _SpellCastTimeDBCID = SpellCastTimeDBCIDsByCastTime[value];
                _CastTimeInMS = value;
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
        protected int _SpellRadiusDBCID = 0;
        public int SpellRadiusDBCID { get { return _SpellRadiusDBCID; } }
        protected int _SpellRadius = 0;
        public int SpellRadius
        {
            get { return _SpellRadius; }
            set
            {
                if (SpellRadiusDBCIDsBySpellRadius.ContainsKey(value) == false)
                    SpellRadiusDBCIDsBySpellRadius.Add(value, SpellRadiusDBC.GenerateDBCID());
                _SpellRadiusDBCID = SpellRadiusDBCIDsBySpellRadius[value];
                _SpellRadius = value;
            }
        }
        public int EQAOERange = 0; // This is used as a data field for illusions
        public int EQBuffDurationInTicks = 0;
        public int EQBuffDurationFormula = 0;
        public SpellDuration AuraDuration = new SpellDuration();
        public int SpellGroupStackingID = -1;
        public int SpellGroupStackingRule = 0;
        public UInt32 RecoveryTimeInMS = 0; // Note that this may be zero for a player but not a creature.  See Configuration.SPELL_RECOVERY_TIME_MINIMUM_IN_MS
        public int EQSpellVisualEffectIndex = 0;
        public UInt32 SpellVisualID1 = 0;
        public UInt32 SpellVisualID2 = 0;
        public bool PlayerLearnableByClassTrainer = false; // Needed?
        public int MinimumPlayerLearnLevel = -1;
        public bool HasEffectBaseFormulaUsingSpellLevel = false;
        public int RequiredAreaIDs = -1;
        public UInt32 SchoolMask = 1;
        public UInt32 DispelType = 0;
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
        public bool CanTargetBothFriendlyAndEnemy = false;
        public UInt32 TargetCreatureType = 0; // No specific creature type
        public bool CastOnCorpse = false;
        public Dictionary<ClassType, SpellLearnScrollProperties> LearnScrollPropertiesByClassType = new Dictionary<ClassType, SpellLearnScrollProperties>();
        public int HighestDirectHealAmountInSpellEffect = 0; // Used in spell priority calculations
        private string TargetDescriptionTextFragment = string.Empty;
        public bool BreakEffectOnNonAutoDirectDamage = false;
        public bool BreakEffectOnAllDamage = false;
        public bool NoPartialImmunity = false;
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
        public int ProcLinkEQSpellID = 0;
        public int WOWSpellIDCastOnMeleeAttacker = 0;
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
        private List<SpellEffectBlock> _GroupedBaseSpellEffectBlocksForOutput = new List<SpellEffectBlock>();
        public bool IsBardSongAura = false;
        public bool HasAdditionalTickOnApply = false;
        public bool InterruptOnMovement = true;
        public bool InterruptOnSchoolLockdown = true;
        public bool InterruptOnPushback = true;
        public bool InterruptOnCast = true;
        public bool InterruptOnDamageTaken = false;
        public bool PreventAuraClickOff = false;
        public SpellFocusBoostType FocusBoostType = SpellFocusBoostType.None;
        public bool IsFocusBoostableEffect = false;
        public bool IsToggleAura = false;
        public int PeriodicAuraWOWSpellID = 0;
        public int PeriodicAuraSpellRadius = 0;
        public bool ShowFocusBoostInDescriptionIfExists = false;
        public bool IsllusionSpellParent = false;
        public SpellTemplate? IllusionSpellParent = null;
        public int MaleFormSpellTemplateID = 0;
        public int FemaleFormSpellTemplateID = 0;

        public List<SpellEffectBlock> GroupedBaseSpellEffectBlocksForOutput
        {
            get
            {
                if (_GroupedBaseSpellEffectBlocksForOutput.Count == 0)
                    GenerateOutputEffectBlocks();
                return _GroupedBaseSpellEffectBlocksForOutput;
            }
        }
        private List<SpellEffectBlock> _GroupedWornSpellEffectBlocksForOutput = new List<SpellEffectBlock>();
        public List<SpellEffectBlock> GroupedWornSpellEffectBlocksForOutput
        {
            get
            {
                if (_GroupedWornSpellEffectBlocksForOutput.Count == 0)
                    GenerateOutputEffectBlocks();
                return _GroupedWornSpellEffectBlocksForOutput;
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

        public static void LoadSpellTemplates(SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID, Dictionary<string, ZoneProperties> zonePropertiesByShortName,
            ref Dictionary<int, CreatureTemplate> creatureTemplatesByEQID)
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
                newSpellTemplate.WOWSpellIDWorn = int.Parse(columns["wow_worn_id"]);
                newSpellTemplate.Name = columns["name"];
                newSpellTemplate.SpellRange = Convert.ToInt32(float.Parse(columns["range"]) * Configuration.SPELLS_RANGE_MULTIPLIER);
                newSpellTemplate.EQAOERange = int.Parse(columns["aoerange"]);
                newSpellTemplate.SpellRadius = Convert.ToInt32(Convert.ToSingle(newSpellTemplate.EQAOERange) * Configuration.SPELLS_RANGE_MULTIPLIER);
                newSpellTemplate.Category = 0; // Temp / TODO: Figure out how/what to set here
                newSpellTemplate.CastTimeInMS = int.Parse(columns["cast_time"]);
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

                // Generic properties
                PopulateAllClassLearnScrollProperties(ref newSpellTemplate, columns);
                newSpellTemplate.ManaCost = Convert.ToUInt32(columns["mana"]);
                bool isDetrimental = int.Parse(columns["goodEffect"]) == 0 ? true : false; // "2" should be non-detrimental group only (not caster).  Ignoring that for now.

                // Buff duration (if any)
                newSpellTemplate.EQBuffDurationInTicks = Convert.ToInt32(columns["buffduration"]);
                newSpellTemplate.EQBuffDurationFormula = Convert.ToInt32(columns["buffdurationformula"]);
                if (newSpellTemplate.EQBuffDurationFormula != 0 || newSpellTemplate.IsModelSizeChangeSpell == true)
                    newSpellTemplate.AuraDuration.CalculateAndSetAuraDuration(newSpellTemplate.MinimumPlayerLearnLevel, newSpellTemplate.EQBuffDurationFormula,
                        newSpellTemplate.EQBuffDurationInTicks, newSpellTemplate.IsModelSizeChangeSpell);

                // Focus and Bard properties
                int skillID = int.Parse(columns["skill"]);
                newSpellTemplate.FocusBoostType = GetFocusBoostType(skillID);
                if ((skillID == 12 || skillID == 41 || skillID == 49 || skillID == 54 || skillID == 70) && newSpellTemplate.RecoveryTimeInMS == 0 && newSpellTemplate.EQBuffDurationInTicks != 0)
                    newSpellTemplate.IsBardSongAura = true;
                if (newSpellTemplate.FocusBoostType != SpellFocusBoostType.None && newSpellTemplate.IsBardSongAura == false)
                    newSpellTemplate.IsFocusBoostableEffect = true;

                // Icon
                int spellIconID = int.Parse(columns["icon"]);
                if (spellIconID >= 2500)
                    newSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(spellIconID - 2500);

                // Visual
                newSpellTemplate.EQSpellVisualEffectIndex = int.Parse(columns["SpellVisualEffectIndex"]);
                if (newSpellTemplate.EQSpellVisualEffectIndex >= 0 && newSpellTemplate.EQSpellVisualEffectIndex < 52)
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

                // Set defensive properties
                newSpellTemplate.DefenseType = 1; // Magic
                newSpellTemplate.PreventionType = 1; // Silence

                // Get targets and convert the spell effects
                int eqTargetTypeID = int.Parse(columns["targettype"]);
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

                // Stacking rules
                SetAuraStackRule(ref newSpellTemplate, int.Parse(columns["spell_category"]), newSpellTemplate.IsBardSongAura);
                for (int i = 0; i < effectGeneratedSpellTemplates.Count; i++)
                {
                    SpellTemplate effectGeneratedSpellTemplate = effectGeneratedSpellTemplates[i];
                    SetAuraStackRule(ref effectGeneratedSpellTemplate, int.Parse(columns["spell_category"]), effectGeneratedSpellTemplate.IsBardSongAura);
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

                // Copy any form effects to the child illusion effect spells
                if (spellTemplate.IllusionSpellParent != null)
                {
                    foreach (SpellEffectWOW spellEffect in spellTemplate.IllusionSpellParent.WOWSpellEffects)
                    {
                        // Dummy is used for the form change trigger, so skip that
                        if (spellEffect.EffectType == SpellWOWEffectType.Dummy)
                            continue;
                        spellTemplate.WOWSpellEffects.Add(spellEffect);
                    }
                }

                // Set the spell and aura descriptions
                SetActionAndAuraDescriptions(ref spellTemplate, recourseSpellTemplate, procLinkSpellTemplate);
            }
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
                int enchantID = SpellItemEnchantmentDBC.GenerateUniqueID();

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

                // Don't do anything if it already exists
                if (SpellTemplatesByFocusTypeAndValue.ContainsKey((focusType, focusValue)) == true)
                {
                    focusSpellTemplate =  SpellTemplatesByFocusTypeAndValue[(focusType, focusValue)];
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
                focusSpellTemplate.WOWSpellID = GenerateUniqueWOWSpellID();
                focusSpellTemplate.EQSpellID = GenerateUniqueEQSpellID();
                focusSpellTemplate.Description = description;
                focusSpellTemplate.AuraDescription = description;
                focusSpellTemplate.WOWSpellEffects.Add(new SpellEffectWOW(SpellWOWEffectType.ApplyAura, SpellWOWAuraType.Dummy, 0, 0, 0, 1, (int)spellDummyType, focusValue));
                focusSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForItemIconID(itemIconID);
                focusSpellTemplate.SpellVisualID1 = 0; // No visual
                focusSpellTemplate.PreventAuraClickOff = true;
                focusSpellTemplate.AuraDuration = new SpellDuration();
                focusSpellTemplate.AuraDuration.IsInfinite = true;
                SpellTemplatesByFocusTypeAndValue.Add((focusType, focusValue), focusSpellTemplate);
            }
        }

        private static void PopulateAllClassLearnScrollProperties(ref SpellTemplate spellTemplate, Dictionary<string, string> rowColumns)
        {
            // In EQ, a scroll can be learned by multiple classes at different levels
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Warrior, "warrior", "bard");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Paladin, "paladin");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Hunter, "ranger", "beastlord");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Rogue, "monk", "rogue");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Priest, "cleric", "enchanter");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.DeathKnight, "shadowknight");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Shaman, "shaman");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Mage, "magician", "wizard");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Warlock, "necromancer");
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
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitTargetEnemy);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.UnitDestinationAreaEnemy);
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

            // Teleports use fixed columns for values
            if (curEffect.EQEffectType == SpellEQEffectType.Teleport)
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
            effectGeneratedSpellTemplate.WOWSpellID = GenerateUniqueWOWSpellID();
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

                if (spellTemplate.IsTransferEffectType)
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
                                newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ManaUpDirect", SpellEffectWOWConversionScaleType.CastTime);
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.PowerDrain;
                                newSpellEffectWOW.EffectMiscValueA = 0; // Power Type = Mana
                                newSpellEffectWOW.EffectMultipleValue = 1;
                                newSpellEffectWOW.ActionDescription = string.Concat("drain ", newSpellEffectWOW.GetFormattedEffectActionString(false), " mana from the target and add it to your own");
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
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HealDirectHPS", SpellEffectWOWConversionScaleType.CastTime);
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
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageDirectDPS", SpellEffectWOWConversionScaleType.CastTime);
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
                                            newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageOverTimeDPS", SpellEffectWOWConversionScaleType.Periodic);
                                            newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds and return it as life to yourself");
                                            newSpellEffectWOW.AuraDescription = string.Concat("suffering", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds and return it as life to yourself");
                                        }
                                        else
                                        {
                                            newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageOverTimeDPS", SpellEffectWOWConversionScaleType.Periodic);
                                            newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds and return it as life to yourself");
                                            newSpellEffectWOW.AuraDescription = string.Concat("suffering", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds and return it as life to yourself");
                                        }
                                    }
                                    newSpellEffectWOW.EffectMultipleValue = 1;
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
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
                                newSpellEffectWOW.AuraDescription = string.Concat(newSpellEffectWOW.GetFormattedEffectAuraString(false, "", ""), "strength stolen by the caster");
                                newSpellEffects.Add(newSpellEffectWOW);

                                // Make a second for the buff on the caster
                                SpellEffectWOW casterStrBuffEffect = newSpellEffectWOW.Clone();
                                casterStrBuffEffect.Invert();
                                casterStrBuffEffect.ActionDescription = string.Empty;
                                casterStrBuffEffect.AuraDescription = string.Empty;
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
                                newSpellEffectWOW.AuraDescription = string.Concat(newSpellEffectWOW.GetFormattedEffectAuraString(false, "", ""), "agility stolen by the caster");
                                newSpellEffects.Add(newSpellEffectWOW);

                                // Make a second for the buff on the caster
                                SpellEffectWOW casterAglBuffEffect = newSpellEffectWOW.Clone();
                                casterAglBuffEffect.Invert();
                                casterAglBuffEffect.ActionDescription = string.Empty;
                                casterAglBuffEffect.AuraDescription = string.Empty;
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
                                newSpellEffectWOW.AuraDescription = string.Concat(newSpellEffectWOW.GetFormattedEffectAuraString(false, "", ""), "armor stolen by the caster");
                                newSpellEffects.Add(newSpellEffectWOW);

                                // Make a second for the buff on the caster
                                SpellEffectWOW casterArmorBuffEffect = newSpellEffectWOW.Clone();
                                casterArmorBuffEffect.Invert();
                                casterArmorBuffEffect.ActionDescription = string.Empty;
                                casterArmorBuffEffect.AuraDescription = string.Empty;
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
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HealDirectHPS", SpellEffectWOWConversionScaleType.CastTime);
                                        newSpellEffectWOW.EffectType = SpellWOWEffectType.Heal;
                                        newSpellEffectWOW.ActionDescription = string.Concat("heal for ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                        spellTemplate.HighestDirectHealAmountInSpellEffect = Math.Max(spellTemplate.HighestDirectHealAmountInSpellEffect, newSpellEffectWOW.CalcEffectHighLevelValue);
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageDirectDPS", SpellEffectWOWConversionScaleType.CastTime);
                                        newSpellEffectWOW.EffectType = SpellWOWEffectType.SchoolDamage;
                                        if (elementalSchoolName.Length > 0)
                                            newSpellEffectWOW.ActionDescription = string.Concat("strike for ", newSpellEffectWOW.GetFormattedEffectActionString(false), " ", elementalSchoolName, " damage");
                                        else
                                            newSpellEffectWOW.ActionDescription = string.Concat("strike for ", newSpellEffectWOW.GetFormattedEffectActionString(false), " damage");
                                    }
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
                                else
                                {
                                    SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                    newSpellEffectWOW.EffectAuraPeriod = Convert.ToUInt32(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW) * 1000;
                                    if (eqEffect.EQBaseValue > 0)
                                    {
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HealOverTimeHPS", SpellEffectWOWConversionScaleType.Periodic);
                                        newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicHeal;
                                        newSpellEffectWOW.ActionDescription = string.Concat("regenerate ", newSpellEffectWOW.GetFormattedEffectActionString(false), " health per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                        newSpellEffectWOW.AuraDescription = string.Concat("regenerating", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " health per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicDamage;
                                        if (elementalSchoolName.Length > 0)
                                        {
                                            newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageOverTimeDPS", SpellEffectWOWConversionScaleType.Periodic);
                                            newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                            newSpellEffectWOW.AuraDescription = string.Concat("suffering", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                        }
                                        else
                                        {
                                            newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageOverTimeDPS", SpellEffectWOWConversionScaleType.Periodic);
                                            newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                            newSpellEffectWOW.AuraDescription = string.Concat("suffering", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                        }
                                    }
                                    newSpellEffects.Add(newSpellEffectWOW);
                                }
                            } break;
                        case SpellEQEffectType.CurrentMana:
                        case SpellEQEffectType.CurrentManaOnce:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;

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
                                        newSpellEffectWOW.AuraDescription = string.Concat("recovering", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PowerBurn;
                                        newSpellEffectWOW.EffectMultipleValue = 0;
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ManaDownOvertimeMPS", SpellEffectWOWConversionScaleType.Periodic);
                                        newSpellEffectWOW.ActionDescription = string.Concat("reduce ", newSpellEffectWOW.GetFormattedEffectActionString(false), " mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                        newSpellEffectWOW.AuraDescription = string.Concat("reducing", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("armor increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArmorClassDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease armor by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("armor decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.AttackSpeed2:
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("attack power increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "AttackDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease attack power by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("attack power decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                newSpellEffects.Add(newSpellEffectWOW);

                                // Add a second for ranged attack power
                                SpellEffectWOW newSpellEffectWOW2 = newSpellEffectWOW.Clone();
                                newSpellEffectWOW2.ActionDescription = string.Empty;
                                newSpellEffectWOW2.AuraDescription = string.Empty;
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("non-mounted movement speed increased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
                                }
                                else
                                {
                                    int eqMaxValue = eqEffect.EQMaxValue;
                                    if (eqMaxValue == 0 || eqMaxValue < Configuration.SPELLS_SLOWEST_MOVE_SPEED_EFFECT_VALUE)
                                        eqMaxValue = Configuration.SPELLS_SLOWEST_MOVE_SPEED_EFFECT_VALUE;
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModDecreaseSpeed;
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease movement speed by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.AuraDescription = string.Concat("movement speed decreased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("maximum health increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease maximum health by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("maximum health decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("strength increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "StrengthDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease strength by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("strength decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("agility increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "AgilityDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease agility by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("agility decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("stamina increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "StaminaDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease stamina by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("stamina decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("intellect increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("intellect decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("spirit increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "SpiritDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease spirit by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("spirit decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("hit chance increased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HitPctDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease hit chance by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.AuraDescription = string.Concat("hit chance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.AttackSpeed:
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("attack speed increased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease attack speed by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.AuraDescription = string.Concat("attack speed decreased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
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
                                effectGeneratedSpellTemplate.WOWSpellID = GenerateUniqueWOWSpellID();
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

                                // Make the stun effect
                                SpellEffectWOW stunSpellEffectWOW = new SpellEffectWOW();
                                stunSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                stunSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStun;
                                stunSpellEffectWOW.ActionDescription = string.Concat("stuns");
                                stunSpellEffectWOW.AuraDescription = string.Concat("stunned");
                                stunSpellEffectWOW.ImplicitTargetA = targets[0];
                                if (targets.Count == 2)
                                    stunSpellEffectWOW.ImplicitTargetB = targets[1];
                                stunSpellEffectWOW.EffectRadiusIndex = Convert.ToUInt32(spellRadiusIndex);
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("fire resistance increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "FireResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease fire resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("fire resistance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("frost resistance increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "FrostResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease frost resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("frost resistance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("nature resistance increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "NatureResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease nature resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("nature resistance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("shadow resistance increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ShadowResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease shadow resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("shadow resistance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("arcane resistance increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArcaneResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease arcane resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("arcane resistance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("all resistances increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArcaneResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("decrease all resistances by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    newSpellEffectWOW.AuraDescription = string.Concat("all resistances decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
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
                                    effectGeneratedSpellTemplate.WOWSpellID = GenerateUniqueWOWSpellID();
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
                                        newSpellEffectWOW.AuraDescription = string.Concat("reflecting", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " ", elementalSchoolName, " damage back at melee attackers");
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.ActionDescription = string.Concat("grants a damage shield that reflects ", newSpellEffectWOW.GetFormattedEffectActionString(false), " damage back melee attackers");
                                        newSpellEffectWOW.AuraDescription = string.Concat("reflecting", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " damage back at melee attackers");
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
                                spellTemplate.BreakEffectOnAllDamage = true;
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
                                spellTemplate.NoPartialImmunity = true;
                                newSpellEffects.Add(newSpellEffectWOW);
                            } break;
                        case SpellEQEffectType.Rune:
                            {
                                if (eqEffect.EQBaseValue == 0)
                                    continue;
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.SchoolAbsorb;
                                newSpellEffectWOW.EffectMiscValueA = 1; // Physical
                                newSpellEffectWOW.SetEffectAmountValues(Math.Abs(eqEffect.EQBaseValue), Math.Abs(eqEffect.EQMaxValue), spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "PhysicalAbsorb", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("applies a shield that absorbs ", newSpellEffectWOW.GetFormattedEffectActionString(false), " physical damage before breaking");
                                newSpellEffectWOW.AuraDescription = string.Concat("absorbing", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " physical damage");
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
                                    newSpellEffectWOW.AuraDescription = string.Concat("grown by ", newSpellEffectWOW.GetFormattedEffectAuraString(true, string.Empty, string.Empty));
                                }
                                else
                                {
                                    newSpellEffectWOW.ActionDescription = string.Concat("shrink the size of the target by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                    newSpellEffectWOW.AuraDescription = string.Concat("shrunk by ", newSpellEffectWOW.GetFormattedEffectAuraString(true, string.Empty, string.Empty));
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
                                    spellTemplate.SummonPropertiesDBCID = SummonPropertiesDBC.GenerateUniqueID();
                                    newSpellEffectWOW.EffectMiscValueB = spellTemplate.SummonPropertiesDBCID;
                                }
                                else
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.SummonPet;
                                newSpellEffects.Add(newSpellEffectWOW);

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

                                // Male form
                                SpellTemplate maleFormSpellTemplate = new SpellTemplate();
                                maleFormSpellTemplate.Name = string.Concat(spellTemplate.Name);
                                maleFormSpellTemplate.WOWSpellID = GenerateUniqueWOWSpellID();
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
                                float scaleMale = creatureRaceMale.Height * creatureRaceMale.SpawnSizeMod;
                                CreatureTemplate maleCreatureTemplate = CreatureTemplate.GenerateCreatureTemplate(maleFormSpellTemplate.Name, creatureRaceMale, creatureRaceMale.Gender, 0, textureID, 0, 0, scaleMale);
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

                                // Female form
                                SpellTemplate femaleFormSpellTemplate = new SpellTemplate();
                                femaleFormSpellTemplate.Name = string.Concat(spellTemplate.Name);
                                femaleFormSpellTemplate.WOWSpellID = GenerateUniqueWOWSpellID();
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
                                float scaleFemale = creatureRaceFemale.Height * creatureRaceFemale.SpawnSizeMod;
                                CreatureTemplate femaleCreatureTemplate = CreatureTemplate.GenerateCreatureTemplate(femaleFormSpellTemplate.Name, creatureRaceFemale, creatureRaceFemale.Gender, 0, textureID, 0, 0, scaleFemale);
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

                                // Parent illusion spell
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.Dummy;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                newSpellEffectWOW.EffectMiscValueA = (int)SpellDummyType.IllusionParent;
                                newSpellEffectWOW.ActionDescription = string.Concat("changes the form to ", textParticle, " ", raceName);
                                newSpellEffectWOW.AuraDescription = string.Concat("appear as ", textParticle, " ", raceName);
                                newSpellEffects.Add(newSpellEffectWOW);
                                spellTemplate.IsllusionSpellParent = true;
                            }
                            break;
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
                spellTemplate.WOWSpellEffects.Add(spellEffect);
            }

            // Sort them so the aura effects are last
            spellTemplate.WOWSpellEffects.Sort();
        }

        private static void SetAuraStackRule(ref SpellTemplate spellTemplate, int eqSpellCategory, bool isBardSongAura)
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
            int groupStackingID;
            if (isBardSongAura == true)
                groupStackingID = Configuration.SQL_SPELL_GROUP_ID_FOR_BARD_AURA_START + eqSpellCategory;
            else
                groupStackingID = Configuration.SQL_SPELL_GROUP_ID_START + eqSpellCategory;
            if (SpellGroupStackRuleByGroup.ContainsKey(groupStackingID) == false)
                SpellGroupStackRuleByGroup[groupStackingID] = spellTemplate.SpellGroupStackingRule;
            spellTemplate.SpellGroupStackingID = groupStackingID;
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

            // Capitalize Norrath
            descriptionSB.Replace("norrath", "Norrath");

            return descriptionSB.ToString();
        }

        private static string GenerateAuraDescription(SpellTemplate spellTemplate)
        {
            StringBuilder auraSB = new StringBuilder();
            bool descriptionTextHasBeenAddedToAura = false;
            for (int i = 0; i < spellTemplate.WOWSpellEffects.Count; i++)
            {
                SpellEffectWOW spellEffectWOW = spellTemplate.WOWSpellEffects[i];
                if (spellEffectWOW.AuraDescription.Length > 0)
                {
                    if (descriptionTextHasBeenAddedToAura == true)
                        auraSB.Append(", ");
                    auraSB.Append(spellTemplate.WOWSpellEffects[i].AuraDescription);
                    descriptionTextHasBeenAddedToAura = true;
                }
            }
            if (auraSB.Length == 0)
                return string.Empty;

            // Store and control capitalization
            auraSB.Append('.');
            auraSB[0] = char.ToUpper(auraSB[0]);

            // Bard song auras need target information
            if (spellTemplate.IsBardSongAura && spellTemplate.TargetDescriptionTextFragment.Length > 0)
            {
                auraSB.Append(" ");
                auraSB.Append(spellTemplate.TargetDescriptionTextFragment);
                auraSB.Append(".");
            }
            
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
                        baseEffectBlock.SpellName = string.Concat(Name, " Split ", _GroupedBaseSpellEffectBlocksForOutput.Count.ToString());
                        baseEffectBlock.WOWSpellID = GenerateUniqueWOWSpellID();
                    }
                    _GroupedBaseSpellEffectBlocksForOutput.Add(baseEffectBlock);

                    // Worn versions also get their own copy
                    if (WOWSpellIDWorn != 0)
                    {
                        SpellEffectBlock wornEffectBlock = new SpellEffectBlock();
                        if (_GroupedWornSpellEffectBlocksForOutput.Count == 0)
                            wornEffectBlock.WOWSpellID = WOWSpellIDWorn;
                        else
                            wornEffectBlock.WOWSpellID = GenerateUniqueWOWSpellID();
                        foreach (SpellEffectWOW spellEffect in baseEffectBlock.SpellEffects)
                            wornEffectBlock.SpellEffects.Add(spellEffect);
                        wornEffectBlock.SpellName = string.Concat(baseEffectBlock.SpellName, " (from gear)");
                        _GroupedWornSpellEffectBlocksForOutput.Add(wornEffectBlock);
                    }
                }
            }
        }
    
        private static int GenerateUniqueWOWSpellID()
        {
            int returnID = CUR_GENERATED_WOW_SPELL_ID;
            CUR_GENERATED_WOW_SPELL_ID++;
            if (CUR_GENERATED_WOW_SPELL_ID >= Configuration.DBCID_SPELL_ID_END)
            {
                Logger.WriteError("Spell DBCID max exceeded");
                throw new Exception("Spell DBCID max exceeded");
            }
            return returnID;
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
