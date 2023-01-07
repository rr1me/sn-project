using core.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace core.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly GatewayAuthenticationHandler _authenticationHandler;

    public AuthController(GatewayAuthenticationHandler authenticationHandler)
    {
        _authenticationHandler = authenticationHandler;
    }

    [HttpPost("/login")]
    public IActionResult Login([FromBody]UserModel userModel)
    {
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
}

public class UserModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}