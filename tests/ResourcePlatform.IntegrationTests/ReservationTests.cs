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