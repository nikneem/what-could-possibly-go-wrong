using Votr.Core.DataTransferObjects;
using Votr.Surveys.DataTransferObjects.Create;
using Votr.Surveys.DataTransferObjects.Details;

namespace Votr.Surveys.Abstractions;

public interface ISurveysService
{

    Task<VotrResponse<List<SurveyDetailsResponse>>> List(CancellationToken cancellationToken);

    Task<VotrResponse<SurveyDetailsResponse>> Create(
        SurveyCreateRequest requestData,
        CancellationToken cancellationToken);
}