using HackerNewsApi.Interfaces;
using HackerNewsApi.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Caching.Memory;
using Scalar.AspNetCore;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHackerNewsApiClient, HackerNewsApiClient>();
builder.Services.AddHttpClient<IHackerNewsApiClient, HackerNewsApiClient>(client =>
{
    client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
});
var memoryCache = new MemoryCache(new MemoryCacheOptions());
builder.Services.AddFusionCache().WithDefaultEntryOptions(options => { options.Duration = TimeSpan.FromMinutes(10); }).WithMemoryCache(memoryCache).AsHybridCache();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
        context.ProblemDetails.Extensions.Add("nodeId", Environment.MachineName);
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // get ui under "/scalar/v1"
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();