//  Author: Nathan Handley(nathanhandley@protonmail.com)
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

using EQWOWConverter.Common;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Spells
{
    internal static class SpellClassAuras
    {
        // Proc flags (AzerothCore SpellMgr.h)
        private const int PROC_FLAG_DONE_MELEE_AUTO_ATTACK = 0x00000004;
        private const int PROC_FLAG_TAKEN_MELEE_AUTO_ATTACK = 0x00000008;
        private const int PROC_FLAG_TAKEN_SPELL_MELEE_DMG_CLASS = 0x00000020;
        private const int PROC_FLAG_TAKEN_RANGED_AUTO_ATTACK = 0x00000080;
        private const int PROC_FLAG_TAKEN_SPELL_RANGED_DMG_CLASS = 0x00000200;
        private const int PROC_FLAG_DONE_RANGED_AUTO_ATTACK = 0x00000040;
        private const int PROC_FLAG_DONE_SPELL_RANGED_DMG_CLASS = 0x00000100;
        private const int PROC_FLAG_DONE_SPELL_MELEE_DMG_CLASS = 0x00000010;
        private const int PROC_FLAG_TAKEN_SPELL_NONE_DMG_CLASS_NEG = 0x00002000;
        private const int PROC_FLAG_TAKEN_SPELL_MAGIC_DMG_CLASS_NEG = 0x00020000;
        private const int PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_POS = 0x00000400;
        private const int PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_NEG = 0x00001000;
        private const int PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_POS = 0x00004000;
        private const int PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_NEG = 0x00010000;
        private const int PROC_SPELL_TYPE_DAMAGE = 0x1;
        private const int PROC_SPELL_TYPE_HEAL = 0x2;
        private const int PROC_SPELL_TYPE_NO_DMG_HEAL = 0x4;
        private const int PROC_SPELL_PHASE_HIT = 0x2;
        private const int PROC_HIT_NORMAL = 0x1;
        private const int PROC_HIT_CRITICAL = 0x2;
        private const int PROC_HIT_MISS = 0x4;
        private const int PROC_HIT_DODGE = 0x10;
        private const int PROC_HIT_PARRY = 0x20;
        private const int PROC_HIT_BLOCK = 0x40;
        private const int PROC_HIT_ABSORB = 0x400;
        private const int PROC_HIT_FULL_BLOCK = 0x2000;

        // Spell school masks
        private const int SCHOOL_MASK_HOLY = 2;
        private const int SCHOOL_MASK_MAGIC = 126; // Every non-physical school
        private const int SCHOOL_MASK_ALL = 127;

        // Mechanics (AzerothCore SharedDefines.h)

        // ModTotalStatPercentage misc value that covers every stat (what Blessing of Kings uses)
        private const int STAT_ALL = -1;

        // Powers
        private const int POWER_MANA = 0;

        // The cast speed helper only lives across one cast, but it is given a real duration as a safety net
        private const int CAST_SPEED_HELPER_DURATION_IN_MS = 60000;

        public static int GetSpellID(SpellClassAuraType spellType)
        {
            return Configuration.CLASSAURA_SPELL_ID_START + (int)spellType;
        }

        public static bool IsClassEnabled(ClassEQType eqClass)
        {
            if (Configuration.CLASSAURA_ENABLED == false)
                return false;
            switch (eqClass)
            {
                case ClassEQType.Enchanter: return Configuration.CLASSAURA_ENCHANTER_ENABLED;
                case ClassEQType.Bard: return Configuration.CLASSAURA_BARD_ENABLED;
                case ClassEQType.Monk: return Configuration.CLASSAURA_MONK_ENABLED;
                case ClassEQType.Ranger: return Configuration.CLASSAURA_RANGER_ENABLED;
                case ClassEQType.Rogue: return Configuration.CLASSAURA_ROGUE_ENABLED;
                case ClassEQType.Paladin: return Configuration.CLASSAURA_PALADIN_ENABLED;
                case ClassEQType.ShadowKnight: return Configuration.CLASSAURA_SHADOWKNIGHT_ENABLED;
                case ClassEQType.Warrior: return Configuration.CLASSAURA_WARRIOR_ENABLED;
                case ClassEQType.Wizard: return Configuration.CLASSAURA_WIZARD_ENABLED;
                case ClassEQType.Magician: return Configuration.CLASSAURA_MAGICIAN_ENABLED;
                case ClassEQType.Necromancer: return Configuration.CLASSAURA_NECROMANCER_ENABLED;
                case ClassEQType.Cleric: return Configuration.CLASSAURA_CLERIC_ENABLED;
                case ClassEQType.Druid: return Configuration.CLASSAURA_DRUID_ENABLED;
                case ClassEQType.Shaman: return Configuration.CLASSAURA_SHAMAN_ENABLED;
                default: return false;
            }
        }

        public static int GetPassiveSpellIDForClass(ClassEQType eqClass)
        {
            if (IsClassEnabled(eqClass) == false)
                return 0;
            switch (eqClass)
            {
                case ClassEQType.Enchanter: return GetSpellID(SpellClassAuraType.EnchanterPassive);
                case ClassEQType.Bard: return GetSpellID(SpellClassAuraType.BardPassive);
                case ClassEQType.Monk: return GetSpellID(SpellClassAuraType.MonkPassive);
                case ClassEQType.Ranger: return GetSpellID(SpellClassAuraType.RangerPassive);
                case ClassEQType.Rogue: return GetSpellID(SpellClassAuraType.RoguePassive);
                case ClassEQType.Paladin: return GetSpellID(SpellClassAuraType.PaladinPassive);
                case ClassEQType.ShadowKnight: return GetSpellID(SpellClassAuraType.ShadowKnightPassive);
                case ClassEQType.Warrior: return GetSpellID(SpellClassAuraType.WarriorPassive);
                case ClassEQType.Wizard: return GetSpellID(SpellClassAuraType.WizardPassive);
                case ClassEQType.Magician: return GetSpellID(SpellClassAuraType.MagicianPassive);
                case ClassEQType.Necromancer: return GetSpellID(SpellClassAuraType.NecromancerPassive);
                case ClassEQType.Cleric: return GetSpellID(SpellClassAuraType.ClericPassive);
                case ClassEQType.Druid: return GetSpellID(SpellClassAuraType.DruidPassive);
                case ClassEQType.Shaman: return GetSpellID(SpellClassAuraType.ShamanPassive);
                default: return 0;
            }
        }

        public static int GetToggleSpellIDForClass(ClassEQType eqClass)
        {
            if (IsClassEnabled(eqClass) == false)
                return 0;
            switch (eqClass)
            {
                case ClassEQType.Ranger: return GetSpellID(SpellClassAuraType.RangerEndlessQuiver);
                case ClassEQType.Shaman: return GetSpellID(SpellClassAuraType.ShamanWarspirit);
                default: return 0;
            }
        }

        public static List<KeyValuePair<string, string>> GetSystemConfigRows()
        {
            List<KeyValuePair<string, string>> rows = new List<KeyValuePair<string, string>>();
            rows.Add(new KeyValuePair<string, string>("ClassAuraEnabled", Configuration.CLASSAURA_ENABLED == true ? "1" : "0"));
            for (int i = 0; i < (int)SpellClassAuraType.Count; i++)
            {
                SpellClassAuraType spellType = (SpellClassAuraType)i;
                int spellID = IsSpellTypeEnabled(spellType) == true ? GetSpellID(spellType) : 0;
                rows.Add(new KeyValuePair<string, string>(string.Concat("ClassAuraSpellID", spellType.ToString()), spellID.ToString()));
            }
            rows.Add(new KeyValuePair<string, string>("ClassAuraPrivateSpellFamilyID", Configuration.SPELL_EQ_PRIVATE_SPELL_FAMILY_ID.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraEnchanterFocusManaThresholdPercent", Configuration.CLASSAURA_ENCHANTER_FOCUS_MANA_THRESHOLD_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraBardInstrumentMeleeAutoAttackDamagePercent", Configuration.CLASSAURA_BARD_INSTRUMENT_MELEE_AUTOATTACK_DAMAGE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraMonkChiSurgeCastTimeReductionPercent", Configuration.CLASSAURA_MONK_CHI_SURGE_CAST_TIME_REDUCTION_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraMonkChiSurgeMaxBaseCastTimeInMS", Configuration.CLASSAURA_MONK_CHI_SURGE_MAX_BASE_CAST_TIME_IN_MS.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraMonkChiSurgeReturnInMS", Configuration.CLASSAURA_MONK_CHI_SURGE_RETURN_IN_MS.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraRogueLuckyStrikeCritPercent", Configuration.CLASSAURA_ROGUE_LUCKY_STRIKE_CRIT_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraRogueLuckyStrikeCooldownInMS", Configuration.CLASSAURA_ROGUE_LUCKY_STRIKE_COOLDOWN_IN_MS.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraMonkDoubleToTripleAttackChancePercent", Configuration.CLASSAURA_MONK_DOUBLE_TO_TRIPLE_ATTACK_CHANCE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraRangerTackShotDamagePercentPerStack", Configuration.CLASSAURA_RANGER_TACK_SHOT_DAMAGE_PERCENT_PER_STACK.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraRangerEndlessQuiverBaseManaCostPercent", Configuration.CLASSAURA_RANGER_ENDLESS_QUIVER_BASE_MANA_COST_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraPaladinHealSelfPercent", Configuration.CLASSAURA_PALADIN_HEAL_SELF_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraPaladinUndeadDemonDoubleDamageChancePercent", Configuration.CLASSAURA_PALADIN_UNDEAD_DEMON_DOUBLE_DAMAGE_CHANCE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraWarriorRiposteChancePercent", Configuration.CLASSAURA_WARRIOR_RIPOSTE_CHANCE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraWarriorUnrelentingAssaultStackIntervalInMS", Configuration.CLASSAURA_WARRIOR_UNRELENTING_ASSAULT_STACK_INTERVAL_IN_MS.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraWizardFocusStacksLostPerMovementEvent", Configuration.CLASSAURA_WIZARD_FOCUS_STACKS_LOST_PER_MOVEMENT_EVENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraWizardFocusMovementIntervalInMS", Configuration.CLASSAURA_WIZARD_FOCUS_MOVEMENT_INTERVAL_IN_MS.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraWizardFocusStillIntervalInMS", Configuration.CLASSAURA_WIZARD_FOCUS_STILL_INTERVAL_IN_MS.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraNecromancerDebuffTransferCooldownInMS", Configuration.CLASSAURA_NECROMANCER_DEBUFF_TRANSFER_COOLDOWN_IN_MS.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraNecromancerMarkDirectDamagePercentPerStack", Configuration.CLASSAURA_NECROMANCER_MARK_DIRECT_DAMAGE_PERCENT_PER_STACK.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraNecromancerMarkDotDamagePercentPerStack", Configuration.CLASSAURA_NECROMANCER_MARK_DOT_DAMAGE_PERCENT_PER_STACK.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraClericCadenceReductionPercent", Configuration.CLASSAURA_CLERIC_CADENCE_REDUCTION_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraPaladinBlockDeflectionDamagePercent", Configuration.CLASSAURA_PALADIN_BLOCK_DEFLECTION_DAMAGE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraDruidDirectHealRegenPercent", Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraDruidDirectHealRegenTickCount", GetDruidRegenTickCount().ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraDruidNaturesBalanceDamagePercentPerStack", Configuration.CLASSAURA_DRUID_NATURES_BALANCE_DAMAGE_PERCENT_PER_STACK.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraDruidNaturesBalanceMinBaseCastTimeInMS", Configuration.CLASSAURA_DRUID_NATURES_BALANCE_MIN_BASE_CAST_TIME_IN_MS.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraDruidEntangleStrikeDamageTakenPercentPerStack", Configuration.CLASSAURA_DRUID_ENTANGLE_STRIKE_DAMAGE_TAKEN_PERCENT_PER_STACK.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraDruidEntangleStrikeBehindDamagePercentPerStack", Configuration.CLASSAURA_DRUID_ENTANGLE_STRIKE_BEHIND_DAMAGE_PERCENT_PER_STACK.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraShamanDotExtendChancePercent", Configuration.CLASSAURA_SHAMAN_DOT_EXTEND_CHANCE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraShamanDotExtendInMS", Configuration.CLASSAURA_SHAMAN_DOT_EXTEND_IN_MS.ToString()));
            return rows;
        }

        private static bool IsSpellTypeEnabled(SpellClassAuraType spellType)
        {
            if (Configuration.CLASSAURA_ENABLED == false)
                return false;
            switch (spellType)
            {
                case SpellClassAuraType.EnchanterPassive:
                case SpellClassAuraType.EnchanterAura:
                case SpellClassAuraType.EnchanterFocus:
                    return Configuration.CLASSAURA_ENCHANTER_ENABLED;
                case SpellClassAuraType.BardPassive:
                case SpellClassAuraType.BardAura:
                case SpellClassAuraType.BardInstrument:
                case SpellClassAuraType.BardVigor:
                    return Configuration.CLASSAURA_BARD_ENABLED;
                case SpellClassAuraType.MonkPassive:
                case SpellClassAuraType.MonkAura:
                case SpellClassAuraType.MonkLightArmor:
                case SpellClassAuraType.MonkHeavyArmor:
                case SpellClassAuraType.MonkChiSurge:
                    return Configuration.CLASSAURA_MONK_ENABLED;
                case SpellClassAuraType.RangerPassive:
                case SpellClassAuraType.RangerAura:
                case SpellClassAuraType.RangerEndlessQuiver:
                case SpellClassAuraType.RangerTackShot:
                    return Configuration.CLASSAURA_RANGER_ENABLED;
                case SpellClassAuraType.RoguePassive:
                case SpellClassAuraType.RogueAura:
                case SpellClassAuraType.RogueExploit:
                case SpellClassAuraType.RogueLuckyStrike:
                case SpellClassAuraType.RogueLuckyStrikeHelper:
                    return Configuration.CLASSAURA_ROGUE_ENABLED;
                case SpellClassAuraType.PaladinPassive:
                case SpellClassAuraType.PaladinAura:
                case SpellClassAuraType.PaladinHeal:
                case SpellClassAuraType.PaladinDeflection:
                    return Configuration.CLASSAURA_PALADIN_ENABLED;
                case SpellClassAuraType.ShadowKnightPassive:
                case SpellClassAuraType.ShadowKnightAura:
                case SpellClassAuraType.ShadowKnightEdge:
                    return Configuration.CLASSAURA_SHADOWKNIGHT_ENABLED;
                case SpellClassAuraType.WarriorPassive:
                case SpellClassAuraType.WarriorAura:
                case SpellClassAuraType.WarriorUnrelentingAssault:
                case SpellClassAuraType.WarriorRiposte:
                    return Configuration.CLASSAURA_WARRIOR_ENABLED;
                case SpellClassAuraType.WizardPassive:
                case SpellClassAuraType.WizardAura:
                case SpellClassAuraType.WizardFocus:
                    return Configuration.CLASSAURA_WIZARD_ENABLED;
                case SpellClassAuraType.MagicianPassive:
                case SpellClassAuraType.MagicianAura:
                case SpellClassAuraType.MagicianPetPassive:
                case SpellClassAuraType.MagicianOwnerFocus:
                case SpellClassAuraType.MagicianPetFury:
                    return Configuration.CLASSAURA_MAGICIAN_ENABLED;
                case SpellClassAuraType.NecromancerPassive:
                case SpellClassAuraType.NecromancerAura:
                case SpellClassAuraType.NecromancerMark:
                    return Configuration.CLASSAURA_NECROMANCER_ENABLED;
                case SpellClassAuraType.ClericPassive:
                case SpellClassAuraType.ClericAura:
                case SpellClassAuraType.ClericCadence:
                case SpellClassAuraType.ClericHaste:
                    return Configuration.CLASSAURA_CLERIC_ENABLED;
                case SpellClassAuraType.DruidPassive:
                case SpellClassAuraType.DruidAura:
                case SpellClassAuraType.DruidRegrowth:
                case SpellClassAuraType.DruidNaturesBalanceFire:
                case SpellClassAuraType.DruidNaturesBalanceCold:
                case SpellClassAuraType.DruidNaturesBalanceNature:
                case SpellClassAuraType.DruidEntangleStrike:
                    return Configuration.CLASSAURA_DRUID_ENABLED;
                case SpellClassAuraType.ShamanPassive:
                case SpellClassAuraType.ShamanAura:
                case SpellClassAuraType.ShamanWarspirit:
                case SpellClassAuraType.ShamanVigor:
                case SpellClassAuraType.ShamanWarspiritVigor:
                    return Configuration.CLASSAURA_SHAMAN_ENABLED;
                case SpellClassAuraType.CastSpeedHelper:
                    return Configuration.CLASSAURA_MONK_ENABLED || Configuration.CLASSAURA_CLERIC_ENABLED;
                default:
                    return false;
            }
        }

        private static int GetDruidRegenTickCount()
        {
            if (Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_TICK_INTERVAL_IN_MS <= 0)
                return 1;
            return Math.Max(1, Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_DURATION_IN_MS / Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_TICK_INTERVAL_IN_MS);
        }

        private static int GetValidatedSpellIconID(int spellIconEQID, string spellName)
        {
            if (spellIconEQID < 0 || spellIconEQID > 22)
            {
                Logger.WriteError(string.Concat("Invalid class aura spell icon id for '", spellName, "', value must be 0-22. Setting to 0"));
                return 0;
            }
            return spellIconEQID;
        }

        private static int GetValidatedSpellItemIconID(int spellItemIconEQID, string spellName)
        {
            if (spellItemIconEQID < 0 || spellItemIconEQID > 750)
            {
                Logger.WriteError(string.Concat("Invalid class aura spell item icon id for '", spellName, "', value must be 0-750. Setting to 0"));
                return 0;
            }
            return spellItemIconEQID;
        }

        private static string Pct(int value)
        {
            return string.Concat(value.ToString(), "%");
        }

        private static string Seconds(int durationInMS)
        {
            return string.Concat((durationInMS / 1000).ToString(), " seconds");
        }

        private static string SecondsWithFraction(int durationInMS)
        {
            return string.Concat(((float)durationInMS / 1000.0f).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture), " seconds");
        }

        private static SpellTemplate BuildBaseTemplate(string name, SpellClassAuraType spellType, int spellIconEQID, string description, string auraDescription, bool useSpellItemIcon = false)
        {
            SpellTemplate spellTemplate = new SpellTemplate();
            spellTemplate.Name = name;
            spellTemplate.WOWSpellID = GetSpellID(spellType);
            spellTemplate.EQSpellID = SpellTemplate.GenerateUniqueEQSpellID();
            spellTemplate.Description = description;
            spellTemplate.AuraDescription = auraDescription;
            if (useSpellItemIcon == false)
                spellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(GetValidatedSpellIconID(spellIconEQID, name));
            else
                spellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForItemIconID(GetValidatedSpellItemIconID(spellIconEQID, name));
            //spellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellItemIconID(GetValidatedSpellItemIconID(spellIconEQID, name));
            spellTemplate.CastTimeInMS = 0;
            spellTemplate.RecoveryTimeInMS = 0;
            spellTemplate.EQSkillCategory = SpellEQSkillCategory.Combat;
            spellTemplate.SkillLine = 0; // The mod applies these, so they never show in the spellbook
            spellTemplate.TriggersGlobalCooldown = false;
            spellTemplate.CannotBeStolen = true;
            spellTemplate.AuraDuration = new SpellDuration();
            return spellTemplate;
        }

        private static SpellEffectWOW BuildAuraEffect(SpellWOWAuraType auraType, int amount, int miscValueA, SpellWOWTargetType target, uint auraPeriodInMS = 0)
        {
            SpellEffectWOW effect = new SpellEffectWOW(SpellWOWEffectType.ApplyAura, auraType, auraPeriodInMS, 0, 0, amount, miscValueA, 0);
            effect.ImplicitTargetA = target;
            return effect;
        }

        private static SpellTemplate BuildPassiveTemplate(string name, SpellClassAuraType spellType, int spellIconEQID, string description)
        {
            SpellTemplate spellTemplate = BuildBaseTemplate(name, spellType, spellIconEQID, description, description);
            spellTemplate.ForceHiddenFromDisplay = true;
            spellTemplate.AuraDuration.IsInfinite = true;
            spellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            spellTemplate.IsPassiveAbility = true;
            spellTemplate.AlwaysPersist = true;
            return spellTemplate;
        }

        private static SpellTemplate BuildPermanentAuraTemplate(string name, SpellClassAuraType spellType, int spellIconEQID, string description, List<SpellEffectWOW> effects)
        {
            SpellTemplate spellTemplate = BuildBaseTemplate(name, spellType, spellIconEQID, description, description);
            spellTemplate.AuraDuration.IsInfinite = true;
            if (effects.Count == 0)
                spellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            else
                spellTemplate.WOWSpellEffects.AddRange(effects);
            spellTemplate.AlwaysPersist = true;
            return spellTemplate;
        }

        private static SpellTemplate BuildStackingAuraTemplate(string name, SpellClassAuraType spellType, int spellIconEQID, string description, List<SpellEffectWOW> effects,
            int maxStacks, int durationInMS, bool isDebuff)
        {
            SpellTemplate spellTemplate = BuildBaseTemplate(name, spellType, spellIconEQID, description, description);
            spellTemplate.AuraDuration.SetFixedDuration(durationInMS);
            spellTemplate.MaxStackAmount = Convert.ToUInt32(Math.Max(1, maxStacks));
            spellTemplate.WOWSpellEffects.AddRange(effects);
            spellTemplate.PreventAuraClickOff = true;
            spellTemplate.ForceAsDebuff = isDebuff;
            return spellTemplate;
        }

        public static void AddSpellTemplates(List<SpellTemplate> spellTemplates)
        {
            if (Configuration.CLASSAURA_ENABLED == false)
                return;
            if (Configuration.CLASSAURA_ENCHANTER_ENABLED == true)
                AddEnchanterSpells(spellTemplates);
            if (Configuration.CLASSAURA_BARD_ENABLED == true)
                AddBardSpells(spellTemplates);
            if (Configuration.CLASSAURA_MONK_ENABLED == true)
                AddMonkSpells(spellTemplates);
            if (Configuration.CLASSAURA_RANGER_ENABLED == true)
                AddRangerSpells(spellTemplates);
            if (Configuration.CLASSAURA_ROGUE_ENABLED == true)
                AddRogueSpells(spellTemplates);
            if (Configuration.CLASSAURA_PALADIN_ENABLED == true)
                AddPaladinSpells(spellTemplates);
            if (Configuration.CLASSAURA_SHADOWKNIGHT_ENABLED == true)
                AddShadowKnightSpells(spellTemplates);
            if (Configuration.CLASSAURA_WARRIOR_ENABLED == true)
                AddWarriorSpells(spellTemplates);
            if (Configuration.CLASSAURA_WIZARD_ENABLED == true)
                AddWizardSpells(spellTemplates);
            if (Configuration.CLASSAURA_MAGICIAN_ENABLED == true)
                AddMagicianSpells(spellTemplates);
            if (Configuration.CLASSAURA_NECROMANCER_ENABLED == true)
                AddNecromancerSpells(spellTemplates);
            if (Configuration.CLASSAURA_CLERIC_ENABLED == true)
                AddClericSpells(spellTemplates);
            if (Configuration.CLASSAURA_DRUID_ENABLED == true)
                AddDruidSpells(spellTemplates);
            if (Configuration.CLASSAURA_SHAMAN_ENABLED == true)
                AddShamanSpells(spellTemplates);
            if (Configuration.CLASSAURA_MONK_ENABLED == true || Configuration.CLASSAURA_CLERIC_ENABLED == true)
                AddCastSpeedHelperSpell(spellTemplates);
        }

        private static void AddEnchanterSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_ENCHANTER_SPELL_ICON_EQ_ID;
            string description = string.Concat("Regenerates ", Pct(Configuration.CLASSAURA_ENCHANTER_MANA_REGEN_PERCENT), " of maximum mana every ",
                Seconds(Configuration.CLASSAURA_ENCHANTER_MANA_REGEN_INTERVAL_IN_MS), ". Spell damage and healing are increased by ", Pct(Configuration.CLASSAURA_ENCHANTER_FOCUS_SPELL_DAMAGE_AND_HEALING_PERCENT),
                " while mana is at ", Pct(Configuration.CLASSAURA_ENCHANTER_FOCUS_MANA_THRESHOLD_PERCENT), " or more.");
            spellTemplates.Add(BuildPassiveTemplate("Mind of Clarity", SpellClassAuraType.EnchanterPassive, icon, description));

            List<SpellEffectWOW> auraEffects = new List<SpellEffectWOW>();
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.ObsModPower, Configuration.CLASSAURA_ENCHANTER_MANA_REGEN_PERCENT, POWER_MANA, SpellWOWTargetType.UnitCaster,
                Convert.ToUInt32(Math.Max(1000, Configuration.CLASSAURA_ENCHANTER_MANA_REGEN_INTERVAL_IN_MS))));
            spellTemplates.Add(BuildPermanentAuraTemplate("Mind of Clarity (Enchanter)", SpellClassAuraType.EnchanterAura, icon, description, auraEffects));

            string focusDescription = string.Concat("Spell damage and healing increased by ", Pct(Configuration.CLASSAURA_ENCHANTER_FOCUS_SPELL_DAMAGE_AND_HEALING_PERCENT), " while mana stays at ",
                Pct(Configuration.CLASSAURA_ENCHANTER_FOCUS_MANA_THRESHOLD_PERCENT), " or more.");
            List<SpellEffectWOW> focusEffects = new List<SpellEffectWOW>();
            focusEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentDone, Configuration.CLASSAURA_ENCHANTER_FOCUS_SPELL_DAMAGE_AND_HEALING_PERCENT, SCHOOL_MASK_MAGIC, SpellWOWTargetType.UnitCaster));
            focusEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModHealingDonePercent, Configuration.CLASSAURA_ENCHANTER_FOCUS_SPELL_DAMAGE_AND_HEALING_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildPermanentAuraTemplate("Clarity of Thought", SpellClassAuraType.EnchanterFocus, icon, focusDescription, focusEffects));
        }

        private static void AddBardSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_BARD_SPELL_ICON_EQ_ID;
            string description = string.Concat("Melee autoattacks deal ", Pct(Configuration.CLASSAURA_BARD_INSTRUMENT_MELEE_AUTOATTACK_DAMAGE_PERCENT),
                " more damage while holding a weapon in one hand and an instrument in the other. Successfully playing a song grants you ",
                Pct(Configuration.CLASSAURA_BARD_VIGOR_HASTE_PERCENT), " haste to melee, ranged, and spells for ", Seconds(Configuration.CLASSAURA_BARD_VIGOR_DURATION_IN_MS),
                ". This haste does not count against the haste cap. Song cast times cannot be changed by haste or slow effects.");
            spellTemplates.Add(BuildPassiveTemplate("Dexteritous Troubadour", SpellClassAuraType.BardPassive, icon, description));

            // Both effects are driven by the mod (the instrument marker below and the vigor cast on a new song), so the aura itself carries nothing
            spellTemplates.Add(BuildPermanentAuraTemplate("Dexteritous Troubadour (Bard)", SpellClassAuraType.BardAura, icon, description, new List<SpellEffectWOW>()));

            // Only a marker: the mod applies the bonus in its melee swing hook since a damage percent aura would also raise melee abilities and ranged shots
            string instrumentDescription = string.Concat("Melee autoattack damage increased by ", Pct(Configuration.CLASSAURA_BARD_INSTRUMENT_MELEE_AUTOATTACK_DAMAGE_PERCENT), " while holding a weapon and an instrument.");
            List<SpellEffectWOW> instrumentEffects = new List<SpellEffectWOW>();
            instrumentEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildPermanentAuraTemplate("Troubadour's Tempo", SpellClassAuraType.BardInstrument, icon, instrumentDescription, instrumentEffects));

            string vigorDescription = string.Concat("Haste increased by ", Pct(Configuration.CLASSAURA_BARD_VIGOR_HASTE_PERCENT), " for melee, ranged, and spells.");
            List<SpellEffectWOW> vigorEffects = new List<SpellEffectWOW>();
            vigorEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModMeleeRangedHaste, Configuration.CLASSAURA_BARD_VIGOR_HASTE_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            vigorEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModCastingSpeedNotStack, Configuration.CLASSAURA_BARD_VIGOR_HASTE_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildStackingAuraTemplate("Virtuoso Vigor", SpellClassAuraType.BardVigor, icon, vigorDescription, vigorEffects, 1,
                Configuration.CLASSAURA_BARD_VIGOR_DURATION_IN_MS, false));
        }

        private static void AddMonkSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_MONK_SPELL_ICON_EQ_ID;
            string description = string.Concat("Chi Surge: any non-instant, non-channeled spell with a base cast time under ", Seconds(Configuration.CLASSAURA_MONK_CHI_SURGE_MAX_BASE_CAST_TIME_IN_MS),
                " casts ", Pct(Configuration.CLASSAURA_MONK_CHI_SURGE_CAST_TIME_REDUCTION_PERCENT), " faster, and it returns ", Seconds(Configuration.CLASSAURA_MONK_CHI_SURGE_RETURN_IN_MS),
                " after use. In cloth or leather: attacks have a ", Pct(Configuration.CLASSAURA_MONK_DOUBLE_ATTACK_CHANCE_PERCENT),
                " chance to strike twice, ", Pct(Configuration.CLASSAURA_MONK_DOUBLE_TO_TRIPLE_ATTACK_CHANCE_PERCENT), " of those strike a third time, and dodge is increased by ",
                Pct(Configuration.CLASSAURA_MONK_LIGHT_ARMOR_DODGE_PERCENT), ". In heavier armor: attacks have a ", Pct(Configuration.CLASSAURA_MONK_DOUBLE_ATTACK_CHANCE_PERCENT),
                " chance to strike twice.");
            spellTemplates.Add(BuildPassiveTemplate("Agile Fighter", SpellClassAuraType.MonkPassive, icon, description));
            spellTemplates.Add(BuildPermanentAuraTemplate("Agile Fighter (Monk)", SpellClassAuraType.MonkAura, icon, description, new List<SpellEffectWOW>()));

            string chiSurgeDescription = string.Concat("Your next non-instant, non-channeled spell with a base cast time under ", Seconds(Configuration.CLASSAURA_MONK_CHI_SURGE_MAX_BASE_CAST_TIME_IN_MS),
                " casts ", Pct(Configuration.CLASSAURA_MONK_CHI_SURGE_CAST_TIME_REDUCTION_PERCENT), " faster. Adds together with a Sacred Focus charge spent on the same cast.");
            spellTemplates.Add(BuildPermanentAuraTemplate("Chi Surge", SpellClassAuraType.MonkChiSurge, Configuration.CLASSAURA_MONK_CHI_SURGE_SPELL_ICON_EQ_ID, chiSurgeDescription, new List<SpellEffectWOW>()));

            // Monk multi-strike
            string lightDescription = string.Concat("Attacks have a ", Pct(Configuration.CLASSAURA_MONK_DOUBLE_ATTACK_CHANCE_PERCENT), " chance to strike twice, ",
                Pct(Configuration.CLASSAURA_MONK_DOUBLE_TO_TRIPLE_ATTACK_CHANCE_PERCENT), " of those strike a third time, and dodge is increased by ",
                Pct(Configuration.CLASSAURA_MONK_LIGHT_ARMOR_DODGE_PERCENT), " while wearing cloth or leather.");
            List<SpellEffectWOW> lightEffects = new List<SpellEffectWOW>();
            lightEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDodgePercent, Configuration.CLASSAURA_MONK_LIGHT_ARMOR_DODGE_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            SpellTemplate lightSpellTemplate = BuildPermanentAuraTemplate("Unburdened Agility", SpellClassAuraType.MonkLightArmor, icon, lightDescription, lightEffects);
            lightSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraMonkLightArmorAuraScript";
            lightSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_MELEE_AUTO_ATTACK, 0, 0, 0, 0, Configuration.CLASSAURA_MONK_DOUBLE_ATTACK_CHANCE_PERCENT);
            spellTemplates.Add(lightSpellTemplate);

            string heavyDescription = string.Concat("Attacks have a ", Pct(Configuration.CLASSAURA_MONK_DOUBLE_ATTACK_CHANCE_PERCENT), " chance to strike twice while wearing mail or plate.");
            SpellTemplate heavySpellTemplate = BuildPermanentAuraTemplate("Burdened Agility", SpellClassAuraType.MonkHeavyArmor, icon, heavyDescription, new List<SpellEffectWOW>());
            heavySpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraMonkHeavyArmorAuraScript";
            heavySpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_MELEE_AUTO_ATTACK, 0, 0, 0, 0, Configuration.CLASSAURA_MONK_DOUBLE_ATTACK_CHANCE_PERCENT);
            spellTemplates.Add(heavySpellTemplate);
        }

        private static void AddRangerSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_RANGER_SPELL_ICON_EQ_ID;
            string description = string.Concat("Endless Quiver can be toggled on so your ranged attacks and abilities stop using up arrows and bullets, costing ",
                Pct(Configuration.CLASSAURA_RANGER_ENDLESS_QUIVER_BASE_MANA_COST_PERCENT), " of base mana per shot instead. Each ranged attack, ranged ability, and offensive spell tacks its target for ",
                Seconds(Configuration.CLASSAURA_RANGER_TACK_SHOT_DURATION_IN_MS),
                ", raising the damage it takes from you and your pet by ", Pct(Configuration.CLASSAURA_RANGER_TACK_SHOT_DAMAGE_PERCENT_PER_STACK), " per stack, up to ",
                Configuration.CLASSAURA_RANGER_TACK_SHOT_MAX_STACKS.ToString(), " stacks. The bonus is doubled while the target moves.");
            spellTemplates.Add(BuildPassiveTemplate("Swift Reactions", SpellClassAuraType.RangerPassive, icon, description));

            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Swift Reactions (Ranger)", SpellClassAuraType.RangerAura, icon, description, new List<SpellEffectWOW>());
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraRangerAuraScript";

            // Ranged autoattacks (bow, gun, thrown), ranged abilities and harmful spells tack the target.  No spell type filter, since the flags already exclude heals
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_RANGED_AUTO_ATTACK | PROC_FLAG_DONE_SPELL_RANGED_DMG_CLASS
                | PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_NEG | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_NEG, 0, PROC_SPELL_PHASE_HIT, PROC_HIT_NORMAL | PROC_HIT_CRITICAL, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            // Endless Quiver (ammo saving but costs mana)
            string quiverManaCostText = string.Concat(Pct(Configuration.CLASSAURA_RANGER_ENDLESS_QUIVER_BASE_MANA_COST_PERCENT), " of base mana");
            string quiverDescription = string.Concat("Toggle. While active, your ranged attacks and abilities no longer use up arrows or bullets. Each shot costs ", quiverManaCostText,
                " instead, and keeps working even when you are out of mana.");
            string quiverAuraDescription = string.Concat("Arrows and bullets are not used up. Each shot costs ", quiverManaCostText, " instead.");
            SpellTemplate quiverSpellTemplate = BuildBaseTemplate("Endless Quiver", SpellClassAuraType.RangerEndlessQuiver, Configuration.CLASSAURA_RANGER_ENDLESS_QUIVER_SPELL_ITEM_ICON_EQ_ID,
                quiverDescription, quiverAuraDescription, true);
            quiverSpellTemplate.AuraDuration.IsInfinite = true;
            quiverSpellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.AbilityConsumeNoAmmo, 0, 0, SpellWOWTargetType.UnitCaster));
            quiverSpellTemplate.SkillLine = SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Combat); // Unlike the other class aura spells, this one is in the spellbook
            quiverSpellTemplate.IsToggleAura = true;
            quiverSpellTemplate.ShowOnShapeshiftBar = true;
            quiverSpellTemplate.PersistThroughDeath = true;
            spellTemplates.Add(quiverSpellTemplate);

            // Debuff on the target increasing damage by the hunter and pet, increasing if creature is in motion
            string tackShotDescription = string.Concat("Takes ", Pct(Configuration.CLASSAURA_RANGER_TACK_SHOT_DAMAGE_PERCENT_PER_STACK),
                " more damage per stack from the ranger who tacked it and that ranger's pet, doubled while moving.");
            List<SpellEffectWOW> tackShotEffects = new List<SpellEffectWOW>();
            tackShotEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitTargetEnemy));
            spellTemplates.Add(BuildStackingAuraTemplate("Tack Shot", SpellClassAuraType.RangerTackShot, icon, tackShotDescription, tackShotEffects,
                Configuration.CLASSAURA_RANGER_TACK_SHOT_MAX_STACKS, Configuration.CLASSAURA_RANGER_TACK_SHOT_DURATION_IN_MS, true));
        }

        private static void AddRogueSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_ROGUE_SPELL_ICON_EQ_ID;
            string description = string.Concat("Lucky Strike: your next ability or spell that would not have been a critical strike becomes one. Cannot occur more than once every ",
                Seconds(Configuration.CLASSAURA_ROGUE_LUCKY_STRIKE_COOLDOWN_IN_MS), ". Autoattacks and Auto Shot are not affected. Each landed attack raises all damage dealt by ",
                Pct(Configuration.CLASSAURA_ROGUE_EXPLOIT_DAMAGE_PERCENT_PER_STACK), " for ", Seconds(Configuration.CLASSAURA_ROGUE_EXPLOIT_DURATION_IN_MS), ", stacking up to ",
                Configuration.CLASSAURA_ROGUE_EXPLOIT_MAX_STACKS.ToString(), " times. A miss, dodge, or parry removes half of the stacks.");
            spellTemplates.Add(BuildPassiveTemplate("Master Exploiter", SpellClassAuraType.RoguePassive, icon, description));

            // Every attack feeds the momentum, and every critical (heals included, hence the positive flags) can spend the lucky strike; the script tells them apart
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Master Exploiter (Rogue)", SpellClassAuraType.RogueAura, icon, description, new List<SpellEffectWOW>());
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraRogueAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_MELEE_AUTO_ATTACK | PROC_FLAG_DONE_RANGED_AUTO_ATTACK | PROC_FLAG_DONE_SPELL_MELEE_DMG_CLASS
                | PROC_FLAG_DONE_SPELL_RANGED_DMG_CLASS | PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_NEG | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_NEG
                | PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_POS | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_POS, 0, PROC_SPELL_PHASE_HIT,
                PROC_HIT_NORMAL | PROC_HIT_CRITICAL | PROC_HIT_MISS | PROC_HIT_DODGE | PROC_HIT_PARRY | PROC_HIT_BLOCK | PROC_HIT_ABSORB, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            string luckyDescription = "Your next ability or spell that would not have been a critical strike becomes one.";
            List<SpellEffectWOW> luckyEffects = new List<SpellEffectWOW>();
            luckyEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildPermanentAuraTemplate("Lucky Strike", SpellClassAuraType.RogueLuckyStrike, Configuration.CLASSAURA_ROGUE_LUCKY_STRIKE_SPELL_ICON_EQ_ID, luckyDescription, luckyEffects));

            SpellTemplate luckyHelperSpellTemplate = BuildBaseTemplate("Lucky Strike Crit", SpellClassAuraType.RogueLuckyStrikeHelper, 0, "The lucky strike in progress.", "The lucky strike in progress.");
            luckyHelperSpellTemplate.AuraDuration.SetFixedDuration(CAST_SPEED_HELPER_DURATION_IN_MS);
            luckyHelperSpellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModSpellCritChanceSchool, Configuration.CLASSAURA_ROGUE_LUCKY_STRIKE_CRIT_PERCENT, SCHOOL_MASK_ALL, SpellWOWTargetType.UnitCaster));
            luckyHelperSpellTemplate.ForceHiddenFromDisplay = true;
            luckyHelperSpellTemplate.PreventAuraClickOff = true;
            spellTemplates.Add(luckyHelperSpellTemplate);

            string exploitDescription = string.Concat("Damage dealt increased by ", Pct(Configuration.CLASSAURA_ROGUE_EXPLOIT_DAMAGE_PERCENT_PER_STACK), " per stack. A miss, dodge, or parry removes half of the stacks.");
            List<SpellEffectWOW> exploitEffects = new List<SpellEffectWOW>();
            exploitEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentDone, Configuration.CLASSAURA_ROGUE_EXPLOIT_DAMAGE_PERCENT_PER_STACK, SCHOOL_MASK_ALL, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildStackingAuraTemplate("Exploitive Momentum", SpellClassAuraType.RogueExploit, icon, exploitDescription, exploitEffects,
                Configuration.CLASSAURA_ROGUE_EXPLOIT_MAX_STACKS, Configuration.CLASSAURA_ROGUE_EXPLOIT_DURATION_IN_MS, false));
        }

        private static void AddPaladinSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_PALADIN_SPELL_ICON_EQ_ID;
            string description = string.Concat("Blessed Deflection: grants the block skill, block chance is increased by ", Pct(Configuration.CLASSAURA_PALADIN_BLOCK_PERCENT), ", and ",
                Pct(Configuration.CLASSAURA_PALADIN_BLOCK_DEFLECTION_DAMAGE_PERCENT), " of the damage you block is dealt as Holy damage to all enemies within ",
                Configuration.CLASSAURA_PALADIN_BLOCK_DEFLECTION_RADIUS_IN_YARDS.ToString(), " yards. Your heals also heal you for ",
                Pct(Configuration.CLASSAURA_PALADIN_HEAL_SELF_PERCENT), " of the amount. Your attacks, abilities, and spells against undead and demons have a ",
                Pct(Configuration.CLASSAURA_PALADIN_UNDEAD_DEMON_DOUBLE_DAMAGE_CHANCE_PERCENT), " chance to deal double damage.");
            spellTemplates.Add(BuildPassiveTemplate("Champion of Light", SpellClassAuraType.PaladinPassive, icon, description));

            List<SpellEffectWOW> auraEffects = new List<SpellEffectWOW>();
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModBlockPercent, Configuration.CLASSAURA_PALADIN_BLOCK_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Champion of Light (Paladin)", SpellClassAuraType.PaladinAura, icon, description, auraEffects);
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraPaladinAuraScript";
            // Direct heals only (no PROC_FLAG_DONE_PERIODIC), so a heal over time tick never earns the reward
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_POS | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_POS
                | PROC_FLAG_TAKEN_MELEE_AUTO_ATTACK | PROC_FLAG_TAKEN_SPELL_MELEE_DMG_CLASS | PROC_FLAG_TAKEN_RANGED_AUTO_ATTACK | PROC_FLAG_TAKEN_SPELL_RANGED_DMG_CLASS,
                PROC_SPELL_TYPE_DAMAGE | PROC_SPELL_TYPE_HEAL | PROC_SPELL_TYPE_NO_DMG_HEAL, PROC_SPELL_PHASE_HIT,
                PROC_HIT_NORMAL | PROC_HIT_CRITICAL | PROC_HIT_BLOCK | PROC_HIT_FULL_BLOCK, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            // Blessed Deflection
            SpellTemplate deflectionSpellTemplate = BuildBaseTemplate("Blessed Deflection", SpellClassAuraType.PaladinDeflection, icon, "Holy damage turned back from a blocked attack.", string.Empty);
            deflectionSpellTemplate.SchoolMask = SCHOOL_MASK_HOLY;
            deflectionSpellTemplate.SpellRadius = Configuration.CLASSAURA_PALADIN_BLOCK_DEFLECTION_RADIUS_IN_YARDS;
            deflectionSpellTemplate.SpellVisualID1 = Convert.ToUInt32(Configuration.CLASSAURA_PALADIN_BLOCK_DEFLECTION_SPELL_VISUAL_ID);
            SpellEffectWOW deflectionEffect = new SpellEffectWOW(SpellWOWEffectType.SchoolDamage, SpellWOWAuraType.None, 0, 0, 0, 0, 0, 0);
            deflectionEffect.ImplicitTargetA = SpellWOWTargetType.UnitSourceAreaEnemy;
            deflectionEffect.EffectRadiusIndex = Convert.ToUInt32(deflectionSpellTemplate.SpellRadiusDBCID);
            deflectionSpellTemplate.WOWSpellEffects.Add(deflectionEffect);
            deflectionSpellTemplate.CannotCrit = true;
            deflectionSpellTemplate.InfluencedBySpellPower = false;
            spellTemplates.Add(deflectionSpellTemplate);

            SpellTemplate healSpellTemplate = BuildBaseTemplate("Light's Reward", SpellClassAuraType.PaladinHeal, icon, "Healed by the light you gave to another.", string.Empty);
            SpellEffectWOW healEffect = new SpellEffectWOW(SpellWOWEffectType.Heal, SpellWOWAuraType.None, 0, 0, 0, 0, 0, 0);
            healEffect.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
            healSpellTemplate.WOWSpellEffects.Add(healEffect);
            healSpellTemplate.SpellVisualID1 = Convert.ToUInt32(Configuration.CLASSAURA_PALADIN_HEAL_SPELL_VISUAL_ID);
            healSpellTemplate.GenerateNoThreat = true;
            healSpellTemplate.CannotCrit = true;
            healSpellTemplate.InfluencedBySpellPower = false;
            spellTemplates.Add(healSpellTemplate);
        }

        private static void AddShadowKnightSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_SHADOWKNIGHT_SPELL_ICON_EQ_ID;
            string description = string.Concat("Spell power increased by an amount equal to ", Pct(Configuration.CLASSAURA_SHADOWKNIGHT_SPELL_POWER_FROM_ATTACK_POWER_PERCENT),
                " of your attack power. Attack critical strikes make your next harmful spell within ", Seconds(Configuration.CLASSAURA_SHADOWKNIGHT_INSTANT_CAST_DURATION_IN_MS),
                " instant. Cannot occur more than once every ", Seconds(Configuration.CLASSAURA_SHADOWKNIGHT_INSTANT_CAST_COOLDOWN_IN_MS), ".");
            spellTemplates.Add(BuildPassiveTemplate("Spellsword", SpellClassAuraType.ShadowKnightPassive, icon, description));

            // The attack power share is the same as Sheath of Light pair
            List<SpellEffectWOW> auraEffects = new List<SpellEffectWOW>();
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModSpellDamageOfAttackPower, Configuration.CLASSAURA_SHADOWKNIGHT_SPELL_POWER_FROM_ATTACK_POWER_PERCENT, SCHOOL_MASK_ALL, SpellWOWTargetType.UnitCaster));
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModSpellHealingOfAttackPower, Configuration.CLASSAURA_SHADOWKNIGHT_SPELL_POWER_FROM_ATTACK_POWER_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Spellsword (Shadow Knight)", SpellClassAuraType.ShadowKnightAura, icon, description, auraEffects);
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraShadowKnightAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_MELEE_AUTO_ATTACK, 0, 0, PROC_HIT_CRITICAL, Configuration.CLASSAURA_SHADOWKNIGHT_INSTANT_CAST_COOLDOWN_IN_MS, 0);
            spellTemplates.Add(auraSpellTemplate);

            List<SpellEffectWOW> edgeEffects = new List<SpellEffectWOW>();
            edgeEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildStackingAuraTemplate("Spellsword's Edge", SpellClassAuraType.ShadowKnightEdge, icon, "The next harmful spell cast is instant.", edgeEffects,
                1, Configuration.CLASSAURA_SHADOWKNIGHT_INSTANT_CAST_DURATION_IN_MS, false));
        }

        private static void AddWarriorSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_WARRIOR_SPELL_ICON_EQ_ID;
            string description = string.Concat("Automatic Riposte: ", Pct(Configuration.CLASSAURA_WARRIOR_RIPOSTE_CHANCE_PERCENT),
                " of melee strikes against you are riposted, avoiding the blow and answering it with your main hand. Unrelenting Assault: every ",
                Seconds(Configuration.CLASSAURA_WARRIOR_UNRELENTING_ASSAULT_STACK_INTERVAL_IN_MS), " you gain a stack that raises the damage of every attack and spell by ",
                Pct(Configuration.CLASSAURA_WARRIOR_UNRELENTING_ASSAULT_DAMAGE_PERCENT_PER_STACK), ", up to ", Configuration.CLASSAURA_WARRIOR_UNRELENTING_ASSAULT_MAX_STACKS.ToString(),
                " stacks. Each melee, ranged, or single target spell attack that lands on you removes a stack, even when partially blocked. Misses, dodges, parries, full blocks, area attacks, and damage over time do not.");
            spellTemplates.Add(BuildPassiveTemplate("Warmaster", SpellClassAuraType.WarriorPassive, icon, description));

            // The riposte comes from the melee outcome roll and the stacks from a timer, both in the mod.  A direct attack landing on the warrior takes a stack away (no PROC_FLAG_TAKEN_PERIODIC, so ticks never do)
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Warmaster (Warrior)", SpellClassAuraType.WarriorAura, icon, description, new List<SpellEffectWOW>());
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraWarriorAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_TAKEN_MELEE_AUTO_ATTACK | PROC_FLAG_TAKEN_SPELL_MELEE_DMG_CLASS | PROC_FLAG_TAKEN_RANGED_AUTO_ATTACK
                | PROC_FLAG_TAKEN_SPELL_RANGED_DMG_CLASS | PROC_FLAG_TAKEN_SPELL_NONE_DMG_CLASS_NEG | PROC_FLAG_TAKEN_SPELL_MAGIC_DMG_CLASS_NEG,
                PROC_SPELL_TYPE_DAMAGE, PROC_SPELL_PHASE_HIT, PROC_HIT_NORMAL | PROC_HIT_CRITICAL | PROC_HIT_ABSORB, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            string assaultDescription = string.Concat("Damage dealt increased by ", Pct(Configuration.CLASSAURA_WARRIOR_UNRELENTING_ASSAULT_DAMAGE_PERCENT_PER_STACK),
                " per stack. Each melee, ranged, or single target spell attack that lands on you removes a stack.");
            List<SpellEffectWOW> assaultEffects = new List<SpellEffectWOW>();
            assaultEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentDone, Configuration.CLASSAURA_WARRIOR_UNRELENTING_ASSAULT_DAMAGE_PERCENT_PER_STACK, SCHOOL_MASK_ALL, SpellWOWTargetType.UnitCaster));
            SpellTemplate assaultSpellTemplate = BuildPermanentAuraTemplate("Unrelenting Assault", SpellClassAuraType.WarriorUnrelentingAssault, icon, assaultDescription, assaultEffects);
            assaultSpellTemplate.MaxStackAmount = Convert.ToUInt32(Math.Max(1, Configuration.CLASSAURA_WARRIOR_UNRELENTING_ASSAULT_MAX_STACKS));
            spellTemplates.Add(assaultSpellTemplate);

            // The visual is the WoW Riposte ability's (spell 14251), and the name is kept distinct from that ability so damage parsers never confuse the two
            SpellTemplate riposteSpellTemplate = BuildBaseTemplate("Warmaster Riposte", SpellClassAuraType.WarriorRiposte, icon, "A riposte answering a melee strike.", string.Empty);
            SpellEffectWOW riposteEffect = new SpellEffectWOW(SpellWOWEffectType.Dummy, SpellWOWAuraType.None, 0, 0, 0, 0, 0, 0);
            riposteEffect.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
            riposteSpellTemplate.WOWSpellEffects.Add(riposteEffect);
            riposteSpellTemplate.SpellVisualID1 = Convert.ToUInt32(Configuration.CLASSAURA_WARRIOR_RIPOSTE_SPELL_VISUAL_ID);
            riposteSpellTemplate.GenerateNoThreat = true;
            spellTemplates.Add(riposteSpellTemplate);
        }

        private static void AddWizardSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_WIZARD_SPELL_ICON_EQ_ID;
            string description = string.Concat("Casting while moving carries no movement speed penalty. Every ", Seconds(Configuration.CLASSAURA_WIZARD_FOCUS_STILL_INTERVAL_IN_MS),
                " standing still raises spell damage by ", Pct(Configuration.CLASSAURA_WIZARD_FOCUS_SPELL_DAMAGE_PERCENT_PER_STACK), ", stacking up to ",
                Configuration.CLASSAURA_WIZARD_FOCUS_MAX_STACKS.ToString(), " times. Starting to move removes ", GetStackWord(Configuration.CLASSAURA_WIZARD_FOCUS_STACKS_LOST_PER_MOVEMENT_EVENT),
                ", and so does every ", (Configuration.CLASSAURA_WIZARD_FOCUS_MOVEMENT_INTERVAL_IN_MS / 1000).ToString(), " second", Configuration.CLASSAURA_WIZARD_FOCUS_MOVEMENT_INTERVAL_IN_MS >= 2000 ? "s" : "",
                " spent moving.");
            spellTemplates.Add(BuildPassiveTemplate("Unshaken Channeler", SpellClassAuraType.WizardPassive, icon, description));
            spellTemplates.Add(BuildPermanentAuraTemplate("Unshaken Channeler (Wizard)", SpellClassAuraType.WizardAura, icon, description, new List<SpellEffectWOW>()));

            string focusDescription = string.Concat("Spell damage increased by ", Pct(Configuration.CLASSAURA_WIZARD_FOCUS_SPELL_DAMAGE_PERCENT_PER_STACK), " per stack. Standing still adds stacks and movement removes them.");
            List<SpellEffectWOW> focusEffects = new List<SpellEffectWOW>();
            focusEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentDone, Configuration.CLASSAURA_WIZARD_FOCUS_SPELL_DAMAGE_PERCENT_PER_STACK, SCHOOL_MASK_MAGIC, SpellWOWTargetType.UnitCaster));
            // No duration, since standing still keeps adding to it and movement is what takes it away (the mod also removes it on death or losing the wizard aura)
            SpellTemplate focusSpellTemplate = BuildStackingAuraTemplate("Channeler's Focus", SpellClassAuraType.WizardFocus, icon, focusDescription, focusEffects,
                Configuration.CLASSAURA_WIZARD_FOCUS_MAX_STACKS, 0, false);
            focusSpellTemplate.AuraDuration.IsInfinite = true;
            spellTemplates.Add(focusSpellTemplate);
        }

        private static string GetStackWord(int stackCount)
        {
            if (stackCount == 1)
                return "a stack";
            return string.Concat(stackCount.ToString(), " stacks");
        }

        private static void AddMagicianSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_MAGICIAN_SPELL_ICON_EQ_ID;
            string description = string.Concat("Your pet's strikes raise your spell damage by ", Pct(Configuration.CLASSAURA_MAGICIAN_PET_STRIKE_SPELL_DAMAGE_PERCENT_PER_STACK), " for ",
                Seconds(Configuration.CLASSAURA_MAGICIAN_PET_STRIKE_DURATION_IN_MS), ", stacking up to ", Configuration.CLASSAURA_MAGICIAN_PET_STRIKE_MAX_STACKS.ToString(),
                " times. Your spell critical strikes raise your pet's damage by ", Pct(Configuration.CLASSAURA_MAGICIAN_OWNER_CRIT_PET_DAMAGE_PERCENT_PER_STACK), " for ",
                Seconds(Configuration.CLASSAURA_MAGICIAN_OWNER_CRIT_DURATION_IN_MS), ", stacking up to ", Configuration.CLASSAURA_MAGICIAN_OWNER_CRIT_MAX_STACKS.ToString(), " times.");
            spellTemplates.Add(BuildPassiveTemplate("Bound Conjurer", SpellClassAuraType.MagicianPassive, icon, description));

            // Owner side: procs on the owner's own damaging spell crits
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Bound Conjurer (Magician)", SpellClassAuraType.MagicianAura, icon, description, new List<SpellEffectWOW>());
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraMagicianAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_NEG | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_NEG, PROC_SPELL_TYPE_DAMAGE, PROC_SPELL_PHASE_HIT,
                PROC_HIT_CRITICAL, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            // Pet side: a hidden passive the mod puts on the pet, procs on the pet's landed melee swings and spell hits
            SpellTemplate petPassiveSpellTemplate = BuildBaseTemplate("Conjurer's Bond", SpellClassAuraType.MagicianPetPassive, icon, "Strikes empower the pet's master.", "Strikes empower the pet's master.");
            petPassiveSpellTemplate.AuraDuration.IsInfinite = true;
            petPassiveSpellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            petPassiveSpellTemplate.IsPassiveAbility = true;
            petPassiveSpellTemplate.ForceHiddenFromDisplay = true;
            petPassiveSpellTemplate.AlwaysPersist = true;
            petPassiveSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraMagicianPetAuraScript";
            petPassiveSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_MELEE_AUTO_ATTACK | PROC_FLAG_DONE_SPELL_MELEE_DMG_CLASS | PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_NEG | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_NEG,
                PROC_SPELL_TYPE_DAMAGE, PROC_SPELL_PHASE_HIT, PROC_HIT_NORMAL | PROC_HIT_CRITICAL, 0, 0);
            spellTemplates.Add(petPassiveSpellTemplate);

            string ownerFocusDescription = string.Concat("Spell damage increased by ", Pct(Configuration.CLASSAURA_MAGICIAN_PET_STRIKE_SPELL_DAMAGE_PERCENT_PER_STACK), " per stack.");
            List<SpellEffectWOW> ownerFocusEffects = new List<SpellEffectWOW>();
            ownerFocusEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentDone, Configuration.CLASSAURA_MAGICIAN_PET_STRIKE_SPELL_DAMAGE_PERCENT_PER_STACK, SCHOOL_MASK_MAGIC, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildStackingAuraTemplate("Conjurer's Insight", SpellClassAuraType.MagicianOwnerFocus, icon, ownerFocusDescription, ownerFocusEffects,
                Configuration.CLASSAURA_MAGICIAN_PET_STRIKE_MAX_STACKS, Configuration.CLASSAURA_MAGICIAN_PET_STRIKE_DURATION_IN_MS, false));

            string petFuryDescription = string.Concat("Damage dealt increased by ", Pct(Configuration.CLASSAURA_MAGICIAN_OWNER_CRIT_PET_DAMAGE_PERCENT_PER_STACK), " per stack.");
            List<SpellEffectWOW> petFuryEffects = new List<SpellEffectWOW>();
            petFuryEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentDone, Configuration.CLASSAURA_MAGICIAN_OWNER_CRIT_PET_DAMAGE_PERCENT_PER_STACK, SCHOOL_MASK_ALL, SpellWOWTargetType.UnitTargetAlly));
            spellTemplates.Add(BuildStackingAuraTemplate("Conjurer's Fury", SpellClassAuraType.MagicianPetFury, icon, petFuryDescription, petFuryEffects,
                Configuration.CLASSAURA_MAGICIAN_OWNER_CRIT_MAX_STACKS, Configuration.CLASSAURA_MAGICIAN_OWNER_CRIT_DURATION_IN_MS, false));
        }

        private static void AddNecromancerSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_NECROMANCER_SPELL_ICON_EQ_ID;
            string description = string.Concat("Harmful effects enemies place on you pass to your pet instead, no more than once every ",
                Seconds(Configuration.CLASSAURA_NECROMANCER_DEBUFF_TRANSFER_COOLDOWN_IN_MS), ". Each pet strike marks its target, raising the damage it takes from your direct spells by ",
                Pct(Configuration.CLASSAURA_NECROMANCER_MARK_DIRECT_DAMAGE_PERCENT_PER_STACK), " and from your damage over time spells by ",
                Pct(Configuration.CLASSAURA_NECROMANCER_MARK_DOT_DAMAGE_PERCENT_PER_STACK), " per mark for ", Seconds(Configuration.CLASSAURA_NECROMANCER_MARK_DURATION_IN_MS),
                ", up to ", Configuration.CLASSAURA_NECROMANCER_MARK_MAX_STACKS.ToString(), " marks.");
            spellTemplates.Add(BuildPassiveTemplate("Grave Pact", SpellClassAuraType.NecromancerPassive, icon, description));
            spellTemplates.Add(BuildPermanentAuraTemplate("Grave Pact (Necromancer)", SpellClassAuraType.NecromancerAura, icon, description, new List<SpellEffectWOW>()));

            // The mark is a counter the mod reads in its damage hooks, so it carries no effect of its own
            string markDescription = string.Concat("Takes ", Pct(Configuration.CLASSAURA_NECROMANCER_MARK_DIRECT_DAMAGE_PERCENT_PER_STACK), " more direct spell damage and ",
                Pct(Configuration.CLASSAURA_NECROMANCER_MARK_DOT_DAMAGE_PERCENT_PER_STACK), " more damage over time from the necromancer per mark.");
            List<SpellEffectWOW> markEffects = new List<SpellEffectWOW>();
            markEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitTargetEnemy));
            spellTemplates.Add(BuildStackingAuraTemplate("Grave Mark", SpellClassAuraType.NecromancerMark, icon, markDescription, markEffects,
                Configuration.CLASSAURA_NECROMANCER_MARK_MAX_STACKS, Configuration.CLASSAURA_NECROMANCER_MARK_DURATION_IN_MS, true));
        }

        private static void AddClericSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_CLERIC_SPELL_ICON_EQ_ID;
            string description = string.Concat("Each area heal you cast readies a Sacred Focus charge for ", Seconds(Configuration.CLASSAURA_CLERIC_CADENCE_DURATION_IN_MS), ", up to ",
                Configuration.CLASSAURA_CLERIC_CADENCE_MAX_STACKS.ToString(), ". A charge is spent to cut the cast time and mana cost of your next single target heal by ",
                Pct(Configuration.CLASSAURA_CLERIC_CADENCE_REDUCTION_PERCENT), ". Complete Healing cannot use a charge. Single target heals also grant the target ",
                Pct(Configuration.CLASSAURA_CLERIC_HEAL_HASTE_PERCENT_PER_STACK), " haste for ", Seconds(Configuration.CLASSAURA_CLERIC_HEAL_HASTE_DURATION_IN_MS), ", stacking up to ",
                Configuration.CLASSAURA_CLERIC_HEAL_HASTE_MAX_STACKS.ToString(), " times and ignoring the haste cap.");
            spellTemplates.Add(BuildPassiveTemplate("Sacred Cadence", SpellClassAuraType.ClericPassive, icon, description));
            spellTemplates.Add(BuildPermanentAuraTemplate("Sacred Cadence (Cleric)", SpellClassAuraType.ClericAura, icon, description, new List<SpellEffectWOW>()));

            string cadenceDescription = string.Concat("Each charge cuts the cast time and mana cost of the next single target heal by ", Pct(Configuration.CLASSAURA_CLERIC_CADENCE_REDUCTION_PERCENT), ". The cast time cut adds together with Chi Surge.");
            List<SpellEffectWOW> cadenceEffects = new List<SpellEffectWOW>();
            cadenceEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildStackingAuraTemplate("Sacred Focus", SpellClassAuraType.ClericCadence, icon, cadenceDescription, cadenceEffects,
                Configuration.CLASSAURA_CLERIC_CADENCE_MAX_STACKS, Configuration.CLASSAURA_CLERIC_CADENCE_DURATION_IN_MS, false));

            // Stacks pool across every cleric healing the target (one shared aura), so the cap holds no matter how many clerics are healing
            string hasteDescription = string.Concat("Haste increased by ", Pct(Configuration.CLASSAURA_CLERIC_HEAL_HASTE_PERCENT_PER_STACK), " per stack.");
            List<SpellEffectWOW> hasteEffects = new List<SpellEffectWOW>();
            hasteEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModMeleeRangedHaste, Configuration.CLASSAURA_CLERIC_HEAL_HASTE_PERCENT_PER_STACK, 0, SpellWOWTargetType.UnitTargetAlly));
            hasteEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModCastingSpeedNotStack, Configuration.CLASSAURA_CLERIC_HEAL_HASTE_PERCENT_PER_STACK, 0, SpellWOWTargetType.UnitTargetAlly));
            spellTemplates.Add(BuildStackingAuraTemplate("Hastened Faith", SpellClassAuraType.ClericHaste, icon, hasteDescription, hasteEffects,
                Configuration.CLASSAURA_CLERIC_HEAL_HASTE_MAX_STACKS, Configuration.CLASSAURA_CLERIC_HEAL_HASTE_DURATION_IN_MS, false));
        }

        private static void AddDruidSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_DRUID_SPELL_ICON_EQ_ID;
            string description = string.Concat("Your direct heals leave a regeneration that heals ", Pct(Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_PERCENT), " of the amount over ",
                Seconds(Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_DURATION_IN_MS), ". Nature's Balance: casting a fire, cold, or nature direct damage spell with a base cast time longer than ",
                SecondsWithFraction(Configuration.CLASSAURA_DRUID_NATURES_BALANCE_MIN_BASE_CAST_TIME_IN_MS), " builds a stack of that element for ",
                Seconds(Configuration.CLASSAURA_DRUID_NATURES_BALANCE_DURATION_IN_MS), ", up to ", Configuration.CLASSAURA_DRUID_NATURES_BALANCE_MAX_STACKS.ToString(),
                " stacks. Your next direct damage spell of a different element spends every stack at once, dealing ", Pct(Configuration.CLASSAURA_DRUID_NATURES_BALANCE_DAMAGE_PERCENT_PER_STACK),
                " more damage for each stack spent. Entangle Strike: your landed melee and ranged attacks entangle the target for ",
                Seconds(Configuration.CLASSAURA_DRUID_ENTANGLE_STRIKE_DURATION_IN_MS), ", stacking up to ", Configuration.CLASSAURA_DRUID_ENTANGLE_STRIKE_MAX_STACKS.ToString(),
                " times. Each stack lowers the physical damage you and your pet take from that target by ", Pct(Configuration.CLASSAURA_DRUID_ENTANGLE_STRIKE_DAMAGE_TAKEN_PERCENT_PER_STACK),
                " and raises the melee damage you and your pet deal to it from behind by ", Pct(Configuration.CLASSAURA_DRUID_ENTANGLE_STRIKE_BEHIND_DAMAGE_PERCENT_PER_STACK), ".");
            spellTemplates.Add(BuildPassiveTemplate("One With Nature", SpellClassAuraType.DruidPassive, icon, description));

            // Direct heals leave the echo and landed physical attacks entangle, both of which the script tells apart (no PROC_FLAG_DONE_PERIODIC, so heal over time ticks never count)
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("One With Nature (Druid)", SpellClassAuraType.DruidAura, icon, description, new List<SpellEffectWOW>());
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraDruidAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_POS | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_POS | PROC_FLAG_DONE_MELEE_AUTO_ATTACK
                | PROC_FLAG_DONE_RANGED_AUTO_ATTACK | PROC_FLAG_DONE_SPELL_MELEE_DMG_CLASS | PROC_FLAG_DONE_SPELL_RANGED_DMG_CLASS,
                PROC_SPELL_TYPE_HEAL | PROC_SPELL_TYPE_DAMAGE, PROC_SPELL_PHASE_HIT, 0, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            // Per-tick amount is handed in by the mod at cast time.  Unlike every other class aura effect, this one stacks per druid (the mod leaves it out of its one-copy-per-target rule), so three druids can each have their own regrowth on the same target
            SpellTemplate regrowthSpellTemplate = BuildBaseTemplate("Nature's Echo", SpellClassAuraType.DruidRegrowth, icon,
                string.Concat("Regenerating health over ", Seconds(Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_DURATION_IN_MS), "."),
                string.Concat("Regenerating health over ", Seconds(Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_DURATION_IN_MS), "."));
            regrowthSpellTemplate.AuraDuration.SetFixedDuration(Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_DURATION_IN_MS);
            regrowthSpellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.PeriodicHeal, 0, 0, SpellWOWTargetType.UnitTargetAlly,
                Convert.ToUInt32(Math.Max(1000, Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_TICK_INTERVAL_IN_MS))));
            regrowthSpellTemplate.InfluencedBySpellPower = false;
            regrowthSpellTemplate.CannotCrit = true;
            regrowthSpellTemplate.GenerateNoThreat = true;
            spellTemplates.Add(regrowthSpellTemplate);

            // One element at a time, since a spell of another element spends what is there before its own stack is built.
            spellTemplates.Add(BuildNaturesBalanceTemplate("Nature's Balance (Fire)", SpellClassAuraType.DruidNaturesBalanceFire, Configuration.CLASSAURA_DRUID_NATURES_BALANCE_FIRE_SPELL_ICON_EQ_ID));
            spellTemplates.Add(BuildNaturesBalanceTemplate("Nature's Balance (Cold)", SpellClassAuraType.DruidNaturesBalanceCold, Configuration.CLASSAURA_DRUID_NATURES_BALANCE_COLD_SPELL_ICON_EQ_ID));
            spellTemplates.Add(BuildNaturesBalanceTemplate("Nature's Balance (Nature)", SpellClassAuraType.DruidNaturesBalanceNature, Configuration.CLASSAURA_DRUID_NATURES_BALANCE_NATURE_SPELL_ICON_EQ_ID));

            // A per-druid copy on the target, since only the druid who entangled it and that druid's pet are affected
            string entangleDescription = string.Concat("The druid who entangled it and that druid's pet take ",
                Pct(Configuration.CLASSAURA_DRUID_ENTANGLE_STRIKE_DAMAGE_TAKEN_PERCENT_PER_STACK), " less physical damage from it per stack, and deal ",
                Pct(Configuration.CLASSAURA_DRUID_ENTANGLE_STRIKE_BEHIND_DAMAGE_PERCENT_PER_STACK), " more melee damage to it from behind per stack.");
            List<SpellEffectWOW> entangleEffects = new List<SpellEffectWOW>();
            entangleEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitTargetEnemy));
            spellTemplates.Add(BuildStackingAuraTemplate("Entangle Strike", SpellClassAuraType.DruidEntangleStrike, Configuration.CLASSAURA_DRUID_ENTANGLE_STRIKE_SPELL_ICON_EQ_ID,
                entangleDescription, entangleEffects, Configuration.CLASSAURA_DRUID_ENTANGLE_STRIKE_MAX_STACKS, Configuration.CLASSAURA_DRUID_ENTANGLE_STRIKE_DURATION_IN_MS, true));
        }

        private static SpellTemplate BuildNaturesBalanceTemplate(string name, SpellClassAuraType spellType, int spellIconEQID)
        {
            string description = string.Concat("Your next direct damage spell of a different element spends every stack at once, dealing ",
                Pct(Configuration.CLASSAURA_DRUID_NATURES_BALANCE_DAMAGE_PERCENT_PER_STACK), " more damage for each stack spent.");
            List<SpellEffectWOW> effects = new List<SpellEffectWOW>();
            effects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            return BuildStackingAuraTemplate(name, spellType, spellIconEQID, description, effects,
                Configuration.CLASSAURA_DRUID_NATURES_BALANCE_MAX_STACKS, Configuration.CLASSAURA_DRUID_NATURES_BALANCE_DURATION_IN_MS, false);
        }

        private static void AddShamanSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_SHAMAN_SPELL_ICON_EQ_ID;
            string statsText = "total stats";
            string warspiritGrantText = string.Concat(Pct(Configuration.CLASSAURA_SHAMAN_WARSPIRIT_STAT_PERCENT_PER_STACK), " increased ", statsText, " for ",
                Seconds(Configuration.CLASSAURA_SHAMAN_WARSPIRIT_STAT_DURATION_IN_MS), ", stacking up to ", Configuration.CLASSAURA_SHAMAN_WARSPIRIT_STAT_MAX_STACKS.ToString(), " times");
            string description = string.Concat("Your autoattacks have a ", Pct(Configuration.CLASSAURA_SHAMAN_DOT_EXTEND_CHANCE_PERCENT),
                " chance to extend any of your damage over time effects on the target by ", Seconds(Configuration.CLASSAURA_SHAMAN_DOT_EXTEND_IN_MS),
                ". Healing an ally grants them ", Pct(Configuration.CLASSAURA_SHAMAN_HEAL_STAT_PERCENT_PER_STACK), " increased ", statsText, " for ",
                Seconds(Configuration.CLASSAURA_SHAMAN_HEAL_STAT_DURATION_IN_MS), ", stacking up to ", Configuration.CLASSAURA_SHAMAN_HEAL_STAT_MAX_STACKS.ToString(),
                " times. Warspirit can be toggled on so your landed attacks, abilities, and damaging spells grant you ", warspiritGrantText, " instead, while your heals no longer grant anything.");
            spellTemplates.Add(BuildPassiveTemplate("Spirit Channeler", SpellClassAuraType.ShamanPassive, icon, description));

            // Direct heals grant the vigor to allies, or with Warspirit on, landed attacks grant it to the shaman, which the script tells apart (no PROC_FLAG_DONE_PERIODIC, so ticks never grant either)
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Spirit Channeler (Shaman)", SpellClassAuraType.ShamanAura, icon, description, new List<SpellEffectWOW>());
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraShamanAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_POS | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_POS | PROC_FLAG_DONE_MELEE_AUTO_ATTACK
                | PROC_FLAG_DONE_RANGED_AUTO_ATTACK | PROC_FLAG_DONE_SPELL_MELEE_DMG_CLASS | PROC_FLAG_DONE_SPELL_RANGED_DMG_CLASS | PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_NEG
                | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_NEG, PROC_SPELL_TYPE_HEAL | PROC_SPELL_TYPE_DAMAGE, PROC_SPELL_PHASE_HIT, 0, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            // Warspirit (self stats from attacks instead of ally stats from heals)
            string warspiritDescription = string.Concat("Toggle. While active, your landed attacks, abilities, and damaging spells grant you ", warspiritGrantText,
                ". Your heals no longer grant Spirit's Vigor to allies.");
            string warspiritAuraDescription = "Landed attacks, abilities, and damaging spells grant you Warspirit's Vigor. Heals no longer grant Spirit's Vigor.";
            SpellTemplate warspiritSpellTemplate = BuildBaseTemplate("Warspirit", SpellClassAuraType.ShamanWarspirit, Configuration.CLASSAURA_SHAMAN_WARSPIRIT_SPELL_ICON_EQ_ID,
                warspiritDescription, warspiritAuraDescription);
            warspiritSpellTemplate.AuraDuration.IsInfinite = true;
            warspiritSpellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            warspiritSpellTemplate.SkillLine = SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Combat); // Unlike the other class aura spells, this one is in the spellbook
            warspiritSpellTemplate.IsToggleAura = true;
            warspiritSpellTemplate.ShowOnShapeshiftBar = true;
            warspiritSpellTemplate.PersistThroughDeath = true;
            warspiritSpellTemplate.AllowInShapeshift = true; // Feral druids can flip it without leaving cat or bear form
            spellTemplates.Add(warspiritSpellTemplate);

            // Stacks pool across every shaman healing the target (one shared aura), so the cap holds no matter how many shamans are healing
            string vigorDescription = string.Concat("Total stats increased by ", Pct(Configuration.CLASSAURA_SHAMAN_HEAL_STAT_PERCENT_PER_STACK), " per stack.");
            List<SpellEffectWOW> vigorEffects = new List<SpellEffectWOW>();
            vigorEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModTotalStatPercentage, Configuration.CLASSAURA_SHAMAN_HEAL_STAT_PERCENT_PER_STACK, STAT_ALL, SpellWOWTargetType.UnitTargetAlly));
            spellTemplates.Add(BuildStackingAuraTemplate("Spirit's Vigor", SpellClassAuraType.ShamanVigor, icon, vigorDescription, vigorEffects,
                Configuration.CLASSAURA_SHAMAN_HEAL_STAT_MAX_STACKS, Configuration.CLASSAURA_SHAMAN_HEAL_STAT_DURATION_IN_MS, false));

            // Only ever on the shaman who built it, and turning Warspirit off takes it away (the mod handles that)
            string warspiritVigorDescription = string.Concat("Total stats increased by ", Pct(Configuration.CLASSAURA_SHAMAN_WARSPIRIT_STAT_PERCENT_PER_STACK), " per stack.");
            List<SpellEffectWOW> warspiritVigorEffects = new List<SpellEffectWOW>();
            warspiritVigorEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModTotalStatPercentage, Configuration.CLASSAURA_SHAMAN_WARSPIRIT_STAT_PERCENT_PER_STACK, STAT_ALL, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildStackingAuraTemplate("Warspirit's Vigor", SpellClassAuraType.ShamanWarspiritVigor, icon, warspiritVigorDescription, warspiritVigorEffects,
                Configuration.CLASSAURA_SHAMAN_WARSPIRIT_STAT_MAX_STACKS, Configuration.CLASSAURA_SHAMAN_WARSPIRIT_STAT_DURATION_IN_MS, false));
        }

        private static void AddCastSpeedHelperSpell(List<SpellTemplate> spellTemplates)
        {
            SpellTemplate helperSpellTemplate = BuildBaseTemplate("Cast Adjustment", SpellClassAuraType.CastSpeedHelper, 0, "Adjusts the cast in progress.", "Adjusts the cast in progress.");
            helperSpellTemplate.AuraDuration.SetFixedDuration(CAST_SPEED_HELPER_DURATION_IN_MS);
            helperSpellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModCastingSpeedNotStack, 0, 0, SpellWOWTargetType.UnitCaster));
            helperSpellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.AddPctModifier, 0, 14, SpellWOWTargetType.UnitCaster)); // 14 = SPELLMOD_COST
            helperSpellTemplate.ForceHiddenFromDisplay = true;
            helperSpellTemplate.PreventAuraClickOff = true;
            spellTemplates.Add(helperSpellTemplate);
        }
    }
}
