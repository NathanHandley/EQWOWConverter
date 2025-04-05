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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.EQFiles
{
    internal class EQSound2DInstances
    {
        public List<SoundInstance> SoundInstances = new List<SoundInstance>();

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDebug(" - Reading 2D Sound Instances Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find 2D Sound Instances file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load the data
            string inputData = FileTool.ReadAllDataFromFile(fileFullPath);
            string[] inputRows = inputData.Split(Environment.NewLine);
            foreach (string inputRow in inputRows)
            {
                // Nothing for blank lines
                if (inputRow.Length == 0)
                    continue;

                // # = comment
                else if (inputRow.StartsWith("#"))
                    continue;

                // 11-blocks is a light instance
                else
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 11)
                    {
                        Logger.WriteError("- 2D Sound instance data is 11 components");
                        continue;
                    }

                    SoundInstance newSoundInstance = new SoundInstance();
                    newSoundInstance.Position.X = float.Parse(blocks[0]);
                    newSoundInstance.Position.Y = float.Parse(blocks[1]);
                    newSoundInstance.Position.Z = float.Parse(blocks[2]);
                    newSoundInstance.Radius = int.Parse(blocks[3]);
                    newSoundInstance.SoundFileNameDayNoExt = blocks[4];
                    newSoundInstance.SoundFileNameNightNoExt = blocks[5];
                    newSoundInstance.CooldownInMSDay = int.Parse(blocks[6]);
                    newSoundInstance.CooldownInMSNight = int.Parse(blocks[7]);
                    newSoundInstance.CooldownInMSRandom = int.Parse(blocks[8]);
                    newSoundInstance.VolumeDay = float.Parse(blocks[9]);
                    newSoundInstance.VolumeNight = float.Parse(blocks[10]);
                    newSoundInstance.Is2DSound = true;
                    SoundInstances.Add(newSoundInstance);
                }
            }

            Logger.WriteDebug(" - Done reading 2D Sound Instances from '" + fileFullPath + "'");
            return true;
        }
    }
}
