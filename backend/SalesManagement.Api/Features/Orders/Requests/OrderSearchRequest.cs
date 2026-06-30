using SalesManagement.Api.Domain.Enums;

namespace SalesManagement.Api.Features.Orders.Requests;

public sealed class OrderSearchRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? OrderNumber { get; set; }

    public Guid? CustomerId { get; set; }

    public DateOnly? OrderDateFrom { get; set; }

    public DateOnly? OrderDateTo { get; set; }

    public OrderStatus? Status { get; set; }
}