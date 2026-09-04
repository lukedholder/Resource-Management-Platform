namespace ResourcePlatform.Domain;


/// <summary>
/// The pure booking rules. There is no database, HTTP, or clock of its own.
/// Everything is passed in, so every rule here is directly unit-testable
/// </summary>
public static class ReservationRules
{
    public const int MaxDurationDays = 30;
    public static readonly TimeSpan MinDuration = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Status that occopy a resource. Cancelled and Rejected do not block,
    /// and Completed is in the past.
    /// </summary>
    public static readonly ReservationStatus[] BlockingStatuses =
    [
        ReservationStatus.Pending,
        ReservationStatus.Approved,
        ReservationStatus.Active
    ];

    /// <summary>
    /// Half-open interval overlap: [aStart, aEnd) intersects [bStart, bEnd).
    /// Touching intervals (10:00-11:00 and 11:00-12:00) do NOT overlap.
    /// </summary>
    public static bool Overlaps(
        DateTimeOffset aStart, DateTimeOffset aEnd,
        DateTimeOffset bStart, DateTimeOffset bEnd)
        => aStart < bEnd && aEnd > bStart;

    public static ReservationValidation Validate(
        DateTimeOffset startUtc,
        DateTimeOffset endUtc,
        DateTimeOffset nowUtc)
    {
        if (endUtc <= startUtc)
            return ReservationValidation.Fail("End must be after start.");

        var duration = endUtc - startUtc;

        if (duration < MinDuration)
            return ReservationValidation.Fail($"A reservation must be at least {MinDuration.TotalMinutes:0} minutes.");

        if (duration > TimeSpan.FromDays(MaxDurationDays))
            return ReservationValidation.Fail($"A reservation cannot exceed {MaxDurationDays} days.");

        if (endUtc <= nowUtc)
            return ReservationValidation.Fail("A reservation cannot end in the past.");

        return ReservationValidation.Ok;
    }
}

public readonly record struct ReservationValidation(bool IsValid, string? Error)
{
    public static ReservationValidation Ok => new(true, null);
    public static ReservationValidation Fail(string error) => new(false, error);
}