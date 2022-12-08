using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MM.WeatherService.Api.Models;
using MM.WeatherService.Api.OpenWeatherMapApi;

namespace MM.WeatherService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly IOpenWeatherMapApiClient _weatherMapApiClient;

        public WeatherController(
            ILogger<WeatherController> logger,
            IOpenWeatherMapApiClient weatherMapApiClient
        )
        {
            _logger = logger;
            _weatherMapApiClient = weatherMapApiClient;
        }

        /// <summary>
        /// Returns a description of the current weather for the requested city
        /// </summary>
        /// <param name="city">Name of city to retrieve weather for</param>
        /// <param name="country">Short or full name of country the city is in</param>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentWeatherAsync(
            [Required] [FromQuery] string city,
            [Required] [FromQuery] string country
        )
        {
            _logger.LogDebug("Requested weather for: {city}, {country}", city, country);

            var currentWeather = await _weatherMapApiClient.GetCurrentWeatherForCityAsync(city, country);
            if (currentWeather?.Weather?.Length > 0)
            {
                return Ok(new WeatherDto()
                {
                    Description = currentWeather.Weather.First().Description
                });
            }

            return NotFound();
        }
    }
}