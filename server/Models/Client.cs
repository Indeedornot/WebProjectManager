namespace server.Models;

public class Client {
    public string ClientName { get; set; }
    public string ClientId { get; set; }

    /// <summary>
    /// Client Password
    /// </summary>
    public string ClientSecret { get; set; }

    public IList<string> GrantType { get; set; }

    /// <summary>
    /// False by default
    /// </summary>
    public bool IsActive { get; set; } = false;

    public IList<string> AllowedScopes { get; set; }

    public string ClientUri { get; set; }
    public string RedirectUri { get; set; }
}