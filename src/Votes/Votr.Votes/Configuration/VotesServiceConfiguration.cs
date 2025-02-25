namespace Votr.Votes.Configuration;

public class VotesServiceConfiguration
{
    public const string DefaultSectionName = "Votes";

    public string StorageAccountName { get; set; } = null!;

    public string Votes { get; set; } = null!;
    
}
