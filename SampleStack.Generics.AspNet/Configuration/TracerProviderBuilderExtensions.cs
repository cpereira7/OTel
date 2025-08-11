using OpenTelemetry.Trace;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SampleStack.Telemetry.Generics.Configuration
{
    public static partial class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder AddRequestIdAspNetCoreEnrichment(this TracerProviderBuilder tracing)
        {
            return tracing.AddAspNetCoreInstrumentation(options =>
            {
                options.EnrichWithHttpRequest = (activity, httpRequest) =>
                {
                    if (httpRequest.Headers.TryGetValue("X-Request-ID", out var requestId))
                    {
                        activity?.SetTag("X-Request-ID", requestId.ToString());
                    }
                };
            });
        }
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure

