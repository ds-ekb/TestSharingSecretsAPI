using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using SecretsSharing.Infrastructure;
using SecretsSharing.Infrastructure.Models;

namespace SecretsSharing.Application.Services;

public class UserService
{
    private readonly PostgresDbContext postgresDbContext;

    public UserService(PostgresDbContext postgresDbContext)
    {
        this.postgresDbContext = postgresDbContext;
    }

    public async Task<User> Register(string login, string password)
    {
        if (await postgresDbContext.Users.AnyAsync(user => user.Login == login))
        {
            throw new ArgumentException("User with this login already exist");
        }

        var newUser = new User() {Login = login, Password = PasswordHashService.HashPassword(password)};

        await postgresDbContext.Users.AddAsync(newUser);
        await postgresDbContext.SaveChangesAsync();

        return newUser;
    }

    public async Task<User> Login(string login, string password)
    {
        var user = await postgresDbContext.Users.FirstOrDefaultAsync(user => user.Login == login);

        if (user == null)
        {
            throw new ArgumentException("User with this login already exist");
        }

        if (!PasswordHashService.VerifyPassword(password, user.Password))
        {
            throw new AuthenticationException("Password incorrect");
        }

        return user;
    }
}