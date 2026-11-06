using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using OrderManagement.Api.Middlewares;
using OrderManagement.Infrastructure;

var builder = WebApplication.CreateSlimBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

builder.AddServiceDefaults();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();
app.UseCors(myAllowSpecificOrigins);
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add test endpoints for demonstration purposes
app.MapGet("/api/v1/test", () => Results.Ok($"Hello World! {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}"))
    .WithName("GetRootTest");
app.MapGet("/api/v1/test/{id}", (int id) => TypedResults.Ok($"Hello World! {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {id}"))
    .WithName("GetRootTestWithId");
app.MapGet("/api/v1/latency-test/{latencyDurationinMs}", 
    async (ILogger<Program> logger, 
    int latencyDurationinMs = 1000, 
    CancellationToken cancellationToken = default) =>
{
    logger.LogInformation("Latency test endpoint called at {Time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    await Task.Delay(latencyDurationinMs, cancellationToken);
    logger.LogInformation("Latency test endpoint completed at {Time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    return TypedResults.Ok($"End of latency request at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
});
app.MapGet("/api/v1/error-test", (ILogger<Program> logger) =>
{
    logger.LogError("Error test endpoint called at {Time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    return TypedResults.Problem("This is a test error response", statusCode: 500);
});


app.Run();

[JsonSerializable(typeof(object[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
