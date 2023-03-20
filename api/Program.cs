using api.Api;
using api.Database;

using IdentityModel;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

using Refit;

using shared.Common;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddRefitClient<IUserClient>()
    .ConfigureHttpClient((opt) => {
        opt.BaseAddress = new Uri(IPs.IdentityServer);
    }).AddHttpMessageHandler<HeaderHandler>();

builder.Services.AddTransient<HeaderHandler>();

//Require certain token
builder.Services.AddAuthorization(options => {
    options.AddPolicy(
        Scopes.ProjectScope.Name,
        policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", Scopes.ProjectScope.Name);
        }
    );
});

//Require any token from IdentityServer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options => {
            options.Authority = IPs.IdentityServer;
            options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = false, ValidTypes = new[] { "at+jwt" } };
        });

builder.Services.AddOpenApiDocument(options => {
    options.DocumentName = "v1";
    options.Title = "Protected API";
    options.Version = "v1";

    options.AddSecurity("oauth2",
        new OpenApiSecurityScheme {
            Type = OpenApiSecuritySchemeType.OAuth2,
            Flow = OpenApiOAuth2Flow.AccessCode,
            Flows = new OpenApiOAuthFlows {
                AuthorizationCode = new OpenApiOAuthFlow {
                    AuthorizationUrl = $"{IPs.IdentityServer}/connect/authorize",
                    TokenUrl = $"{IPs.IdentityServer}/connect/token",
                    Scopes = new Dictionary<string, string> { { Scopes.ProjectScope.Name, Scopes.ProjectScope.DisplayName } }
                }
            }
        });

    options.OperationProcessors.Add(new OperationSecurityScopeProcessor("oauth2"));
});

WebApplication? app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseOpenApi();
    app.UseSwaggerUi3(options => {
        options.OAuth2Client = new OAuth2ClientSettings() { ClientId = "swagger", UsePkceWithAuthorizationCodeGrant = true };
    });
}

app.MapControllers()
    .RequireAuthorization(Scopes.ProjectScope.Name);

app.Run();