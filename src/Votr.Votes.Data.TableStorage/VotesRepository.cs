using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using Votr.Votes.Abstractions;
using Votr.Votes.Configuration;
using Votr.Votes.Data.TableStorage.Entities;
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

    public async Task<List<Vote>> ListPerQuestion( Guid surveyId, Guid questionId, CancellationToken cancellationToken)
    {
        var votes = new List<Vote>();
        var votesTableName = options.Value.VotesTable;
        var tableClient = tableServiceClient.GetTableClient(votesTableName);
        var query = tableClient.QueryAsync<VoteEntity>(ent => ent.PartitionKey == questionId.ToString() && ent.SurveyId == surveyId);
        
        await foreach (var page in query.AsPages().WithCancellation(cancellationToken))
        {
            votes.AddRange(page.Values.Select(p => new Vote(Guid.Parse( p .RowKey), Guid.Parse(p.PartitionKey), p.SurveyId, p.AnswerOption)));
        }

        return votes;
    }
    
}