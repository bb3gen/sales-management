namespace SalesManagement.Api.Features.Orders.Requests;

public sealed class UpdateOrderDetailRequest
{
    public Guid? ProductId { get; set; }

    public decimal Quantity { get; set; }
}
