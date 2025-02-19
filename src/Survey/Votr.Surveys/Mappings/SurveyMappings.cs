using Votr.Surveys.DataTransferObjects.Create;
using Votr.Surveys.DataTransferObjects.Details;
using Votr.Surveys.DomainModels;

namespace Votr.Surveys.Mappings;

public static class SurveyMappings
{
    public static Survey FromCreateModel(this SurveyCreateRequest requestData)
    {
        var survey = Survey.Create(requestData.Name, requestData.ExpiresOn);
        if (requestData.Questions != null)
        {
            foreach (var question in requestData.Questions)
            {
                var q = survey.AddQuestion(question.Text);
                foreach (var answerOption in question.AnswerOptions)
                {
                    survey.AddAnswerOption(q, answerOption.Text);
                }
            }
        }

        return survey;
    }

    public static SurveyDetailsResponse ToDetailsResponse(this Survey survey)
    {
        return new SurveyDetailsResponse(survey.Id, survey.Name, survey.Code, survey.ExpiresOn, survey.Questions.Select(q => q.ToDetailsResponse()).ToList());
    }
    public static SurveyQuestion ToDetailsResponse(this Question question)
    {
        return new SurveyQuestion(question.Id, question.Text, question.Order, question.IsActive,  question.AnswerOptions.Select(a => a.ToDetailsResponse()).ToList());
    }

    public static SurveyAnswerOption ToDetailsResponse(this AnswerOption answerOption)
    {
        return new SurveyAnswerOption(answerOption.Id, answerOption.Text, answerOption.Order);
    }


    //public static Question FromCreateModel(this SurveyCreateQuestion requestData)
    //{
    //    return Question.Create(requestData.Text);
    //}
    //public static AnswerOption FromCreateModel(this SurveyCreateAnswerOption requestData)
    //{
    //    return AnswerOption.Create(requestData.Text);
    //}
}