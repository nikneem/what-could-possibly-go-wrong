using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Votr.Core.Configuration;
using Votr.Core.CosmosDb.Resolvers;
using Votr.Core.Identity;

namespace Votr.Core.CosmosDb.ExtensionMethods;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddCosmosDb(this IServiceCollection services)
    {

        services.AddSingleton<CosmosClient>(sp =>
        {

            var options = sp.GetRequiredService<IOptions<AzureServiceConfiguration>>();
            var optionValues = options.Value;

            // Validate configuration
            if (string.IsNullOrEmpty(optionValues.CosmosDbEndpoint))
            {
                throw new InvalidOperationException($"Setting {AzureServiceConfiguration.DefaultSectionName}.{nameof(AzureServiceConfiguration.CosmosDbEndpoint)} is empty or missing.");
            }
            if (string.IsNullOrEmpty(optionValues.CosmosDbDatabase))
            {
                throw new InvalidOperationException($"Setting {AzureServiceConfiguration.DefaultSectionName}.{nameof(AzureServiceConfiguration.CosmosDbDatabase)} is empty or missing.");
            }
            var identity = CloudIdentity.GetCloudIdentity();

            if (!string.IsNullOrWhiteSpace(optionValues.CosmosDbKey))
            {
                Console.WriteLine("WARNING - Connecting to CosmosDb with a Key, remove the key and use managed identities. Use the Key for debugging scenarios ONLY");
            }

            var cosmosClientBuilder = string.IsNullOrWhiteSpace(optionValues.CosmosDbKey)
                ? new CosmosClientBuilder(optionValues.CosmosDbEndpoint, identity)
                : new CosmosClientBuilder(optionValues.CosmosDbEndpoint, optionValues.CosmosDbKey);

            var cosmosClient = cosmosClientBuilder.WithApplicationSerializer()
                .Build();

            return cosmosClient;
        });

        return services;
    }

    public static CosmosClientBuilder WithApplicationSerializer(this CosmosClientBuilder builder)
    {
        // Register custom converters for polymorphic (sub)entities
        var contractResolver = new PluggableContractResolver();

        builder.WithCustomSerializer(new CustomCosmosSerializer(
            new JsonSerializerSettings { ContractResolver = contractResolver }
        ));

        return builder;
    }

}