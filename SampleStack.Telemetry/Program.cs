using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using SampleStack.Telemetry;
using SampleStack.Telemetry.Configuration;
using SampleStack.Telemetry.Generics.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppLogging()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationServices(context.Configuration);
    })
    .Build();

var configuration = host.Services.GetRequiredService<IConfiguration>();
var consumer = host.Services.GetRequiredService<ApiConsumer>();

host.Services.GetRequiredService<TracerProvider>();

Console.WriteLine($"Telemetry Address: {configuration.GetConnectionString("OpenTelemetry")}");

await consumer.RunApiCallsAsync();