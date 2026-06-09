namespace SalesManagement.Api.Domain.Auth;

public class OtpCode
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string CodeHash { get; set; } = "";

    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public int FailedCount { get; set; }

    public DateTime CreatedAt { get; set; }
}