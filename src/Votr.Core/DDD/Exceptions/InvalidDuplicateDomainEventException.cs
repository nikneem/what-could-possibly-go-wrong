namespace Votr.Core.DDD.Exceptions;

public class InvalidDuplicateDomainEventException(Type domainEventType) : DomainException(
    $"The event of type {domainEventType} was already raised while it is configured to not allow duplicates");
