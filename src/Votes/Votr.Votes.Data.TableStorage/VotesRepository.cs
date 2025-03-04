using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using Votr.Votes.Abstractions;
using Votr.Votes.Configuration;
using Votr.Votes.Data.TableStorage.Entities;
using Votr.Votes.Data.TableStorage.Mappings;
using Votr.Votes.DomainModels;

namespace Votr.Votes.Data.TableStorage;

public class VotesRepository(TableServiceClient tableServiceClient) : IVotesRepository
{

    private const string VotesTableName = "votes";

    public async Task<bool> Save(Vote vote, CancellationToken cancellationToken)
    {
        var tableClient = tableServiceClient.GetTableClient(VotesTableName);
        await tableClient.CreateIfNotExistsAsync(cancellationToken);
        var entity = vote.ToEntity();
        var dirtyReviewsTable = await tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken);
        return !dirtyReviewsTable.IsError;
    }

    public async Task<List<Vote>> ListPerQuestion( Guid surveyId, Guid questionId, CancellationToken cancellationToken)
    {
        var votes = new List<Vote>();
        var tableClient = tableServiceClient.GetTableClient(VotesTableName);
        var query = tableClient.QueryAsync<VoteEntity>(ent => ent.PartitionKey == questionId.ToString() && ent.SurveyId == surveyId);
        
        await foreach (var page in query.AsPages().WithCancellation(cancellationToken))
        {
            votes.AddRange(page.Values.Select(p => new Vote(Guid.Parse( p .RowKey), Guid.Parse(p.PartitionKey), p.SurveyId, p.AnswerOption)));
        }

        return votes;
    }
    
}