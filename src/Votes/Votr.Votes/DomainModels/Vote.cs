using Votr.Core.DDD;

namespace Votr.Votes.DomainModels;

public class Vote : DomainModel<Guid>
{
    public Vote(Guid id, Guid questionId, Guid surveyId, Guid answerOption) : base(id)
    {
        QuestionId = questionId;
        SurveyId = surveyId;
        AnswerOption = answerOption;
    }

    public Guid QuestionId { get; }
    public Guid SurveyId { get; }
    public Guid AnswerOption { get; }
}