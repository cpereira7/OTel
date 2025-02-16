using Microsoft.Extensions.Logging;

namespace SampleStack.Telemetry.Logging
{
    public static partial class LoggingExtensions
    {
        [LoggerMessage(LogLevel.Information, "Starting Client Application with Execution ID: {executionId}")]
        public static partial void LogStartingClientApplication(this ILogger logger, string executionId);

        [LoggerMessage(LogLevel.Information, "Calling API #{runNumber}: {endpoint} with Execution ID: {executionId}")]
        public static partial void LogCallingApi(this ILogger logger, int runNumber, string endpoint, string executionId);

        [LoggerMessage(LogLevel.Error, "Error calling API on run {runNumber} with Execution ID: {executionId}")]
        public static partial void LogErrorCallingApi(this ILogger logger, Exception exception, int runNumber, string executionId);

        [LoggerMessage(LogLevel.Information, "Finished Client Application with Execution ID: {executionId}")]
        public static partial void LogFinishedClientApplication(this ILogger logger, string executionId);
    }
}
