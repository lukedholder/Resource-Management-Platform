using System.Security.Cryptography;
using System.Text;

namespace ResourcePlatform.Domain;


/// <summary
/// The vocabulary of things a member may do inside an organization.
/// Roles are bundles of these. Code never checks a role name directly.
/// </summary>
public static class Permissions
{
    public const string LocationRead = "Location.Read";
    public const string LocationCreate = "Location.Create";
    public const string LocationUpdate = "Location.Update";
    public const string LocationDelete = "Location.Delete";

    public const string ResourceTypeRead = "ResourceType.Read";
    public const string ResourceTypeCreate = "ResourceType.Create";
    public const string ResourceTypeUpdate = "ResourceType.Update";
    public const string ResourceTypeDelete = "ResourceType.Delete";

    public const string ResourceRead = "Resource.Read";
    public const string ResourceCreate = "Resource.Create";
    public const string ResourceUpdate = "Resource.Update";
    public const string ResourceDelete = "Resource.Delete";

    public const string ReservationRead = "Reservation.Read";
    public const string ReservationCreate = "Reservation.Create";
    public const string ReservationCancelOwn = "Reservation.CancelOwn";
    public const string ReservationCancelAny = "Reservation.CancelAny";
    public const string ReservationApprove = "Reservation.Approve";

    public const string MemberRead = "Member.Read";
    public const string MemberInvite = "Member.Invite";
    public const string MemberRemove = "Member.Remove";
    public const string MemberAssignRole = "Member.AssignRole";

    public const string OrganizationUpdate = "Organization.Update";
    public const string OrganizationDelete = "Organization.Delete";

    public const string AuditRead = "Audit.Read";

    public static readonly string[] All =
    {
        LocationRead, LocationCreate, LocationUpdate, LocationDelete,
        ResourceTypeRead, ResourceTypeCreate, ResourceTypeUpdate, ResourceTypeDelete,
        ResourceRead, ResourceCreate, ResourceUpdate, ResourceDelete,
        ReservationRead, ReservationCreate, ReservationCancelOwn, ReservationCancelAny, ReservationApprove,
        MemberRead, MemberInvite, MemberRemove, MemberAssignRole,
        OrganizationUpdate, OrganizationDelete,
        AuditRead
    };

    /// <summary>
    /// Deterministic id derived from the permission name, so seeded rows are
    /// identical on every machine without hand-writing 24 GUID literals.
    /// </summary>
    public static Guid IdFor(string name)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes("resourceplatform.permission:" + name));
        return new Guid(hash.AsSpan(0, 16));
    }

    /// <summary>
    /// Which permissions each built-in role carries.
    /// </summary>
    public static IReadOnlyDictionary<Guid, string[]> ByRole { get; } = new Dictionary<Guid, string[]>
    {
        [SystemRoles.OwnerId] = All,

        [SystemRoles.AdministratorId] =
        [
            LocationRead, LocationCreate, LocationUpdate, LocationDelete,
            ResourceTypeRead, ResourceTypeCreate, ResourceTypeUpdate, ResourceTypeDelete,
            ResourceRead, ResourceCreate, ResourceUpdate, ResourceDelete,
            ReservationRead, ReservationCreate, ReservationCancelOwn, ReservationCancelAny, ReservationApprove,
            MemberRead, MemberInvite, MemberRemove, MemberAssignRole,
            AuditRead
        ],

        [SystemRoles.ManagerId] =
        [
            LocationRead,
            ResourceTypeRead,
            ResourceRead, ResourceCreate, ResourceUpdate,
            ReservationRead, ReservationCreate, ReservationCancelOwn, ReservationCancelAny, ReservationApprove,
            MemberRead
        ],

        [SystemRoles.MemberId] =
        [
            LocationRead,
            ResourceTypeRead,
            ResourceRead,
            ReservationRead, ReservationCreate, ReservationCancelOwn,
            MemberRead
        ],

        [SystemRoles.ViewerId] =
        [
            LocationRead, ResourceTypeRead, ResourceRead, ReservationRead, MemberRead
        ]
    };
}