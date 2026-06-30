namespace SalesManagement.Api.Domain.Entities;

public sealed class OrderDetail : EntityBase
{
    public Guid OrderId { get; set; }

    public int LineNo { get; set; }

    public Guid ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Amount { get; set; }

    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
