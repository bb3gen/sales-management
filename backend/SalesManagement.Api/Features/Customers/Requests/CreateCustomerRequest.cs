namespace SalesManagement.Api.Features.Customers.Requests;

public sealed class CreateCustomerRequest : ICustomerRequest
{
    public string CustomerName { get; set; } = string.Empty;

    public string? CustomerKana { get; set; }

    public string? PostalCode { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }
}
