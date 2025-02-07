namespace Votr.Core.DataTransferObjects;

public record VotrResponse<TObject>(bool IsSuccess, TObject Data, string? ErrorMessage = null)
{
    public static VotrResponse<TObject> Success(TObject result) => new(true, result);
    public static VotrResponse<TObject> Failure(string errorMessage) => new(false, default!, errorMessage);
}