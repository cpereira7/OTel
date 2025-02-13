using Microsoft.Extensions.Logging;
using SampleStack.Telemetry.Diagnostic;
using SampleStack.Telemetry.Helpers;

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
            logger.LogInformation("Starting Client Application with Execution ID: {ExecutionId}", executionId);

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
                    logger.LogError(ex, "Error calling API on run {RunNumber} with Execution ID: {ExecutionId}", run, executionId);
                }
                finally
                {
                    await Task.Delay(15000);
                }
            }

            logger.LogInformation("Finished Client Application with Execution ID: {ExecutionId}", executionId);
        }

        async Task CallApiEndpoint(string endpoint, int run)
        {
            using var activity = DiagnosticActivity.StartActivity("Calling API", run);
            activity?.SetTag("ExecutionId", executionId);

            logger.LogInformation("Calling API #{RunNumber}: {Endpoint} with Execution ID: {ExecutionId}", run, endpoint, executionId);

            await httpClient.GetAsync(endpoint);

            activity?.Dispose();
        }
    }
}
