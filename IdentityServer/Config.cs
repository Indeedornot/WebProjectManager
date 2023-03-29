using Duende.IdentityServer;
using Duende.IdentityServer.Models;

using IdentityModel;

using shared.Common;

using IdentityResources = Duende.IdentityServer.Models.IdentityResources;
using SharedIdentityResources = shared.Common.IdentityResources;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[] { new IdentityResources.OpenId(), new IdentityResources.Profile(), SharedIdentityResources.Avatar };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[] { Scopes.ProjectScope, Scopes.UserDataScope };

    public static IEnumerable<Client> Clients =>
        new Client[] {
            // m2m client credentials flow client
            new() {
                ClientId = "api",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = { OidcConstants.GrantTypes.TokenExchange },
                AllowedScopes = { Scopes.UserDataScope.Name }
            },
            new() {
                ClientId = "swagger",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    Scopes.ProjectScope.Name,
                    SharedIdentityResources.Avatar.Name
                },
                RedirectUris = { $"{IPs.Api}/swagger/oauth2-redirect.html" },
                AllowedCorsOrigins = { $"{IPs.Api}" }
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
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    Scopes.ProjectScope.Name,
                    SharedIdentityResources.Avatar.Name
                }
            }
        };
}
