using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;
using ResourcePlatform.Web.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ResourcePlatform.Web.Endpoints;


public static class ResourceTypeEndpoints
{
    public static RouteGroupBuilder MapResourceTypeEndpoints(this IEndpointRouteBuilder routes)
    {
        // TEMP: {orgId} comes from the client.
        var group = routes.MapGroup("/api/organizations/{orgId:guid}/resource-types").WithTags("ResourceTypes");

        group.MapGet("/", GetAll).WithName("ListResourceTypes");
        group.MapGet("/{id:guid}", GetById).WithName("GetResourceType");
        group.MapPost("/", Create).WithName("CreateResourceType").RequireAuthorization();
        group.MapPut("/{id:guid}", Update).WithName("UpdateResourceType").RequireAuthorization();
        group.MapDelete("/{id:guid}", Delete).WithName("DeleteResourceType").RequireAuthorization();

        return group;
    }

    private static async Task<Results<Ok<PagedResult<ResourceTypeResponse>>, NotFound>> GetAll(
        Guid orgId,
        AppDbContext db,
        CancellationToken ct,
        int? page = null,
        int? pageSize = null)
    {
        if (!await db.Organizations.AnyAsync(o => o.Id == orgId, ct))
            return TypedResults.NotFound();

        var (p, s) = PageDefaults.Clamp(page, pageSize);

        var query = db.ResourceTypes
            .AsNoTracking()
            .Where(rt => rt.OrganizationId == orgId)
            .OrderBy(rt => rt.Name);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((p - 1) * s)
            .Take(s)
            .Select(rt => new ResourceTypeResponse(rt.Id, rt.OrganizationId, rt.Name, rt.Description))
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling(total / (double)s);
        return TypedResults.Ok(new PagedResult<ResourceTypeResponse>(items, p, s, total, totalPages));
    }

    private static async Task<Results<Ok<ResourceTypeResponse>, NotFound>> GetById(
        Guid orgId,
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var resourceType = await db.ResourceTypes
            .AsNoTracking()
            .Where(rt => rt.OrganizationId == orgId && rt.Id == id)
            .Select(rt => new ResourceTypeResponse(rt.Id, rt.OrganizationId, rt.Name, rt.Description))
            .FirstOrDefaultAsync(ct);

        return resourceType is null ? TypedResults.NotFound() : TypedResults.Ok(resourceType);
    }

    private static async Task<Results<Created<ResourceTypeResponse>, NotFound, ValidationProblem>> Create(
        Guid orgId,
        CreateResourceTypeRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        if (!await db.Organizations.AnyAsync(o => o.Id == orgId, ct))
            return TypedResults.NotFound(); // orgId not found

        var resourceType = new ResourceType
        {
            OrganizationId = orgId,
            Name = request.Name,
            Description = request.Description
        };

        db.ResourceTypes.Add(resourceType);
        await db.SaveChangesAsync(ct);

        var response = new ResourceTypeResponse(
            resourceType.Id, resourceType.OrganizationId,
            resourceType.Name, resourceType.Description);

        return TypedResults.Created($"/api/organizations/{orgId}/resource-types/{resourceType.Id}", response);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> Update(
        Guid orgId,
        Guid id,
        UpdateResourceTypeRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var resourceType = await db.ResourceTypes.FirstOrDefaultAsync(rt => rt.OrganizationId == orgId && rt.Id == id, ct);

        if (resourceType is null) return TypedResults.NotFound();

        resourceType.Name = request.Name;
        resourceType.Description = request.Description;

        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> Delete(
        Guid orgId,
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var resourceType = await db.ResourceTypes.FirstOrDefaultAsync(rt => rt.OrganizationId == orgId && rt.Id == id, ct);

        if (resourceType is null) return TypedResults.NotFound();

        // layer 1: pre-check, so the common case gets a precise, helpful answer
        var inUse = await db.Resources.CountAsync(r => r.ResourceTypeId == id, ct);
        if (inUse > 0) return InUseConflict(inUse);

        db.ResourceTypes.Remove(resourceType);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
        {
            // layer 2: lost a race. a Resource was created after the above count
            return InUseConflict(null);
        }

        return TypedResults.NoContent();
    }

    private static Conflict<ProblemDetails> InUseConflict(int? count) =>
        TypedResults.Conflict(new ProblemDetails
        {
            Title = "Resource type is in use",
            Detail = count is null
                    ? "This resource type is referenced by one or more resources and cannot be deleted."
                    : $"This resource type is referenced by {count} resource(s) and cannot be deleted.",
            Status = StatusCodes.Status409Conflict
        });

    private static bool IsForeignKeyViolation(DbUpdateException ex) =>
        ex.InnerException is SqlException { Number: 547 };  // SQL Server's FK-violation number
}