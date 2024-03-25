using Microsoft.AspNetCore.Authorization;

namespace FishMarket.Api.Infrastructure.Authorization;

public static class AuthorizationBuilderExtensions
{
    public static AuthorizationBuilder AddCurrentUserHandler(this AuthorizationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationHandler, AuthenticatedUserAuthorizationHandler>();
        return builder;
    }
}
