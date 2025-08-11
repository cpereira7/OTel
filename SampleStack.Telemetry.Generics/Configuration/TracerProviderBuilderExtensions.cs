using OpenTelemetry.Trace;

namespace SampleStack.Telemetry.Generics.Configuration
{
    public static partial class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder AddRequestIdHttpClientEnrichment(this TracerProviderBuilder tracing)
        {
            return tracing.AddHttpClientInstrumentation(options =>
            {
                options.EnrichWithHttpRequestMessage = (activity, request) =>
                {
                    if (request.Headers.TryGetValues("X-Request-ID", out var requestIds))
                    {
                        activity?.SetTag("X-Request-ID", string.Join(",", requestIds));
                    }
                };
            });
        }
    }
}
