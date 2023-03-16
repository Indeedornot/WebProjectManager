using Duende.IdentityServer;
using Duende.IdentityServer.Models;

using shared.Common;

namespace IdentityServer;

public static class Config {
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[] { new IdentityResources.OpenId(), new IdentityResources.Profile() };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[] { Scopes.ProjectScope };

    public static IEnumerable<Client> Clients =>
        new Client[] {
            // m2m client credentials flow client
            new() {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                AllowedScopes = { Scopes.ProjectScope.Name }
            },

            // interactive client using code flow + pkce
            new() {
                ClientId = "web",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { $"{IPs.Blazor}/signin-oidc" },
                FrontChannelLogoutUri = $"{IPs.Blazor}/signout-oidc",
                PostLogoutRedirectUris = { $"{IPs.Blazor}/signout-callback-oidc" },
                AllowOfflineAccess = true,
                AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, Scopes.ProjectScope.Name }
            }
        };
}