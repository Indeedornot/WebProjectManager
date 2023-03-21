using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using IdentityModel;

using IdentityServer.Models;

using Microsoft.AspNetCore.Identity;

using shared.Common;

using System.Security.Claims;

using IdentityResources = shared.Common.IdentityResources;

namespace IdentityServer.Extensions.CustomValidation;

public class CustomProfileService : ProfileService<ApplicationUser> {
    public CustomProfileService(UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory) : base(userManager, claimsFactory) {
    }

    protected override async Task GetProfileDataAsync(ProfileDataRequestContext context, ApplicationUser user) {
        // add actor claim if needed
        if (context.Subject.GetAuthenticationMethod() == OidcConstants.GrantTypes.TokenExchange) {
            await HandleTokenExchange(context);
            return;
        }

        await AddRequestedClaims(context, user);
    }

    private async Task AddRequestedClaims(ProfileDataRequestContext context, ApplicationUser user) {
        ClaimsPrincipal principal = await GetUserClaimsAsync(user);
        var id = (ClaimsIdentity)principal.Identity;
        id.AddClaim(new Claim(Claims.AvatarClaim, user.Avatar.ToString()));
        context.AddRequestedClaims(principal.Claims);
    }

    private static Task HandleTokenExchange(ProfileDataRequestContext context) {
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