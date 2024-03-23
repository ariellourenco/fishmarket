using FishMarket.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FishMarket.Api.Data;

/// <summary>
/// Database abstraction for extend <see cref="IdentityUserContext{TUser, TKey}" /> base class changing
/// the name of the tables created in order to adhere to the database best practice.
/// </summary>
public sealed class FishMarketDbContext(DbContextOptions<FishMarketDbContext> options): IdentityUserContext<AppUser, int>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize the ASP.NET Identity model and override the default table names.
        builder.Entity<AppUser>().ToTable("User");
        builder.Entity<IdentityRole>().ToTable("Role");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaim");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaim");
        builder.Entity<IdentityUserLogin<int>>().ToTable("Login");
        builder.Entity<IdentityUserToken<int>>().ToTable("Token");
    }
}