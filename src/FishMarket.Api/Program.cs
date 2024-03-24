using FishMarket.Api.Data;
using FishMarket.Api.Domain;
using FishMarket.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Configure database
builder.Services.AddSqlite<FishMarketDbContext>(builder.Configuration.GetConnectionString("Default"));

// Configure Identity
builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<FishMarketDbContext>();

var app = builder.Build();

// Configure the APIs
app.MapUsers();

app.Run();
