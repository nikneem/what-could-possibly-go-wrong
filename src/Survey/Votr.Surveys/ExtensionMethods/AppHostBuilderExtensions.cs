using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Votr.Surveys.Abstractions;
using Votr.Surveys.Services;

namespace Votr.Surveys.ExtensionMethods;

public static class AppHostBuilderExtensions
{


    public static IHostApplicationBuilder AddSurveysApi(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISurveysService, SurveysService>();

        builder.AddAzureCosmosClient("votr", configureClientOptions: options =>
        {
            options.UseSystemTextJsonSerializerWithOptions = JsonSerializerOptions.Web;
        });

        return builder;
    }

}