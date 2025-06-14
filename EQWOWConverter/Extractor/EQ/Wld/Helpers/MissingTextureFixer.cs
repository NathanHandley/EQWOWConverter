using System;
using System.IO;

namespace LanternExtractor.EQ.Wld.Helpers
{
    /// <summary>
    /// In some cases, textures used in zones are not included in the zone S3D
    /// because they are also used in the object S3D. The EQ client reads all
    /// textures into a pool so this is not a problem. With Lantern, textures
    /// are separated by S3D type. Therefore, some post extraction copying
    /// must be done to fix the missing textures.
    /// </summary>
    public static class MissingTextureFixer
    {
        public static void Fix(string shortname)
        {
            switch (shortname)
            {
                case "oasis_obj":
                    CopyTexture("Exports/oasis/Objects/Textures/canwall1.png",
                        "Exports/oasis/Zone/Textures/canwall1.png");
                    break;
                case "fearplane_obj":
                    CopyTexture("Exports/fearplane/Objects/Textures/maywall.png",
                        "Exports/fearplane/Zone/Textures/maywall.png");
                    break;
                case "swampofnohope_obj":
                    CopyTexture("Exports/swampofnohope/Objects/Textures/kruphse3.png",
                        "Exports/swampofnohope/Zone/Textures/kruphse3.png");
                    break;
                case "bothunder":
                case "postorms":
                case "solrotower":
                    // Only needed when characters are exported into zone folders or obj/gltf exports
                    CopyTexture("Exports/airplane/Characters/Textures/shield round-metal.png",
                        $"Exports/{shortname}/Characters/Textures/shield round-metal.png");
                    CopyTexture("Exports/airplane/Characters/Textures/shield round-metalback.png",
                        $"Exports/{shortname}/Characters/Textures/shield round-metalback.png");
                    break;
            }
        }

        private static void CopyTexture(string source, string destination)
        {
            if (!File.Exists(source) || File.Exists(destination) || !Directory.Exists(Path.GetDirectoryName(destination)))
            {
                return;
            }
            
            File.Copy(source, destination);
        }
    }
}