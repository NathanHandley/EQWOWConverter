using System.Collections.Generic;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.EQ.Wld.Helpers;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// Material (0x30)
    /// Internal name: _MDF
    /// Contains information about a material's shader and textures.
    /// </summary>
    public class Material : WldFragment
    {
        /// <summary>
        /// The BitmapInfoReference that this material uses
        /// </summary>
        public BitmapInfoReference BitmapInfoReference { get; private set; }

        /// <summary>
        /// The shader type that this material uses when rendering
        /// </summary>
        public ShaderType ShaderType { get; set; }

        public float Brightness { get; set; }
        public float ScaledAmbient { get; set; }

        /// <summary>
        /// If a material has not been handled, we still need to find the corresponding material list
        /// Used for alternate character skins
        /// </summary>
        public bool IsHandled { get; set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int flags = Reader.ReadInt32();
            int parameters = Reader.ReadInt32();

            // Unsure what this color is used for
            // Referred to as the RGB pen
            byte colorR = Reader.ReadByte();
            byte colorG = Reader.ReadByte();
            byte colorB = Reader.ReadByte();
            byte colorA = Reader.ReadByte();

            Brightness = Reader.ReadSingle();
            ScaledAmbient = Reader.ReadSingle();

            int fragmentReference = Reader.ReadInt32();

            if (fragmentReference != 0)
            {
                BitmapInfoReference = fragments[fragmentReference - 1] as BitmapInfoReference;
            }

            // Thanks to PixelBound for figuring this out
            MaterialType materialType = (MaterialType) (parameters & ~0x80000000);

            switch (materialType)
            {
                case MaterialType.Boundary:
                    ShaderType = ShaderType.Boundary;
                    break;
                case MaterialType.InvisibleUnknown:
                case MaterialType.InvisibleUnknown2:
                case MaterialType.InvisibleUnknown3:
                    ShaderType = ShaderType.Invisible;
                    break;
                case MaterialType.Diffuse:
                case MaterialType.Diffuse3:
                case MaterialType.Diffuse4:
                case MaterialType.Diffuse6:
                case MaterialType.Diffuse7:
                case MaterialType.Diffuse8:
                case MaterialType.Diffuse2:
                case MaterialType.CompleteUnknown:
                case MaterialType.TransparentMaskedPassable:
                    ShaderType = ShaderType.Diffuse;
                    break;
                case MaterialType.Transparent25:
                    ShaderType = ShaderType.Transparent25;
                    break;
                case MaterialType.Transparent50:
                    ShaderType = ShaderType.Transparent50;
                    break;
                case MaterialType.Transparent75:
                    ShaderType = ShaderType.Transparent75;
                    break;
                case MaterialType.TransparentAdditive:
                    ShaderType = ShaderType.TransparentAdditive;
                    break;
                case MaterialType.TransparentAdditiveUnlit:
                    ShaderType = ShaderType.TransparentAdditiveUnlit;
                    break;
                case MaterialType.TransparentMasked:
                case MaterialType.Diffuse5:
                    ShaderType = ShaderType.TransparentMasked;
                    break;
                case MaterialType.DiffuseSkydome:
                    ShaderType = ShaderType.DiffuseSkydome;
                    break;
                case MaterialType.TransparentSkydome:
                    ShaderType = ShaderType.TransparentSkydome;
                    break;
                case MaterialType.TransparentAdditiveUnlitSkydome:
                    ShaderType = ShaderType.TransparentAdditiveUnlitSkydome;
                    break;
                default:
                    ShaderType = BitmapInfoReference == null ? ShaderType.Invisible : ShaderType.Diffuse;
                    break;
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("Material: Shader type: " + ShaderType);

            if (ShaderType != ShaderType.Invisible && BitmapInfoReference != null)
            {
                logger.LogInfo("Material: Reference: " + (BitmapInfoReference.Index + 1));
            }
        }

        /// <summary>
        /// Returns all bitmap names referenced by this material
        /// </summary>
        /// <param name="includeExtension">Should be .bmp extension be included?</param>
        /// <returns>List of bitmap names</returns>
        public List<string> GetAllBitmapNames(bool includeExtension = false)
        {
            var bitmapNames = new List<string>();

            if (BitmapInfoReference == null)
            {
                return bitmapNames;
            }

            foreach (BitmapName bitmapName in BitmapInfoReference.BitmapInfo.BitmapNames)
            {
                string filename = bitmapName.Filename;

                if (!includeExtension)
                {
                    filename = filename.Substring(0, filename.Length - 4);
                }

                bitmapNames.Add(filename);
            }

            return bitmapNames;
        }

        /// <summary>
        /// Returns the first bitmap name this material uses
        /// </summary>
        /// <returns></returns>
        public string GetFirstBitmapNameWithoutExtension()
        {
            if (BitmapInfoReference?.BitmapInfo?.BitmapNames == null || BitmapInfoReference.BitmapInfo.BitmapNames.Count == 0)
            {
                return string.Empty;
            }

            return BitmapInfoReference.BitmapInfo.BitmapNames[0].GetFilenameWithoutExtension();
        }

        public string GetFirstBitmapExportFilename()
        {
            if (BitmapInfoReference?.BitmapInfo?.BitmapNames == null || BitmapInfoReference.BitmapInfo.BitmapNames.Count == 0)
            {
                return string.Empty;
            }

            return BitmapInfoReference.BitmapInfo.BitmapNames[0].GetExportFilename();
        }

        public string GetFullMaterialName()
        {
            return MaterialList.GetMaterialPrefix(ShaderType) +
                    FragmentNameCleaner.CleanName(this);
        }

        public void SetBitmapName(int index, string newName)
        {
            if (BitmapInfoReference == null)
            {
                return;
            }

            BitmapInfoReference.BitmapInfo.BitmapNames[index].Filename = newName + ".bmp";
        }
    }
}
