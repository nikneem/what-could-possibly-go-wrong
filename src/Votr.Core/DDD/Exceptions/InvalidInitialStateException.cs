namespace Votr.Core.DDD.Exceptions;

public class InvalidInitialStateException(string initialState)
    : DomainException($"The initial state {initialState} is not allowed as an initial state of a domain model");
