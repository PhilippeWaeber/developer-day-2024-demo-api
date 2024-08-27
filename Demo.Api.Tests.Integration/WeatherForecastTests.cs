using System.Net;
using System.Text.Json;
using Demo.Api.Data.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Demo.Api.Tests.Integration;

public class WeatherForecastTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WeatherForecastTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
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
}