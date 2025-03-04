using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Votr.Votes.Abstractions;
using Votr.Votes.Services;

namespace Votr.Votes.ExtensionMethods;

public static class AppHostBuilderExtensions
{


    public static IHostApplicationBuilder AddVotesApi(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IVotesService, VotesService>();
        builder.Services.AddSignalR().AddNamedAzureSignalR("signalr");
        return builder;
    }

}