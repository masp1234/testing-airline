using backend.Services;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace backend.Tests.Unit
{
    public class DistanceApiServiceUnitTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private DistanceApiService? _sut;
        private readonly string _validOrigin = "New York, NY";
        private readonly string _validDestination = "Los Angeles, CA";

        public DistanceApiServiceUnitTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://maps.googleapis.com/maps/api/distancematrix/json")
            };

            Environment.SetEnvironmentVariable("GOOGLE_DISTANCE_API_KEY", "mock-api-key");
        }

        private void SetupMockHttpResponse(HttpResponseMessage response)
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }

        private HttpResponseMessage CreateMockHttpResponse(object responseObject, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var responseJson = JsonSerializer.Serialize(responseObject);
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseJson)
            };
        }

        [Fact]
        public async Task GetDistanceData_WithValidApiKeyAndValidResponse_ReturnsDistanceValue()
        {
            // Arrange
            _sut = new DistanceApiService(_httpClient);

            var mockResponse = new DistanceApiResponse
            {
                Rows =
                [
                    new Row
                    {
                        Elements =
                        [
                            new Element
                            {
                                Distance = new Distance
                                {   
                                    // 4500 km
                                    Value = 4500000
                                }
                            }
                        ]
                    }
                ]
            };

            var mockHttpResponse = CreateMockHttpResponse(mockResponse);
            SetupMockHttpResponse(mockHttpResponse);

            // Act
            var result = await _sut.GetDistanceData(_validOrigin, _validDestination);

            // Assert
            Assert.Equal(4500000, result);
        }

        [Fact]
        public async Task GetDistanceData_WithNullApiKey_ReturnsDefaultValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("GOOGLE_DISTANCE_API_KEY", null);
            _sut = new DistanceApiService(_httpClient);

            // Act
            var result = await _sut.GetDistanceData(_validOrigin, _validDestination);

            // Assert
            Assert.Equal(2000000.00, result);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("New York, NY", "")]
        [InlineData("", "New York")]
        [InlineData(null, null)]
        [InlineData(null, "New York")]
        [InlineData("New York", null)]
        [InlineData(null, "")]
        [InlineData("", null)]
        public async Task GetDistanceData_WithEmptyOriginOrDestination_ThrowsArgumentNullException(string origin, string destination)
        {
            // Arrange
            _sut = new DistanceApiService(_httpClient);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetDistanceData(origin, destination));
            Assert.Contains("Origin and destination have to be not null and not be an empty string", exception.Message);
        }

        [Fact]
        public async Task GetDistanceData_WithEmptyRows_ThrowsInvalidOperationException()
        {
            // Arrange
            _sut = new DistanceApiService(_httpClient);

            var invalidResponse = new DistanceApiResponse
            {
                Rows = []
            };

            var mockHttpResponse = CreateMockHttpResponse(invalidResponse);
            SetupMockHttpResponse(mockHttpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetDistanceData(_validOrigin, _validDestination));
            Assert.Contains("not contain the expected data", exception.Message);
        }

        [Fact]
        public async Task GetDistanceData_WithNullRowsInApiResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            _sut = new DistanceApiService(_httpClient);

            var invalidResponse = new DistanceApiResponse
            {
                Rows = null
            };

            var mockHttpResponse = CreateMockHttpResponse(invalidResponse);
            SetupMockHttpResponse(mockHttpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetDistanceData(_validOrigin, _validDestination));
            Assert.Contains("does not contain the expected data", exception.Message);
        }

        [Fact]
        public async Task GetDistanceData_WithEmptyElementsInApiResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            _sut = new DistanceApiService(_httpClient);

            var invalidResponse = new DistanceApiResponse
            {
                Rows =
                [
                    new Row
                    {
                        Elements = []
                    }
                ]
            };

            var mockHttpResponse = CreateMockHttpResponse(invalidResponse);
            SetupMockHttpResponse(mockHttpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetDistanceData(_validOrigin, _validDestination));
            Assert.Contains("does not contain the expected data", exception.Message);
        }

        [Fact]
        public async Task GetDistanceData_WithNullElementsInApiResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            _sut = new DistanceApiService(_httpClient);

            var invalidResponse = new DistanceApiResponse
            {
                Rows =
                [
                    new Row
                    {
                        Elements = null
                    }
                ]
            };

            var mockHttpResponse = CreateMockHttpResponse(invalidResponse);
            SetupMockHttpResponse(mockHttpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetDistanceData(_validOrigin, _validDestination));
            Assert.Contains("does not contain the expected data", exception.Message);
        }

        [Fact]
        public async Task GetDistanceData_WithFailedHttpResponse_ThrowsHttpRequestException()
        {
            // Arrange
            _sut = new DistanceApiService(_httpClient);

            var mockHttpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            SetupMockHttpResponse(mockHttpResponse);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _sut.GetDistanceData(_validOrigin, _validDestination));
        }
    }
}
