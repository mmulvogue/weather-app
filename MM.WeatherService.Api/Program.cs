using System.Reflection;
using MM.WeatherService.Api.OpenWeatherMapApi;
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

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });


    // Setup application services
    builder.Services.AddSingleton<IOpenWeatherMapApiClient, OpenWeatherMapApiClient>();
    builder.Services.Configure<OpenWeatherMapApiClientOptions>(
        builder.Configuration.GetSection(OpenWeatherMapApiClientOptions.SectionName)
    );

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

    app.UseAuthorization();

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
