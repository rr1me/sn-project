using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace core;

public class GatewayAuthScheme : AuthenticationSchemeOptions
{ }

public class AuthenticationMiddleware : AuthenticationHandler<GatewayAuthScheme>
{
    public AuthenticationMiddleware(IOptionsMonitor<GatewayAuthScheme> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Console.WriteLine("AY БЛЯТЬ");
        // AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name));
        var claims = new[]
        {
            new Claim("aye", "aye")
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "GatewayAuthScheme"));
        
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name));
    }
}