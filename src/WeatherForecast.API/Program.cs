using System.Reflection;
using Microsoft.OpenApi.Models;
using WeatherForecast.API.Policies;
using WeatherForecast.Core.Interfaces;
using WeatherForecast.Infra;
using WeatherForecast.Infra.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient<IWeatherApi, WeatherApiClient>
        (client => 
            client.BaseAddress = new Uri("https://api.open-meteo.com", UriKind.Absolute))
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler(Pollycies.ExponentialRetry());

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Weather Forecast API",
        Description = "An ASP.NET Core Web API for weather forecasting",
        TermsOfService = new Uri("https://example.com/terms")
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//to make it discoverable for api tests
public partial class Program { }