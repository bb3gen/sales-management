namespace SalesManagement.Api.Features.Auth.Requests;

public record LogoutRequest(
    string RefreshToken
);
