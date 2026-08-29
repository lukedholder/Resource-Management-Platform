using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Infrastructure;
using System.Text.Json.Serialization;
using ResourcePlatform.Web.Endpoints;
using Microsoft.AspNetCore.Identity;
using ResourcePlatform.Web.Services;


// Service Registration Phase (1): Describe what exists. Nothing runs. Nothing is created.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddValidation();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedAccount = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddSignInManager();

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));


var app = builder.Build();  // The pivot between phase 1 above and phase 2 below


// Request Pipeline Phase (2): Describe what happens when a request arrives.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Resource Management Platform API");

app.MapGet("/api/ping", () => new PingResponse("ok", DateTimeOffset.UtcNow))
    .WithName("Ping")
    .WithSummary("Liveness check.");

app.MapAuthEndpoints();
app.MapOrganizationEndpoints();
app.MapLocationEndpoints();
app.MapResourceTypeEndpoints();


app.Run();  // start listening, blocks forever


public record PingResponse(string Status, DateTimeOffset TimestampUtc);