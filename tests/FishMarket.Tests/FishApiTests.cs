using System.Net;
using FishMarket.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FishMarket.Tests;

public sealed class FishApiTests
{
    private const string BaseUrl = "/fishes";
    private const string Username = "user@test.com";

    [Fact]
    public async Task CanCreateFish()
    {
        // Arrange
        var name = "Salmon";
        var price = 12.34M;

        await using var app = new TestServerFactory();
        await using var db = app.CreateDbContext();
        await app.CreateUserAsync(Username);

        var client = app.CreateClient(Username);

        // Act
        var response = await client.PostAsJsonAsync(BaseUrl, new { Name = name, Price = price });

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var fishes = await client.GetFromJsonAsync<List<FishDto>>(BaseUrl);

        Assert.NotNull(fishes);

        var fish = Assert.Single(fishes);
        Assert.Equal(name, fish.Name);
        Assert.Equal(price, fish.Price);
    }

    [Fact]
    public async Task FishWithoutNameReturnsProblemDetails()
    {
        // Arrange
        var price = 12.34M;

        await using var app = new TestServerFactory();
        await using var db = app.CreateDbContext();
        await app.CreateUserAsync(Username);

        var client = app.CreateClient(Username);

        // Act
        var response = await client.PostAsJsonAsync(BaseUrl, new { Name = string.Empty, Price = price });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.NotEmpty(problemDetails.Errors);
        Assert.Equal("One or more validation errors occurred.", problemDetails.Title);
        Assert.Equal(new[] { "The Name field is required." }, problemDetails.Errors["Name"]);
    }

    [Fact]
    public async Task CanDelete()
    {
        // Arrange
        var name = "Salmon";
        var price = 12.34M;

        await using var app = new TestServerFactory();
        await using var db = app.CreateDbContext();
        await app.CreateUserAsync(Username);

        var client = app.CreateClient(Username);
        var result = await client.PostAsJsonAsync(BaseUrl, new { Name = name, Price = price });
        var fish = await result.Content.ReadFromJsonAsync<FishDto>();

        Assert.NotNull(fish);

        // Act
        var response = await client.DeleteAsync($"{BaseUrl}/{fish.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}