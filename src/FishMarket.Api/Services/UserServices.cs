using FishMarket.Api.Domain;
using FishMarket.Api.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FishMarket.Api.Services;

public class UserServices(ILogger<UserServices> logger, IEmailSender emailSender, ITokenService tokenService, UserManager<AppUser> userManager)
{
    public ILogger<UserServices> Logger { get; } = logger;

    public IEmailSender EmailSender { get; } = emailSender;

    public ITokenService TokenService { get; } = tokenService;

    public UserManager<AppUser> UserManager { get; set; } = userManager;
}