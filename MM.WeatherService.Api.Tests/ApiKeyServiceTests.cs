using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MM.WeatherService.Api.ApiKeyAuth;
using MM.WeatherService.Api.Controllers;
using MM.WeatherService.Api.OpenWeatherMapApi;
using MM.WeatherService.Api.OpenWeatherMapApi.Models;
using Moq;

namespace MM.WeatherService.Api.Tests;

public class ApiKeyServiceTests
{
    [Fact]
    public async Task IsValidAsync_WhenApiKeyPrefixIsIncorrect_ReturnsFalse()
    {
        //Arrange
        var mockApiKeyStore = new Mock<IApiKeyStore>();

        //Act
        var svc = new ApiKeyService(
            mockApiKeyStore.Object
        );
        var result = await svc.IsValidKeyAsync("NotCorrectPrefix-api.key.1");

        //Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsValidAsync_WhenApiKeyIsValid_ReturnsTrue()
    {
        //Arrange
        var validApiKey = "API-valid.api.key";
        var mockApiKeyStore = new Mock<IApiKeyStore>();
        mockApiKeyStore.Setup(x => x.ExistsAsync(It.IsIn(validApiKey))).ReturnsAsync(true);

        //Act
        var svc = new ApiKeyService(
            mockApiKeyStore.Object
        );
        var result = await svc.IsValidKeyAsync(validApiKey);

        //Assert
        Assert.True(result);
    }
}
