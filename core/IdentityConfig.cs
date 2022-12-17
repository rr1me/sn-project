using IdentityServer4.Models;

namespace core.IdentityConfig;

public class IdentityConfig
{
    public IEnumerable<Client> Clients => new List<Client>
    {
        new ()
        {
            ClientId = "client",
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = { "api" }
        }
    };

    public IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
    {
        new ApiScope("api", "my api")
    };
}