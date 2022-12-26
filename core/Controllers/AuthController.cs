using core.Authentication;
using core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace core.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly GatewayAuthenticationHandler _authenticationHandler;
    private readonly IServiceScopeFactory _scopeFactory;

    public AuthController(GatewayAuthenticationHandler authenticationHandler, IServiceScopeFactory scopeFactory)
    {
        _authenticationHandler = authenticationHandler;
        _scopeFactory = scopeFactory;
        // _db = _db = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
    }

    [HttpPost("/login")]
    // [AllowAnonymous]
    public IActionResult Login([FromBody]UserModel userModel)
    {
        // HttpContext.Response.Redirect();
        var isAuthenticated = _authenticationHandler.Authenticate(userModel, HttpContext, out var user);
        if (!isAuthenticated)
            return Unauthorized("Authentication error");
        
        return Ok(new
        {
            username = user!.Username,
            role = user.Role
        });
    }

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        _authenticationHandler.RevokeRefreshToken(HttpContext);
        return Ok("Succeed");
    }

    [HttpGet("/validate")]
    [Authorize]
    public IActionResult Validate()
    {
        return Ok("All cool");
    }

    [HttpPost("/try")]
    [Authorize(Roles = "Admin")]
    public IActionResult TryItOn()
    {
        return Ok("hi");
    }
}

public class UserModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}