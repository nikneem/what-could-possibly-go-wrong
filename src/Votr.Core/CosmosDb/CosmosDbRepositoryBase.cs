using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Votr.Core.Configuration;

namespace Votr.Core.CosmosDb;

public class CosmosDbRepositoryBase(CosmosClient cosmos, IOptions<AzureServiceConfiguration> options, ILogger logger)
{

    private Container? _cosmosContainer;

    protected IOptions<AzureServiceConfiguration> Options => options;

    protected Container GetCosmosDbContainer(string containerId)
    {
        if (_cosmosContainer != null && Equals(_cosmosContainer.Id, containerId))
        {
            return _cosmosContainer;
        }
        var optionValues = options.Value;
        try
        {
            logger.LogInformation("Creating CosmosDb container {name}", containerId);
            var database = cosmos.GetDatabase(optionValues.CosmosDbDatabase);
            _cosmosContainer = database.GetContainer(containerId);
            return _cosmosContainer;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create CosmosDb container {name}", containerId);
        }

        throw new InvalidOperationException("Failed to create CosmosDb container");
    }
}