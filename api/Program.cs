using api.Api;
using api.Database;

using IdentityModel;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using shared.Common;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<HeaderHandler>();
builder.Services.AddHttpClient<UserClient>()
    .ConfigureHttpClient((opt) => {
        opt.BaseAddress = new Uri(IPs.IdentityServer);
    }).AddHttpMessageHandler<HeaderHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization(Scopes.ProjectScope.Name);

app.Run();