namespace FishMarket.Tests;

internal static class DbContextExtensions
{
    /// <summary>
    /// Replaces the configuration for the <see cref="DbContext"/> to use a different configured database
    /// when using <see cref="WebApplicationFactory"/>.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to replace services from.</param>
    /// <param name="configure">A delegate to configure the provided <see cref="DbContextOptionsBuilder"/>.</param>
    /// <returns> The same service collection so that multiple calls can be chained. </returns>
    public static IServiceCollection AddDbContextOptions<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder<TContext>> configure)
        where TContext : DbContext
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TContext>));

        if (descriptor != null)
            services.Remove(descriptor);

        // Add the DbContextOptions<TContext> as a singleton since the IDbContextFactory is a singleton.
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TContext>();

        configure(dbContextOptionsBuilder);
        services.AddSingleton(dbContextOptionsBuilder.Options);
        services.AddSingleton<DbContextOptions>(services => services.GetRequiredService<DbContextOptions<TContext>>());

        return services;
    }
}