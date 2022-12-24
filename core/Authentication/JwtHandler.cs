using System.Security.Cryptography;
using core.Data;
using JWT.Algorithms;
using JWT.Builder;

namespace core.Authentication;

public class JwtHandler
{
    private readonly IConfiguration _config;

    private readonly ES256Algorithm accessTokenAlgorithm;
    private readonly ES256Algorithm refreshTokenAlgorithm;

    public JwtHandler(IConfiguration config)
    {
        _config = config;

        accessTokenAlgorithm = GenerateES256Algorithm("ECKeysForAccessToken");
        refreshTokenAlgorithm = GenerateES256Algorithm("ECKeysRefreshToken");
    }

    private ES256Algorithm GenerateES256Algorithm(string sectionName)
    {
        var ecKeys = _config.GetSection(sectionName).GetChildren().ToList();
        var privateEcKey = ecKeys[0].Value!;
        var publicEcKey = ecKeys[1].Value!;

        var publicEcDsa = ECDsa.Create();
        publicEcDsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicEcKey), out _);

        var privateEcDsa = ECDsa.Create();
        privateEcDsa.ImportECPrivateKey(Convert.FromBase64String(privateEcKey), out _);
        
        return new ES256Algorithm(publicEcDsa, privateEcDsa);
    }

    public string GenerateAccessToken(UserEntity user, out DateTimeOffset accessTokenExpires)
    {
        // var ecKeys = _config.GetSection("ECKeysForAccessToken").GetChildren().ToList();
        // var privateEcKey = ecKeys[0].Value!;
        // var publicEcKey = ecKeys[1].Value!;
        //
        // var publicEcDsa = ECDsa.Create();
        // publicEcDsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicEcKey), out _);
        //
        // var privateEcDsa = ECDsa.Create();
        // privateEcDsa.ImportECPrivateKey(Convert.FromBase64String(privateEcKey), out _);
        
        accessTokenExpires = DateTimeOffset.UtcNow.AddMinutes(10);
        var accessToken = JwtBuilder.Create().WithAlgorithm(accessTokenAlgorithm)
            .AddClaim("exp", accessTokenExpires.ToUnixTimeSeconds())
            .AddClaim("user", user.Username)
            .AddClaim("role", user.Role.ToString())
            .MustVerifySignature()
            .Encode();
        
        return accessToken;
    }

    public string GenerateRefreshToken(UserEntity user, out DateTimeOffset refreshTokenExpires)
    {
        // var ecKeys = _config.GetSection("ECKeysRefreshToken").GetChildren().ToList();
        // var privateEcKey = ecKeys[0].Value!;
        // var publicEcKey = ecKeys[1].Value!;
        //
        // var publicEcDsa = ECDsa.Create();
        // publicEcDsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicEcKey), out _);
        //
        // var privateEcDsa = ECDsa.Create();
        // privateEcDsa.ImportECPrivateKey(Convert.FromBase64String(privateEcKey), out _);
        
        refreshTokenExpires = DateTimeOffset.UtcNow.AddDays(7);
        var refreshToken = JwtBuilder.Create().WithAlgorithm(refreshTokenAlgorithm)
            .AddClaim("exp", refreshTokenExpires.ToUnixTimeSeconds())
            .AddClaim("username", user.Username)
            .Encode();
        
        return refreshToken;
    }

    public void DecodeToken(string token)
    {
        
    }
}