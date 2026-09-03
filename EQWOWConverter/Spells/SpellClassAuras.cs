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
        private const int PROC_FLAG_DONE_RANGED_AUTO_ATTACK = 0x00000040;
        private const int PROC_FLAG_DONE_SPELL_MELEE_DMG_CLASS = 0x00000010;
        private const int PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_POS = 0x00000400;
        private const int PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_NEG = 0x00000800;
        private const int PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_POS = 0x00004000;
        private const int PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_NEG = 0x00010000;
        private const int PROC_SPELL_TYPE_DAMAGE = 0x1;
        private const int PROC_SPELL_TYPE_HEAL = 0x2;
        private const int PROC_SPELL_PHASE_HIT = 0x2;
        private const int PROC_HIT_NORMAL = 0x1;
        private const int PROC_HIT_CRITICAL = 0x2;
        private const int PROC_HIT_MISS = 0x4;
        private const int PROC_HIT_DODGE = 0x10;
        private const int PROC_HIT_PARRY = 0x20;
        private const int PROC_HIT_BLOCK = 0x40;
        private const int PROC_HIT_ABSORB = 0x400;

        // Spell school masks
        private const int SCHOOL_MASK_PHYSICAL = 1;
        private const int SCHOOL_MASK_MAGIC = 126; // Every non-physical school
        private const int SCHOOL_MASK_ALL = 127;

        // Mechanics (AzerothCore SharedDefines.h)
        private const int MECHANIC_ROOT = 7;
        private const int MECHANIC_SNARE = 11;
        private const int MECHANIC_DAZE = 27;

        // ModTotalStatPercentage misc value that covers every stat (what Blessing of Kings uses)
        private const int STAT_ALL = -1;

        // Powers
        private const int POWER_MANA = 0;

        // The cast speed helper only lives across one cast, but it is given a real duration as a safety net
        private const int CAST_SPEED_HELPER_DURATION_IN_MS = 60000;

        public static int GetSpellID(ClassAuraSpellType spellType)
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
                case ClassEQType.Enchanter: return GetSpellID(ClassAuraSpellType.EnchanterPassive);
                case ClassEQType.Bard: return GetSpellID(ClassAuraSpellType.BardPassive);
                case ClassEQType.Monk: return GetSpellID(ClassAuraSpellType.MonkPassive);
                case ClassEQType.Ranger: return GetSpellID(ClassAuraSpellType.RangerPassive);
                case ClassEQType.Rogue: return GetSpellID(ClassAuraSpellType.RoguePassive);
                case ClassEQType.Paladin: return GetSpellID(ClassAuraSpellType.PaladinPassive);
                case ClassEQType.ShadowKnight: return GetSpellID(ClassAuraSpellType.ShadowKnightPassive);
                case ClassEQType.Warrior: return GetSpellID(ClassAuraSpellType.WarriorPassive);
                case ClassEQType.Wizard: return GetSpellID(ClassAuraSpellType.WizardPassive);
                case ClassEQType.Magician: return GetSpellID(ClassAuraSpellType.MagicianPassive);
                case ClassEQType.Necromancer: return GetSpellID(ClassAuraSpellType.NecromancerPassive);
                case ClassEQType.Cleric: return GetSpellID(ClassAuraSpellType.ClericPassive);
                case ClassEQType.Druid: return GetSpellID(ClassAuraSpellType.DruidPassive);
                case ClassEQType.Shaman: return GetSpellID(ClassAuraSpellType.ShamanPassive);
                default: return 0;
            }
        }

        public static List<KeyValuePair<string, string>> GetSystemConfigRows()
        {
            List<KeyValuePair<string, string>> rows = new List<KeyValuePair<string, string>>();
            rows.Add(new KeyValuePair<string, string>("ClassAuraEnabled", Configuration.CLASSAURA_ENABLED == true ? "1" : "0"));
            for (int i = 0; i < (int)ClassAuraSpellType.Count; i++)
            {
                ClassAuraSpellType spellType = (ClassAuraSpellType)i;
                int spellID = IsSpellTypeEnabled(spellType) == true ? GetSpellID(spellType) : 0;
                rows.Add(new KeyValuePair<string, string>(string.Concat("ClassAuraSpellID", spellType.ToString()), spellID.ToString()));
            }
            rows.Add(new KeyValuePair<string, string>("ClassAuraPrivateSpellFamilyID", Configuration.SPELL_EQ_PRIVATE_SPELL_FAMILY_ID.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraEnchanterFocusManaThresholdPercent", Configuration.CLASSAURA_ENCHANTER_FOCUS_MANA_THRESHOLD_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraBardInstrumentMeleeAutoAttackDamagePercent", Configuration.CLASSAURA_BARD_INSTRUMENT_MELEE_AUTOATTACK_DAMAGE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraMonkSelfHealCastTimeReductionPercent", Configuration.CLASSAURA_MONK_SELF_HEAL_CAST_TIME_REDUCTION_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraRangerRicochetChancePercent", Configuration.CLASSAURA_RANGER_RICOCHET_CHANCE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraRangerRicochetRange", Configuration.CLASSAURA_RANGER_RICOCHET_RANGE.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraPaladinHealSelfPercent", Configuration.CLASSAURA_PALADIN_HEAL_SELF_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraPaladinUndeadDemonDoubleDamageChancePercent", Configuration.CLASSAURA_PALADIN_UNDEAD_DEMON_DOUBLE_DAMAGE_CHANCE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraWarriorTripleAttackChancePercent", Configuration.CLASSAURA_WARRIOR_DOUBLE_TO_TRIPLE_ATTACK_CHANCE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraWizardFocusStacksLostPerMovementEvent", Configuration.CLASSAURA_WIZARD_FOCUS_STACKS_LOST_PER_MOVEMENT_EVENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraWizardFocusMovementIntervalInMS", Configuration.CLASSAURA_WIZARD_FOCUS_MOVEMENT_INTERVAL_IN_MS.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraNecromancerDebuffTransferCooldownInMS", Configuration.CLASSAURA_NECROMANCER_DEBUFF_TRANSFER_COOLDOWN_IN_MS.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraNecromancerMarkDirectDamagePercentPerStack", Configuration.CLASSAURA_NECROMANCER_MARK_DIRECT_DAMAGE_PERCENT_PER_STACK.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraNecromancerMarkDotDamagePercentPerStack", Configuration.CLASSAURA_NECROMANCER_MARK_DOT_DAMAGE_PERCENT_PER_STACK.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraClericCadenceReductionPercent", Configuration.CLASSAURA_CLERIC_CADENCE_REDUCTION_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraDruidDirectHealRegenPercent", Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraDruidDirectHealRegenTickCount", GetDruidRegenTickCount().ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraDruidDamageShieldPercent", Configuration.CLASSAURA_DRUID_DAMAGE_SHIELD_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraDruidImpairedTargetDamagePercent", Configuration.CLASSAURA_DRUID_IMPAIRED_TARGET_DAMAGE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraShamanDotExtendChancePercent", Configuration.CLASSAURA_SHAMAN_DOT_EXTEND_CHANCE_PERCENT.ToString()));
            rows.Add(new KeyValuePair<string, string>("ClassAuraShamanDotExtendInMS", Configuration.CLASSAURA_SHAMAN_DOT_EXTEND_IN_MS.ToString()));
            return rows;
        }

        private static bool IsSpellTypeEnabled(ClassAuraSpellType spellType)
        {
            if (Configuration.CLASSAURA_ENABLED == false)
                return false;
            switch (spellType)
            {
                case ClassAuraSpellType.EnchanterPassive:
                case ClassAuraSpellType.EnchanterAura:
                case ClassAuraSpellType.EnchanterFocus:
                    return Configuration.CLASSAURA_ENCHANTER_ENABLED;
                case ClassAuraSpellType.BardPassive:
                case ClassAuraSpellType.BardAura:
                case ClassAuraSpellType.BardInstrument:
                    return Configuration.CLASSAURA_BARD_ENABLED;
                case ClassAuraSpellType.MonkPassive:
                case ClassAuraSpellType.MonkAura:
                case ClassAuraSpellType.MonkLightArmor:
                case ClassAuraSpellType.MonkHeavyArmor:
                    return Configuration.CLASSAURA_MONK_ENABLED;
                case ClassAuraSpellType.RangerPassive:
                case ClassAuraSpellType.RangerAura:
                case ClassAuraSpellType.RangerSpeed:
                case ClassAuraSpellType.RangerRicochet:
                    return Configuration.CLASSAURA_RANGER_ENABLED;
                case ClassAuraSpellType.RoguePassive:
                case ClassAuraSpellType.RogueAura:
                case ClassAuraSpellType.RogueExploit:
                    return Configuration.CLASSAURA_ROGUE_ENABLED;
                case ClassAuraSpellType.PaladinPassive:
                case ClassAuraSpellType.PaladinAura:
                case ClassAuraSpellType.PaladinHeal:
                    return Configuration.CLASSAURA_PALADIN_ENABLED;
                case ClassAuraSpellType.ShadowKnightPassive:
                case ClassAuraSpellType.ShadowKnightAura:
                case ClassAuraSpellType.ShadowKnightEdge:
                    return Configuration.CLASSAURA_SHADOWKNIGHT_ENABLED;
                case ClassAuraSpellType.WarriorPassive:
                case ClassAuraSpellType.WarriorAura:
                    return Configuration.CLASSAURA_WARRIOR_ENABLED;
                case ClassAuraSpellType.WizardPassive:
                case ClassAuraSpellType.WizardAura:
                case ClassAuraSpellType.WizardFocus:
                    return Configuration.CLASSAURA_WIZARD_ENABLED;
                case ClassAuraSpellType.MagicianPassive:
                case ClassAuraSpellType.MagicianAura:
                case ClassAuraSpellType.MagicianPetPassive:
                case ClassAuraSpellType.MagicianOwnerFocus:
                case ClassAuraSpellType.MagicianPetFury:
                    return Configuration.CLASSAURA_MAGICIAN_ENABLED;
                case ClassAuraSpellType.NecromancerPassive:
                case ClassAuraSpellType.NecromancerAura:
                case ClassAuraSpellType.NecromancerMark:
                    return Configuration.CLASSAURA_NECROMANCER_ENABLED;
                case ClassAuraSpellType.ClericPassive:
                case ClassAuraSpellType.ClericAura:
                case ClassAuraSpellType.ClericCadence:
                case ClassAuraSpellType.ClericHaste:
                    return Configuration.CLASSAURA_CLERIC_ENABLED;
                case ClassAuraSpellType.DruidPassive:
                case ClassAuraSpellType.DruidAura:
                case ClassAuraSpellType.DruidRegrowth:
                    return Configuration.CLASSAURA_DRUID_ENABLED;
                case ClassAuraSpellType.ShamanPassive:
                case ClassAuraSpellType.ShamanAura:
                case ClassAuraSpellType.ShamanSlowMark:
                case ClassAuraSpellType.ShamanVigor:
                    return Configuration.CLASSAURA_SHAMAN_ENABLED;
                case ClassAuraSpellType.CastSpeedHelper:
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

        private static string Pct(int value)
        {
            return string.Concat(value.ToString(), "%");
        }

        private static string Seconds(int durationInMS)
        {
            return string.Concat((durationInMS / 1000).ToString(), " seconds");
        }

        private static SpellTemplate BuildBaseTemplate(string name, ClassAuraSpellType spellType, int spellIconEQID, string description, string auraDescription)
        {
            SpellTemplate spellTemplate = new SpellTemplate();
            spellTemplate.Name = name;
            spellTemplate.WOWSpellID = GetSpellID(spellType);
            spellTemplate.EQSpellID = SpellTemplate.GenerateUniqueEQSpellID();
            spellTemplate.Description = description;
            spellTemplate.AuraDescription = auraDescription;
            spellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(GetValidatedSpellIconID(spellIconEQID, name));
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

        private static SpellTemplate BuildPassiveTemplate(string name, ClassAuraSpellType spellType, int spellIconEQID, string description)
        {
            SpellTemplate spellTemplate = BuildBaseTemplate(name, spellType, spellIconEQID, description, description);
            spellTemplate.ForceHiddenFromDisplay = true;
            spellTemplate.AuraDuration.IsInfinite = true;
            spellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            spellTemplate.IsPassiveAbility = true;
            spellTemplate.AlwaysPersist = true;
            return spellTemplate;
        }

        private static SpellTemplate BuildPermanentAuraTemplate(string name, ClassAuraSpellType spellType, int spellIconEQID, string description, List<SpellEffectWOW> effects)
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

        private static SpellTemplate BuildStackingAuraTemplate(string name, ClassAuraSpellType spellType, int spellIconEQID, string description, List<SpellEffectWOW> effects,
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
                Seconds(Configuration.CLASSAURA_ENCHANTER_MANA_REGEN_INTERVAL_IN_MS), ". Spell damage is increased by ", Pct(Configuration.CLASSAURA_ENCHANTER_FOCUS_SPELL_DAMAGE_PERCENT),
                " while mana is at ", Pct(Configuration.CLASSAURA_ENCHANTER_FOCUS_MANA_THRESHOLD_PERCENT), " or more.");
            spellTemplates.Add(BuildPassiveTemplate("Mind of Clarity", ClassAuraSpellType.EnchanterPassive, icon, description));

            List<SpellEffectWOW> auraEffects = new List<SpellEffectWOW>();
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.ObsModPower, Configuration.CLASSAURA_ENCHANTER_MANA_REGEN_PERCENT, POWER_MANA, SpellWOWTargetType.UnitCaster,
                Convert.ToUInt32(Math.Max(1000, Configuration.CLASSAURA_ENCHANTER_MANA_REGEN_INTERVAL_IN_MS))));
            spellTemplates.Add(BuildPermanentAuraTemplate("Mind of Clarity (Enchanter)", ClassAuraSpellType.EnchanterAura, icon, description, auraEffects));

            string focusDescription = string.Concat("Spell damage increased by ", Pct(Configuration.CLASSAURA_ENCHANTER_FOCUS_SPELL_DAMAGE_PERCENT), " while mana stays at ",
                Pct(Configuration.CLASSAURA_ENCHANTER_FOCUS_MANA_THRESHOLD_PERCENT), " or more.");
            List<SpellEffectWOW> focusEffects = new List<SpellEffectWOW>();
            focusEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentDone, Configuration.CLASSAURA_ENCHANTER_FOCUS_SPELL_DAMAGE_PERCENT, SCHOOL_MASK_MAGIC, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildPermanentAuraTemplate("Clarity of Thought", ClassAuraSpellType.EnchanterFocus, icon, focusDescription, focusEffects));
        }

        private static void AddBardSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_BARD_SPELL_ICON_EQ_ID;
            string description = string.Concat("Immune to snares, roots, and dazes. Melee autoattacks deal ", Pct(Configuration.CLASSAURA_BARD_INSTRUMENT_MELEE_AUTOATTACK_DAMAGE_PERCENT),
                " more damage while holding a weapon in one hand and an instrument in the other.");
            spellTemplates.Add(BuildPassiveTemplate("Dexteritous Troubadour", ClassAuraSpellType.BardPassive, icon, description));

            List<SpellEffectWOW> auraEffects = new List<SpellEffectWOW>();
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.MechanicImmunity, 0, MECHANIC_SNARE, SpellWOWTargetType.UnitCaster));
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.MechanicImmunity, 0, MECHANIC_ROOT, SpellWOWTargetType.UnitCaster));
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.MechanicImmunity, 0, MECHANIC_DAZE, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildPermanentAuraTemplate("Dexteritous Troubadour (Bard)", ClassAuraSpellType.BardAura, icon, description, auraEffects));

            // Only a marker: the mod applies the bonus in its melee swing hook since a damage percent aura would also raise melee abilities and ranged shots
            string instrumentDescription = string.Concat("Melee autoattack damage increased by ", Pct(Configuration.CLASSAURA_BARD_INSTRUMENT_MELEE_AUTOATTACK_DAMAGE_PERCENT), " while holding a weapon and an instrument.");
            List<SpellEffectWOW> instrumentEffects = new List<SpellEffectWOW>();
            instrumentEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildPermanentAuraTemplate("Troubadour's Tempo", ClassAuraSpellType.BardInstrument, icon, instrumentDescription, instrumentEffects));
        }

        private static void AddMonkSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_MONK_SPELL_ICON_EQ_ID;
            string description = string.Concat("In cloth or leather with no shield: ", Pct(Configuration.CLASSAURA_MONK_LIGHT_ARMOR_HASTE_PERCENT), " haste and ",
                Pct(Configuration.CLASSAURA_MONK_LIGHT_ARMOR_DODGE_PERCENT), " dodge. In heavier armor or with a shield: ", Pct(Configuration.CLASSAURA_MONK_HEAVY_ARMOR_HASTE_PERCENT),
                " haste. This haste ignores the haste cap. Direct heals cast on yourself finish ", Pct(Configuration.CLASSAURA_MONK_SELF_HEAL_CAST_TIME_REDUCTION_PERCENT), " faster.");
            spellTemplates.Add(BuildPassiveTemplate("Agile Fighter", ClassAuraSpellType.MonkPassive, icon, description));
            spellTemplates.Add(BuildPermanentAuraTemplate("Agile Fighter (Monk)", ClassAuraSpellType.MonkAura, icon, description, new List<SpellEffectWOW>()));

            // Melee + ranged haste are on one effect (192) that the EQ haste cap tracker never watches, and spell haste is the casting speed aura
            string lightDescription = string.Concat("Haste increased by ", Pct(Configuration.CLASSAURA_MONK_LIGHT_ARMOR_HASTE_PERCENT), " and dodge by ",
                Pct(Configuration.CLASSAURA_MONK_LIGHT_ARMOR_DODGE_PERCENT), " while wearing cloth or leather without a shield.");
            List<SpellEffectWOW> lightEffects = new List<SpellEffectWOW>();
            lightEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModMeleeRangedHaste, Configuration.CLASSAURA_MONK_LIGHT_ARMOR_HASTE_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            lightEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModCastingSpeedNotStack, Configuration.CLASSAURA_MONK_LIGHT_ARMOR_HASTE_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            lightEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDodgePercent, Configuration.CLASSAURA_MONK_LIGHT_ARMOR_DODGE_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildPermanentAuraTemplate("Unburdened Agility", ClassAuraSpellType.MonkLightArmor, icon, lightDescription, lightEffects));

            string heavyDescription = string.Concat("Haste increased by ", Pct(Configuration.CLASSAURA_MONK_HEAVY_ARMOR_HASTE_PERCENT), " while wearing mail, plate, or a shield.");
            List<SpellEffectWOW> heavyEffects = new List<SpellEffectWOW>();
            heavyEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModMeleeRangedHaste, Configuration.CLASSAURA_MONK_HEAVY_ARMOR_HASTE_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            heavyEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModCastingSpeedNotStack, Configuration.CLASSAURA_MONK_HEAVY_ARMOR_HASTE_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildPermanentAuraTemplate("Burdened Agility", ClassAuraSpellType.MonkHeavyArmor, icon, heavyDescription, heavyEffects));
        }

        private static void AddRangerSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_RANGER_SPELL_ICON_EQ_ID;
            string description = string.Concat("Each of your melee attacks and Auto Shots raises your movement speed by ", Pct(Configuration.CLASSAURA_RANGER_ATTACK_SPEED_PERCENT_PER_STACK), " for ",
                Seconds(Configuration.CLASSAURA_RANGER_ATTACK_SPEED_DURATION_IN_MS), ", stacking up to ", Configuration.CLASSAURA_RANGER_ATTACK_SPEED_MAX_STACKS.ToString(),
                " times and stacking with other speed effects. Auto Shot has a ", Pct(Configuration.CLASSAURA_RANGER_RICOCHET_CHANCE_PERCENT), " chance to ricochet to another target within ",
                Configuration.CLASSAURA_RANGER_RICOCHET_RANGE.ToString(), " yards, causing no threat.");
            spellTemplates.Add(BuildPassiveTemplate("Swift Reactions", ClassAuraSpellType.RangerPassive, icon, description));
        
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Swift Reactions (Ranger)", ClassAuraSpellType.RangerAura, icon, description, new List<SpellEffectWOW>());
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraRangerAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_MELEE_AUTO_ATTACK | PROC_FLAG_DONE_RANGED_AUTO_ATTACK, PROC_SPELL_TYPE_DAMAGE, PROC_SPELL_PHASE_HIT,
                PROC_HIT_NORMAL | PROC_HIT_CRITICAL, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            // ModSpeedAlways multiplies on top of every other movement speed source instead of competing with the highest one
            string speedDescription = string.Concat("Movement speed increased by ", Pct(Configuration.CLASSAURA_RANGER_ATTACK_SPEED_PERCENT_PER_STACK), " per stack.");
            List<SpellEffectWOW> speedEffects = new List<SpellEffectWOW>();
            speedEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModSpeedAlways, Configuration.CLASSAURA_RANGER_ATTACK_SPEED_PERCENT_PER_STACK, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildStackingAuraTemplate("Quickened Stride", ClassAuraSpellType.RangerSpeed, icon, speedDescription, speedEffects,
                Configuration.CLASSAURA_RANGER_ATTACK_SPEED_MAX_STACKS, Configuration.CLASSAURA_RANGER_ATTACK_SPEED_DURATION_IN_MS, false));

            // The ricochet damage is delivered by the mod with this spell as the combat log source, so only its threat and school matter here
            SpellTemplate ricochetSpellTemplate = BuildBaseTemplate("Ricochet", ClassAuraSpellType.RangerRicochet, icon, "A shot that bounced off its target into another.", string.Empty);
            SpellEffectWOW ricochetEffect = new SpellEffectWOW(SpellWOWEffectType.SchoolDamage, SpellWOWAuraType.None, 0, 0, 0, 0, 0, 0);
            ricochetEffect.ImplicitTargetA = SpellWOWTargetType.UnitTargetEnemy;
            ricochetSpellTemplate.WOWSpellEffects.Add(ricochetEffect);
            ricochetSpellTemplate.SpellRange = 45;
            ricochetSpellTemplate.GenerateNoThreat = true;
            ricochetSpellTemplate.NeverMisses = true;
            ricochetSpellTemplate.CannotCrit = true;
            ricochetSpellTemplate.IgnoreLineOfSight = true;
            spellTemplates.Add(ricochetSpellTemplate);
        }

        private static void AddRogueSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_ROGUE_SPELL_ICON_EQ_ID;
            string description = string.Concat("Critical strike chance increased by ", Pct(Configuration.CLASSAURA_ROGUE_CRITICAL_STRIKE_PERCENT), ". Each landed attack raises all damage dealt by ",
                Pct(Configuration.CLASSAURA_ROGUE_EXPLOIT_DAMAGE_PERCENT_PER_STACK), " for ", Seconds(Configuration.CLASSAURA_ROGUE_EXPLOIT_DURATION_IN_MS), ", stacking up to ",
                Configuration.CLASSAURA_ROGUE_EXPLOIT_MAX_STACKS.ToString(), " times. A miss, dodge, or parry removes every stack.");
            spellTemplates.Add(BuildPassiveTemplate("Master Exploiter", ClassAuraSpellType.RoguePassive, icon, description));

            // ModCritPct feeds melee, ranged and spell crit alike.  The proc row fires the script on every attack outcome it needs to see
            List<SpellEffectWOW> auraEffects = new List<SpellEffectWOW>();
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModCritPct, Configuration.CLASSAURA_ROGUE_CRITICAL_STRIKE_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Master Exploiter (Rogue)", ClassAuraSpellType.RogueAura, icon, description, auraEffects);
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraRogueAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_MELEE_AUTO_ATTACK, 0, 0,
                PROC_HIT_NORMAL | PROC_HIT_CRITICAL | PROC_HIT_MISS | PROC_HIT_DODGE | PROC_HIT_PARRY | PROC_HIT_BLOCK | PROC_HIT_ABSORB, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            string exploitDescription = string.Concat("Damage dealt increased by ", Pct(Configuration.CLASSAURA_ROGUE_EXPLOIT_DAMAGE_PERCENT_PER_STACK), " per stack. Lost on a miss, dodge, or parry.");
            List<SpellEffectWOW> exploitEffects = new List<SpellEffectWOW>();
            exploitEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentDone, Configuration.CLASSAURA_ROGUE_EXPLOIT_DAMAGE_PERCENT_PER_STACK, SCHOOL_MASK_ALL, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildStackingAuraTemplate("Exploited Opening", ClassAuraSpellType.RogueExploit, icon, exploitDescription, exploitEffects,
                Configuration.CLASSAURA_ROGUE_EXPLOIT_MAX_STACKS, Configuration.CLASSAURA_ROGUE_EXPLOIT_DURATION_IN_MS, false));
        }

        private static void AddPaladinSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_PALADIN_SPELL_ICON_EQ_ID;
            string description = string.Concat("Block chance increased by ", Pct(Configuration.CLASSAURA_PALADIN_BLOCK_PERCENT), ". Your heals also heal you for ",
                Pct(Configuration.CLASSAURA_PALADIN_HEAL_SELF_PERCENT), " of the amount. Attacks against undead and demons have a ",
                Pct(Configuration.CLASSAURA_PALADIN_UNDEAD_DEMON_DOUBLE_DAMAGE_CHANCE_PERCENT), " chance to deal double damage.");
            spellTemplates.Add(BuildPassiveTemplate("Champion of Light", ClassAuraSpellType.PaladinPassive, icon, description));

            // Proc on any direct or periodic heal the paladin lands
            List<SpellEffectWOW> auraEffects = new List<SpellEffectWOW>();
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModBlockPercent, Configuration.CLASSAURA_PALADIN_BLOCK_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Champion of Light (Paladin)", ClassAuraSpellType.PaladinAura, icon, description, auraEffects);
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraPaladinAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_POS | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_POS | 0x00040000, // 0x00040000 = PROC_FLAG_DONE_PERIODIC
                PROC_SPELL_TYPE_HEAL, PROC_SPELL_PHASE_HIT, 0, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            SpellTemplate healSpellTemplate = BuildBaseTemplate("Light's Reward", ClassAuraSpellType.PaladinHeal, icon, "Healed by the light you gave to another.", string.Empty);
            SpellEffectWOW healEffect = new SpellEffectWOW(SpellWOWEffectType.Heal, SpellWOWAuraType.None, 0, 0, 0, 0, 0, 0);
            healEffect.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
            healSpellTemplate.WOWSpellEffects.Add(healEffect);
            healSpellTemplate.GenerateNoThreat = true;
            healSpellTemplate.CannotCrit = true;
            healSpellTemplate.InfluencedBySpellPower = false;
            spellTemplates.Add(healSpellTemplate);
        }

        private static void AddShadowKnightSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_SHADOWKNIGHT_SPELL_ICON_EQ_ID;
            string description = string.Concat("Parry chance increased by ", Pct(Configuration.CLASSAURA_SHADOWKNIGHT_PARRY_PERCENT),
                ". Attack critical strikes make your next harmful spell within ", Seconds(Configuration.CLASSAURA_SHADOWKNIGHT_INSTANT_CAST_DURATION_IN_MS),
                " instant. Cannot occur more than once every ", Seconds(Configuration.CLASSAURA_SHADOWKNIGHT_INSTANT_CAST_COOLDOWN_IN_MS), ".");
            spellTemplates.Add(BuildPassiveTemplate("Spellsword", ClassAuraSpellType.ShadowKnightPassive, icon, description));

            // The proc row's cooldown is the once-every-N-seconds rule, enforced by the core
            List<SpellEffectWOW> auraEffects = new List<SpellEffectWOW>();
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModParryPercent, Configuration.CLASSAURA_SHADOWKNIGHT_PARRY_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Spellsword (Shadow Knight)", ClassAuraSpellType.ShadowKnightAura, icon, description, auraEffects);
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraShadowKnightAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_MELEE_AUTO_ATTACK, 0, 0, PROC_HIT_CRITICAL, Configuration.CLASSAURA_SHADOWKNIGHT_INSTANT_CAST_COOLDOWN_IN_MS, 0);
            spellTemplates.Add(auraSpellTemplate);

            List<SpellEffectWOW> edgeEffects = new List<SpellEffectWOW>();
            edgeEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildStackingAuraTemplate("Spellsword's Edge", ClassAuraSpellType.ShadowKnightEdge, icon, "The next harmful spell cast is instant.", edgeEffects,
                1, Configuration.CLASSAURA_SHADOWKNIGHT_INSTANT_CAST_DURATION_IN_MS, false));
        }

        private static void AddWarriorSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_WARRIOR_SPELL_ICON_EQ_ID;
            string description = string.Concat("Maximum health increased by ", Pct(Configuration.CLASSAURA_WARRIOR_MAX_HEALTH_PERCENT), " and dodge by ",
                Pct(Configuration.CLASSAURA_WARRIOR_DODGE_PERCENT), ". Attacks have a ", Pct(Configuration.CLASSAURA_WARRIOR_DOUBLE_ATTACK_CHANCE_PERCENT),
                " chance to strike twice, and ", Pct(Configuration.CLASSAURA_WARRIOR_DOUBLE_TO_TRIPLE_ATTACK_CHANCE_PERCENT), " of those strike a third time.");
            spellTemplates.Add(BuildPassiveTemplate("Warmaster", ClassAuraSpellType.WarriorPassive, icon, description));

            // The double attack roll is the proc row's chance, and the script decides whether it becomes a triple
            List<SpellEffectWOW> auraEffects = new List<SpellEffectWOW>();
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModIncreaseHealthPercent, Configuration.CLASSAURA_WARRIOR_MAX_HEALTH_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            auraEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDodgePercent, Configuration.CLASSAURA_WARRIOR_DODGE_PERCENT, 0, SpellWOWTargetType.UnitCaster));
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Warmaster (Warrior)", ClassAuraSpellType.WarriorAura, icon, description, auraEffects);
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraWarriorAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_MELEE_AUTO_ATTACK, 0, 0, 0, 0, Configuration.CLASSAURA_WARRIOR_DOUBLE_ATTACK_CHANCE_PERCENT);
            spellTemplates.Add(auraSpellTemplate);
        }

        private static void AddWizardSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_WIZARD_SPELL_ICON_EQ_ID;
            string description = string.Concat("Casting while moving carries no movement speed penalty. Each spell cast raises spell damage by ",
                Pct(Configuration.CLASSAURA_WIZARD_FOCUS_SPELL_DAMAGE_PERCENT_PER_STACK), " for ", Seconds(Configuration.CLASSAURA_WIZARD_FOCUS_DURATION_IN_MS), ", stacking up to ",
                Configuration.CLASSAURA_WIZARD_FOCUS_MAX_STACKS.ToString(), " times. Starting to move removes ", GetStackWord(Configuration.CLASSAURA_WIZARD_FOCUS_STACKS_LOST_PER_MOVEMENT_EVENT),
                ", and so does every ", (Configuration.CLASSAURA_WIZARD_FOCUS_MOVEMENT_INTERVAL_IN_MS / 1000).ToString(), " second", Configuration.CLASSAURA_WIZARD_FOCUS_MOVEMENT_INTERVAL_IN_MS >= 2000 ? "s" : "",
                " spent moving.");
            spellTemplates.Add(BuildPassiveTemplate("Unshaken Channeler", ClassAuraSpellType.WizardPassive, icon, description));
            spellTemplates.Add(BuildPermanentAuraTemplate("Unshaken Channeler (Wizard)", ClassAuraSpellType.WizardAura, icon, description, new List<SpellEffectWOW>()));

            string focusDescription = string.Concat("Spell damage increased by ", Pct(Configuration.CLASSAURA_WIZARD_FOCUS_SPELL_DAMAGE_PERCENT_PER_STACK), " per stack. Movement removes stacks.");
            List<SpellEffectWOW> focusEffects = new List<SpellEffectWOW>();
            focusEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentDone, Configuration.CLASSAURA_WIZARD_FOCUS_SPELL_DAMAGE_PERCENT_PER_STACK, SCHOOL_MASK_MAGIC, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildStackingAuraTemplate("Channeler's Focus", ClassAuraSpellType.WizardFocus, icon, focusDescription, focusEffects,
                Configuration.CLASSAURA_WIZARD_FOCUS_MAX_STACKS, Configuration.CLASSAURA_WIZARD_FOCUS_DURATION_IN_MS, false));
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
            spellTemplates.Add(BuildPassiveTemplate("Bound Conjurer", ClassAuraSpellType.MagicianPassive, icon, description));

            // Owner side: procs on the owner's own damaging spell crits
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Bound Conjurer (Magician)", ClassAuraSpellType.MagicianAura, icon, description, new List<SpellEffectWOW>());
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraMagicianAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_NEG | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_NEG, PROC_SPELL_TYPE_DAMAGE, PROC_SPELL_PHASE_HIT,
                PROC_HIT_CRITICAL, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            // Pet side: a hidden passive the mod puts on the pet, procs on the pet's landed melee swings and spell hits
            SpellTemplate petPassiveSpellTemplate = BuildBaseTemplate("Conjurer's Bond", ClassAuraSpellType.MagicianPetPassive, icon, "Strikes empower the pet's master.", "Strikes empower the pet's master.");
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
            spellTemplates.Add(BuildStackingAuraTemplate("Conjurer's Insight", ClassAuraSpellType.MagicianOwnerFocus, icon, ownerFocusDescription, ownerFocusEffects,
                Configuration.CLASSAURA_MAGICIAN_PET_STRIKE_MAX_STACKS, Configuration.CLASSAURA_MAGICIAN_PET_STRIKE_DURATION_IN_MS, false));

            string petFuryDescription = string.Concat("Damage dealt increased by ", Pct(Configuration.CLASSAURA_MAGICIAN_OWNER_CRIT_PET_DAMAGE_PERCENT_PER_STACK), " per stack.");
            List<SpellEffectWOW> petFuryEffects = new List<SpellEffectWOW>();
            petFuryEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentDone, Configuration.CLASSAURA_MAGICIAN_OWNER_CRIT_PET_DAMAGE_PERCENT_PER_STACK, SCHOOL_MASK_ALL, SpellWOWTargetType.UnitTargetAlly));
            spellTemplates.Add(BuildStackingAuraTemplate("Conjurer's Fury", ClassAuraSpellType.MagicianPetFury, icon, petFuryDescription, petFuryEffects,
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
            spellTemplates.Add(BuildPassiveTemplate("Grave Pact", ClassAuraSpellType.NecromancerPassive, icon, description));
            spellTemplates.Add(BuildPermanentAuraTemplate("Grave Pact (Necromancer)", ClassAuraSpellType.NecromancerAura, icon, description, new List<SpellEffectWOW>()));

            // The mark is a counter the mod reads in its damage hooks, so it carries no effect of its own
            string markDescription = string.Concat("Takes ", Pct(Configuration.CLASSAURA_NECROMANCER_MARK_DIRECT_DAMAGE_PERCENT_PER_STACK), " more direct spell damage and ",
                Pct(Configuration.CLASSAURA_NECROMANCER_MARK_DOT_DAMAGE_PERCENT_PER_STACK), " more damage over time from the necromancer per mark.");
            List<SpellEffectWOW> markEffects = new List<SpellEffectWOW>();
            markEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitTargetEnemy));
            spellTemplates.Add(BuildStackingAuraTemplate("Grave Mark", ClassAuraSpellType.NecromancerMark, icon, markDescription, markEffects,
                Configuration.CLASSAURA_NECROMANCER_MARK_MAX_STACKS, Configuration.CLASSAURA_NECROMANCER_MARK_DURATION_IN_MS, true));
        }

        private static void AddClericSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_CLERIC_SPELL_ICON_EQ_ID;
            string description = string.Concat("Each area heal you cast readies a Sacred Rhythm charge for ", Seconds(Configuration.CLASSAURA_CLERIC_CADENCE_DURATION_IN_MS), ", up to ",
                Configuration.CLASSAURA_CLERIC_CADENCE_MAX_STACKS.ToString(), ". A charge is spent to cut the cast time and mana cost of your next single target heal by ",
                Pct(Configuration.CLASSAURA_CLERIC_CADENCE_REDUCTION_PERCENT), ". Complete Healing cannot use a charge. Single target heals also grant the target ",
                Pct(Configuration.CLASSAURA_CLERIC_HEAL_HASTE_PERCENT_PER_STACK), " haste for ", Seconds(Configuration.CLASSAURA_CLERIC_HEAL_HASTE_DURATION_IN_MS), ", stacking up to ",
                Configuration.CLASSAURA_CLERIC_HEAL_HASTE_MAX_STACKS.ToString(), " times and ignoring the haste cap.");
            spellTemplates.Add(BuildPassiveTemplate("Sacred Cadence", ClassAuraSpellType.ClericPassive, icon, description));
            spellTemplates.Add(BuildPermanentAuraTemplate("Sacred Cadence (Cleric)", ClassAuraSpellType.ClericAura, icon, description, new List<SpellEffectWOW>()));

            string cadenceDescription = string.Concat("Each charge cuts the cast time and mana cost of the next single target heal by ", Pct(Configuration.CLASSAURA_CLERIC_CADENCE_REDUCTION_PERCENT), ".");
            List<SpellEffectWOW> cadenceEffects = new List<SpellEffectWOW>();
            cadenceEffects.Add(BuildAuraEffect(SpellWOWAuraType.Dummy, 0, 0, SpellWOWTargetType.UnitCaster));
            spellTemplates.Add(BuildStackingAuraTemplate("Sacred Rhythm", ClassAuraSpellType.ClericCadence, icon, cadenceDescription, cadenceEffects,
                Configuration.CLASSAURA_CLERIC_CADENCE_MAX_STACKS, Configuration.CLASSAURA_CLERIC_CADENCE_DURATION_IN_MS, false));

            // Stacks pool across every cleric healing the target (one shared aura), so the cap holds no matter how many clerics are healing
            string hasteDescription = string.Concat("Haste increased by ", Pct(Configuration.CLASSAURA_CLERIC_HEAL_HASTE_PERCENT_PER_STACK), " per stack.");
            List<SpellEffectWOW> hasteEffects = new List<SpellEffectWOW>();
            hasteEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModMeleeRangedHaste, Configuration.CLASSAURA_CLERIC_HEAL_HASTE_PERCENT_PER_STACK, 0, SpellWOWTargetType.UnitTargetAlly));
            hasteEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModCastingSpeedNotStack, Configuration.CLASSAURA_CLERIC_HEAL_HASTE_PERCENT_PER_STACK, 0, SpellWOWTargetType.UnitTargetAlly));
            spellTemplates.Add(BuildStackingAuraTemplate("Hastened Faith", ClassAuraSpellType.ClericHaste, icon, hasteDescription, hasteEffects,
                Configuration.CLASSAURA_CLERIC_HEAL_HASTE_MAX_STACKS, Configuration.CLASSAURA_CLERIC_HEAL_HASTE_DURATION_IN_MS, false));
        }

        private static void AddDruidSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_DRUID_SPELL_ICON_EQ_ID;
            string description = string.Concat("Your direct heals leave a regeneration that heals ", Pct(Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_PERCENT), " of the amount over ",
                Seconds(Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_DURATION_IN_MS), ". Your damage shields deal ", Pct(Configuration.CLASSAURA_DRUID_DAMAGE_SHIELD_PERCENT),
                " more damage. Your direct damage spells deal ", Pct(Configuration.CLASSAURA_DRUID_IMPAIRED_TARGET_DAMAGE_PERCENT),
                " more to targets that are snared, rooted, or suffering from your damage over time spells.");
            spellTemplates.Add(BuildPassiveTemplate("Skin of the Wild", ClassAuraSpellType.DruidPassive, icon, description));

            // Proc on the druid's direct heals (periodic heals are deliberately not in the flags), which is what leaves the regeneration behind
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Skin of the Wild (Druid)", ClassAuraSpellType.DruidAura, icon, description, new List<SpellEffectWOW>());
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraDruidAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_POS | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_POS, PROC_SPELL_TYPE_HEAL, PROC_SPELL_PHASE_HIT, 0, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            // Per-tick amount is handed in by the mod at cast time.  Unlike every other class aura effect, this one stacks per druid (the mod leaves it out of its one-copy-per-target rule), so three druids can each have their own regrowth on the same target
            SpellTemplate regrowthSpellTemplate = BuildBaseTemplate("Wild Regrowth", ClassAuraSpellType.DruidRegrowth, icon,
                string.Concat("Regenerating health over ", Seconds(Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_DURATION_IN_MS), "."),
                string.Concat("Regenerating health over ", Seconds(Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_DURATION_IN_MS), "."));
            regrowthSpellTemplate.AuraDuration.SetFixedDuration(Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_DURATION_IN_MS);
            regrowthSpellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.PeriodicHeal, 0, 0, SpellWOWTargetType.UnitTargetAlly,
                Convert.ToUInt32(Math.Max(1000, Configuration.CLASSAURA_DRUID_DIRECT_HEAL_REGEN_TICK_INTERVAL_IN_MS))));
            regrowthSpellTemplate.InfluencedBySpellPower = false;
            regrowthSpellTemplate.CannotCrit = true;
            regrowthSpellTemplate.GenerateNoThreat = true;
            spellTemplates.Add(regrowthSpellTemplate);
        }

        private static void AddShamanSpells(List<SpellTemplate> spellTemplates)
        {
            int icon = Configuration.CLASSAURA_SHAMAN_SPELL_ICON_EQ_ID;
            string description = string.Concat("Targets you slow take ", Pct(Configuration.CLASSAURA_SHAMAN_SLOWED_DAMAGE_TAKEN_PERCENT), " more damage from all sources and deal ",
                Pct(Configuration.CLASSAURA_SHAMAN_SLOWED_DAMAGE_DONE_REDUCTION_PERCENT), " less. Your autoattacks have a ", Pct(Configuration.CLASSAURA_SHAMAN_DOT_EXTEND_CHANCE_PERCENT),
                " chance to extend any of your damage over time effects on the target by ", Seconds(Configuration.CLASSAURA_SHAMAN_DOT_EXTEND_IN_MS),
                ". Healing an ally grants them ", Pct(Configuration.CLASSAURA_SHAMAN_HEAL_STAT_PERCENT_PER_STACK), " increased strength, agility, stamina, intellect, and spirit for ",
                Seconds(Configuration.CLASSAURA_SHAMAN_HEAL_STAT_DURATION_IN_MS), ", stacking up to ", Configuration.CLASSAURA_SHAMAN_HEAL_STAT_MAX_STACKS.ToString(), " times.");
            spellTemplates.Add(BuildPassiveTemplate("Spirit Channeler", ClassAuraSpellType.ShamanPassive, icon, description));

            // Proc on any heal the shaman lands, which is what grants the vigor
            SpellTemplate auraSpellTemplate = BuildPermanentAuraTemplate("Spirit Channeler (Shaman)", ClassAuraSpellType.ShamanAura, icon, description, new List<SpellEffectWOW>());
            auraSpellTemplate.AttachedAuraScriptName = "EverQuest_ClassAuraShamanAuraScript";
            auraSpellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_SPELL_MAGIC_DMG_CLASS_POS | PROC_FLAG_DONE_SPELL_NONE_DMG_CLASS_POS | 0x00040000, // 0x00040000 = PROC_FLAG_DONE_PERIODIC
                PROC_SPELL_TYPE_HEAL, PROC_SPELL_PHASE_HIT, 0, 0, 0);
            spellTemplates.Add(auraSpellTemplate);

            // One shared copy per target however many shamans slow it, living as long as the longest qualifying slow (the mod manages the duration)
            string slowMarkDescription = string.Concat("Takes ", Pct(Configuration.CLASSAURA_SHAMAN_SLOWED_DAMAGE_TAKEN_PERCENT), " more damage from all sources and deals ",
                Pct(Configuration.CLASSAURA_SHAMAN_SLOWED_DAMAGE_DONE_REDUCTION_PERCENT), " less.");
            List<SpellEffectWOW> slowMarkEffects = new List<SpellEffectWOW>();
            slowMarkEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentTaken, Configuration.CLASSAURA_SHAMAN_SLOWED_DAMAGE_TAKEN_PERCENT, SCHOOL_MASK_ALL, SpellWOWTargetType.UnitTargetEnemy));
            slowMarkEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModDamagePercentDone, -Configuration.CLASSAURA_SHAMAN_SLOWED_DAMAGE_DONE_REDUCTION_PERCENT, SCHOOL_MASK_ALL, SpellWOWTargetType.UnitTargetEnemy));
            spellTemplates.Add(BuildStackingAuraTemplate("Spirit's Burden", ClassAuraSpellType.ShamanSlowMark, icon, slowMarkDescription, slowMarkEffects, 1, 3600000, true));

            // Stacks pool across every shaman healing the target (one shared aura), so the cap holds no matter how many shamans are healing
            string vigorDescription = string.Concat("Strength, agility, stamina, intellect, and spirit increased by ", Pct(Configuration.CLASSAURA_SHAMAN_HEAL_STAT_PERCENT_PER_STACK), " per stack.");
            List<SpellEffectWOW> vigorEffects = new List<SpellEffectWOW>();
            vigorEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModTotalStatPercentage, Configuration.CLASSAURA_SHAMAN_HEAL_STAT_PERCENT_PER_STACK, STAT_ALL, SpellWOWTargetType.UnitTargetAlly));
            spellTemplates.Add(BuildStackingAuraTemplate("Spirit's Vigor", ClassAuraSpellType.ShamanVigor, icon, vigorDescription, vigorEffects,
                Configuration.CLASSAURA_SHAMAN_HEAL_STAT_MAX_STACKS, Configuration.CLASSAURA_SHAMAN_HEAL_STAT_DURATION_IN_MS, false));
        }

        private static void AddCastSpeedHelperSpell(List<SpellTemplate> spellTemplates)
        {
            SpellTemplate helperSpellTemplate = BuildBaseTemplate("Cast Adjustment", ClassAuraSpellType.CastSpeedHelper, 0, "Adjusts the cast in progress.", "Adjusts the cast in progress.");
            helperSpellTemplate.AuraDuration.SetFixedDuration(CAST_SPEED_HELPER_DURATION_IN_MS);
            helperSpellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.ModCastingSpeedNotStack, 0, 0, SpellWOWTargetType.UnitCaster));
            helperSpellTemplate.WOWSpellEffects.Add(BuildAuraEffect(SpellWOWAuraType.AddPctModifier, 0, 14, SpellWOWTargetType.UnitCaster)); // 14 = SPELLMOD_COST
            helperSpellTemplate.ForceHiddenFromDisplay = true;
            helperSpellTemplate.PreventAuraClickOff = true;
            spellTemplates.Add(helperSpellTemplate);
        }
    }
}
