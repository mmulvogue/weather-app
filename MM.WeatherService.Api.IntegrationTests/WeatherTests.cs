using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MM.WeatherService.Api.IntegrationTests
{
    public class WeatherTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public WeatherTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetWeather_WithoutApiKey_GivesUnauthorizedStatus()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("weather?city=Melbourne&country=Australia");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetWeather_WithInvalidApiKey_GivesUnauthorizedStatus()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", "API-invalid");

            // Act
            var response = await client.GetAsync("weather?city=Melbourne&country=Australia");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact]
        public async Task GetWeather_WithFiveRequests_Returns200ForAllRequests()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", "API-dummy.key.1");

            // Act
            List<Task<HttpResponseMessage>> respTasks = new();
            foreach (var i in Enumerable.Range(1, 5))
            {
                respTasks.Add(client.GetAsync("weather?city=Melbourne&country=Australia"));
            }
            var responses = await Task.WhenAll(respTasks);

            // Assert
            Assert.Equal(5, responses.Length);
            Assert.All(responses.Select(x => x.StatusCode), code => Assert.Equal(HttpStatusCode.OK, code));
        }

        [Fact]
        public async Task GetWeather_WithSixRequests_Returns429ForOneRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", "API-dummy.key.2");

            // Act
            ConcurrentBag<Task<HttpResponseMessage>> respTasks = new();
            Parallel.ForEach(Enumerable.Range(1, 6), idx =>
            {
                respTasks.Add(client.GetAsync("weather?city=Melbourne&country=Australia"));
            });
            var responses = await Task.WhenAll(respTasks);

            // Assert
            Assert.Equal(6, responses.Length);
            var rateLimitedResp = responses.Where(x => x.StatusCode == HttpStatusCode.TooManyRequests).ToArray();
            Assert.Single(rateLimitedResp);
        }
    }
}