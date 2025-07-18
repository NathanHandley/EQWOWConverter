﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
using EQWOWConverter.EQFiles;
using EQWOWConverter.ObjectModels;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Spells
{
    internal class SpellVisual
    {
        private static EQSpellsEFF? EQSpellsEFF = null;
        private static readonly object SpellVisualLock = new object();
        private static List<SpellVisual> BeneficialSpellVisuals = new List<SpellVisual>();
        private static List<SpellVisual> DetrimentialSpellVisuals = new List<SpellVisual>();
        public static Dictionary<string, Sound> SoundsByFileNameNoExt = new Dictionary<string, Sound>();
        public static List<ObjectModel> VisualModels = new List<ObjectModel>();

        public int SpellVisualDBCID = 0;
        public int[] SpellVisualKitDBCIDsInStage = new int[3];
        public AnimationType[] AnimationTypeInStage = new AnimationType[3];
        public int[] SoundEntryDBCIDInStage = new int[3];
        public Dictionary<SpellVisualEffectLocation, ObjectModel> VisualModelByEffectLocation = new Dictionary<SpellVisualEffectLocation, ObjectModel>();

        private static void LoadEQSpellVisualEffectsData()
        {
            Logger.WriteDebug("Loading EQ Spell Visual Effects Data...");
            string spellsEFFFileFullPath = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "clientdata", "spells.eff");
            if (Path.Exists(spellsEFFFileFullPath) == false)
            {
                Logger.WriteError("Could not find spells.eff data that should be at ", spellsEFFFileFullPath, ", did you not run the conditioner step?");
                return;
            }
            EQSpellsEFF = new EQSpellsEFF();
            EQSpellsEFF.LoadFromDisk(spellsEFFFileFullPath);
            Logger.WriteDebug("Loading EQ Spell Visual Effects Data complete...");
        }

        public static List<SpellVisual> GetAllSpellVisuals()
        {
            lock (SpellVisualLock)
            {
                if (BeneficialSpellVisuals.Count == 0)
                    GenerateWOWSpellVisualData();
                List<SpellVisual> combinedSpellVisuals = new List<SpellVisual>();
                combinedSpellVisuals.AddRange(BeneficialSpellVisuals);
                combinedSpellVisuals.AddRange(DetrimentialSpellVisuals);
                return combinedSpellVisuals;
            }
        }

        public static SpellVisual GetSpellVisual(int effectID, bool isBeneficial)
        {
            lock (SpellVisualLock)
            {
                if (BeneficialSpellVisuals.Count == 0)
                    GenerateWOWSpellVisualData();
                if (isBeneficial == true)
                    return BeneficialSpellVisuals[effectID];
                else
                    return DetrimentialSpellVisuals[effectID];
            }
        }

        public static void GenerateWOWSpellVisualData()
        {
            Logger.WriteDebug("Generating wow spell visual data started...");
            lock (SpellVisualLock)
            {
                // Load the EQ spell data
                if (EQSpellsEFF != null)
                {
                    Logger.WriteError("Attempted to generate spell visual data twice.");
                    return;
                }
                LoadEQSpellVisualEffectsData();
                if (EQSpellsEFF == null)
                    return;

                // Create spell visual data for each of the EQ spell data
                for (int i = 0; i < EQSpellsEFF.SpellEffects.Count; i++)
                {
                    // Two copies for good vs bad
                    for (int j = 0; j < 2; j++)
                    {
                        bool isBeneficial = j == 0;
                        EQSpellsEFF.EQSpellEffect spellEffect = EQSpellsEFF.SpellEffects[i];
                        SpellVisual spellVisual = new SpellVisual();
                        spellVisual.SpellVisualDBCID = SpellVisualDBC.GenerateID();

                        // Source data
                        EQSpellsEFF.SectionData sourceSectionData = spellEffect.SectionDatas[0];
                        ConvertStageVisualData(ref spellVisual, sourceSectionData, SpellVisualStageType.Precast, isBeneficial);

                        // Sprite data (missle/projectile?)
                        EQSpellsEFF.SectionData spriteSectionData = spellEffect.SectionDatas[1];
                        ConvertStageVisualData(ref spellVisual, spriteSectionData, SpellVisualStageType.Cast, isBeneficial);

                        // Target data
                        EQSpellsEFF.SectionData targetSectionData = spellEffect.SectionDatas[2];
                        ConvertStageVisualData(ref spellVisual, targetSectionData, SpellVisualStageType.Impact, isBeneficial);

                        if (isBeneficial)
                            BeneficialSpellVisuals.Add(spellVisual);
                        else
                            DetrimentialSpellVisuals.Add(spellVisual);
                    }
                }
            }
            Logger.WriteDebug("Generating wow spell visual data complete.");
        }

        private static void ConvertStageVisualData(ref SpellVisual spellVisual, EQSpellsEFF.SectionData effSectionData, 
            SpellVisualStageType stageType, bool isBeneficial)
        {
            // ID
            spellVisual.SpellVisualKitDBCIDsInStage[(int)stageType] = SpellVisualKitDBC.GenerateID();

            // Sound data
            string soundFileNameNoExt = GetSoundFileNameNoExtFromSoundID(effSectionData.SoundID);
            spellVisual.SoundEntryDBCIDInStage[(int)stageType] = 0;
            if (soundFileNameNoExt != string.Empty)
            {
                if (SoundsByFileNameNoExt.ContainsKey(soundFileNameNoExt) == false)
                {
                    string name = string.Concat("EQ Spell ", soundFileNameNoExt);
                    Sound sound = new Sound(name, soundFileNameNoExt, SoundType.Spell, 8, 45, false);
                    SoundsByFileNameNoExt.Add(soundFileNameNoExt, sound);
                }
                spellVisual.SoundEntryDBCIDInStage[(int)stageType] = SoundsByFileNameNoExt[soundFileNameNoExt].DBCID;
            }

            // Animation
            switch (stageType)
            {
                case SpellVisualStageType.Precast:
                    {
                        if (isBeneficial)
                            spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.ReadySpellOmni;
                        else
                            spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.ReadySpellDirected;
                    } break;
                case SpellVisualStageType.Cast:
                    {
                        if (isBeneficial)
                            spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.SpellCastOmni;
                        else
                            spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.SpellCastDirected;
                    } break;
                case SpellVisualStageType.Impact:
                    {
                        if (isBeneficial)
                            spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.None;
                        else
                            spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.None;
                    } break;
                default: Logger.WriteError("Unhanlded stagetype in ConvertStageVisualData"); break;
            }

            // Model

        }

        private static string GetSoundFileNameNoExtFromSoundID(int soundID)
        {
            switch (soundID)
            {
                case 103: return "spell_1"; // TODO: Confirm
                case 104: return "spell_2";
                case 105: return "spell_3";
                case 106: return "spell_4";
                case 107: return "spell_5";
                case 108: return "spelcast";
                case 109: return "spelgdht";
                case 110: return "spelhit1"; // TODO: Confirm
                case 111: return "spelhit2";
                case 112: return "spelhit3"; // TODO: Confirm
                case 113: return "spelhit4";
                default: return string.Empty;
            }
        }
    }
}
