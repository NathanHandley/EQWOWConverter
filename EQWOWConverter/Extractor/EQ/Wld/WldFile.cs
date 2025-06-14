using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LanternExtractor.EQ.Archive;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.EQ.Wld.Exporters;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.EQ.Wld.Helpers;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld
{
    /// <summary>
    /// Contains shared logic for loading and extracting data from a WLD file
    /// </summary>
    public abstract class WldFile
    {
        public string RootExportFolder;
        public string ZoneShortname => ZoneName;

        /// <summary>
        /// The type of WLD file this is
        /// </summary>
        public WldType WldType { get; }

        /// <summary>
        /// A link of indices to fragments
        /// </summary>
        protected List<WldFragment> Fragments;

        /// <summary>
        /// The string has containing the index in the hash and the decoded string that is there
        /// </summary>
        private Dictionary<int, string> _stringHash;

        /// <summary>
        /// A collection of fragment lists that can be referenced by a fragment type
        /// </summary>
        //protected Dictionary<FragmentType, List<WldFragment>> _fragmentTypeDictionary;
        protected Dictionary<Type, List<WldFragment>> FragmentTypeDictionary;

        /// <summary>
        /// A collection of fragment lists that can be referenced by a fragment type
        /// </summary>
        protected Dictionary<string, WldFragment> FragmentNameDictionary;

        protected List<BspRegion> BspRegions;

        /// <summary>
        /// The shortname of the zone this WLD is from
        /// </summary>
        protected readonly string ZoneName;

        /// <summary>
        /// The logger to use to output WLD information
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The WLD file found in the archive
        /// </summary>
        private readonly ArchiveFile _wldFile;

        /// <summary>
        /// Cached settings
        /// </summary>
        protected readonly Settings Settings;

        /// <summary>
        /// Is this the new WLD format? Some data types are different
        /// </summary>
        private bool _isNewWldFormat;

        protected readonly WldFile WldToInject;


        public Dictionary<string, string> FilenameChanges = new Dictionary<string, string>();

        /// <summary>
        /// Constructor setting data references used during the initialization process
        /// </summary>
        /// <param name="wldFile">The WLD file bytes contained in the PFS file</param>
        /// <param name="zoneName">The shortname of the zone</param>
        /// <param name="type">The type of WLD - used to determine what to extract</param>
        /// <param name="logger">The logger used for debug output</param>
        protected WldFile(ArchiveFile wldFile, string zoneName, WldType type, ILogger logger, Settings settings,
            WldFile fileToInject)
        {
            _wldFile = wldFile;
            ZoneName = zoneName.ToLower();
            WldType = type;
            Logger = logger;
            Settings = settings;
            WldToInject = fileToInject;
        }

        /// <summary>
        /// Initializes and instantiates the WLD file
        /// </summary>
        public virtual bool Initialize(string rootFolder, bool exportData = true)
        {
            RootExportFolder = rootFolder;
            Logger.LogInfo("Extracting WLD archive: " + _wldFile.Name);
            Logger.LogInfo("-----------------------------------");
            Logger.LogInfo("WLD type: " + WldType);

            Fragments = new List<WldFragment>();
            FragmentTypeDictionary = new Dictionary<Type, List<WldFragment>>();
            FragmentNameDictionary = new Dictionary<string, WldFragment>();
            BspRegions = new List<BspRegion>();

            var reader = new BinaryReader(new MemoryStream(_wldFile.Bytes));

            byte[] writeBytes = reader.ReadBytes(_wldFile.Bytes.Length);
            reader.BaseStream.Position = 0;
            var writer = new BinaryWriter(new MemoryStream(writeBytes));

            int identifier = reader.ReadInt32();

            if (identifier != WldIdentifier.WldFileIdentifier)
            {
                Logger.LogError("Not a valid WLD file!");
                return false;
            }

            int version = reader.ReadInt32();

            switch (version)
            {
                case WldIdentifier.WldFormatOldIdentifier:
                    break;
                case WldIdentifier.WldFormatNewIdentifier:
                    _isNewWldFormat = true;
                    Logger.LogWarning("New WLD format not fully supported.");
                    break;
                default:
                    Logger.LogError("Unrecognized WLD format.");
                    return false;
            }

            uint fragmentCount = reader.ReadUInt32();
            uint bspRegionCount = reader.ReadUInt32();
            // Should contain 0x000680D4
            int unknown = reader.ReadInt32();
            uint stringHashSize = reader.ReadUInt32();
            int unknown2 = reader.ReadInt32();

            byte[] stringHash = reader.ReadBytes((int)stringHashSize);

            ParseStringHash(WldStringDecoder.DecodeString(stringHash));

            for (int i = 0; i < fragmentCount; ++i)
            {
                uint fragSize = reader.ReadUInt32();
                int fragId = reader.ReadInt32();

                var newFragment = !WldFragmentBuilder.Fragments.ContainsKey(fragId)
                    ? new Generic()
                    : WldFragmentBuilder.Fragments[fragId]();

                if (newFragment is Generic)
                {
                    Logger.LogWarning($"WldFile: Unhandled fragment type: {fragId:x}");
                }

                newFragment.Initialize(i, (int)fragSize, reader.ReadBytes((int)fragSize), Fragments, _stringHash,
                    _isNewWldFormat,
                    Logger);
                newFragment.OutputInfo(Logger);

                AddFragment(newFragment);
            }

            Logger.LogInfo("-----------------------------------");
            Logger.LogInfo("WLD extraction complete");

            ProcessData();

            if (exportData)
            {
                ExportData();
            }

            return true;
        }

        public List<T> GetFragmentsOfType<T>() where T : WldFragment
        {
            if (!FragmentTypeDictionary.ContainsKey(typeof(T)))
            {
                return new List<T>();
            }

            return FragmentTypeDictionary[typeof(T)].Cast<T>().ToList();
        }

        public T GetFragmentByName<T>(string fragmentName) where T : WldFragment
        {
            if (!FragmentNameDictionary.ContainsKey(fragmentName))
            {
                return default(T);
            }

            return FragmentNameDictionary[fragmentName] as T;
        }

        protected void AddFragment(WldFragment fragment)
        {
            Fragments.Add(fragment);

            if (!FragmentTypeDictionary.ContainsKey(fragment.GetType()))
            {
                FragmentTypeDictionary[fragment.GetType()] = new List<WldFragment>();
            }

            if (!string.IsNullOrEmpty(fragment.Name) && !FragmentNameDictionary.ContainsKey(fragment.Name))
            {
                FragmentNameDictionary[fragment.Name] = fragment;
            }

            FragmentTypeDictionary[fragment.GetType()].Add(fragment);
        }

        protected virtual void ProcessData()
        {
            BuildSkeletonData();
            MaterialFixer.Fix(this);
        }


        private void ParseStringHash(string decodedHash)
        {
            _stringHash = new Dictionary<int, string>();
            int index = 0;
            string[] splitHash = decodedHash.Split('\0');

            foreach (var hashString in splitHash)
            {
                _stringHash[index] = hashString;

                // Advance the position by the length + the null terminator
                index += hashString.Length + 1;
            }
        }

        /// <summary>
        /// Writes the files relevant to this WLD type to disk
        /// </summary>
        public virtual void ExportData()
        {
            ExportMeshes();

            if (Settings.ModelExportFormat == ModelExportFormat.Intermediate)
            {
                ExportActors();
                ExportSkeletonAndAnimations();
            }
        }
        /// <summary>
        /// Returns a mapping of the material name to the shader type
        /// Used in exporting the bitmaps from the PFS archive
        /// </summary>
        /// <returns>HashSet of masked bitmap materials</returns>Add commentMore actions
        public HashSet<string> GetMaskedBitmaps()
        {
            var materialLists = GetFragmentsOfType<MaterialList>();

            if (materialLists.Count == 0)
            {
                Logger.LogWarning("Cannot get material types. No texture list found.");
                return null;
            }

            HashSet<string> maskedTextures = new HashSet<string>();

            foreach (var list in materialLists)
            {
                foreach (var material in list.Materials)
                {
                    if (material.ShaderType != ShaderType.TransparentMasked)
                    {
                        continue;
                    }

                    maskedTextures.UnionWith(material.GetAllBitmapNames(true));
                }

                if (list.AdditionalMaterials != null)
                {
                    foreach (var material in list.AdditionalMaterials)
                    {
                        if (material.ShaderType != ShaderType.TransparentMasked)
                        {
                            continue;
                        }

                        maskedTextures.UnionWith(material.GetAllBitmapNames(true));
                    }
                }
            }

            return maskedTextures;
        }

        private void ExportMeshes()
        {
            if (Settings.ModelExportFormat == ModelExportFormat.Intermediate)
            {
                MeshExporter.ExportMeshes(this, Settings, Logger);
            }
            else if (Settings.ModelExportFormat == ModelExportFormat.Obj)
                ActorObjExporter.ExportActors(this, Settings, Logger);
        }

        public string GetExportFolderForWldType()
        {
            switch (WldType)
            {
                case Wld.WldType.Zone:
                case Wld.WldType.Lights:
                case Wld.WldType.ZoneObjects:
                    return GetRootExportFolder() + "/Zone/";
                case Wld.WldType.Equipment:
                    return GetRootExportFolder();
                case Wld.WldType.Objects:
                    return GetRootExportFolder() + "Objects/";
                case Wld.WldType.Sky:
                    return GetRootExportFolder();
                case Wld.WldType.Characters:
                    if (Settings.ExportCharactersToSingleFolder &&
                        Settings.ModelExportFormat == ModelExportFormat.Intermediate)
                    {
                        return GetRootExportFolder();
                    }
                    else
                    {
                        return GetRootExportFolder() + "Characters/";
                    }
                default:
                    return string.Empty;
            }
        }

        protected string GetRootExportFolder()
        {
            switch (WldType)
            {
                case Wld.WldType.Equipment when Settings.ExportEquipmentToSingleFolder &&
                                                Settings.ModelExportFormat == ModelExportFormat.Intermediate:
                    return RootExportFolder + "equipment/";
                case Wld.WldType.Characters when (Settings.ExportCharactersToSingleFolder &&
                                                  Settings.ModelExportFormat == ModelExportFormat.Intermediate):
                    return RootExportFolder + "characters/";
                default:
                    return RootExportFolder + ShortnameHelper.GetCorrectZoneShortname(ZoneName) + "/";
            }
        }

        private void ExportActors()
        {
            var actors = GetFragmentsOfType<Actor>();

            if (WldToInject != null)
            {
                actors.AddRange(WldToInject.GetFragmentsOfType<Actor>());
            }

            if (actors.Count == 0)
            {
                return;
            }

            TextAssetWriter actorWriterStatic, actorWriterSkeletal, actorWriterParticle, actorWriterSprite2d;

            if (WldType == Wld.WldType.Equipment && Settings.ExportEquipmentToSingleFolder || WldType == Wld.WldType.Characters)
            {
                bool isCharacters = WldType == Wld.WldType.Characters;
                actorWriterStatic = new ActorWriterNewGlobal(ActorType.Static, GetExportFolderForWldType());
                actorWriterSkeletal = new ActorWriterNewGlobal(ActorType.Skeletal, GetExportFolderForWldType());
                actorWriterParticle = new ActorWriterNewGlobal(ActorType.Particle, GetExportFolderForWldType());
                actorWriterSprite2d = new ActorWriterNewGlobal(ActorType.Sprite, GetExportFolderForWldType());
            }
            else
            {
                actorWriterStatic = new ActorWriter(ActorType.Static);
                actorWriterSkeletal = new ActorWriter(ActorType.Skeletal);
                actorWriterParticle = new ActorWriter(ActorType.Particle);
                actorWriterSprite2d = new ActorWriter(ActorType.Sprite);
            }

            foreach (var actorFragment in actors)
            {
                actorWriterStatic.AddFragmentData(actorFragment);
                actorWriterSkeletal.AddFragmentData(actorFragment);
                actorWriterParticle.AddFragmentData(actorFragment);
                actorWriterSprite2d.AddFragmentData(actorFragment);
            }

            string exportPath = GetExportFolderForWldType() + "actors";
            actorWriterStatic.WriteAssetToFile(exportPath + "_static.txt");
            actorWriterSkeletal.WriteAssetToFile(exportPath + "_skeletal.txt");
            actorWriterParticle.WriteAssetToFile(exportPath + "_particle.txt");
            actorWriterSprite2d.WriteAssetToFile(exportPath + "_sprite.txt");
        }

        protected void ExportSkeletonAndAnimations()
        {
            string skeletonsFolder = GetExportFolderForWldType() + "Skeletons/";
            string animationsFolder = GetExportFolderForWldType() + "Animations/";

            var skeletons = GetFragmentsOfType<SkeletonHierarchy>();

            if (skeletons.Count == 0)
            {
                if (WldToInject == null)
                {
                    Logger.LogWarning("Cannot export animations. No model references.");
                    return;
                }

                skeletons = WldToInject.GetFragmentsOfType<SkeletonHierarchy>();

                if (skeletons == null)
                {
                    Logger.LogWarning("Cannot export animations 2. No model references.");
                    return;
                }
            }

            SkeletonHierarchyWriter skeletonWriter = new SkeletonHierarchyWriter(WldType == Wld.WldType.Characters);
            AnimationWriter animationWriter = new AnimationWriter(WldType == Wld.WldType.Characters);

            foreach (var skeleton in skeletons)
            {
                string filePath = skeletonsFolder + skeleton.ModelBase + ".txt";

                skeletonWriter.AddFragmentData(skeleton);

                // TODO: Put this elsewhere - what does this even do?
                if (WldType == Wld.WldType.Characters && Settings.ExportCharactersToSingleFolder)
                {
                    if (File.Exists(filePath))
                    {
                        var file = File.ReadAllText(filePath);
                        int oldFileSize = file.Length;
                        int newFileSize = skeletonWriter.GetExportByteCount();

                        if (newFileSize > oldFileSize)
                        {
                            skeletonWriter.WriteAssetToFile(filePath);
                        }

                        skeletonWriter.ClearExportData();
                    }
                    else
                    {
                        skeletonWriter.WriteAssetToFile(filePath);
                        skeletonWriter.ClearExportData();
                    }
                }
                else
                {
                    skeletonWriter.WriteAssetToFile(filePath);
                    skeletonWriter.ClearExportData();
                }


                foreach (var animation in skeleton.Animations)
                {
                    var modelBase = string.IsNullOrEmpty(animation.Value.AnimModelBase)
                        ? skeleton.ModelBase
                        : animation.Value.AnimModelBase;
                    animationWriter.SetTargetAnimation(animation.Key);
                    animationWriter.AddFragmentData(skeleton);
                    string fileName = modelBase + "_" + animation.Key + ".txt";
                    animationWriter.WriteAssetToFile(animationsFolder + fileName);
                    animationWriter.ClearExportData();
                }
            }
        }

        public HashSet<string> GetAllBitmapNames()
        {
            HashSet<string> bitmaps = new HashSet<string>();
            var bitmapFragments = GetFragmentsOfType<BitmapName>();
            foreach (var fragment in bitmapFragments)
            {
                bitmaps.Add(fragment.Filename);
            }

            return bitmaps;
        }

        private void BuildSkeletonData()
        {
            var skeletons = GetFragmentsOfType<SkeletonHierarchy>();

            foreach (var skeleton in skeletons)
            {
                skeleton.BuildSkeletonData(WldType == Wld.WldType.Characters || Settings.ModelExportFormat == ModelExportFormat.Intermediate);
            }

            (WldToInject as WldFileCharacters)?.BuildSkeletonData();
        }
    }
}
