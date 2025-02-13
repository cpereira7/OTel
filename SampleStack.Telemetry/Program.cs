using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using SampleStack.Telemetry;
using SampleStack.Telemetry.Configuration;

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
var consumer = host.Services.GetRequiredService<ApiConsumer>();

host.Services.GetRequiredService<TracerProvider>();

// Generate a unique identifier for this execution
var executionId = Guid.NewGuid().ToString();

Console.WriteLine($"Base Address: {httpClient.BaseAddress}");
Console.WriteLine($"Telemetry Address: {configuration.GetConnectionString("OpenTelemetry")}");

await consumer.RunApiCallsAsync();