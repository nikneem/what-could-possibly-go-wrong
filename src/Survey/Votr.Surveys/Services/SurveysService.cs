using System.Text.Json;
using Azure.Core;
using Azure.Messaging.WebPubSub;
using Microsoft.Extensions.Logging;
using Votr.Core;
using Votr.Core.Abstractions.Caching;
using Votr.Core.Caching;
using Votr.Core.Caching.Models;
using Votr.Core.DataTransferObjects;
using Votr.Surveys.Abstractions;
using Votr.Surveys.DataTransferObjects;
using Votr.Surveys.DataTransferObjects.Create;
using Votr.Surveys.DataTransferObjects.Details;
using Votr.Surveys.DataTransferObjects.Update;
using Votr.Surveys.DomainModels;
using Votr.Surveys.Mappings;

namespace Votr.Surveys.Services;

public class SurveysService(
    ISurveysRepository surveysRepository, 
    IVotrCacheService cacheService,
    WebPubSubServiceClient webPubSubClient,
    ILogger<SurveysService> logger) : ISurveysService
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

    public async Task<VotrResponse<SurveyDetailsResponse>> Get(
        string code, 
        CancellationToken cancellationToken)
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

    public async Task<VotrResponse<SurveyDetailsResponse>> Update(
        string code, 
        SurveyUpdateRequest requestPayload, 
        CancellationToken cancellationToken)
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

    public async Task<VotrResponse<SurveyDetailsResponse>> ActivateQuestion(
        string code, 
        Guid questionId, 
        CancellationToken cancellationToken)
    {
        try
        {
            var survey = await surveysRepository.Get(code, cancellationToken);
            if (survey.Questions.Any(q => q.Id == questionId))
            {
                var question = survey.ActivateQuestion(questionId);
            if (await surveysRepository.Save(survey, cancellationToken))
            {
                await BroadcastQuestionActivated(survey, question, cancellationToken);
                await AddQuestionToDistributedCache(survey, question, cancellationToken);
                return VotrResponse<SurveyDetailsResponse>.Success(survey.ToDetailsResponse());
            }
            }
            return VotrResponse<SurveyDetailsResponse>.Failure("Failed to save survey");
        }
        catch (Exception ex)
        {
            return VotrResponse<SurveyDetailsResponse>.Failure(ex.Message);
        }
    }

    private async Task AddQuestionToDistributedCache(Survey survey, Question question, CancellationToken cancellationToken)
    {
        var cacheKey = CacheName.QuestionVotes(survey.Id, question.Id);
        var votesState = new QuestionVotesCacheDto(
            survey.Id,
            survey.Code,
            question.Id,
            question.Text,
            question.AnswerOptions.Select(a => new QuestionAnswer(a.Id, a.Text, new List<Guid>())).ToList());

        await cacheService.SetAsAsync(cacheKey, votesState, 60);
    }

    public async Task<WebPubsubConnectionResponse> CreateWebPubSubConnectionString(
        string code, 
        Guid voterId, 
        CancellationToken cancellationToken)
    {
        var clientAccess = await webPubSubClient.GetClientAccessUriAsync(
            userId: voterId.ToString(),
            roles:
            [
                $"webpubsub.sendToGroup.{code}",
                $"webpubsub.joinLeaveGroup.{code}"
            ],
            cancellationToken: cancellationToken);

        return new WebPubsubConnectionResponse(clientAccess.ToString());
    }


    private async Task BroadcastQuestionActivated(Survey survey, Question question, CancellationToken cancellationToken)
    {
        try
        {
            var dataTransferObject = question.ToDetailsResponse();
            var realtimeMessage =
                new RealtimeMessage<SurveyQuestion>(RealtimeMessage.SurveyQuestionActivated, dataTransferObject);

            var json = JsonSerializer.Serialize(realtimeMessage,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await webPubSubClient.SendToGroupAsync(
                group: survey.Code,
                content: json,
                contentType: ContentType.ApplicationJson);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to broadcast question activation message for real-time usage");
        }
    }

}