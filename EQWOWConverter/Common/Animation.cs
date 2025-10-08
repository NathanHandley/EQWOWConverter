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

using EQWOWConverter.EQFiles;

namespace EQWOWConverter.Common
{
    internal class Animation
    {
        public string AnimationName = string.Empty;
        public AnimationType AnimationType;
        public EQAnimationType EQAnimationType;
        public int FrameCount = 0;
        public int TotalTimeInMS = 0;
        public List<BoneAnimationFrame> AnimationFrames = new List<BoneAnimationFrame>();

        public Animation(string animationName, AnimationType animationType, EQAnimationType eqAnimationType, int frameCount, int totalTimeInMS)
        {
            AnimationName = animationName;
            AnimationType = animationType;
            EQAnimationType = eqAnimationType;
            FrameCount = frameCount;
            TotalTimeInMS = totalTimeInMS;
        }

        public Animation(Animation other)
        {
            AnimationName = other.AnimationName;
            AnimationType = other.AnimationType;
            EQAnimationType = other.EQAnimationType;
            FrameCount = other.FrameCount;
            TotalTimeInMS = other.TotalTimeInMS;
            AnimationFrames = new List<BoneAnimationFrame>(other.AnimationFrames);
        }

        public struct BoneAnimationFrame
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

            public BoneAnimationFrame()
            {

            }

            public string GetBoneName()
            {
                string[] nameParts = BoneFullNameInPath.Split('/');
                return nameParts[nameParts.Length - 1];
            }

            public string GetParentBoneName()
            {
                string[] nameParts = BoneFullNameInPath.Split('/');
                if (nameParts.Length <= 1)
                    return string.Empty;
                return nameParts[nameParts.Length - 2];
            }
        }
        
        public Animation GenerateAsReversedVersion(string reversedAnimationName, AnimationType animationType, EQAnimationType reversedEQAnimationType)
        {
            Animation reversedAnimation = new Animation(reversedAnimationName, animationType, reversedEQAnimationType, FrameCount, TotalTimeInMS);

            // Group the frame blocks by the bone name to preserve original ordering
            Dictionary<string, List<BoneAnimationFrame>> groupedFrames = new Dictionary<string, List<BoneAnimationFrame>>();
            foreach (BoneAnimationFrame frame in AnimationFrames)
            {
                string boneName = frame.BoneFullNameInPath;
                if (groupedFrames.ContainsKey(boneName) == false)
                    groupedFrames[boneName] = new List<BoneAnimationFrame>();
                groupedFrames[boneName].Add(frame);
            }

            // Reverse the visual elements on a per-group basis
            foreach (KeyValuePair<string, List<BoneAnimationFrame>> group in groupedFrames)
            {
                List<BoneAnimationFrame> frames = group.Value;
                List<BoneAnimationFrame> reversedFrames = new List<BoneAnimationFrame>();
                for (int i = 0; i < frames.Count; i++)
                {
                    BoneAnimationFrame originalFrame = frames[i];
                    BoneAnimationFrame reversedFrame = new BoneAnimationFrame
                    {
                        BoneFullNameInPath = originalFrame.BoneFullNameInPath,
                        FrameIndex = originalFrame.FrameIndex,
                        FramesMS = originalFrame.FramesMS,
                        XPosition = frames[frames.Count - 1 - i].XPosition,
                        YPosition = frames[frames.Count - 1 - i].YPosition,
                        ZPosition = frames[frames.Count - 1 - i].ZPosition,
                        XRotation = frames[frames.Count - 1 - i].XRotation,
                        YRotation = frames[frames.Count - 1 - i].YRotation,
                        ZRotation = frames[frames.Count - 1 - i].ZRotation,
                        WRotation = frames[frames.Count - 1 - i].WRotation,
                        Scale = frames[frames.Count - 1 - i].Scale
                    };
                    reversedFrames.Add(reversedFrame);
                }
                reversedAnimation.AnimationFrames.AddRange(reversedFrames);
            }

            return reversedAnimation;
        }
    }
}
