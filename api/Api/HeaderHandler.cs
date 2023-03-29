using System.Diagnostics;
using System.Net;

using IdentityModel;
using IdentityModel.Client;

using Microsoft.AspNetCore.Authentication;

using shared.Common;

namespace api.Api;

public class HeaderHandler : DelegatingHandler
{
    private static DiscoveryCache DiscoveryCache { get; } = new(IPs.IdentityServer);
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly HttpClient _httpClient;

    public HeaderHandler(IHttpContextAccessor contextAccessor, HttpClient httpClient)
    {
        _contextAccessor = contextAccessor;
        _httpClient = httpClient;
    }

    public HeaderHandler(HttpMessageHandler innerHandler, IHttpContextAccessor contextAccessor, HttpClient httpClient) : base(innerHandler)
    {
        _contextAccessor = contextAccessor;
        _httpClient = httpClient;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token = await _contextAccessor.HttpContext!.GetTokenAsync("access_token");
        if (token is null)
        {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        string? delegatedToken = await DelegateToken(token);
        if (delegatedToken is null)
        {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        request.SetBearerToken(delegatedToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string?> DelegateToken(string token)
    {
        DiscoveryDocumentResponse? disco = await DiscoveryCache.GetAsync();
        if (disco.IsError)
        {
            return null;
        }

        TokenResponse? response = await _httpClient.RequestTokenExchangeTokenAsync(new TokenExchangeTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "api",
            ClientSecret = "secret",
            SubjectToken = token,
            SubjectTokenType = OidcConstants.TokenTypeIdentifiers.AccessToken,
            Scope = Scopes.UserDataScope.Name,
            Parameters = { { "exchange_style", "delegation" } }
        });

        Console.WriteLine("Access Token: " + response.AccessToken);

        return response.IsError ? null : response.AccessToken;
    }
}
