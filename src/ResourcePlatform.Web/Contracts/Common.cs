namespace ResourcePlatform.Web.Contracts;


public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

public static class PageDefaults
{
    public const int DefaultPageSize = 25;
    public const int MaxPageSize = 100;

    public static (int Page, int PageSize) Clamp(int? page, int? pageSize)
    {
        var p = page is null or < 1 ? 1 : page.Value;
        var s = pageSize is null or < 1 ? DefaultPageSize : pageSize.Value;
        if (s > MaxPageSize) s = MaxPageSize;
        return (p, s);
    }
}