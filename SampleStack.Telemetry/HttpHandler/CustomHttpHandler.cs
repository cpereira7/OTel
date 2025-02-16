using Microsoft.Extensions.Logging;

namespace SampleStack.Telemetry.HttpHandler
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

            _logger.LogInfoHttpRequest(request);

            if (request.Content != null && _logger.IsEnabled(LogLevel.Debug))
            {
                var content = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogDebugHttpRequestContent(requestId, content);
            }

            HttpResponseMessage responseMessage = await base.SendAsync(request, cancellationToken);

            _logger.LogInfoHttpResponse(responseMessage);

            if (responseMessage.Content != null && (_logger.IsEnabled(LogLevel.Debug) || !responseMessage.IsSuccessStatusCode))
            {
                var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                _logger.LogDebugHttpResponseContent(requestId, content);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    _logger.LogFailHttpResponseContent(requestId, content);
                }
            }

            return responseMessage;
        }
    }
}
