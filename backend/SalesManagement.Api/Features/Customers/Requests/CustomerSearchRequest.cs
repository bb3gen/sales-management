namespace SalesManagement.Api.Features.Customers.Requests;

public sealed class CustomerSearchRequest
{
    public string? CustomerCode { get; set; }

    public string? CustomerName { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
