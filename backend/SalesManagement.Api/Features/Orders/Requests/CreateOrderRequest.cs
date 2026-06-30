namespace SalesManagement.Api.Features.Orders.Requests;

public sealed class CreateOrderRequest
{
    public DateOnly? OrderDate { get; set; }

    public Guid? CustomerId { get; set; }

    public List<CreateOrderDetailRequest> Details { get; set; } = [];
}
