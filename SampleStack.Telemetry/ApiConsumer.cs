using Microsoft.Extensions.Logging;
using SampleStack.Telemetry.Diagnostic;
using SampleStack.Telemetry.Helpers;
using SampleStack.Telemetry.Logging;

namespace SampleStack.Telemetry
{
    internal class ApiConsumer
    {
        readonly ILogger<ApiConsumer> logger;
        readonly HttpClient httpClient;
        readonly string executionId;

        public ApiConsumer(ILogger<ApiConsumer> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClient = httpClientFactory.CreateClient("api");
            this.executionId = Guid.NewGuid().ToString();
        }

        public async Task RunApiCallsAsync()
        {
            logger.LogStartingClientApplication(executionId);

            for (int run = 0; run < 15; run++)
            {
                try
                {
                    await CallApiEndpoint("/weatherforecast", run);

                    if (run % 2 == 0)
                    {
                        // simulate an error trace
                        await CallApiEndpoint("/weatherforecast/null", run);
                    }

                    ConsoleHelpers.DisplayProgress(run + 1, 15);
                }
                catch (Exception ex)
                {
                    logger.LogErrorCallingApi(ex, run, executionId);
                }
                finally
                {
                    await Task.Delay(15000);
                }
            }

            logger.LogFinishedClientApplication(executionId);
        }

        async Task CallApiEndpoint(string endpoint, int run)
        {
            using var activity = DiagnosticActivity.StartActivity("Calling API", run);
            activity?.SetTag("ExecutionId", executionId);

            logger.LogCallingApi(run, endpoint, executionId);

            await httpClient.GetAsync(endpoint);

            activity?.Dispose();
        }
    }
}
