namespace ResourcePlatform.Domain;

public enum ResourceStatus
{
    Available = 0,
    Reserved = 1,
    InUse = 2,
    Maintenance = 3,
    Unavailable = 4,
    Retired = 5
}

public enum ReservationStatus
{
    Pending = 0,
    Approved = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4,
    Rejected = 5
}

public enum MembershipStatus
{
    Invited = 0,
    Active = 1,
    Suspended = 2
}