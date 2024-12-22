using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Archive
{
    public class NullArchive : ArchiveBase
    {
        public NullArchive(string filePath, ILogger logger) : base(filePath, logger)
        {
        }

        public override bool Initialize()
        {
            return false;
        }
    }
}
