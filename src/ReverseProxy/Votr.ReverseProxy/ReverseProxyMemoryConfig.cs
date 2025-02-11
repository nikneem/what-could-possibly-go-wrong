using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Votr.ReverseProxy;

internal class ReverseProxyMemoryConfig : IProxyConfig
    {
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public ReverseProxyMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        Routes = routes;
        Clusters = clusters;
        ChangeToken = new CancellationChangeToken(_cts.Token);
    }

    public IReadOnlyList<RouteConfig> Routes { get; }

    public IReadOnlyList<ClusterConfig> Clusters { get; }

    public IChangeToken ChangeToken { get; }

    internal void SignalChange()
    {
        _cts.Cancel();
    }
}