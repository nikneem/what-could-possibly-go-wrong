using Votr.Core.Caching.Models;
using Votr.Core.DataTransferObjects;
using Votr.Votes.DataTransferObjects;

namespace Votr.Votes.Abstractions;

public interface IVotesService
{
    Task<VotrResponse<QuestionVotesResponse>> StoreVote(Guid voterId, VoteCreateRequest requestData, CancellationToken cancellationToken);
}