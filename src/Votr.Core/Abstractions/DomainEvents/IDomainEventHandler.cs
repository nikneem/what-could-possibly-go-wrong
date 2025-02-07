namespace Votr.Core.Abstractions.DomainEvents;

public interface IDomainEventHandler<in TDomainEvent> : IDomainEventHandler where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent domainEvent);
}
public interface IDomainEventHandler
{
    Task Handle(IDomainEvent @event);
}
