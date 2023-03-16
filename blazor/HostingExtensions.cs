using Duende.IdentityServer;

using shared.Common;

namespace blazor;

public static class HostingExtensions {
    public static IServiceCollection AddConfiguredIdentityServer(this IServiceCollection services) {
        services.AddBff();

        services.AddAuthentication(
                options => {
                    options.DefaultScheme = "cookie";
                    options.DefaultChallengeScheme = "oidc";
                    options.DefaultSignOutScheme = "oidc";
                })
            .AddCookie("cookie", options => {
                options.Cookie.Name = "__Host-blazor";
                options.Cookie.SameSite = SameSiteMode.Strict;
            })
            .AddOpenIdConnect("oidc", options => {
                    options.Authority = IPs.IdentityServer;

                    options.ClientId = "web";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";
                    options.ResponseMode = "query";

                    options.SignInScheme = "cookie";

                    options.Scope.Clear();
                    options.Scope.Add(Scopes.ProjectScope.Name);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.OfflineAccess);

                    options.MapInboundClaims = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                }
            );
        return services;
    }
}