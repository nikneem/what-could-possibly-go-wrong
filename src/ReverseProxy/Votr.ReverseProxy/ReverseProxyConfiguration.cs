using Votr.Core;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;

namespace Votr.ReverseProxy;

public class ReverseProxyConfiguration : IProxyConfigProvider
{

    private const string SurveysCluster = "surveysCluster";
    private const string VotesCluster = "votesCluster";

    public ReverseProxyConfiguration()
    {
        var routeConfigs = new[]
        {
            new RouteConfig
            {
                RouteId = "surveysRoute",
                ClusterId = SurveysCluster,
                Match = new RouteMatch
                {
                    Path = "/surveys/{**catch-all}"
                }
            },            new RouteConfig
            {
                RouteId = "votesRoute",
                ClusterId = VotesCluster,
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
                ClusterId = SurveysCluster,
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    {
                        "default", new DestinationConfig
                        {
                            Address = $"http://{ServiceName.SurveysApi}/api",
                            Health = $"http://{ServiceName.SurveysApi}/health",
                            Host = ServiceName.SurveysApi
                        }
                    }
                }
            },
            new ClusterConfig
            {
                ClusterId = VotesCluster,
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    {
                        "default", new DestinationConfig
                        {
                            Address = $"http://{ServiceName.VotesApi}/api",
                            Health = $"http://{ServiceName.VotesApi}/health",
                            Host = ServiceName.VotesApi
                        }
                    }
                }
            },

        };

        _config = new ReverseProxyMemoryConfig(routeConfigs, clusterConfigs);
    }

    private readonly ReverseProxyMemoryConfig _config;

    public IProxyConfig GetConfig() => _config;

}