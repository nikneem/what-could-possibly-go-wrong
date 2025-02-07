using Votr.Core;
using Votr.Core.DDD;
using Votr.Core.DDD.Enums;

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

    public void AddAnswerOption(Question question, string text)
    {
        question.AddAnswerOption(text);
        SetTrackingState(TrackingState.Modified);
    }
    public void UpdateAnswerOption(Question question, Guid id, string text)
    {
        var answer = question.GetAnswerOption(id);
        question.SetText(text);
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

}