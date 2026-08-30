namespace ResourcePlatform.Domain;


// <summary>
// Built-in roles shared by every organization (Role.OrganizationId is null).
// Ids are fixed contants so migrations seed the same rows on every machine.
// </summary>
public static class SystemRoles
{
    public static readonly Guid OwnerId = new("11111111-0000-0000-0000-000000000001");
    public static readonly Guid AdministratorId = new("11111111-0000-0000-0000-000000000002");
    public static readonly Guid ManagerId = new("11111111-0000-0000-0000-000000000003");
    public static readonly Guid MemberId = new("11111111-0000-0000-0000-000000000004");
    public static readonly Guid ViewerId = new("11111111-0000-0000-0000-000000000005");

    public const string Owner = "Owner";
    public const string Administrator = "Administrator";
    public const string Manager = "Manager";
    public const string Member = "Member";
    public const string Viewer = "Viewer";
}