using FishMarket.Api.Domain;
using FishMarket.Api.Dtos;

namespace FishMarket.Api.Extensions;

 public static class MappingExtensions
{
    public static FishDto AsDto(this Fish fish) => new()
    {
        Id = fish.Id,
        Name = fish.Name,
        Price = fish.Price
    };
}