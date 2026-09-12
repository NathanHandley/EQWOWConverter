//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

using EQWOWConverter.Creatures;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Spells
{
    internal static class SpellPetAbility
    {
        private const int SCHOOL_MASK_ALL = 127;
        private const int SCHOOL_MASK_MAGIC = 126; // Every school but physical, so melee swings and Bash are left alone

        // Proc masks (AzerothCore SpellMgr.h), matching what the class aura pet passive uses
        private const int PROC_FLAG_DONE_MELEE_AUTO_ATTACK = 0x00000004;
        private const int PROC_FLAG_DONE_SPELL_MELEE_DMG_CLASS = 0x00000010;
        private const int PROC_SPELL_TYPE_DAMAGE = 0x1;
        private const int PROC_SPELL_PHASE_HIT = 0x2;
        private const int PROC_HIT_NORMAL = 0x1;
        private const int PROC_HIT_CRITICAL = 0x2;

        private static Dictionary<int, int> AttackProcPassiveSpellIDsByPetWOWCreatureTemplateID = new Dictionary<int, int>();

        public static void AddSpellTemplates(List<SpellTemplate> spellTemplates)
        {
            List<SpellPetType> allPetTypes = SpellPetType.GetAllSpellPetTypes();

            if (Configuration.SPELL_PET_AVOIDANCE_ENABLED == true)
            {
                SpellTemplate avoidanceSpellTemplate = BuildAvoidanceSpellTemplate();
                ApplySkillLines(avoidanceSpellTemplate, allPetTypes);
                spellTemplates.Add(avoidanceSpellTemplate);
            }

            List<SpellPetType> frenzyPetTypes = allPetTypes.Where(petType => petType.HasFrenzy == true).ToList();
            if (Configuration.SPELL_PET_FRENZY_ENABLED == true && frenzyPetTypes.Count > 0)
            {
                spellTemplates.Add(BuildFrenzyBuffSpellTemplate());
                SpellTemplate frenzySpellTemplate = BuildFrenzySpellTemplate();
                ApplySkillLines(frenzySpellTemplate, frenzyPetTypes);
                spellTemplates.Add(frenzySpellTemplate);
            }

            if (Configuration.SPELL_PET_SPELL_DAMAGE_MULTIPLIER_ENABLED == true)
            {
                foreach (SpellPetType petType in allPetTypes)
                {
                    if (petType.GetSpellDamageBonusPercent() == 0)
                        continue;
                    spellTemplates.Add(BuildSpellDamageSpellTemplate(petType));
                }
            }

            if (Configuration.SPELL_PET_ATTACK_PROC_ENABLED == true)
                AddAttackProcSpellTemplates(spellTemplates);
        }

        private static void AddAttackProcSpellTemplates(List<SpellTemplate> spellTemplates)
        {
            Dictionary<int, SpellTemplate> spellTemplatesByEQID = SpellTemplate.GetSpellTemplatesByEQID();
            Dictionary<int, CreatureTemplate> creatureTemplatesByEQID = CreatureTemplate.GetCreatureTemplateListByEQID();
            Dictionary<int, CreatureSpellList> creatureSpellListsByID = new Dictionary<int, CreatureSpellList>();
            foreach (CreatureSpellList creatureSpellList in CreatureSpellList.GetCreatureSpellLists())
                creatureSpellListsByID[creatureSpellList.ID] = creatureSpellList;

            Dictionary<(int, int), int> passiveSpellIDsByProcSpellAndChance = new Dictionary<(int, int), int>();
            int nextPassiveSpellID = Configuration.SPELL_PET_ATTACK_PROC_SPELL_ID_START;

            foreach (string spellPetTypeName in SpellPet.GetAllSpellTypeNames())
            {
                SpellPet? spellPet = SpellPet.GetSpellPetByTypeName(spellPetTypeName);
                if (spellPet == null || creatureTemplatesByEQID.ContainsKey(spellPet.EQCreatureTemplateID) == false)
                    continue;
                CreatureTemplate petCreatureTemplate = creatureTemplatesByEQID[spellPet.EQCreatureTemplateID];

                (int, int)? attackProc = GetAttackProcForSpellList(petCreatureTemplate.CreatureSpellListID, creatureSpellListsByID, new HashSet<int>());
                if (attackProc == null)
                    continue;
                int procEQSpellID = attackProc.Value.Item1;
                int procChancePercent = attackProc.Value.Item2;
                if (spellTemplatesByEQID.ContainsKey(procEQSpellID) == false)
                {
                    Logger.WriteError("Pet '", spellPet.TypeName, "' has an attack proc of EQ spell ", procEQSpellID.ToString(), " which has no spell template, so the proc is being skipped");
                    continue;
                }

                if (passiveSpellIDsByProcSpellAndChance.ContainsKey((procEQSpellID, procChancePercent)) == false)
                {
                    if (nextPassiveSpellID > Configuration.SPELL_PET_ATTACK_PROC_SPELL_ID_START + 18)
                    {
                        Logger.WriteError("Ran out of reserved pet attack proc spell IDs (", Configuration.SPELL_PET_ATTACK_PROC_SPELL_ID_START.ToString(),
                            " - ", (Configuration.SPELL_PET_ATTACK_PROC_SPELL_ID_START + 18).ToString(), "), so the proc for pet '", spellPet.TypeName, "' is being skipped");
                        continue;
                    }
                    passiveSpellIDsByProcSpellAndChance.Add((procEQSpellID, procChancePercent), nextPassiveSpellID);
                    spellTemplates.Add(BuildAttackProcSpellTemplate(nextPassiveSpellID, spellTemplatesByEQID[procEQSpellID], procChancePercent));
                    nextPassiveSpellID++;
                }
                AttackProcPassiveSpellIDsByPetWOWCreatureTemplateID[petCreatureTemplate.WOWCreatureTemplateID] = passiveSpellIDsByProcSpellAndChance[(procEQSpellID, procChancePercent)];
            }
        }

        private static (int, int)? GetAttackProcForSpellList(int creatureSpellListID, Dictionary<int, CreatureSpellList> creatureSpellListsByID, HashSet<int> visitedListIDs)
        {
            if (creatureSpellListID <= 0 || visitedListIDs.Add(creatureSpellListID) == false)
                return null;
            if (creatureSpellListsByID.ContainsKey(creatureSpellListID) == false)
                return null;
            CreatureSpellList creatureSpellList = creatureSpellListsByID[creatureSpellListID];
            if (creatureSpellList.AttackProcID > 0)
                return (creatureSpellList.AttackProcID, creatureSpellList.ProcChance);
            return GetAttackProcForSpellList(creatureSpellList.ParentListID, creatureSpellListsByID, visitedListIDs);
        }

        public static int GetAttackProcPassiveSpellIDForPet(int petWOWCreatureTemplateID)
        {
            if (AttackProcPassiveSpellIDsByPetWOWCreatureTemplateID.ContainsKey(petWOWCreatureTemplateID) == false)
                return 0; // None
            return AttackProcPassiveSpellIDsByPetWOWCreatureTemplateID[petWOWCreatureTemplateID];
        }

        private static SpellTemplate BuildAttackProcSpellTemplate(int wowSpellID, SpellTemplate procSpellTemplate, int procChancePercent)
        {
            string description = string.Concat("The pet's melee attacks have a ", procChancePercent.ToString(), "% chance to cast ", procSpellTemplate.Name, ".");
            SpellTemplate spellTemplate = BuildBaseTemplate(procSpellTemplate.Name, wowSpellID, 0, description, description);
            spellTemplate.SpellIconID = procSpellTemplate.SpellIconID; // Wear the icon of the spell the proc casts
            spellTemplate.RankName = "Passive";
            spellTemplate.IsGoodEffect = true;
            spellTemplate.IsPassiveAbility = true;
            spellTemplate.AlwaysPersist = true;
            spellTemplate.AuraDuration.IsInfinite = true;
            spellTemplate.ProcsOnMeleeAttacks = true;
            spellTemplate.ProcChance = Convert.ToUInt32(procChancePercent);
            spellTemplate.ProcRow = new SpellProcRow(PROC_FLAG_DONE_MELEE_AUTO_ATTACK | PROC_FLAG_DONE_SPELL_MELEE_DMG_CLASS, PROC_SPELL_TYPE_DAMAGE, PROC_SPELL_PHASE_HIT,
                PROC_HIT_NORMAL | PROC_HIT_CRITICAL, Configuration.CREATURE_SPELL_ATTACK_PROC_COOLDOWN_IN_MS, procChancePercent);
            SpellEffectWOW procEffect = new SpellEffectWOW(SpellWOWEffectType.ApplyAura, SpellWOWAuraType.ProcTriggerSpell, 0, 0, 1, -1, 0, 0);
            procEffect.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
            procEffect.EffectTriggerSpell = procSpellTemplate.GetWOWSpellIDForCreatureCast();
            spellTemplate.WOWSpellEffects.Add(procEffect);
            return spellTemplate;
        }

        public static void ApplySkillLines(SpellTemplate spellTemplate, List<SpellPetType> petTypes)
        {
            foreach (SpellPetType petType in petTypes)
            {
                if (spellTemplate.SkillLine == 0)
                    spellTemplate.SkillLine = petType.GetSkillLineID();
                else
                    spellTemplate.AdditionalSkillLineIDs.Add(petType.GetSkillLineID());
            }
        }

        private static SpellTemplate BuildBaseTemplate(string name, int wowSpellID, int spellIconEQID, string description, string auraDescription)
        {
            SpellTemplate spellTemplate = new SpellTemplate();
            spellTemplate.Name = name;
            spellTemplate.WOWSpellID = wowSpellID;
            spellTemplate.EQSpellID = SpellTemplate.GenerateUniqueEQSpellID();
            spellTemplate.Description = description;
            spellTemplate.AuraDescription = auraDescription;
            spellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(GetValidatedSpellIconID(spellIconEQID, name));
            spellTemplate.CastTimeInMS = 0;
            spellTemplate.RecoveryTimeInMS = 0;
            spellTemplate.TriggersGlobalCooldown = false;
            spellTemplate.CannotBeStolen = true;
            spellTemplate.EQSkillCategory = SpellEQSkillCategory.Combat;
            spellTemplate.AuraDuration = new SpellDuration();
            return spellTemplate;
        }

        private static int GetValidatedSpellIconID(int spellIconEQID, string name)
        {
            if (spellIconEQID >= 0 && spellIconEQID <= 22)
                return spellIconEQID;
            Logger.WriteError("Pet ability '", name, "' has a spell icon EQ ID of ", spellIconEQID.ToString(), " which must be 0-22, so 0 is being used instead");
            return 0;
        }
        
        private static SpellTemplate BuildAvoidanceSpellTemplate()
        {
            // A rank-for-rank clone of the warlock pet passive Avoidance (32233), which every EQ summoned pet learns at the configured level
            int reductionPercent = Configuration.SPELL_PET_AVOIDANCE_DAMAGE_REDUCTION_PERCENT;
            string description = string.Concat("Reduces the damage your summoned pet takes from creature area of effect attacks by an additional ",
                reductionPercent.ToString(), "%.");
            SpellTemplate spellTemplate = BuildBaseTemplate("Avoidance", Configuration.SPELL_PET_AVOIDANCE_SPELL_ID, Configuration.SPELL_PET_AVOIDANCE_SPELL_ICON_EQ_ID,
                description, description);
            spellTemplate.RankName = "Passive";
            spellTemplate.IsGoodEffect = true;
            spellTemplate.IsPassiveAbility = true;
            spellTemplate.AlwaysPersist = true;
            spellTemplate.AuraDuration.IsInfinite = true;
            spellTemplate.SkillLineAcquireMethod = 2;
            spellTemplate.SpellLevel = Configuration.SPELL_PET_AVOIDANCE_LEARN_LEVEL;
            spellTemplate.MinimumPlayerLearnLevel = Configuration.SPELL_PET_AVOIDANCE_LEARN_LEVEL;

            SpellEffectWOW avoidanceEffect = new SpellEffectWOW(SpellWOWEffectType.ApplyAura, SpellWOWAuraType.ModCreatureAoeDamageAvoidance, 0, 0, 0,
                -reductionPercent, SCHOOL_MASK_ALL, 0);
            avoidanceEffect.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
            spellTemplate.WOWSpellEffects.Add(avoidanceEffect);
            return spellTemplate;
        }

        private static SpellTemplate BuildFrenzySpellTemplate()
        {
            // The Felguard's Demonic Frenzy (32850) is a passive that procs off landed melee damage and stacks an attack power buff (32851) on the pet
            SpellTemplate spellTemplate = BuildBaseTemplate("Pet Frenzy", Configuration.SPELL_PET_FRENZY_SPELL_ID, Configuration.SPELL_PET_FRENZY_SPELL_ICON_EQ_ID,
                GetFrenzyDescription(), GetFrenzyDescription());
            spellTemplate.RankName = "Passive";
            spellTemplate.IsGoodEffect = true;
            spellTemplate.IsPassiveAbility = true;
            spellTemplate.AlwaysPersist = true;
            spellTemplate.AuraDuration.IsInfinite = true;
            spellTemplate.SkillLineAcquireMethod = 2;
            spellTemplate.SpellLevel = Configuration.SPELL_PET_FRENZY_LEARN_LEVEL;
            spellTemplate.MinimumPlayerLearnLevel = Configuration.SPELL_PET_FRENZY_LEARN_LEVEL;
            spellTemplate.ProcsOnMeleeAttacks = true; // Proc flags 4 | 16, exactly what Demonic Frenzy carries
            spellTemplate.ProcChance = 100;

            SpellEffectWOW frenzyProcEffect = new SpellEffectWOW(SpellWOWEffectType.ApplyAura, SpellWOWAuraType.ProcTriggerSpell, 0, 0, 1, -1, 0, 0);
            frenzyProcEffect.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
            frenzyProcEffect.EffectTriggerSpell = Configuration.SPELL_PET_FRENZY_BUFF_SPELL_ID;
            spellTemplate.WOWSpellEffects.Add(frenzyProcEffect);
            return spellTemplate;
        }

        private static SpellTemplate BuildFrenzyBuffSpellTemplate()
        {
            int percentPerStack = Configuration.SPELL_PET_FRENZY_ATTACK_POWER_PERCENT_PER_STACK;
            string auraDescription = string.Concat("Attack power increased by ", percentPerStack.ToString(), "%.");
            SpellTemplate spellTemplate = BuildBaseTemplate("Pet Frenzy", Configuration.SPELL_PET_FRENZY_BUFF_SPELL_ID, Configuration.SPELL_PET_FRENZY_SPELL_ICON_EQ_ID,
                GetFrenzyDescription(), auraDescription);
            spellTemplate.IsGoodEffect = true;
            spellTemplate.PreventAuraClickOff = true;
            spellTemplate.AuraDuration.SetFixedDuration(Configuration.SPELL_PET_FRENZY_DURATION_IN_MS);
            spellTemplate.MaxStackAmount = Convert.ToUInt32(Math.Max(1, Configuration.SPELL_PET_FRENZY_MAX_STACKS));

            SpellEffectWOW attackPowerEffect = new SpellEffectWOW(SpellWOWEffectType.ApplyAura, SpellWOWAuraType.ModAttackPowerPct, 0, 0, 0, percentPerStack, 0, 0);
            attackPowerEffect.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
            spellTemplate.WOWSpellEffects.Add(attackPowerEffect);
            return spellTemplate;
        }

        private static string GetFrenzyDescription()
        {
            return string.Concat("The pet's damaging melee attacks cause a frenzy, increasing attack power by ",
                Configuration.SPELL_PET_FRENZY_ATTACK_POWER_PERCENT_PER_STACK.ToString(), "% for ",
                (Configuration.SPELL_PET_FRENZY_DURATION_IN_MS / 1000).ToString(), " sec. This effect can stack up to ",
                Configuration.SPELL_PET_FRENZY_MAX_STACKS.ToString(), " times.");
        }

        private static SpellTemplate BuildSpellDamageSpellTemplate(SpellPetType petType)
        {
            // Hidden damage multiplier that is always-on
            int bonusPercent = petType.GetSpellDamageBonusPercent();
            string description = string.Concat("Increases the damage of this pet's spells and abilities by ", bonusPercent.ToString(), "%.");
            SpellTemplate spellTemplate = BuildBaseTemplate(string.Concat("Pet Spell Damage (", petType.TypeName, ")"), petType.GetSpellDamagePassiveSpellID(),
                Configuration.SPELL_PET_FRENZY_SPELL_ICON_EQ_ID, description, description);
            spellTemplate.IsGoodEffect = true;
            spellTemplate.IsPassiveAbility = true;
            spellTemplate.ForceHiddenFromDisplay = true;
            spellTemplate.AlwaysPersist = true;
            spellTemplate.AuraDuration.IsInfinite = true;
            spellTemplate.SkillLineAcquireMethod = 2;
            spellTemplate.SpellLevel = 0; // Zero keeps it out of the level up map and in the always-learned pet family passive store
            spellTemplate.SkillLine = petType.GetSkillLineID();

            SpellEffectWOW spellDamageEffect = new SpellEffectWOW(SpellWOWEffectType.ApplyAura, SpellWOWAuraType.ModDamagePercentDone, 0, 0, 0, bonusPercent,
                SCHOOL_MASK_MAGIC, 0);
            spellDamageEffect.ImplicitTargetA = SpellWOWTargetType.UnitCaster;
            spellTemplate.WOWSpellEffects.Add(spellDamageEffect);
            return spellTemplate;
        }
    }
}
