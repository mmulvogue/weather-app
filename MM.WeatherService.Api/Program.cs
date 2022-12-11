using System.Reflection;
using MM.WeatherService.Api.ApiKeyAuth;
using MM.WeatherService.Api.OpenWeatherMapApi;
using MM.WeatherService.Api.RateLimiter;
using Serilog;
using Serilog.Events;


// Configure SeriLog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Clear standard logging config and add SeriLog provider
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();

    builder.Services.AddControllers();

    // Setup custom api key auth service
    builder.Services.AddApiKeyAuthentication("x-api-key");

    // Setup Swagger/OAS
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(setup =>
    {
        // Use XML comments to populate spec
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        // Add api key security requirement
        setup.AddApiKeySecurityRequirement("x-api-key");
    });

    // Setup application services
    builder.Services.AddSingleton<IOpenWeatherMapApiClient, OpenWeatherMapApiClient>();
    builder.Services.Configure<OpenWeatherMapApiClientOptions>(
        builder.Configuration.GetSection(OpenWeatherMapApiClientOptions.SectionName)
    );
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<IApiClientStatisticsStore, InMemoryApiClientStatisticsStore>();
    var app = builder.Build();


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseExceptionHandler("/error-detailed");
    }
    else
    {
        app.UseExceptionHandler("/error");
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<RateLimitingMiddleware>();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
public partial class Program { }
