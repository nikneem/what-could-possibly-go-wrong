using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");
var cosmos = builder.AddAzureCosmosDB("cosmos")
    .WithHttpEndpoint(51234, 1234, "explorer-port")
    .WithExternalHttpEndpoints();
var cache = builder.AddRedis("cache")
    .WithRedisInsight();
var signalR = builder.AddAzureSignalR("signalr");

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

    signalR.RunAsEmulator();
}

var database = cosmos.AddCosmosDatabase("votr");
var container = database.AddContainer("surveys", "/id");
var tables = storage.AddTables("votes");

//builder.AddDapr();
//var options = new DaprSidecarOptions
//{
//    ResourcesPaths = ImmutableHashSet.Create(Directory.GetCurrentDirectory() + "/../../../../dapr/components")
//};

var surveysApi = builder.AddProject<Projects.Votr_Surveys_Api>("votr-surveys-api")
    .WaitFor(cosmos)
    .WaitFor(cache)
    .WaitFor(signalR)
    .WithReference(container)
    .WithReference(cache)
    .WithReference(signalR)
    .WithEnvironment("AzureServices:CosmosDbDatabase", "votr")
    .WithEnvironment("AzureServices:SurveysContainer", "surveys");

//    .WithDaprSidecar(options);

var votesApi = builder.AddProject<Projects.Votr_Votes_Api>("votr-votes-api")
    .WaitFor(storage)
    .WaitFor(cache)
    .WaitFor(signalR)
    .WithReference(tables)
    .WithReference(signalR)
    .WithReference(cache);
//    .WithDaprSidecar(options);

builder.AddProject<Projects.Votr_ReverseProxy_Api>("votr-reverseproxy-api")
    .WithReference(surveysApi)
    .WithReference(votesApi);

builder.Build().Run();


