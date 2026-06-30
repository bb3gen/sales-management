namespace SalesManagement.Api.Features.Products.Requests;

public sealed class CreateProductRequest : IProductRequest
{
    public string ProductName { get; set; } = string.Empty;

    public decimal? UnitPrice { get; set; }

    public string? Unit { get; set; }

    public string? Remarks { get; set; }
}