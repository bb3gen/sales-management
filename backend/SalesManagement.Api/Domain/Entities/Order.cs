using SalesManagement.Api.Domain.Enums;

namespace SalesManagement.Api.Domain.Entities;

public sealed class Order : EntityBase
{
    public string OrderNumber { get; set; } = string.Empty;

    public DateOnly OrderDate { get; set; }

    public Guid CustomerId { get; set; }

    public decimal TotalAmount { get; set; }

    public Customer Customer { get; set; } = null!;

    public OrderStatus Status { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = [];
}