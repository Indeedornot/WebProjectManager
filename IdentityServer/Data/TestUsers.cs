using IdentityModel;

using IdentityServer.Models;

using System.Security.Claims;

namespace IdentityServer.Data;

public static class TestUsers {
    public class TestUser {
        public ApplicationUser User;
        public string Password;
        public Claim[] Claims;
    }

    public static IEnumerable<TestUser> Users => new List<TestUser> {
        new() {
            User = new ApplicationUser() { UserName = "alice", Email = "AliceSmith@email.com", EmailConfirmed = true },
            Claims = new Claim[] {
                new(JwtClaimTypes.Name, "Alice Smith"), new(JwtClaimTypes.GivenName, "Alice"), new(JwtClaimTypes.FamilyName, "Smith"),
                new(JwtClaimTypes.WebSite, "http://alice.com")
            },
            Password = "Pass123$"
        },
        new() {
            User = new ApplicationUser() { UserName = "bob", Email = "BobSmith@email.com", EmailConfirmed = true },
            Claims = new Claim[] {
                new(JwtClaimTypes.Name, "Bob Smith"), new(JwtClaimTypes.GivenName, "Bob"), new(JwtClaimTypes.FamilyName, "Smith"),
                new(JwtClaimTypes.WebSite, "http://bob.com"), new("location", "somewhere")
            },
            Password = "Pass123$"
        }
    };
}