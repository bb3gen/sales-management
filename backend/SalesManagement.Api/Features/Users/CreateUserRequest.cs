namespace SalesManagement.Api.Features.Users;

public record CreateUserRequest(
    string Email,
    string Name
);