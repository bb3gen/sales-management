namespace SalesManagement.Api.Features.Customers.Dtos;

public sealed class CustomerEditDto
{
    public Guid Id { get; set; }

    public string CustomerCode { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string? CustomerKana { get; set; }

    public string? PostalCode { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public DateTime UpdatedAt { get; set; }
}
