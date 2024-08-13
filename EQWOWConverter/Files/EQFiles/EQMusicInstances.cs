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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.EQFiles
{
    internal class EQMusicInstances
    {
        public List<MusicInstance> MusicInstances = new List<MusicInstance>();

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDetail(" - Reading EQ Music Instances Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find music instances file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load the data
            string inputData = File.ReadAllText(fileFullPath);
            string[] inputRows = inputData.Split(Environment.NewLine);
            foreach (string inputRow in inputRows)
            {
                // Nothing for blank lines
                if (inputRow.Length == 0)
                    continue;

                // # = comment
                else if (inputRow.StartsWith("#"))
                    continue;

                // 9-blocks is a music instance
                else
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 9)
                    {
                        Logger.WriteError("- Music instance data is 9 components");
                        continue;
                    }
                    MusicInstance newMusicInstance = new MusicInstance();
                    newMusicInstance.CenterPosition.X = float.Parse(blocks[0]);
                    newMusicInstance.CenterPosition.Y = float.Parse(blocks[1]);
                    newMusicInstance.CenterPosition.Z = float.Parse(blocks[2]);
                    newMusicInstance.Radius = float.Parse(blocks[3]);
                    newMusicInstance.DayIndex = int.Parse(blocks[4]);
                    newMusicInstance.NightIndex = int.Parse(blocks[5]);
                    newMusicInstance.DayLoopCount = int.Parse(blocks[6]);
                    newMusicInstance.NightLoopCount = int.Parse(blocks[7]);
                    newMusicInstance.FadeOutInMS = int.Parse(blocks[8]);
                    MusicInstances.Add(newMusicInstance);
                }
            }

            Logger.WriteDetail(" - Done reading EQ Music Instances from '" + fileFullPath + "'");
            return true;
        }
    }
}
