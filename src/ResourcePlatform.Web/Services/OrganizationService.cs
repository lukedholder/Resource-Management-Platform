using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ResourcePlatform.Domain;
using ResourcePlatform.Infrastructure;

namespace ResourcePlatform.Web.Services;


public enum OrganizationOutcome { Created, Invalid, SlugTaken }

public readonly record struct OrganizationResult(OrganizationOutcome Outcome, string? Error, Organization? Organization)
{
    public static OrganizationResult Ok(Organization o) => new(OrganizationOutcome.Created, null, o);
    public static OrganizationResult Invalid(string e) => new(OrganizationOutcome.Invalid, e, null);
    public static OrganizationResult SlugTaken(string slug) =>
        new(OrganizationOutcome.SlugTaken, $"The URL name '{slug}' is already taken.", null);
}

/// <summary>
/// Creating an organization, in one place, called by both the JSON API and the UI form post.
/// </summary>
public sealed class OrganizationService(AppDbContext db, ICurrentUser currentUser)
{
    public async Task<OrganizationResult> CreateAsync(string name, string slug, CancellationToken ct)
    {
        name = name?.Trim() ?? "";
        slug = slug?.Trim() ?? "";

        var error = OrganizationRules.Validate(name, slug);
        if (error is not null) return OrganizationResult.Invalid(error);

        // Layer 1: a friendly answer in the common case.
        if (await db.Organizations.AsNoTracking().AnyAsync(o => o.Slug == slug, ct))
            return OrganizationResult.SlugTaken(slug);

        var userId = currentUser.UserId!.Value;

        var organization = new Organization
        {
            Name = name,
            Slug = slug,
            CreatedByUserId = userId
        };

        // The creator becomes the Owner. Organization and membership save in one transaction.
        db.Organizations.Add(organization);
        db.Memberships.Add(new OrganizationMembership
        {
            OrganizationId = organization.Id,
            UserId = userId,
            RoleId = SystemRoles.OwnerId,
            Status = MembershipStatus.Active
        });

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // Layer 2: lost a race - someone took the slug between the check above and this save.
            return OrganizationResult.SlugTaken(slug);
        }

        return OrganizationResult.Ok(organization);
    }
}