namespace SalesManagement.Api.Features.Products.Responses;

public sealed class CreateProductResponse
{
    public Guid Id { get; set; }

    public string ProductCode { get; set; } = string.Empty;
}