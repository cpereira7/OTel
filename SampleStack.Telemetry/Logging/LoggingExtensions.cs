using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleStack.Telemetry.Logging
{
    public static partial class LoggingExtensions
    {
        [LoggerMessage(LogLevel.Information, "Request: {requestMessage}")]
        public static partial void InfoHttpRequest(this ILogger logger, HttpRequestMessage requestMessage);

        [LoggerMessage(LogLevel.Debug, "Request Content ({requestId}):\n {content}")]
        public static partial void DebugHttpRequestContent(this ILogger logger, string requestId, string content);

        [LoggerMessage(LogLevel.Information, "Response: {responseMessage}")]
        public static partial void InfoHttpResponse(this ILogger logger, HttpResponseMessage responseMessage);

        [LoggerMessage(LogLevel.Debug, "Response Content ({requestId}):\n {content}")]
        public static partial void DebugHttpResponseContent(this ILogger logger, string requestId, string content);

        [LoggerMessage(LogLevel.Warning, "Response Content ({requestId}):\n {content}")]
        public static partial void FailHttpResponseContent(this ILogger logger, string requestId, string content);
    }
}
