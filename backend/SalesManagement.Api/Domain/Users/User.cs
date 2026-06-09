namespace SalesManagement.Api.Domain.Users;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = "";

    public string Name { get; set; } = "";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
}