namespace Votr.Surveys.Data.CosmosDb.Entities;

public record SurveyQuestionEntity(
    Guid Id, 
    string Text, 
    int Order, 
    bool IsActive, 
    List<AnswerOptionEntity> AnswerOptions);