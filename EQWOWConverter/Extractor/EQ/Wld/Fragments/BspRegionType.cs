using System;
using System.Collections.Generic;
using GlmSharp;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// BspRegionType (0x29)
    /// Internal Name: None
    /// Associates a list of regions with a specified region type (Water, Lava, PvP or Zoneline).
    /// </summary>
    public class BspRegionType : WldFragment
    {
        /// <summary>
        /// The region type associated with the region
        /// </summary>
        public List<RegionType> RegionTypes { get; private set; }

        public List<int> BspRegionIndices { get; private set; }
        
        public string RegionString { get; set; }
        
        public ZonelineInfo Zoneline;

        public bool PvpRegion => RegionString?.ElementAtOrDefault(2) == 'p'; 
        public bool TeleportRegion => RegionString?.ElementAtOrDefault(3) == 't' && RegionString?.ElementAtOrDefault(4) == 'p';
        public bool SlipperyRegion => RegionString?.ElementAtOrDefault(32) == 's';

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int flags = Reader.ReadInt32();
            int regionCount = Reader.ReadInt32();
        
            BspRegionIndices = new List<int>();
            for (int i = 0; i < regionCount; ++i)
            {
                BspRegionIndices.Add(Reader.ReadInt32());
            }
            
            int regionStringSize = Reader.ReadInt32();

            RegionString = regionStringSize == 0 ? Name.ToLower() :
               WldStringDecoder.DecodeString(Reader.ReadBytes(regionStringSize)).ToLower();

            RegionTypes = new List<RegionType>();

            RegionType prefixRegionType = PrefixRegionType(RegionString.Substring(0, 2));

            if (PvpRegion)
            {
                RegionTypes.Add(RegionType.Pvp);
            }

            if (TeleportRegion)
            {
                RegionTypes.Add(RegionType.Zoneline);
                DecodeZoneline();
            }

            if (SlipperyRegion)
            {
                RegionTypes.Add(RegionType.Slippery);
            }

            // This condition replicates the previous implementation where pvp/zoneline/slippery are assumed normal.
            // EQ treats pvp/zoneline/slippery as flags in addition to the region's environment type
            if (prefixRegionType == RegionType.Normal && (PvpRegion || TeleportRegion || SlipperyRegion))
            {
                // Let region flags replace normal
            }
            else
            {
                RegionTypes.Insert(0, prefixRegionType);
            }
        }

        private RegionType PrefixRegionType(string prefix)
        {
            switch (prefix)
            {
                case "dr":
                    return RegionType.Normal;
                case "wt":
                    return RegionType.Water;
                case "sl":
                    return RegionType.WaterBlockLos;
                case "la":
                    return RegionType.Lava;
                case "vw":
                    return RegionType.FreezingWater;
                case "w2":
                case "w3":
                    // Newer clients
                    return RegionType.Unknown;
                default:
                    // Technically EQ defaults to a normal fallback
                    return RegionType.Unknown;
            }
        }

        private void DecodeZoneline()
        {
            Zoneline = new ZonelineInfo();

            // TODO: Verify this
            if (RegionString == "drntp_zone")
            {
                Zoneline.Type = ZonelineType.Reference;
                Zoneline.Index = 0;
                return;
            }

            int zoneId = Convert.ToInt32(RegionString.Substring(5, 5));

            if (zoneId == 255)
            {
                int zonelineId = Convert.ToInt32(RegionString.Substring(10, 6));
                Zoneline.Type = ZonelineType.Reference;
                Zoneline.Index = zonelineId;

                return;
            }

            Zoneline.ZoneIndex = zoneId;

            float x = GetFloatFromRegionString(RegionString.Substring(10, 6));
            float y = GetFloatFromRegionString(RegionString.Substring(16, 6));
            float z = GetFloatFromRegionString(RegionString.Substring(22, 6));
            int rot = Convert.ToInt32(RegionString.Substring(28, 3));

            Zoneline.Type = ZonelineType.Absolute;
            Zoneline.Position = new vec3(x, y, z);
            Zoneline.Heading = rot;
        }

        private float GetFloatFromRegionString(string substring)
        {
            if (substring.StartsWith("-"))
            {
                return -Convert.ToSingle(substring.Substring(1, 5));
            }
            else
            {
                return Convert.ToSingle(substring);
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("BspRegionType: Region type: " + RegionTypes);
        }

        internal void LinkRegionType(List<BspRegion> bspRegions)
        {
            foreach(var regionIndex in BspRegionIndices)
            {
                bspRegions[regionIndex].SetRegionFlag(this);
            }
        }
    }
}