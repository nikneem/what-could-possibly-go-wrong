using Votr.Core;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;

namespace Votr.ReverseProxy;

public class ReverseProxyConfiguration : IProxyConfigProvider
{

    private const string MainApiCluster = "surveysCluster";

    public ReverseProxyConfiguration()
    {
        var routeConfigs = new[]
        {
            new RouteConfig
            {
                RouteId = "surveysRoute",
                ClusterId = MainApiCluster,
                Match = new RouteMatch
                {
                    Path = "/surveys/{**catch-all}"
                }
            },            new RouteConfig
            {
                RouteId = "votesRoute",
                ClusterId = MainApiCluster,
                Match = new RouteMatch
                {
                    Path = "/votes/{**catch-all}"
                }
            },

        };

        var clusterConfigs = new[]
        {
            new ClusterConfig
            {
                ClusterId = MainApiCluster,
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    {
                        "default", new DestinationConfig
                        {
                            Address = $"http://{ServiceName.VotrApi}/api",
                            Health = $"http://{ServiceName.VotrApi}/health",
                            Host = ServiceName.VotrApi
                        }
                    }
                }
            }
        };

        _config = new ReverseProxyMemoryConfig(routeConfigs, clusterConfigs);
    }

    private readonly ReverseProxyMemoryConfig _config;

    public IProxyConfig GetConfig() => _config;

}