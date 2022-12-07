using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet()]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(
            [Required] string city,
            [Required] string country
        )
        {
            _logger.LogDebug("Requested weather for: {city}, {country}", city,country);
            var currentWeather = await _weatherMapApiClient.GetCurrentWeatherForCityAsync(city, country);
            if (currentWeather?.Weather?.Count > 0)
            {
                return Ok(currentWeather?.Weather[0].Description);
            }
            else
            {
                return NotFound();
            }
        }
    }
}