using Demo.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;

namespace Demo.Api.Tests.Component;

public class ComponentTestsWebApplicationFactory: WebApplicationFactory<Program>
{
    private MsSqlContainer? _msSqlContainer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        // Start test container
        _msSqlContainer = new MsSqlBuilder().Build();
        _msSqlContainer.StartAsync().Wait();

        // Replace sql server with test container
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<WeatherForecastDbContext>));

            // Add DB context pointing to test container
            services.AddDbContext<WeatherForecastDbContext>(dbContextOptionsBuilder =>
            {
                var connectionStringBuilder = new SqlConnectionStringBuilder(_msSqlContainer.GetConnectionString())
                {
                    InitialCatalog = "demo-api-component-tests-db"
                };

                dbContextOptionsBuilder.UseSqlServer(connectionStringBuilder.ConnectionString);
            });
        });
    }

    public override async ValueTask DisposeAsync()
    {
        if (_msSqlContainer != null)
        {
            await _msSqlContainer.DisposeAsync();
        }

        await base.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
