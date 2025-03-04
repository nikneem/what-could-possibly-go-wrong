using Microsoft.Extensions.DependencyInjection;
using Votr.Core.Abstractions.Caching;
using Votr.Core.Caching;

namespace Votr.Core.ExtensionMethods;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVotrCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IVotrCacheService, VotrCacheService>();

        return services;
    }
}