using FishMarket.Api.Data;
using FishMarket.Api.Infrastructure.Authorization;

namespace FishMarket.Api.Services;

public class FishService(CurrentUser currentUser, FishMarketDbContext context, ILogger<FishService> logger)
{
    public CurrentUser CurrentUser { get; } = currentUser;

    public FishMarketDbContext Context { get; } = context;

    public ILogger<FishService> Logger { get; } = logger;
}
