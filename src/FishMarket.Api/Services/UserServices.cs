using FishMarket.Api.Domain;
using Microsoft.AspNetCore.Identity;

namespace FishMarket.Api.Services;

public class UserServices(ILogger<UserServices> logger, UserManager<AppUser> userManager)
{
    public ILogger<UserServices> Logger { get; } = logger;

    public UserManager<AppUser> UserManager { get; set; } = userManager;
}