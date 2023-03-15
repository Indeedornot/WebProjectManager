using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace IdentityServer;

public class SeedData {
    public static void EnsureSeedData(WebApplication app) {
        using IServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

        ConfigurationDbContext context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
        context.Database.Migrate();
        EnsureSeedData(context);
    }

    private static void EnsureSeedData(ConfigurationDbContext context) {
        if (!context.Clients.Any()) {
            Log.Debug("Clients being populated");
            foreach (Client client in Config.Clients.ToList()) {
                context.Clients.Add(client.ToEntity());
            }

            context.SaveChanges();
        }
        else {
            Log.Debug("Clients already populated");
        }

        if (!context.IdentityResources.Any()) {
            Log.Debug("IdentityResources being populated");
            foreach (IdentityResource resource in Config.IdentityResources.ToList()) {
                context.IdentityResources.Add(resource.ToEntity());
            }

            context.SaveChanges();
        }
        else {
            Log.Debug("IdentityResources already populated");
        }

        if (!context.ApiScopes.Any()) {
            Log.Debug("ApiScopes being populated");
            foreach (ApiScope resource in Config.ApiScopes.ToList()) {
                context.ApiScopes.Add(resource.ToEntity());
            }

            context.SaveChanges();
        }
        else {
            Log.Debug("ApiScopes already populated");
        }

        if (!context.IdentityProviders.Any()) {
            Log.Debug("OIDC IdentityProviders being populated");
            context.IdentityProviders.Add(new OidcProvider {
                Scheme = "demoidsrv", DisplayName = "IdentityServer", Authority = "https://demo.duendesoftware.com", ClientId = "login"
            }.ToEntity());
            context.SaveChanges();
        }
        else {
            Log.Debug("OIDC IdentityProviders already populated");
        }
    }
}