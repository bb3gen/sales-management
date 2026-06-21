namespace SalesManagement.Api.Shared.Dtos;

public sealed class PagedResult<T>
{
    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public IReadOnlyList<T> Items { get; set; } = [];
}
