using System.Security.Claims;
using SecretsSharing.Infrastructure.Models;

namespace SecretsSharing.Application.Services;

public class UserIdentityService
{
    public UserIdentityService()
    {
        CurrentUser = null;
    }

    public UserIdentityService(ClaimsPrincipal claimsPrincipal)
    {
        CurrentUser = new User
        {
            Id = long.Parse(claimsPrincipal.FindFirst("id")!.Value),
            Login = claimsPrincipal.FindFirst("login")!.Value,
        };
    }

    public User? CurrentUser { get; }
}