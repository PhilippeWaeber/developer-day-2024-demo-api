using Demo.Api.Data;
using Demo.Api.Data.Models;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

try
{
    Log.Information("Start configuring Demo.API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddWeatherForecastDbContext(
        builder.Configuration.GetConnectionString("WeatherForecastDbContext")!);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    app.MapGet("/weatherforecast",
               async (WeatherForecastDbContext dbContext,
                   ILogger<WeatherForecast> logger) =>
               {
                   var forecasts = Enumerable.Range(1, 5)
                       .Select(index =>
                                   new WeatherForecast
                                   {
                                       Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                       TemperatureC = Random.Shared.Next(-20, 55),
                                       Summary = summaries[Random.Shared.Next(summaries.Length)]
                                   })
                       .ToList();

                   logger.LogDebug("Generated weather forecast: {@forecast}", forecasts);

                   var requestedOn = DateTime.Now;
                   foreach (var forecast in forecasts)
                   {
                       dbContext.WeatherForecastRequests
                           .Add(new WeatherForecastRequest
                           {
                               WeatherForecast = forecast,
                               RequestedBy = Environment.UserName,
                               RequestedOn = requestedOn
                           });
                   }
                   await dbContext.SaveChangesAsync();

                   return forecasts;
               })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

    Log.Information("Finished configuring Demo.API. Run it.");

    app.EnsureDatabaseCreated();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}