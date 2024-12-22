using System;
using System.Collections.Generic;
using LanternExtractor.EQ.Wld.Helpers;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// TrackFragment (0x13)
    /// Internal name: _TRACK
    /// Reference to a TrackDefFragment, a bone in a skeleton
    /// </summary>
    public class TrackFragment : WldFragment
    {
        /// <summary>
        /// Reference to a skeleton piece
        /// </summary>
        public TrackDefFragment TrackDefFragment { get; set; }

        public bool IsPoseAnimation { get; set; }
        public bool IsProcessed { get; set; }

        public int FrameMs { get; set; }

        public string ModelName;
        public string AnimationName;
        public string PieceName;

        public bool IsNameParsed;

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];

            int reference = Reader.ReadInt32();
            TrackDefFragment = fragments[reference - 1] as TrackDefFragment;

            if (TrackDefFragment == null)
            {
                logger.LogError("Bad track def reference'");
            }

            // Either 4 or 5 - maybe something to look into
            // Bits are set 0, or 2. 0 has the extra field for delay.
            // 2 doesn't have any additional fields.
            int flags = Reader.ReadInt32();

            BitAnalyzer bitAnalyzer = new BitAnalyzer(flags);
            FrameMs = bitAnalyzer.IsBitSet(0) ? Reader.ReadInt32() : 0;

            if (Reader.BaseStream.Position != Reader.BaseStream.Length)
            {

            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);

            if (TrackDefFragment != null)
            {
                logger.LogInfo("-----");
                logger.LogInfo("0x13: Skeleton piece reference: " + TrackDefFragment.Index + 1);
            }
        }

        public void SetTrackData(string modelName, string animationName, string pieceName)
        {
            ModelName = modelName;
            AnimationName = animationName;
            PieceName = pieceName;
        }

        /// <summary>
        /// This is only ever called when we are finding additional animations.
        /// All animations that are not the default skeleton animations:
        /// 1. Start with a 3 letter animation abbreviation (e.g. C05)
        /// 2. Continue with a 3 letter model name
        /// 3. Continue with the skeleton piece name
        /// 4. End with _TRACK
        /// </summary>
        /// <param name="logger"></param>
        public void ParseTrackData(ILogger logger)
        {
            string cleanedName = FragmentNameCleaner.CleanName(this, true);

            if (cleanedName.Length < 6)
            {
                if (cleanedName.Length == 3)
                {
                    ModelName = cleanedName;
                    IsNameParsed = true;
                    return;
                }

                ModelName = cleanedName;
                return;
            }

            // Equipment edge case
            if (cleanedName.Substring(0, 3) == cleanedName.Substring(3, 3))
            {
                AnimationName = cleanedName.Substring(0, 3);
                ModelName = cleanedName.Substring(Math.Min(7, cleanedName.Length));
                PieceName = "root";
                IsNameParsed = true;
                return;
            }

            AnimationName = cleanedName.Substring(0, 3);
            cleanedName = cleanedName.Remove(0, 3);
            ModelName = cleanedName.Substring(0, 3);
            cleanedName = cleanedName.Remove(0, 3);
            PieceName = cleanedName;

            IsNameParsed = true;
            //logger.LogError($"Split into, {AnimationName} {ModelName} {PieceName}");
        }

        public void ParseTrackDataEquipment(SkeletonHierarchy skeletonHierarchy, ILogger logger)
        {
            string cleanedName = FragmentNameCleaner.CleanName(this, true);

            // Equipment edge case
            if (cleanedName == skeletonHierarchy.ModelBase && cleanedName.Length > 6 || cleanedName.Substring(0, 3) == cleanedName.Substring(3, 3))
            {
                AnimationName = cleanedName.Substring(0, 3);
                ModelName = cleanedName.Substring(7);
                PieceName = "root";
                IsNameParsed = true;
                return;
            }

            AnimationName = cleanedName.Substring(0, 3);
            cleanedName = cleanedName.Remove(0, 3);
            ModelName = skeletonHierarchy.ModelBase;
            cleanedName = cleanedName.Replace(skeletonHierarchy.ModelBase, string.Empty);
            PieceName = cleanedName;
            IsNameParsed = true;
        }
    }
}