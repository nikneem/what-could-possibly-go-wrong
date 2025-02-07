namespace Votr.Core.DDD.Enums;

public sealed class TrackingStateTouched : TrackingState
{
    public override string Key => TrackingStateKey.Touched;
    public override bool CanSwitchTo(TrackingState newState)
    {
        return newState == Deleted || newState == Modified;

    }
}