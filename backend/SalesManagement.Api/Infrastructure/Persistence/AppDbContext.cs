using Microsoft.EntityFrameworkCore;
using SalesManagement.Api.Domain.Entities;

namespace SalesManagement.Api.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();    
    public DbSet<Customer> Customers => Set<Customer>();    
}