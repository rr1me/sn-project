using System.Security.Cryptography;
using core.Controllers;
using core.Data;
using JWT.Algorithms;
using JWT.Builder;

namespace core.Authentication;

public class GatewayAuthenticationHandler
{
    private readonly DatabaseContext _db;
    private readonly IConfiguration _config;

    public GatewayAuthenticationHandler(DatabaseContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
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

        var refreshTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = refreshTokenExpires
        };
        context.Response.Cookies.Append("refreshToken", refreshToken, refreshTokenCookieOptions);

        
        var accessToken = GenerateAccessToken(user, out var accessTokenExpires);
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

        return true;
    }

    private string GenerateAccessToken(UserEntity user, out DateTimeOffset accessTokenExpires)
    {
        var ecKeysAT = _config.GetSection("ECKeysAT").GetChildren().ToList();
        var privateECKeyForAT = ecKeysAT[0].Value;
        var publicECKeyForAT = ecKeysAT[1].Value;
        Console.WriteLine(privateECKeyForAT);
        Console.WriteLine(publicECKeyForAT);

        var publicEcDsaForAT = ECDsa.Create();
        publicEcDsaForAT.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicECKeyForAT), out _);

        var privateEcDsaForAT = ECDsa.Create();
        privateEcDsaForAT.ImportECPrivateKey(Convert.FromBase64String(privateECKeyForAT), out _);
        
        accessTokenExpires = DateTimeOffset.UtcNow.AddMinutes(10);
        var accessToken = JwtBuilder.Create().WithAlgorithm(new ES256Algorithm(publicEcDsaForAT, privateEcDsaForAT))
            .AddClaim("exp", accessTokenExpires.ToUnixTimeSeconds())
            .AddClaim("user", user.Login)
            .AddClaim("role", user.Role.ToString())
            .MustVerifySignature()
            .Encode();
        
        return accessToken;
    }

    private string GenerateRefreshToken(UserEntity user)
    {
        return "1";
    }
}