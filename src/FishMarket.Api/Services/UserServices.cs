using FishMarket.Api.Domain;
using FishMarket.Api.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FishMarket.Api.Services;

public class UserServices(ILogger<UserServices> logger, ITokenService tokenService, UserManager<AppUser> userManager)
{
    public ILogger<UserServices> Logger { get; } = logger;

    public ITokenService TokenService { get; } = tokenService;

    public UserManager<AppUser> UserManager { get; set; } = userManager;
}