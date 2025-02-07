namespace Votr.Core.Configuration;

public class AzureAppConfigurationSettings
{
    public const string DefaultSectionName = "AzureAppConfiguration";

    public string? EndpointUrl { get; set; }
    public string? ConnectionString { get; set; }
}