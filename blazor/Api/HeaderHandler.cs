using IdentityModel.Client;

using Microsoft.AspNetCore.Authentication;

using System.Diagnostics;

namespace blazor.Api;

public class HeaderHandler : DelegatingHandler {
    private readonly IHttpContextAccessor _contextAccessor;

    public HeaderHandler(IHttpContextAccessor contextAccessor) {
        _contextAccessor = contextAccessor;
    }

    public HeaderHandler(
        HttpMessageHandler innerHandler,
        IHttpContextAccessor contextAccessor) : base(innerHandler) {
        _contextAccessor = contextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        string? token = await _contextAccessor.HttpContext!.GetTokenAsync("access_token");
        request.SetBearerToken(token);
        Debug.Write("Access Token: " + token);
        return await base.SendAsync(request, cancellationToken);
    }
}