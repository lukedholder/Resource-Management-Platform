using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;

namespace ResourcePlatform.Web.Services;


public enum BookingOutcome { Created, ResourceNotFound, Invalid, Conflict }

public readonly record struct BookingResult(BookingOutcome Outcome, string? Error, Reservation? Reservation)
{
    public static BookingResult Ok(Reservation r) => new(BookingOutcome.Created, null, r);
    public static BookingResult NotFound() => new(BookingOutcome.ResourceNotFound, null, null);
    public static BookingResult Invalid(string e) => new(BookingOutcome.Invalid, e, null);
    public static BookingResult Conflict(string e) => new(BookingOutcome.Conflict, e, null);
}

/// <summary>
/// The booking transaction, in one place, called by both the JSON API and the UI form post.
/// </summary>
public sealed class BookingService(AppDbContext db, ICurrentUser currentUser)
{
    public async Task<BookingResult> BookAsync(Guid resourceId, DateTimeOffset startUtc, DateTimeOffset endUtc, string? purpose, CancellationToken ct)
    {
        var check = ReservationRules.Validate(startUtc, endUtc, DateTimeOffset.UtcNow);
        if (!check.IsValid) return BookingResult.Invalid(check.Error!);

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        // Update lock on the resource row serializes every booking attemp for it.
        var resource = await db.Resources
            .FromSql($"SELECT * FROM Resources WITH (UPDLOCK, HOLDLOCK) WHERE Id = {resourceId}")
            .FirstOrDefaultAsync(ct);

        if (resource is null) return BookingResult.NotFound();

        if (!resource.IsReservable)
            return BookingResult.Conflict("That resource is not reservable.");

        if (resource.Status is ResourceStatus.Maintenance or ResourceStatus.Retired)
            return BookingResult.Conflict($"That resource is currently {resource.Status}.");

        var clash = await db.Reservations.AnyAsync(r =>
            r.ResourceId == resourceId &&
            ReservationRules.BlockingStatuses.Contains(r.Status) &&
            startUtc < r.EndUtc && endUtc > r.StartUtc, ct);

        if (clash) return BookingResult.Conflict("That time range conflicts with an existing reservation.");

        var reservation = new Reservation
        {
            ResourceId = resource.Id,
            CreatedByUserId = currentUser.UserId!.Value,
            StartUtc = startUtc,
            EndUtc = endUtc,
            Purpose = purpose,
            Status = resource.RequiresApproval ? ReservationStatus.Pending : ReservationStatus.Approved
        };

        db.Reservations.Add(reservation);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        reservation.Resource = resource;
        return BookingResult.Ok(reservation);
    }
}