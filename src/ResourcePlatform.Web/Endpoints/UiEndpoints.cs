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

        http.Response.Cookies.Append(TenantResolutionMiddleware.CookieName, organizationId.ToString(),
            new CookieOptions
            {
                HttpOnly = true,    // no JavaScript
                Secure = true,
                SameSite = SameSiteMode.Lax,    // block cross-site sends
                IsEssential = true,
                MaxAge = TimeSpan.FromDays(30)
            });

        return Results.Redirect("/dashboard");
    }
}