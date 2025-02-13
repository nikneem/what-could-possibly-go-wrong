using Azure.Messaging.WebPubSub;
using Microsoft.Extensions.Options;
using Votr.Core.Configuration;
using Votr.Core.DataTransferObjects;
using Votr.Core.Identity;
using Votr.Surveys.Abstractions;
using Votr.Surveys.DataTransferObjects;
using Votr.Surveys.DataTransferObjects.Create;
using Votr.Surveys.DataTransferObjects.Details;
using Votr.Surveys.DataTransferObjects.Update;
using Votr.Surveys.Mappings;

namespace Votr.Surveys.Services;

public class SurveysService(ISurveysRepository surveysRepository, IOptions<AzureServiceConfiguration> options) : ISurveysService
{
    public async Task<VotrResponse<List<SurveyDetailsResponse>>> List(CancellationToken cancellationToken)
    {
        try
        {
            var surveys = await surveysRepository.List(cancellationToken);
            return VotrResponse<List<SurveyDetailsResponse>>.Success(surveys);
        }
        catch (Exception ex)
        {
            return VotrResponse<List<SurveyDetailsResponse>>.Failure(ex.Message);
        }
    }

    public async Task<VotrResponse<SurveyDetailsResponse>> Get(string code, CancellationToken cancellationToken)
    {
        try
        {
            var survey = await surveysRepository.Get(code,cancellationToken);
            return VotrResponse<SurveyDetailsResponse>.Success(survey.ToDetailsResponse());
        }
        catch (Exception ex)
        {
            return VotrResponse<SurveyDetailsResponse>.Failure(ex.Message);
        }
    }

    public async Task<VotrResponse<SurveyDetailsResponse>> Create(
        SurveyCreateRequest requestData,
        CancellationToken cancellationToken)
    {
        var survey = requestData.FromCreateModel();
        if (await surveysRepository.Save(survey, cancellationToken))
        {
            return VotrResponse<SurveyDetailsResponse>.Success(survey.ToDetailsResponse());
        }
        return VotrResponse<SurveyDetailsResponse>.Failure("Failed to save survey");
    }

    public async Task<VotrResponse<SurveyDetailsResponse>> Update(string code, SurveyUpdateRequest requestPayload, CancellationToken cancellationToken)
    {
        try
        {
            var survey = await surveysRepository.Get(code, cancellationToken);
            survey.Update(requestPayload);
            if (await surveysRepository.Save(survey, cancellationToken))
            {
                return VotrResponse<SurveyDetailsResponse>.Success(survey.ToDetailsResponse());
            }
            return VotrResponse<SurveyDetailsResponse>.Failure("Failed to save survey");
        }
        catch (Exception ex)
        {
            return VotrResponse<SurveyDetailsResponse>.Failure(ex.Message);
        }
    }

    public async Task<WebPubsubConnectionResponse> CreateWebPubSubConnectionString(string code, Guid voterId, CancellationToken cancellationToken)
    {
        var configurationOptions = options.Value;
        var webPubSubEndpoint = new Uri(configurationOptions.WebPubSub);

        var pubSubClient = new WebPubSubServiceClient(webPubSubEndpoint, options.Value.WebPubSubHub, CloudIdentity.GetCloudIdentity());
        var clientAccess = await pubSubClient.GetClientAccessUriAsync(
            userId: voterId.ToString(),
            roles:
            [
                $"webpubsub.sendToGroup.{code}",
                $"webpubsub.joinLeaveGroup.{code}"
            ],
            cancellationToken:cancellationToken);

        return new WebPubsubConnectionResponse(clientAccess.ToString());
    }

    //public async Task<VotrResponse<SurveyDetailsResponse>> Update(
    //    SurveyUpdateRequest requestData,
    //    CancellationToken cancellationToken)
    //{
    //    var survey = requestData.FromCreateModel();
    //    if (await surveysRepository.Save(survey, cancellationToken))
    //    {
    //        return VotrResponse<SurveyDetailsResponse>.Success(survey.ToDetailsResponse());
    //    }
    //    return VotrResponse<SurveyDetailsResponse>.Failure("Failed to save survey");
    //}

}