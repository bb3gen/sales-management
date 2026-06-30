namespace SalesManagement.Api.Features.Orders.Requests;

public sealed class CreateOrderDetailRequest
{
    public Guid? ProductId { get; set; }

    public decimal Quantity { get; set; }
}
