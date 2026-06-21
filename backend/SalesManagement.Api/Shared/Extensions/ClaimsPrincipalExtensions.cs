using System.Security.Claims;

namespace SalesManagement.Api.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        return Guid.Parse(
            user.FindFirst(
                ClaimTypes.NameIdentifier)!.Value);
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(
            ClaimTypes.Email)!.Value;
    }
}