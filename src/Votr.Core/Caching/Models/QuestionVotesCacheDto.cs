namespace Votr.Core.Caching.Models;

public record QuestionVotesCacheDto(
    Guid SurveyId, 
    string SurveyCode,
    Guid QuestionId, 
    string Question, 
    List<QuestionAnswer> Answers);