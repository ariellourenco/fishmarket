using System.Net;
using FishMarket.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FishMarket.Tests;

public sealed class UserApiTests
{
    private const string BaseUrl = "/users";

    [Fact]
    public async Task CanCreateUser()
    {
        // Arrange
        await using var app = new TestServerFactory();
        await using var db = app.CreateDbContext();

        var client = app.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(BaseUrl, new UserInfo { Email = "todouser@test.com", Password = "@pwd" });

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        var user = db.Users.Single();

        Assert.NotNull(user);
        Assert.Equal("todouser@test.com", user.Email);
        Assert.Equal("todouser@test.com", user.UserName);
    }

    [Fact]
    public async Task MissingEmailReturnsBadRequest()
    {
        // Arrange
        await using var app = new TestServerFactory();
        await using var db = app.CreateDbContext();

        var client = app.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(BaseUrl, new UserInfo { Email = "", Password = "@pwd" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("One or more validation errors occurred.", problemDetails.Title);
        Assert.NotEmpty(problemDetails.Errors);
        Assert.Equal(new [] {"The Email field is required."}, problemDetails.Errors["Email"]);
    }

    [Fact]
    public async Task MissingPasswordReturnsBadRequest()
    {
        // Arrange
        await using var app = new TestServerFactory();
        await using var db = app.CreateDbContext();

        var client = app.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(BaseUrl, new UserInfo { Email = "todouser@test.com", Password = "" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("One or more validation errors occurred.", problemDetails.Title);
        Assert.NotEmpty(problemDetails.Errors);
        Assert.Equal(new[] {"The Password field is required."}, problemDetails.Errors["Password"]);
    }
}