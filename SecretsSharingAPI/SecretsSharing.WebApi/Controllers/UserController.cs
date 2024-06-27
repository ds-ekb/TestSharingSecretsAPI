using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretsSharing.Application.Services;
using SecretsSharing.Infrastructure.Models;
using SecretsSharing.WebApi.Models;

namespace SecretsSharing.WebApi.Controllers;

[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserService userService;
    private readonly UserIdentityService userIdentityService;

    public UserController(UserService userService, UserIdentityService userIdentityService)
    {
        this.userService = userService;
        this.userIdentityService = userIdentityService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody]UserDto userDto)
    {
        User newUser;

        try
        {
            newUser = await userService.Register(userDto.Login, userDto.Password);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return Conflict(exception.Message);
        }

        await CreateCookieSession(newUser);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody]UserDto userDto)
    {
        User user;

        try
        {
            user = await userService.Login(userDto.Login, userDto.Password);
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception);
            return NotFound(exception.Message);
        }
        catch (AuthenticationException exception)
        {
            Console.WriteLine(exception);
            return BadRequest(exception.Message);
        }

        await CreateCookieSession(user);

        return Ok();
    }

    [HttpGet("signOut"), Authorize]
    public async Task<ActionResult> SignOut()
    {
        if (userIdentityService.CurrentUser == null)
        {
            BadRequest("Can't exit, no logged user");
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok();
    }

    [HttpGet("getCurrentUser")]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        return Ok(userIdentityService.CurrentUser);
    }

    private async Task CreateCookieSession(User user)
    {
        var claims = new List<Claim>
        {
            new("id", user.Id.ToString()),
            new("login", user.Login),
        };

        var claimIdentity = new ClaimsIdentity(claims, "Cookies");

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimIdentity));
    }
}