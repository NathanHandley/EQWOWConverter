using System;
using System.Collections.Generic;
using System.IO;
using GlmSharp;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// TrackDefFragment (0x12)
    /// Internal name: _TRACKDEF
    /// Describes how a bone of a skeleton is rotated and shifted in relation to the parent
    /// </summary>
    public class TrackDefFragment : WldFragment
    {
        /// <summary>
        /// A list of bone positions for each frame
        /// </summary>
        public List<BoneTransform> Frames { get; set; }

        public bool IsAssigned;

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);

            Reader = new BinaryReader(new MemoryStream(data));
            Name = stringHash[-Reader.ReadInt32()];

            int flags = Reader.ReadInt32();

            // Flags are always 8 when dealing with object animations
            if (flags != 8)
            {

            }

            BitAnalyzer bitAnalyzer = new BitAnalyzer(flags);

            bool isS3dTrack2 = bitAnalyzer.IsBitSet(3);

            int frameCount = Reader.ReadInt32();

            Frames = new List<BoneTransform>();

            if (isS3dTrack2)
            {
                for (int i = 0; i < frameCount; ++i)
                {
                    Int16 rotDenominator = Reader.ReadInt16();
                    Int16 rotX = Reader.ReadInt16();
                    Int16 rotY = Reader.ReadInt16();
                    Int16 rotZ = Reader.ReadInt16();
                    Int16 shiftX = Reader.ReadInt16();
                    Int16 shiftY = Reader.ReadInt16();
                    Int16 shiftZ = Reader.ReadInt16();
                    Int16 shiftDenominator = Reader.ReadInt16();

                    BoneTransform frameTransform = new BoneTransform();

                    if (shiftDenominator != 0)
                    {
                        float x = shiftX / 256f;
                        float y = shiftY / 256f;
                        float z = shiftZ / 256f;

                        frameTransform.Scale = shiftDenominator / 256f;
                        frameTransform.Translation = new vec3(x, y, z);
                    }
                    else
                    {
                        frameTransform.Translation = vec3.Zero;
                    }

                    frameTransform.Rotation = new quat(rotX, rotY, rotZ, rotDenominator).Normalized;
                    Frames.Add(frameTransform);
                }
            }
            else
            {
                for (int i = 0; i < frameCount; ++i)
                {
                    var shiftDenominator = Reader.ReadSingle();
                    var shiftX = Reader.ReadSingle();
                    var shiftY = Reader.ReadSingle();
                    var shiftZ = Reader.ReadSingle();
                    var rotW = Reader.ReadSingle();
                    var rotX = Reader.ReadSingle();
                    var rotY = Reader.ReadSingle();
                    var rotZ = Reader.ReadSingle();

                    var frameTransform = new BoneTransform()
                    {
                        Scale = shiftDenominator,
                        Translation = new vec3(shiftX, shiftY, shiftZ),
                        Rotation = new quat(rotX, rotY, rotZ, rotW).Normalized,
                    };

                    Frames.Add(frameTransform);
                }
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("0x12: Bone frame count: " + Frames.Count);
        }
    }
}
