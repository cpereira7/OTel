using Microsoft.Extensions.Configuration;
using SampleStack.Telemetry.Generics.Diagnostics;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace SampleStack.Telemetry.Generics.Telemetry
{
    public static class LoggingConfiguration
    {
        /// <summary>
        /// Configures OpenTelemetry logging for the provided <see cref="LoggerConfiguration"/>.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration to apply OpenTelemetry settings to.</param>
        /// <param name="configuration">The application configuration containing connection strings and other settings.</param>
        public static void ConfigureOpenTelemetryLogging(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            loggerConfiguration.WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = configuration.GetConnectionString("OpenTelemetry");
                options.Protocol = OtlpProtocol.Grpc;

                options.ResourceAttributes = DiagnosticNames.Attributes;

                options.IncludedData = IncludedData.MessageTemplateTextAttribute |
                    IncludedData.TraceIdField | IncludedData.SpanIdField;

                options.BatchingOptions.BatchSizeLimit = 700;
                options.BatchingOptions.BufferingTimeLimit = TimeSpan.FromSeconds(5);
                options.BatchingOptions.QueueLimit = 10;
            })
            .Enrich.FromLogContext()
            .Enrich.With<ActivityEnricher>();
        }
    }
}
