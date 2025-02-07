using Votr.Surveys.Data.CosmosDb.Entities;
using Votr.Surveys.DomainModels;

namespace Votr.Surveys.Data.CosmosDb.Mappings;

public static class SurveyMappings
{

    public static SurveyEntity ToEntity(this Survey domainModel)
    {

        var questions = domainModel.Questions.Select(q => q.ToEntity()).ToList();
        return new SurveyEntity
        {
            Id = domainModel.Id,
            Name = domainModel.Name,
            Code = domainModel.Code,
            ExpiresOn = domainModel.ExpiresOn,
            Questions = questions
        };

    }

    public static SurveyQuestionEntity ToEntity(this Question domainModel)
    {
        var answerOptions = domainModel.AnswerOptions.Select(a => a.ToEntity()).ToList();
        return new SurveyQuestionEntity(domainModel.Id, domainModel.Text, domainModel.Order, answerOptions);
    }
    public static AnswerOptionEntity ToEntity(this AnswerOption domainModel)
    {
        return new AnswerOptionEntity(domainModel.Id, domainModel.Text, domainModel.Order);
    }

}