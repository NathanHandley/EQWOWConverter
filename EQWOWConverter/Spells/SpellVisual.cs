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
using EQWOWConverter.EQFiles;

namespace EQWOWConverter.Spells
{
    internal class SpellVisual
    {
        enum SpellVisualStageType : int
        {
            Precast,
            Cast,
            Impact
        }

        private static EQSpellsEFF? EQSpellsEFF = null;
        private static readonly object SpellVisualLock = new object();
        private static List<SpellVisual> SpellVisuals = new List<SpellVisual>();
        public static Dictionary<string, Sound> SoundsByFileNameNoExt = new Dictionary<string, Sound>();

        public int[] AnimationIDInStage = new int[3];
        public int[] SoundEntryDBCIDInStage = new int[3];

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
                    EQSpellsEFF.EQSpellEffect spellEffect = EQSpellsEFF.SpellEffects[i];
                    SpellVisual spellVisual = new SpellVisual();

                    // Source data
                    EQSpellsEFF.SectionData sourceSectionData = spellEffect.SectionDatas[0];
                    ConvertStageVisualData(ref spellVisual, sourceSectionData, SpellVisualStageType.Precast);                    

                    // Sprite data (missle/projectile?)
                    EQSpellsEFF.SectionData spriteSectionData = spellEffect.SectionDatas[1];
                    ConvertStageVisualData(ref spellVisual, spriteSectionData, SpellVisualStageType.Cast);

                    // Target data
                    EQSpellsEFF.SectionData targetSectionData = spellEffect.SectionDatas[2];
                    ConvertStageVisualData(ref spellVisual, targetSectionData, SpellVisualStageType.Impact);

                    SpellVisuals.Add(spellVisual);
                }
            }
            Logger.WriteDebug("Generating wow spell visual data complete.");
        }

        private static void ConvertStageVisualData(ref SpellVisual spellVisual, EQSpellsEFF.SectionData effSectionData, SpellVisualStageType stageType)
        {
            // Sound data
            string soundFileNameNoExt = GetSoundFileNameNoExtFromSoundID(effSectionData.SoundID);
            spellVisual.SoundEntryDBCIDInStage[(int)stageType] = 0;
            if (soundFileNameNoExt != string.Empty)
            {
                if (SoundsByFileNameNoExt.ContainsKey(soundFileNameNoExt) == true)
                    spellVisual.SoundEntryDBCIDInStage[(int)stageType] = SoundsByFileNameNoExt[soundFileNameNoExt].DBCID;
                else
                {
                    string name = string.Concat("EQ Spell ", soundFileNameNoExt);
                    Sound sound = new Sound(name, soundFileNameNoExt, SoundType.Casting, 8, 45, false);
                    SoundsByFileNameNoExt.Add(soundFileNameNoExt, sound);
                }
            }
        }

        private static string GetSoundFileNameNoExtFromSoundID(int soundID)
        {
            switch (soundID)
            {
                case 103: return "spell_1.wav"; // TODO: Confirm
                case 104: return "spell_2.wav";
                case 105: return "spell_3.wav";
                case 106: return "spell_4.wav";
                case 107: return "spell_5.wav";
                case 108: return "spelcast.wav";
                case 109: return "spelgdht.wav";
                case 110: return "spellhit1.wav"; // TODO: Confirm
                case 111: return "spellhit2.wav";
                case 112: return "spellhit3.wav"; // TODO: Confirm
                case 113: return "spellhit4.wav";
                default: return string.Empty;
            }
        }

        private static int GetCasterAnimationIDByEffectIndex(int effectIndex)
        {
            return 0;
        }
    }
}
