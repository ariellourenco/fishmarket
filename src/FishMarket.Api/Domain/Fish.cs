using System.ComponentModel.DataAnnotations;

namespace FishMarket.Api.Domain;

public sealed class Fish
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }

    public Image? Image { get; set; }

    public int OwnerId { get; set; }
}