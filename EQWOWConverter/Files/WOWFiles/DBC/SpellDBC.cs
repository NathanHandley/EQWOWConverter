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

using Google.Protobuf.WellKnownTypes;
using System.Xml.Linq;
using System;

namespace EQWOWConverter.WOWFiles
{
    internal class SpellDBC : DBCFile
    {
        public void AddRow()
        {
            DBCRow newRow = new DBCRow();            
            newRow.AddInt(0); // ID
            newRow.AddInt(0); // Category
            newRow.AddInt(0); // DispelType
            newRow.AddInt(0); // Mechanic
            newRow.AddInt(0); // Attributes
            newRow.AddInt(0); // AttributesEx
            newRow.AddInt(0); // AttributesExB
            newRow.AddInt(0); // AttributesExC
            newRow.AddInt(0); // AttributesExD
            newRow.AddInt(0); // AttributesExE
            newRow.AddInt(0); // AttributesExF
            newRow.AddInt(0); // AttributesExG
            newRow.AddInt(0); // ShapeshiftMask
            newRow.AddInt(0); // ShapeshiftExclude
            newRow.AddInt(0); // Targets
            newRow.AddInt(0); // TargetCreatureType
            newRow.AddInt(0); // RequiresSpellFocus
            newRow.AddInt(0); // FacingCasterFlags
            newRow.AddInt(0); // CasterAuraState
            newRow.AddInt(0); // TargetAuraState
            newRow.AddInt(0); // ExcludeCasterAuraState
            newRow.AddInt(0); // ExcludeTargetAuraState
            newRow.AddInt(0); // CasterAuraSpell
            newRow.AddInt(0); // TargetAuraSpell
            newRow.AddInt(0); // ExcludeCasterAuraSpell
            newRow.AddInt(0); // ExcludeTargetAuraSpell
            newRow.AddInt(0); // CastingTimeIndex
            newRow.AddInt(0); // RecoveryTime
            newRow.AddInt(0); // CategoryRecoveryTime
            newRow.AddInt(0); // InterruptFlags
            newRow.AddInt(0); // AuraInterruptFlags
            newRow.AddInt(0); // ChannelInterruptFlags
            newRow.AddInt(0); // ProcTypeMask
            newRow.AddInt(0); // ProcChance
            newRow.AddInt(0); // ProcCharges
            newRow.AddInt(0); // MaxLevel
            newRow.AddInt(0); // BaseLevel
            newRow.AddInt(0); // SpellLevel
            newRow.AddInt(0); // DurationIndex
            newRow.AddInt(0); // PowerType
            newRow.AddInt(0); // ManaCost
            newRow.AddInt(0); // ManaCostPerLevel
            newRow.AddInt(0); // ManaPerSecond
            newRow.AddInt(0); // ManaPerSecondPerLevel
            newRow.AddInt(0); // RangeIndex
            newRow.AddFloat(0); // Speed
            newRow.AddInt(0); // ModalNextSpell
            newRow.AddInt(0); // CumulativeAura
            newRow.AddInt(0); // Totem1
            newRow.AddInt(0); // Totem2
            newRow.AddInt(0); // Reagent1
            newRow.AddInt(0); // Reagent2
            newRow.AddInt(0); // Reagent3
            newRow.AddInt(0); // Reagent4
            newRow.AddInt(0); // Reagent5
            newRow.AddInt(0); // Reagent6
            newRow.AddInt(0); // Reagent7
            newRow.AddInt(0); // Reagent8
            newRow.AddInt(0); // ReagentCount1
            newRow.AddInt(0); // ReagentCount2
            newRow.AddInt(0); // ReagentCount3
            newRow.AddInt(0); // ReagentCount4
            newRow.AddInt(0); // ReagentCount5
            newRow.AddInt(0); // ReagentCount6
            newRow.AddInt(0); // ReagentCount7
            newRow.AddInt(0); // ReagentCount8
            newRow.AddInt(0); // EquippedItemClass
            newRow.AddInt(0); // EquippedItemSubclass
            newRow.AddInt(0); // EquippedItemInvTypes
            newRow.AddInt(0); // Effect1
            newRow.AddInt(0); // Effect2
            newRow.AddInt(0); // Effect3
            newRow.AddInt(0); // EffectDieSides3
            newRow.AddInt(0); // EffectDieSides2
            newRow.AddInt(0); // EffectDieSides1
            newRow.AddFloat(0); // EffectRealPointsPerLevel1
            newRow.AddFloat(0); // EffectRealPointsPerLevel2
            newRow.AddFloat(0); // EffectRealPointsPerLevel3
            newRow.AddInt(0); // EffectBasePoints1
            newRow.AddInt(0); // EffectBasePoints2
            newRow.AddInt(0); // EffectBasePoints3
            newRow.AddInt(0); // EffectMechanic1
            newRow.AddInt(0); // EffectMechanic2
            newRow.AddInt(0); // EffectMechanic3
            newRow.AddInt(0); // ImplicitTargetA1
            newRow.AddInt(0); // ImplicitTargetA2
            newRow.AddInt(0); // ImplicitTargetA3
            newRow.AddInt(0); // ImplicitTargetB1
            newRow.AddInt(0); // ImplicitTargetB2
            newRow.AddInt(0); // ImplicitTargetB3
            newRow.AddInt(0); // EffectRadiusIndex1
            newRow.AddInt(0); // EffectRadiusIndex2
            newRow.AddInt(0); // EffectRadiusIndex3
            newRow.AddInt(0); // EffectAura1
            newRow.AddInt(0); // EffectAura2
            newRow.AddInt(0); // EffectAura3
            newRow.AddInt(0); // EffectAuraPeriod1
            newRow.AddInt(0); // EffectAuraPeriod2
            newRow.AddInt(0); // EffectAuraPeriod3
            newRow.AddFloat(0); // EffectMultipleValue1
            newRow.AddFloat(0); // EffectMultipleValue2
            newRow.AddFloat(0); // EffectMultipleValue3
            newRow.AddInt(0); // EffectChainTargets1
            newRow.AddInt(0); // EffectChainTargets2
            newRow.AddInt(0); // EffectChainTargets3
            newRow.AddInt(0); // EffectItemType1
            newRow.AddInt(0); // EffectItemType2
            newRow.AddInt(0); // EffectItemType3
            newRow.AddInt(0); // EffectMiscValue1
            newRow.AddInt(0); // EffectMiscValue2
            newRow.AddInt(0); // EffectMiscValue3
            newRow.AddInt(0); // EffectMiscValueB1
            newRow.AddInt(0); // EffectMiscValueB2
            newRow.AddInt(0); // EffectMiscValueB3
            newRow.AddInt(0); // EffectTriggerSpell1
            newRow.AddInt(0); // EffectTriggerSpell2
            newRow.AddInt(0); // EffectTriggerSpell3
            newRow.AddFloat(0); // EffectPointsPerCombo1
            newRow.AddFloat(0); // EffectPointsPerCombo2
            newRow.AddFloat(0); // EffectPointsPerCombo3
            newRow.AddInt(0); // EffectSpellClassMaskA1
            newRow.AddInt(0); // EffectSpellClassMaskA2
            newRow.AddInt(0); // EffectSpellClassMaskA3
            newRow.AddInt(0); // EffectSpellClassMaskB1
            newRow.AddInt(0); // EffectSpellClassMaskB2
            newRow.AddInt(0); // EffectSpellClassMaskB3
            newRow.AddInt(0); // EffectSpellClassMaskC1
            newRow.AddInt(0); // EffectSpellClassMaskC2
            newRow.AddInt(0); // EffectSpellClassMaskC2
            newRow.AddInt(0); // SpellVisualID1
            newRow.AddInt(0); // SpellVisualID2
            newRow.AddInt(0); // SpellIconID
            newRow.AddInt(0); // ActiveIconID
            newRow.AddInt(0); // SpellPriority
            newRow.AddStringLang(""); // Name_Lang
            newRow.AddStringLang(""); // NameSubtext_Lang
            newRow.AddStringLang(""); // Description_Lang
            newRow.AddStringLang(""); // AuraDescription_Lang
            newRow.AddInt(0); // ManaCostPct
            newRow.AddInt(0); // StartRecoveryCategory
            newRow.AddInt(0); // StartRecoveryTime
            newRow.AddInt(0); // MaxTargetLevel
            newRow.AddInt(0); // SpellClassSet
            newRow.AddInt(0); // SpellClassMask1
            newRow.AddInt(0); // SpellClassMask2
            newRow.AddInt(0); // SpellClassMask3
            newRow.AddInt(0); // MaxTargets
            newRow.AddInt(0); // DefenseType
            newRow.AddInt(0); // PreventionType
            newRow.AddInt(0); // StanceBarOrder
            newRow.AddFloat(0); // EffectChainAmplitude1
            newRow.AddFloat(0); // EffectChainAmplitude2
            newRow.AddFloat(0); // EffectChainAmplitude3
            newRow.AddInt(0); // MinFactionID
            newRow.AddInt(0); // MinReputation
            newRow.AddInt(0); // RequiredAuraVision
            newRow.AddInt(0); // RequiredTotemCategoryID1
            newRow.AddInt(0); // RequiredTotemCategoryID2
            newRow.AddInt(0); // RequiredAreasID
            newRow.AddInt(0); // SchoolMask
            newRow.AddInt(0); // RuneCostID
            newRow.AddInt(0); // SpellMissileID
            newRow.AddInt(0); // PowerDisplayID
            newRow.AddFloat(0); // Field227
            newRow.AddFloat(0); // Field228
            newRow.AddFloat(0); // Field229
            newRow.AddInt(0); // SpellDescriptionVariableID
            newRow.AddInt(0); // SpellDifficultyID
            Rows.Add(newRow);
        }
    }
}
