namespace Votr.Core.Caching.Models;

public record QuestionVotesResponse(Guid SurveyId, Guid QuestionId, string Question, List<QuestionAnswer> Answers);