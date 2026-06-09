namespace SalesManagement.Api.Features.Users;

public record UserDto(
    Guid Id,
    string Email,
    string Name,
    bool IsActive
);