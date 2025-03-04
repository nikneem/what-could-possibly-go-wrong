using System.Collections.Immutable;
using CommunityToolkit.Aspire.Hosting.Dapr;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");
var cosmos = builder.AddAzureCosmosDB("cosmos");



if (builder.Environment.IsDevelopment())
{
    storage.RunAsEmulator(options =>
    {
        options.WithLifetime(ContainerLifetime.Persistent);
    });
#pragma warning disable ASPIRECOSMOSDB001
    cosmos.RunAsPreviewEmulator(options =>
    {
        options.WithLifetime(ContainerLifetime.Persistent);
    });
#pragma warning restore ASPIRECOSMOSDB001

}

var database = cosmos.AddCosmosDatabase("votr");
var container = database.AddContainer("surveys", "/id");
var tables = storage.AddTables("votes");

builder.AddDapr();
var options = new DaprSidecarOptions
{
    ResourcesPaths = ImmutableHashSet.Create(Directory.GetCurrentDirectory() + "/../../../../dapr/components")
};

var surveysApi = builder.AddProject<Projects.Votr_Surveys_Api>("votr-surveys-api")
    .WaitFor(cosmos)
    .WithReference(database)
    .WithDaprSidecar(options);

var votesApi = builder.AddProject<Projects.Votr_Votes_Api>("votr-votes-api")
    .WithReference(tables)
    .WithDaprSidecar(options);

builder.AddProject<Projects.Votr_ReverseProxy_Api>("votr-reverseproxy-api")
    .WithReference(surveysApi)
    .WithReference(votesApi);

builder.Build().Run();


