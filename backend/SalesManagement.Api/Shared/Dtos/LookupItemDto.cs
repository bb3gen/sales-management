namespace SalesManagement.Api.Shared.Dtos;

public sealed class LookupItemDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string DisplayName => $"{Code} {Name}";
}
