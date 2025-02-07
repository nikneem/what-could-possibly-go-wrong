namespace Votr.Core.DDD.Enums;

public sealed class TrackingStateDeleted : TrackingState
{
    public override string Key => TrackingStateKey.Deleted;
}