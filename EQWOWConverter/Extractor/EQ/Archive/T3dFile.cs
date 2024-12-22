namespace LanternExtractor.EQ.Archive
{
    /// <summary>
    /// This class represents a single file in the archive
    /// </summary>
    public class T3dFile : ArchiveFile
    {
        public T3dFile(uint size, uint offset, byte[] bytes) : base(size, offset, bytes)
        {
        }
    }
}
