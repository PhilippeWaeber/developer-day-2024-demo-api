using Microsoft.EntityFrameworkCore;

namespace Demo.Api.Data;

public static class DataExtensions
{
    public static IServiceCollection AddWeatherForecastDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<WeatherForecastDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }

    public static IHost EnsureDatabaseCreated(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        using var dbContext = services.GetRequiredService<WeatherForecastDbContext>();
        dbContext.Database.EnsureCreated();

        return host;
    }
}
