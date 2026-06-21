using Microsoft.EntityFrameworkCore;
using SalesManagement.Api.Infrastructure.Persistence;
using SalesManagement.Api.Domain.Entities;

namespace SalesManagement.Api.Features.Users;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        //group.RequireAuthorization();

        group.MapGet("/", GetUsers);
        group.MapPost("/", CreateUser);

        return group;
    }

    private static async Task<IResult> GetUsers(AppDbContext db)
    {
        var users = await db.Users
            .Select(x => new UserDto(
                x.Id,
                x.Email,
                x.Name,
                x.IsActive))
            .ToListAsync();

        return Results.Ok(users);
    }

    private static async Task<IResult> CreateUser(CreateUserRequest request, AppDbContext db)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);

        await db.SaveChangesAsync();

        return Results.Created(
            $"/api/users/{user.Id}",
            new UserDto(
                user.Id,
                user.Email,
                user.Name,
                user.IsActive));
    }

}