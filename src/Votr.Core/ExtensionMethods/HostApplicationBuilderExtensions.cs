using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Votr.Core.Configuration;
using Votr.Core.CosmosDb.ExtensionMethods;
using Votr.Core.Identity;

namespace Votr.Core.ExtensionMethods;

public static class HostApplicationBuilderExtensions
{

    public static IHostApplicationBuilder AddVotrCore(this IHostApplicationBuilder builder)
    {
        var azureAppConfigurationUrl = builder.Configuration.GetSection(AzureAppConfigurationSettings.DefaultSectionName);
        var settings = azureAppConfigurationUrl.Get<AzureAppConfigurationSettings>();
        try
        {
            if (settings != null)
            {
                var azureCredential = CloudIdentity.GetCloudIdentity();

                var appConfigEndpoint = settings.EndpointUrl;
                if (!string.IsNullOrWhiteSpace(appConfigEndpoint))
                {
                    builder.Configuration.AddAzureAppConfiguration(options =>
                    {
                        Console.WriteLine($"Loading configuration from {appConfigEndpoint}");
                        options.Connect(new Uri(appConfigEndpoint), azureCredential)
                            .ConfigureKeyVault(cfg => { cfg.SetCredential(azureCredential); })
                            .UseFeatureFlags(ff => { ff.SetRefreshInterval(TimeSpan.FromMinutes(5)); });
                    });
                }
                else
                {
                    var connectionString = settings.ConnectionString;
                    if (!string.IsNullOrWhiteSpace(connectionString))
                    {
                        builder.Configuration.AddAzureAppConfiguration(options =>
                        {
                            Console.WriteLine("Loading configuration from connectionString");
                            options.Connect(connectionString)
                                .ConfigureKeyVault(cfg => { cfg.SetCredential(azureCredential); })
                                .UseFeatureFlags(ff => { ff.SetRefreshInterval(TimeSpan.FromMinutes(5)); });
                        });
                    }
                    else
                    {
                        throw new Exception("!! App is not configured with Azure App Configuration !!");
                    }
                }
            }
            else
            {
                Console.WriteLine("Skipping configuration through Azure App Configuration");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Cannot load configuration from Azure App Configuration");
            Console.WriteLine(azureAppConfigurationUrl);
            Console.WriteLine(ex.ToString());
        }

        builder.Services
            .AddSingleton<IValidateOptions<AzureServiceConfiguration>, AzureServiceConfigurationValidation>();

        var azureServicesSection = builder.Configuration.GetSection(AzureServiceConfiguration.DefaultSectionName);
        builder.Services.AddOptions<AzureServiceConfiguration>().Bind(azureServicesSection).ValidateOnStart();

        builder.Services.AddVotrCoreServices();
        builder.Services.AddCosmosDb();
        return builder;
    }


}