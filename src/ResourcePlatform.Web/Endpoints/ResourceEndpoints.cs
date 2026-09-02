using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;
using ResourcePlatform.Web.Contracts;
using ResourcePlatform.Web.Services;

namespace ResourcePlatform.Web.Endpoints;


public static class ResourceEndpoints
{
    public static RouteGroupBuilder MapResourceEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/resources")
                            .WithTags("Resources")
                            .RequireAuthorization()
                            .RequireTenant();

        group.MapGet("/", GetAll)
            .WithName("ListResources").RequirePermission(Permissions.ResourceRead);

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetResource").RequirePermission(Permissions.ResourceRead);

        group.MapPost("/", Create)
            .WithName("CreateResource").RequirePermission(Permissions.ResourceCreate);

        group.MapPut("/{id:guid}", Update)
            .WithName("UpdateResource").RequirePermission(Permissions.ResourceUpdate);

        group.MapDelete("/{id:guid}", Delete)
            .WithName("DeleteResource").RequirePermission(Permissions.ResourceDelete);

        return group;
    }

    private static async Task<Ok<PagedResult<ResourceResponse>>> GetAll(
        AppDbContext db,
        CancellationToken ct,
        int? page = null,
        int? pageSize = null)
    {
        var (p, s) = PageDefaults.Clamp(page, pageSize);

        var query = db.Resources
            .AsNoTracking()
            .OrderBy(r => r.Name);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((p - 1) * s)
            .Take(s)
            .Select(Project())
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling(total / (double)s);
        return TypedResults.Ok(new PagedResult<ResourceResponse>(items, p, s, total, totalPages));
    }

    private static async Task<Results<Ok<ResourceResponse>, NotFound>> GetById(
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var resource = await db.Resources
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(Project())
            .FirstOrDefaultAsync(ct);

        return resource is null ? TypedResults.NotFound() : TypedResults.Ok(resource);
    }

    private static async Task<Results<Created<ResourceResponse>, ValidationProblem>> Create(
        CreateResourceRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var problems = await ValidateReferences(db, request.ResourceTypeId, request.LocationId, ct);
        if (problems is not null) return problems;

        var resource = new Resource
        {
            ResourceTypeId = request.ResourceTypeId,
            LocationId = request.LocationId,
            Name = request.Name,
            Description = request.Description,
            AssetTag = request.AssetTag,
            IsReservable = request.IsReservable,
            RequiresApproval = request.RequiresApproval
        };

        db.Resources.Add(resource);
        await db.SaveChangesAsync(ct);

        var response = new ResourceResponse(
            resource.Id, resource.OrganizationId, resource.ResourceTypeId, resource.LocationId,
            resource.DepartmentId, resource.Name, resource.Description, resource.AssetTag,
            resource.Status, resource.IsReservable, resource.RequiresApproval);

        return TypedResults.Created($"/api/resources/{resource.Id}", response);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> Update(
        Guid id,
        UpdateResourceRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var resource = await db.Resources.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (resource is null) return TypedResults.NotFound();

        var problems = await ValidateReferences(db, request.ResourceTypeId, request.LocationId, ct);
        if (problems is not null) return problems;

        resource.ResourceTypeId = request.ResourceTypeId;
        resource.LocationId = request.LocationId;
        resource.Name = request.Name;
        resource.Description = request.Description;
        resource.AssetTag = request.AssetTag;
        resource.Status = request.Status;
        resource.IsReservable = request.IsReservable;
        resource.RequiresApproval = request.RequiresApproval;
        resource.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, Conflict<string>>> Delete(
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var resource = await db.Resources.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (resource is null) return TypedResults.NotFound();

        // Check if there are any reservations for this resource
        var reservations = await db.Reservations.CountAsync(r => r.ResourceId == id, ct);
        if (reservations > 0)
            return TypedResults.Conflict($"This resource has {reservations} reservation(s) and cannot be deleted.");

        db.Resources.Remove(resource);
        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    private static System.Linq.Expressions.Expression<Func<Resource, ResourceResponse>> Project() =>
        r => new ResourceResponse(
            r.Id, r.OrganizationId, r.ResourceTypeId, r.LocationId, r.DepartmentId,
            r.Name, r.Description, r.AssetTag, r.Status, r.IsReservable, r.RequiresApproval);

    private static async Task<ValidationProblem?> ValidateReferences(
        AppDbContext db,
        Guid resourceTypeId,
        Guid? locationId,
        CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>();

        if (!await db.ResourceTypes.AnyAsync(rt => rt.Id == resourceTypeId, ct))
            errors["ResourceTypeId"] = ["No resource type with that id exists in this organization."];

        if (locationId is not null && !await db.Locations.AnyAsync(l => l.Id == locationId, ct))
            errors["LocationId"] = ["No location with that id exists in this organization."];

        return errors.Count > 0 ? TypedResults.ValidationProblem(errors) : null;
    }
}