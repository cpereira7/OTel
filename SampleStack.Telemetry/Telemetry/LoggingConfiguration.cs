using Microsoft.Extensions.Configuration;
using SampleStack.Telemetry.Generics.Diagnostics;
using SampleStack.Telemetry.Generics.Telemetry;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace SampleStack.Telemetry.Telemetry
{
    internal static class LoggingConfiguration
    {
        internal static void ConfigureOpenTelemetryLogging(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
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
