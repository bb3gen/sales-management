namespace SalesManagement.Api.Features.Orders.Requests;

public sealed class UpdateOrderRequest
{
    public DateOnly? OrderDate { get; set; }

    public Guid? CustomerId { get; set; }

    public List<UpdateOrderDetailRequest> Details { get; set; } = [];

    public DateTime UpdatedAt { get; set; }
}
