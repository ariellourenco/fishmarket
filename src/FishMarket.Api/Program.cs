using FishMarket.Api.Data;
using FishMarket.Api.Domain;
using FishMarket.Api.Endpoints;
using FishMarket.Api.Infrastructure.Authentication;
using FishMarket.Api.Infrastructure.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Configure authentication & authorization
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorizationBuilder().AddCurrentUserHandler();
builder.Services.AddSingleton<ITokenService, TokenService>();

// Configure database
builder.Services.AddSqlite<FishMarketDbContext>(builder.Configuration.GetConnectionString("Default"));

// Configure Identity
builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<FishMarketDbContext>();

// Configure Services
builder.Services.AddCurrentUser();

var app = builder.Build();

// Configure the APIs
app.MapFishes();
app.MapUsers();

app.Run();
