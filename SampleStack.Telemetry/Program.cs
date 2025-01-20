using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleStack.Telemetry.Http;

Console.WriteLine("Hello, World!");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddScoped<CustomHttpHandler>();

        var httpClientConfiguration = new Action<HttpClient>(c =>
        {
            c.BaseAddress = new Uri("http://localhost:5225/");
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
    })
    .Build();

var httpClient = host.Services.GetRequiredService<IHttpClientFactory>().CreateClient("api");


var response = await httpClient.GetAsync("/weatherforecast");

var responseContent = await response.Content.ReadAsStringAsync();
Console.WriteLine(response.Headers);
Console.WriteLine(responseContent);