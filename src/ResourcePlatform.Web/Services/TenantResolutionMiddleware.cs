using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;

namespace ResourcePlatform.Web.Services;


public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Organization-Id";
    public const string CookieName = "rp_org";

    public async Task InvokeAsync(
        HttpContext context,
        AppDbContext db,
        ITenantContextSetter setter,
        ICurrentUser currentUser)
    {
        var userId = currentUser.UserId;

        if (userId is not null
            && TryReadOrganizationId(context, out var organizationId))
        {
            // IgnoreQueryFilters: this is the one query that must look across tenants,
            // because it is the query that decides which tenant you are in
            var isMember = await db.Memberships
                .IgnoreQueryFilters()
                .AnyAsync(m => m.OrganizationId == organizationId
                            && m.UserId == userId.Value
                            && m.Status == MembershipStatus.Active);

            if (isMember)
                setter.Set(organizationId);
        }

        await next(context);
    }

    /// <summary>
    /// API clients send a header. The Blazor UI cannot set one on a navigation,
    /// so it uses a cookie instead. Either way membership is rechecked above.
    /// </summary>
    private static bool TryReadOrganizationId(HttpContext context, out Guid organizationId)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var raw)
            && Guid.TryParse(raw, out organizationId))
            return true;

        if (context.Request.Cookies.TryGetValue(CookieName, out var cookie)
            && Guid.TryParse(cookie, out organizationId))
            return true;

        organizationId = Guid.Empty;
        return false;
    }
}

public sealed class RequireTenantFilter(ITenantContext tenant) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (!tenant.IsResolved)
        {
            return TypedResults.Problem(
                title: "Organization context required",
                detail: $"Supply a valid '{TenantResolutionMiddleware.HeaderName}' header for an organization you belong to.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        return await next(context);
    }
}

public static class TenantExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app) =>
        app.UseMiddleware<TenantResolutionMiddleware>();

    public static TBuilder RequireTenant<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder =>
        builder.AddEndpointFilter<TBuilder, RequireTenantFilter>();
}