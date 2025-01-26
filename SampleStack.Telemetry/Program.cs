using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using SampleStack.Telemetry.Configuration;
using SampleStack.Telemetry.Diagnostic;
using SampleStack.Telemetry.HttpHandler;
using SampleStack.Telemetry.Telemetry;
using Serilog;

Console.WriteLine("Hello, World!");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppLogging()
    .ConfigureServices((context, services) =>
    {
        services.AddScoped<CustomHttpHandler>();

        var httpClientConfiguration = new Action<HttpClient>(c =>
        {
            var apiAdress = Environment.GetEnvironmentVariable($"API_HOST") ?? "localhost";
            var apiPort = Environment.GetEnvironmentVariable($"API_HTTP_PORT") ?? "8080";

            c.BaseAddress = new Uri($"http://{apiAdress}:{apiPort}");
            c.Timeout = TimeSpan.FromSeconds(60);
        });

        var httpClientHandlerConfiguration = new HttpClientHandler
        {
            // Ignore Server Certificate Validation
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
        };

        services.AddHttpClient("api", httpClientConfiguration)
            .AddHttpMessageHandler<CustomHttpHandler>()
            .ConfigurePrimaryHttpMessageHandler(() => httpClientHandlerConfiguration);

        services.ConfigureOpenTelemetryTraces(context.Configuration);
    })
    .Build();

var httpClient = host.Services.GetRequiredService<IHttpClientFactory>().CreateClient("api");
var logger = host.Services.GetRequiredService<ILogger<Program>>();
host.Services.GetRequiredService<TracerProvider>();

logger.LogInformation("Starting application");

Console.WriteLine("Calling API");

while (true)
{

    _ = DiagnosticActivity.StartActivity("Calling API");

    var response = await httpClient.GetAsync("/weatherforecast");

    var responseContent = await response.Content.ReadAsStringAsync();
    Console.WriteLine(response.Headers);
    Console.WriteLine(responseContent);

    Console.WriteLine("Calling 404");

    _ = await httpClient.GetAsync("/weatherforecast/null");

    Thread.Sleep(5000);
}