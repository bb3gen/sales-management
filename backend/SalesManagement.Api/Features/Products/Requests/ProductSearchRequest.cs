namespace SalesManagement.Api.Features.Products.Requests;

public sealed class ProductSearchRequest
{
    public string? Keyword { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}