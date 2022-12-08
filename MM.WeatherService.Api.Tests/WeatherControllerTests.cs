using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MM.WeatherService.Api.Controllers;
using MM.WeatherService.Api.OpenWeatherMapApi;
using MM.WeatherService.Api.OpenWeatherMapApi.Models;
using Moq;

namespace MM.WeatherService.Api.Tests
{
    public class WeatherControllerTests
    {
        [Fact]
        public async Task GetCurrentWeatherAsync_WhenNoWeatherData_ReturnsNotFound()
        {
            //Arrange
            var mockWeatherApiClient = new Mock<IOpenWeatherMapApiClient>();
            mockWeatherApiClient.Setup(x => x.GetCurrentWeatherForCityAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => null);

            //Act
            var controller = new WeatherController(
                new Mock<ILogger<WeatherController>>().Object,
                mockWeatherApiClient.Object
            );
            var actionResult = (await controller.GetCurrentWeatherAsync("dummyCity", "dummyCountry"));

            //Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_WhenWeatherDataFound_ReturnsOkResult()
        {
            //Arrange
            var dummyWeatherData = new CurrentWeather
            {
                Weather = new[] {
                    new WeatherCondition
                    {
                        Description = "Dummy weather description"
                    }
                }
            };
            var mockWeatherApiClient = new Mock<IOpenWeatherMapApiClient>();
            mockWeatherApiClient.Setup(x => x.GetCurrentWeatherForCityAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => dummyWeatherData);

            //Act
            var controller = new WeatherController(
                new Mock<ILogger<WeatherController>>().Object,
                mockWeatherApiClient.Object
            );
            var actionResult = (await controller.GetCurrentWeatherAsync("dummyCity", "dummyCountry"));

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);
        }
    }
}