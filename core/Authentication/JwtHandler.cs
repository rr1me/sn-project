﻿using System.Security.Cryptography;
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

        accessTokenExpires = DateTimeOffset.UtcNow.AddMinutes(10);
        
        return JwtBuilder.Create().WithAlgorithm(accessTokenAlgorithm)
            .AddClaim("exp", accessTokenExpires.ToUnixTimeSeconds())
            .AddClaim("username", user.Username)
            .AddClaim("role", user.Role.ToString())
            .MustVerifySignature()
            .Encode();;
    }

    public string GenerateRefreshToken(UserEntity user, out DateTimeOffset refreshTokenExpires)
    {
        refreshTokenExpires = DateTimeOffset.UtcNow.AddDays(7);

        return JwtBuilder.Create().WithAlgorithm(refreshTokenAlgorithm)
            .AddClaim("exp", refreshTokenExpires.ToUnixTimeSeconds())
            .AddClaim("username", user.Username)
            .Encode();;
    }

    public IDictionary<string, object> DecodeToken(string token, TokenType type)
    {
        var algorithm = type == TokenType.Access ? accessTokenAlgorithm : refreshTokenAlgorithm;

        return JwtBuilder.Create()
            .WithAlgorithm(algorithm)
            .MustVerifySignature()
            .Decode<IDictionary<string, object>>(token);
    }

    // public bool ValidateToken(string token, TokenType type)
    // {
        // var algorithm = type == TokenType.ACCESS ? accessTokenAlgorithm : refreshTokenAlgorithm;
        //
        // new JwtValidator().
        // var valid = JwtBuilder.Create()
        //     .WithAlgorithm(algorithm)
        //     .WithValidationParameters(ValidationParameters.Default);
        // return false;
    // }

    public bool TryDecodeToken(string token, TokenType type, out IDictionary<string, object> payload)
    {
        var algorithm = type == TokenType.Access ? accessTokenAlgorithm : refreshTokenAlgorithm;

        try
        {
            payload = JwtBuilder.Create()
                .WithAlgorithm(algorithm)
                .MustVerifySignature()
                .Decode<IDictionary<string, object>>(token);
        }
        catch (Exception e)
        {
            Console.WriteLine("Decode exception: " + e.Message);
            payload = null;
            return false;
        }

        return true;
    }
}

public enum TokenType
{
    Access,
    Refresh
}