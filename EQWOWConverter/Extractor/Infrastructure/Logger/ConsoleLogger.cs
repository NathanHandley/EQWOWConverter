using System;

namespace LanternExtractor.Infrastructure.Logger
{
    /// <summary>
    /// Output the log info to the console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public LogVerbosity Verbosity { get; set; }

        public void SetVerbosity(LogVerbosity verbosity)
        {
            Verbosity = verbosity;
        }

        public void LogInfo(string message)
        {
            if (Verbosity > LogVerbosity.Info)
            {
                return;
            }

            Console.WriteLine(message);
        }

        public void LogWarning(string message)
        {
            if (Verbosity > LogVerbosity.Warning)
            {
                return;
            }

            Console.WriteLine(message);
        }

        public void LogError(string message)
        {
            Console.WriteLine(message);
        }
    }
}