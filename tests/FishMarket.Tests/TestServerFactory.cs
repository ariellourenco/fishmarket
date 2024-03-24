using FishMarket.Api.Data;

namespace FishMarket.Tests;

/// <summary>
/// Factory for bootstrapping an application in memory for functional end to end tests.
/// See: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
/// </summary>
internal sealed class TestServerFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Externally manage the lifetime of in-memory SQLite connections, thus, when the class is disposed the
    /// connection are closed and the database is destroyed.
    /// </summary>
    private readonly SqliteConnection _connection = new($"DataSource=:memory:");

    public FishMarketDbContext CreateDbContext()
    {
        var db = Services.GetRequiredService<IDbContextFactory<FishMarketDbContext>>().CreateDbContext();
        db.Database.EnsureCreated();
        return db;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            services.AddDbContextFactory<FishMarketDbContext>();
            services.AddDbContextOptions<FishMarketDbContext>(options => options.UseSqlite(_connection));

            // Lower the requirements for the tests
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredLength = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            });
        });

        return base.CreateHost(builder);
    }

    protected override void Dispose(bool disposing)
    {
        _connection.Close();
        _connection.Dispose();

        base.Dispose(disposing);
    }
}