using System.Security.Cryptography;
using core.Controllers;
using core.Data;
using JWT;
using JWT.Algorithms;
using JWT.Builder;

namespace core.Authentication;

public class GatewayAuthenticationHandler
{
    private readonly DatabaseContext _db;

    public GatewayAuthenticationHandler(DatabaseContext db)
    {
        _db = db;
    }

    public bool Authenticate(UserModel userModel, HttpContext context)
    {
        var user = _db.Users.FirstOrDefault(x =>x .Login == userModel.Login);
        if (user == null || !BCrypt.Net.BCrypt.Verify(userModel.Password, user.Password))
            return false;

        var refreshTokenExpires = DateTimeOffset.UtcNow.AddDays(7);
        var refreshToken = JwtBuilder.Create().WithAlgorithm(new RS256Algorithm(RSA.Create(), RSA.Create()))
            .AddClaim("exp", refreshTokenExpires.ToUnixTimeSeconds())
            .Encode();

        
        var rsa = RSA.Create();
        var accessTokenExpires = DateTimeOffset.UtcNow.AddMinutes(10);
        var accessToken = JwtBuilder.Create().WithAlgorithm(new RS256Algorithm(rsa, rsa))
            .AddClaim("exp", accessTokenExpires.ToUnixTimeSeconds())
            .AddClaim("user", user.Login)
            .AddClaim("role", user.Role.ToString())
            .MustVerifySignature()
            .Encode();
        // Console.WriteLine(rsa.ExportRSAPublicKeyPem());
        // Console.WriteLine(rsa.ExportRSAPrivateKeyPem());

        var refreshTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = refreshTokenExpires
        };
        context.Response.Cookies.Append("refreshToken", refreshToken, refreshTokenCookieOptions);

        var accessTokenCookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            Expires = accessTokenExpires
        };
        context.Response.Cookies.Append("accessToken", accessToken, accessTokenCookieOptions);
        
        user.RefreshToken = refreshToken;
        _db.Users.Update(user);
        _db.SaveChanges();

        // var p = ValidationParameters.Default;
        // var json = JwtValidator.Create().WithAlgorithm(new RS256Algorithm(rsa, rsa)).WithValidationParameters()
        //     .Decode(accessToken);
        // Console.WriteLine(json);

        return true;
    }
}