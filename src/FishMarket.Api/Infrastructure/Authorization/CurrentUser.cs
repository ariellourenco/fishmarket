using System.Security.Claims;
using FishMarket.Api.Domain;

namespace FishMarket.Api.Infrastructure.Authorization;

public sealed class CurrentUser
{
    public AppUser? User { get; set; }
    public ClaimsPrincipal Principal { get; set; } = default!;
    public string Id => Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
    public bool IsAdmin => Principal.IsInRole("admin");
}