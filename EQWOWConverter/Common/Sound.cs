//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class Sound
    {
        private static int CURRENT_SOUNDENTRY_ID = Configuration.CONFIG_DBCID_SOUNDENTRIES_ID_START;

        public int DBCID;
        public string Name = string.Empty;
        public string AudioFileNameNoExt = string.Empty;
        public SoundType Type = SoundType.None;
        public float Volume;
        public bool Loop = false;
        public float MinDistance;
        public float DistanceCutoff;

        public Sound(string name, string audioFileName, float volume, SoundType type, float minDistance, float distanceCutoff, bool loop)
        {
            DBCID = CURRENT_SOUNDENTRY_ID;
            CURRENT_SOUNDENTRY_ID++;
            Name = name;
            AudioFileNameNoExt = audioFileName;
            Volume = volume;
            Type = type;
            MinDistance = minDistance;
            DistanceCutoff = distanceCutoff;
            Loop = loop;
        }

        public float GetVolume()
        {
            if (Type == SoundType.GameObject || Type == SoundType.ZoneAmbience)
            {
                float volume = Volume;
                switch (AudioFileNameNoExt.ToLower().Trim())
                {
                    case "bigbell": volume = 0.8912509f; break; // Test: Fearplane
                    case "darkwds1": volume = 0.13931568f; break;
                    case "darkwds2": volume = 0.098401114f; break;
                    case "cauldron": volume = 0.8912509f; break; // Test: Qeynos
                    case "caveloop": volume = 0.23442288f; break; // Test: Befallen
                    case "clock": volume = 0.8912509f; break; // Test: Qeynos
                    case "dockbell": volume = 0.8912509f; break; // Test: Qeynos
                    case "fire_lp": volume = 0.8912509f; break; // Test: Cauldron, Cazic
                    case "flagloop": volume = 0.8912509f; break; // Test: Butcher, oot
                    case "lakelap1": volume = 0.8912509f; break; // Test: Qeynos, Cazicthule
                    case "lava_lp": volume = 0.8912509f; break; // Test: Lavastorm
                    case "lava2_lp": volume = 0.8912509f; break; // Test: Lavastorm
                    case "night": volume = 0.2786121f; break; // Test: freportw
                    case "ocean": volume = 0.24445728f; break; // Test: erudsxing, lakerathe, oasis, oot (0.39445728), nro (0.8912509)
                    case "oceanlap": volume = 0.8912509f; break; // Test: Butcher, Erudnext
                    case "oceanwav": volume = 0.8912509f; break; // Test: freporte
                    case "rumblelp": volume = 0.8912509f; break; // Test: Blackburrow
                    case "silence": volume = 0f; break;
                    case "slmefall": volume = 0.8912509f; break; // Test: Runnyeye
                    case "slmeloop": volume = 0.8912509f; break; // Test: Runnyeye, CazicThule
                    case "slmestrm": volume = 1f; break; // TBD Test: Cazicthule, Runnyeye
                    case "space": volume = 0.25f; break; // Test: Butcher, Nektulos, rathemnt (0.8912509), qeynos2
                    case "spinnrlp": volume = 0.36f; break; // PASS: Oggok, Steamfont (0.8912509)
                    case "steamlp": volume = 0.8912509f; break; // Test: Akanon, Oggok (0.91201085)
                    case "streamlg": volume = 0.8912509f; break; // Test: Eastkarana
                    case "streammd": volume = 0.8912509f; break; // TBD Test: Everfrost
                    case "streamsm": volume = 0.8912509f; break; // Test: Crushbone
                    case "swmp1": volume = 0.3f; break; // Test: Feerrott, Grobb (0.7897689), Kithicor (0.8912509)
                    case "swmp2": volume = 0.8912509f; break; // Test: Feerrott, Innothule
                    case "swmp3": volume = 0.6637431f; break; // Test: Feerrott, Kithicor (0.9278975)
                    case "thunder1": volume = 0.15f; break; // Test: Qeynos (0.33151257)
                    case "thunder2": volume = 0.15f; break; // Test: qeynos (0.33151257)
                    case "torch3d": volume = 0.8912509f; break; // Test: Beholder, Commons
                    case "torch_lp": volume = 0.165577f; break; // Test: Butcher, Cazicthule
                    case "wfall_lg": volume = 0.5f; break; // Test: Akanon (0.8912509)
                    case "wfall_md": volume = 0.5f; break; // Test: Akanon, Erudnext (0.8912509)
                    case "wind_lp1": volume = 0.13931568f; break; // PASS: Kithicor - Volume good
                    case "wind_lp2": volume = 0.13931568f; break; // Test: Cauldron
                    case "wind_lp3": volume = 0.13931568f; break; // Test: Commons
                    case "wind_lp4": volume = 0.098401114f; break; // Test: Beholder
                    case "wtr_pool": volume = 0.20f; break; // PASS: Akanon, Cazic, Commons (0.8912509)
                    default:
                        {
                            Logger.WriteInfo("No static defined volume for sound with file name '" + AudioFileNameNoExt + "'");
                        }
                        break;
                }
                if (Type == SoundType.GameObject)
                    return volume * Configuration.CONFIG_AUDIO_SOUNDINSTANCE_VOLUME_MOD;
                else
                    return volume * Configuration.CONFIG_AUDIO_AMBIENT_SOUND_VOLUME_MOD;
            }

            return Volume;
        }
    }
}
