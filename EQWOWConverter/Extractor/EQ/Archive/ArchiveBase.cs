using System.Collections.Generic;
using System.IO;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Archive
{
    public abstract class ArchiveBase
    {
        public string FilePath { get; }
        public string FileName { get; }
        protected List<ArchiveFile> Files = new List<ArchiveFile>();
        protected Dictionary<string, ArchiveFile> FileNameReference = new Dictionary<string, ArchiveFile>();
        protected ILogger Logger;
        public bool IsWldArchive { get; set; }
        public Dictionary<string, string> FilenameChanges = new Dictionary<string, string>();

        protected ArchiveBase(string filePath, ILogger logger)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            Logger = logger;
        }

        public abstract bool Initialize();

        public ArchiveFile GetFile(string fileName)
        {
            return !FileNameReference.ContainsKey(fileName) ? null : FileNameReference[fileName];
        }

        public ArchiveFile GetFile(int index)
        {
            if (index < 0 || index >= Files.Count)
            {
                return null;
            }

            return Files[index];
        }

        public ArchiveFile[] GetAllFiles()
        {
            return Files.ToArray();
        }

        public void WriteAllFiles(string folder)
        {
            foreach (var file in Files)
            {
                FileWriter.WriteBytesToDisk(file.Bytes, folder, file.Name);
            }
        }

        public void RenameFile(string originalName, string newName)
        {
            if (!FileNameReference.ContainsKey(originalName))
            {
                return;
            }

            var file = FileNameReference[originalName];
            FileNameReference.Remove(originalName);
            file.Name = newName;
            FileNameReference[newName] = file;
        }
    }
}
