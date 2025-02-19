using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Votr.Votes.Abstractions;
using Votr.Votes.Configuration;
using Votr.Votes.Data.TableStorage.Mappings;
using Votr.Votes.DomainModels;

namespace Votr.Votes.Data.TableStorage;

public class VotesRepository(TableServiceClient tableServiceClient, IOptions<VotesServiceConfiguration> options) : IVotesRepository
{
    public async Task<bool> Save(Vote vote, CancellationToken cancellationToken)
    {

        var votesTableName = options.Value.VotesTable;
        var tableClient = tableServiceClient.GetTableClient(votesTableName);
        var entity = vote.ToEntity();
        var dirtyReviewsTable = await tableClient.AddEntityAsync(entity, cancellationToken);
        return !dirtyReviewsTable.IsError;
    }


    //public async Task<bool> PersistDirtyReview(Vote vote, CancellationToken cancellationToken)
    //{
    //    var tableClient = tableServiceClient.GetTableClient(ReviewConstants.DirtyReviewsTableName);
    //    var entity = review.ToDirtyEntity();
    //    var dirtyReviewsTable = await tableClient.AddEntityAsync(entity,cancellationToken);
    //    return !dirtyReviewsTable.IsError;
    //}

    //public async Task<bool> DeleteDirtyReview(string rowKey, CancellationToken cancellationToken)
    //{
    //    var tableClient = tableServiceClient.GetTableClient(ReviewConstants.DirtyReviewsTableName);
    //    var dirtyReviewsTable = await tableClient.DeleteEntityAsync(ReviewConstants.DirtyReviewsPartitionKey, rowKey, ETag.All, cancellationToken);
    //    return !dirtyReviewsTable.IsError;
    //}

    //public async Task DeleteDirtyReview(List<string> rowKeys, CancellationToken cancellationToken)
    //{
    //    logger.LogInformation("Cleaning up {count} dirty reviews while processing", rowKeys.Count);
    //    var tableClient = tableServiceClient.GetTableClient(ReviewConstants.DirtyReviewsTableName);
    //    var tableActions = new List<TableTransactionAction>();
    //    foreach (var rowKey in rowKeys)
    //    {
    //        var entity = new DirtyReviewEntity()
    //        {
    //            PartitionKey = ReviewConstants.DirtyReviewsPartitionKey,
    //            RowKey = rowKey
    //        };
    //        tableActions.Add(new TableTransactionAction(TableTransactionActionType.Delete, entity, ETag.All));
    //        if (tableActions.Count == 100)
    //        {
    //            logger.LogInformation("Submitting a transaction with a batch of {count} dirty reviews for deletion", tableActions.Count);
    //            var batchResponse = await tableClient.SubmitTransactionAsync(tableActions, cancellationToken);
    //            var errorCount = batchResponse.Value.Count(x => x.IsError);
    //            logger.LogInformation("Deleted {count} dirty reviews with {success} OK and {error} error results", tableActions.Count, 100-errorCount, errorCount);
    //            tableActions.Clear();
    //        }
    //    }

    //    if (tableActions.Count > 0)
    //    {
    //        logger.LogInformation("Submitting a transaction with a batch of {count} dirty reviews for deletion", tableActions.Count);
    //        var batchResponse = await tableClient.SubmitTransactionAsync(tableActions, cancellationToken);
    //        var errorCount = batchResponse.Value.Count(x => x.IsError);
    //        logger.LogInformation("Deleted {count} dirty reviews with {success} OK and {error} error results", tableActions.Count, 100 - errorCount, errorCount);
    //    }
    //}

    //public async Task<List<IReview>> ReadDirtyReviews(int pageSize, string? continuationToken, CancellationToken cancellationToken)
    //{
    //    var responseList = new List<IReview>();
    //    var tableClient = tableServiceClient.GetTableClient(ReviewConstants.DirtyReviewsTableName);
    //    var query = tableClient.QueryAsync<DirtyReviewEntity>(filter: "", maxPerPage: pageSize);
    //    await foreach (var response in query.AsPages(continuationToken).WithCancellation(cancellationToken))
    //    {
    //        responseList.AddRange(
    //            response.Values.Select(v => new Review(
    //                Guid.Parse(v.RowKey),
    //                v.OrganizationId,
    //                v.ConferenceId,
    //                v.PresentationId,
    //                v.ReviewerId,
    //                v.Content,
    //                v.Speaker,
    //                v.Delivery,
    //                v.Interaction,
    //                v.Comment,
    //                v.ReviewedOn)));
    //    }
    //    return responseList.OrderBy(r => r.PresentationId).ToList();
    //}


}