namespace Votr.Core.Abstractions.DomainEvents;

public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent domainEvent);
    Task Dispatch(IEnumerable<IDomainEvent> domainEvents);
}