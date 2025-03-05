using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");
var cosmos = builder.AddAzureCosmosDB("cosmos")
    .WithHttpEndpoint(51234, 1234, "explorer-port")
    .WithExternalHttpEndpoints();
var cache = builder.AddRedis("cache")
    .WithRedisInsight();
var webpubsub = builder.AddAzureWebPubSub("webpubsub");
    

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


var surveysApi = builder.AddProject<Projects.Votr_Surveys_Api>("votr-surveys-api")
    .WaitFor(cosmos)
    .WaitFor(cache)
    .WaitFor(webpubsub)
    .WithReference(container)
    .WithReference(cache)
    .WithReference(webpubsub)
    .WithEnvironment("AzureServices:CosmosDbDatabase", "votr")
    .WithEnvironment("AzureServices:SurveysContainer", "surveys");

var votesApi = builder.AddProject<Projects.Votr_Votes_Api>("votr-votes-api")
    .WaitFor(storage)
    .WaitFor(cache)
    .WaitFor(webpubsub)
    .WithReference(tables)
    .WithReference(webpubsub)
    .WithReference(cache);

builder.AddProject<Projects.Votr_ReverseProxy_Api>("votr-reverseproxy-api")
    .WaitFor(surveysApi)
    .WaitFor(votesApi)
    .WithReference(surveysApi)
    .WithReference(votesApi);

builder.Build().Run();


