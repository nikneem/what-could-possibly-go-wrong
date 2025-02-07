namespace Votr.Core.DDD.Enums;

public abstract class TrackingState
{

    public static TrackingState New;
    public static TrackingState Pristine;
    public static TrackingState Touched;
    public static TrackingState Modified;
    public static TrackingState Deleted;

    public static TrackingState[] All;
    public static TrackingState FromKey(string key)
    {
        if (All.All(k => k.Key != key))
        {
            throw new ArgumentException($"Unknown tracking state key: {key}");
        }
        return All.First(x => x.Key == key);
    }

    public abstract string Key { get; }
    public virtual bool CanSwitchTo(TrackingState newState)
    {
        return false;
    }

    static TrackingState()
    {
        All =
        [
            New = new TrackingStateNew(),
            Pristine = new TrackingStatePristine(),
            Touched = new TrackingStateTouched(),
            Modified = new TrackingStateModified(),
            Deleted = new TrackingStateDeleted()
        ];
    }
}