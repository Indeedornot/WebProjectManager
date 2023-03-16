using api.Database;

using IdentityModel;

using Microsoft.IdentityModel.Tokens;

using shared.Common;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<DataContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Require certain token
builder.Services.AddAuthorization(options => {
    options.AddPolicy(
        Scopes.ProjectScope.Name,
        policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireAssertion(context => context.User.HasClaim(JwtClaimTypes.Scope, Scopes.ProjectScope.Name));
        }
    );
});

//Require any token from IdentityServer
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer",
        options => {
            options.Authority = IPs.IdentityServer;
            options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = false };
        });

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization(Scopes.ProjectScope.Name);

app.Run();