using Votr.Core.Abstractions.DomainEvents;
using Votr.Core.DDD.Enums;
using Votr.Core.DDD.Exceptions;

namespace Votr.Core.DDD;

public abstract class DomainModel < TId>
{
    private readonly List<IDomainEvent> _domainEvents;
    private readonly List<DomainValidationError> _validationErrors;

    public TId Id { get; }
    public TrackingState TrackingState { get; private set; }
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public IReadOnlyList<DomainValidationError> ValidationErrors => _validationErrors.AsReadOnly();

    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        if (!domainEvent.AllowDuplicates)
        {
            var eventType = domainEvent.GetType();
            // See if there is already an event of the same type in the list
            if (_domainEvents.Any(evt => evt.GetType() == eventType))
            {
                throw new InvalidDuplicateDomainEventException(eventType);
            }
        }

        if (_domainEvents.All(evt => evt.EventId != domainEvent.EventId))
        {
            _domainEvents.Add(domainEvent);
        }
    }
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        if (_domainEvents.Contains(domainEvent))
        {
            _domainEvents.Remove(domainEvent);
        }
    }
    public virtual void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void AddValidationError(string propertyName, string errorMessage)
    {
        _validationErrors.Add(DomainValidationError.Create(GetType(), propertyName, errorMessage));
    }
    public void AddValidationError(DomainValidationError validationError)
    {
        _validationErrors.Add(validationError);
    }

    protected void SetTrackingState(TrackingState state)
    {
        if (TrackingState.CanSwitchTo(state))
        {
            TrackingState = state;
        }
    }

    protected DomainModel(TId id) : this(id, TrackingState.Pristine)
    {
    }

    protected DomainModel(TId id, TrackingState state)
    {
        if (state != TrackingState.New && state != TrackingState.Pristine)
        {
            throw new InvalidInitialStateException(state.Key);
        }

        Id = id;
        TrackingState = state;
        _domainEvents = [];
        _validationErrors = [];
    }

}