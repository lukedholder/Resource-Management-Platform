using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;

namespace ResourcePlatform.Web.Services;


public interface IPermissionService
{
    Task<bool> HasAsync(string permission, CancellationToken ct = default);
    Task<IReadOnlySet<string>> GetAllAsync(CancellationToken ct = default);
}

/// <summary>
/// Resolves the current user's permissions inside the current tenant.
/// Scoped, so the database is hit at most once per request.
/// </summary>
public sealed class PermissionService(
    AppDbContext db,
    ICurrentUser currentUser,
    ITenantContext tenant) : IPermissionService
{
    private static readonly HashSet<string> None = [];
    private HashSet<string>? _cache;

    public async Task<bool> HasAsync(string permission, CancellationToken ct = default) =>
        (await GetAllAsync(ct)).Contains(permission);

    public async Task<IReadOnlySet<string>> GetAllAsync(CancellationToken ct = default)
    {
        if (_cache is not null) return _cache;

        if (!tenant.IsResolved || currentUser.UserId is null)
            return _cache = None;

        var names = await db.Memberships
            .AsNoTracking()
            .Where(m => m.UserId == currentUser.UserId.Value
                    && m.Status == MembershipStatus.Active)
            .SelectMany(m => m.Role!.RolePermissions.Select(rp => rp.Permission!.Name))
            .ToListAsync(ct);

        return _cache = names.ToHashSet(StringComparer.Ordinal);
    }
}

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}

public sealed class PermissionHandler(IPermissionService permissions) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (await permissions.HasAsync(requirement.Permission))
            context.Succeed(requirement);
    }
}

/// <summary>
/// Builds a policy on demand for any name of the form "perm:Some.Permission",
/// so permissions never have to be pre-registered one by one.
/// </summary>
public sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    public const string Prefix = "perm:";

    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(Prefix, StringComparison.Ordinal))
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(policyName[Prefix.Length..]))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return base.GetPolicyAsync(policyName);
    }
}

public static class PermissionExtensions
{
    public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, string permission)
        where TBuilder : IEndpointConventionBuilder =>
            builder.RequireAuthorization(PermissionPolicyProvider.Prefix + permission);
}