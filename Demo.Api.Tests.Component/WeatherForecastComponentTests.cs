using System.Net;
using System.Text.Json;
using Demo.Api.Data;
using Demo.Api.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Api.Tests.Component;

public class WeatherForecastTests : IClassFixture<ComponentTestsWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IServiceProvider _services;

    public WeatherForecastTests(ComponentTestsWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _services = factory.Services;
    }

    [Fact]
    public async Task GetWeatherForecasts_ShouldReturnWeatherForecasts()
    {
        // Arrange
        const string endpoint = "weatherforecast";

        // Act
        var response = await _client.GetAsync(endpoint);
        var contentString = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<IReadOnlyCollection<WeatherForecast>>(contentString);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(content);
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GetWeatherForecasts_ShouldCreateRequestsInDb()
    {
        // Arrange
        const string endpoint = "weatherforecast";
        var dbContext = _services.CreateScope().ServiceProvider.GetRequiredService<WeatherForecastDbContext>();

        // Act
        var beforeRequest = DateTime.Now;
        var response = await _client.GetAsync(endpoint);
        var afterRequest = DateTime.Now;
        var contentString = await response.Content.ReadAsStringAsync();
        var forecasts = JsonSerializer.Deserialize<IReadOnlyCollection<WeatherForecast>>(contentString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var forecastRequestsInDb = await dbContext.WeatherForecastRequests
            .Where(request => request.RequestedOn > beforeRequest
                              && request.RequestedOn < afterRequest
                              && request.RequestedBy == Environment.UserName)
            .ToListAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(forecasts);
        Assert.NotEmpty(forecasts);

        Assert.NotNull(forecastRequestsInDb);
        Assert.NotEmpty(forecastRequestsInDb);

        Assert.Equal(forecasts.Count, forecastRequestsInDb.Count);

        foreach (var forecast in forecasts)
        {
            Assert.Contains(forecastRequestsInDb, request => request.WeatherForecast.Date == forecast.Date
                                                && request.WeatherForecast.TemperatureC == forecast.TemperatureC
                                                && request.WeatherForecast.TemperatureF == forecast.TemperatureF
                                                && request.WeatherForecast.Summary == forecast.Summary);
        }
    }
}