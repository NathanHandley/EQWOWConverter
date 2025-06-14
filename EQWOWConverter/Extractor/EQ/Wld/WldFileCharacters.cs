using System;
using System.Collections.Generic;
using System.IO;
using LanternExtractor.EQ.Archive;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.EQ.Wld.Helpers;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld
{
    public class WldFileCharacters : WldFile
    {
        private readonly Dictionary<string, string> _animationSources = new Dictionary<string, string>();

        public WldFileCharacters(ArchiveFile wldFile, string zoneName, WldType type, ILogger logger, Settings settings,
            WldFile wldToInject = null) : base(wldFile, zoneName, type, logger, settings, wldToInject)
        {
            ParseAnimationSources();
        }

        private void ParseAnimationSources()
        {
            string filename = "ClientData/animationsources.txt";
            if (!File.Exists(filename))
            {
                Logger.LogError("WldFileCharacters: No animationsources.txt file found.");
                return;
            }

            string fileText = File.ReadAllText(filename);
            List<List<string>> parsedText = TextParser.ParseTextByDelimitedLines(fileText, ',', '#');

            foreach (var line in parsedText)
            {
                if (line.Count != 2)
                {
                    continue;
                }

                _animationSources[line[0].ToLower()] = line[1].ToLower();
            }
        }

        private string GetAnimationModelLink(string modelName)
        {
            return !_animationSources.ContainsKey(modelName) ? modelName : _animationSources[modelName];
        }

        protected override void ProcessData()
        {
            base.ProcessData();
            FindAdditionalAnimationsAndMeshes();
            BuildSlotMapping();
            FindMaterialVariants();

            if (Settings.ExportCharactersToSingleFolder)
            {
                var characterFixer = new CharacterFixer();
                characterFixer.Fix(this);
            }

            foreach (var skeleton in GetFragmentsOfType<SkeletonHierarchy>())
            {
                skeleton.BuildSkeletonData(WldType == Wld.WldType.Characters);
            }
        }

        private void BuildSlotMapping()
        {
            var materialLists = GetFragmentsOfType<MaterialList>();

            foreach (var list in materialLists)
            {
                list.BuildSlotMapping(Logger);
            }
        }

        private void FindMaterialVariants()
        {
            var materialLists = GetFragmentsOfType<MaterialList>();

            foreach (var list in materialLists)
            {
                string materialListModelName = FragmentNameCleaner.CleanName(list);

                foreach (var material in GetFragmentsOfType<Material>())
                {
                    if (material.IsHandled)
                    {
                        continue;
                    }

                    string materialName = FragmentNameCleaner.CleanName(material);

                    if (materialName.StartsWith(materialListModelName))
                    {
                        list.AddVariant(material, Logger);
                    }
                }
            }

            foreach (var material in GetFragmentsOfType<Material>())
            {
                if (material.IsHandled)
                {
                    continue;
                }

                Logger.LogWarning("WldFileCharacters: Material not assigned: " + material.Name);
            }
        }

        private void FindAdditionalAnimationsAndMeshes()
        {
            if (GetFragmentsOfType<TrackFragment>().Count == 0)
            {
                return;
            }

            var skeletons = GetFragmentsOfType<SkeletonHierarchy>();

            if (skeletons.Count == 0)
            {
                if (WldToInject == null)
                {
                    return;
                }

                skeletons = WldToInject.GetFragmentsOfType<SkeletonHierarchy>();
            }

            if (skeletons.Count == 0)
            {
                return;
            }

            foreach (var skeletonFragment in skeletons)
            {
                SkeletonHierarchy skeleton = skeletonFragment as SkeletonHierarchy;

                if (skeleton == null)
                {
                    continue;
                }

                string modelBase = skeleton.ModelBase;
                string alternateModel = GetAnimationModelLink(modelBase);

                // TODO: Alternate model bases
                foreach (var track in GetFragmentsOfType<TrackFragment>())
                {
                    if (track.IsPoseAnimation)
                    {
                        continue;
                    }

                    if (!track.IsNameParsed)
                    {
                        track.ParseTrackData(Logger);
                    }

                    string trackModelBase = track.ModelName;

                    if (trackModelBase != modelBase && alternateModel != trackModelBase)
                    {
                        continue;
                    }

                    skeleton.AddTrackData(track);
                }

                // TODO: Split to another function
                if(GetFragmentsOfType<Mesh>().Count != 0)
                {
                    foreach (var mesh in GetFragmentsOfType<Mesh>())
                    {
                        if (mesh.IsHandled)
                        {
                            continue;
                        }

                        string cleanedName = FragmentNameCleaner.CleanName(mesh);

                        string basename = cleanedName;

                        bool endsWithTwoDigits = cleanedName.Substring(cleanedName.Length - 2).All((c) => char.IsDigit(c));

                        if (endsWithTwoDigits)
                        {
                            cleanedName = cleanedName.Substring(0, cleanedName.Length - 2);

                            if (cleanedName.Length != 3)
                            {

                                cleanedName = cleanedName.Substring(0, cleanedName.Length - 2);
                            }

                            basename = cleanedName;
                        }

                        if (basename == modelBase)
                        {
                            skeleton.AddAdditionalMesh(mesh);
                        }
                    }
                }
            }

            foreach (var track in GetFragmentsOfType<TrackFragment>())
            {
                if (track.IsPoseAnimation || track.IsProcessed)
                {
                    continue;
                }

                Logger.LogWarning("WldFileCharacters: Track not assigned: " + track.Name);
            }
        }
    }
}
