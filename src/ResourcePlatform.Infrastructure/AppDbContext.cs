using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;

// The DbSet is called Permissions too, so alias the catalog to keep both reachable
using Perms = ResourcePlatform.Domain.Permissions;

namespace ResourcePlatform.Infrastructure;


public class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenant)
    : IdentityUserContext<ApplicationUser, Guid>(options)
{
    private readonly ITenantContext _tenant = tenant;

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationMembership> Memberships => Set<OrganizationMembership>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<ResourceType> ResourceTypes => Set<ResourceType>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Reservation> Reservations => Set<Reservation>();


    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<ApplicationUser>(e =>
        {
            e.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        });

        b.Entity<Organization>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Slug).IsUnique();
        });

        b.Entity<Department>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
            e.HasOne(x => x.Organization)
                .WithMany(o => o.Departments)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.OrganizationId);
        });

        b.Entity<Location>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.AddressLine1).HasMaxLength(200);
            e.Property(x => x.AddressLine2).HasMaxLength(200);
            e.Property(x => x.City).HasMaxLength(100);
            e.Property(x => x.State).HasMaxLength(100);
            e.Property(x => x.PostalCode).HasMaxLength(20);
            e.Property(x => x.Country).HasMaxLength(100);
            e.Property(x => x.TimeZoneId).HasMaxLength(100).IsRequired();
            e.HasOne(x => x.Organization)
                .WithMany(o => o.Locations)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.OrganizationId);
        });

        b.Entity<ResourceType>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
            e.HasOne(x => x.Organization)
                .WithMany(o => o.ResourceTypes)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.OrganizationId);
        });

        b.Entity<Resource>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(2000);
            e.Property(x => x.AssetTag).HasMaxLength(100);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

            e.HasOne(x => x.Organization)
                .WithMany(o => o.Resources)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.ResourceType)
                .WithMany(t => t.Resources)
                .HasForeignKey(x => x.ResourceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Location)
                .WithMany(l => l.Resources)
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Department)
                .WithMany(d => d.Resources)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.OrganizationId);
            e.HasIndex(x => new { x.OrganizationId, x.LocationId });
            e.HasIndex(x => new { x.OrganizationId, x.ResourceTypeId });
        });

        b.Entity<Reservation>(e =>
        {
            e.Property(x => x.Purpose).HasMaxLength(1000);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

            e.HasOne(x => x.Organization)
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Resource)
                .WithMany(r => r.Reservations)
                .HasForeignKey(x => x.ResourceId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.OrganizationId);
            e.HasIndex(x => new { x.ResourceId, x.StartUtc, x.EndUtc });
        });

        // Join Entities:
        b.Entity<OrganizationMembership>(e =>
        {
            e.HasKey(x => new { x.OrganizationId, x.UserId });  // composite primary key
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

            e.HasOne(x => x.Organization)
                .WithMany(o => o.Memberships)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Role)
                .WithMany(r => r.Memberships)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.UserId);
        });

        b.Entity<Role>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();

            e.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.OrganizationId, x.Name }).IsUnique();

            e.HasData(
                new Role { Id = SystemRoles.OwnerId, Name = SystemRoles.Owner, IsSystemRole = true },
                new Role { Id = SystemRoles.AdministratorId, Name = SystemRoles.Administrator, IsSystemRole = true },
                new Role { Id = SystemRoles.ManagerId, Name = SystemRoles.Manager, IsSystemRole = true },
                new Role { Id = SystemRoles.MemberId, Name = SystemRoles.Member, IsSystemRole = true },
                new Role { Id = SystemRoles.ViewerId, Name = SystemRoles.Viewer, IsSystemRole = true });
        });

        b.Entity<Permission>(e =>
        {
            e.HasData(Perms.All.Select(n => new Permission { Id = Perms.IdFor(n), Name = n }));
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasIndex(x => x.Name).IsUnique();
        });

        b.Entity<RolePermission>(e =>
        {
            e.HasData(Perms.ByRole.SelectMany(kv =>
                kv.Value.Select(n => new RolePermission
                {
                    RoleId = kv.Key,
                    PermissionId = Perms.IdFor(n)
                })));

            e.HasKey(x => new { x.RoleId, x.PermissionId });

            e.HasOne(x => x.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Tenant isolation: every tenant-owned entity is filtered by the current organization
        // Guid.Empty (unresolved tenant) matches nothing, so the safe deafult is "no data"
        b.Entity<Department>().HasQueryFilter(x => x.OrganizationId == _tenant.OrganizationId);
        b.Entity<Location>().HasQueryFilter(x => x.OrganizationId == _tenant.OrganizationId);
        b.Entity<ResourceType>().HasQueryFilter(x => x.OrganizationId == _tenant.OrganizationId);
        b.Entity<Resource>().HasQueryFilter(x => x.OrganizationId == _tenant.OrganizationId);
        b.Entity<Reservation>().HasQueryFilter(x => x.OrganizationId == _tenant.OrganizationId);
        b.Entity<OrganizationMembership>().HasQueryFilter(x => x.OrganizationId == _tenant.OrganizationId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampTenant();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        StampTenant();
        return base.SaveChanges();
    }

    private void StampTenant()
    {
        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State != EntityState.Added) continue;

            var current = entry.Property(nameof(ITenantEntity.OrganizationId));
            if (Equals(current.CurrentValue, Guid.Empty))
                current.CurrentValue = _tenant.OrganizationId;
        }
    }
}