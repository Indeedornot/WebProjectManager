using Duende.IdentityServer.Models;

namespace shared.Common;

public static class IdentityResources
{
    public static IdentityResource Avatar =>
        new() { Name = "avatar", DisplayName = "Avatar", Description = "User's Avatar", UserClaims = { Claims.AvatarClaim } };
}
