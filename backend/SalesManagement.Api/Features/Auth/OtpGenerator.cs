namespace SalesManagement.Api.Features.Auth;

public static class OtpGenerator
{
    // 6桁のランダムな数値
    public static string Generate()
    {
        return Random.Shared
            .Next(100000, 999999)
            .ToString();
    }
}