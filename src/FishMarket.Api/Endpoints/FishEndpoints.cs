using System.Net;
using FishMarket.Api.Domain;
using FishMarket.Api.Dtos;
using FishMarket.Api.Extensions;
using FishMarket.Api.Helpers;
using FishMarket.Api.Infrastructure.Authorization;
using FishMarket.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishMarket.Api.Endpoints;

public static class FishEndpoints
{
    private const long UploadThresholdInBytes = 2 * 1024 * 1024; // 2MB

    public static RouteGroupBuilder MapFishes(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/fishes");

        // Add security requirements, all incoming requests to this API MUST
        // be authenticated with a valid user.
        group.RequireAuthorization(policy => policy.RequireCurrentUser());

        group.MapPost("/", CreateFishAsync);
        group.MapPost("/{id:int}/image", UploadImageAsync).DisableAntiforgery();
        group.MapPut("/{id:int}", UpdateFishAsync);
        group.MapDelete("/{id:int}", DeleteAsync);
        group.MapGet("/", GetAllAsync);
        group.MapGet("/{id:int}", GetAsync);

        group.WithParameterValidation(typeof(FishDto));
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

    private static async Task<Results<Ok, BadRequest<string>, NotFound, ProblemHttpResult>> UploadImageAsync(int id, IFormFile file, [AsParameters] FishService services)
    {
        // This check doesn't catch files that only have a BOM as their content.
        // https://en.wikipedia.org/wiki/Byte_order_mark
        if (file == null || file.Length == 0)
            return TypedResults.BadRequest("No file received from the upload.");

        // There is a maximum size for the files and when the max size is exceeded,
        // the request will fail and the response error code will be RequestEntityTooLarge.
        if (file.Length > UploadThresholdInBytes)
            return TypedResults.Problem(
                detail: $"The file exceeds {(UploadThresholdInBytes / 1048576):N1} MB.",
                statusCode: (int)HttpStatusCode.RequestEntityTooLarge,
                title: "The request is too large to be processed.",
                type: $"https://httpstatuses.com/413");

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        // Check the content length in case the file's only content was a BOM and
        // the content is actually empty after removing the BOM.
        if (stream.Length == 0)
            TypedResults.BadRequest("The file received from the upload is empty.");

        if (!FileHelper.IsValidFileExtension(file.FileName, stream))
            return TypedResults.Problem(
                detail: "You tried to upload a file with an unsupported extension. Please convert it or use another file.",
                statusCode: (int)HttpStatusCode.UnsupportedMediaType,
                title: "Unsupported extension.",
                type: $"https://httpstatuses.com/415");

        stream.Position = 0;

        var fish = await services.Context.Fishes.FindAsync(id);

        if (fish is null || fish.OwnerId != services.CurrentUser.User!.Id)
            return TypedResults.NotFound();

        fish.Image = new() { Data = stream.ToArray() };

        await services.Context.SaveChangesAsync();

        return TypedResults.Ok();
    }
}