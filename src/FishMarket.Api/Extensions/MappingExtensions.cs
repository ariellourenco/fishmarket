using FishMarket.Api.Domain;
using FishMarket.Api.Dtos;

namespace FishMarket.Api.Extensions;

 public static class MappingExtensions
{
    public static FishDto AsDto(this Fish fish)
    {
        var dto = new FishDto
        {
            Id = fish.Id,
            Name = fish.Name,
            Price = fish.Price
        };

        if (fish.Image is not null)
        {
            var base64 = Convert.ToBase64String(fish.Image.Data);
            var imageDataURL = $"data:image/jpg;base64,{base64}";
            dto.Image = imageDataURL;
        }

        return dto;
    }
}