using System.Net;
using System.Net.Http;
using System.Text;
using FishMarket.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FishMarket.Tests;

public sealed class FishApiTests
{
    private const string BaseUrl = "/fishes";

    private const string Username = "user@test.com";

    private const string Base64Image = @"/9j/4AAQSkZJRgABAQEAAAAAAAD/2wBDAAoHBwkHBgoJCAkLCwoMDxkQDw4ODx4WFxIZJCAmJSMgIyIoLTkwKCo2KyIjMkQyNjs9QEBAJjBGS0U+Sjk/
    QD3/2wBDAQsLCw8NDx0QEB09KSMpPT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT3/wAARCAAIAAgDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFB
    gcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eX
    qDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgE
    CBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOU
    lZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKAP/2Q==";

    [Fact]
    public async Task GetFishes()
    {
        // Arrange
        var name = "Salmon";
        var price = 12.34M;

        await using var app = new TestServerFactory();
        await using var db = app.CreateDbContext();
        await app.CreateUserAsync(Username);

        var client = app.CreateClient(Username);
        var response = await client.PostAsJsonAsync(BaseUrl, new { Name = name, Price = price });

        // Act
        var fishes = await client.GetFromJsonAsync<List<FishDto>>("/fishes");

        // Assert
        Assert.NotNull(fishes);
        Assert.Single(fishes);
    }

    [Theory]
    [InlineData("GET", "/fishes")]
    [InlineData("GET", "/fishes/1")]
    [InlineData("POST", "/fishes")]
    [InlineData("PUT", "/fishes/1")]
    [InlineData("DELETE", "/fishes/1")]
    public async Task NonAuthenticatedUserReturns(string method, string url)
    {
        // Arrange
        await using var app = new TestServerFactory();
        await using var db = app.CreateDbContext();

        var client = app.CreateClient();
        var request = new HttpRequestMessage(new HttpMethod(method), url);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

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
    public async Task CanUploadImage()
    {
        // Arrange
        var name = "Salmon";
        var price = 12.34M;

        await using var app = new TestServerFactory();
        await using var db = app.CreateDbContext();
        await app.CreateUserAsync(Username);

        var client = app.CreateClient(Username);
        var response = await client.PostAsJsonAsync(BaseUrl, new { Name = name, Price = price });

        var fish = await response.Content.ReadFromJsonAsync<FishDto>();

        Assert.NotNull(fish);

        // Act
        response = await client.PostAsync($"{BaseUrl}/{fish.Id}/image", new MultipartFormDataContent
            {
                { new ByteArrayContent(Convert.FromBase64String(Base64Image)), "file", "test.jpg" }
            });

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task FileGreaterThan2MbReturnsProblemDetails()
    {
        // Arrange
        var name = "Salmon";
        var price = 12.34M;

        await using var app = new TestServerFactory();
        await using var db = app.CreateDbContext();
        await app.CreateUserAsync(Username);

        var client = app.CreateClient(Username);
        var response = await client.PostAsJsonAsync(BaseUrl, new { Name = name, Price = price });

        var fish = await response.Content.ReadFromJsonAsync<FishDto>();

        Assert.NotNull(fish);

        // Act
        response = await client.PostAsync($"{BaseUrl}/{fish.Id}/image", new MultipartFormDataContent
            {
                { new ByteArrayContent(new byte[2 * 1024 * 1024 + 1]), "file", "test.jpg" }
            });

        // Assert
        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("The request is too large to be processed.", problemDetails.Title);
        Assert.Equal("The file exceeds 2.0 MB.", problemDetails.Detail);
    }

    [Fact]
    public async Task FileWithUnsupportedExtensionReturnsProblemDetails()
    {
        // Arrange
        var name = "Salmon";
        var price = 12.34M;

        await using var app = new TestServerFactory();
        await using var db = app.CreateDbContext();
        await app.CreateUserAsync(Username);

        var client = app.CreateClient(Username);
        var response = await client.PostAsJsonAsync(BaseUrl, new { Name = name, Price = price });

        var fish = await response.Content.ReadFromJsonAsync<FishDto>();

        Assert.NotNull(fish);

        // Act
        response = await client.PostAsync($"{BaseUrl}/{fish.Id}/image", new MultipartFormDataContent
            {
                { new ByteArrayContent(new byte[1024]), "file", "test.jpeg" }
            });

        // Assert
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Unsupported extension.", problemDetails.Title);
        Assert.Equal("You tried to upload a file with an unsupported extension. Please convert it or use another file.", problemDetails.Detail);
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