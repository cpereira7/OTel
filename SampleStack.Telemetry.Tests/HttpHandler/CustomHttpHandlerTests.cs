using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;
using SampleStack.Telemetry.HttpHandler;
using SampleStack.Telemetry.Tests.Helpers;
using System.Net;

namespace SampleStack.Telemetry.Tests.HttpHandler
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
            _loggerMock.ShouldHaveLogged(LogLevel.Information, expectedInfoHttpRequest);
        }

        [Fact]
        public async Task SendAsync_ShouldLogHttpRequestContent()
        {
            // Arrange
            var request = new HttpRequestMessage
            {
                Content = new StringContent("Test Content")
            };

            // Act
            await _httpClient.SendAsync(request, default!);

            // Assert
            var header = request.Headers.GetValues("X-Request-ID").First();
            var expectedDebugHttpRequestContent = $"Request Content ({header}):\n Test Content";

            _loggerMock.ShouldHaveLogged(LogLevel.Debug, expectedDebugHttpRequestContent);
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
            _loggerMock.ShouldHaveLogged(LogLevel.Information, expectedInfoHttpResponse);
        }

        [Fact]
        public async Task SendAsync_ShouldLogHttpResponseContent()
        {
            // Arrange
            var request = new HttpRequestMessage();

            // Act
            await _httpClient.SendAsync(request, default!);

            // Assert
            var header = request.Headers.GetValues("X-Request-ID").First();
            var expectedDebugHttpResponseContent = $"Response Content ({header}):\n Test Content";

            _loggerMock.ShouldHaveLogged(LogLevel.Debug, expectedDebugHttpResponseContent);
        }

        [Fact]
        public async Task SendAsync_ShouldLogFailHttpResponseContent()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/fail");

            // Act
            await _httpClient.SendAsync(request, default!);

            // Assert
            var header = request.Headers.GetValues("X-Request-ID").First();
            var expectedFailHttpResponseContent = $"Response Content ({header}):\n Test Content";

            _loggerMock.ShouldHaveLogged(LogLevel.Warning, expectedFailHttpResponseContent);
        }
    }
}