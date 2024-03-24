using FishMarket.Api.Dtos;
using FishMarket.Api.Filters;
using FishMarket.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FishMarket.Api.Endpoints;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/users");

        group.MapPost("/", CreateUserAsync);
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

        return (Results<Ok, ValidationProblem>)TypedResults.Ok();
    }
}