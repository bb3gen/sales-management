namespace SalesManagement.Api.Features.Auth.Responses;

public sealed class VerifyOtpResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}