using Votr.ReverseProxy;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var myCorsPolicyName = "allow-dev"; 
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(false);

// Add services to the container.
var proxyBuilder = builder.Services
    .AddSingleton<IProxyConfigProvider, ReverseProxyConfiguration>()
    .AddReverseProxy()
    .AddTransforms(transformBuilderContext =>
    {
        transformBuilderContext.AddRequestTransform(context =>
        {
            Console.WriteLine(context.ProxyRequest.RequestUri?.AbsoluteUri);

            return ValueTask.CompletedTask;
        });
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy(myCorsPolicyName, bldr =>
    {
        bldr.WithOrigins( "http://localhost:4200", "http://localhost:53824")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

if (builder.Environment.IsDevelopment())
{
    proxyBuilder.AddServiceDiscoveryDestinationResolver();
    //builder.Services.AddHttpForwarderWithServiceDiscovery();
}

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapDefaultEndpoints(myCorsPolicyName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapReverseProxy();
app.Run();

