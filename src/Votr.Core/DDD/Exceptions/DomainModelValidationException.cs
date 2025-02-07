namespace Votr.Core.DDD.Exceptions;

public class DomainModelValidationException(IEnumerable<DomainValidationError> validationErrors) : DomainException("Validation errors occured in the domain model")
{
    public IEnumerable<DomainValidationError> ValidationErrors { get; } = validationErrors;
}