using Votr.Core.DDD;
using Votr.Core.DDD.Enums;

namespace Votr.Surveys.DomainModels;

public class Question:DomainModel<Guid>
{
    private readonly List<AnswerOption> _answerOptions;

    public int Order { get; private set; }
    public string Text { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyList<AnswerOption> AnswerOptions => _answerOptions.AsReadOnly();

    public bool IsValid => AnswerOptions.Count > 1;

    internal AnswerOption GetAnswerOption(Guid id)
    {
        return AnswerOptions.First(a => a.Id == id);
    }
    internal void AddAnswerOption(string text)
    {
        var answerOption = AnswerOption.Create(text, _answerOptions.Count + 2);
        _answerOptions.Add(answerOption);
    }
    internal void RemoveAnswerOption(AnswerOption answerOption)
    {
        _answerOptions.Remove(answerOption);
    }
    internal void RemoveAnswerOption(Guid id)
    {
        var answerOption = _answerOptions.First(a => a.Id == id);
        _answerOptions.Remove(answerOption);
    }

    internal void SetText(string text)
    {
        if (!Equals(Text, text))
        {
            Text = text;
            SetTrackingState(TrackingState.Modified);
        }
    }


    public Question(Guid id, string text, int order, bool isActive, List<AnswerOption> answerOptions) : base(id)
    {
        Text = text;
        Order = order;
        IsActive = isActive;
        _answerOptions = answerOptions;
    }

    public Question(string text, int order) : base(Guid.NewGuid(), TrackingState.New)
    {
        Text = text;
        Order = order;
        _answerOptions = [];
    }

    public static Question Create(string text, int? displayOrder = null)
    {
        var order = displayOrder ?? 99;
        return new Question(text, order);
    }

    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            SetTrackingState(TrackingState.Modified);
        }
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            SetTrackingState(TrackingState.Modified);
        }
    }
}