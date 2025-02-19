using Votr.Votes.Data.TableStorage.Entities;
using Votr.Votes.DomainModels;

namespace Votr.Votes.Data.TableStorage.Mappings;

public static class VoteMappings
{


    public static VoteEntity ToEntity(this Vote vote)
    {
        return new VoteEntity
        {
            PartitionKey = vote.QuestionId.ToString(),
            RowKey = vote.VoterId.ToString(),
            SurveyId = vote.SurveyId,
            AnswerOption = vote.AnswerOption,
            Timestamp = DateTimeOffset.UtcNow
        };

    }

}