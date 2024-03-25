using Microsoft.AspNetCore.Authorization;

namespace FishMarket.Api.Infrastructure.Authorization;

/// <summary>
/// Represents the requirement for determining whether <see cref="CheckCurrentUserRequirement"/> in authorization
/// policies are satisfied or not.
/// </summary>
internal sealed class AuthenticatedUserAuthorizationHandler(CurrentUser user) : AuthorizationHandler<CheckCurrentUserRequirement>
{
    private readonly CurrentUser _currentUser = user;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckCurrentUserRequirement requirement)
    {
        if (_currentUser.User is not null)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

internal class CheckCurrentUserRequirement : IAuthorizationRequirement { }