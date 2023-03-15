using Duende.IdentityServer;
using Duende.IdentityServer.Models;

using shared.IdentityServer;

namespace IdentityServer;

public static class Config {
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource> { new IdentityResources.OpenId(), new IdentityResources.Profile() };


    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope> { Scopes.ProjectScope };

    public static IEnumerable<Client> Clients =>
        new List<Client> {
            // machine to machine client
            new() {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                // scopes that client has access to
                AllowedScopes = { Scopes.ProjectScope.Name }
            },

            // interactive ASP.NET Core Web App
            new() {
                ClientId = "web",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect to after login
                RedirectUris = { "https://localhost:5002/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                AllowOfflineAccess = true,
                AllowedScopes = new List<string> {
                    IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, Scopes.ProjectScope.Name
                }
            }
        };
}