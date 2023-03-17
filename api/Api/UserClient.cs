using shared.Common;

namespace api.Api;

public class UserClient {
    private readonly HttpClient _httpClient;

    public UserClient(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<string?> GetHello() {
        return await _httpClient.GetStringAsync($"{IPs.IdentityServer}/api/Hello");
    }
}