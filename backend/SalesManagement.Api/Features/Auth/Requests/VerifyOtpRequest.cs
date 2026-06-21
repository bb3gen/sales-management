namespace SalesManagement.Api.Features.Auth.Requests;

public record VerifyOtpRequest(
    string Email,
    string Code
);
