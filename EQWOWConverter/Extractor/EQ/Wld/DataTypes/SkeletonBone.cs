using System.Collections.Generic;
using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.DataTypes
{
    /// <summary>
    /// A node in the skeleton tree
    /// </summary>
    public class SkeletonBone
    {
        public int Index;
        public string Name;
        public string FullPath;
        public string CleanedName;
        public string CleanedFullPath;
        public List<int> Children;
        public TrackFragment Track;
        public MeshReference MeshReference;
        public ParticleCloud ParticleCloud { get; set; }
        public Dictionary<string, TrackFragment> AnimationTracks { get; set; }
        public SkeletonBone Parent { get; set; }
    }
}
