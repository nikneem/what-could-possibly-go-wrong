using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var resourceGroupName = builder.AddParameter("AzureResourceGroupName");
var existingPubSubResourceName = builder.AddParameter("AzureWebPubSubResourceName");


var storage = builder.AddAzureStorage("storage");
var cosmos = builder.AddAzureCosmosDB("cosmos")
    .WithHttpEndpoint(51234, 1234, "explorer-port")
    .WithExternalHttpEndpoints();
var cache = builder.AddRedis("cache")
    .WithRedisInsight();
var webpubsub = builder
    .AddAzureWebPubSub("webpubsub")
    .RunAsExisting(existingPubSubResourceName, resourceGroupName);
    webpubsub.AddHub("votr");



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


var mainApi = builder.AddProject<Projects.Votr_Api>("mainApi")
    .WaitFor(cosmos)
    .WaitFor(cache)
    .WaitFor(webpubsub)
    .WaitFor(storage)
    .WithReference(container)
    .WithReference(cache)
    .WithReference(webpubsub)
    .WithReference(tables)
    .WithEnvironment("AzureServices:CosmosDbDatabase", "votr")
    .WithEnvironment("AzureServices:SurveysContainer", "surveys");


builder.AddProject<Projects.Votr_ReverseProxy_Api>("votr-reverseproxy-api")
    .WaitFor(mainApi)
    .WaitFor(mainApi)
    .WithReference(mainApi)
    .WithReference(mainApi);



builder.Build().Run();


