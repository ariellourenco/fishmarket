using FishMarket.Api.Data;
using FishMarket.Api.Domain;
using FishMarket.Api.Endpoints;
using FishMarket.Api.Extensions;
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

// Configure Open API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.InferSecuritySchemes());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();

// Configure the APIs
app.MapFishes();
app.MapStock();
app.MapUsers();

app.Run();
