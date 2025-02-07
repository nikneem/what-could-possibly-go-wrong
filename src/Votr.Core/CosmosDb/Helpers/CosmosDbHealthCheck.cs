using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Votr.Core.CosmosDb.Helpers;


public static class CosmosDbHealtCheckExtensions
{
    public static IHealthChecksBuilder AddCosmosDb(this IHealthChecksBuilder builder, string name, string database, string[] containers, string[] tags)
    {
        builder.Services.AddSingleton(new CosmosDbHealthCheckOptions(database, containers));
        builder.AddTypeActivatedCheck<CosmosDbHealthCheck>(name, HealthStatus.Unhealthy, tags, TimeSpan.FromSeconds(30));

        return builder;
    }
}

internal sealed record CosmosDbHealthCheckOptions(string Database, string[] Containers)
{
    public static CosmosDbHealthCheckOptions None = new("", Array.Empty<string>());
}

internal class CosmosDbHealthCheck : IHealthCheck
{
    private readonly CosmosClient _cosmosClient;
    private readonly CosmosDbHealthCheckOptions _options;

    /// <summary>
    /// Creates new instance of Azure Cosmos DB health check.
    /// </summary>
    /// <param name="cosmosClient">
    /// The <see cref="Microsoft.Azure.Cosmos.CosmosClient"/> used to perform Azure Cosmos DB operations.
    /// Azure SDK recommends treating clients as singletons <see href="https://devblogs.microsoft.com/azure-sdk/lifetime-management-and-thread-safety-guarantees-of-azure-sdk-net-clients/"/>,
    /// so this should be the exact same instance used by other parts of the application.
    /// </param>
    /// <param name="options">Optional settings used by the health check.</param>
    public CosmosDbHealthCheck(CosmosClient cosmosClient, CosmosDbHealthCheckOptions? options)
    {
        _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
        _options = options ?? CosmosDbHealthCheckOptions.None;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var data = GetData();

        try
        {
            await _cosmosClient.ReadAccountAsync().ConfigureAwait(false);
        }
        catch (CosmosException ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, description: $"Unable to connect to CosmosDB. Status code: {(int)ex.StatusCode}", ex, data: data);
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, description: "Unable to connect to CosmosDB.", ex, data: data);
        }

        if (!string.IsNullOrEmpty(_options.Database))
        {
            var database = _cosmosClient.GetDatabase(_options.Database);
            try
            {
                await database.ReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (CosmosException cosmosEx)
            {
                if (cosmosEx.StatusCode == HttpStatusCode.NotFound)
                {
                    return new HealthCheckResult(context.Registration.FailureStatus, description: $"Database {_options.Database} not found.", data: data);
                }

                return new HealthCheckResult(context.Registration.FailureStatus, description: $"Database {_options.Database} not accessible. Status code {cosmosEx.StatusCode}.", data: data);
            }

            if (_options.Containers.Length > 0)
            {
                List<string> notAvailable = new();

                foreach (var container in _options.Containers)
                {
                    try
                    {
                        await database
                            .GetContainer(container)
                            .ReadContainerAsync(cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        notAvailable.Add(container);
                    }
                }

                if (notAvailable.Count > 0)
                {
                    return new HealthCheckResult(context.Registration.FailureStatus, description: $"Failed to access containers: " + string.Join(", ", notAvailable), data: data);
                }
            }
        }

        return HealthCheckResult.Healthy(data: data);
    }

    private Dictionary<string, object> GetData()
    {
        var data = new Dictionary<string, object>
        {
            { "type", "CosmosDB" },
            { "uri", _cosmosClient.Endpoint.ToString() }
        };

        if (!string.IsNullOrWhiteSpace(_options.Database))
        {
            data.Add("database", _options.Database);
        }

        if (_options.Containers.Length > 0)
        {
            data.Add("containers", _options.Containers);
        }

        return data;
    }
}