using Votr.Core;
using Votr.Core.DDD;
using Votr.Core.DDD.Enums;
using Votr.Surveys.DataTransferObjects.Update;

namespace Votr.Surveys.DomainModels;

public class Survey : DomainModel<Guid>
{
    private readonly List<Question> _questions;
    public string Name { get; private set; }
    public string Code { get; }
    public DateTimeOffset ExpiresOn { get; private set; }
    public IReadOnlyList<Question> Questions => _questions.AsReadOnly();

    public bool IsValid => Questions.All(q => q.IsValid);


    public void UpdateName(string value)
    {
        if (ValidateRoomName(value, out string name))
        {
            Name = name;
            SetTrackingState(TrackingState.Modified);
        }
    }

    private bool ValidateRoomName(string value, out string formattedValue)
    {
        formattedValue = null!;
        if (string.IsNullOrWhiteSpace(value))
        {
            AddValidationError(nameof(Name), "The name of a room cannot be null or only contain whitespace");
            return false;
        }
        if (value.Length < 5 || value.Length > 100)
        {
            AddValidationError(nameof(Name), "The room name must contain at least 5 and maximum 100 characters");
            return false;
        }

        formattedValue = value.Trim();
        return !Equals(Name, formattedValue);
    }


    public void UpdateExpiry(DateTimeOffset value)
    {
        if (ValidateExpiryDate(value))
        {
            ExpiresOn = value;
            SetTrackingState(TrackingState.Modified);
        }
    }

    private bool ValidateExpiryDate(DateTimeOffset expiryDate)
    {
        if (expiryDate < DateTimeOffset.UtcNow)
        {
            AddValidationError(nameof(ExpiresOn), "The expiry date must be in the future");
            return false;
        }
        return !Equals(ExpiresOn, expiryDate);
    }

    public void AddQuestion(string text, List<AnswerOption> answerOptions)
    {
        var question = Question.Create(text, _questions.Count + 2);
        _questions.Add(question);
        SetTrackingState(TrackingState.Modified);
    }
    public Question AddQuestion(string text)
    {
        var question = Question.Create(text, _questions.Count + 2);
        _questions.Add(question);
        SetTrackingState(TrackingState.Modified);
        return question;
    }
    public Question UpdateQuestion(Guid id, string text)
    {
        var question = _questions.First(q => q.Id == id);
        question.SetText(text);
        SetTrackingState(TrackingState.Modified);
        return question;
    }
    public void RemoveQuestion(Question question)
    {
        _questions.Remove(question);
        SetTrackingState(TrackingState.Modified);
    }
    public void RemoveQuestion(Guid id)
    {
        var question = _questions.First(q => q.Id == id);
        _questions.Remove(question);
        SetTrackingState(TrackingState.Modified);
    }
    public Question ActivateQuestion(Guid questionId)
    {
        foreach (var q in Questions)
        {
            q.Deactivate();
        }
        var question = _questions.First(q => q.Id == questionId);
        question.Activate();
        SetTrackingState(TrackingState.Modified);
        return question;
    }

    public void AddAnswerOption(Question question, string text)
    {
        question.AddAnswerOption(text);
        SetTrackingState(TrackingState.Modified);
    }

    public void UpdateAnswerOption(Question question, Guid id, string text)
    {
        var answer = question.GetAnswerOption(id);
        if (string.IsNullOrWhiteSpace(text))
        {
            RemoveQuestion(question);
        }
        else
        {
            answer.SetText(text);
        }

        SetTrackingState(TrackingState.Modified);
    }

    public void DeleteAnswerOption(Question question, Guid id)
    {
        var answer = question.GetAnswerOption(id);
        question.RemoveAnswerOption(answer);
        SetTrackingState(TrackingState.Modified);
    }


    public Survey(Guid id, string name, string code, DateTimeOffset expiresOn, List<Question> questions) : base(id)
    {
        Name = name;
        Code = code;
        ExpiresOn = expiresOn;
        _questions = questions;
    }

    private Survey(string name, DateTimeOffset expiresOn) : base(Guid.NewGuid(), TrackingState.New)
    {
        Name = name;
        Code = Randomizer.GetRandomRoomCode();
        ExpiresOn = expiresOn;
        _questions = new List<Question>();
    }

    public static Survey Create(string name, DateTimeOffset? expiresOn = null)
    {
        var expiryDate = expiresOn ?? DateTimeOffset.UtcNow.AddDays(14);
        return new Survey(name, expiryDate);
    }

    public void Update(SurveyUpdateRequest requestPayload)
    {
        UpdateName(requestPayload.Name);
        UpdateExpiry(requestPayload.ExpiresOn);
        foreach (var payloadQuestion in requestPayload.Questions)
        {
            var question = payloadQuestion.Id.HasValue
                ? UpdateQuestion(payloadQuestion.Id.Value, payloadQuestion.Text)
                : AddQuestion(payloadQuestion.Text);

            foreach (var payloadAnswerOption in payloadQuestion.Answers)
            {
                if (payloadAnswerOption.Id.HasValue)
                {
                    UpdateAnswerOption(question, payloadAnswerOption.Id.Value, payloadAnswerOption.Text);
                }
                else if (!string.IsNullOrWhiteSpace(payloadAnswerOption.Text))
                {
                    AddAnswerOption(question, payloadAnswerOption.Text);
                }
            }
            // Delete all answers from the domain model that are not in the DTO
            var answerIds = payloadQuestion.Answers.Select(a => a.Id).ToList();
            var answersToDelete = question.AnswerOptions.Where(a => a.TrackingState != TrackingState.New && !answerIds.Contains(a.Id)).ToList();
            foreach (var answerToDelete in answersToDelete)
            {
                question.RemoveAnswerOption(answerToDelete);
            }
        }
        // Delete all questions from the domain model that are not in the DTO
        var questionIds = requestPayload.Questions.Select(q => q.Id).ToList();
        var questionsToDelete = Questions.Where(q => q.TrackingState != TrackingState.New && !questionIds.Contains(q.Id)).ToList();
        foreach (var question in questionsToDelete)
        {
            RemoveQuestion(question);
        }



    }


}