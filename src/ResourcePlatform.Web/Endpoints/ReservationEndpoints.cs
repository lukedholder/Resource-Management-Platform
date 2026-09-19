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
        BookingService booking,
        CancellationToken ct)
    {
        var result = await booking.BookAsync(request.ResourceId, request.StartUtc, request.EndUtc, request.Purpose, ct);

        switch (result.Outcome)
        {
            case BookingOutcome.ResourceNotFound:
                return TypedResults.NotFound();

            case BookingOutcome.Invalid:
                return TypedResults.ValidationProblem(
                    new Dictionary<string, string[]> { ["StartUtc"] = [result.Error!] });

            case BookingOutcome.Conflict:
                return TypedResults.Conflict(result.Error!);
        }

        var r = result.Reservation!;
        return TypedResults.Created(
            $"/api/reservations/{r.Id}",
            new ReservationResponse(r.Id, r.ResourceId, r.Resource!.Name, r.CreatedByUserId,
                r.ApprovedByUserId, r.StartUtc, r.EndUtc, r.Status, r.Purpose));
    }
}