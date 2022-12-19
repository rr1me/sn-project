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
    [AllowAnonymous]
    public IActionResult Login(UserModel user)
    {
        var isAuthenticated = _authenticationHandler.Authenticate(user, HttpContext);
        if (!isAuthenticated)
            return Unauthorized("Authentication error");
        
        return Ok("cool");
    }
}

public class UserModel
{
    public string Login { get; set; }
    public string Password { get; set; }
}