using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace core.IdentityConfig;

public class IdentityConfig
{
    public IEnumerable<Client> Clients => new List<Client>
    {
        new Client
        {
            ClientId="client1",
            AllowedGrantTypes=GrantTypes.ClientCredentials,
            ClientSecrets=
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes={"api", "IdentityServerApi"}
        },
        new ()
        {
            ClientId = "client",
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = { "openid", "api", "profile", "IdentityServerApi" },
            RedirectUris = { "https://localhost:3000" },
            AllowedGrantTypes = { GrantType.Implicit },
            AllowAccessTokensViaBrowser = true
            // AllowRememberConsent = false,
            // RequireConsent = true,
            // Enabled = true,
        },
        new ()
        {
            ClientId = "clienttoken",
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = { "openid", "api", "profile", "IdentityServerApi" },
            RedirectUris = { "https://localhost:3000" },
            AllowedGrantTypes = { GrantType.ClientCredentials },
            AllowAccessTokensViaBrowser = true
            // AllowRememberConsent = false,
            // RequireConsent = true,
            // Enabled = true,
        }
    };

    public IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
    {
        new ("api", "my api"),
        new ("IdentityServerApi")
        // new ApiScope(OpenIdConnectScope.OpenId),
    };
    
    public IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    };
    
    public List<TestUser> Users => new List<TestUser> {
        new TestUser
        {
            Username = "alice",
            Claims = new Claim[] {
                new Claim(JwtClaimTypes.Name, "Alice Smith"),
                new Claim(JwtClaimTypes.GivenName, "Alice"),
                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                new Claim(JwtClaimTypes.Email, "AliceSmith@email.com")
            },
            Password = "Abcd@1234",
            IsActive = true,
            SubjectId = Guid.NewGuid().ToString(),
        }
    };

    public IEnumerable<ApiResource> ApiResources => new[]
    {
        new ApiResource("api")
        {
            ApiSecrets = { new Secret("secret".Sha256()) }
        }
    };
}