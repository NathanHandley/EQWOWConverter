using System.IO;
using System.Linq;

namespace LanternExtractor.EQ
{
    public static class VideoCopier
    {
        private const string VideoDirectory = "video";

        public static void Copy(string shortname, Settings settings)
        {
            if (shortname != "video" && shortname != "all")
            {
                return;
            }

            if (!settings.CopyVideo)
            {
                return;
            }

            var files = Directory.GetFiles(settings.EverQuestDirectory, "*.*", SearchOption.AllDirectories)
                .Where(EqFileHelper.IsVideoFile).ToList();
            var destinationFolder = "Exports/" + VideoDirectory;

            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var destination = Path.Combine(destinationFolder, fileName);
                if (File.Exists(destination))
                {
                    continue;
                }

                File.Copy(file, destination);
            }
        }
    }
}