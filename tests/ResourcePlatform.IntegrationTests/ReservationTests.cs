using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ResourcePlatform.IntegrationTests;


[Collection(nameof(ApiCollection))]
public class ReservationTests(ApiFactory factory)
{
    private static object Booking(Guid resourceId, string start, string end) =>
        new { resourceId, startUtc = start, endUtc = end };

    [Fact]
    public async Task Overlapping_bookings_are_rejected()
    {
        var tenant = await TestClient.NewTenantAsync(factory);
        var resourceId = await TestClient.CreateResourceAsync(tenant.Client, "Room 201");

        var first = await tenant.Client.PostAsJsonAsync("/api/reservations",
            Booking(resourceId, "2029-03-01T10:00:00Z", "2029-03-01T11:00:00Z"));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var overlapping = await tenant.Client.PostAsJsonAsync("/api/reservations",
            Booking(resourceId, "2029-03-01T10:30:00Z", "2029-03-01T11:30:00Z"));
        Assert.Equal(HttpStatusCode.Conflict, overlapping.StatusCode);
    }

    [Fact]
    public async Task Adjacent_bookings_are_allowed()
    {
        var tenant = await TestClient.NewTenantAsync(factory);
        var resourceId = await TestClient.CreateResourceAsync(tenant.Client, "Room 202");

        var first = await tenant.Client.PostAsJsonAsync("/api/reservations",
            Booking(resourceId, "2029-04-01T10:00:00Z", "2029-04-01T11:00:00Z"));
        first.EnsureSuccessStatusCode();

        // Half-open intervals: a booking ending at 11:00 does not block one starting at 11:00.
        var adjacent = await tenant.Client.PostAsJsonAsync("/api/reservations",
            Booking(resourceId, "2029-04-01T11:00:00Z", "2029-04-01T12:00:00Z"));

        Assert.Equal(HttpStatusCode.Created, adjacent.StatusCode);
    }

    [Fact]
    public async Task An_inverted_time_range_is_a_bad_request()
    {
        var tenant = await TestClient.NewTenantAsync(factory);
        var resourceId = await TestClient.CreateResourceAsync(tenant.Client, "Room 203");

        var response = await tenant.Client.PostAsJsonAsync("/api/reservations",
            Booking(resourceId, "2029-05-01T15:00:00Z", "2029-05-01T14:00:00Z"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task A_booking_on_another_tenants_resource_is_not_found()
    {
        var alice = await TestClient.NewTenantAsync(factory);
        var bob = await TestClient.NewTenantAsync(factory);

        var aliceResource = await TestClient.CreateResourceAsync(alice.Client, "Alice Only Room");

        var response = await bob.Client.PostAsJsonAsync("/api/reservations",
            Booking(aliceResource, "2029-07-01T10:00:00Z", "2029-07-01T11:00:00Z"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Only_one_of_many_simultaneous_bookings_wins()
    {
        var tenant = await TestClient.NewTenantAsync(factory);
        var resourceId = await TestClient.CreateResourceAsync(tenant.Client, "Contested Room");

        var attempts = Enumerable.Range(0, 12).Select(_ =>
            tenant.Client.PostAsJsonAsync("/api/reservations",
                Booking(resourceId, "2029-06-01T09:00:00Z", "2029-06-01T10:00:00Z")));

        var responses = await Task.WhenAll(attempts);

        Assert.Equal(1, responses.Count(r => r.StatusCode == HttpStatusCode.Created));
        Assert.Equal(11, responses.Count(r => r.StatusCode == HttpStatusCode.Conflict));
    }
}