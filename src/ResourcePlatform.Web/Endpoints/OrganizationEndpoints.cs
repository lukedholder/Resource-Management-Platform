using Microsoft.AspNetCore.Http.HttpResults;    // Ok<>, NotFound, Created<>, Conflict<>, ValidationProblem, Results<>
using Microsoft.AspNetCore.Mvc;                 // ProblemDetails
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
        // Organizations are the tenant boundary itself, so the group is NOT tenant-scoped
        // access is controlled by membership instead
        var group = routes.MapGroup("/api/organizations")
                            .WithTags("Organizations")
                            .RequireAuthorization();

        group.MapGet("/", GetMine)
            .WithName("ListMyOrganizations")
            .WithSummary("List organizations the current user belongs to.");

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetOrganization")
            .WithSummary("Get one organization by id.");

        group.MapPost("/", Create)
            .RequireAuthorization()
            .WithName("CreateOrganization")
            .WithSummary("Create an organization.");

        return group;
    }

    // route handlers
    private static async Task<Ok<List<OrganizationResponse>>> GetMine(
        AppDbContext db,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;

        // Deliberately cross-tenant. The question "Which organization do I belong to?"
        // spans tenants by definition
        var items = await db.Memberships
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.Status == MembershipStatus.Active)
            .OrderBy(m => m.Organization!.Name)  // order before projecting
            .Select(m => new OrganizationResponse(
                m.Organization!.Id, m.Organization.Name, m.Organization.Slug,
                m.Organization.IsActive, m.Organization.CreatedAt))
            .ToListAsync(ct);

        return TypedResults.Ok(items);
    }

    private static async Task<Results<Ok<OrganizationResponse>, NotFound>> GetById(
        Guid id,
        AppDbContext db,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;

        // 404, not 403: a non-member must not be able to tell whether this organization exists
        var org = await db.Memberships
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.OrganizationId == id && m.Status == MembershipStatus.Active)
            .Select(m => new OrganizationResponse(
                m.Organization!.Id, m.Organization.Name, m.Organization.Slug,
                m.Organization.IsActive, m.Organization.CreatedAt))
            .FirstOrDefaultAsync(ct);

        return org is null ? TypedResults.NotFound() : TypedResults.Ok(org);
    }

    private static async Task<Results<Created<OrganizationResponse>, Conflict<ProblemDetails>, ValidationProblem>> Create(
        CreateOrganizationRequest request,
        OrganizationService organizations,
        CancellationToken ct)
    {
        var result = await organizations.CreateAsync(request.Name, request.Slug, ct);

        switch (result.Outcome)
        {
            case OrganizationOutcome.Invalid:
                return TypedResults.ValidationProblem(
                    new Dictionary<string, string[]> { ["Slug"] = [result.Error!] });

            case OrganizationOutcome.SlugTaken:
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = "Slug already in use",
                    Detail = $"An organization with slug '{request.Slug}' already exists.",
                    Status = StatusCodes.Status409Conflict
                });
        }

        var org = result.Organization!;
        var response = new OrganizationResponse(org.Id, org.Name, org.Slug, org.IsActive, org.CreatedAt);
        return TypedResults.Created($"/api/organizations/{org.Id}", response);
    }
}