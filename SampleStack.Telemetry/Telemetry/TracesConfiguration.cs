using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SampleStack.Telemetry.Generics.Diagnostics;

namespace SampleStack.Telemetry.Telemetry
{
    internal static class TracesConfiguration
    {
        [Obsolete("Method is deprecated, please the method of the Generics Library instead.", false)]
        internal static void ConfigureOpenTelemetryTraces(this IServiceCollection services, IConfiguration configuration)
        {
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(DiagnosticNames.ServiceName, serviceVersion: DiagnosticNames.ServiceVersion)
                .AddTelemetrySdk()
                .AddAttributes(DiagnosticNames.Attributes);

            services.AddOpenTelemetry().WithTracing(tracing => tracing
                .SetResourceBuilder(resourceBuilder)
                .AddSource(DiagnosticNames.ServiceName)
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(configuration.GetConnectionString("OpenTelemetry"));
                    otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                }));
        }
    }
}
