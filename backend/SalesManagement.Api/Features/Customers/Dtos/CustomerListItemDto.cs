namespace SalesManagement.Api.Features.Customers.Dtos;

public sealed class CustomerListItemDto
{
    public Guid Id { get; set; }

    public string CustomerCode { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }
}
