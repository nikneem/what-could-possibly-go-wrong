
namespace Votr.Core.DDD;

public class DomainValidationError
{
    public required Type DomainModel { get; set; }
    public required string PropertyName { get; set; }
    public required string ErrorMessage { get; set; }

    public static DomainValidationError Create(Type domainModel, string propertyName, string errorMessage)
    {
        return new DomainValidationError
        {
            DomainModel = domainModel,
            PropertyName = propertyName,
            ErrorMessage = errorMessage
        };
    }
}