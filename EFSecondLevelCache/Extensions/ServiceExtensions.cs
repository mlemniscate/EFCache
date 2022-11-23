using EFCoreSecondLevelCacheInterceptor;
using EFSecondLevelCache.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EFSecondLevelCache.Extensions;

public static class ServiceExtensions
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
                            .MigrationsAssembly(typeof(Program).Assembly.FullName);
                    })
                .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
        return services;
    }

    public static IServiceCollection AddMyCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", options =>
                options.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin());
        });
        return services;
    }
}