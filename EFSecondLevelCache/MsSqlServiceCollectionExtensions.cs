using EFCoreSecondLevelCacheInterceptor;
using EFSecondLevelCache.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EFSecondLevelCache;

public static class MsSqlServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguredMsSqlDbContext(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContextPool<AppDbContext>((serviceProvider, optionsBuilder) =>
            optionsBuilder
                .UseSqlServer(
                    connectionString,
                    sqlServerOptionsBuilder =>
                    {
                        sqlServerOptionsBuilder
                            .CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds)
                            .EnableRetryOnFailure()
                            .MigrationsAssembly(typeof(MsSqlServiceCollectionExtensions).Assembly.FullName);
                    })
                .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
        return services;
    }
}