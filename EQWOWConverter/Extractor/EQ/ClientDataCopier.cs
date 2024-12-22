using System.Collections.Generic;
using System.IO;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ
{
    public static class ClientDataCopier
    {
        private const string ClientDataDirectory = "clientdata";

        public static void Copy(string fileName, string rootFolder, ILogger logger, Settings settings)
        {
            if (settings.ClientDataToCopy == null ||
                settings.ModelExportFormat != ModelExportFormat.Intermediate ||
                IsInvalidName(fileName))
            {
                return;
            }

            WriteAllFiles(rootFolder, logger, settings);
        }

        private static void WriteAllFiles(string rootFolder, ILogger logger, Settings settings)
        {
            Directory.CreateDirectory(rootFolder + ClientDataDirectory);

            var filePaths = GetClientDataFilePaths(settings);

            foreach (var filePath in filePaths)
            {
                var destFilePath = GetDestinationPath(rootFolder, filePath);
                logger.LogInfo($"Copying {filePath} to {destFilePath}");
                File.Copy(filePath, destFilePath, true);
            }
        }

        private static List<string> GetClientDataFilePaths(Settings settings)
        {
            var paths = new List<string>();

            foreach (var filePath in settings.ClientDataToCopy.Split(','))
            {
                var clientDataFilePath = Path.Combine(settings.EverQuestDirectory, filePath.Trim());
                if (File.Exists(clientDataFilePath))
                {
                    paths.Add(clientDataFilePath);
                }
            }

            return paths;
        }

        private static string GetDestinationPath(string rootFolder, string sourceFilePath)
        {
            var sourceFileName = Path.GetFileName(sourceFilePath);
            return Path.Combine(rootFolder + ClientDataDirectory, sourceFileName);
        }

        private static bool IsInvalidName(string fileName)
        {
            return !EqFileHelper.IsClientDataFile(fileName) && fileName != "all";
        }
    }
}
