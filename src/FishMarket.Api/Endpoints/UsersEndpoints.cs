namespace FishMarket.Api.Endpoints;

using FishMarket.Api.Domain;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/users");

        group.MapPost("/", CreateUserAsync);
        group.WithTags("Users");

        return group;
    }

    public static async Task<Results<Ok, ValidationProblem>> CreateUserAsync(UserInfo user, [AsParameters] UserServices services)
    {
        var result = await services.UserManager.CreateAsync(new()
        {
            Email = user.Email,
            UserName = user.Email.ToLowerInvariant()
        }, user.Password);

        if (!result.Succeeded)
        {
            return (Results<Ok, ValidationProblem>)TypedResults.ValidationProblem(result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));
        }

        services.Logger.LogInformation("Successfully created '{email}' user", user.Email);

        return (Results<Ok, ValidationProblem>)TypedResults.Ok();
    }
}

public class UserInfo
{
    [Required]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}

public class UserServices(ILogger<UserServices> logger, UserManager<AppUser> userManager)
{
    public ILogger<UserServices> Logger { get; } = logger;

    public UserManager<AppUser> UserManager { get; set; } = userManager;
}