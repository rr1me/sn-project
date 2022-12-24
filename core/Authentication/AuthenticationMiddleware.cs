using System.Security.Claims;
using System.Text.Encodings.Web;
using core.Authentication;
using core.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace core;

public class GatewayAuthScheme : AuthenticationSchemeOptions { }

public class AuthenticationMiddleware : AuthenticationHandler<GatewayAuthScheme>
{
    private readonly DatabaseContext _db;
    private readonly JwtHandler _jwtHandler;

    public AuthenticationMiddleware(IOptionsMonitor<GatewayAuthScheme> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, JwtHandler jwtHandler, DatabaseContext db) 
        : base(options, logger, encoder, clock)
    {
        _jwtHandler = jwtHandler;
        _db = db;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accessToken = Request.Cookies.FirstOrDefault(x => x.Key == "accessToken").Value;

        string role;
        IDictionary<string, object> payload;
        // var isValid = _jwtHandler.TryDecodeToken(accessToken, TokenType.ACCESS, out payload);
        if (string.IsNullOrEmpty(accessToken) || !_jwtHandler.TryDecodeToken(accessToken, TokenType.ACCESS, out payload))
        {
            var refreshToken = Request.Cookies.FirstOrDefault(x => x.Key == "refreshToken").Value;
            if (string.IsNullOrEmpty(refreshToken) || _jwtHandler.TryDecodeToken(refreshToken, TokenType.REFRESH, out payload))
                return AuthenticateResult.Fail("No valid access token or refresh token");
            
            
            // payload = _jwtHandler.TryDecodeToken(refreshToken, TokenType.REFRESH, out payload);
            var user = _db.Users.First(x => x.Username == payload["username"].ToString());
            
            if (user.RefreshToken != refreshToken)
                return AuthenticateResult.Fail("Challenged refresh token");
            
            accessToken = _jwtHandler.GenerateAccessToken(user, out _);
            Response.Cookies.Append("accessToken", accessToken);

            role = user.Role.ToString();
        }
        else
        {
            // _jwtHandler.ValidateToken(accessToken, TokenType.ACCESS);
            // payload = _jwtHandler.DecodeToken(accessToken, TokenType.ACCESS);
            role = payload["role"].ToString();
        }


        // AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name));
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, role)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "GatewayAuthScheme"));
        
        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }
}