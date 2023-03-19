using Duende.IdentityServer;
using Duende.IdentityServer.ResponseHandling;
using Duende.IdentityServer.Validation;

using IdentityServer.Data;
using IdentityServer.Extensions.CustomAuthorizeInteraction;
using IdentityServer.Extensions.CustomValidation;
using IdentityServer.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Serilog;

using shared.Common;

namespace IdentityServer;

internal static class HostingExtensions {
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder) {
        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        IIdentityServerBuilder isBuilder = builder.Services
            .AddIdentityServer(options => {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddAspNetIdentity<ApplicationUser>();

        builder.Services.AddScoped<IAuthorizeInteractionResponseGenerator, CreateAuthorizeInteractionResponseGenerator>();
        builder.Services.AddScoped<ICustomAuthorizeRequestValidator, CreateCustomAuthorizeRequestValidator>();

        isBuilder.AddExtensionGrantValidator<CustomGrantValidators.TokenExchangeGrantValidator>();
        isBuilder.AddProfileService<CustomProfileService>();

        isBuilder.Services.AddAuthentication()
            .AddLocalApi(options => {
                options.ExpectedScope = Scopes.UserDataScope.Name;
            });

        isBuilder.Services.AddAuthorization(options => {
            options.AddPolicy(Scopes.UserDataPolicyName, policy => {
                policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });
        });

        // .AddGoogle(options => {
        //     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        //
        //     // register your IdentityServer with Google at https://console.developers.google.com
        //     // enable the Google+ API
        //     // set the redirect URI to https://localhost:5001/signin-google
        //     options.ClientId = "copy client ID from Google here";
        //     options.ClientSecret = "copy client secret from Google here";
        // });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app) {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapControllers();
        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}