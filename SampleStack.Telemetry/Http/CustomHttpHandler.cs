using Microsoft.Extensions.Logging;
using SampleStack.Telemetry.Logging;

namespace SampleStack.Telemetry.Http
{
    internal class CustomHttpHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public CustomHttpHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CustomHttpHandler>();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return SendAsyncInternal(request, cancellationToken);
        }

        private async Task<HttpResponseMessage> SendAsyncInternal(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestId = Guid.NewGuid().ToString();
            request.Headers.Add("X-Request-ID", requestId);

            _logger.InfoHttpRequest(request);

            if (request.Content != null && _logger.IsEnabled(LogLevel.Debug))
            {
                var content = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.DebugHttpRequestContent(requestId, content);
            }

            HttpResponseMessage responseMessage = await base.SendAsync(request, cancellationToken);

            _logger.InfoHttpResponse(responseMessage);

            if (responseMessage.Content != null && (_logger.IsEnabled(LogLevel.Debug) || !responseMessage.IsSuccessStatusCode))
            {
                var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                _logger.DebugHttpResponseContent(requestId, content);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    _logger.FailHttpResponseContent(requestId, content);
                }
            }

            return responseMessage;
        }
    }
}
