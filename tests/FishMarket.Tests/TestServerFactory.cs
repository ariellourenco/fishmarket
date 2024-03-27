using System.Net.Http.Headers;
using System.Security.Cryptography;
using FishMarket.Api.Data;
using FishMarket.Api.Domain;
using FishMarket.Api.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

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

    public async Task CreateUserAsync(string email, string? password = null)
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var newUser = new AppUser { Email = email, UserName = email};
        var result = await userManager.CreateAsync(newUser, password ?? Guid.NewGuid().ToString());

        Assert.True(result.Succeeded);
    }

    public HttpClient CreateClient(string email) =>
        CreateDefaultClient(new TestAuthenticationHandler(request =>
        {
            var service = Services.GetRequiredService<ITokenService>();
            var token = service.GenerateToken(email);

            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
        }));

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

            // Configure the signing key for CI scenarios
            var key = new byte[32];
            RandomNumberGenerator.Fill(key);

            builder.ConfigureAppConfiguration(configuration =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:Schemes:Bearer:SigningKeys:0:Issuer"] = "dotnet-user-jwts",
                    ["Authentication:Schemes:Bearer:SigningKeys:0:Value"] = Convert.ToBase64String(key)
                }));
        });

        return base.CreateHost(builder);
    }

    protected override void Dispose(bool disposing)
    {
        _connection.Close();
        _connection.Dispose();

        base.Dispose(disposing);
    }

    private sealed class TestAuthenticationHandler(Action<HttpRequestMessage> onRequest) : DelegatingHandler
    {
        private readonly Action<HttpRequestMessage> _onRequest = onRequest;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _onRequest(request);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
