using Scalar.AspNetCore;
using ResourcePlatform.Domain;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Infrastructure;


// Registration
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


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