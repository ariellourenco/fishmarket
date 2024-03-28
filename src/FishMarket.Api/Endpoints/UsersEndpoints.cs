using FishMarket.Api.Dtos;
using FishMarket.Api.Services;
using FishMarket.Api.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FishMarket.Api.Endpoints;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/users");

        group.MapPost("/", CreateUserAsync);
        group.MapPost("/token", GetTokenAsync);
        group.WithParameterValidation(typeof(UserInfo));
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

        // We can safely ignore the result of this operation as it uses a fake Email service just
        // for demonstration sake.
        await services.EmailSender.SendEmailAsync(user.Email.ToLower(),
            "Welcome to FishMarket",
            "You have successfully created an account at FishMarket!");

        return (Results<Ok, ValidationProblem>)TypedResults.Ok();
    }

    private static async Task<Results<BadRequest, Ok<AuthToken>>> GetTokenAsync(UserInfo user, [AsParameters] UserServices services)
    {
        var entity = await services.UserManager.FindByEmailAsync(user.Email);

        return entity is not null || await services.UserManager.CheckPasswordAsync(entity!, user.Password)
            ? TypedResults.Ok(new AuthToken(services.TokenService.GenerateToken(entity!.NormalizedEmail!)))
            : TypedResults.BadRequest();
    }
}

public record AuthToken(string Token);