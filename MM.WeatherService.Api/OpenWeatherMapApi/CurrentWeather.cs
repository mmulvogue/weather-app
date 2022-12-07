namespace MM.WeatherService.Api.OpenWeatherMapApi
{
    public class CurrentWeather
    {
        public List<WeatherCondition> Weather { get; set; }
    }

    public class WeatherCondition
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}
