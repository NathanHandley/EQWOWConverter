namespace LanternExtractor.Infrastructure.Logger
{
    /// <summary>
    /// A simple logger interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// The verbosity the log - what is the minimum severity level that we will handle
        /// </summary>
        LogVerbosity Verbosity { get; set; }

        /// <summary>
        /// Sets the verbosity of the logger
        /// </summary>
        void SetVerbosity(LogVerbosity verbosity);
        
        /// <summary>
        /// Logs information as as debug info
        /// </summary>
        void LogInfo(string message);

        /// <summary>
        /// Logs information as a warning
        /// </summary>
        void LogWarning(string message);

        /// <summary>
        /// Logs information as an error
        /// </summary>
        void LogError(string message);
    }
}