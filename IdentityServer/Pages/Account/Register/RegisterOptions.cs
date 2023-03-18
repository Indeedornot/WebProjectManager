namespace IdentityServer.Pages.Account.Register;

public class RegisterOptions {
    public static bool AllowLocalLogin = true;
    public static bool AllowRememberLogin = true;
    public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
    public static string UserAlreadyExistsErrorMessage = "User with this username or email already exists";
}