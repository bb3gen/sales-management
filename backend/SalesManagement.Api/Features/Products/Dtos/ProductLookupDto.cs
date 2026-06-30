namespace SalesManagement.Api.Features.Products.Dtos;

public sealed class ProductLookupDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public string? Unit { get; set; }

    public string DisplayName => $"{Code} {Name}";
}