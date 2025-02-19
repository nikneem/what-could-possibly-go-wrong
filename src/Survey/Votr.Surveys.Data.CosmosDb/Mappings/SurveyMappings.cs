using Votr.Surveys.Data.CosmosDb.Entities;
using Votr.Surveys.DataTransferObjects.Details;
using Votr.Surveys.DomainModels;

namespace Votr.Surveys.Data.CosmosDb.Mappings;

public static class SurveyMappings
{
    public static Survey ToDomainModel(this SurveyEntity entity)
    {
        var questions = entity.Questions.Select(q => q.ToDomainModel()).ToList();
        return new Survey(entity.Id, entity.Name, entity.Code, entity.ExpiresOn, questions);
    }
    public static Question ToDomainModel(this SurveyQuestionEntity entity)
    {
        var answerOptions = entity.AnswerOptions.Select(a => a.ToDomainModel()).ToList();
        return new Question(entity.Id, entity.Text, entity.Order, entity.IsActive,  answerOptions);
    }
    public static AnswerOption ToDomainModel(this AnswerOptionEntity entity)
    {
        return new AnswerOption(entity.Id, entity.Text, entity.Order);
    }
    
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
        return new SurveyQuestionEntity(
            domainModel.Id, 
            domainModel.Text, 
            domainModel.Order, 
            domainModel.IsActive,
            answerOptions);
    }
    public static AnswerOptionEntity ToEntity(this AnswerOption domainModel)
    {
        return new AnswerOptionEntity(domainModel.Id, domainModel.Text, domainModel.Order);
    }

    public static IEnumerable<SurveyDetailsResponse> ToDetailsResponse(this IEnumerable<SurveyEntity> surveys)
    {
        return surveys.Select(s => s.ToDetailsResponse());
        }
    public static SurveyDetailsResponse ToDetailsResponse(this SurveyEntity survey)
    {
        return new SurveyDetailsResponse(survey.Id, survey.Name, survey.Code, survey.ExpiresOn, survey.Questions.Select(q => q.ToDetailsResponse()).ToList());
    }
    public static SurveyQuestion ToDetailsResponse(this SurveyQuestionEntity question)
    {
        return new SurveyQuestion(question.Id, question.Text, question.Order, question.IsActive, question.AnswerOptions.Select(a => a.ToDetailsResponse()).ToList());
    }
    public static SurveyAnswerOption ToDetailsResponse(this AnswerOptionEntity answerOption)
    {
        return new SurveyAnswerOption(answerOption.Id, answerOption.Text, answerOption.Order);
    }
}