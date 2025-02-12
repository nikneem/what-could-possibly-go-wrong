using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Votr.Core.Configuration;
using Votr.Core.CosmosDb;
using Votr.Core.DDD.Enums;
using Votr.Surveys.Abstractions;
using Votr.Surveys.Data.CosmosDb.Entities;
using Votr.Surveys.Data.CosmosDb.Mappings;
using Votr.Surveys.DataTransferObjects.Details;
using Votr.Surveys.DomainModels;

namespace Votr.Surveys.Data.CosmosDb;

public class SurveysRepository (CosmosClient cosmos,
    IOptions<AzureServiceConfiguration> options,
    ILogger<SurveysRepository> logger
    ) : CosmosDbRepositoryBase(cosmos, options, logger), ISurveysRepository
{

    private const string SurveysPartitionId = "surveys";

    public async Task<List<SurveyDetailsResponse>> List(CancellationToken cancellationToken)
    {
        var container  = GetContainer();
        var queryable = container.GetItemLinqQueryable<SurveyEntity>()
            .Where(ent => ent.EntityType == nameof(SurveyEntity))
            .OrderByDescending(ent => ent.ExpiresOn)
            .ToFeedIterator();

        var list = new List<SurveyDetailsResponse>();
        while (queryable.HasMoreResults)
        {
            var batch = await queryable.ReadNextAsync(cancellationToken);
            list.AddRange(batch.ToDetailsResponse());
        }

        return list;
    }

    public async Task<Survey> Get(Guid id, CancellationToken cancellationToken)
    {
        var container = GetContainer();
        var response = await container.ReadItemAsync<SurveyEntity>(id.ToString(), new PartitionKey(SurveysPartitionId), cancellationToken: cancellationToken);
        return response.Resource.ToDomainModel();
    }

    public async Task<Survey> Get(string code, CancellationToken cancellationToken)
    {
        var container = GetContainer();
        var queryable = container.GetItemLinqQueryable<SurveyEntity>()
            .Where(ent => ent.EntityType == nameof(SurveyEntity))
            .Where(ent => ent.Code == code)
            .ToFeedIterator();

        var entity = await queryable.ReadNextAsync(cancellationToken);
            return entity.First().ToDomainModel();
    }


    public async Task<bool> Save(Survey domainModel, CancellationToken cancellationToken)
    {
        if (!domainModel.IsValid)
        {
            throw new InvalidOperationException("Survey is not in a valid state");
        }

        if (domainModel.TrackingState == TrackingState.Modified || 
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

    public async Task<bool> Cleanup(CancellationToken cancellationToken)
    {
        // Cleanup all surveys older than two months
        var container = GetContainer();

        var expiryDate = DateTimeOffset.Now.AddMonths(-2);

        var oldSurveysQuery = container.GetItemLinqQueryable<SurveyEntity>()
            .Where(ent => ent.EntityType == nameof(SurveyEntity))
            .Where(ent => ent.ExpiresOn < expiryDate)
            .ToFeedIterator();

        var batch = container.CreateTransactionalBatch(new PartitionKey(SurveysPartitionId));
        while (oldSurveysQuery.HasMoreResults)
        {
            var entityBatch = await oldSurveysQuery.ReadNextAsync(cancellationToken);
            foreach (var entity in entityBatch)
            {
                batch.DeleteItem(entity.Id.ToString());
            }
        }

        var outcome = await batch.ExecuteAsync(cancellationToken);
        return outcome.IsSuccessStatusCode;
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