using Dapr.Client;
using Votr.Core.Caching;
using Votr.Core.Caching.Models;
using Votr.Core.DataTransferObjects;
using Votr.Votes.Abstractions;
using Votr.Votes.DataTransferObjects;
using Votr.Votes.DomainModels;

namespace Votr.Votes.Services;

public class VotesService(DaprClient daprClient, IVotesRepository repository) : IVotesService
{
    public async Task<VotrResponse<QuestionVotesResponse>> StoreVote(Guid voterId, VoteCreateRequest requestData, CancellationToken cancellationToken)
    {

        var vote = new Vote(voterId, requestData.QuestionId, requestData.SurveyId, requestData.AnswerId);
        if (await repository.Save(vote, cancellationToken))
        {
            var responseData = await UpdateQuestionVotesInStateStore(
                requestData.SurveyId,
                requestData.QuestionId,
                requestData.AnswerId,
                voterId,
                cancellationToken);

            if (responseData != null)
            {
                return VotrResponse<QuestionVotesResponse>.Success(responseData);
            }
            return VotrResponse<QuestionVotesResponse>.Failure("Vote was stored, but the votes summary for this question could not be produced");
        }
        return VotrResponse<QuestionVotesResponse>.Failure("Failed to store your vote");


    }


    private async Task<QuestionVotesResponse?> UpdateQuestionVotesInStateStore(Guid surveyId, Guid questionId, Guid answerId, Guid voterId, CancellationToken cancellationToken)
    {
        var cacheKey = CacheName.QuestionVotes(surveyId, questionId);
        var votesState = await daprClient.GetStateAsync<QuestionVotesResponse>(
            "votr-state-store",
        cacheKey,
            cancellationToken: cancellationToken);
        if (votesState != null)
        {
            // Remove the old vote if this voter has already voted
            var oldAnswer = votesState.Answers.FirstOrDefault(a => a.Voters.Contains(voterId));
            if (oldAnswer != null)
            {
                oldAnswer.Voters.Remove(voterId);
            }

            var newAnswer = votesState.Answers.FirstOrDefault(a => a.AnswerId == answerId);
            if (newAnswer != null)
            {
                newAnswer.Voters.Add(voterId);
            }

            // Save state
            await daprClient.SaveStateAsync(
                "votr-state-store",
                cacheKey,
            votesState,
                cancellationToken: cancellationToken);
        }

        return votesState;
    }
}