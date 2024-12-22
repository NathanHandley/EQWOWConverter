using System.Collections.Generic;
using System.IO;
using GlmSharp;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.Infrastructure;

namespace LanternExtractor.EQ.Wld.DataTypes
{
    public class RenderInfo
    {
        public int Flags { get; set; }
        public int Pen { get; set; }
        public float Brightness { get; set; }
        public float ScaledAmbient { get; set; }
        public BitmapInfoReference SimpleSpriteReference { get; set; }
        public UvInfo UvInfo { get; set; }
        public List<vec2> UvMap { get; set; }

        public static RenderInfo Parse(BinaryReader reader, List<WldFragment> fragments)
        {
            var renderInfo = new RenderInfo
            {
                Flags = reader.ReadInt32()
            };

            var ba = new BitAnalyzer(renderInfo.Flags);

            var hasPen = ba.IsBitSet(0);
            var hasBrightness = ba.IsBitSet(1);
            var hasScaledAmbient = ba.IsBitSet(2);
            var hasSimpleSprite = ba.IsBitSet(3);
            var hasUvInfo = ba.IsBitSet(4);
            var hasUvMap = ba.IsBitSet(5);
            var isTwoSided = ba.IsBitSet(6);

            if (hasPen)
            {
                renderInfo.Pen = reader.ReadInt32();
            }

            if (hasBrightness)
            {
                renderInfo.Brightness = reader.ReadSingle();
            }

            if (hasScaledAmbient)
            {
                renderInfo.ScaledAmbient = reader.ReadSingle();
            }

            if (hasSimpleSprite)
            {
                var fragmentRef = reader.ReadInt32();
                renderInfo.SimpleSpriteReference = fragments[fragmentRef - 1] as BitmapInfoReference;
            }

            if (hasUvInfo)
            {
                renderInfo.UvInfo = new UvInfo
                {
                    UvOrigin = new vec3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    UAxis = new vec3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    VAxis = new vec3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                };
            }

            if (hasUvMap)
            {
                var uvMapCount = reader.ReadInt32();
                renderInfo.UvMap = new List<vec2>();
                for (var i = 0; i < uvMapCount; i++)
                {
                    renderInfo.UvMap.Add(new vec2(reader.ReadSingle(), reader.ReadSingle()));
                }
            }

            return renderInfo;
        }
    }
}
