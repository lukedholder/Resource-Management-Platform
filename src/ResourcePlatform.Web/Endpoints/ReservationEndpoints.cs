using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;
using ResourcePlatform.Web.Contracts;
using ResourcePlatform.Web.Services;

namespace ResourcePlatform.Web.Endpoints;


public static class ReservationEndpoints
{
    public static RouteGroupBuilder MapReservationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/reservations")
                            .WithTags("Reservations")
                            .RequireAuthorization()
                            .RequireTenant();

        group.MapGet("/", GetAll).WithName("ListReservations").RequirePermission(Permissions.ReservationRead);
        group.MapPost("/", Create).WithName("CreateReservation").RequirePermission(Permissions.ReservationCreate);

        return group;
    }

    private static async Task<Ok<List<ReservationResponse>>> GetAll(
        AppDbContext db,
        CancellationToken ct,
        Guid? resourceId = null)
    {
        var query = db.Reservations.AsNoTracking();
        if (resourceId is not null) query = query.Where(r => r.ResourceId == resourceId);

        var items = await query.OrderBy(r => r.StartUtc)
            .Select(r => new ReservationResponse(
                r.Id, r.ResourceId, r.Resource!.Name, r.CreatedByUserId, r.ApprovedByUserId,
                r.StartUtc, r.EndUtc, r.Status, r.Purpose))
            .ToListAsync(ct);

        return TypedResults.Ok(items);
    }

    private static async Task<Results<Created<ReservationResponse>, NotFound, ValidationProblem, Conflict<string>>> Create(
        CreateReservationRequest request,
        AppDbContext db,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        var check = ReservationRules.Validate(request.StartUtc, request.EndUtc, DateTimeOffset.UtcNow);
        if (!check.IsValid)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]> { ["StartUtc"] = [check.Error!] });

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        // Take an update lock on the resource row for the life of this transaction.
        // Every booking attempt for this resource must pass through here, so they serialize.
        var resource = await db.Resources
            .FromSql($"SELECT * FROM Resources WITH (UPDLOCK, HOLDLOCK) WHERE Id = {request.ResourceId}")
            .FirstOrDefaultAsync(ct);

        if (resource is null) return TypedResults.NotFound();

        if (!resource.IsReservable)
            return TypedResults.Conflict("That resource is not reservable.");

        if (resource.Status is ResourceStatus.Maintenance or ResourceStatus.Retired or ResourceStatus.Unavailable)
            return TypedResults.Conflict($"That resource is currently {resource.Status}.");

        var conflict = await db.Reservations.AnyAsync(r =>
            r.ResourceId == request.ResourceId &&
            ReservationRules.BlockingStatuses.Contains(r.Status) &&
            request.StartUtc < r.EndUtc && request.EndUtc > r.StartUtc, ct);

        if (conflict) return TypedResults.Conflict("That time range conflicts with an existing reservation.");

        var reservation = new Reservation
        {
            ResourceId = resource.Id,
            CreatedByUserId = currentUser.UserId!.Value,
            StartUtc = request.StartUtc,
            EndUtc = request.EndUtc,
            Purpose = request.Purpose,
            Status = resource.RequiresApproval ? ReservationStatus.Pending : ReservationStatus.Approved
        };

        db.Reservations.Add(reservation);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return TypedResults.Created(
            $"/api/reservations/{reservation.Id}",
            new ReservationResponse(
                reservation.Id, resource.Id, resource.Name, reservation.CreatedByUserId,
                null, reservation.StartUtc, reservation.EndUtc, reservation.Status, reservation.Purpose)
        );
    }
}