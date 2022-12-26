using System.Security.Claims;
using System.Text.Encodings.Web;
using core.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace core.Authentication;

public class GatewayAuthScheme : AuthenticationSchemeOptions { }

public class AuthenticationMiddleware : AuthenticationHandler<GatewayAuthScheme>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly JwtHandler _jwtHandler;

    public AuthenticationMiddleware(IOptionsMonitor<GatewayAuthScheme> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, JwtHandler jwtHandler, IServiceScopeFactory scopeFactory) 
        : base(options, logger, encoder, clock)
    {
        _jwtHandler = jwtHandler;
        _scopeFactory = scopeFactory;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
        
        var accessToken = Request.Cookies.FirstOrDefault(x => x.Key == "accessToken").Value;
        var refreshToken = Request.Cookies.FirstOrDefault(x => x.Key == "refreshToken").Value;
        
        var isAccessTokenValid = _jwtHandler.TryDecodeToken(accessToken, TokenType.Access, out var accessTokenPayload);
        var isRefreshTokenValid = _jwtHandler.TryDecodeToken(refreshToken, TokenType.Refresh, out var refreshTokenPayload);

        string role;
        if (string.IsNullOrEmpty(accessToken) || !isAccessTokenValid)
        {
            if (string.IsNullOrEmpty(refreshToken) || !isRefreshTokenValid)
                return AuthenticateResult.Fail("No valid access token or refresh token");
            
            var user = db.Users.First(x => x.Username == refreshTokenPayload["username"].ToString());

            if (user.RefreshToken != refreshToken)
            {
                DeleteCookies();
                return AuthenticateResult.Fail("Challenged refresh token");
            }
            
            accessToken = _jwtHandler.GenerateAccessToken(user, out _);
            Response.Cookies.Append("accessToken", accessToken);

            role = user.Role.ToString();
        }
        else if (string.IsNullOrEmpty(refreshToken) || !isRefreshTokenValid)
        {
            DeleteCookies();
            return AuthenticateResult.Fail("No valid refresh token");
        }
        else if (!accessTokenPayload["username"].ToString()!.Equals(refreshTokenPayload["username"].ToString()!))
        {
            Console.WriteLine(accessTokenPayload["username"]);
            Console.WriteLine(refreshTokenPayload["username"]);

            // string str = refreshTokenPayload["username"];
            Console.WriteLine(accessTokenPayload["username"] == refreshTokenPayload["username"]);
            Console.WriteLine(accessTokenPayload["username"].ToString()!.Equals(refreshTokenPayload["username"].ToString()!));
            
            var firstUser = db.Users.FirstOrDefault(x => x.Username == accessTokenPayload["username"].ToString());
            if (firstUser != null)
                firstUser.RefreshToken = null;

            var secondUser = db.Users.FirstOrDefault(x => x.Username == refreshTokenPayload["username"].ToString());

            if (secondUser != null)
                secondUser.RefreshToken = null;

            db.SaveChangesAsync();
            DeleteCookies();
            return AuthenticateResult.Fail("Both tokens challenged");
        }
        else
            role = accessTokenPayload["role"].ToString()!;

        var claims = new[]
        {
            new Claim(ClaimTypes.Role, role)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "GatewayAuthScheme"));
        
        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }

    private void DeleteCookies()
    {
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");
    }
}