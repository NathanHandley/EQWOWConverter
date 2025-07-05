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
    // Scraped from wowwiki on 2025/02/25, which took from TrinityCore
    // TARGET_FLAG_NONE            = 0x00000000,
    // TARGET_FLAG_UNUSED_1        = 0x00000001,               // not used
    // TARGET_FLAG_UNIT            = 0x00000002,               // pguid
    // TARGET_FLAG_UNIT_RAID       = 0x00000004,               // not sent, used to validate target (if raid member)
    // TARGET_FLAG_UNIT_PARTY      = 0x00000008,               // not sent, used to validate target (if party member)
    // TARGET_FLAG_ITEM            = 0x00000010,               // pguid
    // TARGET_FLAG_SOURCE_LOCATION = 0x00000020,               // pguid, 3 float
    // TARGET_FLAG_DEST_LOCATION   = 0x00000040,               // pguid, 3 float
    // TARGET_FLAG_UNIT_ENEMY      = 0x00000080,               // not sent, used to validate target (if enemy)
    // TARGET_FLAG_UNIT_ALLY       = 0x00000100,               // not sent, used to validate target (if ally) - Used by teaching spells
    // TARGET_FLAG_CORPSE_ENEMY    = 0x00000200,               // pguid
    // TARGET_FLAG_UNIT_DEAD       = 0x00000400,               // not sent, used to validate target (if dead creature)
    // TARGET_FLAG_GAMEOBJECT      = 0x00000800,               // pguid, used with TARGET_GAMEOBJECT_TARGET
    // TARGET_FLAG_TRADE_ITEM      = 0x00001000,               // pguid
    // TARGET_FLAG_STRING          = 0x00002000,               // string
    // TARGET_FLAG_GAMEOBJECT_ITEM = 0x00004000,               // not sent, used with TARGET_GAMEOBJECT_ITEM_TARGET
    // TARGET_FLAG_CORPSE_ALLY     = 0x00008000,               // pguid
    // TARGET_FLAG_UNIT_MINIPET    = 0x00010000,               // pguid, used to validate target (if non combat pet)
    // TARGET_FLAG_GLYPH_SLOT      = 0x00020000,               // used in glyph spells
    // TARGET_FLAG_DEST_TARGET     = 0x00040000,               // sometimes appears with DEST_TARGET spells (may appear or not for a given spell)
    // TARGET_FLAG_UNUSED20        = 0x00080000,               // uint32 counter, loop { vec3 - screen position (?), guid }, not used so far
    // TARGET_FLAG_UNIT_PASSENGER  = 0x00100000,               // guessed, used to validate target (if vehicle passenger)

    internal class SpellDBC : DBCFile
    {
        public void AddRow(SpellTemplate spellTemplate)
        {
            DBCRow newRow = new DBCRow();            
            newRow.AddInt32(spellTemplate.WOWSpellID); // ID
            newRow.AddUInt32(spellTemplate.Category); // Category (SpellCategory.ID)
            newRow.AddUInt32(0); // DispelType
            newRow.AddUInt32(0); // Mechanic
            newRow.AddUInt32(GetAttributes(spellTemplate)); // Attributes
            newRow.AddUInt32(GetAttributesEx(spellTemplate)); // AttributesEx
            newRow.AddUInt32(GetAttributesExB(spellTemplate)); // AttributesExB
            newRow.AddUInt32(GetAttributesExC(spellTemplate)); // AttributesExC
            newRow.AddUInt32(GetAttributesExD(spellTemplate)); // AttributesExD
            newRow.AddUInt32(GetAttributesExE(spellTemplate)); // AttributesExE
            newRow.AddUInt32(GetAttributesExF(spellTemplate)); // AttributesExF
            newRow.AddUInt32(0); // AttributesExG
            newRow.AddUInt64(0); // ShapeshiftMask
            newRow.AddUInt64(0); // ShapeshiftExclude
            newRow.AddUInt32(GetTargetValue(spellTemplate)); // Targets
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
            newRow.AddUInt32(spellTemplate.RecoveryTimeInMS); // RecoveryTime
            newRow.AddUInt32(0); // CategoryRecoveryTime
            newRow.AddUInt32(spellTemplate.InterruptFlags); // InterruptFlags (15 is standard interrupt for things like moving, pushback, and interrupt cast)
            newRow.AddUInt32(0); // AuraInterruptFlags
            newRow.AddUInt32(0); // ChannelInterruptFlags
            newRow.AddUInt32(0); // ProcTypeMask
            newRow.AddUInt32(101); // ProcChance
            newRow.AddUInt32(0); // ProcCharges
            newRow.AddUInt32(0); // MaxLevel
            newRow.AddUInt32(0); // BaseLevel
            newRow.AddUInt32(0); // SpellLevel
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
            newRow.AddInt32(Convert.ToInt32(spellTemplate.EffectType1)); // Effect1
            newRow.AddInt32(Convert.ToInt32(spellTemplate.EffectType2)); // Effect2
            newRow.AddInt32(Convert.ToInt32(spellTemplate.EffectType3)); // Effect3
            newRow.AddInt32(spellTemplate.EffectDieSides1); // EffectDieSides1
            newRow.AddInt32(spellTemplate.EffectDieSides2); // EffectDieSides2
            newRow.AddInt32(spellTemplate.EffectDieSides3); // EffectDieSides3
            newRow.AddFloat(0); // EffectRealPointsPerLevel1
            newRow.AddFloat(0); // EffectRealPointsPerLevel2
            newRow.AddFloat(0); // EffectRealPointsPerLevel3
            newRow.AddInt32(spellTemplate.EffectBasePoints1); // EffectBasePoints1
            newRow.AddInt32(spellTemplate.EffectBasePoints2); // EffectBasePoints2
            newRow.AddInt32(spellTemplate.EffectBasePoints3); // EffectBasePoints3
            newRow.AddUInt32(0); // EffectMechanic1
            newRow.AddUInt32(0); // EffectMechanic2
            newRow.AddUInt32(0); // EffectMechanic3
            newRow.AddUInt32(GetImplicitTargetA1Value(spellTemplate)); // ImplicitTargetA1
            newRow.AddUInt32(0); // ImplicitTargetA2
            newRow.AddUInt32(0); // ImplicitTargetA3
            newRow.AddUInt32(0); // ImplicitTargetB1
            newRow.AddUInt32(0); // ImplicitTargetB2
            newRow.AddUInt32(0); // ImplicitTargetB3
            newRow.AddUInt32(0); // EffectRadiusIndex1
            newRow.AddUInt32(0); // EffectRadiusIndex2
            newRow.AddUInt32(0); // EffectRadiusIndex3
            newRow.AddUInt32(Convert.ToUInt32(spellTemplate.EffectAuraType1)); // EffectAura1
            newRow.AddUInt32(Convert.ToUInt32(spellTemplate.EffectAuraType2)); // EffectAura2
            newRow.AddUInt32(Convert.ToUInt32(spellTemplate.EffectAuraType3)); // EffectAura3
            newRow.AddUInt32(0); // EffectAuraPeriod1
            newRow.AddUInt32(0); // EffectAuraPeriod2
            newRow.AddUInt32(0); // EffectAuraPeriod3
            newRow.AddFloat(0); // EffectMultipleValue1
            newRow.AddFloat(0); // EffectMultipleValue2
            newRow.AddFloat(0); // EffectMultipleValue3
            newRow.AddUInt32(0); // EffectChainTargets1
            newRow.AddUInt32(0); // EffectChainTargets2
            newRow.AddUInt32(0); // EffectChainTargets3
            newRow.AddUInt32(spellTemplate.EffectItemType1); // EffectItemType1
            newRow.AddUInt32(spellTemplate.EffectItemType2); // EffectItemType2
            newRow.AddUInt32(spellTemplate.EffectItemType3); // EffectItemType3
            newRow.AddInt32(spellTemplate.EffectMiscValueA1); // EffectMiscValue1
            newRow.AddInt32(spellTemplate.EffectMiscValueA2); // EffectMiscValue2
            newRow.AddInt32(spellTemplate.EffectMiscValueA3); // EffectMiscValue3
            newRow.AddInt32(spellTemplate.EffectMiscValueB1); // EffectMiscValueB1
            newRow.AddInt32(spellTemplate.EffectMiscValueB2); // EffectMiscValueB2
            newRow.AddInt32(spellTemplate.EffectMiscValueB3); // EffectMiscValueB3
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
            newRow.AddStringLang(spellTemplate.Name); // Name_Lang
            newRow.AddStringLang(""); // NameSubtext_Lang
            newRow.AddStringLang(spellTemplate.Description); // Description_Lang
            newRow.AddStringLang(spellTemplate.AuraDescription); // AuraDescription_Lang
            newRow.AddUInt32(0); // ManaCostPct
            newRow.AddUInt32(0); // StartRecoveryCategory
            newRow.AddUInt32(0); // StartRecoveryTime
            newRow.AddUInt32(0); // MaxTargetLevel
            newRow.AddUInt32(0); // SpellClassSet
            newRow.AddUInt32(0); // SpellClassMask1
            newRow.AddUInt32(0); // SpellClassMask2
            newRow.AddUInt32(0); // SpellClassMask3
            newRow.AddUInt32(0); // MaxTargets
            newRow.AddUInt32(0); // DefenseType
            newRow.AddUInt32(0); // PreventionType
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

        private UInt32 GetAttributes(SpellTemplate spellTemplate)
        {
            if (spellTemplate.EffectAuraType1 == SpellWOWAuraType.Phase) // Phase Aura
                return 2843738496;
            UInt32 attributeFlags = 0;
            if (spellTemplate.AllowCastInCombat == false)
                attributeFlags |= 268435456; // SPELL_ATTR0_NOT_IN_COMBAT_ONLY_PEACEFUL (0x10000000)
            if (spellTemplate.TradeskillRecipe != null)
            {
                attributeFlags |= 16; // SPELL_ATTR0_IS_ABILITY (0x00000010)
                attributeFlags |= 32; // SPELL_ATTR0_IS_TRADESKILL (0x00000020)
            }
            attributeFlags |= 65536; // SPELL_ATTR0_NOT_SHAPESHIFTED (0x00010000)
            return attributeFlags;
        }

        private UInt32 GetAttributesEx(SpellTemplate spellTemplate)
        {
            if (spellTemplate.EffectAuraType1 == SpellWOWAuraType.Phase) // Phase Aura
                return 3072;
            return 0;
        }

        private UInt32 GetAttributesExB(SpellTemplate spellTemplate)
        {
            if (spellTemplate.EffectAuraType1 == SpellWOWAuraType.Phase) // Phase Aura
                return 16385;
            return 0;
        }

        private UInt32 GetAttributesExC(SpellTemplate spellTemplate)
        {
            if (spellTemplate.EffectAuraType1 == SpellWOWAuraType.Phase) // Phase Aura
                return 1048576;
            return 0;
        }

        private UInt32 GetAttributesExD(SpellTemplate spellTemplate)
        {
            if (spellTemplate.EffectAuraType1 == SpellWOWAuraType.Phase) // Phase Aura
                return 128;
            return 0;
        }

        private UInt32 GetAttributesExE(SpellTemplate spellTemplate)
        {
            if (spellTemplate.EffectAuraType1 == SpellWOWAuraType.Phase) // Phase Aura
                return 393224;
            return 0;
        }

        private UInt32 GetAttributesExF(SpellTemplate spellTemplate)
        {
            if (spellTemplate.EffectAuraType1 == SpellWOWAuraType.Phase) // Phase Aura
                return 4096;
            return 0;
        }

        private UInt32 GetTargetValue(SpellTemplate spellTemplate)
        {
            switch (spellTemplate.WOWTargetType)
            {
                //case SpellTargetType.SelfSingle: return 0;
                //case SpellTargetType.AllyGroupedSingle: return 16;
                default: return 0;
            }
        }

        private UInt32 GetImplicitTargetA1Value(SpellTemplate spellTemplate)
        {
            return Convert.ToUInt32(spellTemplate.WOWTargetType);
        }
    }
}
