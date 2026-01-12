using HackerNewsAPI.Configuration;
using HackerNewsAPI.Services;
using HackerNewsAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<HackerNewsOptions>()
    .Bind(builder.Configuration.GetSection("HackerNews"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl), "BaseUrl is required")
    .Validate(o => o.MaxConcurrency > 0, "MaxConcurrency must be greater than zero")
    .ValidateOnStart();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IHackerNewsClient, HackerNewsClient>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackerNews API v1");
    c.RoutePrefix = "swagger";
});

app.MapGet("/api/stories/best", async (int n, IHackerNewsClient client) =>
{
    var stories = await client.GetBestStoriesAsync(n);
    return Results.Ok(stories);
});

app.Run();