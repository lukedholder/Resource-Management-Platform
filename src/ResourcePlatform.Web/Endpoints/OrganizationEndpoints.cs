using Microsoft.AspNetCore.Http.HttpResults;    // Ok<>, NotFound, Create<>, Conflict<>, Results<>
using Microsoft.AspNetCore.Mvc;                 // ProblemDetails
using Microsoft.Data.SqlClient;                 // SqlException
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;
using ResourcePlatform.Web.Contracts;
using ResourcePlatform.Web.Services;

namespace ResourcePlatform.Web.Endpoints;


public static class OrganizationEndpoints
{
    // route registration
    public static RouteGroupBuilder MapOrganizationEndpoints(this IEndpointRouteBuilder routes)  // extension method
    {
        var group = routes.MapGroup("/api/organizations").WithTags("Organizations");

        group.MapGet("/", GetAll)
            .WithName("ListOrganizations")
            .WithSummary("List organizations (paged).");

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetOrganization")
            .WithSummary("Get one organization by id.");

        group.MapPost("/", Create)
            .RequireAuthorization()
            .WithName("CreateOrganization")
            .WithSummary("Create an organization.");

        return group;
    }

    private static async Task<Ok<PagedResult<OrganizationResponse>>> GetAll(
        AppDbContext db,
        CancellationToken ct,
        int? page = null,
        int? pageSize = null)
    {
        var (p, s) = PageDefaults.Clamp(page, pageSize);

        var query = db.Organizations.AsNoTracking().OrderBy(o => o.Name);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((p - 1) * s)
            .Take(s)
            .Select(o => new OrganizationResponse(o.Id, o.Name, o.Slug, o.IsActive, o.CreatedAt))
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling(total / (double)s);
        return TypedResults.Ok(new PagedResult<OrganizationResponse>(items, p, s, total, totalPages));
    }

    private static async Task<Results<Ok<OrganizationResponse>, NotFound>> GetById(
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var org = await db.Organizations
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrganizationResponse(o.Id, o.Name, o.Slug, o.IsActive, o.CreatedAt))
            .FirstOrDefaultAsync(ct);

        return org is null ? TypedResults.NotFound() : TypedResults.Ok(org);
    }

    private static async Task<Results<Created<OrganizationResponse>, Conflict<ProblemDetails>>> Create(
        CreateOrganizationRequest request,
        AppDbContext db,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        if (await db.Organizations.AsNoTracking().AnyAsync(o => o.Slug == request.Slug, ct))
            return SlugConflict(request.Slug);

        var org = new Organization
        {
            Name = request.Name,
            Slug = request.Slug,
            CreatedByUserId = currentUser.UserId!.Value
        };

        db.Organizations.Add(org);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            // List a race: another request inserted the same slug between the check above and this save
            return SlugConflict(request.Slug);
        }

        var response = new OrganizationResponse(org.Id, org.Name, org.Slug, org.IsActive, org.CreatedAt);
        return TypedResults.Created($"/api/organizations/{org.Id}", response);
    }

    private static Conflict<ProblemDetails> SlugConflict(string slug) =>
        TypedResults.Conflict(new ProblemDetails
        {
            Title = "Slug already in use",
            Detail = $"An organization with slug '{slug}' already exists.",
            Status = StatusCodes.Status409Conflict
        });

    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is SqlException { Number: 2601 or 2627 };
}