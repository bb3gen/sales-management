namespace SalesManagement.Api.Features.Customers.Requests;

public interface ICustomerRequest
{
    string CustomerName { get; }
    string? CustomerKana { get; }
    string? PostalCode { get; }
    string? Address { get; }
    string? PhoneNumber { get; }
    string? Email { get; }
}