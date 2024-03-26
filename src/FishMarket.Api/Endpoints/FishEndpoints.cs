using FishMarket.Api.Domain;
using FishMarket.Api.Dtos;
using FishMarket.Api.Extensions;
using FishMarket.Api.Infrastructure.Authorization;
using FishMarket.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FishMarket.Api.Endpoints;

public static class FishEndpoints
{
    public static RouteGroupBuilder MapFishes(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/fishes");

        // Add security requirements, all incoming requests to this API MUST
        // be authenticated with a valid user.
        group.RequireAuthorization(policy => policy.RequireCurrentUser());
        group.MapPost("/", CreateFishAsync);
        group.MapPut("/{id:int}", UpdateFishAsync);
        group.MapDelete("/{id:int}", DeleteAsync);
        group.MapGet("/", GetAllAsync);
        group.MapGet("/{id:int}", GetAsync);
        group.WithTags("Fishes");

        return group;
    }

    private static async Task<Created<FishDto>> CreateFishAsync(FishDto fish, [AsParameters] FishService services)
    {
        var newFish = new Fish
        {
            Name = fish.Name,
            Price = fish.Price,
            OwnerId = services.CurrentUser.User!.Id
        };

        services.Context.Fishes.Add(newFish);
        await services.Context.SaveChangesAsync();

        services.Logger.LogInformation("Fish {FishName} created by {UserName}", newFish.Name, services.CurrentUser.User!.UserName);

        return TypedResults.Created($"/fishes/{newFish.Id}", newFish.AsDto());
    }

    private static async Task<Results<NoContent, NotFound>> DeleteAsync(int id, [AsParameters] FishService services)
    {
        var rowsAffected = await services.Context.Fishes
            .Where(fish => fish.Id == id && fish.OwnerId == services.CurrentUser.User!.Id)
            .ExecuteDeleteAsync();

        if (rowsAffected == 0)
            return TypedResults.NotFound();

        services.Logger.LogInformation("Fish {FishId} deleted by {UserName}", id, services.CurrentUser.User!.UserName);

        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok, BadRequest<string>, NotFound>> UpdateFishAsync(int id, FishDto fish, [AsParameters] FishService services)
    {
        if (id != fish.Id)
        {
            return TypedResults.BadRequest("The fish id in the URL does not match the fish id in the body.");
        }

        var rowsAffected = await services.Context.Fishes
            .Where(entity => entity.Id == id && entity.OwnerId == services.CurrentUser.User!.Id)
            .ExecuteUpdateAsync(updates => updates.SetProperty(entity => entity.Price, fish.Price));

        if (rowsAffected == 0)
            return TypedResults.NotFound();

        services.Logger.LogInformation("Fish {FishId} updated by {UserName}", id, services.CurrentUser.User!.UserName);

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok<FishDto>, NotFound>> GetAsync(int id, [AsParameters] FishService services) =>
        await services.Context.Fishes.FindAsync(id) switch
        {
            Fish fish when fish.OwnerId == services.CurrentUser.User!.Id => TypedResults.Ok(fish.AsDto()),
            _ => TypedResults.NotFound()
        };

    private static async Task<Ok<List<FishDto>>> GetAllAsync([AsParameters] FishService services)
    {
        var fishes = await services.Context.Fishes.Where(fish => fish.OwnerId == services.CurrentUser.User!.Id)
            .Select(fish => fish.AsDto())
            .AsNoTracking()
            .ToListAsync();

        return TypedResults.Ok(fishes);
    }
}