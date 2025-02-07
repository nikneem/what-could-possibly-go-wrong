namespace Votr.Core.Configuration;

public class AzureServiceConfiguration
{
    public const string DefaultSectionName = "AzureServices";

    public string CosmosDbEndpoint { get; set; } = null!;
    public string CosmosDbKey { get; set; } = null!;
    public string CosmosDbDatabase { get; set; } = null!;

    public string CosmosDbConfigurationContainer { get; set; } = null!;
    public string CosmosDbConferencesContainer { get; set; } = null!;
    public string OrganizationsContainer => CosmosDbConfigurationContainer;
    public string OrganizationInvitationsContainer => CosmosDbConfigurationContainer;
    public string OrganizersContainer => CosmosDbConfigurationContainer;
    public string ConferencesContainer => CosmosDbConferencesContainer;

    public string? EventGridEndpoint { get; set; }
}
