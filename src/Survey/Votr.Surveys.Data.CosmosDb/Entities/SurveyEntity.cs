using Votr.Core.CosmosDb;

namespace Votr.Surveys.Data.CosmosDb.Entities;

public class SurveyEntity : VotrDataEntity<Guid>
{
    public required string Name { get; set; }
    public required string Code { get; set; }
    public DateTimeOffset ExpiresOn { get; set; }
    public required List<SurveyQuestionEntity> Questions { get; set; }
}