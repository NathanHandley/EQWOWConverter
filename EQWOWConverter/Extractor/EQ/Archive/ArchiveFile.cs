namespace LanternExtractor.EQ.Archive
{
    /// <summary>
    /// This class represents a single file in the archive
    /// </summary>
    public abstract class ArchiveFile
    {
        /// <summary>
        /// The inflated size of the file in bytes
        /// </summary>
        public uint Size { get; }

        /// <summary>
        /// The positional offset of the file in the archive
        /// </summary>
        public uint Offset { get; }

        /// <summary>
        /// The inflated bytes of the file
        /// </summary>
        public byte[] Bytes { get; }

        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name { get; set; }

        public ArchiveFile(uint size, uint offset, byte[] bytes)
        {
            Size = size;
            Offset = offset;
            Bytes = bytes;
        }
    }
}
