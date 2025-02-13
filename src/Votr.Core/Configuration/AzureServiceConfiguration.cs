namespace Votr.Core.Configuration;

public class AzureServiceConfiguration
{
    public const string DefaultSectionName = "AzureServices";

    public string CosmosDbEndpoint { get; set; } = null!;
    public string CosmosDbKey { get; set; } = null!;
    public string CosmosDbDatabase { get; set; } = null!;

    public string SurveysContainer { get; set; } = null!;
    public string WebPubSub { get; set; } = null!;
    public string WebPubSubHub { get; set; } = null!;
    


}
