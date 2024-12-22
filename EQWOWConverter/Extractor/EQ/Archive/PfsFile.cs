namespace LanternExtractor.EQ.Archive
{
    /// <summary>
    /// This class represents a single file in the archive
    /// </summary>
    public class PfsFile : ArchiveFile
    {
        /// <summary>
        /// The CRC of the PFSFile
        /// </summary>
        public uint Crc { get; }

        public PfsFile(uint crc, uint size, uint offset, byte[] bytes) : base(size, offset, bytes)
        {
            Crc = crc;
        }
    }
}
