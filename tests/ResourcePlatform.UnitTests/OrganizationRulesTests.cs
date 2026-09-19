using ResourcePlatform.Domain;

namespace ResourcePlatform.UnitTests;


public class OrganizationRulesTests
{
    [Theory]
    [InlineData("Nashville Medical Group", "nashville-medical-group")]
    [InlineData("  Acme   Manufacturing  ", "acme-manufacturing")]
    [InlineData("Contoso, Inc.", "contoso-inc")]
    [InlineData("St. Thomas -- West", "st-thomas-west")]
    [InlineData("Unit 42", "unit-42")]
    [InlineData("École Café", "ecole-cafe")]
    [InlineData("!!!", "")]
    public void SuggestSlug_produces_lowercase_hyphenated_text(string name, string expected)
    {
        Assert.Equal(expected, OrganizationRules.SuggestSlug(name));
    }

    [Fact]
    public void SuggestSlug_never_exceeds_the_maximum_or_ends_with_a_hyphen()
    {
        var name = string.Join(" ", Enumerable.Repeat("word", 60));   // ~300 characters

        var slug = OrganizationRules.SuggestSlug(name);

        Assert.True(slug.Length <= OrganizationRules.SlugMaxLength);
        Assert.False(slug.EndsWith('-'));
        Assert.Null(OrganizationRules.Validate("Name", slug));
    }

    [Theory]
    [InlineData("nashville-medical")]
    [InlineData("acme")]
    [InlineData("unit-42")]
    public void Validate_accepts_good_slugs(string slug)
    {
        Assert.Null(OrganizationRules.Validate("Some Org", slug));
    }

    [Theory]
    [InlineData("Nashville")]      // uppercase
    [InlineData("two--hyphens")]   // double hyphen
    [InlineData("-leading")]
    [InlineData("trailing-")]
    [InlineData("has space")]
    [InlineData("")]
    public void Validate_rejects_bad_slugs(string slug)
    {
        Assert.NotNull(OrganizationRules.Validate("Some Org", slug));
    }

    [Fact]
    public void Validate_rejects_a_missing_name()
    {
        Assert.NotNull(OrganizationRules.Validate("   ", "fine-slug"));
    }

    [Fact]
    public void Validate_rejects_a_slug_one_character_too_long()
    {
        var slug = new string('a', OrganizationRules.SlugMaxLength + 1);
        Assert.NotNull(OrganizationRules.Validate("Some Org", slug));
    }

    [Fact]
    public void Validate_accepts_a_slug_exactly_at_the_maximum()
    {
        var slug = new string('a', OrganizationRules.SlugMaxLength);
        Assert.Null(OrganizationRules.Validate("Some Org", slug));
    }
}