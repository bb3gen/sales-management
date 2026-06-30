namespace SalesManagement.Api.Features.Products.Dtos;

public sealed class ProductEditDto
{
    public Guid Id { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public string? Unit { get; set; }

    public string? Remarks { get; set; }

    public DateTime UpdatedAt { get; set; }
}