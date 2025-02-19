using Votr.Votes.DomainModels;

namespace Votr.Votes.Abstractions;

public interface IVotesRepository
{
    Task<bool> Save(Vote vote, CancellationToken cancellationToken);
}