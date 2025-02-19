using Azure;
using Azure.Data.Tables;

namespace Votr.Votes.Data.TableStorage.Entities;

public class VoteEntity : ITableEntity
{
    public required string PartitionKey { get; set; } // Question ID
    public required string RowKey { get; set; } // Voter ID
    public Guid SurveyId { get; set; }
    public Guid AnswerOption { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}