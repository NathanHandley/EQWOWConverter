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

namespace EQWOWConverter.Common
{
    internal class Sound
    {
        private static int CURRENT_SOUNDENTRY_ID = Configuration.DBCID_SOUNDENTRIES_ID_START;
        private static readonly object SoundLock = new object();
        private static Dictionary<string, float> AmbientSoundVolumesByAudioFileNameNoExt = new Dictionary<string, float>();
        private static Dictionary<string, string> SoundFileNameRemapsBySourceName = new Dictionary<string, string>();

        public int DBCID;
        public string Name = string.Empty;
        public string AudioFileNameNoExt = string.Empty;
        public SoundType Type = SoundType.None;
        public bool Loop = false;
        public bool NoOverlap = false;
        private float Volume;
        public float MinDistance;
        public float DistanceCutoff;

        public Sound(string name, string audioFileNameNoExt, SoundType type, float minDistance, float distanceCutoff, bool loop, float volume = 1f)
        {
            lock (SoundLock)
            {
                if (AmbientSoundVolumesByAudioFileNameNoExt.Count == 0)
                    PopulateSoundPropertiesFromFile();

                DBCID = CURRENT_SOUNDENTRY_ID;
                CURRENT_SOUNDENTRY_ID++;
                Name = name;
                AudioFileNameNoExt = audioFileNameNoExt;
                if (SoundFileNameRemapsBySourceName.ContainsKey(AudioFileNameNoExt.Trim().ToLower()) == true)
                    AudioFileNameNoExt = SoundFileNameRemapsBySourceName[AudioFileNameNoExt.Trim().ToLower()];
                Type = type;
                MinDistance = minDistance;
                DistanceCutoff = distanceCutoff;
                Loop = loop;
                Volume = volume;
            }
        }

        private static void PopulateSoundPropertiesFromFile()
        {
            string volumeFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ZoneSounds.csv");
            Logger.WriteDebug("Populating ambient sound volumes via file '" + volumeFile + "'");
            List<Dictionary<string, string>> volumeRows = FileTool.ReadAllRowsFromFileWithHeader(volumeFile, "|");
            foreach (Dictionary<string, string> volumeRow in volumeRows)
            {
                string fileName = volumeRow["SoundFileNameNoExt"].ToLower().Trim();
                float volume = Convert.ToSingle(volumeRow["AmbientVolume"]);
                AmbientSoundVolumesByAudioFileNameNoExt.Add(fileName, volume);
                string remapFileName = volumeRow["FileNameRemap"].ToLower().Trim();
                if (remapFileName.Length > 0)
                    SoundFileNameRemapsBySourceName.Add(fileName, remapFileName);
            }
        }

        public float GetVolume()
        {
            float volume = Volume;
            if (Type == SoundType.GameObject || Type == SoundType.ZoneAmbience)
            {
                // Volume
                if (AmbientSoundVolumesByAudioFileNameNoExt.ContainsKey(AudioFileNameNoExt.ToLower().Trim()) == true)
                    volume = AmbientSoundVolumesByAudioFileNameNoExt[AudioFileNameNoExt.ToLower().Trim()];
                else
                    Logger.WriteInfo("No static defined volume for sound with file name '" + AudioFileNameNoExt + "'");
                    
                // Mods
                if (Type == SoundType.GameObject)
                    return volume * Configuration.AUDIO_SOUNDINSTANCE_VOLUME_MOD;
                else
                    return volume * Configuration.AUDIO_AMBIENT_SOUND_VOLUME_MOD;
            }
            else if (Type == SoundType.ZoneMusic)
            {
                return volume * Configuration.AUDIO_MUSIC_VOLUME_MOD;
            }
            else if (Type == SoundType.NPCCombat)
            {
                return Configuration.AUDIO_CREATURE_SOUND_VOLUME;
            }
            else if (Type == SoundType.Spell)
            {
                return Configuration.AUDIO_SPELL_SOUND_VOLUME;
            }

            Logger.WriteError("Type of SoundType is not handled, so volume will be 1");
            return volume;
        }            
    }
}
