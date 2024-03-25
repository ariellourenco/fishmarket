namespace FishMarket.Api.Infrastructure.Authentication;

/// <summary>
/// Defines properties and methods for create and manage JSON Web Token (JWT) authentication.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the given username.
    /// </summary>
    /// <param name="username">The user name of the current user.</param>
    /// <param name="isAdmin"><see langword="true"/> if the current user is an administrator; otherwise <see langword="false"/>.</param>
    /// <returns>A JSON Web Token (JWT) string.</returns>
    string GenerateToken(string username, bool isAdmin = false);
}