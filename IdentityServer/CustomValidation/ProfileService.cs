using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using IdentityModel;

using System.Security.Claims;

namespace IdentityServer.CustomValidation;

public class ProfileService : IProfileService {
    public Task GetProfileDataAsync(ProfileDataRequestContext context) {
        // add actor claim if needed
        if (context.Subject.GetAuthenticationMethod() != OidcConstants.GrantTypes.TokenExchange) {
            return Task.CompletedTask;
        }

        Claim act = context.Subject.FindFirst(JwtClaimTypes.Actor);
        if (act != null) {
            context.IssuedClaims.Add(act);
        }

        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context) {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}