
namespace Votr.Surveys.DataTransferObjects.Update;

public record SurveyUpdateRequest(string Name, DateTimeOffset ExpiresOn, List<SurveyUpdateQuestion> Questions);