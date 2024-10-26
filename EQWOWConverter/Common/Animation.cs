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
    internal class Animation
    {
        public AnimationType AnimationType;
        public int FrameCount = 0;
        public int TotalTimeInMS = 0;

        public Animation(AnimationType animationType, int frameCount, int totalTimeInMS)
        {
            AnimationType = animationType;
            FrameCount = frameCount;
            TotalTimeInMS = totalTimeInMS;
        }

        public class EQBoneAnimationFrame
        {
            public string BoneFullNameInPath = string.Empty;
            public int FrameIndex = 0;
            public float XPosition = 0;
            public float YPosition = 0;
            public float ZPosition = 0;
            public float XRotation = 0;
            public float YRotation = 0;
            public float ZRotation = 0;
            public float WRotation = 0;
            public float Scale = 0;
            public int FramesMS = 0;
        }

        public List<EQBoneAnimationFrame> AnimationFrames = new List<EQBoneAnimationFrame>();
    }
}
