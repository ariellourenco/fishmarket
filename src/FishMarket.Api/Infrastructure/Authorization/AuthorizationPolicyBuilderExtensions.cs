using Microsoft.AspNetCore.Authorization;

namespace FishMarket.Api.Infrastructure.Authorization;

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder RequireCurrentUser(this AuthorizationPolicyBuilder builder) =>
        builder.RequireAuthenticatedUser()
            .AddRequirements(new CheckCurrentUserRequirement());
}