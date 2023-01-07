using core.Authentication;
using core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace core.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly GatewayAuthenticationHandler _authenticationHandler;
    private readonly DatabaseContext _db;

    public AuthController(GatewayAuthenticationHandler authenticationHandler, DatabaseContext db)
    {
        _authenticationHandler = authenticationHandler;
        _db = db;
    }

    [HttpPost("/login")]
    public IActionResult Login([FromBody]UserModel userModel)
    {
        var isAuthenticated = _authenticationHandler.Authenticate(userModel, HttpContext, out var user, _db);
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
        _authenticationHandler.RevokeRefreshToken(HttpContext, _db);
        return Ok("Succeed");
    }

    [HttpGet("/validate")]
    [Authorize]
    public IActionResult Validate() => Ok("All cool");
}

public class UserModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}