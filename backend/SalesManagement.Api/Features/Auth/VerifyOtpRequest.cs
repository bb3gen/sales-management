namespace SalesManagement.Api.Features.Auth;

public record VerifyOtpRequest(
    string Email,
    string Code
);
