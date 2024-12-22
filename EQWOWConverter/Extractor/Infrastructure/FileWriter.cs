using System.IO;

namespace LanternExtractor.Infrastructure
{
    public static class FileWriter
    {
        public static void WriteBytesToDisk(byte[] bytes, string filePath, string fileName)
        {
            if (bytes == null || string.IsNullOrEmpty(filePath))
            {
                return;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var binaryWriter =
                new BinaryWriter(
                    File.OpenWrite(Path.Combine(filePath, fileName)));
            binaryWriter.Write(bytes);
            binaryWriter.Close();
        }
    }
}