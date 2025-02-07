namespace Votr.Surveys.DataTransferObjects.Create;

public record SurveyCreateRequest(string Name, DateTimeOffset? ExpiresOn, List<SurveyCreateQuestion> Questions);