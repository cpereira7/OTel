using SampleStack.Telemetry.Generics.Configuration;
using SampleStack.Telemetry.Generics.Telemetry;

namespace SampleStack.Telemetry.Api.Configuration
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureOpenTelemetryTraces(configuration, tracing =>
            {
                tracing.AddRequestIdAspNetCoreEnrichment();
            });
        }
    }
}
