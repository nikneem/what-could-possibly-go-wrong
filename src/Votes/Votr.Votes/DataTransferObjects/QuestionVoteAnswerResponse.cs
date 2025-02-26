namespace Votr.Votes.DataTransferObjects;

public record QuestionVoteAnswerResponse(Guid AnswerId, string Name, int Votes, double Percentage);