
namespace Votr.Surveys.DataTransferObjects.Update;

public record SurveyUpdateQuestion(Guid? Id, string Text, int Order, List<SurveyUpdateAnswerOption> Answers);