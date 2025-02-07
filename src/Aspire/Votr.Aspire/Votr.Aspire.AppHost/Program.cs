var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Votr_Votes_Api>("votr-votes-api");

builder.AddProject<Projects.Votr_Surveys_Api>("votr-surveys-api");

builder.Build().Run();
