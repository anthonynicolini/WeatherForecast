using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using WeatherForecast.Core.Interfaces;
using WeatherForecast.Core.Models;
using Xunit;

namespace ApiTests.Controllers;

public class ForecastControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program>
        _factory;

    public ForecastControllerTests(
        CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        
        PreSeedData().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Post_ReturnsCreated()
    {
        // Arrange
        var postModel = new ForecastModel
        {
            Latitude = 32,
            Longitude = -15,
            Timezone = "UTC",
            HourlyTemperature = new decimal[] { 1, 2 },
            HourlyTime = new[] { DateTime.Now.AddHours(-1), DateTime.Now }
        };

        //Act
        var response = await _client.PostAsJsonAsync("forecasts", postModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task Post_ReturnsBadRequest()
    {
        // Arrange
        var postModel = new ForecastModel
        {
            Latitude = 91,
            Longitude = 181,
        };

        //Act
        var response = await _client.PostAsJsonAsync("forecasts", postModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(40.73, -73.93)] // NY
    [InlineData(51.50, -0.12)] // London
    [InlineData(48.86,  2.34)] // Paris
    public async Task Get_RetrievesFromApi(decimal latitude, decimal longitude)
    {
        // Arrange
        var postModel = new ForecastModel
        {
            Latitude = latitude,
            Longitude = longitude,
        };

        //Act
        var response = await _client.GetFromJsonAsync<ForecastModel>($"forecasts/{latitude}&{longitude}");

        // Assert
        response.Should().NotBeNull();
        response.HourlyTemperature.Should().NotBeEmpty();
        response.HourlyTime.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task GetAll_ReturnsSomething()
    {
        // Arrange

        //Act
        var response = await _client.GetFromJsonAsync<IEnumerable<ForecastModel>>($"forecasts");

        // Assert
        response.Should().NotBeNull();
        response.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task Put_ReturnsCreatedOrOk()
    {
        // Arrange
        var put1 = new ForecastModel
        {
            Latitude = 33,
            Longitude = -16,
            Timezone = "UTC",
            HourlyTemperature = new decimal[] { 1, 2 },
            HourlyTime = new[] { DateTime.Now.AddHours(-1), DateTime.Now }
        };
        var put2 = new ForecastModel
        {
            Latitude = 33,
            Longitude = -16,
            Timezone = "UTC",
            HourlyTemperature = new decimal[] { 1, 2, 3 },
            HourlyTime = new[] { DateTime.Now.AddHours(-1), DateTime.Now, DateTime.Now.AddHours(1) }
        };

        //Act
        var response1 = await _client.PutAsJsonAsync("forecasts", put1);
        var cont = response1.Content.ReadAsStringAsync();
        var content1 = await response1.Content.ReadFromJsonAsync<ForecastModel>();
        
        var response2 = await _client.PutAsJsonAsync("forecasts", put2);
        var content2 = await response2.Content.ReadFromJsonAsync<ForecastModel>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        content1.HourlyTime.Count.Should().Be(2);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        content2.HourlyTime.Count.Should().Be(3);
    }
    
    [Fact]
    public async Task Put_ReturnsBadRequest()
    {
        // Arrange
        var badModel = new ForecastModel
        {
            Latitude = 91,
            Longitude = 181,
        };

        //Act
        var response = await _client.PutAsJsonAsync("forecasts", badModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(1, 1, HttpStatusCode.NoContent)]
    [InlineData(15, 15, HttpStatusCode.NotFound)]
    [InlineData(91, 181, HttpStatusCode.BadRequest)]
    public async Task Delete_ReturnsExpected(decimal latitude, decimal longitude, HttpStatusCode expected)
    {
        // Arrange
        //Act
        var response = await _client.DeleteAsync($"forecasts/{latitude}&{longitude}");

        // Assert
        response.StatusCode.Should().Be(expected);
    }
    
    private async Task PreSeedData()
    {
        var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IForecastRepository>();
        var existing1 = new ForecastModel
        {
            Latitude = 1,
            Longitude = 1,
            HourlyTemperature = new decimal[] { 1, 2 },
            HourlyTime = new[] { DateTime.Now.AddHours(-1), DateTime.Now }
        };
        await repo.AddOrUpdateAsync(existing1, CancellationToken.None);
        var existing2 = new ForecastModel
        {
            Latitude = 2,
            Longitude = 2,
            HourlyTemperature = new decimal[] { 1, 2 },
            HourlyTime = new[] { DateTime.Now.AddHours(-1), DateTime.Now }
        };
        await repo.AddOrUpdateAsync(existing2, CancellationToken.None);
    }
}