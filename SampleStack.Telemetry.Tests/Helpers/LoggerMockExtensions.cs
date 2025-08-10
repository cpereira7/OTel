using Microsoft.Extensions.Logging;
using NSubstitute;

namespace SampleStack.Telemetry.Tests.Helpers
{
    public static class LoggerMockExtensions
    {
        public static void ShouldHaveLogged(
            this ILogger loggerMock,
            LogLevel expectedLevel,
            string expectedMessagePart,
            Exception? expectedException = null)
        {
            loggerMock.Received(1).Log(
                Arg.Is<LogLevel>(l => l == expectedLevel),
                Arg.Any<EventId>(),
                Arg.Is<object>(state =>
                    state != null &&
                    state.ToString()!.Contains(expectedMessagePart, StringComparison.Ordinal)),
                Arg.Is<Exception>(e => e == expectedException),
                Arg.Any<Func<object, Exception?, string>>()
            );
        }
    }
}
