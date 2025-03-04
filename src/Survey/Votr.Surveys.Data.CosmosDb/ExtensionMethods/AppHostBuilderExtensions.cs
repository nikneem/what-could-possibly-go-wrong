using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Votr.Surveys.Abstractions;

namespace Votr.Surveys.Data.CosmosDb.ExtensionMethods;

public static class AppHostBuilderExtensions
{


    public static IHostApplicationBuilder WithCosmosDbRepository(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISurveysRepository, SurveysRepository>();
        builder.Services.AddSignalR().AddNamedAzureSignalR("sr");
        return builder;
    }

}