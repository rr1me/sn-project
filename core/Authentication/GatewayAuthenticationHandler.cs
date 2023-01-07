using core.Controllers;
using core.Data;

namespace core.Authentication;

public class GatewayAuthenticationHandler
{
    private readonly JwtHandler _jwtHandler;

    public GatewayAuthenticationHandler(JwtHandler jwtHandler)
    {
        _jwtHandler = jwtHandler;
    }

    public bool Authenticate(UserModel userModel, HttpContext context, out UserEntity? user, DatabaseContext db)
    {
        user = db.Users.FirstOrDefault(x =>x .Username == userModel.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(userModel.Password, user.Password))
            return false;

        var refreshToken = _jwtHandler.GenerateRefreshToken(user, out var refreshTokenExpires);
        var refreshTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = refreshTokenExpires
        };
        context.Response.Cookies.Append("refreshToken", refreshToken, refreshTokenCookieOptions);
        
        var accessToken = _jwtHandler.GenerateAccessToken(user, out var accessTokenExpires);
        var accessTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = accessTokenExpires
        };
        context.Response.Cookies.Append("accessToken", accessToken, accessTokenCookieOptions);
        
        user.RefreshToken = refreshToken;
        db.Users.Update(user);
        db.SaveChanges();

        return true;
    }

    public void RevokeRefreshToken(HttpContext context, DatabaseContext db)
    {
        context.Response.Cookies.Delete("refreshToken");
        context.Response.Cookies.Delete("accessToken");

        var refreshToken = context.Request.Cookies.FirstOrDefault(x => x.Key == "refreshToken").Value;

        if (string.IsNullOrEmpty(refreshToken)) return;
        
        var user = db.Users.FirstOrDefault(x => x.RefreshToken == refreshToken);

        if (user == null) return;

        user.RefreshToken = null;

        db.SaveChanges();
    }
}