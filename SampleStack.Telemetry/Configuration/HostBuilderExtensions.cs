using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleStack.Telemetry.Telemetry;
using Serilog;

namespace SampleStack.Telemetry.Configuration
{
    internal static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureAppLogging(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureLogging(logging =>
            {
                logging.Configure(options =>
                {
                    options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId | ActivityTrackingOptions.TraceId;
                });
            })
            .UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
                configuration.ConfigureOpenTelemetryLogging(context.Configuration);
            });

            return hostBuilder;
        }
    }
}
