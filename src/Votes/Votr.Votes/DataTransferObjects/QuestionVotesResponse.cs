using Votr.Core.Caching.Models;

namespace Votr.Votes.DataTransferObjects;

public record QuestionVotesResponse(
    Guid SurveyId,
    string SurveyCode,
    Guid QuestionId,
    string Question,
    List<QuestionVoteAnswerResponse> Answers
)
{
    public static QuestionVotesResponse FromCacheData(QuestionVotesCacheDto cachedQuestionVotes)
    {
        var totalVotes = cachedQuestionVotes.Answers.Sum(a => a.VoteCount);
        return new QuestionVotesResponse(
            cachedQuestionVotes.SurveyId,
            cachedQuestionVotes.SurveyCode,
            cachedQuestionVotes.QuestionId,
            cachedQuestionVotes.Question,
            cachedQuestionVotes.Answers.Select(a => new QuestionVoteAnswerResponse(
                a.AnswerId,
                a.Name,
                a.VoteCount,
                totalVotes == 0 ? 0 : (double)a.VoteCount / totalVotes
            )).ToList()
        );
    }
}