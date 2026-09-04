using ResourcePlatform.Domain;

namespace ResourcePlatform.UnitTests;


public class OverlapTests
{
    private static readonly DateTimeOffset Ten = new(2027, 3, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Eleven = new(2027, 3, 1, 11, 0, 0, TimeSpan.Zero);

    private static DateTimeOffset At(int hour, int minute = 0) =>
        new(2027, 3, 1, hour, minute, 0, TimeSpan.Zero);

    [Theory]
    // existing booking is 10:00-11:00
    [InlineData(10, 30, 11, 30, true, "starts inside")]
    [InlineData(9, 30, 10, 30, true, "ends inside")]
    [InlineData(10, 15, 10, 45, true, "fully contained")]
    [InlineData(9, 0, 12, 0, true, "fully contains")]
    [InlineData(10, 0, 11, 0, true, "identical")]
    [InlineData(11, 0, 12, 0, false, "adjacent after")]
    [InlineData(9, 0, 10, 0, false, "adjacent before")]
    [InlineData(12, 0, 13, 0, false, "well after")]
    [InlineData(7, 0, 8, 0, false, "well before")]
    public void Overlaps_matches_the_half_open_rule(
        int startHour, int startMin, int endHour, int endMin, bool expected, string because)
    {
        var actual = ReservationRules.Overlaps(At(startHour, startMin), At(endHour, endMin), Ten, Eleven);
        Assert.True(expected == actual, $"{because}: expected overlaps={expected}, got {actual}");
    }

    [Fact]
    public void Overlaps_is_symmetric()
    {
        var a1 = At(10);
        var a2 = At(11);
        var b1 = At(10, 30);
        var b2 = At(11, 30);

        Assert.Equal(
            ReservationRules.Overlaps(a1, a2, b1, b2),
            ReservationRules.Overlaps(b1, b2, a1, a2));
    }

    [Fact]
    public void Touching_intervals_do_not_overlap_this_is_rule_two_characters_can_break()
    {
        Assert.False(ReservationRules.Overlaps(At(11), At(12), At(10), At(11)));
    }
}

public class ValidateTest
{
    private static readonly DateTimeOffset Now = new(2027, 3, 1, 9, 0, 0, TimeSpan.Zero);
    private static DateTimeOffset At(int hour, int minute = 0) =>
        new(2027, 3, 1, hour, minute, 0, TimeSpan.Zero);

    [Fact]
    public void Accepts_a_normal_booking()
    {
        var result = ReservationRules.Validate(At(10), At(11), Now);
        Assert.True(result.IsValid);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Rejects_end_before_start()
    {
        var result = ReservationRules.Validate(At(15), At(14), Now);
        Assert.False(result.IsValid);
        Assert.Contains("after start", result.Error);
    }

    [Fact]
    public void Rejects_zero_length() =>
        Assert.False(ReservationRules.Validate(At(13), At(13), Now).IsValid);

    [Fact]
    public void Rejects_shorter_than_the_minimum() =>
        Assert.False(ReservationRules.Validate(At(10), At(10, 5), Now).IsValid);

    [Fact]
    public void Accepts_exactly_the_minimum() =>
        Assert.True(ReservationRules.Validate(At(10), At(10, 15), Now).IsValid);

    [Fact]
    public void Rejects_longer_than_the_maximum()
    {
        var start = At(10);
        Assert.False(ReservationRules.Validate(start, start.AddDays(ReservationRules.MaxDurationDays + 1), Now).IsValid);
    }

    [Fact]
    public void Rejects_a_booking_that_already_ended()
    {
        // Only possible to test cleanly because Validate takes nowUtc instead of reading the clock.
        var result = ReservationRules.Validate(At(7), At(8), Now);
        Assert.False(result.IsValid);
        Assert.Contains("past", result.Error);
    }

    [Fact]
    public void Accepts_a_booking_in_progress_right_now() =>
        Assert.True(ReservationRules.Validate(At(8, 30), At(10), Now).IsValid);

}
