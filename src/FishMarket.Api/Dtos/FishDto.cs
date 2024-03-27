namespace FishMarket.Api.Dtos;

using System.ComponentModel.DataAnnotations;

public class FishDto
{
    public int Id { get; init; }

    [Required]
    public string Name { get; init; } = default!;

    [Required]
    [Range(0.01, 1000)]
    public decimal Price { get; init; }

    public string? Image { get; set; }
}
