namespace FishMarket.Api.Endpoints;

using FishMarket.Api.Dtos;
using FishMarket.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public static class StockEndpoints
{
    public static RouteGroupBuilder MapStock(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/stock");

        group.MapGet("/{name:minlength(3)}", GetFishesByName);
        group.WithTags("Stock");

        return group;
    }

   public static async Task<Ok<PaginatedItems<object>>> GetFishesByName(string name,
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] FishService services)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        var totalItems = await services.Context.Fishes
            .Where(c => c.Name.StartsWith(name))
            .LongCountAsync();

        var itemsOnPage = await services.Context.Fishes
            .Where(c => c.Name.StartsWith(name))
            .Select(item => new { item.Price })
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(new PaginatedItems<object>(pageIndex, pageSize, totalItems, itemsOnPage));
    }
}

public record PaginationRequest(int PageSize = 10, int PageIndex = 0);