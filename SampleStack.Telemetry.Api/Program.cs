using OpenTelemetry.Trace;
using SampleStack.Telemetry.Api;
using SampleStack.Telemetry.Generics.Telemetry;
using SampleStack.Telemetry.Generics.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Host.ConfigureAppLogging();

builder.Services.ConfigureOpenTelemetryTraces(builder.Configuration, tracing =>
{
    tracing.AddAspNetCoreInstrumentation(options =>
    {
        options.EnrichWithHttpRequest = (activity, httpRequest) =>
        {
            if (httpRequest.Headers.TryGetValue("X-Request-ID", out var requestId))
            {
                activity?.SetTag("X-Request-ID", requestId.ToString());
            }
        };
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();
