namespace SalesManagement.Api.Features.Customers.Responses;

public sealed class CreateCustomerResponse
{
    public Guid Id { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
}