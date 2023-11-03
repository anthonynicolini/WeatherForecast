using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Core.Interfaces;
using WeatherForecast.Core.Models;

namespace WeatherForecast.API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class ForecastsController : ControllerBase
{
    private readonly IForecastService _forecastService;

    public ForecastsController(IForecastService forecastService)
    {
        _forecastService = forecastService;
    }

    /// <summary>
    /// Returns all weather forecasts saved in the database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _forecastService.GetAllForecastsAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns weather forecast for given coordinates. If no recent (less than 24h) result is found in the database, retrieves a new one from weather API
    /// </summary>
    /// <param name="latitude">Latitude, will be rounded to 2 decimal points precision</param>
    /// <param name="longitude">Longitude, will be rounded to 2 decimal points precision</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{latitude:double}&{longitude:double}")]
    [ProducesResponseType(typeof(ForecastModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindForecast(
        [Range(-90, 90)] decimal latitude,
        [Range(-180, 180)] decimal longitude,
        CancellationToken cancellationToken)
    {
        var result = await _forecastService.ProvideForecastAsync(latitude, longitude, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Adds a new weather forecast
    /// </summary>
    /// <param name="create"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateForecast(ForecastModel create, CancellationToken cancellationToken)
    {
        var result = await _forecastService.CreateForecastAsync(create, cancellationToken);
        var uri = new Uri($"Forecasts/{result.Latitude}&{result.Longitude}", UriKind.Relative);
        return Created(uri, result);
    }

    /// <summary>
    /// Update an existing weather forecast, or adds a new one 
    /// </summary>
    /// <param name="update"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateForecast(ForecastModel update, CancellationToken cancellationToken)
    {
        var existing =
            await _forecastService.GetExistingForecastAsync(update.Latitude, update.Longitude, cancellationToken);
        if (existing is null)
        {
            var created = await _forecastService.CreateForecastAsync(update, cancellationToken);
            var uri = new Uri($"Forecasts/{created.Latitude}&{created.Longitude}", UriKind.Relative);
            return Created(uri, created);
        }

        await _forecastService.UpdateForecastAsync(update, cancellationToken);
        return Ok(update);
    }

    /// <summary>
    /// Deletes existing weather forecast
    /// </summary>
    /// <param name="latitude">Latitude, will be rounded to 2 decimal points precision</param>
    /// <param name="longitude">Longitude, will be rounded to 2 decimal points precision</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{latitude:double}&{longitude:double}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteForecast(
        [Range(-90, 90)] decimal latitude,
        [Range(-180, 180)] decimal longitude,
        CancellationToken cancellationToken)
    {
        var existing =
            await _forecastService.GetExistingForecastAsync(latitude, longitude, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        await _forecastService.DeleteForecastAsync(latitude, longitude, cancellationToken);
        return NoContent();
    }
}