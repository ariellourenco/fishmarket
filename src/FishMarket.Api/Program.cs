using FishMarket.Api.Data;
using FishMarket.Api.Domain;

var builder = WebApplication.CreateBuilder(args);

// Configure database
builder.Services.AddSqlite<FishMarketDbContext>("Data Source=.db/Todos.db");

// Configure Identity
builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<FishMarketDbContext>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
