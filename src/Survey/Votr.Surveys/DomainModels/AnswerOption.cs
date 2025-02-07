using Votr.Core.DDD;
using Votr.Core.DDD.Enums;

namespace Votr.Surveys.DomainModels;

public class AnswerOption : DomainModel<Guid>
{

    public string Text { get; private set; }
    public int Order { get; private set; }


internal    void MoveUp()
    {
        Order = Order - 2;
        SetTrackingState(TrackingState.Modified);
    }
    internal void MoveDown()
    {
        Order = Order + 2;
        SetTrackingState(TrackingState.Modified);
    }

    public AnswerOption(Guid id, string text, int order) : base(id)
    {
        Text = text;
        Order = order;
    }

    public AnswerOption(string text, int order) : base(Guid.NewGuid(), TrackingState.New)
    {
        Text = text;
        Order = order;
    }

    public static AnswerOption Create(string text, int? displayOrder = 0)
    {
        var order = displayOrder ?? 99;
        return new AnswerOption(text, order);
    }
}