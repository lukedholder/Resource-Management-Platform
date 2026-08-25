using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;

namespace ResourcePlatform.Infrastructure;


public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
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
            e.HasIndex(x => new { x.OrganizationId, x.ResourceTypeId});
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
        });

        b.Entity<Permission>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasIndex(x => x.Name).IsUnique();
        });

        b.Entity<RolePermission>(e =>
        {
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
    }
}