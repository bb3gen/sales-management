namespace SalesManagement.Api.Features.Products.Requests;

public interface IProductRequest
{
    string ProductName { get; }

    decimal? UnitPrice { get; }

    string? Unit { get; }

    string? Remarks { get; }
}
