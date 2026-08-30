using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;
using ResourcePlatform.Web.Contracts;
using ResourcePlatform.Web.Services;

namespace ResourcePlatform.Web.Endpoints;


public static class LocationEndpoints
{
    public static RouteGroupBuilder MapLocationEndpoints(this IEndpointRouteBuilder routes)
    {
        // No {orgId}. The tenant comes from the request context, validated against membership
        var group = routes.MapGroup("/api/locations")
                            .WithTags("Locations")
                            .RequireAuthorization()
                            .RequireTenant();

        group.MapGet("/", GetAll).WithName("ListLocations");
        group.MapGet("/{id:guid}", GetById).WithName("GetLocation");
        group.MapPost("/", Create).WithName("CreateLocation");
        group.MapPut("/{id:guid}", Update).WithName("UpdateLocation");
        group.MapDelete("/{id:guid}", Delete).WithName("DeleteLocation");

        return group;
    }

    private static async Task<Ok<PagedResult<LocationResponse>>> GetAll(
        AppDbContext db,
        CancellationToken ct,
        int? page = null,
        int? pageSize = null)
    {
        var (p, s) = PageDefaults.Clamp(page, pageSize);

        var query = db.Locations
            .AsNoTracking()
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
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var location = await db.Locations
            .AsNoTracking()
            .Select(l => new LocationResponse(l.Id, l.OrganizationId, l.Name, l.AddressLine1, l.City, l.State, l.PostalCode, l.TimeZoneId))
            .FirstOrDefaultAsync(ct);

        return location is null ? TypedResults.NotFound() : TypedResults.Ok(location);
    }

    private static async Task<Results<Created<LocationResponse>, NotFound, ValidationProblem>> Create(
        CreateLocationRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        if (!IsKnownTimeZone(request.TimeZoneId))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.TimeZoneId)] = [$"'{request.TimeZoneId}' is not a recognized time zone id."]
            });
        }

        // OrganizationId is deliberately not set here. SaveChangesAsync stamps it from the tenant context
        var location = new Location
        {
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

        return TypedResults.Created($"/api/locations/{location.Id}", response);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> Update(
        Guid id,
        UpdateLocationRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var location = await db.Locations.FirstOrDefaultAsync(l => l.Id == id, ct);

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
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var location = await db.Locations.FirstOrDefaultAsync(l => l.Id == id, ct);

        if (location is null) return TypedResults.NotFound();

        db.Locations.Remove(location);
        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }


    private static bool IsKnownTimeZone(string id) =>
        TimeZoneInfo.TryFindSystemTimeZoneById(id, out _);
}