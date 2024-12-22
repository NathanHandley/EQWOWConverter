using System.Collections.Generic;
using System.IO;
using System.Linq;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Archive
{
    /// <summary>
    /// Loads and can extract files in the T3D archive
    /// </summary>
    public class T3dArchive : ArchiveBase
    {
        private static readonly byte[] T3dMagic = new byte[] {0x02, 0x3D, 0xFF, 0xFF};
        private static readonly byte[] T3dVersion = new byte[] {0x00, 0x57, 0x01, 0x00};

        public T3dArchive(string filePath, ILogger logger) : base(filePath, logger)
        {
        }

        public override bool Initialize()
        {
            Logger.LogInfo("T3dArchive: Started initialization of archive: " + FileName);

            if (!File.Exists(FilePath))
            {
                Logger.LogError("T3dArchive: File does not exist at: " + FilePath);
                return false;
            }

            using (var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                var reader = new BinaryReader(fileStream);

                var magic = reader.ReadBytes(4);
                if (!magic.SequenceEqual(T3dMagic))
                {
                    Logger.LogError("T3dArchive: Incorrect file magic");
                    return false;
                }

                var version = reader.ReadBytes(4);
                if (!version.SequenceEqual(T3dVersion))
                {
                    Logger.LogError("T3dArchive: Incorrect file version");
                    return false;
                }

                var fileCount = reader.ReadUInt32();
                var filenamesLength = reader.ReadUInt32();

                var offsetPairs = new List<(uint FileOffset, uint FileNameOffset)>();
                for (int i = 0; i < fileCount; i++)
                {
                    var fileNameBaseOffset = (uint) reader.BaseStream.Position;
                    offsetPairs.Add((reader.ReadUInt32(), reader.ReadUInt32() + fileNameBaseOffset));
                }

                var totalFilesize = reader.ReadUInt64();

                for (int i = 0; i < fileCount - 1; i++)
                {
                    var fileOffset = offsetPairs[i].FileOffset;
                    var fileNameOffset = offsetPairs[i].FileNameOffset;
                    var nextFileOffset = i == fileCount - 2 ? totalFilesize : offsetPairs[i + 1].FileOffset;
                    uint fileSize = (uint) (nextFileOffset - fileOffset);
                    var fileBytes = new byte[fileSize];

                    reader.BaseStream.Position = fileOffset;
                    reader.Read(fileBytes);

                    var file = new T3dFile(fileSize, fileOffset, fileBytes);

                    reader.BaseStream.Position = fileNameOffset;
                    file.Name = reader.ReadNullTerminatedString().ToLower();

                    if (!IsWldArchive && file.Name.EndsWith(LanternStrings.WldFormatExtension))
                    {
                        IsWldArchive = true;
                    }

                    Files.Add(file);
                    FileNameReference[file.Name] = file;
                }
            }

            return true;
        }
    }
}
