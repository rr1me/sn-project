using core.Controllers;
using core.Data;

namespace core.Authentication;

public class GatewayAuthenticationHandler
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly JwtHandler _jwtHandler;

    public GatewayAuthenticationHandler(IServiceScopeFactory scopeFactory, JwtHandler jwtHandler)
    {
        _scopeFactory = scopeFactory;
        _jwtHandler = jwtHandler;
    }

    public bool Authenticate(UserModel userModel, HttpContext context, out UserEntity? user)
    {
        var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
        
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

    public void RevokeRefreshToken(HttpContext context)
    {
        context.Response.Cookies.Delete("refreshToken");
        context.Response.Cookies.Delete("accessToken");

        var refreshToken = context.Request.Cookies.FirstOrDefault(x => x.Key == "refreshToken").Value;

        if (string.IsNullOrEmpty(refreshToken)) return;
        
        var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();

        var user = db.Users.FirstOrDefault(x => x.RefreshToken == refreshToken);

        if (user == null) return;

        user.RefreshToken = null;

        db.SaveChanges();
    }
    
    // public void RevokeRefreshToken(user)

    // public string GenerateAccessToken(UserEntity user, out DateTimeOffset accessTokenExpires)
    // {
    //     var ecKeys = _config.GetSection("ECKeysForAccessToken").GetChildren().ToList();
    //     var privateEcKey = ecKeys[0].Value!;
    //     var publicEcKey = ecKeys[1].Value!;
    //
    //     var publicEcDsa = ECDsa.Create();
    //     publicEcDsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicEcKey), out _);
    //
    //     var privateEcDsa = ECDsa.Create();
    //     privateEcDsa.ImportECPrivateKey(Convert.FromBase64String(privateEcKey), out _);
    //     
    //     accessTokenExpires = DateTimeOffset.UtcNow.AddMinutes(10);
    //     var accessToken = JwtBuilder.Create().WithAlgorithm(new ES256Algorithm(publicEcDsa, privateEcDsa))
    //         .AddClaim("exp", accessTokenExpires.ToUnixTimeSeconds())
    //         .AddClaim("user", user.Username)
    //         .AddClaim("role", user.Role.ToString())
    //         .MustVerifySignature()
    //         .Encode();
    //     
    //     return accessToken;
    // }
    //
    // private string GenerateRefreshToken(UserEntity user, out DateTimeOffset refreshTokenExpires)
    // {
    //     var ecKeys = _config.GetSection("ECKeysRefreshToken").GetChildren().ToList();
    //     var privateEcKey = ecKeys[0].Value!;
    //     var publicEcKey = ecKeys[1].Value!;
    //
    //     var publicEcDsa = ECDsa.Create();
    //     publicEcDsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicEcKey), out _);
    //
    //     var privateEcDsa = ECDsa.Create();
    //     privateEcDsa.ImportECPrivateKey(Convert.FromBase64String(privateEcKey), out _);
    //     
    //     refreshTokenExpires = DateTimeOffset.UtcNow.AddDays(7);
    //     var refreshToken = JwtBuilder.Create().WithAlgorithm(new ES256Algorithm(publicEcDsa, privateEcDsa))
    //         .AddClaim("exp", refreshTokenExpires.ToUnixTimeSeconds())
    //         .AddClaim("username", user.Username)
    //         .Encode();
    //     
    //     return refreshToken;
    // }
}