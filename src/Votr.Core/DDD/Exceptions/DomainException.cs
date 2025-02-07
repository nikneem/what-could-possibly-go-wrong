namespace Votr.Core.DDD.Exceptions;

public class DomainException(string message, Exception? inner = null) : Exception(message, inner);
