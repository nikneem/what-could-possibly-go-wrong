namespace Votr.Core.Abstractions.DomainEvents;

public interface IDomainEvent
{
    Guid EventId { get; }
    bool AllowDuplicates { get; }
}