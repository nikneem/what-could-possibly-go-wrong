namespace Votr.Core.Caching.Models;

public record QuestionAnswer(Guid AnswerId, string Name, List<Guid> Voters)
{
    public int VoteCount => Voters.Count;
}