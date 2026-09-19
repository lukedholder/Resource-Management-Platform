using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ResourcePlatform.Domain;


/// <summary>
/// Rules for naming an organization. The constants are the single source of truth:
/// the database column sizes, the API request attributes, the HTML form limits,
/// and the server-side check all read from here.
/// </summary>
public static class OrganizationRules
{
    public const int NameMaxLength = 200;
    public const int SlugMaxLength = 100;

    // Lowercase letters and digits, separated by single hyphens: "nashville-medical"
    public const string SlugPattern = "^[a-z0-9]+(-[a-z0-9]+)*$";

    // Returns an error message, or null when the name and slug are acceptable
    public static string? Validate(string? name, string? slug)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Name is required.";

        if (name.Length > NameMaxLength)
            return $"Name cannot exceed {NameMaxLength} characters.";

        if (string.IsNullOrWhiteSpace(slug))
            return "URL name is required.";

        if (slug.Length > SlugMaxLength)
            return $"URL name cannot exceed {SlugMaxLength} characters.";

        if (!Regex.IsMatch(slug, SlugPattern))
            return "URL name must be lowercase letters, digits, and single hyphens.";

        return null;
    }

    /// <summary>
    /// Turns a display name into a slug candidate: "Nashville Medical Group!" → "nashville-medical-group".
    /// The result still has to pass Validate, and still has to be unique.
    /// </summary>
    public static string SuggestSlug(string name)
    {
        var builder = new StringBuilder(name.Length);
        var pendingHyphen = false;

        // FormD splits "é" into "e" plus a separate accent mark, so the accent can be dropped below.
        var decomposed = name.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);

        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;

            if (c is >= 'a' and <= 'z' or >= '0' and <= '9')
            {
                if (pendingHyphen && builder.Length > 0)
                    builder.Append('-');

                builder.Append(c);
                pendingHyphen = false;
            }
            else
            {
                pendingHyphen = true;
            }
        }

        var slug = builder.ToString();
        return slug.Length <= SlugMaxLength ? slug : slug[..SlugMaxLength].TrimEnd('-');
    }
}