using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;
using ResourcePlatform.Web.Services;

namespace ResourcePlatform.Web.Endpoints;


/// <summary>
/// Form-post endpoints for the Blazor UI. They do the work, set cookies, and
/// redirect; things a statically rendered component cannot do after the
/// response has started.
/// </summary>
public static class UiEndpoints
{
    public static RouteGroupBuilder MapUiEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/ui").ExcludeFromDescription();

        group.MapPost("/login", Login);
        group.MapPost("/register", Register);
        group.MapPost("/logout", Logout).RequireAuthorization();
        group.MapPost("/select-org", SelectOrganization).RequireAuthorization();
        group.MapPost("/organizations/create", CreateOrganization).RequireAuthorization();

        var tenant = group.MapGroup("").RequireAuthorization().RequireTenant();

        tenant.MapPost("/locations/create", CreateLocation).RequirePermission(Permissions.LocationCreate);
        tenant.MapPost("/locations/delete", DeleteLocation).RequirePermission(Permissions.LocationDelete);
        tenant.MapPost("/resource-types/create", CreateResourceType).RequirePermission(Permissions.ResourceTypeCreate);
        tenant.MapPost("/resources/create", CreateResource).RequirePermission(Permissions.ResourceCreate);
        tenant.MapPost("/reservations/create", CreateReservation).RequirePermission(Permissions.ReservationCreate);

        return group;
    }

    private static async Task<IResult> Login(
        [FromForm] string email,
        [FromForm] string password,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Results.Redirect("/login?error=1");

        var result = await signInManager.PasswordSignInAsync(user, password, isPersistent: true, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Results.Redirect("/login?error=1");

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await userManager.UpdateAsync(user);

        return Results.Redirect("/organizations");
    }

    private static async Task<IResult> Register(
        [FromForm] string email,
        [FromForm] string displayName,
        [FromForm] string password,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        var user = new ApplicationUser { UserName = email, Email = email, DisplayName = displayName };
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var message = Uri.EscapeDataString(string.Join(" ", result.Errors.Select(e => e.Description)));
            return Results.Redirect($"/register?error={message}");
        }

        await signInManager.SignInAsync(user, isPersistent: true);
        return Results.Redirect("/organizations");
    }

    private static async Task<IResult> Logout(
        SignInManager<ApplicationUser> signInManager,
        HttpContext http)
    {
        await signInManager.SignOutAsync();
        http.Response.Cookies.Delete(TenantResolutionMiddleware.CookieName);
        return Results.Redirect("/login");
    }

    private static async Task<IResult> SelectOrganization(
        [FromForm] Guid organizationId,
        AppDbContext db,
        ICurrentUser currentUser,
        HttpContext http)
    {
        // Never trust the posted id: confirm membership before writing the cookie
        var isMember = await db.Memberships
            .IgnoreQueryFilters()
            .AnyAsync(m => m.OrganizationId == organizationId
                        && m.UserId == currentUser.UserId!.Value
                        && m.Status == MembershipStatus.Active);

        if (!isMember) return Results.Redirect("/organizations?error=1");

        WriteTenantCookie(http, organizationId);
        return Results.Redirect("/dashboard");
    }

    private static async Task<IResult> CreateOrganization(
        [FromForm] string name,
        [FromForm] string? slug,
        OrganizationService organizations,
        HttpContext http,
        CancellationToken ct)
    {
        // The slug box is optional in the UI; derive one from the name when it is left blank.
        var finalSlug = string.IsNullOrWhiteSpace(slug) ? OrganizationRules.SuggestSlug(name ?? "") : slug;

        var result = await organizations.CreateAsync(name ?? "", finalSlug, ct);

        if (result.Outcome != OrganizationOutcome.Created)
            return Back("/organizations", result.Error!);

        // The creator is now the Owner, so open the new organization straight away.
        WriteTenantCookie(http, result.Organization!.Id);
        return Results.Redirect("/dashboard");
    }

    private static void WriteTenantCookie(HttpContext http, Guid organizationId) =>
        http.Response.Cookies.Append(TenantResolutionMiddleware.CookieName, organizationId.ToString(),
            new CookieOptions
            {
                HttpOnly = true,    // no JavaScript
                Secure = true,
                SameSite = SameSiteMode.Lax,    // block cross-site sends
                IsEssential = true,
                MaxAge = TimeSpan.FromDays(30)
            });


    private static async Task<IResult> CreateLocation(
        [FromForm] string name,
        [FromForm] string timeZoneId,
        [FromForm] string? city,
        [FromForm] string? state,
        AppDbContext db)
    {
        if (!TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out _))
            return Back("/locations", $"'{timeZoneId}' is not a recognized time zone.");

        db.Locations.Add(new Location
        {
            Name = name,
            TimeZoneId = timeZoneId,
            City = string.IsNullOrWhiteSpace(city) ? null : city,
            State = string.IsNullOrWhiteSpace(state) ? null : state
        });

        await db.SaveChangesAsync();
        return Results.Redirect("/locations");
    }

    private static async Task<IResult> DeleteLocation(
        [FromForm] Guid id,
        AppDbContext db)
    {
        var location = await db.Locations.FirstOrDefaultAsync(l => l.Id == id);
        if (location is null) return Results.Redirect("/locations");

        var inUse = await db.Resources.CountAsync(r => r.LocationId == id);
        if (inUse > 0)
            return Back("/locations", $"That location is used by {inUse} resource(s) and cannot be deleted.");

        db.Locations.Remove(location);
        await db.SaveChangesAsync();
        return Results.Redirect("/locations");
    }

    private static async Task<IResult> CreateResourceType(
        [FromForm] string name,
        [FromForm] string? description,
        AppDbContext db)
    {
        db.ResourceTypes.Add(new ResourceType
        {
            Name = name,
            Description = string.IsNullOrWhiteSpace(description) ? null : description
        });

        await db.SaveChangesAsync();
        return Results.Redirect("/resources");
    }

    private static async Task<IResult> CreateResource(
        [FromForm] string name,
        [FromForm] Guid resourceTypeId,
        [FromForm] Guid? locationId,
        [FromForm] string? assetTag,
        [FromForm] bool? requiresApproval,
        AppDbContext db)
    {
        if (!await db.ResourceTypes.AnyAsync(t => t.Id == resourceTypeId))
            return Back("/resources", "Pick a resource type.");

        // Guid.Empty is the "(no location)" option; a real id must exist in this tenant.
        if (locationId is not null && locationId != Guid.Empty
            && !await db.Locations.AnyAsync(l => l.Id == locationId))
            return Back("/resources", "That location does not exist.");

        db.Resources.Add(new Resource
        {
            Name = name,
            ResourceTypeId = resourceTypeId,
            LocationId = locationId == Guid.Empty ? null : locationId,
            AssetTag = string.IsNullOrWhiteSpace(assetTag) ? null : assetTag,

            // An unchecked checkbox is not sent at all, so absent means false.
            RequiresApproval = requiresApproval ?? false
        });

        await db.SaveChangesAsync();
        return Results.Redirect("/resources");
    }

    private static async Task<IResult> CreateReservation(
        [FromForm] Guid resourceId,
        [FromForm] DateTime startLocal,
        [FromForm] DateTime endLocal,
        [FromForm] string? purpose,
        BookingService booking,
        CancellationToken ct)
    {
        // Datetime-local inputs have no timezone; the UI treats them as UTC for now.
        var start = new DateTimeOffset(DateTime.SpecifyKind(startLocal, DateTimeKind.Utc));
        var end = new DateTimeOffset(DateTime.SpecifyKind(endLocal, DateTimeKind.Utc));

        var result = await booking.BookAsync(resourceId, start, end, purpose, ct);

        return result.Outcome switch
        {
            BookingOutcome.Created => Results.Redirect("/reservations"),
            BookingOutcome.ResourceNotFound => Back("/reservations", "That resource no longer exists."),
            _ => Back("/reservations", result.Error!)
        };
    }

    private static IResult Back(string path, string error) =>
        Results.Redirect($"{path}?error={Uri.EscapeDataString(error)}");
}