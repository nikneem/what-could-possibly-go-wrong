using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Votr.Core.Identity;
using Votr.Votes.Abstractions;
using Votr.Votes.Configuration;

namespace Votr.Votes.Data.TableStorage.ExtensionMethods;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder WithTableStorage(this IHostApplicationBuilder builder)
    {
        var identity = CloudIdentity.GetCloudIdentity();
        var configSection = builder.Configuration.GetRequiredSection(VotesServiceConfiguration.DefaultSectionName);
        var reviewsConfiguration = configSection.Get<VotesServiceConfiguration>();
        if (reviewsConfiguration == null)
        {
            throw new Exception($"Reviews service misconfiguration! Missing configuration section '{VotesServiceConfiguration.DefaultSectionName}'.");
        }
        builder.Services.AddAzureClients(azure =>
        {
            azure.UseCredential(identity);
            azure.AddTableServiceClient(new Uri($"https://{reviewsConfiguration.StorageAccountName}.table.core.windows.net"));
        });


        builder.Services.AddScoped<IVotesRepository, VotesRepository>();

        return builder;
    }
}