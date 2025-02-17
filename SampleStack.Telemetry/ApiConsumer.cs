using Microsoft.Extensions.Logging;
using SampleStack.Telemetry.Generics.Diagnostics;
using SampleStack.Telemetry.Helpers;
using SampleStack.Telemetry.Logging;

namespace SampleStack.Telemetry
{
    internal class ApiConsumer
    {
        private const string WeatherForecastEndpoint = "/weatherforecast";
        private const string WeatherForecastDetailsEndpoint = "/weatherforecast/details";

        private readonly ILogger<ApiConsumer> logger;
        private readonly HttpClient httpClient;

        public ApiConsumer(ILogger<ApiConsumer> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClient = httpClientFactory.CreateClient("api");
        }

        public async Task RunApiCallsAsync()
        {
            logger.LogStartingClientApplication();

            for (int run = 0; run < 15; run++)
            {
                try
                {
                    using var activity = DiagnosticActivity.StartActivity("Calling API Service");

                    var data = await CallApiEndpoint(WeatherForecastEndpoint);

                    // simulate an error trace span
                    if (data.Contains("Scorching"))
                        await CallApiEndpoint(WeatherForecastDetailsEndpoint);
                }
                catch (Exception ex)
                {
                    logger.LogErrorCallingApi(ex);
                }
                finally
                {
                    ConsoleHelpers.DisplayProgress(run + 1, 15);
                    await Task.Delay(15000);
                }
            }

            logger.LogFinishedClientApplication();
        }

        private async Task<string> CallApiEndpoint(string endpoint)
        {
            var response = await httpClient.GetAsync(endpoint).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();

            return responseData;
        }
    }
}
