namespace SalesManagement.Api.Domain.Entities;

public sealed class Product : EntityBase
{
    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public string? Unit { get; set; }

    public string? Remarks { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = [];
}
