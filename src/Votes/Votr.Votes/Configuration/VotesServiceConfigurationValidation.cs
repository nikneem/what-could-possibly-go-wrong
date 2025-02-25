using Microsoft.Extensions.Options;

namespace Votr.Votes.Configuration;

public class VotesServiceConfigurationValidation : IValidateOptions<VotesServiceConfiguration>
{
    public ValidateOptionsResult Validate(string? name, VotesServiceConfiguration options)
    {
        var errorList = new List<string>();
        if (string.IsNullOrWhiteSpace(options.StorageAccountName))
        {
            errorList.Add($"Votes service misconfiguration! Missing configuration value for '{VotesServiceConfiguration.DefaultSectionName}.{nameof(VotesServiceConfiguration.StorageAccountName)}'.");
        }
        if (string.IsNullOrWhiteSpace(options.Votes))
        {
            errorList.Add($"Votes service misconfiguration! Missing configuration value for '{VotesServiceConfiguration.DefaultSectionName}.{nameof(VotesServiceConfiguration.Votes)}'.");
        }

        if (errorList.Any())
        {
            return ValidateOptionsResult.Fail(errorList);
        }

        return ValidateOptionsResult.Success;
    }
}