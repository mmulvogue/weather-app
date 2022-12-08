using System.Reflection;
using Microsoft.OpenApi.Models;
using MM.WeatherService.Api.ApiKeyAuth;
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
    builder.Services.AddSwaggerGen(setup =>
    {
        // Use XML comments to populate open api spec
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        // Add api key security scheme
        setup.AddSecurityDefinition(ApiKeyAuthenticationOptions.SchemeName, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "x-api-key",
            Type = SecuritySchemeType.ApiKey
        });

        // Require API key auth for all endpoints
        setup.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = ApiKeyAuthenticationOptions.SchemeName
                    }
                },
                Array.Empty<string>()
            }
        });
    });


    // Setup application services
    builder.Services.AddSingleton<IOpenWeatherMapApiClient, OpenWeatherMapApiClient>();
    builder.Services.Configure<OpenWeatherMapApiClientOptions>(
        builder.Configuration.GetSection(OpenWeatherMapApiClientOptions.SectionName)
    );

    builder.Services.AddAuthentication(ApiKeyAuthenticationOptions.SchemeName)
        .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            ApiKeyAuthenticationOptions.SchemeName,
            (options) => { options.HeaderName = "x-api-key"; }
        );
    builder.Services.AddTransient<IApiKeyService, ApiKeyService>();

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
