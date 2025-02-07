namespace Votr.Surveys.DataTransferObjects.Create;

public record SurveyCreateQuestion(string Text, List<SurveyCreateAnswerOption> AnswerOptions);