using FluentAssertions;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using WeatherForecast.Core;
using WeatherForecast.Core.Interfaces;
using WeatherForecast.Core.Models;

namespace UnitTests;

public class ForecastServiceTests
{
    private readonly ForecastService _sut;
    private readonly IForecastRepository _repository;
    private readonly IWeatherApi _apiClient;
    private readonly ISystemClock _clock;

    public ForecastServiceTests()
    {
        _repository = Substitute.For<IForecastRepository>();
        _apiClient = Substitute.For<IWeatherApi>();
        _clock = Substitute.For<ISystemClock>();
        var logger = NullLogger<ForecastService>.Instance;

        _sut = new ForecastService(_repository, _apiClient, _clock, logger);
    }


    [Fact]
    public async Task ReturnsIfRecentDataFoundInDatabase()
    {
        //arrange
        _repository
            .FindAsync(Arg.Any<decimal>(), Arg.Any<decimal>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new ForecastModel { UpdatedOn = DateTimeOffset.UtcNow } );
        _clock.UtcNow.Returns(DateTimeOffset.UtcNow);
        
        //act
        var result = await _sut.ProvideForecastAsync(1, 2, CancellationToken.None);
        //assert

        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task RetrieveFromApiWhenNotFound()
    {
        //arrange
        _clock.UtcNow.Returns(DateTimeOffset.UtcNow);
        _apiClient
            .GetForecastAsync(Arg.Any<decimal>(), Arg.Any<decimal>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new ForecastModel());
        
        //act
        var result = await _sut.ProvideForecastAsync(1, 2, CancellationToken.None);
        
        //assert
        await _apiClient.Received().GetForecastAsync(1, 2, CancellationToken.None);
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task RetrieveFromApiWhenOldData()
    {
        //arrange
        _repository
            .FindAsync(Arg.Any<decimal>(), Arg.Any<decimal>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new ForecastModel { UpdatedOn = DateTimeOffset.UtcNow.AddDays(-2) } );
        _clock.UtcNow
            .Returns(DateTimeOffset.UtcNow);
        _apiClient
            .GetForecastAsync(Arg.Any<decimal>(), Arg.Any<decimal>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new ForecastModel());
        
        //act
        var result = await _sut.ProvideForecastAsync(1, 2, CancellationToken.None);
        
        //assert
        await _apiClient.Received().GetForecastAsync(1, 2, CancellationToken.None);
        result.Should().NotBeNull();
    }
}