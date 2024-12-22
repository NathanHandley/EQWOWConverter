using System;
using System.Collections.Generic;
using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.Helpers
{
    public static class FragmentNameCleaner
    {
        private static Dictionary<Type, string> _prefixes = new Dictionary<Type, string>
        {
            // Materials
            {typeof(MaterialList), "_MP"},
            {typeof(Material), "_MDF"},
            {typeof(Mesh), "_DMSPRITEDEF"},
            {typeof(LegacyMesh), "_DMSPRITEDEF"},
            {typeof(Actor), "_ACTORDEF"},
            {typeof(SkeletonHierarchy), "_HS_DEF"},
            {typeof(TrackDefFragment), "_TRACKDEF"},
            {typeof(TrackFragment), "_TRACK"},
            {typeof(ParticleCloud), "_PCD"},
        };

        public static string CleanName(WldFragment fragment, bool toLower = true)
        {
            string cleanedName = fragment.Name;

            if(_prefixes.ContainsKey(fragment.GetType()))
            {
                cleanedName = cleanedName.Replace(_prefixes[fragment.GetType()], string.Empty);
            }

            if(toLower)
            {
                cleanedName = cleanedName.ToLower();
            }

            return cleanedName.Trim();
        }
    }
}
