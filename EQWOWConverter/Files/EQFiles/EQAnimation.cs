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
using static EQWOWConverter.EQFiles.EQSkeleton;

namespace EQWOWConverter.EQFiles
{
    internal class EQAnimation
    {
        public Animation Animation = new Animation();

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDetail(" - Reading EQ Animation Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find EQ Animation file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load the core data
            Animation = new Animation();
            string inputData = File.ReadAllText(fileFullPath);
            string[] inputRows = inputData.Split(Environment.NewLine);

            // Make sure there is the minimum number of rows
            if (inputRows.Length < 4)
            {
                Logger.WriteError("- Could not load EQ Animation file because there were less than 4 rows at '" + fileFullPath + "'");
                return false;
            }

            foreach (string inputRow in inputRows)
            {
                // Nothing for blank lines
                if (inputRow.Length == 0)
                    continue;

                // # = comment
                else if (inputRow.StartsWith("#"))
                    continue;

                // Get the blocks for this row
                string[] blocks = inputRow.Split(",");
                if (blocks.Length == 0)
                    continue;

                // Number of frames
                if (blocks[0] == "framecount")
                {
                    Animation.FrameCount = int.Parse(blocks[1]);
                    continue;
                }

                // Total time of animation
                if (blocks[0] == "totalTimeMs")
                {
                    Animation.TotalTimeInMS = int.Parse(blocks[1]);
                    continue;
                }

                // Ensure it's an animation block
                if (blocks.Length != 11)
                {
                    Logger.WriteError("EQ Animation Frame data must be 11 components");
                    continue;
                }

                Animation.EQBoneAnimationFrame animationFrame = new Animation.EQBoneAnimationFrame();
                animationFrame.BoneFullNameInPath = blocks[0];
                animationFrame.FrameIndex = int.Parse(blocks[1]);
                animationFrame.XPosition = float.Parse(blocks[2]);
                animationFrame.YPosition = float.Parse(blocks[3]);
                animationFrame.ZPosition = float.Parse(blocks[4]);
                animationFrame.XRotation = float.Parse(blocks[5]);
                animationFrame.YRotation = float.Parse(blocks[6]);
                animationFrame.ZRotation = float.Parse(blocks[7]);
                animationFrame.WRotation = float.Parse(blocks[8]);
                animationFrame.Scale = float.Parse(blocks[9]);
                animationFrame.FramesMS = int.Parse(blocks[10]);
                Animation.AnimationFrames.Add(animationFrame);
            }

            Logger.WriteDetail(" - Done reading EQ Animation Data from '" + fileFullPath + "'");
            return true;
        }
    }
}
