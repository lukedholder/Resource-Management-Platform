using Scalar.AspNetCore;
using ResourcePlatform.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseHttpsRedirection();


app.MapGet("/", () => "Resource Management Platform API");

app.MapGet("/api/ping", () => new PingResponse("ok", DateTimeOffset.UtcNow))
    .WithName("Ping")
    .WithSummary("Liveness check.");

app.MapGet("/api/dev/sample", () => new Organization
{
    Name = "Nashville Medical Group",
    Slug = "nashville-medical"
});

app.Run();

public record PingResponse(string Status, DateTimeOffset TimestampUtc);