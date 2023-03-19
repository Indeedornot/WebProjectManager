// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public string Avatar { get; set; }

    public ApplicationUser() {
    }

    public ApplicationUser(string userName, string email, bool emailConfirmed) {
        UserName = userName;
        Email = email;
        EmailConfirmed = emailConfirmed;
    }
}