using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SampleStack.Telemetry.Generics.Diagnostics;

namespace SampleStack.Telemetry.Generics.Telemetry
{
    public static class TracesConfiguration
    {
        /// <summary>
        /// Configures OpenTelemetry tracing for the application.
        /// </summary>
        /// <param name="services">The service collection to add the tracing to.</param>
        /// <param name="configuration">The application configuration to use for setting up tracing.</param>
        /// <param name="configureInstrumentation">An action to configure additional instrumentation for the tracer provider.</param>
        public static void ConfigureOpenTelemetryTraces(this IServiceCollection services, IConfiguration configuration, Action<TracerProviderBuilder> configureInstrumentation)
        {
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(DiagnosticNames.ServiceName, serviceVersion: DiagnosticNames.ServiceVersion)
                .AddTelemetrySdk()
                .AddAttributes(DiagnosticNames.Attributes);

            services.AddOpenTelemetry().WithTracing(tracing =>
            {
                tracing.SetResourceBuilder(resourceBuilder)
                    .AddSource(DiagnosticNames.ServiceName)
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(configuration.GetConnectionString("OpenTelemetry")!);
                        otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                    });

                configureInstrumentation(tracing);
            });
        }
    }
}
