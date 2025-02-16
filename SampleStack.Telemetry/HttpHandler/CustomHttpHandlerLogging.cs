using Microsoft.Extensions.Logging;

namespace SampleStack.Telemetry.HttpHandler
{
    public static partial class CustomHttpHandlerLogging
    {
        [LoggerMessage(LogLevel.Information, "Request: {requestMessage}")]
        public static partial void LogInfoHttpRequest(this ILogger logger, HttpRequestMessage requestMessage);

        [LoggerMessage(LogLevel.Debug, "Request Content ({requestId}):\n {content}")]
        public static partial void LogDebugHttpRequestContent(this ILogger logger, string requestId, string content);

        [LoggerMessage(LogLevel.Information, "Response: {responseMessage}")]
        public static partial void LogInfoHttpResponse(this ILogger logger, HttpResponseMessage responseMessage);

        [LoggerMessage(LogLevel.Debug, "Response Content ({requestId}):\n {content}")]
        public static partial void LogDebugHttpResponseContent(this ILogger logger, string requestId, string content);

        [LoggerMessage(LogLevel.Warning, "Response Content ({requestId}):\n {content}")]
        public static partial void LogFailHttpResponseContent(this ILogger logger, string requestId, string content);
    }
}
