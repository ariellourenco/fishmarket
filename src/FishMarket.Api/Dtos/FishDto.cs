namespace FishMarket.Api.Dtos;

public class FishDto
{
    public int Id { get; init; }

    public string Name { get; init; } = default!;

    public decimal Price { get; init; }
}
