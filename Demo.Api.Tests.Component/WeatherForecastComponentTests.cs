using System.Net;
using System.Text.Json;
using Demo.Api.Data.Models;

namespace Demo.Api.Tests.Component;

public class WeatherForecastTests : IClassFixture<ComponentTestsWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WeatherForecastTests(ComponentTestsWebApplicationFactory factory)
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