using Microsoft.Extensions.Logging;

namespace SampleStack.Telemetry.Logging
{
    public static partial class LoggingExtensions
    {
        [LoggerMessage(LogLevel.Information, "Starting Client Application")]
        public static partial void LogStartingClientApplication(this ILogger logger);

        [LoggerMessage(LogLevel.Error, "Error calling API")]
        public static partial void LogErrorCallingApi(this ILogger logger, Exception exception);

        [LoggerMessage(LogLevel.Information, "Finished Client Application")]
        public static partial void LogFinishedClientApplication(this ILogger logger);
    }
}
