using SalesManagement.Api.Domain.Enums;

namespace SalesManagement.Api.Features.Orders.Dtos;

public sealed class OrderListItemDto
{
    public Guid Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public DateOnly OrderDate { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }
}