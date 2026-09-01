using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;
using ResourcePlatform.Web.Contracts;
using ResourcePlatform.Web.Services;

namespace ResourcePlatform.Web.Endpoints;


public static class MemberEndpoitns
{
    public static RouteGroupBuilder MapMemberEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/members")
                            .WithTags("Members")
                            .RequireAuthorization()
                            .RequireTenant();

        group.MapGet("/", GetAll).WithName("ListMembers")
             .RequirePermission(Permissions.MemberRead);

        group.MapPost("/", Add).WithName("AddMember")
             .RequirePermission(Permissions.MemberInvite);

        group.MapPut("/{userId:guid}/role", ChangeRole).WithName("ChangeMemberRole")
             .RequirePermission(Permissions.MemberAssignRole);

        group.MapDelete("/{userId:guid}", Remove).WithName("RemoveMember")
             .RequirePermission(Permissions.MemberRemove);

        return group;
    }

    private static async Task<Ok<List<MemberResponse>>> GetAll(
        AppDbContext db,
        CancellationToken ct)
    {
        var members = await db.Memberships
            .AsNoTracking()
            .OrderBy(m => m.JoinedAt)
            .Select(m => new MemberResponse(
                m.UserId,
                db.Users.Where(u => u.Id == m.UserId).Select(u => u.Email!).FirstOrDefault()!,
                db.Users.Where(u => u.Id == m.UserId).Select(u => u.DisplayName).FirstOrDefault()!,
                m.Role!.Name,
                m.Status.ToString(),
                m.JoinedAt))
            .ToListAsync(ct);

        return TypedResults.Ok(members);
    }

    private static async Task<Results<Created<MemberResponse>, NotFound, ValidationProblem, Conflict<string>>> Add(
        AddMemberRequest request,
        AppDbContext db,
        ITenantContext tenant,
        CancellationToken ct)
    {
        var role = await FindSystemRole(db, request.Role, ct);
        if (role is null) return RoleProblem(request.Role);

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        if (user is null) return TypedResults.NotFound();

        if (await db.Memberships.AnyAsync(m => m.UserId == user.Id, ct))
            return TypedResults.Conflict("That user is already a member of this organization.");

        var membership = new OrganizationMembership
        {
            OrganizationId = tenant.OrganizationId,
            UserId = user.Id,
            RoleId = role.Id,
            Status = MembershipStatus.Active
        };

        db.Memberships.Add(membership);
        await db.SaveChangesAsync(ct);

        var response = new MemberResponse(
            user.Id, user.Email!, user.DisplayName, role.Name,
            membership.Status.ToString(), membership.JoinedAt);

        return TypedResults.Created($"/api/members/{user.Id}", response);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> ChangeRole(
        Guid userId,
        UpdateMemberRoleRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var role = await FindSystemRole(db, request.Role, ct);
        if (role is null) return RoleProblem(request.Role);

        var membership = await db.Memberships.FirstOrDefaultAsync(m => m.UserId == userId, ct);
        if (membership is null) return TypedResults.NotFound();

        membership.RoleId = role.Id;
        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> Remove(
        Guid userId,
        AppDbContext db,
        CancellationToken ct)
    {
        var membership = await db.Memberships.FirstOrDefaultAsync(m => m.UserId == userId, ct);
        if (membership is null) return TypedResults.NotFound();

        db.Memberships.Remove(membership);
        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    private static Task<Role?> FindSystemRole(AppDbContext db, string name, CancellationToken ct) =>
        db.Roles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.IsSystemRole && r.OrganizationId == null && r.Name == name, ct);

    private static ValidationProblem RoleProblem(string name) =>
        TypedResults.ValidationProblem(new Dictionary<string, string[]>
        {
            ["Role"] = [$"'{name}' is not a known role. Valid roles: Owner, Administrator, Manager, Member, Viewer."]
        });
}