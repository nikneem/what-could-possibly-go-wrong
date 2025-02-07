namespace Votr.Surveys.Data.CosmosDb.Entities;

public record AnswerOptionEntity(Guid Id, string Text, int Order);