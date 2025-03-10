using Votr.Surveys.ExtensionMethods;
using Votr.Votes.Data.TableStorage.ExtensionMethods;
using Votr.Votes.ExtensionMethods;
using Votr.Surveys.Data.CosmosDb.ExtensionMethods;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults()
    .AddSurveysApi()
    .AddVotesApi()
    .WithCosmosDbRepository()
    .WithTableStorage()
    .AddAzureTableClient("votes");


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
