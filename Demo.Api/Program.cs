using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Start configuring Demo.API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

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
               (ILogger<WeatherForecast> logger) =>
               {
                   var forecast = Enumerable.Range(1, 5)
                       .Select(index =>
                                   new WeatherForecast(
                                       DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                       Random.Shared.Next(-20, 55),
                                       summaries[Random.Shared.Next(summaries.Length)]
                                   ))
                       .ToList();

                   logger.LogDebug("Generated weather forecast: {forecast}", forecast);

                   return forecast;
               })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

    Log.Information("Finished configuring Demo.API. Run it.");

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

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}