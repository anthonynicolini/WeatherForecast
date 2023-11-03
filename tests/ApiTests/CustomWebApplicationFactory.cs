using LiteDB;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiTests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //test services go here
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ILiteDatabase>();
            db.DropCollection("ForecastModel");
        });

        builder.UseEnvironment("Test");
    }
}