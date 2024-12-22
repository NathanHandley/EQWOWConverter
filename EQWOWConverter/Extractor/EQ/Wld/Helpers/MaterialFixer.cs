using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.Helpers
{
    /// <summary>
    /// Fixes issues with EverQuest materials.
    /// Currently, only fixes incorrect shader assignment
    /// but can be extended in the future.
    /// </summary>
    public static class MaterialFixer
    {
        public static void Fix(WldFile wld)
        {
            var materials = wld.GetFragmentsOfType<Material>();

            foreach (var material in materials)
            {
                switch (material.Name)
                {
                    case "TREE7_MDF":
                    case "TREE9B1_MDF":
                    case "TREE16_MDF":
                    case "TREE16B1_MDF":
                    case "TREE17_MDF":
                    case "TREE18_MDF":
                    case "TREE18B1_MDF":
                    case "TREE20_MDF":
                    case "TREE20B1_MDF":
                    case "TREE21_MDF":
                    case "TREE22_MDF":
                    case "TREE22B1_MDF":
                    case "TOP_MDF":
                    case "TOPB_MDF":
                    case "FURPILE1_MDF":
                    case "BEARRUG_MDF":
                        FixShaderAssignment(material, ShaderType.TransparentMasked);
                        break;
                    case "FIRE1_MDF":
                        FixShaderAssignment(material, ShaderType.TransparentAdditiveUnlit);
                        break;
                    case "ICE1_MDF":
                        FixShaderAssignment(material, ShaderType.Invisible);
                        break;
                    case "AIRCLOUD_MDF":
                    case "NORMALCLOUD_MDF":
                        FixShaderAssignment(material, ShaderType.TransparentSkydome);
                        break;
                }
            }
        }

        private static void FixShaderAssignment(Material material, ShaderType shader)
        {
            material.ShaderType = shader;
        }
    }
}
