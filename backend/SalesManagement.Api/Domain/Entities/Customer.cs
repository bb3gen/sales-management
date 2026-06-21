namespace SalesManagement.Api.Domain.Entities;

public class Customer : EntityBase
{
    public string CustomerCode { get; set; } = "";

    public string CustomerName { get; set; } = "";

    public string? CustomerKana { get; set; }

    public string? PostalCode { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }
}
