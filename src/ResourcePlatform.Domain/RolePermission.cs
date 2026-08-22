namespace ResourcePlatform.Domain;


// A join entity, composite key (RoleID, PermissionId)
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }

    public Guid PermissionId { get; set; }
    public Permission? Permission { get; set; }
}