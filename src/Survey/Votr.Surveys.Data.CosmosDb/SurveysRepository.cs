using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Votr.Core.Configuration;
using Votr.Core.CosmosDb;
using Votr.Core.DDD.Enums;
using Votr.Surveys.Abstractions;
using Votr.Surveys.Data.CosmosDb.Mappings;
using Votr.Surveys.DomainModels;

namespace Votr.Surveys.Data.CosmosDb;

public class SurveysRepository (  CosmosClient cosmos,
    IOptions<AzureServiceConfiguration> options,
ILogger<SurveysRepository> logger
    ) : CosmosDbRepositoryBase(cosmos, options, logger), ISurveysRepository
{
    public Task<List<Survey>> List(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Survey> Get(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Save(Survey domainModel, CancellationToken cancellationToken)
    {
        if (!domainModel.IsValid)
        {
            throw new InvalidOperationException("Survey is not in a valid state");
        }

        if (domainModel.TrackingState == TrackingState.Modified && 
            domainModel.TrackingState == TrackingState.New)
        {
            // Something happened to the domain model. Delete is not implemented
            // so is not one of the states we are expecting
            var container = GetContainer();
            var entity = domainModel.ToEntity();
            if (domainModel.TrackingState == TrackingState.New)
            {
                var itemResponse = await container.CreateItemAsync(entity, cancellationToken: cancellationToken);
                return itemResponse.StatusCode == HttpStatusCode.Created || itemResponse.StatusCode == HttpStatusCode.OK;
            }

            if (domainModel.TrackingState == TrackingState.Modified)
            {
                var itemResponse = await container.UpsertItemAsync(entity, new PartitionKey(entity.Id.ToString()), cancellationToken: cancellationToken);
                return itemResponse.StatusCode == HttpStatusCode.OK;
            }
            return false;
        }
        return true;
    }

    private Container GetContainer()
    {
        return CosmosDbContainer(ContainerName());
    }
    private string ContainerName()
    {
        var optionsValue = Options.Value;
        return optionsValue.SurveysContainer;
    }

}