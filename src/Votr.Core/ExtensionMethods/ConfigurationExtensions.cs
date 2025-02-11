using Microsoft.Extensions.Configuration;

namespace Votr.Core.ExtensionMethods;

public static class ConfigurationExtensions
{
    public static string GetRequiredValue(this IConfiguration configuration, string key, string? description = null)
    {
        var value = configuration.GetValue<string>(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("Missing setting " + ((description != null) ? ("for " + description) : "") + " : " + key);
        }
        return value;
    }
}