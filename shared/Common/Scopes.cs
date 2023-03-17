using Duende.IdentityServer.Models;

namespace shared.Common;

public static class Scopes {
    public const string ProjectPolicyName = "ProjectPolicy";
    public static readonly ApiScope ProjectScope = new("ProjectApi", "Project Api");

    public const string UserDataPolicyName = "UserDataPolicy";
    public static readonly ApiScope UserDataScope = new("UserDataApi", "User Data Api");
}