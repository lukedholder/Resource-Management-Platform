using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;
using ResourcePlatform.Web.Contracts;

namespace ResourcePlatform.Web.Endpoints;


public static class LocationEndpoints
{
    public static RouteGroupBuilder MapLocationEndpoints(this IEndpointRouteBuilder routes)
    {
        // TEMP: {orgId} comes from the client.
        var group = routes.MapGroup("/api/organizations/{orgId:guid}/locations").WithTags("Locations");

        group.MapGet("/", GetAll).WithName("ListLocations");
        group.MapGet("/{id:guid}", GetById).WithName("GetLocation");
        group.MapPost("/", Create).WithName("CreateLocation").RequireAuthorization();
        group.MapPut("/{id:guid}", Update).WithName("UpdateLocation").RequireAuthorization();
        group.MapDelete("/{id:guid}", Delete).WithName("DeleteLocation").RequireAuthorization();

        return group;
    }

    private static async Task<Results<Ok<PagedResult<LocationResponse>>, NotFound>> GetAll(
        Guid orgId,
        AppDbContext db,
        CancellationToken ct,
        int? page = null,
        int? pageSize = null)
    {
        if (!await db.Organizations.AnyAsync(o => o.Id == orgId, ct))
            return TypedResults.NotFound();

        var (p, s) = PageDefaults.Clamp(page, pageSize);

        var query = db.Locations
            .AsNoTracking()
            .Where(l => l.OrganizationId == orgId)
            .OrderBy(l => l.Name);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((p - 1) * s)
            .Take(s)
            .Select(l => new LocationResponse(l.Id, l.OrganizationId, l.Name, l.AddressLine1, l.City, l.State, l.PostalCode, l.TimeZoneId))
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling(total / (double)s);
        return TypedResults.Ok(new PagedResult<LocationResponse>(items, p, s, total, totalPages));
    }

    private static async Task<Results<Ok<LocationResponse>, NotFound>> GetById(
        Guid orgId,
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var location = await db.Locations
            .AsNoTracking()
            .Where(l => l.OrganizationId == orgId && l.Id == id)
            .Select(l => new LocationResponse(l.Id, l.OrganizationId, l.Name, l.AddressLine1, l.City, l.State, l.PostalCode, l.TimeZoneId))
            .FirstOrDefaultAsync(ct);

        return location is null ? TypedResults.NotFound() : TypedResults.Ok(location);
    }

    private static async Task<Results<Created<LocationResponse>, NotFound, ValidationProblem>> Create(
        Guid orgId,
        CreateLocationRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        if (!await db.Organizations.AnyAsync(o => o.Id == orgId, ct))
            return TypedResults.NotFound();

        if (!IsKnownTimeZone(request.TimeZoneId))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.TimeZoneId)] = [$"'{request.TimeZoneId}' is not a recognized time zone id."]
            });
        }

        var location = new Location
        {
            OrganizationId = orgId,
            Name = request.Name,
            AddressLine1 = request.AddressLine1,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            TimeZoneId = request.TimeZoneId
        };

        db.Locations.Add(location);
        await db.SaveChangesAsync(ct);

        var response = new LocationResponse(
            location.Id, location.OrganizationId, location.Name,
            location.AddressLine1, location.City, location.State,
            location.PostalCode, location.TimeZoneId);

        return TypedResults.Created($"/api/organizations/{orgId}/locations/{location.Id}", response);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> Update(
        Guid orgId,
        Guid id,
        UpdateLocationRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var location = await db.Locations.FirstOrDefaultAsync(l => l.OrganizationId == orgId && l.Id == id, ct);

        if (location is null) return TypedResults.NotFound();

        if (!IsKnownTimeZone(request.TimeZoneId))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.TimeZoneId)] = [$"'{request.TimeZoneId}' is not a recognized time zone id."]
            });
        }

        location.Name = request.Name;
        location.AddressLine1 = request.AddressLine1;
        location.City = request.City;
        location.State = request.State;
        location.PostalCode = request.PostalCode;
        location.TimeZoneId = request.TimeZoneId;

        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> Delete(
        Guid orgId,
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var location = await db.Locations.FirstOrDefaultAsync(l => l.OrganizationId == orgId && l.Id == id, ct);

        if (location is null) return TypedResults.NotFound();

        db.Locations.Remove(location);
        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }


    private static bool IsKnownTimeZone(string id) =>
        TimeZoneInfo.TryFindSystemTimeZoneById(id, out _);
}