namespace Votr.Core.Caching;

public class CacheName
{
    public static string QuestionVotes(Guid surveyId, Guid questionId) => $"votr: surveys:{surveyId}:questions:{questionId}:votes";
}