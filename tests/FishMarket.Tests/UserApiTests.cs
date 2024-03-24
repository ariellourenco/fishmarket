using FishMarket.Api.Endpoints;

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
}