using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Infrastructure;
using System.Text.Json.Serialization;
using ResourcePlatform.Web.Endpoints;
using Microsoft.AspNetCore.Identity;
using ResourcePlatform.Web.Services;
using Microsoft.AspNetCore.Authorization;
using ResourcePlatform.Web.Components;


// Service Registration Phase (1): Describe what exists. Nothing runs. Nothing is created.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

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

builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/login";
    o.LogoutPath = "/ui/logout";
    o.AccessDeniedPath = "/denied";
});
builder.Services.AddAuthorization();

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// One TenantContext instanc per request, exposed through two interfaces:
// the middleware writes it, everything else only reads it
builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ITenantContext>(sp =>
    sp.GetRequiredService<TenantContext>());
builder.Services.AddScoped<ITenantContextSetter>(sp =>
    sp.GetRequiredService<TenantContext>());

builder.Services.AddScoped<BookingService>();

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));


var app = builder.Build();  // The pivot between phase 1 above and phase 2 below


// Request Pipeline Phase (2): Describe what happens when a request arrives.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();  //enable exception handler first so everything that follows can use it
app.UseHttpsRedirection();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseTenantResolution();  // needs the authenticated user, so it runs after authentication
app.UseAuthorization();     // needs the resolved tenant to know which membership to look up, so it runs after Tenant Resolution

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapUiEndpoints();

app.MapGet("/api/ping", () => new PingResponse("ok", DateTimeOffset.UtcNow))
    .WithName("Ping")
    .WithSummary("Liveness check.");

app.MapAuthEndpoints();
app.MapOrganizationEndpoints();
app.MapLocationEndpoints();
app.MapResourceTypeEndpoints();
app.MapMemberEndpoints();
app.MapResourceEndpoints();
app.MapReservationEndpoints();


app.Run();  // start listening, blocks forever


public record PingResponse(string Status, DateTimeOffset TimestampUtc);


public partial class Program { }    // Make Program visible