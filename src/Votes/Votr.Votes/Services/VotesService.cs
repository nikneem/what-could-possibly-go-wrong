using Microsoft.Extensions.Options;
using System.Text.Json;
using Azure.Core;
using Votr.Core;
using Votr.Core.Abstractions.Caching;
using Votr.Core.Caching;
using Votr.Core.Caching.Models;
using Votr.Core.DataTransferObjects;
using Votr.Core.Identity;
using Votr.Votes.Abstractions;
using Votr.Votes.DataTransferObjects;
using Votr.Votes.DomainModels;
using Votr.Core.Configuration;

namespace Votr.Votes.Services;

public class VotesService( IVotesRepository repository, IVotrCacheService cacheService, IOptions<AzureServiceConfiguration> options) : IVotesService
{
    public async Task<VotrResponse<QuestionVotesResponse>> StoreVote(Guid voterId, VoteCreateRequest requestData, CancellationToken cancellationToken)
    {
        var vote = new Vote(voterId, requestData.QuestionId, requestData.SurveyId, requestData.AnswerId);
        if (await repository.Save(vote, cancellationToken))
        {
            var cachedQuestionVotes = await UpdateQuestionVotesInStateStore(
                requestData.SurveyId,
                requestData.QuestionId,
                requestData.AnswerId,
                voterId,
                cancellationToken);

            if (cachedQuestionVotes != null)
            {
                var responseModel = QuestionVotesResponse.FromCacheData(cachedQuestionVotes);
                await BroadcastSurveyQuestionVotesChanged(responseModel, cancellationToken);
                return VotrResponse<QuestionVotesResponse>.Success(responseModel);
            }
            return VotrResponse<QuestionVotesResponse>.Failure("Vote was stored, but the votes summary for this question could not be produced");
        }
        return VotrResponse<QuestionVotesResponse>.Failure("Failed to store your vote");
    }

    public async Task<VotrResponse<QuestionVotesCacheDto>> GetInitialVotes(
        Guid surveyId, 
        Guid questionId,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheName.QuestionVotes(surveyId, questionId);
        var votes = await repository.ListPerQuestion(surveyId, questionId, cancellationToken);

        var votesState = await cacheService.GetAsAsync<QuestionVotesCacheDto>(cacheKey);
        if (votesState != null)
        {
            var groupedVotes = votes.GroupBy(v => v.AnswerOption).Select(g => new
            {
                AnswerId = g.Key,
                Voters = g.Select(v => v.Id).ToList()
            }).ToList();
            foreach (var vote in groupedVotes)
            {
                var answer = votesState.Answers.FirstOrDefault(a => a.AnswerId == vote.AnswerId);
                if (answer != null)
                {
                    answer.Voters.Clear();
                    answer.Voters.AddRange(vote.Voters);
                }
            }

            await cacheService.SetAsAsync(cacheKey, votesState, 60);

            return VotrResponse<QuestionVotesCacheDto>.Success(votesState);
        }
        return VotrResponse<QuestionVotesCacheDto>.Failure("Could not fetch question votes from cache");
    }

    private async Task<QuestionVotesCacheDto?> UpdateQuestionVotesInStateStore(Guid surveyId, Guid questionId, Guid answerId, Guid voterId, CancellationToken cancellationToken)
    {
        var cacheKey = CacheName.QuestionVotes(surveyId, questionId);
        var votesState = await cacheService.GetAsAsync<QuestionVotesCacheDto>(cacheKey);
        if (votesState != null)
        {
            // Remove the old vote if this voter has already voted
            var oldAnswer = votesState.Answers.FirstOrDefault(a => a.Voters.Contains(voterId));
            oldAnswer?.Voters.Remove(voterId);

            var newAnswer = votesState.Answers.FirstOrDefault(a => a.AnswerId == answerId);
            newAnswer?.Voters.Add(voterId);

            // Save state
            await cacheService.SetAsAsync(cacheKey, votesState, 60);
        }

        return votesState;
    }

    private async Task BroadcastSurveyQuestionVotesChanged(QuestionVotesResponse votes, CancellationToken cancellationToken)
    {
        //var pubSubClient = GetWebPubSubServiceClient();
        //var realtimeMessage = new RealtimeMessage<QuestionVotesResponse>(RealtimeMessage.SurveyQuestionVotesChanged, votes);

        //var json = JsonSerializer.Serialize(realtimeMessage, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        //await pubSubClient.SendToGroupAsync(
        //    group: votes.SurveyCode,
        //    content: json,
        //    contentType: ContentType.ApplicationJson);
    }

    //private WebPubSubServiceClient GetWebPubSubServiceClient()
    //{
    //    var configurationOptions = options.Value;
    //    var webPubSubEndpoint = new Uri(configurationOptions.WebPubSub);

    //    var pubSubClient = new WebPubSubServiceClient(webPubSubEndpoint, options.Value.WebPubSubHub, CloudIdentity.GetCloudIdentity());
    //    return pubSubClient;
    //}

}