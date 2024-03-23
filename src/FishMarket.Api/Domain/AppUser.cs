using Microsoft.AspNetCore.Identity;

namespace FishMarket.Api.Domain;

/// <summary>
/// Extends the default implementation of <see cref="Microsoft.AspNetCore.Identity.IdentityUser{TKey}" />
/// adding profile data for application user.
/// </summary>
public sealed class AppUser : IdentityUser<int>
{
}