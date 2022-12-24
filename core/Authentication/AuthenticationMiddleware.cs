using System.Security.Claims;
using System.Text.Encodings.Web;
using core.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace core;

public class GatewayAuthScheme : AuthenticationSchemeOptions { }

public class AuthenticationMiddleware : AuthenticationHandler<GatewayAuthScheme>
{
    private readonly JwtHandler _jwtHandler;

    public AuthenticationMiddleware(IOptionsMonitor<GatewayAuthScheme> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, JwtHandler jwtHandler) 
        : base(options, logger, encoder, clock)
    {
        _jwtHandler = jwtHandler;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accessToken = Context.Request.Cookies.FirstOrDefault(x => x.Key == "accessToken").Value;
        if (string.IsNullOrEmpty(accessToken))
        {
            var refreshToken = Context.Request.Cookies.FirstOrDefault(x => x.Key == "refreshToken").Value;
            if (string.IsNullOrEmpty(refreshToken))
                return AuthenticateResult.Fail("No access token or refresh token");

            
            
            
            accessToken = _jwtHandler.GenerateAccessToken(null, out _);
        }


        // AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name));
        var claims = new[]
        {
            new Claim("role", "role")
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "GatewayAuthScheme"));
        
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name));
    }
}