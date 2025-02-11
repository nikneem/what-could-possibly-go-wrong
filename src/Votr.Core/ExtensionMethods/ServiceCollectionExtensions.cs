using Microsoft.Extensions.DependencyInjection;

namespace Votr.Core.ExtensionMethods;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVotrCoreServices(this IServiceCollection services)
    {
        services.AddDaprClient();

        return services;
    }
}