using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FishMarket.Api.Infrastructure.Authentication;

/// <summary>
/// Provides methods for creating and managing JSON Web Token (JWT) authentication.
/// </summary>
public sealed class TokenService : ITokenService
{
    private readonly string _issuer;
    private readonly SigningCredentials _credentials;
    private readonly Claim[] _audiences;


    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class.
    /// </summary>
    /// <param name="provider">Provides access to authentication-related configuration.</param>
    public TokenService(IAuthenticationConfigurationProvider provider)
    {
        var bearerSection = provider.GetSchemeConfiguration(JwtBearerDefaults.AuthenticationScheme);
        var signingSection = bearerSection.GetSection("SigningKeys:0");
        var signingKeyBase64 = signingSection["Value"] ?? throw new InvalidOperationException("Signing key is not specified.");
        var signingKey = Convert.FromBase64String(signingKeyBase64);

        _issuer = bearerSection["ValidIssuer"] ?? throw new InvalidOperationException("Issuer is not specified.");
        _credentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature);
        _audiences = bearerSection.GetSection("ValidAudiences").GetChildren()
            .Where(audience => !string.IsNullOrEmpty(audience.Value))
            .Select(audience => new Claim(JwtRegisteredClaimNames.Aud, audience.Value!))
            .ToArray();
    }

    /// <summary>
    /// Generates a JWT token for the given username.
    /// </summary>
    /// <param name="username">The user name of the current user.</param>
    /// <param name="isAdmin"><see langword="true"/> if the current user is an administrator; otherwise <see langword="false"/>.</param>
    /// <returns>A JSON Web Token (JWT) string.</returns>
    public string GenerateToken(string username, bool isAdmin = false)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User"),
            new Claim(JwtRegisteredClaimNames.Iss, _issuer),
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString().GetHashCode().ToString("x", CultureInfo.InvariantCulture))
        ], JwtBearerDefaults.AuthenticationScheme);

        identity.AddClaims(_audiences);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(
            _issuer,
            audience: null,
            identity,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(30),
            issuedAt: DateTime.UtcNow,
            _credentials);

        return handler.WriteToken(token);
    }
}