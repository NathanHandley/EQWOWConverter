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

using EQWOWConverter.Spells;

namespace EQWOWConverter.WOWFiles
{
    internal class SpellDBC : DBCFile
    {
        public void AddRow(int spellID, string spellName, string spellDescription, SpellTemplate spellTemplate, List<SpellEffectWOW> spellEffects, bool doHideFromDisplay, bool overrideDurationToInfinite, 
            bool preventClickOff)
        {
            if (spellEffects.Count != 3)
            {
                Logger.WriteError("Failed to add row to SpelLDBC for spellID ", spellID.ToString(), " since there were not exactly three spellEffects");
                return;
            }

            DBCRow newRow = new DBCRow();            
            newRow.AddInt32(spellID); // ID
            newRow.AddUInt32(spellTemplate.Category); // Category (SpellCategory.ID)
            newRow.AddUInt32(spellTemplate.DispelType); // DispelType
            newRow.AddUInt32(0); // Mechanic
            newRow.AddUInt32(GetAttributes(spellTemplate, spellEffects[0].EffectAuraType, doHideFromDisplay, preventClickOff)); // Attributes
            newRow.AddUInt32(GetAttributesEx(spellTemplate, spellEffects[0].EffectAuraType)); // AttributesEx
            newRow.AddUInt32(GetAttributesExB(spellTemplate, spellEffects[0].EffectAuraType)); // AttributesExB
            newRow.AddUInt32(GetAttributesExC(spellTemplate, spellEffects[0].EffectAuraType)); // AttributesExC
            newRow.AddUInt32(GetAttributesExD(spellTemplate, spellEffects[0].EffectAuraType)); // AttributesExD
            newRow.AddUInt32(GetAttributesExE(spellTemplate, spellEffects[0].EffectAuraType)); // AttributesExE
            newRow.AddUInt32(GetAttributesExF(spellTemplate, spellEffects[0].EffectAuraType)); // AttributesExF
            newRow.AddUInt32(0); // AttributesExG
            newRow.AddUInt64(0); // ShapeshiftMask
            newRow.AddUInt64(0); // ShapeshiftExclude
            newRow.AddUInt32(0); // Targets (should this be non-zero?)
            newRow.AddUInt32(spellTemplate.TargetCreatureType); // TargetCreatureType
            newRow.AddUInt32(spellTemplate.SpellFocusID); // RequiresSpellFocus
            newRow.AddUInt32(0); // FacingCasterFlags
            newRow.AddUInt32(0); // CasterAuraState
            newRow.AddUInt32(0); // TargetAuraState
            newRow.AddUInt32(0); // ExcludeCasterAuraState
            newRow.AddUInt32(0); // ExcludeTargetAuraState
            newRow.AddUInt32(0); // CasterAuraSpell
            newRow.AddUInt32(0); // TargetAuraSpell
            newRow.AddUInt32(0); // ExcludeCasterAuraSpell
            newRow.AddUInt32(0); // ExcludeTargetAuraSpell
            newRow.AddUInt32(Convert.ToUInt32(spellTemplate.SpellCastTimeDBCID)); // CastingTimeIndex
            if (spellTemplate.RecoveryTimeInMS < Configuration.SPELL_RECOVERY_TIME_MINIMUM_IN_MS)
                newRow.AddUInt32(0); // RecoveryTime
            else
                newRow.AddUInt32(spellTemplate.RecoveryTimeInMS); // RecoveryTime
            newRow.AddUInt32(0); // CategoryRecoveryTime
            newRow.AddUInt32(spellTemplate.InterruptFlags); // InterruptFlags (15 is standard interrupt for things like moving, pushback, and interrupt cast)
            newRow.AddUInt32(0); // AuraInterruptFlags
            newRow.AddUInt32(0); // ChannelInterruptFlags
            newRow.AddUInt32(GetProcFlags(spellTemplate)); // ProcTypeMask
            newRow.AddUInt32(101); // ProcChance
            newRow.AddUInt32(0); // ProcCharges
            newRow.AddUInt32(0); // MaxLevel
            newRow.AddUInt32(Convert.ToUInt32(Math.Max(0, spellTemplate.MinimumPlayerLearnLevel))); // BaseLevel
            newRow.AddUInt32(Convert.ToUInt32(Math.Max(0, spellTemplate.MinimumPlayerLearnLevel))); // SpellLevel
            if (overrideDurationToInfinite == true)
                newRow.AddUInt32(21); // DurationIndex (SpellDuration.dbc id) - 21 is infinite (auras use it)
            else
                newRow.AddUInt32(Convert.ToUInt32(spellTemplate.SpellDurationDBCID)); // DurationIndex (SpellDuration.dbc id)
            newRow.AddInt32(0); // PowerType
            newRow.AddUInt32(spellTemplate.ManaCost); // ManaCost
            newRow.AddUInt32(0); // ManaCostPerLevel
            newRow.AddUInt32(0); // ManaPerSecond
            newRow.AddUInt32(0); // ManaPerSecondPerLevel
            newRow.AddUInt32(Convert.ToUInt32(spellTemplate.SpellRangeDBCID)); // RangeIndex (SpellRange.ID)
            newRow.AddFloat(0); // Speed
            newRow.AddUInt32(0); // ModalNextSpell
            newRow.AddUInt32(0); // CumulativeAura
            newRow.AddUInt32(0); // Totem1
            newRow.AddUInt32(0); // Totem2
            for (int i = 0; i < 8; i++)
            {
                if (i < spellTemplate.Reagents.Count)
                    newRow.AddInt32(spellTemplate.Reagents[i].WOWItemTemplateEntryID); // ReagentX
                else
                    newRow.AddInt32(0); // ReagentX
            }
            for (int i = 0; i < 8; i++)
            {
                if (i < spellTemplate.Reagents.Count)
                    newRow.AddInt32(spellTemplate.Reagents[i].Count); // ReagentCountX
                else
                    newRow.AddInt32(0); // ReagentCountX
            }
            newRow.AddInt32(-1); // EquippedItemClass
            newRow.AddInt32(0); // EquippedItemSubclass
            newRow.AddInt32(0); // EquippedItemInvTypes
            foreach (SpellEffectWOW spellEffect in spellEffects) 
                newRow.AddInt32(Convert.ToInt32(spellEffect.EffectType)); // Effect1, Effect2, Effect3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddInt32(spellEffect.EffectDieSides); // EffectDieSides1, EffectDieSides2, EffectDieSides3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddFloat(spellEffect.EffectRealPointsPerLevel); // EffectRealPointsPerLevel1, EffectRealPointsPerLevel2, EffectRealPointsPerLevel3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddInt32(spellEffect.EffectBasePoints); // EffectBasePoints1, EffectBasePoints2, EffectBasePoints3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddUInt32(Convert.ToUInt32(spellEffect.EffectMechanic)); // EffectMechanic1, EffectMechanic2, EffectMechanic3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddUInt32(Convert.ToUInt32(spellEffect.ImplicitTargetA)); // ImplicitTargetA1, ImplicitTargetA2, ImplicitTargetA3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddUInt32(Convert.ToUInt32(spellEffect.ImplicitTargetB)); // ImplicitTargetB1, ImplicitTargetB2, ImplicitTargetB3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddUInt32(spellEffect.EffectRadiusIndex); // EffectRadiusIndex1, EffectRadiusIndex2, EffectRadiusIndex3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddUInt32(Convert.ToUInt32(spellEffect.EffectAuraType)); // EffectAura1, EffectAura2, EffectAura3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddUInt32(spellEffect.EffectAuraPeriod); // EffectAuraPeriod1, EffectAuraPeriod2, EffectAuraPeriod3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddFloat(spellEffect.EffectMultipleValue); // EffectMultipleValue1, EffectMultipleValue2, EffectMultipleValue3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddUInt32(spellEffect.EffectChainTargets); // EffectChainTargets1, EffectChainTargets2, EffectChainTargets3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddUInt32(spellEffect.EffectItemType); // EffectItemType1, EffectItemType2, EffectItemType3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddInt32(spellEffect.EffectMiscValueA); // EffectMiscValueA1, EffectMiscValueA2, EffectMiscValueA3
            foreach (SpellEffectWOW spellEffect in spellEffects)
                newRow.AddInt32(spellEffect.EffectMiscValueB); // EffectMiscValueB1, EffectMiscValueB2, EffectMiscValueB3
            newRow.AddUInt32(0); // EffectTriggerSpell1
            newRow.AddUInt32(0); // EffectTriggerSpell2
            newRow.AddUInt32(0); // EffectTriggerSpell3
            newRow.AddFloat(0); // EffectPointsPerCombo1
            newRow.AddFloat(0); // EffectPointsPerCombo2
            newRow.AddFloat(0); // EffectPointsPerCombo3
            newRow.AddUInt32(0); // EffectSpellClassMaskA1
            newRow.AddUInt32(0); // EffectSpellClassMaskA2
            newRow.AddUInt32(0); // EffectSpellClassMaskA3
            newRow.AddUInt32(0); // EffectSpellClassMaskB1
            newRow.AddUInt32(0); // EffectSpellClassMaskB2
            newRow.AddUInt32(0); // EffectSpellClassMaskB3
            newRow.AddUInt32(0); // EffectSpellClassMaskC1
            newRow.AddUInt32(0); // EffectSpellClassMaskC2
            newRow.AddUInt32(0); // EffectSpellClassMaskC2
            newRow.AddUInt32(spellTemplate.SpellVisualID1); // SpellVisualID1
            newRow.AddUInt32(spellTemplate.SpellVisualID2); // SpellVisualID2
            newRow.AddUInt32(Convert.ToUInt32(spellTemplate.SpellIconID)); // SpellIconID
            newRow.AddUInt32(0); // ActiveIconID
            newRow.AddUInt32(0); // SpellPriority
            newRow.AddStringLang(spellName); // Name_Lang
            newRow.AddStringLang(""); // NameSubtext_Lang
            newRow.AddStringLang(spellDescription); // Description_Lang
            newRow.AddStringLang(spellTemplate.AuraDescription); // AuraDescription_Lang
            newRow.AddUInt32(0); // ManaCostPct
            newRow.AddUInt32(133); // StartRecoveryCategory
            newRow.AddUInt32(1500); // StartRecoveryTime
            newRow.AddUInt32(0); // MaxTargetLevel
            newRow.AddUInt32(0); // SpellClassSet
            newRow.AddUInt32(0); // SpellClassMask1
            newRow.AddUInt32(0); // SpellClassMask2
            newRow.AddUInt32(0); // SpellClassMask3
            newRow.AddUInt32(0); // MaxTargets
            newRow.AddUInt32(spellTemplate.DefenseType); // DefenseType
            newRow.AddUInt32(spellTemplate.PreventionType); // PreventionType
            newRow.AddUInt32(0); // StanceBarOrder
            newRow.AddFloat(0); // EffectChainAmplitude1
            newRow.AddFloat(0); // EffectChainAmplitude2
            newRow.AddFloat(0); // EffectChainAmplitude3
            newRow.AddUInt32(0); // MinFactionID
            newRow.AddUInt32(0); // MinReputation
            newRow.AddUInt32(0); // RequiredAuraVision
            newRow.AddUInt32(spellTemplate.RequiredTotemID1); // RequiredTotemCategoryID1
            newRow.AddUInt32(spellTemplate.RequiredTotemID2); // RequiredTotemCategoryID2
            newRow.AddInt32(spellTemplate.RequiredAreaIDs); // RequiredAreasID
            newRow.AddUInt32(spellTemplate.SchoolMask); // SchoolMask
            newRow.AddUInt32(0); // RuneCostID
            newRow.AddUInt32(0); // SpellMissileID
            newRow.AddInt32(0); // PowerDisplayID
            newRow.AddFloat(0); // Field227
            newRow.AddFloat(0); // Field228
            newRow.AddFloat(0); // Field229
            newRow.AddUInt32(0); // SpellDescriptionVariableID
            newRow.AddUInt32(0); // SpellDifficultyID
            Rows.Add(newRow);
        }
        
        private UInt32 GetAttributes(SpellTemplate spellTemplate, SpellWOWAuraType auraType, bool doHideFromDisplay, bool preventClickOff)
        {
            if (auraType == SpellWOWAuraType.Phase) // Phase Aura
                return 2843738496;
            UInt32 attributeFlags = 0;
            if (doHideFromDisplay == true)
                attributeFlags |= 128; // SPELL_ATTR0_DO_NOT_DISPLAY_SPELLBOOK_AURA_ICON_COMBAT_LOG
            if (spellTemplate.AllowCastInCombat == false)
                attributeFlags |= 268435456; // SPELL_ATTR0_NOT_IN_COMBAT_ONLY_PEACEFUL (0x10000000)
            if (spellTemplate.TradeskillRecipe != null)
            {
                attributeFlags |= 16; // SPELL_ATTR0_IS_ABILITY (0x00000010)
                attributeFlags |= 32; // SPELL_ATTR0_IS_TRADESKILL (0x00000020)
            }
            attributeFlags |= 65536; // SPELL_ATTR0_NOT_SHAPESHIFTED (0x00010000)
            if (preventClickOff == true)
                attributeFlags |= 2147483648; // SPELL_ATTR0_NO_AURA_CANCEL (0x80000000)
            return attributeFlags;
        }

        private UInt32 GetAttributesEx(SpellTemplate spellTemplate, SpellWOWAuraType auraType)
        {
            if (auraType == SpellWOWAuraType.Phase) // Phase Aura
                return 3072;
            return 0;
        }

        private UInt32 GetAttributesExB(SpellTemplate spellTemplate, SpellWOWAuraType auraType)
        {
            if (auraType == SpellWOWAuraType.Phase) // Phase Aura
                return 16385;
            return 0;
        }

        private UInt32 GetAttributesExC(SpellTemplate spellTemplate, SpellWOWAuraType auraType)
        {
            if (auraType == SpellWOWAuraType.Phase) // Phase Aura
                return 1048576;
            return 0;
        }

        private UInt32 GetAttributesExD(SpellTemplate spellTemplate, SpellWOWAuraType auraType)
        {
            if (auraType == SpellWOWAuraType.Phase) // Phase Aura
                return 128;
            UInt32 attributeFlags = 0;
            if (spellTemplate.NoPartialImmunity == true)
            {
                attributeFlags |= 2048; // SPELL_ATTR4_NO_PARTIAL_IMMUNITY
                attributeFlags |= 536870912; // SPELL_ATTR4_AURA_BOUNCE_FAILS_SPELL
            }
            return attributeFlags;
        }

        private UInt32 GetAttributesExE(SpellTemplate spellTemplate, SpellWOWAuraType auraType)
        {
            if (auraType == SpellWOWAuraType.Phase) // Phase Aura
                return 393224;
            UInt32 attributeFlags = 0;
            return attributeFlags;
        }

        private UInt32 GetAttributesExF(SpellTemplate spellTemplate, SpellWOWAuraType auraType)
        {
            if (auraType == SpellWOWAuraType.Phase) // Phase Aura
                return 4096;
            return 0;
        }

        private UInt32 GetProcFlags(SpellTemplate spellTemplate)
        {
            UInt32 procFlags = 0;
            if (spellTemplate.BreakEffectOnNonAutoDirectDamage == true)
            {   
                procFlags |= 32; // PROC_FLAG_TAKEN_SPELL_MELEE_DMG_CLASS
                procFlags |= 512; // PROC_FLAG_TAKEN_SPELL_RANGED_DMG_CLASS
                procFlags |= 8192; // PROC_FLAG_TAKEN_SPELL_NONE_DMG_CLASS_NEG
                procFlags |= 131072; // PROC_FLAG_TAKEN_SPELL_MAGIC_DMG_CLASS_NEG
            }
            return procFlags;
        }
    }
}
