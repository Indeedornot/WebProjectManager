using IdentityModel;

using IdentityServer.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Serilog;

using System.Security.Claims;

namespace IdentityServer.Data;

public static class SeedData {
    public static void EnsureSeedData(WebApplication app) {
        using IServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        ApplicationDbContext context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        context.Database.EnsureDeleted();
        context.Database.Migrate();

        UserManager<ApplicationUser> userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        foreach (TestUsers.TestUser testUser in TestUsers.Users) {
            ApplicationUser user = userMgr.FindByNameAsync(testUser.User.UserName).Result;
            if (user != null) {
                Log.Debug($"{testUser.User.UserName} already exists");
                continue;
            }

            IdentityResult result = userMgr.CreateAsync(testUser.User, testUser.Password).Result;
            if (!result.Succeeded) {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(testUser.User,
                new Claim[] {
                    new(JwtClaimTypes.Name, "Alice Smith"), new(JwtClaimTypes.GivenName, "Alice"), new(JwtClaimTypes.FamilyName, "Smith"),
                    new(JwtClaimTypes.WebSite, "http://alice.com")
                }).Result;

            if (!result.Succeeded) {
                throw new Exception(result.Errors.First().Description);
            }

            Log.Debug($"{testUser.User.UserName} created");
        }
    }
}