using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using SampleStack.Telemetry.Configuration;
using SampleStack.Telemetry.Diagnostic;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppLogging()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationServices(context.Configuration);
    })
    .Build();

var httpClient = host.Services.GetRequiredService<IHttpClientFactory>().CreateClient("api");
var logger = host.Services.GetRequiredService<ILogger<Program>>();
var configuration = host.Services.GetRequiredService<IConfiguration>();

host.Services.GetRequiredService<TracerProvider>();

logger.LogInformation("Starting Client Application");

Console.WriteLine($"Base Address: {httpClient.BaseAddress}");
Console.WriteLine($"Telemetry Address: {configuration.GetConnectionString("OpenTelemetry")}");

await RunApiCallsAsync(httpClient, logger);

static async Task RunApiCallsAsync(HttpClient httpClient, ILogger logger)
{
    for (int run = 0; run < 15; run++)
    {
        try
        {
            using var activity = DiagnosticActivity.StartActivity("Calling API", run);
            activity?.SetTag("run", run);

            await httpClient.GetAsync("/weatherforecast");

            if (run % 2 == 0)
            {
                Console.WriteLine("Calling 404");
                await httpClient.GetAsync("/weatherforecast/null");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling API");
        }
        finally
        {
            await Task.Delay(15000);
        }
    }
}
