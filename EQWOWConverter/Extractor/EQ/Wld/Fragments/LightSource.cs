using System.Collections.Generic;
using GlmSharp;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// LightSource (0x1B)
    /// Internal name: _LIGHTDEF/_LDEF
    /// Defines color information about a light
    /// </summary>
    class LightSource : WldFragment
    {
        /// <summary>
        /// Is a placed light source if used in the light.wld and is not if used in the main zone file
        /// </summary>
        public bool IsPlacedLightSource { get; private set; }

        /// <summary>
        /// Is this a colored light? If so, the fragment size is larger
        /// </summary>
        public bool IsColoredLight { get; private set; }

        /// <summary>
        /// The color of the light - if it is colored
        /// </summary>
        public vec4 Color { get; private set; }

        /// <summary>
        /// The attenuation (?) - guess from Windcatcher. Not sure what it is.
        /// </summary>
        public int Attenuation { get; private set; }
        public float SomeValue { get; private set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int flags = Reader.ReadInt32();
            var bitAnalyzer = new BitAnalyzer(flags);

            if (bitAnalyzer.IsBitSet(1))
            {
                IsPlacedLightSource = true;
            }

            if (bitAnalyzer.IsBitSet(4))
            {
                IsColoredLight = true;
            }

            if (!IsPlacedLightSource)
            {
                if (!IsColoredLight)
                {
                    int something1 = Reader.ReadInt32();
                    SomeValue = Reader.ReadSingle();
                    return;
                }

                Attenuation = Reader.ReadInt32();

                float alpha = Reader.ReadSingle();
                float red = Reader.ReadSingle();
                float green = Reader.ReadSingle();
                float blue = Reader.ReadSingle();
                Color = new vec4(red, green, blue, alpha);

                if (Attenuation != 1)
                {
                        
                }

                return;
            }

            if (!IsColoredLight)
            {
                int something1 = Reader.ReadInt32();
                float something2 = Reader.ReadSingle();
                return;
            }
            
            // Not sure yet what the purpose of this fragment is in the main zone file
            // For now, return
            if (!IsPlacedLightSource && Name == "DEFAULT_LIGHTDEF")
            {
                int unknown = Reader.ReadInt32();
                float unknown6 = Reader.ReadSingle();
                return;
            }

            int unknown1 = Reader.ReadInt32();

            if (!IsColoredLight)
            {
                int unknown = Reader.ReadInt32();
                Color = new vec4(1.0f);
                int unknown2 = Reader.ReadInt32();
                int unknown3 = Reader.ReadInt32();

            }
            else
            {
                Attenuation = Reader.ReadInt32();

                float alpha = Reader.ReadSingle();
                float red = Reader.ReadSingle();
                float green = Reader.ReadSingle();
                float blue = Reader.ReadSingle();

                Color = new vec4(red, green, blue, alpha);
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("LightSource: Is a placed light: " + IsPlacedLightSource);
            logger.LogInfo("LightSource: Is a colored light: " + IsColoredLight);

            if (IsColoredLight)
            {
                logger.LogInfo("LightSource: Color: " + Color);
                logger.LogInfo("LightSource: Attenuation (?): " + Attenuation);
            }
        }
    }
}