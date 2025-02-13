using Microsoft.Extensions.Options;

namespace Votr.Core.Configuration;

public class AzureServiceConfigurationValidation : IValidateOptions<AzureServiceConfiguration>
{
    public ValidateOptionsResult Validate(string? name, AzureServiceConfiguration options)
    {
        var errorList = new List<string>();
        if (string.IsNullOrWhiteSpace(options.CosmosDbEndpoint))
        {
            errorList.Add($"Reviews service misconfiguration! Missing configuration value for '{AzureServiceConfiguration.DefaultSectionName}.{nameof(AzureServiceConfiguration.CosmosDbEndpoint)}'.");
        }
        if (string.IsNullOrWhiteSpace(options.CosmosDbDatabase))
        {
            errorList.Add($"Reviews service misconfiguration! Missing configuration value for '{AzureServiceConfiguration.DefaultSectionName}.{nameof(AzureServiceConfiguration.CosmosDbDatabase)}'.");
        }
        if (string.IsNullOrWhiteSpace(options.SurveysContainer))
        {
            errorList.Add($"Reviews service misconfiguration! Missing configuration value for '{AzureServiceConfiguration.DefaultSectionName}.{nameof(AzureServiceConfiguration.SurveysContainer)}'.");
        }
        if (string.IsNullOrWhiteSpace(options.WebPubSub))
        {
            errorList.Add($"Reviews service misconfiguration! Missing configuration value for '{AzureServiceConfiguration.DefaultSectionName}.{nameof(AzureServiceConfiguration.WebPubSub)}'.");
        }

        

        if (errorList.Any())
        {
            return ValidateOptionsResult.Fail(errorList);
        }

        return ValidateOptionsResult.Success;
    }
}