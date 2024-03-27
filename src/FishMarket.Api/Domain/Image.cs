namespace FishMarket.Api.Domain;

public class Image
{
    public int Id { get; set; }

    public byte[] Data { get; set; } = default!;

    public int FishId { get; set; }
}