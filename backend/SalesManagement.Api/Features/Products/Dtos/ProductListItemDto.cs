namespace SalesManagement.Api.Features.Products.Dtos;

public sealed class ProductListItemDto
{
    public Guid Id { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }
}