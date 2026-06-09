namespace SalesManagement.Api.Features.Auth;

public record LogoutRequest(
    string RefreshToken
);
