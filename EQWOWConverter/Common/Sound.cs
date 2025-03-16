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
            DBCID = CURRENT_SOUNDENTRY_ID;
            CURRENT_SOUNDENTRY_ID++;
            Name = name;
            AudioFileNameNoExt = audioFileNameNoExt;
            Type = type;
            MinDistance = minDistance;
            DistanceCutoff = distanceCutoff;
            Loop = loop;
            Volume = volume;
        }

        public float GetVolume()
        {
            float volume = Volume;
            if (Type == SoundType.GameObject || Type == SoundType.ZoneAmbience)
            {
                switch (AudioFileNameNoExt.ToLower().Trim())
                {
                    case "bigbell": volume = 0.2f; break;
                    case "darkwds1": volume = 0.2f; break;
                    case "darkwds2": volume = 0.2f; break;
                    case "cauldron": volume = 0.15f; break;
                    case "caveloop": volume = 0.2f; break;
                    case "clock": volume = 0.2f; break;
                    case "dockbell": volume = 0.2f; break;
                    case "doorst_o": volume = 0.2f; break;
                    case "doorst_c": volume = 0.2f; break;
                    case "elevloop": volume = 0.25f; break;
                    case "fire_lp": volume = 0.15f; break;
                    case "flagloop": volume = 0.2f; break;
                    case "lakelap1": volume = 0.2f; break;
                    case "lava_lp": volume = 0.25f; break;
                    case "lava2_lp": volume = 0.25f; break;
                    case "night": volume = 0.2f; break;
                    case "ocean": volume = 0.2f; break;
                    case "oceanlap": volume = 0.2f; break;
                    case "oceanwav": volume = 0.2f; break;
                    case "rumblelp": volume = 0.2f; break;
                    case "silence": volume = 0f; break;
                    case "slmefall": volume = 0.2f; break;
                    case "slmeloop": volume = 0.2f; break;
                    case "slmestrm": volume = 0.15f; break;
                    case "space": volume = 0.25f; break;
                    case "spinnrlp": volume = 0.36f; break;
                    case "steamlp": volume = 0.18f; break;
                    case "streamlg": volume = 0.2f; break;
                    case "streammd": volume = 0.15f; break;
                    case "streamsm": volume = 0.2f; break;
                    case "swmp1": volume = 0.2f; break;
                    case "swmp2": volume = 0.2f; break;
                    case "swmp3": volume = 0.2f; break;
                    case "thunder1": volume = 0.2f; break;
                    case "thunder2": volume = 0.2f; break;
                    case "torch3d": volume = 0.15f; break;
                    case "torch_lp": volume = 0.2f; break;
                    case "wfall_lg": volume = 0.2f; break;
                    case "wfall_md": volume = 0.2f; break;
                    case "wind_lp1": volume = 0.2f; break;
                    case "wind_lp2": volume = 0.2f; break;
                    case "wind_lp3": volume = 0.2f; break;
                    case "wind_lp4": volume = 0.2f; break;
                    case "wtr_pool": volume = 0.2f; break;
                    default:
                        {
                            Logger.WriteInfo("No static defined volume for sound with file name '" + AudioFileNameNoExt + "'");
                        }
                        break;
                }
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

            Logger.WriteError("Type of SoundType is not handled, so volume will be 1");
            return volume;
        }
    }
}
