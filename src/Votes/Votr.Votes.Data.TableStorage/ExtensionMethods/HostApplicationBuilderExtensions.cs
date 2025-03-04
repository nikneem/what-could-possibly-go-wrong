using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Votr.Votes.Abstractions;

namespace Votr.Votes.Data.TableStorage.ExtensionMethods;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder WithTableStorage(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IVotesRepository, VotesRepository>();
        return builder;
    }
}