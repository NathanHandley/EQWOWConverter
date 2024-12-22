namespace LanternExtractor.Infrastructure.Logger
{
    /// <summary>
    /// A logger with no output
    /// </summary>
    public class EmptyLogger : ILogger
    {
        public LogVerbosity Verbosity { get; set; }

        public void SetVerbosity(LogVerbosity verbosity)
        {
            
        }
        
        public void LogInfo(string message)
        {
        }

        public void LogWarning(string message)
        {
        }

        public void LogError(string message)
        {
        }
    }
}