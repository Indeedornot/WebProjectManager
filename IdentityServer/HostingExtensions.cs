using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;

using IdentityServer;
using IdentityServer.Data;
using IdentityServer.Pages.Admin.ApiScopes;
using IdentityServer.Pages.Admin.Clients;
using IdentityServer.Pages.Admin.IdentityScopes;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using Serilog;

namespace IdentityServer;

internal static class HostingExtensions {
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder) {
        builder.Services.AddRazorPages();

        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        IIdentityServerBuilder isBuilder = builder.Services
            .AddIdentityServer(options => {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            .AddTestUsers(TestUsers.Users)
            // this adds the config data from DB (clients, resources, CORS)
            .AddConfigurationStore(options => {
                options.ConfigureDbContext = b =>
                    b.UseSqlite(connectionString, dbOpts => dbOpts.MigrationsAssembly(typeof(Program).Assembly.FullName));
            })
            // this is something you will want in production to reduce load on and requests to the DB
            //.AddConfigurationStoreCache()
            //
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options => {
                options.ConfigureDbContext = b =>
                    b.UseSqlite(connectionString, dbOpts => dbOpts.MigrationsAssembly(typeof(Program).Assembly.FullName));

                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
                options.RemoveConsumedTokens = true;
            });

        builder.Services.AddAuthentication();
        // .AddGoogle(options =>
        // {
        //     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        //
        //     // register your IdentityServer with Google at https://console.developers.google.com
        //     // enable the Google+ API
        //     // set the redirect URI to https://localhost:5001/signin-google
        //     options.ClientId = "copy client ID from Google here";
        //     options.ClientSecret = "copy client secret from Google here";
        // });


        // this adds the necessary config for the simple admin/config pages
        builder.Services.AddAuthorization(options =>
            options.AddPolicy("admin",
                policy => policy.RequireClaim("sub", "1"))
        );

        builder.Services.Configure<RazorPagesOptions>(options =>
            options.Conventions.AuthorizeFolder("/Admin", "admin"));

        builder.Services.AddTransient<ClientRepository>();
        builder.Services.AddTransient<IdentityScopeRepository>();
        builder.Services.AddTransient<ApiScopeRepository>();

        // if you want to use server-side sessions: https://blog.duendesoftware.com/posts/20220406_session_management/
        // then enable it
        //isBuilder.AddServerSideSessions();
        //
        // and put some authorization on the admin/management pages using the same policy created above
        //builder.Services.Configure<RazorPagesOptions>(options =>
        //    options.Conventions.AuthorizeFolder("/ServerSideSessions", "admin"));

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app) {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }

        InitializeDatabase(app);

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }

    private static void InitializeDatabase(IApplicationBuilder app) {
        using IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

        ConfigurationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        context.Database.Migrate();
        if (!context.Clients.Any()) {
            foreach (Client client in Config.Clients) {
                context.Clients.Add(client.ToEntity());
            }

            context.SaveChanges();
        }

        if (!context.IdentityResources.Any()) {
            foreach (IdentityResource resource in Config.IdentityResources) {
                context.IdentityResources.Add(resource.ToEntity());
            }

            context.SaveChanges();
        }

        if (context.ApiScopes.Any()) {
            return;
        }

        foreach (ApiScope resource in Config.ApiScopes) {
            context.ApiScopes.Add(resource.ToEntity());
        }

        context.SaveChanges();
    }
}