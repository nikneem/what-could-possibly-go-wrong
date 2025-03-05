using Votr.Surveys.Data.CosmosDb.ExtensionMethods;
using Votr.Surveys.ExtensionMethods;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults()
    .AddSurveysApi()
    .WithCosmosDbRepository();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

//app.MapHub<VotrHub>("/api/hubs/votr");

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
