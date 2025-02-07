namespace Votr.Core.DDD.Enums;

public sealed class TrackingStateModified : TrackingState
{
    public override string Key => TrackingStateKey.Modified;
    public override bool CanSwitchTo(TrackingState newState)
    {
        return newState == Deleted;
    }
}