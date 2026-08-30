namespace ResourcePlatform.Infrastructure;


// <summary>
// The organization the current request is acting inside.
// Guid.Empty when unresolved, which makes every tenant query match nothing
// </summary>
public interface ITenantContext
{
    Guid OrganizationId { get; }
    bool IsResolved { get; }
}

public interface ITenantContextSetter
{
    void Set(Guid organizationId);
}

public sealed class TenantContext : ITenantContext, ITenantContextSetter
{
    public Guid OrganizationId { get; private set; } = Guid.Empty;
    public bool IsResolved => OrganizationId != Guid.Empty;

    public void Set(Guid organizationId)
    {
        if (IsResolved)
            throw new InvalidOperationException("Tenant context is already resolved for this request.");
        OrganizationId = organizationId;
    }
}