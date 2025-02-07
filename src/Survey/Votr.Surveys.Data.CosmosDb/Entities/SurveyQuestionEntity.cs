namespace Votr.Surveys.Data.CosmosDb.Entities;

public record SurveyQuestionEntity(Guid Id, string Text, int Order, List<AnswerOptionEntity> AnswerOptions);