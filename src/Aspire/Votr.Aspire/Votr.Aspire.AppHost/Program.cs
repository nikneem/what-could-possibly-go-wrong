using System.Collections.Immutable;
using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDapr();
var options = new DaprSidecarOptions
{
    ResourcesPaths = ImmutableHashSet.Create(Directory.GetCurrentDirectory() + "/../../../../dapr/components")
};

var surveysApi = builder.AddProject<Projects.Votr_Surveys_Api>("votr-surveys-api")
    .WithDaprSidecar(options);

var votesApi = builder.AddProject<Projects.Votr_Votes_Api>("votr-votes-api")
    .WithDaprSidecar(options);

builder.AddProject<Projects.Votr_ReverseProxy_Api>("votr-reverseproxy-api")
    .WithReference(surveysApi)
    .WithReference(votesApi);

builder.Build().Run();
