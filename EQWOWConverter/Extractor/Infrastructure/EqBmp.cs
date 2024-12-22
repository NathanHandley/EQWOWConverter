using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace LanternExtractor.Infrastructure
{
    /// <summary>
    /// Wrapper over Bitmap with various hacks to make libgdiplus
    /// consistently create transparent pngs on MacOS & Linux
    /// </summary>
    public class EqBmp
    {
        private static readonly bool NeedsGdipHacks = Environment.OSVersion.Platform == PlatformID.MacOSX ||
            Environment.OSVersion.Platform == PlatformID.Unix;
        private static bool _hasCheckedForPaletteFlagsField;
        private static System.Reflection.FieldInfo _paletteFlagsField = null;

        public PixelFormat PixelFormat => _bitmap.PixelFormat;

        private readonly Bitmap _bitmap;
        private readonly ColorPalette _palette;

        public EqBmp(Stream stream)
        {
            SetPaletteFlagsField();

            _bitmap = new Bitmap(stream);
            _palette = _bitmap.Palette;
        }

        public void WritePng(string outputFilePath)
        {
            _bitmap.Save(outputFilePath, ImageFormat.Png);
        }

        public void MakeMagentaTransparent()
        {
            _bitmap.MakeTransparent(Color.Magenta);
            if (NeedsGdipHacks)
            {
                // https://github.com/mono/libgdiplus/commit/bf9a1440b7bfea704bf2cb771f5c2b5c09e7bcfa
                _bitmap.MakeTransparent(Color.FromArgb(0, Color.Magenta));
            }
        }

        public void MakePaletteTransparent(int transparentIndex)
        {
            if (NeedsGdipHacks)
            {
                // https://github.com/mono/libgdiplus/issues/702
                _paletteFlagsField?.SetValue(_palette, _palette.Flags | (int)PaletteFlags.HasAlpha);
            }

            var transparentColor = GetTransparentPaletteColor();
            _palette.Entries[transparentIndex] = transparentColor;
            _bitmap.Palette = _palette;

            if (NeedsGdipHacks)
            {
                // Due to a bug with the libgdiplus implementation of System.Drawing, setting a color palette
                // entry to transparent does not work. The workaround is to ensure that the transparent
                // key is unique and then use MakeTransparent()
                _bitmap.MakeTransparent(transparentColor);
            }
        }

        private Color GetTransparentPaletteColor()
        {
            var transparencyColor = Color.FromArgb(0, 0, 0, 0);

            if (!NeedsGdipHacks)
            {
                return transparencyColor;
            }

            var random = new Random();
            var foundUnique = false;

            while (!foundUnique)
            {
                foundUnique = _palette.Entries.All(e => e != transparencyColor);
                transparencyColor = Color.FromArgb(0, random.Next(256), random.Next(256), random.Next(256));
            }

            return transparencyColor;
        }

        // https://github.com/Robmaister/SharpFont/blob/422bdab059dd8e594b4b061a3b53152e71342ce2/Source/SharpFont.GDI/FTBitmapExtensions.cs
        // https://github.com/Robmaister/SharpFont/pull/136
        private static void SetPaletteFlagsField()
        {
            if (!NeedsGdipHacks || _hasCheckedForPaletteFlagsField)
            {
                return;
            }

            _hasCheckedForPaletteFlagsField = true;

            // The field needed may be named "flags" or "_flags", depending on the version of Mono. To be thorough, check for the first Name that contains "lags".
            var fields = typeof(ColorPalette).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            foreach (var t in fields)
            {
                if (t.Name.Contains("lags"))
                {
                    _paletteFlagsField = t;
                    break;
                }
            }
        }

        enum PaletteFlags
        {
            HasAlpha = 0x0001,
            GrayScale = 0x0002,
            HalfTone = 0x0004,
        }
    }
}
