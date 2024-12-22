using System.IO;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.Infrastructure
{
    public static class SoundWriter
    {
        public static void WriteSoundAsWav(byte[] bytes, string filePath, string fileName,
            ILogger logger)
        {
            Directory.CreateDirectory(filePath);
            var path = Path.Combine(filePath, fileName);

            if (File.Exists(path))
            {
                logger.LogInfo($"SoundWriter: overwriting {fileName}");
            }

            File.WriteAllBytes(path, bytes);
        }
    }
}
