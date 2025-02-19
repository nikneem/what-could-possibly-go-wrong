namespace Votr.Votes.DataTransferObjects;

public record VoteCreateRequest(
    Guid SurveyId,
    Guid QuestionId,
    Guid AnswerId);