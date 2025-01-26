using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Net;

namespace SampleStack.Telemetry.HttpHandler.Tests
{
    public class CustomHttpHandlerTests
    {
        private readonly ILogger<CustomHttpHandler> _loggerMock;
        private readonly ILoggerFactory _loggerFactoryMock;
        private readonly CustomHttpHandler _customHttpHandler;
        private readonly HttpClient _httpClient;

        public CustomHttpHandlerTests()
        {
            _loggerMock = Substitute.For<ILogger<CustomHttpHandler>>();
            _loggerMock.IsEnabled(Arg.Any<LogLevel>()).Returns(true);

            _loggerFactoryMock = Substitute.For<ILoggerFactory>();
            _loggerFactoryMock.CreateLogger<CustomHttpHandler>().Returns(_loggerMock);

            _customHttpHandler = new CustomHttpHandler(_loggerFactoryMock);

            var mockHttp = new MockHttpMessageHandler();
            var response = new HttpResponseMessage
            {
                Content = new StringContent("Test Content"),
            };

            mockHttp.When("/").Respond(HttpStatusCode.OK, response.Content);
            mockHttp.When("/fail").Respond(HttpStatusCode.InternalServerError, response.Content);

            _httpClient = HttpClientFactory.Create(mockHttp, _customHttpHandler);
            _httpClient.BaseAddress = new Uri("http://localhost");
        }

        [Fact]
        public async Task SendAsync_ShouldAddRequestIdToHeaders()
        {
            // Arrange
            var request = new HttpRequestMessage();

            // Act
            await _httpClient.SendAsync(request, default);

            // Assert
            Assert.True(request.Headers.Contains("X-Request-ID"));
        }

        [Fact]
        public async Task SendAsync_ShouldLogHttpRequestMessage()
        {
            // Arrange
            var request = new HttpRequestMessage();
            var expectedInfoHttpRequest = $"Request: Method: GET, RequestUri: 'http://localhost/', Version: 1.1, Content: <null>";

            // Act
            await _httpClient.SendAsync(request, default!);

            // Assert
            _loggerMock.Received(1).Log(
                Arg.Is<LogLevel>(l => l == LogLevel.Information),
                Arg.Any<EventId>(),
                Arg.Is<object>(x => x.ToString().Contains(expectedInfoHttpRequest)),
                Arg.Is<Exception>(e => e == null),
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public async Task SendAsync_ShouldLogHttpRequestContent()
        {
            // Arrange
            var request = new HttpRequestMessage
            {
                Content = new StringContent("Test Content")
            };
            var expectedDebugHttpRequestContent = string.Empty;

            // Act
            await _httpClient.SendAsync(request, default!);

            // Assert
            var header = request.Headers.GetValues("X-Request-ID").First();
            expectedDebugHttpRequestContent = $"Request Content ({header}):\n Test Content";

            _loggerMock.Received(1).Log(
                Arg.Is<LogLevel>(l => l == LogLevel.Debug),
                Arg.Any<EventId>(),
                Arg.Is<object>(x => x.ToString().Contains(expectedDebugHttpRequestContent)),
                Arg.Is<Exception>(e => e == null),
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public async Task SendAsync_ShouldLogHttpResponse()
        {
            // Arrange
            var request = new HttpRequestMessage();
            var expectedInfoHttpResponse = $"Response: StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StringContent";

            // Act
            await _httpClient.SendAsync(request, default!);

            // Assert
            _loggerMock.Received(1).Log(
                Arg.Is<LogLevel>(l => l == LogLevel.Information),
                Arg.Any<EventId>(),
                Arg.Is<object>(x => x.ToString().Contains(expectedInfoHttpResponse)),
                Arg.Is<Exception>(e => e == null),
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public async Task SendAsync_ShouldLogHttpResponseContent()
        {
            // Arrange
            var request = new HttpRequestMessage();
            var expectedDebugHttpResponseContent = string.Empty;

            // Act
            await _httpClient.SendAsync(request, default!);

            // Assert
            var header = request.Headers.GetValues("X-Request-ID").First();
            expectedDebugHttpResponseContent = $"Response Content ({header}):\n Test Content";

            _loggerMock.Received(1).Log(
                Arg.Is<LogLevel>(l => l == LogLevel.Debug),
                Arg.Any<EventId>(),
                Arg.Is<object>(x => x.ToString().Contains(expectedDebugHttpResponseContent)),
                Arg.Is<Exception>(e => e == null),
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public async Task SendAsync_ShouldLogFailHttpResponseContent()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/fail");
            var expectedFailHttpResponseContent = string.Empty;

            // Act
            await _httpClient.SendAsync(request, default!);

            // Assert
            var header = request.Headers.GetValues("X-Request-ID").First();
            expectedFailHttpResponseContent = $"Response Content ({header}):\n Test Content";

            _loggerMock.Received(1).Log(
                Arg.Is<LogLevel>(l => l == LogLevel.Warning),
                Arg.Any<EventId>(),
                Arg.Is<object>(x => x.ToString().Contains(expectedFailHttpResponseContent)),
                Arg.Is<Exception>(e => e == null),
                Arg.Any<Func<object, Exception?, string>>());
        }
    }
}