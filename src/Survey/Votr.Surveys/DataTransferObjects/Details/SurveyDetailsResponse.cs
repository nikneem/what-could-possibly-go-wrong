namespace Votr.Surveys.DataTransferObjects.Details;

public record SurveyDetailsResponse(Guid Id, string Name, string Code, DateTimeOffset ExpiresOn, List<SurveyQuestion> Question);