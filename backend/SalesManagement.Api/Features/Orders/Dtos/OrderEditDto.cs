using SalesManagement.Api.Domain.Enums;

namespace SalesManagement.Api.Features.Orders.Dtos;

public sealed class OrderEditDto
{
    public Guid Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public DateOnly OrderDate { get; set; }

    public Guid CustomerId { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<OrderDetailDto> Details { get; set; } = [];
}