using Demo.Api.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Api.Data;

public class WeatherForecastDbContext: DbContext
{
    public DbSet<WeatherForecastRequest> WeatherForecastRequests { get; set; }

    public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherForecastRequest>(ConfigureWeatherForecastRequest);
    }

    private static void ConfigureWeatherForecastRequest(EntityTypeBuilder<WeatherForecastRequest> builder)
    {
        builder.HasKey(wfr => wfr.Id);
        builder.Property(wfr => wfr.Id).ValueGeneratedOnAdd();
        builder.Property(wfr => wfr.RequestedBy).IsRequired().HasMaxLength(100);
        builder.OwnsOne(wfr => wfr.WeatherForecast);
    }
}
