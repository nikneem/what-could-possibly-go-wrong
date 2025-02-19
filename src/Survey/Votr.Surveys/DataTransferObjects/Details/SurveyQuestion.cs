namespace Votr.Surveys.DataTransferObjects.Details;

public record SurveyQuestion(Guid Id, string Text, int Order, bool IsActive,List<SurveyAnswerOption> AnswerOptions);