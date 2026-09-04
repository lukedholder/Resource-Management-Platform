using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ResourcePlatform.Infrastructure;
using Testcontainers.MsSql;

namespace ResourcePlatform.IntegrationTests;


/// <summary>
/// Boots the real application in memory against a real SQL Server in a throwaway
/// container. No mocks: the same middleware, the same EF model, the same SQL.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // local image
    private readonly MsSqlContainer _sql = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();

    private string _connectionString = string.Empty;

    public async Task InitializeAsync()
    {
        await _sql.StartAsync();

        await using (var master = new SqlConnection(_sql.GetConnectionString()))
        {
            await master.OpenAsync();
            await using var cmd = master.CreateCommand();
            cmd.CommandText = "CREATE DATABASE ResourcePlatformTests";
            await cmd.ExecuteNonQueryAsync();
        }

        _connectionString = new SqlConnectionStringBuilder(_sql.GetConnectionString())
        {
            InitialCatalog = "ResourcePlatformTests"
        }.ConnectionString;

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Developent");
        builder.UseSetting("ConnectionStrings:Default", _connectionString);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _sql.DisposeAsync();
        await base.DisposeAsync();
    }
}