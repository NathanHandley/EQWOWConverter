using System;
using System.Collections.Generic;
using LanternExtractor.EQ.Wld.Helpers;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// MaterialList (0x31)
    /// Internal name: _MP
    /// A list of material fragments (0x30) that make up a single list.
    /// This list is used in the rendering of an mesh (via the list indices).
    /// </summary>
    public class MaterialList : WldFragment
    {
        /// <summary>
        /// The materials in the list
        /// </summary>
        public List<Material> Materials { get; private set; }

        /// <summary>
        /// A mapping of slot names to alternate skins
        /// </summary>
        public Dictionary<string, Dictionary<int, Material>> Slots { get; private set; }

        /// <summary>
        /// The number of alternate skins
        /// </summary>
        public int VariantCount { get; set; }

        public List<Material> AdditionalMaterials { get; set; }

        /// <summary>
        /// Prevents the material list from being exported multiple times due to being shared
        /// TODO: Move this out of here
        /// </summary>
        public bool HasBeenExported { get; set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];

            Materials = new List<Material>();

            int flags = Reader.ReadInt32();
            int materialCount = Reader.ReadInt32();

            for (int i = 0; i < materialCount; ++i)
            {
                int reference = Reader.ReadInt32() - 1;
                Material material = fragments[reference] as Material;

                if (material == null)
                {
                    logger.LogError("Unable to get material reference for fragment id: " + reference);
                    continue;
                }

                Materials.Add(material);

                // Materials that are referenced in the MaterialList are already handled
                material.IsHandled = true;
            }
        }

        public void BuildSlotMapping(ILogger logger)
        {
            Slots = new Dictionary<string, Dictionary<int, Material>>();

            if (Materials == null || Materials.Count == 0)
            {
                return;
            }

            foreach (var material in Materials)
            {
                string character = string.Empty;
                string skinId = string.Empty;
                string partName = string.Empty;

                ParseCharacterSkin(FragmentNameCleaner.CleanName(material), out character, out skinId, out partName,
                    logger);

                string key = character + "_" + partName;
                Slots[key] = new Dictionary<int, Material>();
            }

            AdditionalMaterials = new List<Material>();
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("0x30: Material count: " + Materials.Count);

            string references = string.Empty;

            for (var i = 0; i < Materials.Count; i++)
            {
                if (i != 0)
                {
                    references += ", ";
                }

                references += (Materials[i].Index + 1);
            }

            logger.LogInfo("0x30: References: " + references);
        }

        /// <summary>
        /// Parses the info from the material name
        /// </summary>
        /// <param name="materialName">The name of the material</param>
        /// <param name="character">The character mesh it belongs to</param>
        /// <param name="skinId">The skin ID</param>
        /// <param name="partName">The name of the body part this material is applied to</param>
        private static void ParseCharacterSkin(string materialName, out string character, out string skinId,
            out string partName, ILogger logger)
        {
            if (materialName.Length != 9)
            {
                character = string.Empty;
                skinId = string.Empty;
                partName = string.Empty;
                return;
            }

            character = materialName.Substring(0, 3);
            skinId = materialName.Substring(5, 2);
            partName = materialName.Substring(3, 2) + materialName.Substring(7, 2);
        }

        public static string GetMaterialPrefix(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.Diffuse:
                    return "d_";
                case ShaderType.Invisible:
                    return "i_";
                case ShaderType.Boundary:
                    return "b_";
                case ShaderType.Transparent25:
                    return "t25_";
                case ShaderType.Transparent50:
                    return "t50_";
                case ShaderType.Transparent75:
                    return "t75_";
                case ShaderType.TransparentAdditive:
                    return "ta_";
                case ShaderType.TransparentAdditiveUnlit:
                    return "tau_";
                case ShaderType.TransparentMasked:
                    return "tm_";
                case ShaderType.DiffuseSkydome:
                    return "ds_";
                case ShaderType.TransparentSkydome:
                    return "ts_";
                case ShaderType.TransparentAdditiveUnlitSkydome:
                    return "taus_";
                default:
                    return "d_";
            }
        }

        public void AddVariant(Material material, ILogger logger)
        {
            string character = string.Empty;
            string skinId = string.Empty;
            string partName = string.Empty;
            ParseCharacterSkin(FragmentNameCleaner.CleanName(material), out character, out skinId, out partName,
                logger);

            string key = character + "_" + partName;

            if (!Slots.ContainsKey(key))
            {
                Slots[key] = new Dictionary<int, Material>();
            }

            int skinIdNumber = Convert.ToInt32(skinId);
            Slots[key][skinIdNumber] = material;
            material.IsHandled = true;

            if (skinIdNumber > VariantCount)
            {
                VariantCount = skinIdNumber;
            }

            AdditionalMaterials.Add(material);
        }

        public List<Material> GetMaterialVariants(Material material, ILogger logger)
        {
            List<Material> additionalSkins = new List<Material>();

            if (Slots == null)
            {
                return additionalSkins;
            }

            string character = string.Empty;
            string skinId = string.Empty;
            string partName = string.Empty;
            ParseCharacterSkin(FragmentNameCleaner.CleanName(material), out character, out skinId, out partName,
                logger);

            string key = character + "_" + partName;

            if (!Slots.ContainsKey(key))
            {
                return additionalSkins;
            }

            var variants = Slots[key];
            for (int i = 0; i < VariantCount; ++i)
            {
                if (!variants.ContainsKey(i + 1))
                {
                    additionalSkins.Add(null);
                    continue;
                }

                additionalSkins.Add(variants[i + 1]);
            }

            return additionalSkins;
        }
    }
}