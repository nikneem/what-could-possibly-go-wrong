using Votr.Core.DataTransferObjects;
using Votr.Surveys.DataTransferObjects;
using Votr.Surveys.DataTransferObjects.Create;
using Votr.Surveys.DataTransferObjects.Details;
using Votr.Surveys.DataTransferObjects.Update;

namespace Votr.Surveys.Abstractions;

public interface ISurveysService
{

    Task<VotrResponse<List<SurveyDetailsResponse>>> List(CancellationToken cancellationToken);
    Task<VotrResponse<SurveyDetailsResponse>> Get(string code, CancellationToken cancellationToken);

    Task<VotrResponse<SurveyDetailsResponse>> Create(
        SurveyCreateRequest requestData,
        CancellationToken cancellationToken);

    Task<VotrResponse<SurveyDetailsResponse>> Update(string code, SurveyUpdateRequest requestPayload, CancellationToken cancellationToken);

    Task<VotrResponse<SurveyDetailsResponse>> ActivateQuestion(string code, Guid questionId,  CancellationToken cancellationToken);
    Task<WebPubsubConnectionResponse> CreateWebPubSubConnectionString(string code, Guid voterId, CancellationToken cancellationToken);
}