using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleStack.Telemetry.HttpHandler;
using SampleStack.Telemetry.Generics.Telemetry;
using OpenTelemetry.Trace;

namespace SampleStack.Telemetry.Configuration
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<CustomHttpHandler>();

            var httpClientConfiguration = new Action<HttpClient>(c =>
            {
                var apiAddress = Environment.GetEnvironmentVariable("API_HOST") ?? "localhost";
                var apiPort = Environment.GetEnvironmentVariable("API_HTTP_PORT") ?? "8080";

                c.BaseAddress = new Uri($"http://{apiAddress}:{apiPort}");
                c.Timeout = TimeSpan.FromSeconds(60);
            });

            var httpClientHandlerConfiguration = new HttpClientHandler
            {
                // Ignore Server Certificate Validation
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            services.AddHttpClient("api", httpClientConfiguration)
                .AddHttpMessageHandler<CustomHttpHandler>()
                .ConfigurePrimaryHttpMessageHandler(() => httpClientHandlerConfiguration);

            services.ConfigureOpenTelemetryTraces(configuration, tracing =>
            {
                tracing.AddHttpClientInstrumentation(options =>
                {
                    options.EnrichWithHttpRequestMessage = (activity, request) =>
                    {
                        if (request.Headers.TryGetValues("X-Request-ID", out var requestIds))
                        {
                            activity?.SetTag("X-Request-ID", string.Join(",", requestIds));
                        }
                    };
                });
            });

            services.AddScoped<ApiConsumer>();
        }
    }
}
